Imports System.Web.Mvc

Namespace Controllers
    Public Class CartController
        Inherits Controller
        Dim om As New OrderManager()
        Private ReadOnly db As New indiabobblesEntities
        ' GET: Cart
        Function Index() As ActionResult
            Return View(om.GetCart())
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function Add(ByVal id As Integer) As ActionResult
            Dim p = db.Products.FirstOrDefault(Function(t) t.ID = id)
            If p IsNot Nothing Then
                Dim o As Order = om.GetCart()
                If o.ID = 0 Then
                    o = om.Create(String.Empty, String.Empty, Nothing, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, OrderStatusType.[New], String.Empty, String.Empty, DateTime.Now, 0, 0, 0, 0, 0, 0, 40.0, "", "", "", "", "")
                    CookieWorker.SetCookie(CookieWorker.OrderIdKey, "cartid", o.ID.ToString(), DateTime.Now.AddHours(50))
                End If
                Dim imgpath = String.Empty
                If p.ProductPhotoes.Count > 0 Then
                    imgpath = p.ProductPhotoes.First().ImagePath
                End If

                om.AddItem(1, o.ID, imgpath, p.Name, p.ProductCode, p.SalePrice)
            End If
            Return RedirectToAction("Index")
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function Remove(ByVal id As Integer) As ActionResult

            Dim o As Order = om.GetCart()
            om.RemoveItem(id, o.ID)
            Return RedirectToAction("Index")
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function ApplyCoupon(ByVal coupon As String) As ActionResult

            Dim o As Order = om.GetCart()
            om.UpdateCouponCode(o.ID, coupon)

            Return RedirectToAction("Index")
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function RemoveCoupon() As ActionResult

            Dim o As Order = om.GetCart()
            om.UpdateCouponCode(o.ID, "")

            Return RedirectToAction("Index")
        End Function
    End Class
End Namespace