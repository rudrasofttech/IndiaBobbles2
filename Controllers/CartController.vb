Imports System.Web.Mvc

Namespace Controllers
    Public Class CartController
        Inherits Controller

        ReadOnly om As New OrderManager()
        ReadOnly db As New indiabobblesEntities
        ' GET: Cart
        Function Index() As ActionResult
            Return View(om.GetCart())
        End Function

        Function Add(ByVal id As Integer) As ActionResult
            Dim p = db.Products.FirstOrDefault(Function(t) t.ID = id)
            If p IsNot Nothing Then
                Dim o As Order = om.GetCart()
                If o.ID = 0 Then
                    o = om.Create(String.Empty, String.Empty, Nothing, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, OrderStatusType.[New], String.Empty, String.Empty, DateTime.Now, 0, 0, 0, 0, 0, 0, 40.0, "", "", "", "", "")
                    CookieWorker.SetCookie(CookieWorker.OrderIdKey, "cartid", o.ID.ToString(), DateTime.Now.AddDays(10))
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
        Function ApplyCoupon(ByVal coupon As String, ByVal returnto As String) As ActionResult

            Dim o As Order = om.GetCart()
            om.UpdateCouponCode(o.ID, coupon)
            Return RedirectToAction(returnto)
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function RemoveCoupon(ByVal returnto As String) As ActionResult

            Dim o As Order = om.GetCart()
            om.UpdateCouponCode(o.ID, "")

            Return RedirectToAction(returnto)
        End Function

        Function AddQuantity(ByVal id As Integer) As ActionResult
            Dim o As Order = om.GetCart()
            om.AddItemQuantity(id, o.ID)
            Return RedirectToAction("Index")
        End Function

        Function SubtractQuantity(ByVal id As Integer) As ActionResult
            Dim o As Order = om.GetCart()
            om.ReduceItemQuantity(id, o.ID)
            Return RedirectToAction("Index")
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function Address(ByVal model As OrderAddressDTO) As ActionResult
            If Not ModelState.IsValid Then
                Return View(model)
            End If
            Dim o As Order = om.GetCart()
            om.UpdateOrderContact(o.ID, model.Name, model.Email, Nothing, model.Phone)
            om.UpdateOrderBillingAddress(o.ID, model.BillingAddress, model.BillingCity, model.BillingState, model.BillingCountry, model.BillingPinCode)
            om.UpdateOrderShippingAddress(o.ID, model.ShippingAddress, model.ShippingCity, model.ShippingState, model.ShippingCountry, model.ShippingPincode, model.ShippingName, "", model.ShippingPhone)
            Return RedirectToAction("Checkout")
        End Function

        Function Address() As ActionResult
            Dim o As Order = om.GetCart()
            If o.ID = 0 Then
                Return RedirectToAction("Index")
            End If
            Return View(New OrderAddressDTO(o))
        End Function

        Function Checkout() As ActionResult
            Dim o As Order = om.GetCart()
            If o.ID = 0 Then
                Return RedirectToAction("Index")
            End If
            Return View(o)
        End Function
    End Class
End Namespace