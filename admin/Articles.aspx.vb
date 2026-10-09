Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports IndiaBobbles

Public Class Articles
    Inherits AdminPage
    Dim dc As New indiabobblesEntities

    Private Sub Articles_Load(sender As Object, e As EventArgs) Handles Me.Load
        If ForbidUserAccess(MemberTypeType.Admin, MemberTypeType.Editor, MemberTypeType.Author) Then
            Response.Redirect("default.aspx")
        End If
        If Not IsPostBack AndAlso Session("ArticleFlash") IsNot Nothing Then
            MessageLabel.Text = "<i class=""fa fa-check""></i> " & Server.HtmlEncode(CStr(Session("ArticleFlash")))
            MessageLabel.Visible = True
            Session.Remove("ArticleFlash")
        End If
    End Sub

    Protected Sub ArticleGridView_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
        Select Case e.CommandName
            Case "TopStory"
                Try
                    Dim ts As TopStory = New TopStory()
                    ts.CreatedBy = CurrentUser.ID
                    ts.DateCreated = DateTime.UtcNow
                    ts.PostId = Long.Parse(e.CommandArgument.ToString())
                    dc.TopStories.Add(ts)
                    dc.SaveChanges()
                    Response.Redirect("topstory.aspx")
                Catch ex As Exception
                    ShowError("Unable to set top story. Error - " & ex.Message, ex)
                End Try

            Case "TogglePublish"
                ' Publish (2) <-> Draft (1); Inactive (3) becomes Published
                Try
                    Dim pid As Integer = Integer.Parse(e.CommandArgument.ToString())
                    dc.Database.ExecuteSqlCommand("UPDATE Post SET Status = CASE WHEN Status = 2 THEN 1 ELSE 2 END WHERE ID = {0}", pid)
                    Dim nowLive As Boolean = dc.Database.SqlQuery(Of Integer)("SELECT CAST(Status AS int) FROM Post WHERE ID = {0}", pid).FirstOrDefault() = 2
                    Done(If(nowLive, "Article published – it's now live on the blog.", "Article moved back to Draft."))
                Catch ex As Exception
                    ShowError("Unable to change the article status. Error - " & ex.Message, ex)
                End Try

            Case "ToggleSitemap"
                Try
                    Dim pid As Integer = Integer.Parse(e.CommandArgument.ToString())
                    dc.Database.ExecuteSqlCommand("UPDATE Post SET Sitemap = CASE WHEN Sitemap = 1 THEN 0 ELSE 1 END WHERE ID = {0}", pid)
                    Done("Sitemap setting updated.")
                Catch ex As Exception
                    ShowError("Unable to update the sitemap setting. Error - " & ex.Message, ex)
                End Try

            Case "DeleteCommand"
                Try
                    Dim pid As Integer = Integer.Parse(e.CommandArgument.ToString())
                    Dim item = (From u In dc.Posts Where u.ID = pid Select u).SingleOrDefault()

                    If item IsNot Nothing Then
                        Try
                            System.IO.File.Delete(Server.MapPath(String.Format("{1}/articlexml-{0}.txt", item.ID, Utility.CustomPageFolder)))
                        Catch iex As Exception
                            Trace.Write("Unable to delete article file.")
                            Trace.Write(iex.Message)
                            Trace.Write(iex.StackTrace)
                            Trace.Write(iex.Source)
                        End Try

                        dc.Posts.Remove(item)
                        dc.SaveChanges()
                    End If
                    Done("Article deleted.")
                Catch ex As Exception
                    ShowError("Unable to delete article. Error - " & ex.Message, ex)
                End Try
        End Select
    End Sub

    ''' <summary>Reload the list (so a refresh can't repeat the action) and show a green message.</summary>
    Private Sub Done(msg As String)
        Session("ArticleFlash") = msg
        Response.Redirect("~/admin/articles.aspx", False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub

    Private Sub ShowError(msg As String, ex As Exception)
        message1.Text = msg
        message1.Visible = True
        message1.Indicate = AlertType.[Error]
        Trace.Write(msg)
        Trace.Write(ex.Message)
        Trace.Write(ex.StackTrace)
    End Sub
End Class