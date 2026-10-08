@ModelType IndiaBobbles.Order
@Code
    ViewData("Title") = "Checkout – India Bobbles"
    Dim inr = New System.Globalization.CultureInfo("en-IN")
    Dim hasItems As Boolean = Model IsNot Nothing AndAlso Model.OrderItems.Count > 0
    Dim hasCoupon As Boolean = Model IsNot Nothing AndAlso Not String.IsNullOrEmpty(Model.Coupon)
    Dim shipName As String = If(Model IsNot Nothing, Trim(Model.ShippingFirstName & " " & Model.ShippingLastName), "")
    Dim sameAddress As Boolean = Model IsNot Nothing AndAlso Model.BillingAddress = Model.ShippingAddress AndAlso Model.BillingZip = Model.ShippingZip
End Code

@section scripts
    <script>
        // Prevent double payment clicks
        $('#ibPayForm').on('submit', function () {
            var b = $('#ibPayBtn');
            setTimeout(function () { b.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Taking you to secure payment…'); }, 0);
        });
    </script>
End Section

<div class="ib-cart ib-co container fullbody py-4">

    <!-- Progress -->
    <ol class="progress-steps">
        <li class="done"><a href="~/cart"><span><i class="fa fa-check"></i></span> Cart</a></li>
        <li class="done"><a href="~/cart/address"><span><i class="fa fa-check"></i></span> Address</a></li>
        <li class="on"><span>3</span> Payment</li>
    </ol>

    @If Not hasItems Then
        @<div class="empty">
            <i class="fa fa-shopping-cart" aria-hidden="true"></i>
            <h2>Your cart is empty</h2>
            <p>Add a bobblehead to your cart to check out.</p>
            <a class="btn btn-ibp" href="~/tag/collectibles">Shop bobbleheads</a>
        </div>
    Else
        @<div class="row g-4">

            <!-- ============ Left: review ============ -->
            <div class="col-lg-7">
                <h1 class="mb-1">Review &amp; pay</h1>
                <p class="text-muted mb-4">Check your details, then pay securely.</p>

                <!-- Delivery -->
                <div class="box">
                    <div class="box-head">
                        <h3><i class="fa fa-truck" aria-hidden="true"></i> Delivering to</h3>
                        <a href="~/cart/address" class="change">Change</a>
                    </div>
                    <p class="mb-0">
                        <b>@shipName</b><br />
                        @Model.ShippingAddress<br />
                        @Model.ShippingCity, @Model.ShippingState – @Model.ShippingZip<br />
                        <i class="fa fa-phone" aria-hidden="true"></i> +91 @Model.ShippingPhone
                    </p>
                    <p class="eta"><i class="fa fa-clock-o" aria-hidden="true"></i> Ships within 2 business days · usually delivered in 2–10 working days</p>
                </div>

                <!-- Contact & billing -->
                <div class="box">
                    <div class="box-head">
                        <h3><i class="fa fa-user" aria-hidden="true"></i> Contact &amp; billing</h3>
                        <a href="~/cart/address" class="change">Change</a>
                    </div>
                    <p class="mb-0">
                        <b>@Model.Name</b> · @Model.Email · +91 @Model.Phone<br />
                        @If sameAddress Then
                            @<span class="text-muted">Billing address same as delivery address</span>
                        Else
                            @<span>@Model.BillingAddress, @Model.BillingCity, @Model.BillingState – @Model.BillingZip</span>
                        End If
                    </p>
                </div>

                <!-- Items -->
                <div class="box">
                    <div class="box-head">
                        <h3><i class="fa fa-shopping-bag" aria-hidden="true"></i> Your items</h3>
                        <a href="~/cart" class="change">Edit cart</a>
                    </div>
                    @For Each o In Model.OrderItems
                        @<div class="line">
                            <img src="@o.ProductImg" alt="@o.ProductName" />
                            <div class="flex-grow-1">
                                <div class="name">@o.ProductName</div>
                                <div class="small text-muted">Qty @o.Quantity × ₹@(o.Price.ToString("#,##0.##", inr))</div>
                            </div>
                            <div class="amt">₹@(o.Amount.ToString("#,##0.##", inr))</div>
                        </div>
                    Next
                </div>
            </div>

            <!-- ============ Right: summary + pay ============ -->
            <div class="col-lg-5">
                <div class="summary">
                    <h3 class="sum-title">Order summary</h3>

                    @If hasCoupon Then
                        @<div class="coupon applied">
                            <span><i class="fa fa-tag" aria-hidden="true"></i> <b>@Model.Coupon</b> applied</span>
                            <form method="post" action="@Url.Content("~/cart/RemoveCoupon")" class="m-0">
                                @Html.AntiForgeryToken()
                                <input type="hidden" name="returnto" value="Checkout" />
                                <button type="submit" class="link">Remove</button>
                            </form>
                        </div>
                    Else
                        @<form method="post" action="@Url.Content("~/cart/applycoupon")" class="coupon">
                            @Html.AntiForgeryToken()
                            <input type="hidden" name="returnto" value="Checkout" />
                            <input type="text" name="coupon" maxlength="100" class="form-control" placeholder="Have a coupon code?" aria-label="Coupon code" />
                            <button type="submit" class="btn btn-apply">Apply</button>
                        </form>
                    End If

                    <div class="totals">
                        <div><span>Subtotal</span><span>₹@(Model.Amount.ToString("#,##0.##", inr))</span></div>
                        @If hasCoupon Then
                            @<div class="disc"><span>Coupon discount</span><span>− ₹@(Model.Discount.ToString("#,##0.##", inr))</span></div>
                        End If
                        <div>
                            <span>Shipping</span>
                            @If Model.ShippingPrice = 0 Then
                                @<span class="free">FREE</span>
                            Else
                                @<span>₹@(Model.ShippingPrice.ToString("#,##0.##", inr))</span>
                            End If
                        </div>
                        <div class="grand"><span>Total to pay</span><span>₹@(Model.Total.ToString("#,##0.##", inr))</span></div>
                        <div class="tax">Inclusive of all taxes</div>
                    </div>

                    @* PayU form – field names and values unchanged (the hash depends on them) *@
                    <form id="ibPayForm" action="@ViewBag.action1" method="post">
                        <input type="hidden" name="hash" value="@ViewBag.hash1" />
                        <input type="hidden" name="key" value="@ViewBag.key" />
                        <input type="hidden" name="txnid" value="@Model.ID" />
                        <input type="hidden" name="amount" value="@Model.Total.ToString("g29")" />
                        <input type="hidden" name="firstname" value="@Model.Name" />
                        <input type="hidden" name="lastname" value="" />
                        <input type="hidden" name="email" value="@Model.Email" />
                        <input type="hidden" name="phone" value="@Model.Phone" />
                        <input type="hidden" name="productinfo" value="IndiaBobblesProducts" />
                        <input type="hidden" name="surl" value="https://www.indiabobbles.com/cart/payumoneyresponse?id=@Model.ID" />
                        <input type="hidden" name="furl" value="https://www.indiabobbles.com/cart/checkout?id=@Model.ID" />
                        <input type="hidden" name="curl" value="https://www.indiabobbles.com/cart/checkout?id=@Model.ID" />
                        <input type="hidden" name="address1" value="@Model.BillingAddress" />
                        <input type="hidden" name="address2" value="" />
                        <input type="hidden" name="city" value="@Model.BillingCity" />
                        <input type="hidden" name="state" value="@Model.BillingState" />
                        <input type="hidden" name="country" value="@Model.BillingCountry" />
                        <input type="hidden" name="zipcode" value="@Model.BillingZip" />
                        <input type="hidden" name="udf1" value="" />
                        <input type="hidden" name="udf2" value="" />
                        <input type="hidden" name="udf3" value="" />
                        <input type="hidden" name="udf4" value="" />
                        <input type="hidden" name="udf5" value="" />
                        <input type="hidden" name="pg" value="" />
                        <input type="hidden" name="service_provider" value="payu_paisa" />

                        <button type="submit" id="ibPayBtn" class="btn btn-success btn-lg w-100">
                            <i class="fa fa-lock" aria-hidden="true"></i> Pay ₹@(Model.Total.ToString("#,##0.##", inr)) securely
                        </button>
                    </form>

                    <div class="pay-methods">
                        <span><i class="fa fa-mobile" aria-hidden="true"></i> UPI</span>
                        <span><i class="fa fa-credit-card" aria-hidden="true"></i> Cards</span>
                        <span><i class="fa fa-university" aria-hidden="true"></i> Net banking</span>
                    </div>
                    <p class="secure"><i class="fa fa-shield" aria-hidden="true"></i> Payments processed securely by PayU. We never see or store your card or UPI details.</p>

                    @*<div>
                            <div style="height:20px;position:relative;width:100%;text-align:center;" class="my-3">
                                <hr style="width: 100%; position: absolute; top: 0px; padding: 0px;" />
                                <span class="bg-light" style=" border-radius: 15px; padding: 5px; position: absolute; left: calc(50% - 15px);">Or</span>
                            </div>
                        </div>
                        <form method="post" action="@Url.Content("~/cart/cod")">
                            @Html.AntiForgeryToken()
                            <button type="submit" class="btn btn-success btn-lg">Cash On Delivery</button>
                        </form>*@

                    <p class="terms">By paying, you agree to our <a href="~/terms-and-conditions">Terms</a> and <a href="~/shipping-policy">Shipping &amp; Returns</a> policy.</p>
                </div>
            </div>
        </div>
    End If
</div>