Public Class OrderManager
    Public ShippingPriceConstant As Decimal = 0
    Public FreeShippingAmount As Decimal = 500
    Public CODFee As Decimal = 50
    Private ReadOnly dc As New indiabobblesEntities

    Public Sub New()
    End Sub

    Public Function GetCart() As Order
        Dim o As Order = Nothing
        Dim cartid As Integer

        If Not Integer.TryParse(CookieWorker.GetCookie(CookieWorker.OrderIdKey, "cartid"), cartid) Then
            o = New [Order] With {
                .OrderItems = New List(Of OrderItem)
            }
            'o = Create(String.Empty, String.Empty, Nothing, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, OrderStatusType.[New], String.Empty, String.Empty, DateTime.UtcNow, 0, 0, 0, 0, 0, 0, CODFee, "", "", "", "", "")
            'CookieWorker.SetCookie(CookieWorker.OrderIdKey, "cartid", o.ID.ToString(), DateTime.UtcNow.AddHours(50))
        Else
            o = dc.Orders.SingleOrDefault(Function(item) item.ID = cartid)
        End If
        If o Is Nothing Then
            o = New [Order] With {
                .OrderItems = New List(Of OrderItem)
            }
        End If
        Return o
    End Function

    Public Function GetCartItemCount() As Integer
        Return GetCart().OrderItems.Count
    End Function

    Public Function Create(ByVal name As String, ByVal email As String, ByVal memberid As Long?, ByVal phone As String, ByVal billingAddress As String, ByVal billingCity As String, ByVal billingState As String, ByVal billingCountry As String, ByVal billingZip As String, ByVal shippingAddress As String, ByVal shippingCity As String, ByVal shippingState As String, ByVal shippingCountry As String, ByVal shippinZip As String, ByVal coupon As String, ByVal status As OrderStatusType, ByVal trackCode As String, ByVal shippingNotes As String, ByVal modified As DateTime, ByVal amount As Decimal, ByVal tax As Decimal, ByVal taxPercentage As Decimal, ByVal discount As Decimal, ByVal total As Decimal, ByVal shippingPrice As Decimal, ByVal cod As Decimal, ByVal paymentMode As String, ByVal shippingService As String, ByVal shippingFirstName As String, ByVal shippingLastName As String, ByVal shippingPhone As String) As Order
        Dim o As New Order() With {
            .Amount = amount,
            .BillingAddress = billingAddress,
            .BillingCity = billingCity,
            .BillingCountry = billingCountry,
            .BillingState = billingState,
            .BillingZip = billingZip,
            .Coupon = coupon,
            .DateCreated = DateTime.UtcNow,
            .DateModified = modified,
            .Discount = discount,
            .Email = email,
            .MemberID = memberid,
            .Name = name,
            .Phone = phone,
            .ShippingAddress = shippingAddress,
            .ShippingCity = shippingCity,
            .ShippingCountry = shippingCountry,
            .ShippingNotes = shippingNotes,
            .ShippingState = shippingState,
            .ShippingTrackCode = trackCode,
            .ShippingZip = shippinZip,
            .Status = CByte(status),
            .Tax = tax,
            .TaxPercentage = taxPercentage,
            .Total = total,
            .TransactionCode = String.Empty,
            .TransactionDetail = String.Empty,
            .ShippingPrice = shippingPrice,
            .COD = cod,
            .PaymentMode = paymentMode,
            .ShippingService = shippingService,
            .ShippingFirstName = shippingFirstName,
            .ShippingLastName = shippingLastName,
            .ShippingPhone = shippingPhone
        }
        dc.Orders.Add(o)
        dc.SaveChanges()
        Return o
    End Function

    Public Sub AddItem(ByVal quantity As Integer, ByVal orderId As Integer, ByVal productImg As String, ByVal productName As String, ByVal productCode As String, ByVal price As Decimal)

        Dim oi As OrderItem = dc.OrderItems.SingleOrDefault(Function(item) item.OrderID = orderId AndAlso item.ProductCode = productCode)

        If oi Is Nothing Then
            oi = New OrderItem With {
                .OrderID = orderId,
                .Price = price,
                .ProductCode = productCode,
                .ProductImg = productImg,
                .ProductName = productName,
                .Quantity = quantity,
                .Amount = price * quantity
            }
            dc.OrderItems.Add(oi)
        Else
            oi.Price = price
            oi.Quantity += quantity
            oi.Amount = oi.Quantity * oi.Price

            If oi.Quantity <= 0 Then
                dc.OrderItems.Remove(oi)
            End If
        End If

        dc.SaveChanges()
        Dim list = From item In dc.OrderItems Where item.OrderID = orderId Select item
        Dim amount As Decimal = 0

        For Each item In list
            amount += item.Amount
        Next

        Dim o As Order = dc.Orders.SingleOrDefault(Function(item) item.ID = orderId)
        o.Amount = amount

        If o.Amount < FreeShippingAmount Then
            o.ShippingPrice = ShippingPriceConstant
        Else
            o.ShippingPrice = 0
        End If

        o.Total = o.Amount + o.ShippingPrice - o.Discount
        dc.SaveChanges()
    End Sub

    Public Sub RemoveItem(ByVal itemId As Integer, ByVal orderId As Integer)

        Dim oi As OrderItem = dc.OrderItems.SingleOrDefault(Function(item) item.ID = itemId AndAlso item.OrderID = orderId)
        dc.OrderItems.Remove(oi)
        dc.SaveChanges()
        Dim list = From item In dc.OrderItems Where item.OrderID = orderId Select item
        Dim amount As Decimal = 0

        For Each item In list
            amount += item.Amount
        Next

        Dim o As Order = dc.Orders.SingleOrDefault(Function(item) item.ID = orderId)
        o.Amount = amount

        If o.Amount = 0 Then
            o.Coupon = ""
            o.Discount = 0
            o.ShippingPrice = 0
            o.COD = 0
            o.PaymentMode = ""
        End If

        o.Total = o.Amount + o.ShippingPrice - o.Discount
        dc.SaveChanges()
    End Sub

    Public Sub AddItemQuantity(ByVal itemId As Integer, ByVal orderId As Integer)

        Dim oi As OrderItem = dc.OrderItems.SingleOrDefault(Function(item) item.ID = itemId AndAlso item.OrderID = orderId)
        oi.Quantity += 1
        oi.Amount = oi.Quantity * oi.Price
        dc.SaveChanges()
        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        Dim amount As Decimal = 0

        For Each i As OrderItem In o.OrderItems
            amount += i.Amount
        Next

        o.Amount = amount

        If o.Amount = 0 Then
            o.Coupon = ""
            o.Discount = 0
            o.ShippingPrice = 0
            o.COD = 0
            o.PaymentMode = ""
        End If

        dc.SaveChanges()
    End Sub

    Public Sub ReduceItemQuantity(ByVal itemId As Integer, ByVal orderId As Integer)
        Dim oi As OrderItem = dc.OrderItems.SingleOrDefault(Function(item) item.ID = itemId AndAlso item.OrderID = orderId)
        oi.Quantity -= 1
        oi.Amount = oi.Quantity * oi.Price
        dc.SaveChanges()

        If oi.Quantity = 0 Then
            dc.OrderItems.Remove(oi)
            dc.SaveChanges()
        End If

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        Dim amount As Decimal = 0

        For Each i As OrderItem In o.OrderItems
            amount += i.Amount
        Next

        o.Amount = amount

        If o.Amount = 0 Then
            o.Coupon = ""
            o.Discount = 0
            o.ShippingPrice = 0
            o.COD = 0
            o.PaymentMode = ""
        End If

        dc.SaveChanges()
    End Sub

    Public Sub UpdateItem(ByVal itemId As Integer, ByVal quantity As Integer, ByVal orderId As Integer, ByVal productImg As String, ByVal productName As String, ByVal productCode As String, ByVal price As Decimal)
        Dim oi As OrderItem = dc.OrderItems.SingleOrDefault(Function(item) item.ID = itemId AndAlso item.OrderID = orderId)
        oi.Price = price
        oi.ProductCode = productCode
        oi.ProductImg = productImg
        oi.ProductName = productName
        oi.Quantity = quantity
        oi.Amount = price * quantity
        dc.SaveChanges()

    End Sub

    Public Sub DeleteOrder(ByVal orderId As Integer)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        dc.OrderItems.RemoveRange(o.OrderItems)
        dc.Orders.Remove(o)
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrder(ByVal orderId As Integer, ByVal name As String, ByVal email As String, ByVal memberid As Long?, ByVal phone As String, ByVal billingAddress As String, ByVal billingCity As String, ByVal billingState As String, ByVal billingCountry As String, ByVal billingZip As String, ByVal shippingAddress As String, ByVal shippingCity As String, ByVal shippingState As String, ByVal shippingCountry As String, ByVal shippinZip As String, ByVal coupon As String, ByVal status As OrderStatusType, ByVal trackCode As String, ByVal shippingNotes As String, ByVal amount As Decimal, ByVal tax As Decimal, ByVal taxPercentage As Decimal, ByVal discount As Decimal, ByVal total As Decimal, ByVal shippingPrice As Decimal, ByVal cod As Decimal)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.BillingAddress = billingAddress
        o.BillingCity = billingCity
        o.BillingCountry = billingCountry
        o.BillingState = billingState
        o.BillingZip = billingZip
        o.Coupon = coupon
        o.DateModified = DateTime.UtcNow
        o.Discount = discount
        o.Email = email
        o.MemberID = memberid
        o.Name = name
        o.Phone = phone
        o.ShippingAddress = shippingAddress
        o.ShippingCity = shippingCity
        o.ShippingCountry = shippingCountry
        o.ShippingNotes = shippingNotes
        o.ShippingState = shippingState
        o.ShippingTrackCode = trackCode
        o.ShippingZip = shippinZip
        o.Status = CByte(status)
        o.Tax = tax
        o.TaxPercentage = taxPercentage
        o.Total = total
        o.Amount = amount
        o.ShippingPrice = shippingPrice
        o.COD = cod
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderBillingAddress(ByVal orderId As Integer, ByVal billingAddress As String, ByVal billingCity As String, ByVal billingState As String, ByVal billingCountry As String, ByVal billingZip As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.BillingAddress = billingAddress
        o.BillingCity = billingCity
        o.BillingCountry = billingCountry
        o.BillingState = billingState
        o.BillingZip = billingZip
        o.DateModified = DateTime.UtcNow
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderShippingAddress(ByVal orderId As Integer, ByVal shippingAddress As String, ByVal shippingCity As String, ByVal shippingState As String, ByVal shippingCountry As String, ByVal shippingZip As String, ByVal shippingFirstName As String, ByVal shippingLastName As String, ByVal shippingPhone As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.ShippingAddress = shippingAddress
        o.ShippingCity = shippingCity
        o.ShippingCountry = shippingCountry
        o.ShippingState = shippingState
        o.ShippingZip = shippingZip
        o.DateModified = DateTime.UtcNow
        o.ShippingFirstName = shippingFirstName
        o.ShippingLastName = shippingLastName
        o.ShippingPhone = shippingPhone
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderContact(ByVal orderId As Integer, ByVal name As String, ByVal email As String, ByVal memberid As Long?, ByVal phone As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.Name = name
        o.Email = email
        o.MemberID = memberid
        o.Phone = phone
        o.DateModified = DateTime.UtcNow
        dc.SaveChanges()

    End Sub

    Public Sub UpdateCOD(ByVal orderId As Integer)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)

        If o.PaymentMode = "COD" Then
            o.COD = 0
            'If (o.Amount + o.ShippingPrice + o.Tax - o.Discount) > 2000 Then
            '    o.COD = ((o.Amount + o.ShippingPrice + o.Tax - o.Discount) / 100) * 2
            'Else
            '    o.COD = 40
            'End If
        Else
            o.COD = 0
        End If

        o.DateModified = DateTime.UtcNow
        dc.SaveChanges()

    End Sub

    Public Sub UpdateShippingPrice(ByVal orderId As Integer)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        Dim quantity As Integer = 0

        For Each item As OrderItem In o.OrderItems
            quantity += item.Quantity
        Next

        If o.ShippingState.ToLower() = "jammu and kashmir" OrElse o.ShippingState.ToLower() = "andaman and nicobar islands" OrElse o.ShippingState.ToLower() = "lakshadweep" Then
            o.ShippingPrice = quantity * 200
        ElseIf o.ShippingState.ToLower() = "national capital territory of delhi" Then
            o.ShippingPrice = quantity * 0
        Else
            o.ShippingPrice = 0
        End If

        o.DateModified = DateTime.UtcNow
        dc.SaveChanges()

    End Sub

    Public Sub UpdateTotal(ByVal orderId As Integer)
        UpdateShippingPrice(orderId)
        UpdateCOD(orderId)


        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.Total = o.Amount + o.ShippingPrice + o.COD + o.Tax - o.Discount
        o.DateModified = DateTime.UtcNow
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderShippingService(ByVal orderId As Integer, ByVal shippingservice As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.ShippingService = shippingservice
        o.DateModified = DateTime.UtcNow
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderShippingCode(ByVal orderId As Integer, ByVal trackingCode As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.ShippingTrackCode = trackingCode
        o.DateModified = DateTime.UtcNow
        dc.SaveChanges()

    End Sub

    Public Sub UpdateCouponCode(ByVal orderId As Integer, ByVal coupon As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.Coupon = String.Empty
        o.Discount = 0
        o.DateModified = DateTime.UtcNow
        Dim coupons As List(Of CouponCode) = dc.CouponCodes.Where(Function(t) t.Status = CByte(GeneralStatusType.Active)).ToList()

        For Each cc As CouponCode In coupons

            If coupon.ToLower() = cc.Name.ToLower().Trim() Then
                If cc.IsPercent Then
                    o.Discount = (o.Amount * cc.Value) / 100
                Else
                    o.Discount = cc.Value
                End If
                o.Coupon = cc.Name
                o.DateModified = DateTime.UtcNow
                Exit For
            End If
        Next

        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderShippingNotes(ByVal orderId As Integer, ByVal shippingNotes As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.ShippingNotes = shippingNotes
        o.DateModified = DateTime.UtcNow
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderStatus(ByVal orderId As Integer, ByVal status As OrderStatusType, ByVal notes As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.Status = CByte(status)
        o.ShippingNotes = notes
        o.DateModified = DateTime.UtcNow
        dc.SaveChanges()

    End Sub

    Public Sub UpdateShippingTrackingCode(ByVal orderId As Integer, ByVal trackingcode As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.ShippingTrackCode = trackingcode
        o.DateModified = DateTime.UtcNow
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderPayment(ByVal orderId As Integer, ByVal transactionCode As String, ByVal transactionDate As DateTime, ByVal transactionDetail As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.TransactionCode = transactionCode
        o.TransactionDate = transactionDate
        o.TransactionDetail = transactionDetail

        o.DateModified = DateTime.UtcNow
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderTransactionDetail(ByVal orderId As Integer, ByVal transactionDetail As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.TransactionDetail = transactionDetail
        o.DateModified = DateTime.UtcNow
        dc.SaveChanges()

    End Sub


    Public Function GetOrderDetail(ByVal orderId As Integer) As Order

        Return dc.Orders.Include("OrderItems").SingleOrDefault(Function(item) item.ID = orderId)

    End Function

    Public Function GetOrderList(ByVal keyword As String) As List(Of Order)

        Dim orderid As Integer

        If Integer.TryParse(keyword, orderid) Then
            Dim query = From d In dc.Orders Where d.ID = orderid Order By d.ID Select d
            Return query.ToList()
        Else
            Dim query = From d In dc.Orders Where d.Phone = keyword OrElse d.Email = keyword Order By d.ID Select d
            Return query.ToList()
        End If

    End Function

    ' =====================================================================
    '  GenerateReceipt – branded, mobile-friendly HTML email receipt
    '  • Email-safe: tables + inline styles only (works in Gmail, Outlook, Apple Mail)
    '  • All customer-entered text is HTML-encoded
    '  • Same signature as before: Public Function GenerateReceipt(orderId) As String
    ' =====================================================================
    Public Function GenerateReceipt(ByVal orderId As Integer) As String
        Dim o As Order = dc.Orders.SingleOrDefault(Function(item) item.ID = orderId)
        If o Is Nothing Then Return String.Empty

        Dim site As String = "https://www.indiabobbles.com"
        Dim inr As New System.Globalization.CultureInfo("en-IN")
        Dim enc = Function(s As Object) System.Net.WebUtility.HtmlEncode(If(s, "").ToString())
        Dim money = Function(d As Decimal) "&#8377;" & d.ToString("#,##0.00", inr)
        Dim absUrl = Function(p As String) As String
                         If String.IsNullOrEmpty(p) Then Return ""
                         If p.StartsWith("http", StringComparison.OrdinalIgnoreCase) Then Return p
                         If p.StartsWith("//") Then Return "https:" & p
                         p = p.TrimStart("~"c)
                         Return site & If(p.StartsWith("/"), "", "/") & p
                     End Function

        ' Friendly status
        Dim statusText As String = "Order placed"
        Dim statusBg As String = "#eef0f3", statusFg As String = "#4b5563"
        Select Case o.Status
            Case 1 : statusText = "Awaiting payment"
            Case 2 : statusText = "Being prepared" : statusBg = "#e8f0fe" : statusFg = "#1d4ed8"
            Case 3 : statusText = "Paid" : statusBg = "#fff4cc" : statusFg = "#330B3F"
            Case 4 : statusText = "Cash on delivery" : statusBg = "#fff4cc" : statusFg = "#330B3F"
            Case 5 : statusText = "Shipped" : statusBg = "#e0f2fe" : statusFg = "#0369a1"
            Case 6 : statusText = "Delivered" : statusBg = "#e7f6ec" : statusFg = "#157a3c"
            Case 7 : statusText = "Refunded" : statusBg = "#f3e8ff" : statusFg = "#7e22ce"
            Case 8 : statusText = "Cancelled" : statusBg = "#fdecee" : statusFg = "#b4232f"
        End Select

        Dim firstName As String = If(String.IsNullOrWhiteSpace(o.Name), "there", o.Name.Trim().Split(" "c)(0))
        Dim shipName As String = Trim(If(o.ShippingFirstName, "") & " " & If(o.ShippingLastName, ""))
        Dim font As String = "font-family:Arial,Helvetica,sans-serif;"
        Dim muted As String = "color:#6b5f72;"
        Dim b As New StringBuilder()

        ' ---------- Wrapper ----------
        b.Append("<!DOCTYPE html><html><head><meta charset='utf-8'><meta name='viewport' content='width=device-width,initial-scale=1'>")
        b.AppendFormat("<title>India Bobbles – Order #{0}</title></head>", o.ID)
        b.AppendFormat("<body style='margin:0;padding:0;background:#f4f1f6;{0}'>", font)
        ' Preheader (inbox preview text)
        b.AppendFormat("<div style='display:none;max-height:0;overflow:hidden;'>Your India Bobbles order #{0} – {1}. Total {2}.</div>", o.ID, statusText, money(o.Total))

        b.Append("<table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='background:#f4f1f6;'><tr><td align='center' style='padding:24px 12px;'>")
        b.Append("<table role='presentation' width='600' cellpadding='0' cellspacing='0' style='width:100%;max-width:600px;background:#ffffff;border-radius:14px;overflow:hidden;'>")

        ' ---------- Header ----------
        b.Append("<tr><td style='background:#ffc107;padding:18px 24px;' align='left'>")
        b.AppendFormat("<a href='{0}'><img src='{0}/theme/khichdi/img/ib-logo.png' alt='India Bobbles' height='44' style='display:block;border:0;height:44px;'></a>", site)
        b.Append("</td></tr>")

        ' ---------- Hero ----------
        b.Append("<tr><td style='background:#330B3F;padding:26px 24px;color:#ffffff;'>")
        b.AppendFormat("<div style='font-size:22px;font-weight:bold;margin-bottom:6px;'>Thank you, {0}! &#127881;</div>", enc(firstName))
        b.Append("<div style='font-size:14px;color:#e2d3ea;line-height:1.5;'>Here's your receipt. Every piece is hand-painted by our artists and packed with care.</div>")
        b.Append("</td></tr>")

        ' ---------- Order meta ----------
        b.Append("<tr><td style='padding:20px 24px 8px;'>")
        b.Append("<table role='presentation' width='100%' cellpadding='0' cellspacing='0'><tr>")
        b.AppendFormat("<td style='font-size:13px;{0}'>Order number<br><span style='font-size:22px;font-weight:bold;color:#330B3F;'>#{1}</span></td>", muted, o.ID)
        b.AppendFormat("<td align='right' valign='top'><span style='display:inline-block;background:{0};color:{1};font-size:12px;font-weight:bold;padding:6px 12px;border-radius:20px;'>{2}</span></td>", statusBg, statusFg, statusText)
        b.Append("</tr></table>")

        b.AppendFormat("<div style='font-size:13px;{0}margin-top:8px;line-height:1.7;'>", muted)
        b.AppendFormat("Order date: <b style='color:#1f1724;'>{0}</b>", o.DateCreated.ToString("d MMM yyyy"))
        If Not String.IsNullOrEmpty(o.PaymentMode) Then b.AppendFormat(" &nbsp;·&nbsp; Payment: <b style='color:#1f1724;'>{0}</b>", enc(o.PaymentMode))
        If Not String.IsNullOrEmpty(o.TransactionCode) Then b.AppendFormat("<br>Transaction: <b style='color:#1f1724;'>{0}</b>", enc(o.TransactionCode))
        If o.TransactionDate.HasValue Then b.AppendFormat(" &nbsp;·&nbsp; {0}", o.TransactionDate.Value.ToString("d MMM yyyy, h:mm tt"))
        b.Append("</div>")

        ' Tracking box
        If Not String.IsNullOrEmpty(o.ShippingTrackCode) Then
            b.Append("<div style='background:#e0f2fe;color:#0369a1;border-radius:10px;padding:12px 14px;margin-top:14px;font-size:14px;'>&#128666; ")
            If Not String.IsNullOrEmpty(o.ShippingService) Then b.AppendFormat("Shipped via <b>{0}</b> · ", enc(o.ShippingService))
            b.AppendFormat("Tracking number: <b>{0}</b></div>", enc(o.ShippingTrackCode))
        End If
        b.Append("</td></tr>")

        ' ---------- Items ----------
        b.Append("<tr><td style='padding:12px 24px;'>")
        b.Append("<table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='border-top:2px solid #330B3F;'>")
        For Each oi As OrderItem In o.OrderItems
            Dim img As String = absUrl(oi.ProductImg)
            b.Append("<tr>")
            If img <> "" Then
                b.AppendFormat("<td width='64' style='padding:12px 12px 12px 0;border-bottom:1px solid #ece6ef;' valign='top'><img src='{0}' alt='' width='56' height='56' style='display:block;width:56px;height:56px;object-fit:contain;background:#faf8fb;border-radius:8px;border:0;'></td>", enc(img))
            End If
            b.AppendFormat("<td style='padding:12px 0;border-bottom:1px solid #ece6ef;font-size:14px;' valign='top'><b style='color:#1f1724;'>{0}</b>", enc(oi.ProductName))
            b.AppendFormat("<br><span style='font-size:12px;{0}'>{1}Qty {2} × {3}</span></td>",
                       muted, If(String.IsNullOrEmpty(oi.ProductCode), "", enc(oi.ProductCode) & " · "), oi.Quantity, money(oi.Price))
            b.AppendFormat("<td align='right' style='padding:12px 0;border-bottom:1px solid #ece6ef;font-size:14px;font-weight:bold;color:#330B3F;white-space:nowrap;' valign='top'>{0}</td>", money(oi.Amount))
            b.Append("</tr>")
        Next
        b.Append("</table>")

        ' ---------- Totals ----------
        Dim row = Function(label As String, value As String, style As String) _
        String.Format("<tr><td style='padding:5px 0;font-size:14px;{2}'>{0}</td><td align='right' style='padding:5px 0;font-size:14px;{2}'>{1}</td></tr>", label, value, style)

        b.Append("<table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='margin-top:10px;'>")
        b.Append(row("Subtotal", money(o.Amount), "color:#3e3445;"))
        If o.Discount > 0 Then
            b.Append(row("Discount" & If(String.IsNullOrEmpty(o.Coupon), "", " (" & enc(o.Coupon) & ")"), "&minus; " & money(o.Discount), "color:#157a3c;"))
        End If
        b.Append(row("Shipping", If(o.ShippingPrice = 0, "<b style='color:#157a3c;'>FREE</b>", money(o.ShippingPrice)), "color:#3e3445;"))
        If o.PaymentMode = "COD" Then b.Append(row("Cash on delivery fee", money(o.COD), "color:#3e3445;"))
        b.AppendFormat("<tr><td style='padding:12px 0 4px;border-top:1px solid #ece6ef;font-size:18px;font-weight:bold;color:#330B3F;'>Total</td><td align='right' style='padding:12px 0 4px;border-top:1px solid #ece6ef;font-size:18px;font-weight:bold;color:#330B3F;'>{0}</td></tr>", money(o.Total))
        b.AppendFormat("<tr><td colspan='2' style='font-size:11px;{0}'>Inclusive of all taxes</td></tr>", muted)
        b.Append("</table>")
        b.Append("</td></tr>")

        ' ---------- Addresses (2 columns, stack on narrow screens) ----------
        b.Append("<tr><td style='padding:12px 24px 4px;'>")
        b.Append("<table role='presentation' width='100%' cellpadding='0' cellspacing='0'><tr>")

        b.Append("<td valign='top' width='50%' style='padding:0 8px 12px 0;'><div style='background:#faf8fb;border-radius:10px;padding:14px;font-size:13px;line-height:1.6;color:#3e3445;'>")
        b.Append("<div style='font-weight:bold;color:#330B3F;margin-bottom:4px;'>&#128666; Delivery address</div>")
        b.AppendFormat("<b>{0}</b><br>{1}<br>{2}, {3} – {4}<br>{5}", enc(shipName), enc(o.ShippingAddress), enc(o.ShippingCity), enc(o.ShippingState), enc(o.ShippingZip), enc(o.ShippingCountry))
        If Not String.IsNullOrEmpty(o.ShippingPhone) Then b.AppendFormat("<br>&#128222; +91 {0}", enc(o.ShippingPhone))
        b.Append("</div></td>")

        b.Append("<td valign='top' width='50%' style='padding:0 0 12px 8px;'><div style='background:#faf8fb;border-radius:10px;padding:14px;font-size:13px;line-height:1.6;color:#3e3445;'>")
        b.Append("<div style='font-weight:bold;color:#330B3F;margin-bottom:4px;'>&#128179; Billing</div>")
        b.AppendFormat("<b>{0}</b><br>{1}<br>+91 {2}<br>{3}<br>{4}, {5} – {6}", enc(o.Name), enc(o.Email), enc(o.Phone), enc(o.BillingAddress), enc(o.BillingCity), enc(o.BillingState), enc(o.BillingZip))
        b.Append("</div></td>")

        b.Append("</tr></table></td></tr>")

        ' ---------- What's next ----------
        If o.Status = 3 OrElse o.Status = 4 OrElse o.Status = 2 Then
            b.Append("<tr><td style='padding:4px 24px 8px;'><div style='background:#fff4cc;border-radius:10px;padding:14px;font-size:13px;color:#330B3F;line-height:1.6;'>")
            b.Append("<b>What happens next?</b><br>We pack your order within 2 business days and email you the tracking number once it ships. Delivery usually takes 2–3 days to metros and up to 8–10 working days elsewhere.")
            b.Append("</div></td></tr>")
        End If

        ' ---------- Buttons ----------
        b.Append("<tr><td align='center' style='padding:16px 24px 8px;'>")
        b.AppendFormat("<a href='{0}/orders' style='display:inline-block;background:#330B3F;color:#ffffff;text-decoration:none;font-weight:bold;font-size:14px;padding:12px 22px;border-radius:8px;margin:4px;'>View my orders</a>", site)
        b.AppendFormat("<a href='{0}/tag/collectibles' style='display:inline-block;background:#ffc107;color:#330B3F;text-decoration:none;font-weight:bold;font-size:14px;padding:12px 22px;border-radius:8px;margin:4px;'>Shop more bobbleheads</a>", site)
        b.Append("</td></tr>")

        ' ---------- Help ----------
        b.AppendFormat("<tr><td style='padding:12px 24px 22px;font-size:13px;{0}line-height:1.7;' align='center'>", muted)
        b.AppendFormat("Questions? Reply to this email or write to <a href='mailto:indiabobbles@rudrasofttech.com?subject=Order%20%23{0}' style='color:#330B3F;font-weight:bold;'>indiabobbles@rudrasofttech.com</a><br>", o.ID)
        b.Append("or call <a href='tel:+919871500276' style='color:#330B3F;font-weight:bold;'>+91 98715 00276</a> (Mon–Sat, 10 AM–6 PM). Please mention your order number.<br>")
        b.AppendFormat("Damaged in transit? Email photos within 4 hours of delivery. <a href='{0}/shipping-policy' style='color:#330B3F;'>Shipping &amp; Returns</a>", site)
        b.Append("</td></tr>")

        ' ---------- Footer ----------
        b.Append("<tr><td style='background:#330B3F;padding:18px 24px;color:#cbb8d4;font-size:12px;line-height:1.6;' align='center'>")
        b.Append("<b style='color:#ffc107;'>India Bobbles</b> · Hand-painted Bollywood &amp; Indian bobbleheads<br>")
        b.Append("H104, Ajnara Daffodil, Sector 137, Noida, Uttar Pradesh – 201305<br>")
        b.AppendFormat("<a href='{0}' style='color:#ffc107;text-decoration:none;'>indiabobbles.com</a> &nbsp;·&nbsp; ", site)
        b.Append("<a href='https://www.instagram.com/indiabobbles' style='color:#ffc107;text-decoration:none;'>Instagram</a> &nbsp;·&nbsp; ")
        b.Append("<a href='https://www.facebook.com/IndiaBobbles' style='color:#ffc107;text-decoration:none;'>Facebook</a>")
        b.Append("</td></tr>")

        b.Append("</table></td></tr></table></body></html>")
        Return b.ToString()
    End Function

    Public Sub UpdatePaymentMode(ByVal orderId As Integer, ByVal paymentmode As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.PaymentMode = paymentmode
        o.DateModified = DateTime.UtcNow
        dc.SaveChanges()

    End Sub
End Class
