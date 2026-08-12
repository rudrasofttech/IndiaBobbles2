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

        ' POST: /product/notifyme
        <HttpPost>
        <ValidateAntiForgeryToken>
        Function NotifyMe(ByVal productId As Integer, ByVal email As String, ByVal honeypot As String) As JsonResult
            Try
                ' Honeypot: bots fill hidden fields, humans don't
                If Not String.IsNullOrEmpty(honeypot) Then
                    Return Json(New With {.success = True, .message = "Thank you! We will notify you when this item is back in stock."})
                End If

                ' Basic email validation
                If String.IsNullOrWhiteSpace(email) OrElse Not System.Text.RegularExpressions.Regex.IsMatch(email.Trim(), "^[^@\s]+@[^@\s]+\.[^@\s]+$") Then
                    Return Json(New With {.success = False, .message = "Please enter a valid email address."})
                End If

                ' Limit email length to prevent injection
                If email.Length > 250 Then
                    Return Json(New With {.success = False, .message = "Please enter a valid email address."})
                End If

                email = email.Trim().ToLower()

                ' Rate limit: max 3 notify-me requests per IP per hour
                Dim ipAddress As String = Request.UserHostAddress
                Dim cacheKey As String = String.Format("notifyme_ip_{0}", ipAddress)
                Dim cachedValue = HttpContext.Cache(cacheKey)
                Dim requestCount As Integer = If(cachedValue IsNot Nothing, CInt(cachedValue), 0)

                If requestCount >= 3 Then
                    Return Json(New With {.success = False, .message = "Too many requests. Please try again later."})
                End If

                ' Increment rate limit counter, expires in 1 hour
                HttpContext.Cache.Insert(cacheKey, requestCount + 1, Nothing, DateTime.Now.AddHours(1), System.Web.Caching.Cache.NoSlidingExpiration)

                ' Prevent same email submitting for same product multiple times
                Dim dupCacheKey As String = String.Format("notifyme_{0}_{1}", productId, email)
                If HttpContext.Cache(dupCacheKey) IsNot Nothing Then
                    Return Json(New With {.success = True, .message = "Thank you! We will notify you when this item is back in stock."})
                End If

                HttpContext.Cache.Insert(dupCacheKey, True, Nothing, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration)

                Dim product = db.Products.FirstOrDefault(Function(t) t.ID = productId)
                If product Is Nothing Then
                    Return Json(New With {.success = False, .message = "Product not found."})
                End If

                ' Only allow notify requests for actually out-of-stock products
                If Not product.OutofStock Then
                    Return Json(New With {.success = False, .message = "This product is currently in stock."})
                End If

                Dim safeEmail = System.Web.HttpUtility.HtmlEncode(email)
                Dim safeProductName = System.Web.HttpUtility.HtmlEncode(product.Name)

                Dim body As String = String.Format(
                    "<p>Hi,</p><p>A customer with email <strong>{0}</strong> has requested to be notified when <strong>{1}</strong> is back in stock.</p>",
                    safeEmail, safeProductName)

                Dim subject As String = String.Format("Back in Stock Request: {0}", safeProductName)

                Dim eman As New EmailManager()
                eman.SendMail(Utility.NewsletterEmail, "IB@rudrasofttech.com", Utility.AdminName, "Raj Kiran Singh", body, subject, EmailMessageType.Communication, "Back in Stock")

                Dim customerBody As String = String.Format(
                    "<p>Hi,</p><p>Thank you for your interest in <strong>{0}</strong>. We will notify you as soon as it is back in stock.</p>",
                    safeProductName)
                eman.SendMail(Utility.NewsletterEmail, email, Utility.AdminName, email, customerBody,
                              String.Format("We noted your interest in {0}", safeProductName),
                              EmailMessageType.Communication, "Back in Stock")

                Return Json(New With {.success = True, .message = "Thank you! We will notify you when this item is back in stock."})
            Catch ex As Exception
                Trace.Write(ex.Message)
                Trace.Write(ex.StackTrace)
                Return Json(New With {.success = False, .message = "Something went wrong. Please try again."})
            End Try
        End Function
    End Class
End Namespace