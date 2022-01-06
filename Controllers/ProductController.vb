Imports System.Web.Mvc

Namespace Controllers
    Public Class ProductController
        Inherits Controller
        Private ReadOnly db As New indiabobblesEntities
        ' GET: Product
        Function Detail(ByVal id As Integer) As ActionResult
            Return View(db.Products.FirstOrDefault(Function(t) t.ID = id))
        End Function

        Function OldPageRedirect(ByVal id As String) As ActionResult
            Dim product = db.Products.FirstOrDefault(Function(t) t.URL = id)
            If product IsNot Nothing Then
                Return RedirectPermanent("~/product/" & product.ID & "/" & Utility.Slugify(product.Name))
            Else
                Return HttpNotFound()
            End If
        End Function
    End Class
End Namespace