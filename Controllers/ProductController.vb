Imports System.Web.Mvc

Namespace Controllers
    Public Class ProductController
        Inherits Controller
        Private ReadOnly db As New indiabobblesEntities
        ' GET: Product
        Function Detail(ByVal id As String) As ActionResult
            Return View(db.Products.FirstOrDefault(Function(t) t.URL = id))
        End Function
    End Class
End Namespace