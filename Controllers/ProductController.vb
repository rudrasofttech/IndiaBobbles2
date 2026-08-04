Imports System.Web.Mvc

Namespace Controllers
    Public Class ProductController
        Inherits Controller
        Private ReadOnly db As New indiabobblesEntities
        ' GET: Product
        Function Detail(ByVal id As Integer, ByVal name As String) As ActionResult
            Dim product = db.Products.FirstOrDefault(Function(t) t.ID = id)
            If product IsNot Nothing Then
                Dim correctSlug = Utility.Slugify(product.Name)
                ' 301 redirect if slug in URL doesn't match canonical slug
                If Not String.Equals(name, correctSlug, StringComparison.OrdinalIgnoreCase) Then
                    Return RedirectToRoutePermanent("ProductRoute", New With {.id = product.ID, .name = correctSlug})
                End If

                ViewBag.CanonicalUrl = String.Format("{0}/product/{1}/{2}", Utility.SiteURL, product.ID, correctSlug)
            Else
                Response.StatusCode = 404
            End If
            Return View(product)
        End Function

        Function OldPageDetail(ByVal id As String) As ActionResult
            Dim product = db.Products.FirstOrDefault(Function(t) t.URL = id)
            If product IsNot Nothing Then
                Return RedirectPermanent("~/product/" & product.ID & "/" & Utility.Slugify(product.Name))
            Else
                Return RedirectPermanent("~/tag/trending")
            End If

        End Function
    End Class
End Namespace