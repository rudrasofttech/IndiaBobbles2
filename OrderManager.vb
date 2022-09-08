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
            'o = Create(String.Empty, String.Empty, Nothing, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, OrderStatusType.[New], String.Empty, String.Empty, DateTime.Now, 0, 0, 0, 0, 0, 0, CODFee, "", "", "", "", "")
            'CookieWorker.SetCookie(CookieWorker.OrderIdKey, "cartid", o.ID.ToString(), DateTime.Now.AddHours(50))
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
            .DateCreated = DateTime.Now,
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
        o.DateModified = DateTime.Now
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
        o.DateModified = DateTime.Now
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderShippingAddress(ByVal orderId As Integer, ByVal shippingAddress As String, ByVal shippingCity As String, ByVal shippingState As String, ByVal shippingCountry As String, ByVal shippingZip As String, ByVal shippingFirstName As String, ByVal shippingLastName As String, ByVal shippingPhone As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.ShippingAddress = shippingAddress
        o.ShippingCity = shippingCity
        o.ShippingCountry = shippingCountry
        o.ShippingState = shippingState
        o.ShippingZip = shippingZip
        o.DateModified = DateTime.Now
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
        o.DateModified = DateTime.Now
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

        o.DateModified = DateTime.Now
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

        o.DateModified = DateTime.Now
        dc.SaveChanges()

    End Sub

    Public Sub UpdateTotal(ByVal orderId As Integer)
        UpdateShippingPrice(orderId)
        UpdateCOD(orderId)


        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.Total = o.Amount + o.ShippingPrice + o.COD + o.Tax - o.Discount
        o.DateModified = DateTime.Now
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderShippingService(ByVal orderId As Integer, ByVal shippingservice As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.ShippingService = shippingservice
        o.DateModified = DateTime.Now
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderShippingCode(ByVal orderId As Integer, ByVal trackingCode As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.ShippingTrackCode = trackingCode
        o.DateModified = DateTime.Now
        dc.SaveChanges()

    End Sub

    Public Sub UpdateCouponCode(ByVal orderId As Integer, ByVal coupon As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.Coupon = String.Empty
        o.Discount = 0
        o.DateModified = DateTime.Now
        Dim coupons As List(Of CouponCode) = dc.CouponCodes.Where(Function(t) t.Status = CByte(GeneralStatusType.Active)).ToList()

        For Each cc As CouponCode In coupons

            If coupon.ToLower() = cc.Name.ToLower().Trim() Then
                o.Coupon = cc.Name
                o.Discount = cc.Value
                o.DateModified = DateTime.Now
                Exit For
            End If
        Next

        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderShippingNotes(ByVal orderId As Integer, ByVal shippingNotes As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.ShippingNotes = shippingNotes
        o.DateModified = DateTime.Now
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderStatus(ByVal orderId As Integer, ByVal status As OrderStatusType, ByVal notes As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.Status = CByte(status)
        o.ShippingNotes = notes
        o.DateModified = DateTime.Now
        dc.SaveChanges()

    End Sub

    Public Sub UpdateShippingTrackingCode(ByVal orderId As Integer, ByVal trackingcode As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.ShippingTrackCode = trackingcode
        o.DateModified = DateTime.Now
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderPayment(ByVal orderId As Integer, ByVal transactionCode As String, ByVal transactionDate As DateTime, ByVal transactionDetail As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.TransactionCode = transactionCode
        o.TransactionDate = transactionDate
        o.TransactionDetail = transactionDetail

        o.DateModified = DateTime.Now
        dc.SaveChanges()

    End Sub

    Public Sub UpdateOrderTransactionDetail(ByVal orderId As Integer, ByVal transactionDetail As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.TransactionDetail = transactionDetail
        o.DateModified = DateTime.Now
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

    Public Function GenerateReceipt(ByVal orderId As Integer) As String
        Dim builder As New StringBuilder()


        Dim o As Order = dc.Orders.SingleOrDefault(Function(item) item.ID = orderId)
        builder.Append(String.Format("<h1>Order Number: {0}</h1>", o.ID))
        builder.Append("<table style='width:100%;'>")
        builder.Append("<thead><th style='text-align:left;'>Name</th><th style='text-align:left;'>Code</th><th style='text-align:left;'>Price</th><th style='text-align:left;'>Quantity</th><th style='text-align:left;'>Amount</th></tr></thead>")
        builder.Append("<tbody>")

        For Each oi As OrderItem In o.OrderItems
            builder.Append("<tr>")
            builder.Append(String.Format("<td style='text-align:left;'>{0}</td>", oi.ProductName))
            builder.Append(String.Format("<td style='text-align:left;'>{0}</td>", oi.ProductCode))
            builder.Append(String.Format("<td style='text-align:left;'>{0}</td>", oi.Price))
            builder.Append(String.Format("<td style='text-align:left;'>{0}</td>", oi.Quantity))
            builder.Append(String.Format("<td style='text-align:left;'>{0}</td>", oi.Amount))
            builder.Append("</tr>")
        Next

        builder.Append(String.Format("<tr><td colspan='4' align='right'>Amount</td><td>{0}</td></tr>", o.Amount.ToString("##00.00")))
        builder.Append(String.Format("<tr><td colspan='4' align='right'>Shipping</td><td>{0}</td></tr>", o.ShippingPrice.ToString("##00.00")))

        If o.Discount > 0 Then
            builder.Append(String.Format("<tr><td colspan='4' align='right'>Discount</td><td>- {0}</td></tr>", o.Discount.ToString("##00.00")))
        End If

        If o.Coupon <> String.Empty Then
            builder.Append(String.Format("<tr><td colspan='4' align='right'>Coupon</td><td>{0}</td></tr>", o.Coupon))
        End If

        If o.PaymentMode = "COD" Then
            builder.Append(String.Format("<tr><td colspan='4' align='right'>COD</td><td>{0}</td></tr>", o.COD.ToString("##00.00")))
        Else
        End If

        builder.Append(String.Format("<tr><td colspan='4' align='right'>Total</td><td>{0}</td></tr>", o.Total.ToString("##00.00")))
        builder.Append("</tbody>")
        builder.Append("</table>")
        builder.Append("<table style='width:100%; border:0px;'>")
        builder.Append("<tr>")
        builder.Append("<td><h3>Billing Address</h3>")
        builder.Append(String.Format("<div>{0}</div>", o.Name))
        builder.Append(String.Format("<div>{0}</div>", o.Email))
        builder.Append(String.Format("<div>{0}</div>", o.Phone))
        builder.Append(String.Format("<div>{0}</div>", o.BillingAddress))
        builder.Append(String.Format("<div>{0}</div>", o.BillingCity))
        builder.Append(String.Format("<div>{0}</div>", o.BillingState))
        builder.Append(String.Format("<div>{0}</div>", o.BillingZip))
        builder.Append(String.Format("<div>{0}</div>", o.BillingCountry))
        builder.Append("</td>")
        builder.Append("<td><h3>Shipping Address</h3>")
        builder.Append(String.Format("<div>{0} {1}</div>", o.ShippingFirstName, o.ShippingLastName))
        builder.Append(String.Format("<div>{0}</div>", o.ShippingPhone))
        builder.Append(String.Format("<div>{0}</div>", o.ShippingAddress))
        builder.Append(String.Format("<div>{0}</div>", o.ShippingCity))
        builder.Append(String.Format("<div>{0}</div>", o.ShippingState))
        builder.Append(String.Format("<div>{0}</div>", o.ShippingZip))
        builder.Append(String.Format("<div>{0}</div>", o.ShippingCountry))
        builder.Append("</td>")
        builder.Append("</tr>")
        builder.Append("</table>")
        builder.Append("<div>")
        builder.Append(String.Format("<div>Order Status : {0}</div>", [Enum].Parse(GetType(OrderStatusType), o.Status.ToString()).ToString()))

        If o.ShippingTrackCode <> "" Then
            builder.Append(String.Format("<div>Shipping Tracking Code : {0}</div>", o.ShippingTrackCode))
        End If

        If o.TransactionCode <> "" Then
            builder.Append(String.Format("<div>Transaction Code : {0}</div>", o.TransactionCode))
        End If

        If o.TransactionDate.HasValue Then
            builder.Append(String.Format("<div>Transaction Date : {0}</div>", o.TransactionDate.Value.ToString()))
        End If

        builder.Append("<p>Thanks for your Purchase. If you have any questions please contact us at ib@rudrasofttech.com or call us at 9871500276. Please provide your order number.</p>")
        builder.Append("</div>")


        Return builder.ToString()
    End Function

    Public Sub UpdatePaymentMode(ByVal orderId As Integer, ByVal paymentmode As String)

        Dim o As Order = dc.Orders.Single(Function(item) item.ID = orderId)
        o.PaymentMode = paymentmode
        o.DateModified = DateTime.Now
        dc.SaveChanges()

    End Sub
End Class
