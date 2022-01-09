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
                Dim a As New Article With {
                    .Category = i.Category,
                    .CategoryName = i.Category1.Name,
                    .CreatedBy = i.CreatedBy,
                    .CreatedByName = i.Member.MemberName,
                    .DateCreated = i.DateCreated,
                    .DateModified = i.DateModified,
                    .Description = i.Description,
                    .ID = i.ID,
                    .ModifiedBy = i.ModifiedBy,
                    .Status = i.Status,
                    .Tag = i.Tag,
                    .Text = String.Empty,
                    .Title = i.Title,
                    .WriterEmail = i.WriterEmail,
                    .WriterName = i.WriterName,
                    .OGDescription = i.OGDescription,
                    .OGImage = i.OGImage,
                    .Viewed = i.Viewed,
                    .URL = i.URL,
                    .MetaTitle = i.MetaTitle
                }
                articles.Add(a)
            Next
            Return View(articles)
        End Function
    End Class
End Namespace