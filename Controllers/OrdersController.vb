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

        Function Delete(ByVal id As Integer) As ActionResult
            Dim order = db.Orders.Include("OrderItems").FirstOrDefault(Function(t) t.ID = id)
            If (order IsNot Nothing) Then
                db.OrderItems.RemoveRange(order.OrderItems)
                db.Orders.Remove(order)
                db.SaveChanges()
            End If
            Return Redirect("~/orders/list")
        End Function

        Function Email(ByVal id As Integer) As ActionResult
            Dim om As New OrderManager()
            Dim o As Order = om.GetOrderDetail(id)

            If o IsNot Nothing Then
                Try
                    Dim body As String = om.GenerateReceipt(o.ID)
                    Dim eman As New EmailManager()
                    eman.SendMail(Utility.NewsletterEmail, o.Email, Utility.AdminName, o.Name, body, String.Format("Order Receipt : {0} from {1}", o.ID, Utility.SiteName), EmailMessageType.Communication, "Order Receipt")
                    eman.SendMail(Utility.NewsletterEmail, "rajkiran.singh@rudrasofttech.com", Utility.AdminName, "Raj Kiran Singh", body, String.Format("Order Receipt : {0} from {1}", o.ID, Utility.SiteName), EmailMessageType.Communication, "Order Receipt")
                Catch ex As Exception
                    Trace.Write(ex.Message)
                End Try
            End If

            Return Redirect("~/orders/detail/" & id)
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function Receipt(ByVal id As Integer) As ActionResult
            Return View(db.Orders.Include("OrderItems").FirstOrDefault(Function(t) t.ID = id))
        End Function
    End Class
End Namespace