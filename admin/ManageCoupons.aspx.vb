Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

' Admin › Coupon codes – list with usage stats, add/edit in one form, pause/delete/restore.
Public Class ManageCoupons
    Inherits AdminPage

    Private Shared ReadOnly INR As New CultureInfo("en-IN")

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
            Using da As New SqlDataAdapter("SELECT Id, Name, DateCreated, IsPercent, Value, Status FROM CouponCode ORDER BY CASE WHEN Status = 2 THEN 1 ELSE 0 END, DateCreated DESC, Id DESC", con)
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
            If Convert.ToInt32(r("Status")) = 0 Then active += 1
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
    Protected Shared Function OffText(isPercent As Object, value As Object) As String
        Dim v As Decimal = Convert.ToDecimal(value)
        If Convert.ToBoolean(isPercent) Then Return v.ToString("0.##", INR) & "% off"
        Return "₹" & v.ToString("#,##0.##", INR) & " off"
    End Function

    Protected Shared Function StatusText(o As Object) As String
        Select Case Convert.ToString(o)
            Case "0" : Return "Active"
            Case "1" : Return "Inactive"
            Case "2" : Return "Deleted"
            Case Else : Return Convert.ToString(o)
        End Select
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
    End Sub

    Private Sub LoadIntoForm(id As Integer)
        Using con As New SqlConnection(ConnStr), cmd As New SqlCommand("SELECT Name, IsPercent, Value, Status FROM CouponCode WHERE Id = @Id", con)
            cmd.Parameters.AddWithValue("@Id", id)
            con.Open()
            Using r = cmd.ExecuteReader()
                If Not r.Read() Then Return
                EditIdHidden.Value = id.ToString()
                CouponCodeTextBox.Text = Convert.ToString(r("Name"))
                TypeDropDown.SelectedValue = If(Convert.ToBoolean(r("IsPercent")), "1", "0")
                ValueTextBox.Text = Convert.ToDecimal(r("Value")).ToString("0.##", CultureInfo.InvariantCulture)
                StatusDropDown.SelectedValue = Convert.ToString(r("Status"))
            End Using
        End Using
    End Sub

    Private Sub ResetForm()
        EditIdHidden.Value = "0"
        CouponCodeTextBox.Text = ""
        ValueTextBox.Text = ""
        TypeDropDown.SelectedValue = "0"
        StatusDropDown.SelectedValue = "0"
    End Sub

    Protected Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click
        ResetForm()
    End Sub

    ' ------------------------------------------------------------------
    '  Save (create or update)
    ' ------------------------------------------------------------------
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
                "INSERT INTO CouponCode (Name, DateCreated, IsPercent, Value, Status) VALUES (@Name, GETUTCDATE(), @IsPercent, @Value, @Status)",
                "UPDATE CouponCode SET Name = @Name, IsPercent = @IsPercent, Value = @Value, Status = @Status WHERE Id = @Id")
            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@Name", code)
                cmd.Parameters.AddWithValue("@IsPercent", isPercent)
                cmd.Parameters.AddWithValue("@Value", value)
                cmd.Parameters.AddWithValue("@Status", status)
                cmd.Parameters.AddWithValue("@Id", id)
                cmd.ExecuteNonQuery()
            End Using
        End Using

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