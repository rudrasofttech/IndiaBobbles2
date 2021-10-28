Imports System.Web.Mvc

Namespace Controllers
    Public Class OrdersController
        Inherits Controller
        Private ReadOnly db As New indiabobblesEntities
        ' GET: Orders
        Function Index() As ActionResult
            Dim filter As String = Request.QueryString("q")
            If Not String.IsNullOrEmpty(filter) Then
                Return View(db.Orders.Where(Function(o) o.Email = filter Or o.Phone = filter Or o.ID.ToString() = filter).OrderByDescending(Function(f) f.DateCreated).ToList())
            End If
            Return View()
        End Function

        Function Detail(ByVal id As Integer) As ActionResult
            Return View(db.Orders.Include("OrderItems").FirstOrDefault(Function(t) t.ID = id))
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function Receipt(ByVal id As Integer) As ActionResult
            Return View(db.Orders.Include("OrderItems").FirstOrDefault(Function(t) t.ID = id))
        End Function
    End Class
End Namespace