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
        ViewBag.CanonicalUrl = IndiaBobbles.Utility.SiteURL
        Return View()
    End Function

    Function Tag(ByVal id As String) As ActionResult
        Dim t = db.CategoryTags.FirstOrDefault(Function(m) m.UrlName = id)
        ViewBag.Tag = id
        If t IsNot Nothing Then
            Return View(db.ProductTags.Where(Function(m) m.TagID = t.ID).OrderBy(Function(m) m.Product.OutofStock).Select(Function(m) m.Product).ToList())
        Else
            Return View(New List(Of Product))
        End If
    End Function

    Function About() As ActionResult
        ViewBag.CanonicalUrl = IndiaBobbles.Utility.SiteURL & "/about"
        Return View()
    End Function

    Function Contact() As ActionResult
        ViewBag.CanonicalUrl = IndiaBobbles.Utility.SiteURL & "/contact"
        Return View()
    End Function

    Function Privacy() As ActionResult
        ViewBag.CanonicalUrl = IndiaBobbles.Utility.SiteURL & "/privacy-policy"
        Return View()
    End Function

    Function Shipping() As ActionResult
        ViewBag.CanonicalUrl = IndiaBobbles.Utility.SiteURL & "/shipping-policy"
        Return View()
    End Function

    Function Terms() As ActionResult
        ViewBag.CanonicalUrl = IndiaBobbles.Utility.SiteURL & "/terms-and-conditions"
        Return View()
    End Function

    Function Payment() As ActionResult
        ViewBag.CanonicalUrl = IndiaBobbles.Utility.SiteURL & "/payment-options"
        Return View()
    End Function

    Function Collectibles() As ActionResult
        ViewBag.CanonicalUrl = IndiaBobbles.Utility.SiteURL & "/collectibles"
        Return RedirectPermanent("~/tag/collectibles")
    End Function

    Function Games() As ActionResult
        ViewBag.CanonicalUrl = IndiaBobbles.Utility.SiteURL & "/games"
        Return View(db.Posts.Where(Function(t) t.Status = PostStatusType.Publish And t.Category1.Name = "Games").OrderByDescending(Function(t) t.DateCreated).ToList())
    End Function

    Function CustomBobblehead() As ActionResult
        ViewBag.CanonicalUrl = IndiaBobbles.Utility.SiteURL & "/order-custom-bobbleheads"
        Return View()
    End Function

    ' GET: /contact/whatsapp
    Function WhatsAppRedirect() As ActionResult
        Dim number As String = "919871500276"
        Dim message As String = Uri.EscapeDataString("Hi, I am interested in a bobblehead")
        Return Redirect($"https://wa.me/{number}?text={message}")
    End Function
End Class
