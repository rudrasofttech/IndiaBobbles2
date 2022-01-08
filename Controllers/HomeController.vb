Public Class HomeController
    Inherits System.Web.Mvc.Controller
    Private ReadOnly db As New indiabobblesEntities

    Function Index() As ActionResult
        Dim hl = db.CategoryTags.FirstOrDefault(Function(m) m.UrlName = "highlight")
        If hl IsNot Nothing Then
            ViewBag.Highlights = db.ProductTags.Where(Function(m) m.TagID = hl.ID).Select(Function(m) m.Product).ToList()
        Else
            ViewBag.Highlights = New List(Of Product)
        End If
        Return View()
    End Function

    Function Tag(ByVal id As String) As ActionResult
        Dim t = db.CategoryTags.FirstOrDefault(Function(m) m.UrlName = id)
        ViewBag.Tag = id
        If t IsNot Nothing Then
            Return View(db.ProductTags.Where(Function(m) m.TagID = t.ID).Select(Function(m) m.Product).ToList())
        Else
            Return View(New List(Of Product))
        End If
    End Function

    Function About() As ActionResult
        Return View()
    End Function

    Function Privacy() As ActionResult
        Return View()
    End Function

    Function Shipping() As ActionResult
        Return View()
    End Function

    Function Terms() As ActionResult
        Return View()
    End Function

    Function Payment() As ActionResult
        Return View()
    End Function

    Function Collectibles() As ActionResult
        Return RedirectPermanent("~/tag/collectibles")
    End Function

    Function Games() As ActionResult
        Return View(db.Posts.Where(Function(t) t.Status = PostStatusType.Publish And t.Category1.Name = "Games").OrderByDescending(Function(t) t.DateCreated).ToList())
    End Function

    Function CustomBobblehead() As ActionResult
        Return View()
    End Function

    Function Contact() As ActionResult
        ViewData("Message") = "Your contact page."

        Return View()
    End Function
End Class
