﻿Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports IndiaBobbles
Public Class ManageArticle
    Inherits AdminPage
    Private a As Article = New Article()
    Private dc As New indiabobblesEntities()
    Private Sub ManageArticle_Load(sender As Object, e As EventArgs) Handles Me.Load
        If ForbidUserAccess(MemberTypeType.Admin, MemberTypeType.Editor, MemberTypeType.Author) Then
            Response.Redirect("default.aspx")
        End If

        'If Mode = "edit" Then
        '    PopulateArticle()
        'End If

        If Not Page.IsCallback AndAlso Not Page.IsPostBack Then
            If Mode = "edit" Then
                PopulateForm()
            End If
        End If
    End Sub

    'Private Sub PopulateArticle()

    '    Try
    '        a = Utility.Deserialize(Of Article)(System.IO.File.ReadAllText(Server.MapPath(String.Format("{1}/articlexml-{0}.txt", TargetID, Utility.ArticleFolder))))

    '        If String.IsNullOrEmpty(a.MetaTitle) Then
    '            a.MetaTitle = a.Title
    '        End If

    '    Catch ex As Exception
    '        a = New Article()
    '        Trace.Write("Unable to read xml file of article.")
    '        Trace.Write(ex.Message)
    '        Trace.Write(ex.StackTrace)
    '        Dim p As Post = (From t In dc.Posts Where t.ID = TargetID Select t).SingleOrDefault()

    '        If p IsNot Nothing Then
    '            a.Category = p.Category
    '            a.CreatedBy = p.CreatedBy
    '            a.DateCreated = p.DateCreated
    '            a.DateModified = p.DateModified
    '            a.Description = p.Description
    '            a.ID = p.ID
    '            a.ModifiedBy = p.ModifiedBy
    '            a.Status = CType([Enum].Parse(GetType(PostStatusType), p.Status.ToString()), PostStatusType)
    '            a.Tag = p.Tag
    '            a.Title = p.Title
    '            a.WriterEmail = p.WriterEmail
    '            a.WriterName = p.WriterName
    '            a.Viewed = p.Viewed
    '            a.Sitemap = p.Sitemap
    '            a.URL = p.URL

    '            Try
    '                a.Text = System.IO.File.ReadAllText(Server.MapPath(String.Format("{1}/article-{0}.txt", p.ID, Utility.ArticleFolder)))
    '            Catch ex2 As Exception
    '                Trace.Write(ex2.Message)
    '                Trace.Write(ex2.StackTrace)
    '            End Try
    '        Else
    '            Response.Redirect("articles.aspx")
    '        End If
    '    End Try

    'End Sub

    Private Sub PopulateForm()
        Dim p As Post = (From t In dc.Posts Where t.ID = TargetID Select t).SingleOrDefault()

        If p IsNot Nothing Then
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
        End If
    End Sub

    Protected Sub SubmitButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Page.Validate("VideoGrp")
        If Not Page.IsValid Then Return

        Try
            'a.Category = Integer.Parse(CategoryDropDown.SelectedValue)
            'a.Description = DescTextBox.Text.Trim()
            'a.OGDescription = FacebookDescTextBox.Text.Trim()
            'a.OGImage = FacebookImageTextBox.Text.Trim()
            'a.TemplateName = String.Empty
            'a.Status = CType([Enum].Parse(GetType(PostStatusType), StatusDropDown.SelectedValue), PostStatusType)
            'a.Tag = TagTextBox.Text.Trim()
            'a.Text = TextTextBox.Text.Trim()
            'a.Title = TitleTextBox.Text.Trim()
            'a.WriterEmail = WriterEmailTextBox.Text.Trim()
            'a.WriterName = WriterTextBox.Text.Trim()
            'a.URL = URLTextBox.Text.Trim()
            'a.Sitemap = SitemapCheckBox.Checked
            'a.MetaTitle = MetaTitleTextBox.Text.Trim()

            If Mode = "edit" Then
                Dim p As Post = (From t In dc.Posts Where t.ID = TargetID Select t).SingleOrDefault()
                p.Category = Integer.Parse(CategoryDropDown.SelectedValue)
                p.DateModified = DateTime.Now
                p.Description = DescTextBox.Text.Trim()
                p.ID = TargetID
                p.ModifiedBy = CurrentUser.ID
                p.Status = CByte(StatusDropDown.SelectedValue)
                p.Tag = TagTextBox.Text.Trim()
                p.Title = TitleTextBox.Text.Trim()
                p.WriterEmail = WriterEmailTextBox.Text.Trim()
                p.WriterName = WriterTextBox.Text.Trim()
                p.OGDescription = FacebookDescTextBox.Text.Trim()
                p.OGImage = FacebookImageTextBox.Text.Trim()
                p.URL = URLTextBox.Text.Trim()
                p.Sitemap = SitemapCheckBox.Checked
                p.Article = TextTextBox.Text.Trim()
                p.MetaTitle = MetaTitleTextBox.Text.Trim()
                dc.SaveChanges()
                'Dim str As String = Utility.Serialize(Of Article)(a)
                'System.IO.File.WriteAllText(Server.MapPath(String.Format("{1}/articlexml-{0}.txt", p.ID, Utility.ArticleFolder)), str)
            Else
                Dim p As Post = New Post()
                p.Category = Integer.Parse(CategoryDropDown.SelectedValue)
                p.DateCreated = DateTime.Now
                p.Description = DescTextBox.Text.Trim()
                p.ID = TargetID
                p.CreatedBy = CurrentUser.ID
                p.Status = CByte(StatusDropDown.SelectedValue)
                p.Tag = TagTextBox.Text.Trim()
                p.Title = TitleTextBox.Text.Trim()
                p.WriterEmail = WriterEmailTextBox.Text.Trim()
                p.WriterName = WriterTextBox.Text.Trim()
                p.OGDescription = FacebookDescTextBox.Text.Trim()
                p.OGImage = FacebookImageTextBox.Text.Trim()
                p.URL = URLTextBox.Text.Trim()
                p.Sitemap = SitemapCheckBox.Checked
                p.Article = TextTextBox.Text.Trim()
                p.MetaTitle = MetaTitleTextBox.Text.Trim()
                dc.Posts.Add(p)
                dc.SaveChanges()
                'Dim str As String = Utility.Serialize(Of Article)(a)
                'System.IO.File.WriteAllText(Server.MapPath(String.Format("{1}/articlexml-{0}.txt", p.ID, Utility.ArticleFolder)), str)
            End If
            Response.Redirect("articles.aspx")
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

        If Mode = "edit" Then
            Dim p As Post = (From t In dc.Posts Where t.ID <> TargetID AndAlso t.URL = URLTextBox.Text.Trim() Select t).SingleOrDefault()

            If p IsNot Nothing Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        Else
            Dim p As Post = (From t In dc.Posts Where t.URL = URLTextBox.Text.Trim() Select t).SingleOrDefault()

            If p IsNot Nothing Then
                args.IsValid = False
            Else
                args.IsValid = True
            End If
        End If
    End Sub

    Protected Sub TitleTextBox_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
        URLTextBox.Text = Utility.Slugify(TitleTextBox.Text.Trim())
    End Sub

    Protected Sub URLTextBox_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
        URLTextBox.Text = Utility.Slugify(URLTextBox.Text.Trim())
    End Sub
End Class