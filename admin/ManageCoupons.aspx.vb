Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

' Admin › Coupon codes – list with usage stats, add/edit in one form (incl. rules), pause/delete/restore.
Public Class ManageCoupons
    Inherits AdminPage

    Private Shared ReadOnly INR As New CultureInfo("en-IN")
    Private Const ColList As String = "Id, Name, DateCreated, IsPercent, Value, Status, MinItems, MinAmount, FirstOrderOnly, ReturningOnly, ShowOnCart, Pitch, ValidFrom, ValidTo"

    ' Coupon usage from paid / shipped / completed orders, keyed by coupon code (and by ID, in case orders store the ID)
    Private _usage As New Dictionary(Of String, Tuple(Of Integer, Decimal))(StringComparer.OrdinalIgnoreCase)

    Private ReadOnly Property ConnStr As String
        Get
            Return ConfigurationManager.ConnectionStrings("indiabobblesConnectionString").ConnectionString
        End Get
    End Property

    ''' <summary>ID of the coupon in the form (0 = creating a new one).</summary>
    Protected ReadOnly Property EditingId As Integer
        Get
            Dim id As Integer
            Integer.TryParse(EditIdHidden.Value, id)
            Return id
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If ForbidUserAccess(MemberTypeType.Admin) Then
            Response.Redirect("default.aspx")
        End If
        If Not IsPostBack AndAlso Session("CouponFlash") IsNot Nothing Then
            Flash(CStr(Session("CouponFlash")))
            Session.Remove("CouponFlash")
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        BindList()
        Dim editing As Boolean = EditingId > 0
        FormTitleLiteral.Text = If(editing, "Edit coupon", "New coupon")
        SaveButton.Text = If(editing, "Save changes", "Create coupon")
        CancelButton.Visible = editing
        EditingNote.Visible = editing
    End Sub

    ' ------------------------------------------------------------------
    '  List + stats
    ' ------------------------------------------------------------------
    Private Sub BindList()
        Dim dt As New DataTable()
        Using con As New SqlConnection(ConnStr)
            Using da As New SqlDataAdapter("SELECT " & ColList & " FROM CouponCode ORDER BY CASE WHEN Status = 2 THEN 1 ELSE 0 END, DateCreated DESC, Id DESC", con)
                da.Fill(dt)
            End Using

            ' Usage is a nice-to-have: never break the page if the Order table differs
            Try
                Using cmd As New SqlCommand("SELECT CAST(Coupon AS nvarchar(200)) AS Coupon, COUNT(*) AS N, ISNULL(SUM(Total),0) AS Sales FROM [Order] WHERE Status IN (3,4,5,6) AND Coupon IS NOT NULL GROUP BY CAST(Coupon AS nvarchar(200))", con)
                    con.Open()
                    Using r = cmd.ExecuteReader()
                        While r.Read()
                            Dim key As String = Convert.ToString(r("Coupon")).Trim()
                            If key <> "" AndAlso Not _usage.ContainsKey(key) Then
                                _usage(key) = Tuple.Create(Convert.ToInt32(r("N")), Convert.ToDecimal(r("Sales")))
                            End If
                        End While
                    End Using
                End Using
            Catch ex As Exception
            End Try
        End Using

        CouponRepeater.DataSource = dt
        CouponRepeater.DataBind()

        ' Stats
        Dim active As Integer = 0
        For Each r As DataRow In dt.Rows
            If Convert.ToInt32(r("Status")) = 0 AndAlso Not IsExpired(r("ValidTo")) Then active += 1
        Next
        Dim used As Integer = 0, sales As Decimal = 0, topName As String = "–", topN As Integer = 0
        For Each r As DataRow In dt.Rows
            Dim u = GetUsage(r("Id"), r("Name"))
            If u Is Nothing Then Continue For
            used += u.Item1 : sales += u.Item2
            If u.Item1 > topN Then topN = u.Item1 : topName = Convert.ToString(r("Name"))
        Next
        ActiveCountLiteral.Text = active.ToString()
        UsedCountLiteral.Text = used.ToString("#,##0")
        RevenueLiteral.Text = "₹" & sales.ToString("#,##0", INR)
        TopLiteral.Text = If(topN > 0, Server.HtmlEncode(topName) & " <span style=""font-size:.8rem;color:#6b5f72;font-weight:600;"">(" & topN & ")</span>", "–")
    End Sub

    Private Function GetUsage(id As Object, name As Object) As Tuple(Of Integer, Decimal)
        Dim u As Tuple(Of Integer, Decimal) = Nothing
        If _usage.TryGetValue(Convert.ToString(name).Trim(), u) Then Return u
        If _usage.TryGetValue(Convert.ToString(id), u) Then Return u
        Return Nothing
    End Function

    ' ------------------------------------------------------------------
    '  Helpers used in the markup
    ' ------------------------------------------------------------------
    Private Shared Function NDate(o As Object) As Date?
        If o Is Nothing OrElse IsDBNull(o) Then Return Nothing
        Return Convert.ToDateTime(o)
    End Function

    Private Shared Function IsExpired(validTo As Object) As Boolean
        Dim d As Date? = NDate(validTo)
        Return d.HasValue AndAlso d.Value < DateTime.Now
    End Function

    Protected Shared Function OffText(isPercent As Object, value As Object) As String
        Dim v As Decimal = Convert.ToDecimal(value)
        If Convert.ToBoolean(isPercent) Then Return v.ToString("0.##", INR) & "% off"
        Return "₹" & v.ToString("#,##0.##", INR) & " off"
    End Function

    ''' <summary>Status badge; an active coupon outside its dates shows as Expired / Scheduled.</summary>
    Protected Shared Function StatusHtml(item As Object) As String
        Dim r As DataRowView = CType(item, DataRowView)
        Dim s As Integer = Convert.ToInt32(r("Status"))
        If s = 0 Then
            Dim fromD As Date? = NDate(r("ValidFrom"))
            Dim toD As Date? = NDate(r("ValidTo"))
            If toD.HasValue AndAlso toD.Value < DateTime.Now Then Return "<span class=""cs cs-x"">Expired</span>"
            If fromD.HasValue AndAlso fromD.Value > DateTime.Now Then Return "<span class=""cs cs-s"">Scheduled</span>"
            Return "<span class=""cs cs-0"">Active</span>"
        End If
        If s = 1 Then Return "<span class=""cs cs-1"">Inactive</span>"
        If s = 2 Then Return "<span class=""cs cs-2"">Deleted</span>"
        Return "<span class=""cs"">" & s & "</span>"
    End Function

    ''' <summary>Small chips describing who can use the coupon and when.</summary>
    Protected Shared Function RulesHtml(item As Object) As String
        Dim r As DataRowView = CType(item, DataRowView)
        Dim chips As New List(Of String)
        If Convert.ToBoolean(r("ShowOnCart")) Then chips.Add("<span class=""rc on""><i class=""fa fa-shopping-cart""></i> On cart</span>")
        If Convert.ToBoolean(r("FirstOrderOnly")) Then chips.Add("<span class=""rc who"">First order</span>")
        If Convert.ToBoolean(r("ReturningOnly")) Then chips.Add("<span class=""rc who"">Past buyers</span>")
        Dim mi As Integer = Convert.ToInt32(r("MinItems"))
        If mi > 1 Then chips.Add("<span class=""rc"">" & mi & "+ items</span>")
        Dim ma As Decimal = Convert.ToDecimal(r("MinAmount"))
        If ma > 0 Then chips.Add("<span class=""rc"">₹" & ma.ToString("#,##0", INR) & "+</span>")
        Dim fromD As Date? = NDate(r("ValidFrom"))
        Dim toD As Date? = NDate(r("ValidTo"))
        If fromD.HasValue AndAlso fromD.Value > DateTime.Now Then chips.Add("<span class=""rc dt"">From " & fromD.Value.ToString("d MMM", INR) & "</span>")
        If toD.HasValue Then chips.Add("<span class=""rc dt" & If(toD.Value < DateTime.Now, " old", "") & """>Till " & toD.Value.ToString("d MMM yyyy", INR) & "</span>")
        If chips.Count = 0 Then Return "<span class=""sm"">Anyone, any cart</span>"
        Return String.Join(" ", chips)
    End Function

    Protected Function UsageHtml(id As Object, name As Object) As String
        Dim u = GetUsage(id, name)
        If u Is Nothing Then Return "<span class=""sm"">Not used yet</span>"
        Return "<b>" & u.Item1 & "</b> order" & If(u.Item1 = 1, "", "s") &
               "<div class=""sm"">₹" & u.Item2.ToString("#,##0", INR) & " sales</div>"
    End Function

    ' ------------------------------------------------------------------
    '  Row actions
    ' ------------------------------------------------------------------
    Protected Sub CouponRepeater_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles CouponRepeater.ItemCommand
        Dim id As Integer
        If Not Integer.TryParse(Convert.ToString(e.CommandArgument), id) Then Return

        Select Case e.CommandName
            Case "EditCoupon"
                LoadIntoForm(id)
            Case "Toggle"
                Exec("UPDATE CouponCode SET Status = CASE WHEN Status = 0 THEN 1 ELSE 0 END WHERE Id = @Id AND Status IN (0,1)", id)
                Flash("Coupon status updated.")
            Case "DeleteCoupon"
                Exec("UPDATE CouponCode SET Status = 2 WHERE Id = @Id", id)   ' soft delete, as before
                If EditingId = id Then ResetForm()
                Flash("Coupon deleted. You can restore it from the Deleted tab.")
            Case "Restore"
                Exec("UPDATE CouponCode SET Status = 1 WHERE Id = @Id", id)
                Flash("Coupon restored as inactive. Activate it when you're ready.")
        End Select
        CouponCode.ClearCache()      ' cart/checkout pick up the change straight away
    End Sub

    Private Sub LoadIntoForm(id As Integer)
        Using con As New SqlConnection(ConnStr), cmd As New SqlCommand("SELECT " & ColList & " FROM CouponCode WHERE Id = @Id", con)
            cmd.Parameters.AddWithValue("@Id", id)
            con.Open()
            Using r = cmd.ExecuteReader()
                If Not r.Read() Then Return
                EditIdHidden.Value = id.ToString()
                CouponCodeTextBox.Text = Convert.ToString(r("Name"))
                TypeDropDown.SelectedValue = If(Convert.ToBoolean(r("IsPercent")), "1", "0")
                ValueTextBox.Text = Convert.ToDecimal(r("Value")).ToString("0.##", CultureInfo.InvariantCulture)
                StatusDropDown.SelectedValue = Convert.ToString(r("Status"))

                WhoDropDown.SelectedValue = If(Convert.ToBoolean(r("FirstOrderOnly")), "1", If(Convert.ToBoolean(r("ReturningOnly")), "2", "0"))
                Dim mi As Integer = Convert.ToInt32(r("MinItems"))
                MinItemsTextBox.Text = If(mi > 1, mi.ToString(), "")
                Dim ma As Decimal = Convert.ToDecimal(r("MinAmount"))
                MinAmountTextBox.Text = If(ma > 0, ma.ToString("0.##", CultureInfo.InvariantCulture), "")
                ShowOnCartCheckBox.Checked = Convert.ToBoolean(r("ShowOnCart"))
                PitchTextBox.Text = If(IsDBNull(r("Pitch")), "", Convert.ToString(r("Pitch")))
                Dim fromD As Date? = NDate(r("ValidFrom"))
                Dim toD As Date? = NDate(r("ValidTo"))
                ValidFromTextBox.Text = If(fromD.HasValue, fromD.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), "")
                ValidToTextBox.Text = If(toD.HasValue, toD.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), "")
            End Using
        End Using
    End Sub

    Private Sub ResetForm()
        EditIdHidden.Value = "0"
        CouponCodeTextBox.Text = ""
        ValueTextBox.Text = ""
        TypeDropDown.SelectedValue = "0"
        StatusDropDown.SelectedValue = "0"
        WhoDropDown.SelectedValue = "0"
        MinItemsTextBox.Text = ""
        MinAmountTextBox.Text = ""
        ShowOnCartCheckBox.Checked = False
        PitchTextBox.Text = ""
        ValidFromTextBox.Text = ""
        ValidToTextBox.Text = ""
    End Sub

    Protected Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click
        ResetForm()
    End Sub

    ' ------------------------------------------------------------------
    '  Save (create or update)
    ' ------------------------------------------------------------------
    Private Shared Function ParseDate(s As String, ByRef d As Date?) As Boolean
        d = Nothing
        If String.IsNullOrWhiteSpace(s) Then Return True
        Dim v As Date
        If Date.TryParseExact(s.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, v) Then
            d = v
            Return True
        End If
        Return False
    End Function

    Private Sub ShowRuleError(msg As String)
        RulesErrorLabel.Text = msg
        RulesErrorLabel.Visible = True
    End Sub

    Protected Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Page.Validate("CouponGrp")
        If Not Page.IsValid Then Return

        Dim code As String = CouponCodeTextBox.Text.Trim().ToUpperInvariant()
        Dim isPercent As Boolean = TypeDropDown.SelectedValue = "1"
        Dim value As Decimal = Decimal.Parse(ValueTextBox.Text.Trim(), CultureInfo.InvariantCulture)
        Dim status As Byte = Byte.Parse(StatusDropDown.SelectedValue)
        Dim id As Integer = EditingId

        ' Checks the validators can't do
        If value <= 0 Then ValueErrorLabel.Text = "Value must be more than 0." : ValueErrorLabel.Visible = True : Return
        If isPercent AndAlso value > 100 Then ValueErrorLabel.Text = "A percent discount can't be more than 100%." : ValueErrorLabel.Visible = True : Return

        ' ---- Rules ----
        Dim who As String = WhoDropDown.SelectedValue
        Dim firstOnly As Boolean = (who = "1")
        Dim returningOnly As Boolean = (who = "2")

        Dim minItems As Integer = 0
        If MinItemsTextBox.Text.Trim() <> "" AndAlso (Not Integer.TryParse(MinItemsTextBox.Text.Trim(), minItems) OrElse minItems < 0 OrElse minItems > 50) Then
            ShowRuleError("Minimum bobbleheads must be a whole number between 0 and 50.") : Return
        End If

        Dim minAmount As Decimal = 0
        If MinAmountTextBox.Text.Trim() <> "" AndAlso (Not Decimal.TryParse(MinAmountTextBox.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, minAmount) OrElse minAmount < 0) Then
            ShowRuleError("Minimum cart value must be a number, e.g. 1500.") : Return
        End If
        If Not isPercent AndAlso minAmount > 0 AndAlso value >= minAmount Then
            ShowRuleError("A flat discount should be less than the minimum cart value.") : Return
        End If

        Dim validFrom As Date? = Nothing
        Dim validTo As Date? = Nothing
        If Not ParseDate(ValidFromTextBox.Text, validFrom) OrElse Not ParseDate(ValidToTextBox.Text, validTo) Then
            ShowRuleError("Please pick valid dates.") : Return
        End If
        If validTo.HasValue Then validTo = validTo.Value.Date.AddDays(1).AddSeconds(-1)     ' valid until the end of that day
        If validFrom.HasValue AndAlso validTo.HasValue AndAlso validTo.Value < validFrom.Value Then
            ShowRuleError("The end date is before the start date.") : Return
        End If

        Dim showOnCart As Boolean = ShowOnCartCheckBox.Checked
        Dim pitch As String = PitchTextBox.Text.Trim()
        If pitch.Length > 150 Then pitch = pitch.Substring(0, 150)

        Using con As New SqlConnection(ConnStr)
            con.Open()
            Using chk As New SqlCommand("SELECT COUNT(*) FROM CouponCode WHERE UPPER(LTRIM(RTRIM(Name))) = @Name AND Status <> 2 AND Id <> @Id", con)
                chk.Parameters.AddWithValue("@Name", code)
                chk.Parameters.AddWithValue("@Id", id)
                If CInt(chk.ExecuteScalar()) > 0 Then
                    CodeErrorLabel.Text = "A coupon with this code already exists."
                    CodeErrorLabel.Visible = True
                    Return
                End If
            End Using

            Dim sql As String = If(id = 0,
                "INSERT INTO CouponCode (Name, DateCreated, IsPercent, Value, Status, MinItems, MinAmount, FirstOrderOnly, ReturningOnly, ShowOnCart, Pitch, ValidFrom, ValidTo) " &
                "VALUES (@Name, GETUTCDATE(), @IsPercent, @Value, @Status, @MinItems, @MinAmount, @FirstOrderOnly, @ReturningOnly, @ShowOnCart, @Pitch, @ValidFrom, @ValidTo)",
                "UPDATE CouponCode SET Name = @Name, IsPercent = @IsPercent, Value = @Value, Status = @Status, " &
                "MinItems = @MinItems, MinAmount = @MinAmount, FirstOrderOnly = @FirstOrderOnly, ReturningOnly = @ReturningOnly, " &
                "ShowOnCart = @ShowOnCart, Pitch = @Pitch, ValidFrom = @ValidFrom, ValidTo = @ValidTo WHERE Id = @Id")
            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@Name", code)
                cmd.Parameters.AddWithValue("@IsPercent", isPercent)
                cmd.Parameters.AddWithValue("@Value", value)
                cmd.Parameters.AddWithValue("@Status", status)
                cmd.Parameters.AddWithValue("@MinItems", minItems)
                cmd.Parameters.AddWithValue("@MinAmount", minAmount)
                cmd.Parameters.AddWithValue("@FirstOrderOnly", firstOnly)
                cmd.Parameters.AddWithValue("@ReturningOnly", returningOnly)
                cmd.Parameters.AddWithValue("@ShowOnCart", showOnCart)
                cmd.Parameters.Add("@Pitch", SqlDbType.NVarChar, 150).Value = If(pitch = "", CObj(DBNull.Value), pitch)
                cmd.Parameters.Add("@ValidFrom", SqlDbType.DateTime).Value = If(validFrom.HasValue, CObj(validFrom.Value), DBNull.Value)
                cmd.Parameters.Add("@ValidTo", SqlDbType.DateTime).Value = If(validTo.HasValue, CObj(validTo.Value), DBNull.Value)
                cmd.Parameters.AddWithValue("@Id", id)
                cmd.ExecuteNonQuery()
            End Using
        End Using

        CouponCode.ClearCache()      ' cart/checkout pick up the change straight away

        ' Redirect after saving so a page refresh can't create the coupon twice
        Session("CouponFlash") = If(id = 0, "Coupon " & Server.HtmlEncode(code) & " created.", "Coupon " & Server.HtmlEncode(code) & " updated.")
        Response.Redirect("~/admin/managecoupons.aspx", False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub

    ' ------------------------------------------------------------------
    Private Sub Exec(sql As String, id As Integer)
        Using con As New SqlConnection(ConnStr), cmd As New SqlCommand(sql, con)
            cmd.Parameters.AddWithValue("@Id", id)
            con.Open()
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub Flash(msg As String)
        MessageLabel.Text = "<i class=""fa fa-check""></i> " & msg
        MessageLabel.Visible = True
    End Sub
End Class