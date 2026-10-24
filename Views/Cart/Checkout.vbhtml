@ModelType IndiaBobbles.Order
@Code
    ViewData("Title") = "Checkout – India Bobbles"
    Dim inr = New System.Globalization.CultureInfo("en-IN")
    Dim hasItems As Boolean = Model IsNot Nothing AndAlso Model.OrderItems.Count > 0
    Dim hasCoupon As Boolean = Model IsNot Nothing AndAlso Not String.IsNullOrEmpty(Model.Coupon)
    Dim shipName As String = If(Model IsNot Nothing, Trim(Model.ShippingFirstName & " " & Model.ShippingLastName), "")
    Dim sameAddress As Boolean = Model IsNot Nothing AndAlso Model.BillingAddress = Model.ShippingAddress AndAlso Model.BillingZip = Model.ShippingZip

    Dim itemCount As Integer = If(hasItems, Model.OrderItems.Sum(Function(x) x.Quantity), 0)
    Dim shippingPerItem As Decimal = 150D
    Dim shippingNotional As Decimal = itemCount * shippingPerItem

    Dim subtotal As Decimal = If(Model IsNot Nothing, Model.Amount, 0D)
    Dim shipping As Decimal = If(Model IsNot Nothing, Model.ShippingPrice, 0D)
    Dim codFee As Decimal = 40D                                   ' keep in step with OrderManager.CODFee
    Dim toPay As Decimal = If(Model IsNot Nothing, Model.Total - Model.COD, 0D)   ' online amount (without any COD fee)

    ' ---- Offers: phone and email are known here, so first-order / returning rules can be checked properly ----
    Dim cPhone As String = If(Model IsNot Nothing, Model.Phone, "")
    Dim cEmail As String = If(Model IsNot Nothing, Model.Email, "")
    Dim offers = If(hasItems, IndiaBobbles.CouponCode.CartOffers(), New List(Of IndiaBobbles.CouponCode)())
    Dim best = If(hasItems, IndiaBobbles.CouponCode.Best(offers, itemCount, subtotal, cPhone, cEmail), Nothing)
    Dim nextOffer = If(hasItems, IndiaBobbles.CouponCode.NextTier(offers, itemCount, subtotal, cPhone, cEmail), Nothing)
    Dim bestSave As Decimal = If(best Is Nothing, 0D, best.DiscountOn(subtotal))
    Dim currentSave As Decimal = If(hasCoupon, Model.Discount, 0D)
    Dim upgrade = If(hasCoupon AndAlso best IsNot Nothing AndAlso Not String.Equals(best.Code, Model.Coupon.Trim(), StringComparison.OrdinalIgnoreCase) AndAlso bestSave > currentSave, best, Nothing)
    Dim showOffer As Boolean = Not hasCoupon AndAlso best IsNot Nothing
    Dim couponMsg As String = TryCast(TempData("CouponMsg"), String)
    Dim couponOk As String = TryCast(TempData("CouponOk"), String)

    Dim nextSave As Decimal = 0D
    Dim nextNeed As Integer = 0
    If nextOffer IsNot Nothing AndAlso itemCount > 0 Then
        nextNeed = nextOffer.MinItems - itemCount
        nextSave = nextOffer.DiscountOn(subtotal / itemCount * nextOffer.MinItems)
    End If
End Code

