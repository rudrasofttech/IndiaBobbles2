Public Class SaveArctileToDB
    Inherits AdminPage
    Private ReadOnly dc As New indiabobblesEntities
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim posts = dc.Posts.Where(Function(m) True)
        For Each p In posts
            Dim a As Article
            Try
                Trace.Write(Server.MapPath(String.Format("~/sitedata/Article/articlexml-{0}.txt", p.ID)))
                a = Utility.Deserialize(Of Article)(System.IO.File.ReadAllText(Server.MapPath(String.Format("~/sitedata/Article/articlexml-{0}.txt", p.ID))))

                p.Article = a.Text

            Catch ex As Exception
                Trace.Write("Unable to read xml file of article. ")
                Trace.Write(ex.Message)
                Trace.Write(ex.StackTrace)
                If ex.InnerException IsNot Nothing Then
                    Trace.Write(ex.InnerException.Message)
                End If

            End Try
        Next
        dc.SaveChanges()
    End Sub
End Class