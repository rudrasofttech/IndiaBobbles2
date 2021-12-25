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
    Protected Sub ArticleGridView_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
        If e.CommandName = "TopStory" Then
            Try
                Dim ts As TopStory = New TopStory()
                ts.CreatedBy = CurrentUser.ID
                ts.DateCreated = DateTime.UtcNow
                ts.PostId = Long.Parse(e.CommandArgument.ToString())
                dc.TopStories.Add(ts)
                dc.SaveChanges()
                Response.Redirect("topstory.aspx")
            Catch ex As Exception
                message1.Text = String.Format("Unable to set top story. Error - {0}", ex.Message)
                message1.Visible = True
                message1.Indicate = AlertType.[Error]
                Trace.Write("Unable to set top story.")
                Trace.Write(ex.Message)
                Trace.Write(ex.StackTrace)
            End Try
        ElseIf e.CommandName = "DeleteCommand" Then

            Try


                Dim item = (From u In dc.Posts Where u.ID = Integer.Parse(e.CommandArgument.ToString()) Select u).SingleOrDefault()

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

                Response.Redirect("default.aspx")
            Catch ex As Exception
                message1.Text = String.Format("Unable to delete article. Error - {0}", ex.Message)
                message1.Visible = True
                message1.Indicate = AlertType.[Error]
                Trace.Write("Unable to delete article.")
                Trace.Write(ex.Message)
                Trace.Write(ex.StackTrace)
                Trace.Write(ex.Source)
            End Try
        End If
    End Sub

    Private Sub Articles_Load(sender As Object, e As EventArgs) Handles Me.Load
        If ForbidUserAccess(MemberTypeType.Admin, MemberTypeType.Editor, MemberTypeType.Author) Then
            Response.Redirect("default.aspx")
        End If
    End Sub
End Class