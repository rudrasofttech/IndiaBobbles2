Imports System.Web.Mvc

Namespace Controllers
    Public Class ArticlesController
        Inherits Controller

        Private ReadOnly db As New indiabobblesEntities

        Function Detail(url As String) As ActionResult
            Dim post = db.Posts.FirstOrDefault(Function(p) p.URL = url)
            If post IsNot Nothing Then
                Return View(post)
            Else
                Return View()
            End If
        End Function

        ' GET: Article
        Function Index() As ActionResult
            Dim articles As New List(Of Article)
            Dim items = db.Posts.Where(Function(t) t.Status = PostStatusType.Publish And t.Category1.Name <> "Games").OrderByDescending(Function(t) t.DateCreated)
            For Each i In items
                Dim a As Article = New Article()
                a.Category = i.Category
                a.CategoryName = i.Category1.Name
                a.CreatedBy = i.CreatedBy
                a.CreatedByName = i.Member.MemberName
                a.DateCreated = i.DateCreated
                a.DateModified = i.DateModified
                a.Description = i.Description
                a.ID = i.ID
                a.ModifiedBy = i.ModifiedBy
                a.Status = i.Status
                a.Tag = i.Tag
                a.Text = String.Empty
                a.Title = i.Title
                a.WriterEmail = i.WriterEmail
                a.WriterName = i.WriterName
                a.OGDescription = i.OGDescription
                a.OGImage = i.OGImage
                a.Viewed = i.Viewed
                a.URL = i.URL
                articles.Add(a)
            Next
            Return View(articles)
        End Function
    End Class
End Namespace