@ModelType IndiaBobbles.Order
@Code
    ViewData("Title") = "Order #" & Model.ID & " – India Bobbles"
    Dim inr = New System.Globalization.CultureInfo("en-IN")

    ' Status → label, badge colour, tracker stage (0 = no tracker)
    Dim statusText As String = "Order placed"
    Dim statusCss As String = "st-new"
    Dim stage As Integer = 1
    Select Case Model.Status
        Case 1 : statusText = "Awaiting payment" : statusCss = "st-new" : stage = 1
        Case 2 : statusText = "Being prepared" : statusCss = "st-proc" : stage = 2
        Case 3 : statusText = "Paid" : statusCss = "st-paid" : stage = 2
        Case 4 : statusText = "Cash on delivery" : statusCss = "st-paid" : stage = 2
        Case 5 : statusText = "Shipped" : statusCss = "st-ship" : stage = 3
        Case 6 : statusText = "Delivered" : statusCss = "st-done" : stage = 4
        Case 7 : statusText = "Refunded" : statusCss = "st-ref" : stage = 0
        Case 8 : statusText = "Cancelled" : statusCss = "st-del" : stage = 0
    End Select

    ' Personal details (name, email, address) are shown only to logged-in users.
    ' Order numbers are sequential, so anyone could guess another order's URL.
    Dim showPersonal As Boolean = Request.IsAuthenticated
    Dim isConfirmed As Boolean = (Model.Status = 3 OrElse Model.Status = 4)
    Dim shipName As String = Trim(If(Model.ShippingFirstName, "") & " " & If(Model.ShippingLastName, ""))
    Dim hasTrack As Boolean = Not String.IsNullOrEmpty(Model.ShippingTrackCode)
    Dim orderEmail As String = "iborder." & Model.ID & "@rudrasofttech.com"
    Dim waUrl As String = Url.Action("WhatsAppRedirect", "Home", New With {.message = "Hi, I have a question about my order #" & Model.ID})
    Dim steps() As String = {"Order placed", "Confirmed", "Shipped", "Delivered"}
    Dim stepIcons() As String = {"fa-shopping-cart", "fa-check", "fa-truck", "fa-home"}
End Code

@section scripts
    <script>
        function ibCopyTrack(btn) {
            var code = '@Html.Raw(System.Web.HttpUtility.JavaScriptStringEncode(If(Model.ShippingTrackCode, "")))';
            if (navigator.clipboard) {
                navigator.clipboard.writeText(code).then(function () {
                    $(btn).html('<i class="fa fa-check"></i> Copied');
                    setTimeout(function () { $(btn).html('<i class="fa fa-copy"></i> Copy'); }, 2000);
                });
            }
        }
    </script>
End Section