<style>
    /* ---- Coupon / offer (same look as the cart page) ---- */
    .ib-co .offer {
        margin: 4px 0 6px;
    }

    .ib-co .tkt {
        display: flex;
        align-items: stretch;
        border-radius: 14px;
        overflow: hidden;
        border: 2px dashed #330B3F;
        background: #fff;
    }

        .ib-co .tkt .l {
            background: #330B3F;
            color: #fff;
            padding: 12px 14px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            min-width: 104px;
            text-align: center;
        }

            .ib-co .tkt .l .v {
                color: #ffc107;
                font-weight: 800;
                font-size: 19px;
                line-height: 1.1;
            }

            .ib-co .tkt .l .c {
                font-family: Consolas, Menlo, monospace;
                font-size: 12px;
                letter-spacing: 1px;
                margin-top: 4px;
                opacity: .9;
            }

        .ib-co .tkt .r {
            flex: 1;
            padding: 10px 12px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

            .ib-co .tkt .r .t {
                flex: 1;
                min-width: 0;
            }

            .ib-co .tkt .r b {
                display: block;
                color: #1f1724;
                font-size: 15px;
                line-height: 1.3;
            }

            .ib-co .tkt .r small {
                display: block;
                color: #157a3c;
                font-weight: 600;
                margin-top: 2px;
            }

    .ib-co .btn-offer {
        background: #ffc107;
        color: #330B3F;
        font-weight: 800;
        border: 0;
        border-radius: 10px;
        padding: 9px 18px;
        white-space: nowrap;
    }

        .ib-co .btn-offer:hover {
            background: #ffb300;
            color: #330B3F;
        }

    .ib-co .nudge {
        margin-top: 10px;
        background: #f3ecf6;
        color: #330B3F;
        border-radius: 10px;
        padding: 9px 12px;
        font-size: 14px;
    }

        .ib-co .nudge i {
            color: #e23744;
            margin-right: 4px;
        }

        .ib-co .nudge a {
            color: #330B3F;
            font-weight: 700;
        }

    .ib-co .other {
        background: none;
        border: 0;
        padding: 0;
        margin-top: 10px;
        color: #6b5f72;
        font-size: 14px;
        text-decoration: underline;
        cursor: pointer;
    }

    .ib-co .coupon-ok {
        margin: 10px 0 0;
        background: #e9f7ee;
        border: 1px solid #b7e4c7;
        color: #157a3c;
        border-radius: 10px;
        padding: 9px 12px;
        font-size: 14px;
        font-weight: 600;
    }

    .ib-co .coupon-msg {
        margin: 10px 0 0;
        background: #fff4e5;
        border: 1px solid #ffd59a;
        color: #7a4b00;
        border-radius: 10px;
        padding: 9px 12px;
        font-size: 14px;
    }

    .ib-co .upgrade {
        margin-top: 10px;
        display: flex;
        align-items: center;
        justify-content: space-between;
        gap: 10px;
        background: #e9f7ee;
        color: #157a3c;
        border-radius: 10px;
        padding: 9px 12px;
        font-size: 14px;
        font-weight: 600;
    }

        .ib-co .upgrade .btn-offer {
            padding: 6px 14px;
        }

    .ib-co .totals .grand s {
        color: #8a7f90;
        font-weight: 400;
        font-size: .7em;
        margin-right: 6px;
    }

    .ib-co .saved {
        margin: 6px 0 0;
        text-align: right;
        color: #157a3c;
        font-weight: 700;
        font-size: 14px;
    }

    @@media (max-width: 575px) {
        .ib-co .tkt .l {
            min-width: 86px;
            padding: 10px;
        }

            .ib-co .tkt .l .v {
                font-size: 16px;
            }
    }
</style>

@section scripts
    <script>
        // Prevent double payment clicks
        $('#ibPayForm').on('submit', function () {
            var b = $('#ibPayBtn');
            setTimeout(function () { b.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Taking you to secure payment…'); }, 0);
        });
        $('#ibCodForm').on('submit', function () {
            var b = $(this).find('button');
            setTimeout(function () { b.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Placing your order…'); }, 0);
        });
        // Offer / coupon buttons: show progress
        $('form.ib-offer-form').on('submit', function () {
            var b = $(this).find('button');
            setTimeout(function () { b.prop('disabled', true).text('Applying…'); }, 0);
        });
        // "Have a different code?"
        $('#ibOtherCode').on('click', function () {
            $(this).addClass('d-none');
            $('#ibCodeForm').removeClass('d-none').find('input[name=coupon]').focus();
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

                    <!-- Coupon / offer -->
                    @If hasCoupon Then
                        @<div class="coupon applied">
                            <span><i class="fa fa-tag" aria-hidden="true"></i> <b>@Model.Coupon</b> applied · you save ₹@(currentSave.ToString("#,##0.##", inr))</span>
                            <form method="post" action="@Url.Content("~/cart/RemoveCoupon")" class="m-0">
                                @Html.AntiForgeryToken()
                                <input type="hidden" name="returnto" value="Checkout" />
                                <button type="submit" class="link">Remove</button>
                            </form>
                        </div>
                        If upgrade IsNot Nothing Then
                            @<div class="upgrade">
                                <span><i class="fa fa-arrow-circle-up" aria-hidden="true"></i> Save ₹@((bestSave - currentSave).ToString("#,##0", inr)) more with @upgrade.Code</span>
                                <form method="post" action="@Url.Content("~/cart/applycoupon")" class="m-0 ib-offer-form">
                                    @Html.AntiForgeryToken()
                                    <input type="hidden" name="returnto" value="Checkout" />
                                    <input type="hidden" name="coupon" value="@upgrade.Code" />
                                    <button type="submit" class="btn btn-offer">Switch</button>
                                </form>
                            </div>
                        End If
                    ElseIf showOffer Then
                        @<div class="offer">
                            <div class="tkt">
                                <div class="l">
                                    <span class="v">@best.Label().ToUpperInvariant()</span>
                                    <span class="c">@best.Code</span>
                                </div>
                                <div class="r">
                                    <div class="t">
                                        <b>@best.Label() @(If(String.IsNullOrWhiteSpace(best.Pitch), "on your order", best.Pitch))</b>
                                        <small>You pay ₹@((toPay - bestSave).ToString("#,##0", inr))</small>
                                    </div>
                                    <form method="post" action="@Url.Content("~/cart/applycoupon")" class="m-0 ib-offer-form">
                                        @Html.AntiForgeryToken()
                                        <input type="hidden" name="returnto" value="Checkout" />
                                        <input type="hidden" name="coupon" value="@best.Code" />
                                        <button type="submit" class="btn btn-offer">Apply</button>
                                    </form>
                                </div>
                            </div>

                            @If nextOffer IsNot Nothing Then
                                @<div class="nudge"><i class="fa fa-gift" aria-hidden="true"></i> Or add @nextNeed more and save <b>₹@(nextSave.ToString("#,##0", inr))</b> with @nextOffer.Code. <a href="~/cart">Add from cart</a></div>
                            End If

                            <button type="button" class="other" id="ibOtherCode">Have a different code?</button>
                            <form method="post" action="@Url.Content("~/cart/applycoupon")" class="coupon d-none" id="ibCodeForm">
                                @Html.AntiForgeryToken()
                                <input type="hidden" name="returnto" value="Checkout" />
                                <input type="text" name="coupon" maxlength="100" class="form-control" placeholder="Enter coupon code" aria-label="Coupon code" />
                                <button type="submit" class="btn btn-apply">Apply</button>
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

                    @If Not String.IsNullOrEmpty(couponOk) Then
                        @<div class="coupon-ok"><i class="fa fa-check-circle" aria-hidden="true"></i> @couponOk</div>
                    End If
                    @If Not String.IsNullOrEmpty(couponMsg) Then
                        @<div class="coupon-msg"><i class="fa fa-info-circle" aria-hidden="true"></i> @couponMsg</div>
                    End If

                    <div class="totals">
                        <div><span>Subtotal</span><span>₹@(subtotal.ToString("#,##0.##", inr))</span></div>
                        @If hasCoupon Then
                            @<div class="disc"><span>Coupon discount (@Model.Coupon)</span><span>− ₹@(Model.Discount.ToString("#,##0.##", inr))</span></div>
                        End If
                        <div>
                            <span>Shipping</span>
                            @If shipping > 0 Then
                                @<span>₹@(shipping.ToString("#,##0.##", inr))</span>
                            Else
                                @<span>
                                    <s class="text-muted">₹@(shippingNotional.ToString("#,##0.##", inr))</s>
                                    <span class="free ms-1">₹0 (FREE)</span>
                                </span>
                            End If
                        </div>

                        <div class="grand">
                            <span>Total to pay</span>
                            <span>
                                @If hasCoupon AndAlso Model.Discount > 0 Then
                                    @<s>₹@((subtotal + shipping).ToString("#,##0", inr))</s>
                                End If
                                ₹@(Model.Total.ToString("#,##0.##", inr))
                            </span>
                        </div>
                        <div class="tax">Inclusive of all taxes</div>
                        @If hasCoupon AndAlso Model.Discount > 0 Then
                            @<div class="saved"><i class="fa fa-smile-o" aria-hidden="true"></i> You're saving ₹@(Model.Discount.ToString("#,##0", inr)) on this order</div>
                        End If
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
                    <form method="post" action="@Url.Content("~/cart/cod")" class="mt-3 mb-2" id="ibCodForm">
                        @Html.AntiForgeryToken()
                        <button type="submit" class="btn btn-outline-success btn-lg w-100">
                            <i class="fa fa-money" aria-hidden="true"></i> Cash on Delivery · ₹@((toPay + codFee).ToString("#,##0.##", inr))
                        </button>
                    </form>
                    <p class="small text-muted mb-3">A convenience fee of ₹@(codFee.ToString("#,##0", inr)) is added for COD orders@(If(hasCoupon AndAlso Model.Discount > 0, ", your coupon still applies", "")).</p>
                    <div class="pay-methods">
                        <span><i class="fa fa-mobile" aria-hidden="true"></i> UPI</span>
                        <span><i class="fa fa-credit-card" aria-hidden="true"></i> Cards</span>
                        <span><i class="fa fa-university" aria-hidden="true"></i> Net banking</span>
                    </div>
                    <p class="secure"><i class="fa fa-shield" aria-hidden="true"></i> Payments processed securely by PayU. We never see or store your card or UPI details.</p>

                    <p class="terms">By paying, you agree to our <a href="~/terms-and-conditions">Terms</a> and <a href="~/shipping-policy">Shipping &amp; Returns</a> policy.</p>
                </div>
            </div>
        </div>
    End If
</div>