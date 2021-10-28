Public Class HomeController
    Inherits System.Web.Mvc.Controller
    Private ReadOnly db As New indiabobblesEntities

    Function Index() As ActionResult
        Return View()
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
        Return View(db.Products.ToList())
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
