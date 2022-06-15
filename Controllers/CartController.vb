Imports System.Security.Cryptography
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
            Dim memberid As Long? = Nothing
            If Session("member") IsNot Nothing Then
                memberid = CType(Session("member"), Member).ID
            End If
            Dim p = db.Products.FirstOrDefault(Function(t) t.ID = id)
            If p IsNot Nothing Then
                Dim o As Order = om.GetCart()
                If o.ID = 0 Then
                    o = om.Create(String.Empty, String.Empty, memberid, String.Empty, String.Empty, String.Empty, String.Empty, "India", String.Empty, String.Empty, String.Empty, String.Empty, "India", String.Empty, String.Empty, OrderStatusType.[New], String.Empty, String.Empty, DateTime.Now, 0, 0, 0, 0, 0, 0, 40.0, "", "", "", "", "")
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

        Function UpdateContact(ByVal name As String, ByVal email As String, ByVal phone As String) As ActionResult
            Dim o As Order = om.GetCart()
            om.UpdateOrderContact(o.ID, name, email, Nothing, phone)
            Return RedirectToAction("Index")
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function Address(ByVal model As OrderAddressDTO) As ActionResult
            If Not ModelState.IsValid Then
                Return View(model)
            End If
            Dim o As Order = om.GetCart()
            Dim m As Member = db.Members.FirstOrDefault(Function(item) item.Email = model.Email)
            om.UpdateOrderContact(o.ID, model.Name, model.Email, IIf(m Is Nothing, Nothing, m.ID), model.Phone)
            om.UpdateOrderBillingAddress(o.ID, model.BillingAddress, model.BillingCity, model.BillingState, model.BillingCountry, model.BillingPinCode)
            om.UpdateOrderShippingAddress(o.ID, model.ShippingAddress, model.ShippingCity, model.ShippingState, model.ShippingCountry, model.ShippingPincode, model.ShippingName, "", model.ShippingPhone)
            om.UpdateTotal(o.ID)
            Return RedirectToAction("Checkout")
        End Function

        Function Address() As ActionResult
            Dim o As Order = om.GetCart()
            If o.ID = 0 Then
                Return RedirectToAction("Index")
            End If
            Return View(New OrderAddressDTO(o))
        End Function

        Function PayuMoneyResponse() As ActionResult
            Try
                Dim merc_hash_vars_seq As String()
                Dim merc_hash_string As String = String.Empty
                Dim merc_hash As String = String.Empty
                Dim order_id As String = String.Empty
                Dim hash_seq As String = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10"

                If Request.Form("status") = "success" Then
                    merc_hash_vars_seq = hash_seq.Split("|"c)
                    Array.Reverse(merc_hash_vars_seq)
                    merc_hash_string = ConfigurationManager.AppSettings("SALT") & "|" + Request.Form("status")

                    For Each merc_hash_var As String In merc_hash_vars_seq
                        merc_hash_string += "|"
                        merc_hash_string &= (If(Request.Form(merc_hash_var), ""))
                    Next

                    Response.Write(merc_hash_string)
                    merc_hash = Generatehash512(merc_hash_string).ToLower()

                    If merc_hash <> Request.Form("hash") Then
                        Response.Write("Hash value did not matched")
                    Else
                        order_id = Request.Form("txnid")
                        Response.Write("value matched")

                        If Request.Form("mihpayid") IsNot Nothing Then
                            Dim payumoneytransid As String = Request.Form("mihpayid")
                            Dim paymentmode As String = Request.Form("mode")
                            Dim status As String = Request.Form("status")
                            Dim payuMoneyId As String = Request.Form("payuMoneyId")
                            Dim array = (From key In Request.Form.AllKeys From value In Request.Form.GetValues(key) Select String.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value))).ToArray()
                            Dim payumoneydata As String = String.Join("&", array)
                            Dim om As New OrderManager()
                            Dim o As Order = om.GetOrderDetail(Integer.Parse(order_id))

                            If o IsNot Nothing Then
                                om.UpdateOrderPayment(o.ID, payumoneytransid, DateTime.Now, payumoneydata)
                                Dim notes As String = o.ShippingNotes
                                om.UpdateOrderStatus(o.ID, OrderStatusType.CardPaid, notes)

                                Try
                                    Dim body As String = om.GenerateReceipt(o.ID)
                                    Dim eman As New EmailManager()
                                    eman.SendMail(Utility.NewsletterEmail, o.Email, Utility.AdminName, o.Name, body, String.Format("Order Receipt : {0} from {1}", o.ID, Utility.SiteName), EmailMessageType.Communication, "Order Receipt")
                                    eman.SendMail(Utility.NewsletterEmail, "preeti.singh@rudrasofttech.com", Utility.AdminName, "Preeti Singh", body, String.Format("Order Receipt : {0} from {1}", o.ID, Utility.SiteName), EmailMessageType.Communication, "Order Receipt")
                                    eman.SendMail(Utility.NewsletterEmail, "rajkiran.singh@rudrasofttech.com", Utility.AdminName, "Raj Kiran Singh", body, String.Format("Order Receipt : {0} from {1}", o.ID, Utility.SiteName), EmailMessageType.Communication, "Order Receipt")
                                Catch ex As Exception
                                    Trace.Write(ex.Message)
                                End Try

                                CookieWorker.RemoveCookie(CookieWorker.OrderIdKey)
                                o = om.GetOrderDetail(Integer.Parse(order_id))
                                Response.Redirect("~/orders/detail/" & o.ID)
                            End If
                        End If
                    End If
                Else
                    Response.Write("Hash value did not matched")
                End If

            Catch ex As Exception
                Response.Write("<span style='color:red'>" & ex.Message & "</span>")
            End Try

            Return View()
        End Function

        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function COD() As ActionResult
            Try

                Dim om As New OrderManager()
                Dim o As Order = om.GetCart()

                If o IsNot Nothing Then
                    om.UpdatePaymentMode(o.ID, "COD")
                    om.UpdateTotal(o.ID)
                    om.UpdateOrderPayment(o.ID, "COD", Date.Now, "COD")
                    Dim notes As String = o.ShippingNotes
                    om.UpdateOrderStatus(o.ID, OrderStatusType.CODPaid, notes)

                    Try
                        Dim body As String = om.GenerateReceipt(o.ID)
                        Dim eman As New EmailManager()
                        eman.SendMail(Utility.NewsletterEmail, o.Email, Utility.AdminName, o.Name, body, String.Format("Order Receipt : {0} from {1}", o.ID, Utility.SiteName), EmailMessageType.Communication, "Order Receipt")
                        eman.SendMail(Utility.NewsletterEmail, "preeti.singh@rudrasofttech.com", Utility.AdminName, "Preeti Singh", body, String.Format("Order Receipt : {0} from {1}", o.ID, Utility.SiteName), EmailMessageType.Communication, "Order Receipt")
                        eman.SendMail(Utility.NewsletterEmail, "rajkiran.singh@rudrasofttech.com", Utility.AdminName, "Raj Kiran Singh", body, String.Format("Order Receipt : {0} from {1}", o.ID, Utility.SiteName), EmailMessageType.Communication, "Order Receipt")
                    Catch ex As Exception
                        Trace.Write(ex.Message)
                    End Try

                    CookieWorker.RemoveCookie(CookieWorker.OrderIdKey)

                    Response.Redirect("~/orders/detail/" & o.ID)
                End If
            Catch ex As Exception
                Response.Write("<span style='color:red'>" & ex.Message & "</span>")
            End Try

            Return View()
        End Function

        Function Checkout() As ActionResult
            Dim o As Order = om.GetCart()
            If o.ID = 0 Then
                Return RedirectToAction("Index")
            End If
            Dim hashVarsSeq() As String = ConfigurationManager.AppSettings("hashSequence").Split("|".ToCharArray())
            Dim hash_string As New StringBuilder()
            For Each hash_var In hashVarsSeq
                If hash_var = "key" Then
                    hash_string.Append(ConfigurationManager.AppSettings("MERCHANT_KEY"))
                    hash_string.Append("|")
                ElseIf hash_var = "txnid" Then
                    hash_string.Append(o.ID.ToString())
                    hash_string.Append("|")
                ElseIf hash_var = "amount" Then
                    hash_string.Append(o.Total.ToString("g29"))
                    hash_string.Append("|")
                ElseIf hash_var = "productinfo" Then
                    hash_string.Append("IndiaBobblesProducts")
                    hash_string.Append("|")
                ElseIf hash_var = "firstname" Then
                    hash_string.Append(o.Name)
                    hash_string.Append("|")
                ElseIf hash_var = "email" Then
                    hash_string.Append(o.Email)
                    hash_string.Append("|")
                Else
                    hash_string.Append(IIf(String.IsNullOrEmpty(Request.Form(hash_var)), "", Request.Form(hash_var)))
                    hash_string.Append("|")
                End If
            Next
            hash_string.Append(ConfigurationManager.AppSettings("SALT"))
            ViewBag.hash1 = Generatehash512(hash_string.ToString()).ToLower()
            ViewBag.action1 = ConfigurationManager.AppSettings("PAYU_BASE_URL") & "/_payment"
            ViewBag.key = ConfigurationManager.AppSettings("MERCHANT_KEY")
            Return View(o)
        End Function

        Public Function Generatehash512(ByVal text As String) As String
            Dim message As Byte() = Encoding.UTF8.GetBytes(text)
            Dim UE As New UnicodeEncoding()
            Dim hashValue As Byte()
            Dim hashString As New SHA512Managed()
            Dim hex As String = ""
            hashValue = hashString.ComputeHash(message)

            For Each x As Byte In hashValue
                hex += String.Format("{0:x2}", x)
            Next

            Return hex
        End Function
    End Class
End Namespace