<div class="ib-cart ib-co ib-order container fullbody py-4">

    @If isConfirmed Then
        @<div class="thanks">
            <span class="tick"><i class="fa fa-check" aria-hidden="true"></i></span>
            <div>
                <h1>Thank you@(If(Not showPersonal OrElse String.IsNullOrEmpty(Model.Name), "", ", " & Model.Name.Split(" "c)(0)))! 🎉</h1>
                <p class="mb-0">Your order is confirmed. Our artists are getting your bobblehead ready.</p>
                @If showPersonal AndAlso Not String.IsNullOrEmpty(Model.Email) Then
                    @<p class="mb-0">A confirmation has been sent to <b>@Model.Email</b>.</p>
                End If
            </div>
        </div>
    End If

    <!-- Header -->
    <div class="head">
        <div>
            <div class="small text-muted">Order number</div>
            <h2>#@Model.ID</h2>
            <div class="meta">
                <span><i class="fa fa-calendar-o" aria-hidden="true"></i> @(Model.DateCreated.ToString("d MMM yyyy"))</span>
                @If Not String.IsNullOrEmpty(Model.PaymentMode) Then
                    @<span><i class="fa fa-credit-card" aria-hidden="true"></i> @Model.PaymentMode</span>
                End If
                @If Not String.IsNullOrEmpty(Model.TransactionCode) Then
                    @<span><i class="fa fa-hashtag" aria-hidden="true"></i> Txn @Model.TransactionCode</span>
                End If
            </div>
        </div>
        <span class="status @statusCss">@statusText</span>
    </div>

    <!-- Tracker -->
    @If stage > 0 Then
        @<div class="box tracker">
            <ol>
                @For i As Integer = 0 To 3
                    Dim cls As String = If(i + 1 < stage, "done", If(i + 1 = stage, "on", ""))
                    @<li class="@cls"><span class="dot"><i class="fa @(stepIcons(i))" aria-hidden="true"></i></span>@(steps(i))</li>
                Next
            </ol>

            @If hasTrack OrElse Not String.IsNullOrEmpty(Model.ShippingService) Then
                @<div class="track">
                    <div>
                        @If Not String.IsNullOrEmpty(Model.ShippingService) Then
                            @<span>Courier: <b>@Model.ShippingService</b></span>
                        End If
                        @If hasTrack Then
                            @<span>Tracking number: <b>@Model.ShippingTrackCode</b></span>
                        End If
                    </div>
                    @If hasTrack Then
                        @<button type="button" class="copy" onclick="ibCopyTrack(this)"><i class="fa fa-copy" aria-hidden="true"></i> Copy</button>
                    End If
                </div>
                @<p class="small text-muted mb-0 mt-2">Use the tracking number on the courier's website to see live delivery status.</p>
            ElseIf stage < 3 Then
                @<p class="small text-muted mb-0 mt-3"><i class="fa fa-info-circle" aria-hidden="true"></i> Orders ship within 2 business days. We'll share your tracking number once it's on its way.</p>
            End If
        </div>
    Else
        @<div class="box notice">
            <i class="fa fa-info-circle" aria-hidden="true"></i>
            @If Model.Status = 7 Then
                @<span>This order has been refunded. Refunds usually reach your account within 5–7 working days, depending on your bank.</span>
            Else
                @<span>This order has been cancelled. If you think this is a mistake, please contact us.</span>
            End If
        </div>
    End If

    <div class="row g-4">
        <!-- ============ Items + totals ============ -->
        <div class="col-lg-7">
            <div class="box">
                <div class="box-head"><h3><i class="fa fa-shopping-bag" aria-hidden="true"></i> Items</h3></div>
                @For Each oi In Model.OrderItems
                    @<div class="line">
                        @If Not String.IsNullOrEmpty(oi.ProductImg) Then
                            @<img src="@oi.ProductImg" alt="@oi.ProductName" />
                        End If
                        <div class="flex-grow-1">
                            <div class="name">@oi.ProductName</div>
                            <div class="small text-muted">@(If(String.IsNullOrEmpty(oi.ProductCode), "", oi.ProductCode & " · "))Qty @oi.Quantity × ₹@(oi.Price.ToString("#,##0.00", inr))</div>
                        </div>
                        <div class="amt">₹@(oi.Amount.ToString("#,##0.00", inr))</div>
                    </div>
                Next

                <div class="totals mt-2">
                    <div><span>Subtotal</span><span>₹@(Model.Amount.ToString("#,##0.00", inr))</span></div>
                    @If Model.Discount > 0 Then
                        @<div class="disc"><span>Discount@(If(String.IsNullOrEmpty(Model.Coupon), "", " (" & Model.Coupon & ")"))</span><span>− ₹@(Model.Discount.ToString("#,##0.00", inr))</span></div>
                    End If
                    <div>
                        <span>Shipping</span>
                        @If Model.ShippingPrice = 0 Then
                            @<span class="free">FREE</span>
                        Else
                            @<span>₹@(Model.ShippingPrice.ToString("#,##0.00", inr))</span>
                        End If
                    </div>
                    @If Model.PaymentMode = "COD" Then
                        @<div><span>Cash on delivery fee</span><span>₹@(Model.COD.ToString("#,##0.00", inr))</span></div>
                    End If
                    <div class="grand"><span>Total</span><span>₹@(Model.Total.ToString("#,##0.00", inr))</span></div>
                </div>
            </div>
        </div>

        <!-- ============ Address + actions ============ -->
        <div class="col-lg-5">
            @If showPersonal AndAlso Not String.IsNullOrEmpty(Model.ShippingAddress) Then
                @<div class="box">
                    <div class="box-head"><h3><i class="fa fa-map-marker" aria-hidden="true"></i> Delivery address</h3></div>
                    <p class="mb-0">
                        <b>@shipName</b><br />
                        @Model.ShippingAddress<br />
                        @Model.ShippingCity, @Model.ShippingState – @Model.ShippingZip<br />
                        @If Not String.IsNullOrEmpty(Model.ShippingPhone) Then
                            @<span><i class="fa fa-phone" aria-hidden="true"></i> +91 @Model.ShippingPhone</span>
                        End If
                    </p>
                </div>
            End If

            <div class="box actions-box">
                <div class="box-head"><h3><i class="fa fa-file-text-o" aria-hidden="true"></i> Receipt</h3></div>
                <div class="d-flex flex-wrap gap-2">
                    <form method="post" action="@Url.Content("~/orders/receipt/" & Model.ID)" target="_blank" class="m-0">
                        @Html.AntiForgeryToken()
                        <button type="submit" class="btn btn-ibp"><i class="fa fa-print" aria-hidden="true"></i> Print receipt</button>
                    </form>
                    @If Not String.IsNullOrEmpty(Model.Email) Then
                        @<a href="@Url.Content("~/orders/email/" & Model.ID)" class="btn btn-apply"><i class="fa fa-envelope-o" aria-hidden="true"></i> Email receipt</a>
                    End If
                </div>
            </div>

            <div class="box help">
                <div class="box-head"><h3><i class="fa fa-life-ring" aria-hidden="true"></i> Need help with this order?</h3></div>
                <a class="btn btn-wa w-100 mb-2" href="@waUrl" target="_blank" rel="noopener"><i class="fa fa-whatsapp" aria-hidden="true"></i> Chat on WhatsApp</a>
                <p class="small mb-0">
                    <i class="fa fa-envelope" aria-hidden="true"></i> <a href="mailto:@orderEmail">@orderEmail</a><br />
                    <i class="fa fa-phone" aria-hidden="true"></i> <a href="tel:+919871500276">+91 98715 00276</a> · Mon–Sat, 10 AM–6 PM<br />
                    <i class="fa fa-undo" aria-hidden="true"></i> <a href="~/shipping-policy#damaged">Damaged item? Report within 4 hours of delivery</a>
                </p>
            </div>
        </div>
    </div>

    <div class="text-center mt-4 no-print">
        <a href="~/orders" class="change me-3"><i class="fa fa-list" aria-hidden="true"></i> All my orders</a>
        <a href="~/tag/collectibles" class="btn btn-ibp">Continue shopping</a>
    </div>
</div>