Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports IndiaBobbles

Public Class ManageArticle
    Inherits AdminPage

    Private dc As New indiabobblesEntities()

    Private Sub ManageArticle_Load(sender As Object, e As EventArgs) Handles Me.Load
        If ForbidUserAccess(MemberTypeType.Admin, MemberTypeType.Editor, MemberTypeType.Author) Then
            Response.Redirect("default.aspx")
        End If

        If Not Page.IsCallback AndAlso Not Page.IsPostBack Then
            If Mode = "edit" Then
                PopulateForm()
            Else
                ' New article: start as a draft, written by the logged-in user
                StatusDropDown.SelectedValue = "1"
                Try
                    If String.IsNullOrEmpty(WriterTextBox.Text) Then WriterTextBox.Text = Convert.ToString(CurrentUser.MemberName)
                    If String.IsNullOrEmpty(WriterEmailTextBox.Text) Then WriterEmailTextBox.Text = Convert.ToString(CurrentUser.Email)
                Catch
                End Try
            End If

            If Session("ArticleSaved") IsNot Nothing Then
                message1.Text = CStr(Session("ArticleSaved"))
                message1.Visible = True
                Session.Remove("ArticleSaved")
            End If
        End If
    End Sub

    Private Sub PopulateForm()
        Dim p As Post = (From t In dc.Posts Where t.ID = TargetID Select t).SingleOrDefault()
        If p Is Nothing Then
            Response.Redirect("articles.aspx")
            Return
        End If

        HeadingLit.Text = "Edit article"
        Page.Title = p.Title
        TitleTextBox.Text = p.Title
        TagTextBox.Text = p.Tag
        WriterTextBox.Text = p.WriterName
        WriterEmailTextBox.Text = p.WriterEmail
        CategoryDropDown.SelectedValue = p.Category.ToString()
        FacebookImageTextBox.Text = p.OGImage
        FacebookDescTextBox.Text = p.OGDescription
        StatusDropDown.SelectedValue = (CByte(p.Status)).ToString()
        DescTextBox.Text = p.Description
        TextTextBox.Text = p.Article
        SitemapCheckBox.Checked = p.Sitemap
        URLTextBox.Text = p.URL
        MetaTitleTextBox.Text = p.MetaTitle

        ViewLink.NavigateUrl = Utility.SiteURL & "/blog/" & p.URL & If(CByte(p.Status) = 2, "", "?preview=true")
        ViewLink.Text = If(CByte(p.Status) = 2, "<i class=""fa fa-external-link""></i> View on website", "<i class=""fa fa-eye""></i> Preview")
        ViewLink.Visible = True
    End Sub

    ''' <summary>Used by both "Save" (back to the list) and "Save & keep editing".</summary>
    Protected Sub SubmitButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Clean the web address before validating it (the duplicate check uses it)
        URLTextBox.Text = Utility.Slugify(If(String.IsNullOrWhiteSpace(URLTextBox.Text), TitleTextBox.Text, URLTextBox.Text).Trim())

        Page.Validate("VideoGrp")
        If Not Page.IsValid Then Return

        Dim stay As Boolean = sender Is SaveStayButton
        Dim title As String = TitleTextBox.Text.Trim()
        Dim metaTitle As String = If(String.IsNullOrWhiteSpace(MetaTitleTextBox.Text), title, MetaTitleTextBox.Text.Trim())

        Try
            Dim p As Post
            If Mode = "edit" Then
                p = (From t In dc.Posts Where t.ID = TargetID Select t).SingleOrDefault()
                If p Is Nothing Then Response.Redirect("articles.aspx") : Return
                p.DateModified = DateTime.Now
                p.ModifiedBy = CurrentUser.ID
            Else
                p = New Post()
                p.DateCreated = DateTime.Now
                p.CreatedBy = CurrentUser.ID
                dc.Posts.Add(p)
            End If

            p.Category = Integer.Parse(CategoryDropDown.SelectedValue)
            p.Description = DescTextBox.Text.Trim()
            p.Status = CByte(StatusDropDown.SelectedValue)
            p.Tag = TagTextBox.Text.Trim()
            p.Title = title
            p.WriterEmail = WriterEmailTextBox.Text.Trim()
            p.WriterName = WriterTextBox.Text.Trim()
            p.OGDescription = FacebookDescTextBox.Text.Trim()
            p.OGImage = FacebookImageTextBox.Text.Trim()
            p.URL = URLTextBox.Text.Trim()
            p.Sitemap = SitemapCheckBox.Checked
            p.Article = TextTextBox.Text.Trim()
            p.MetaTitle = metaTitle
            dc.SaveChanges()

            If stay Then
                Session("ArticleSaved") = "Saved at " & DateTime.Now.ToString("h:mm tt") & "."
                Response.Redirect("managearticle.aspx?id=" & p.ID & "&mode=edit", False)
            Else
                Session("ArticleFlash") = "Article """ & title & """ saved."     ' shown on the Articles list
                Response.Redirect("articles.aspx", False)
            End If
            Context.ApplicationInstance.CompleteRequest()
        Catch ex As Exception
            message1.Text = String.Format("Unable to save article. {0}", ex.Message)
            message1.Visible = True
            message1.Indicate = AlertType.[Error]
            Trace.Write("Unable to save article.")
            Trace.Write(ex.Message)
            Trace.Write(ex.StackTrace)
        End Try
    End Sub

    Protected Sub CustomValidator1_ServerValidate(ByVal source As Object, ByVal args As ServerValidateEventArgs)
        Dim url As String = URLTextBox.Text.Trim()
        Dim id As Integer = If(Mode = "edit", TargetID, 0)
        args.IsValid = Not dc.Posts.Any(Function(t) t.URL = url AndAlso t.ID <> id)
    End Sub
End Class