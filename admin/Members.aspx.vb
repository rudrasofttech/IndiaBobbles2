Imports System.Data
Imports System.Linq
Imports System.Data.SqlClient
Imports System.Text
Imports System.Text.RegularExpressions
Imports IndiaBobbles

' Admin › Members – stats, search/filters, spam detection, bulk remove/unsubscribe, CSV export.
' Assumes the members table is [Member] with the columns the old grid showed.
Public Class Members
    Inherits AdminPage

    Private _data As DataTable

    Private ReadOnly Property ConnStr As String
        Get
            Return ConfigurationManager.ConnectionStrings("indiabobblesConnectionString").ConnectionString
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            If Not String.IsNullOrEmpty(Request.QueryString("q")) Then FilterTextBox.Text = Request.QueryString("q").Trim()
            StatusDropDown.SelectedValue = "0"      ' active members by default
            BindGrid()
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        LoadStats()
    End Sub

    ' ------------------------------------------------------------------
    '  Data
    ' ------------------------------------------------------------------
    Private Function LoadMembers() As DataTable
        If _data IsNot Nothing Then Return _data

        Dim q As String = FilterTextBox.Text.Trim()
        Dim sql As String =
            "SELECT M.ID, M.Email, M.Mobile, M.Createdate, M.Newsletter, M.UserType, M.MemberName, M.Status, ISNULL(OC.N, 0) AS Orders " &
            "FROM Member AS M LEFT JOIN (SELECT MemberID, COUNT(*) AS N FROM [Order] WHERE Status IN (3,4,5,6) GROUP BY MemberID) AS OC ON OC.MemberID = M.ID " &
            "WHERE (@q = '' OR M.Email LIKE @like OR M.MemberName LIKE @like OR M.Mobile LIKE @like) " &
            "AND (@st = '' OR CAST(M.Status AS varchar(10)) = @st) " &
            "AND (@nl = '' OR CAST(M.Newsletter AS varchar(5)) = @nl) " &
            "ORDER BY M.Createdate DESC, M.ID DESC"

        Dim dt As New DataTable()
        Using con As New SqlConnection(ConnStr), da As New SqlDataAdapter(sql, con)
            da.SelectCommand.Parameters.AddWithValue("@q", q)
            da.SelectCommand.Parameters.AddWithValue("@like", "%" & q & "%")
            da.SelectCommand.Parameters.AddWithValue("@st", StatusDropDown.SelectedValue)
            da.SelectCommand.Parameters.AddWithValue("@nl", SubscribeList.SelectedValue)
            da.Fill(dt)
        End Using

        dt.Columns.Add("Spam", GetType(Boolean))
        dt.Columns.Add("NewsletterOn", GetType(Boolean))
        For Each r As DataRow In dt.Rows
            r("NewsletterOn") = Not IsDBNull(r("Newsletter")) AndAlso Convert.ToBoolean(r("Newsletter"))
            r("Spam") = IsLikelySpam(Convert.ToString(r("Email")), Convert.ToString(r("MemberName")), Convert.ToInt32(r("Orders"))) AndAlso Not IsAdminType(r("UserType"))
        Next

        ' Type filter (done here because spam detection isn't SQL)
        Dim kind As String = KindDropDown.SelectedValue
        If kind <> "" Then
            Dim filtered = dt.Clone()
            For Each r As DataRow In dt.Rows
                Dim orders As Integer = Convert.ToInt32(r("Orders"))
                Dim keep As Boolean = (kind = "buyers" AndAlso orders > 0) OrElse
                                      (kind = "none" AndAlso orders = 0) OrElse
                                      (kind = "spam" AndAlso CBool(r("Spam")))
                If keep Then filtered.ImportRow(r)
            Next
            dt = filtered
        End If

        _data = dt
        Return dt
    End Function

    Private Sub BindGrid()
        Dim dt = LoadMembers()
        MemberGridView.DataSource = dt
        MemberGridView.DataBind()
        FilteredCountLiteral.Text = dt.Rows.Count.ToString("#,##0")
        DeleteFilteredButton.OnClientClick =
            "return confirm('Remove all " & dt.Rows.Count & " members in this view? Admins and members with paid orders are skipped. They will be marked Deleted.');"
        DeleteFilteredButton.Enabled = dt.Rows.Count > 0
    End Sub

    Private Sub LoadStats()
        Const sql As String =
            "SELECT SUM(CASE WHEN Status = 0 THEN 1 ELSE 0 END) AS Total," &
            " SUM(CASE WHEN Status = 0 AND Newsletter = 1 THEN 1 ELSE 0 END) AS Subs," &
            " SUM(CASE WHEN Status = 0 AND Createdate >= DATEADD(day, -30, GETDATE()) THEN 1 ELSE 0 END) AS New30," &
            " (SELECT COUNT(DISTINCT MemberID) FROM [Order] WHERE Status IN (3,4,5,6) AND MemberID > 0) AS Buyers " &
            "FROM Member"
        Try
            Using con As New SqlConnection(ConnStr), cmd As New SqlCommand(sql, con)
                con.Open()
                Using r = cmd.ExecuteReader()
                    If r.Read() Then
                        TotalLiteral.Text = ToInt(r("Total")).ToString("#,##0")
                        SubscribedLiteral.Text = ToInt(r("Subs")).ToString("#,##0")
                        NewLiteral.Text = ToInt(r("New30")).ToString("#,##0")
                        BuyersLiteral.Text = ToInt(r("Buyers")).ToString("#,##0")
                    End If
                End Using
            End Using
        Catch ex As Exception
        End Try
    End Sub

    Private Shared Function ToInt(o As Object) As Integer
        Return If(o Is Nothing OrElse IsDBNull(o), 0, Convert.ToInt32(o))
    End Function

    ' ------------------------------------------------------------------
    '  Helpers used by the markup
    ' ------------------------------------------------------------------
    Protected Shared Function StatusText(o As Object) As String
        Select Case Convert.ToString(o)
            Case "0" : Return "Active"
            Case "1" : Return "Inactive"
            Case "2" : Return "Deleted"
            Case Else : Return Convert.ToString(o)
        End Select
    End Function

    Protected Shared Function Initial(name As Object, email As Object) As String
        Dim s As String = Convert.ToString(name).Trim()
        If s = "" Then s = Convert.ToString(email).Trim()
        Return If(s = "", "?", s.Substring(0, 1))
    End Function

    ''' <summary>True if UserType is the Admin member type (works whether it's stored as a number or as text).</summary>
    Protected Shared Function IsAdminType(o As Object) As Boolean
        Dim s As String = Convert.ToString(o).Trim()
        If s = "" Then Return False
        Return s = CInt(MemberTypeType.Admin).ToString() OrElse s.Equals(MemberTypeType.Admin.ToString(), StringComparison.OrdinalIgnoreCase)
    End Function

    Private Shared ReadOnly SpamDomains As String() = {
        "yandex.", "mail.ru", "bk.ru", "list.ru", "inbox.ru", "rambler.ru", "o5o5.ru", "meta.ua", "ukr.net", "tut.by",
        "emersets.com", "avalins.com", "elfastes.com", "bambo-mebel", "course-fitness", "mailbox.in.ua", "wp.pl", "o2.pl"}

    ''' <summary>
    ''' Rough spam check for sign-ups with no paid orders:
    ''' Russian/throwaway domains, Gmail "dot trick" addresses (a.b.c.d@gmail), .ru/.ua emails,
    ''' or bot-style names (links, "...escah", random letters).
    ''' </summary>
    Protected Shared Function IsLikelySpam(email As String, name As String, orders As Integer) As Boolean
        If orders > 0 Then Return False
        Dim em As String = If(email, "").Trim().ToLowerInvariant()
        Dim nm As String = If(name, "").Trim().ToLowerInvariant()
        If em = "" Then Return False

        Dim at As Integer = em.LastIndexOf("@"c)
        Dim local As String = If(at > 0, em.Substring(0, at), em)
        Dim domain As String = If(at > 0, em.Substring(at + 1), "")

        If SpamDomains.Any(Function(d) domain.Contains(d)) Then Return True
        If domain.EndsWith(".ru") OrElse domain.EndsWith(".ua") OrElse domain.EndsWith(".by") OrElse domain.EndsWith(".top") Then Return True
        If local.Count(Function(c) c = "."c) >= 3 Then Return True                     ' j.e.n.t.a.l.w.o.r.l.d@
        If nm.Contains("http") OrElse nm.Contains("://") OrElse nm.Contains("www.") Then Return True
        If Regex.IsMatch(nm, "(escah|ewiz|ffeli)$") Then Return True                   ' common bot name endings
        If nm.Length >= 12 AndAlso Not nm.Contains(" ") AndAlso Regex.IsMatch(nm, "[bcdfghjklmnpqrstvwxz]{5,}") Then Return True
        Return False
    End Function

    ' ------------------------------------------------------------------
    '  Grid events
    ' ------------------------------------------------------------------
    Protected Sub MemberGridView_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles MemberGridView.PageIndexChanging
        MemberGridView.PageIndex = e.NewPageIndex
        BindGrid()
    End Sub

    Protected Sub MemberGridView_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles MemberGridView.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow AndAlso CBool(CType(e.Row.DataItem, DataRowView)("Spam")) Then
            e.Row.CssClass = "spam"
        End If
    End Sub

    Protected Sub MemberGridView_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles MemberGridView.RowCommand
        If e.CommandName <> "Remove" Then Return
        Dim id As Integer
        If Integer.TryParse(Convert.ToString(e.CommandArgument), id) Then
            Dim n = SetDeleted(New List(Of Integer) From {id}, skipProtected:=False)
            Flash(If(n > 0, "Member removed.", "Nothing changed."))
        End If
        BindGrid()
    End Sub

    ' ------------------------------------------------------------------
    '  Toolbar + bulk actions
    ' ------------------------------------------------------------------
    Protected Sub SubmitButton_Click(sender As Object, e As EventArgs) Handles SubmitButton.Click
        MemberGridView.PageIndex = 0
        BindGrid()
    End Sub

    Protected Sub ResetButton_Click(sender As Object, e As EventArgs) Handles ResetButton.Click
        FilterTextBox.Text = ""
        StatusDropDown.SelectedValue = "0"
        SubscribeList.SelectedValue = ""
        KindDropDown.SelectedValue = ""
        MemberGridView.PageIndex = 0
        BindGrid()
    End Sub

    Private Function SelectedIds() As List(Of Integer)
        Dim ids As New List(Of Integer)
        For Each row As GridViewRow In MemberGridView.Rows
            Dim cb = CType(row.FindControl("cbSelect"), CheckBox)
            If cb IsNot Nothing AndAlso cb.Checked Then ids.Add(CInt(MemberGridView.DataKeys(row.RowIndex).Value))
        Next
        Return ids
    End Function

    Protected Sub DeleteButton_Click(sender As Object, e As EventArgs) Handles DeleteButton.Click
        Dim ids = SelectedIds()
        Dim n = SetDeleted(ids, skipProtected:=False)
        Flash(n & " member" & If(n = 1, "", "s") & " removed.")
        BindGrid()
    End Sub

    Protected Sub DeleteFilteredButton_Click(sender As Object, e As EventArgs) Handles DeleteFilteredButton.Click
        Dim ids As New List(Of Integer)
        For Each r As DataRow In LoadMembers().Rows
            ids.Add(Convert.ToInt32(r("ID")))
        Next
        Dim n = SetDeleted(ids, skipProtected:=True)
        Flash(n & " member" & If(n = 1, "", "s") & " removed. Admins and buyers were kept.")
        _data = Nothing
        MemberGridView.PageIndex = 0
        BindGrid()
    End Sub

    Protected Sub UnsubscribeButton_Click(sender As Object, e As EventArgs) Handles UnsubscribeButton.Click
        Dim ids = SelectedIds()
        Dim n = RunForIds("UPDATE Member SET Newsletter = 0 WHERE ID IN ({0})", ids)
        Flash(n & " member" & If(n = 1, "", "s") & " unsubscribed from the newsletter.")
        BindGrid()
    End Sub

    ''' <summary>Soft delete (Status = 2). Bulk "remove all" never touches admins or members with paid orders.</summary>
    Private Function SetDeleted(ids As List(Of Integer), skipProtected As Boolean) As Integer
        Dim sql As String = "UPDATE Member SET Status = 2 WHERE ID IN ({0}) AND Status <> 2"
        If skipProtected Then
            sql &= " AND ID NOT IN (SELECT MemberID FROM [Order] WHERE Status IN (3,4,5,6) AND MemberID IS NOT NULL)" &
                   " AND CAST(UserType AS varchar(20)) NOT IN ('" & CInt(MemberTypeType.Admin) & "', '" & MemberTypeType.Admin.ToString() & "')"
        End If
        Return RunForIds(sql, ids)
    End Function

    ''' <summary>Runs an UPDATE for a list of IDs in batches of 500 (IDs are integers, so they're safe to inline).</summary>
    Private Function RunForIds(sqlTemplate As String, ids As List(Of Integer)) As Integer
        If ids Is Nothing OrElse ids.Count = 0 Then Return 0
        Dim total As Integer = 0
        Using con As New SqlConnection(ConnStr)
            con.Open()
            For i As Integer = 0 To ids.Count - 1 Step 500
                Dim batch = String.Join(",", ids.Skip(i).Take(500))
                Using cmd As New SqlCommand(String.Format(sqlTemplate, batch), con)
                    cmd.CommandTimeout = 120
                    total += cmd.ExecuteNonQuery()
                End Using
            Next
        End Using
        _data = Nothing
        Return total
    End Function

    ''' <summary>Downloads every member in the current view as CSV (for Excel or an email tool).</summary>
    Protected Sub ExportButton_Click(sender As Object, e As EventArgs) Handles ExportButton.Click
        Dim csv As New StringBuilder("ID,Name,Email,Mobile,Joined,Newsletter,Paid orders,Status,Looks like spam" & vbCrLf)
        For Each r As DataRow In LoadMembers().Rows
            csv.AppendLine(String.Join(",", {
                Q(r("ID")), Q(r("MemberName")), Q(r("Email")), Q(r("Mobile")),
                Q(Convert.ToDateTime(r("Createdate")).ToString("yyyy-MM-dd")),
                Q(If(CBool(r("NewsletterOn")), "Yes", "No")), Q(r("Orders")),
                Q(StatusText(r("Status"))), Q(If(CBool(r("Spam")), "Yes", "No"))}))
        Next
        Response.Clear()
        Response.ContentType = "text/csv"
        Response.ContentEncoding = Encoding.UTF8
        Response.AddHeader("Content-Disposition", "attachment; filename=members-" & DateTime.Now.ToString("yyyyMMdd-HHmm") & ".csv")
        Response.BinaryWrite(Encoding.UTF8.GetPreamble())
        Response.Write(csv.ToString())
        Response.Flush()
        Response.SuppressContent = True
        HttpContext.Current.ApplicationInstance.CompleteRequest()
    End Sub

    Private Shared Function Q(o As Object) As String
        Dim s As String = If(o Is Nothing OrElse IsDBNull(o), "", Convert.ToString(o))
        If s.Length > 0 AndAlso "=+-@".IndexOf(s(0)) >= 0 Then s = "'" & s
        Return """" & s.Replace("""", """""") & """"
    End Function

    Private Sub Flash(msg As String)
        MessageLabel.Text = "<i class=""fa fa-check""></i> " & Server.HtmlEncode(msg)
        MessageLabel.Visible = True
    End Sub
End Class