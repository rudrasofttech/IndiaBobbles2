Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports IndiaBobbles

Public Class Emails
    Inherits AdminPage
    Private dc As New indiabobblesEntities()

    Private Sub Emails_Load(sender As Object, e As EventArgs) Handles Me.Load
        If ForbidUserAccess(MemberTypeType.Admin, MemberTypeType.Editor) Then
            Response.Redirect("default.aspx")
        End If

        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            ' Allow links like emails.aspx?group=Diwali2026
            Dim g As String = Request.QueryString("group")
            If Not String.IsNullOrEmpty(g) Then
                GroupDropDown.DataBind()
                If GroupDropDown.Items.FindByValue(g) IsNot Nothing Then GroupDropDown.SelectedValue = g
            End If
            Bind(0)
            BindCampaigns()
        End If
    End Sub

    ' ------------------------------------------------------------------
    '  Filtered query (shared by list, stats and paging)
    ' ------------------------------------------------------------------
    Private Function FilteredQuery() As IQueryable(Of EmailMessage)
        Dim query As IQueryable(Of EmailMessage) = dc.EmailMessages

        Dim kw As String = KeywordTextBox.Text.Trim()
        If kw <> "" Then
            query = query.Where(Function(m) m.ToAddress.Contains(kw) OrElse m.ToName.Contains(kw) OrElse m.Subject.Contains(kw) OrElse m.Message.Contains(kw))
        End If

        If TypeDropDown.SelectedValue <> "" Then
            Dim t As Byte = Byte.Parse(TypeDropDown.SelectedValue)
            query = query.Where(Function(m) m.EmailType = t)
        End If

        Dim grp As String = GroupDropDown.SelectedValue.Trim()
        If grp <> "" Then query = query.Where(Function(m) m.EmailGroup = grp)

        If SentDropDown.SelectedValue = "1" Then query = query.Where(Function(m) m.IsSent)
        If SentDropDown.SelectedValue = "0" Then query = query.Where(Function(m) Not m.IsSent)
        If ReadDropDown.SelectedValue = "1" Then query = query.Where(Function(m) m.IsRead)
        If ReadDropDown.SelectedValue = "0" Then query = query.Where(Function(m) Not m.IsRead)

        Return query
    End Function

    ''' <summary>Loads only one page from the database (the old version loaded every email into memory).</summary>
    Private Sub Bind(ByVal pageIndex As Integer)
        Try
            Dim query = FilteredQuery()
            Dim total As Integer = query.Count()
            Dim sent As Integer = query.Count(Function(m) m.IsSent)
            Dim opened As Integer = query.Count(Function(m) m.IsRead)

            Dim pageSize As Integer = EmailGrid.PageSize
            Dim lastPage As Integer = Math.Max(0, CInt(Math.Ceiling(total / pageSize)) - 1)
            pageIndex = Math.Min(Math.Max(pageIndex, 0), lastPage)

            ' Don't pull the (large) Message body for the list
            Dim rows = query.OrderByDescending(Function(m) m.CreateDate) _
                            .Skip(pageIndex * pageSize).Take(pageSize) _
                            .Select(Function(m) New With {
                                m.ID, m.ToAddress, m.ToName, m.Subject, m.EmailGroup, m.EmailType,
                                m.IsSent, m.IsRead, m.CreateDate, m.SentDate, m.ReadDate, m.LastAttempt}) _
                            .ToList()

            EmailGrid.VirtualItemCount = total
            EmailGrid.PageIndex = pageIndex
            EmailGrid.DataSource = rows
            EmailGrid.DataBind()

            TotalLiteral.Text = total.ToString("#,##0")
            SentLiteral.Text = sent.ToString("#,##0")
            PendingLiteral.Text = (total - sent).ToString("#,##0")
            OpenedLiteral.Text = opened.ToString("#,##0") & " opened"
            OpenRateLiteral.Text = If(sent > 0, Pct(opened, sent) & "%", "–")
            PageInfoLiteral.Text = If(total = 0, "", String.Format("Showing {0:#,##0}–{1:#,##0} of {2:#,##0}", pageIndex * pageSize + 1, pageIndex * pageSize + rows.Count, total))
        Catch ex As Exception
            Trace.Write("Unable to fetch email records.")
            Trace.Write(ex.Message)
            Trace.Write(ex.StackTrace)
            MessageLabel.Text = "<i class=""fa fa-exclamation-triangle""></i> Couldn't load emails: " & Server.HtmlEncode(ex.Message)
            MessageLabel.CssClass = "adm-error d-block"
            MessageLabel.Visible = True
        End Try
    End Sub

    ''' <summary>The 6 most recent email groups with sent / opened counts.</summary>
    Private Sub BindCampaigns()
        Try
            Dim camps = dc.EmailMessages.Where(Function(m) m.EmailGroup IsNot Nothing AndAlso m.EmailGroup <> "") _
                .GroupBy(Function(m) m.EmailGroup) _
                .Select(Function(g) New With {
                    .Group = g.Key,
                    .Total = g.Count(),
                    .Sent = g.Count(Function(x) x.IsSent),
                    .Read = g.Count(Function(x) x.IsRead),
                    .Last = g.Max(Function(x) x.CreateDate)}) _
                .OrderByDescending(Function(c) c.Last).Take(6).ToList()
            CampaignRepeater.DataSource = camps
            CampaignRepeater.DataBind()
        Catch ex As Exception
            Trace.Write("Unable to load campaigns: " & ex.Message)
        End Try
    End Sub

    ' ------------------------------------------------------------------
    '  Helpers used by the markup
    ' ------------------------------------------------------------------
    Protected Shared Function Pct(part As Object, whole As Object) As Integer
        Dim w As Double = Convert.ToDouble(whole)
        If w <= 0 Then Return 0
        Return CInt(Math.Round(Convert.ToDouble(part) * 100 / w))
    End Function

    Protected Shared Function TypeChip(t As Object) As String
        Dim name As String
        Select Case Convert.ToString(t)
            Case "1" : name = "Activation"
            Case "2" : name = "Unsubscribe"
            Case "3" : name = "Newsletter"
            Case "4" : name = "Change password"
            Case "5" : name = "Reminder"
            Case "6" : name = "Communication"
            Case Else : Return ""
        End Select
        Return "<span class=""em-chip type"">" & name & "</span>"
    End Function

    Protected Shared Function StatusHtml(isSent As Object, isRead As Object, lastAttempt As Object) As String
        If Convert.ToBoolean(isRead) Then Return "<span class=""es es-read""><i class=""fa fa-envelope-open-o""></i> Opened</span>"
        If Convert.ToBoolean(isSent) Then Return "<span class=""es es-sent""><i class=""fa fa-check""></i> Sent</span>"
        If lastAttempt IsNot Nothing AndAlso Not IsDBNull(lastAttempt) Then
            Return "<span class=""es es-retry"" title=""Last tried " & Convert.ToDateTime(lastAttempt).ToString("d MMM yyyy, h:mm tt") & """><i class=""fa fa-refresh""></i> Retrying</span>"
        End If
        Return "<span class=""es es-queued""><i class=""fa fa-clock-o""></i> Queued</span>"
    End Function

    Protected Shared Function WhenHtml(created As Object, sent As Object, read As Object) As String
        Dim f = Function(o As Object) If(o Is Nothing OrElse IsDBNull(o), "", Convert.ToDateTime(o).ToString("d MMM yyyy, h:mm tt"))
        Dim html As String = f(created)
        If f(sent) <> "" Then html &= "<div class=""s"">Sent " & f(sent) & "</div>"
        If f(read) <> "" Then html &= "<div class=""s"">Opened " & f(read) & "</div>"
        Return html
    End Function

    ' ------------------------------------------------------------------
    '  Events
    ' ------------------------------------------------------------------
    Protected Sub SubmitButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Bind(0)
        BindCampaigns()
    End Sub

    Protected Sub ResetButton_Click(sender As Object, e As EventArgs) Handles ResetButton.Click
        KeywordTextBox.Text = ""
        GroupDropDown.SelectedValue = ""
        TypeDropDown.SelectedValue = ""
        SentDropDown.SelectedValue = ""
        ReadDropDown.SelectedValue = ""
        Bind(0)
        BindCampaigns()
    End Sub

    Protected Sub CampaignRepeater_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles CampaignRepeater.ItemCommand
        If e.CommandName <> "Group" Then Return
        Dim g As String = Convert.ToString(e.CommandArgument)
        ' Click the selected campaign again to clear the filter
        GroupDropDown.SelectedValue = If(GroupDropDown.SelectedValue = g, "", If(GroupDropDown.Items.FindByValue(g) IsNot Nothing, g, ""))
        Bind(0)
        BindCampaigns()
    End Sub

    Protected Sub EmailGrid_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
        Bind(e.NewPageIndex)
        BindCampaigns()
    End Sub

    Protected Sub DeleteButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim removed As Integer = 0
        For Each row As GridViewRow In EmailGrid.Rows
            If row.RowIndex > -1 Then
                Dim ck As CheckBox = TryCast(row.FindControl("cbSelect"), CheckBox)
                If ck IsNot Nothing AndAlso ck.Checked Then
                    Dim lt As Literal = TryCast(row.FindControl("EmailIDLt"), Literal)
                    Dim idText As String = lt.Text
                    Dim em = dc.EmailMessages.SingleOrDefault(Function(m) m.ID.ToString() = idText)
                    If em IsNot Nothing Then
                        dc.EmailMessages.Remove(em)
                        removed += 1
                    End If
                End If
            End If
        Next
        dc.SaveChanges()

        MessageLabel.Text = "<i class=""fa fa-check""></i> " & removed & " email" & If(removed = 1, "", "s") & " removed."
        MessageLabel.Visible = True
        Bind(EmailGrid.PageIndex)
        BindCampaigns()
    End Sub
End Class