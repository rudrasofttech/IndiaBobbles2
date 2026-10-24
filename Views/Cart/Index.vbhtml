
@ModelType IndiaBobbles.Order
@Code
    ViewData("Title") = "Your Cart – India Bobbles"
    Dim inr = New System.Globalization.CultureInfo("en-IN")
    Dim items = Model.OrderItems.ToList()
    Dim itemCount As Integer = items.Sum(Function(x) x.Quantity)
    Dim hasCoupon As Boolean = Model IsNot Nothing AndAlso Not String.IsNullOrEmpty(Model.Coupon)
    ' Amount = items subtotal, Discount = coupon (both kept up to date by OrderManager.Recalculate)
    Dim subtotal As Decimal = If(Model IsNot Nothing, Model.Amount, 0D)
    Dim shipping As Decimal = If(Model IsNot Nothing, Model.ShippingPrice, 0D)
    Dim total As Decimal = subtotal - If(hasCoupon, Model.Discount, 0D) + shipping

    Dim shippingPerItem As Decimal = 150D
    Dim shippingNotional As Decimal = itemCount * shippingPerItem

    Dim cName As String = If(Model IsNot Nothing, Model.Name, "")
    Dim cEmail As String = If(Model IsNot Nothing, Model.Email, "")
    Dim cPhone As String = If(Model IsNot Nothing, Model.Phone, "")

    ' ---- Offers: coupons marked "Show on cart" (rules + amounts in Admin › Coupon Codes) ----
    Dim offers = IndiaBobbles.CouponCode.CartOffers()
    Dim best = IndiaBobbles.CouponCode.Best(offers, itemCount, subtotal, cPhone, cEmail)
    Dim nextOffer = IndiaBobbles.CouponCode.NextTier(offers, itemCount, subtotal, cPhone, cEmail)
    Dim bestSave As Decimal = If(best Is Nothing, 0D, best.DiscountOn(subtotal))
    Dim currentSave As Decimal = If(hasCoupon, Model.Discount, 0D)
    ' A better offer than the coupon already applied?
    Dim upgrade = If(hasCoupon AndAlso best IsNot Nothing AndAlso Not String.Equals(best.Code, Model.Coupon.Trim(), StringComparison.OrdinalIgnoreCase) AndAlso bestSave > currentSave, best, Nothing)
    Dim showOffer As Boolean = Not hasCoupon AndAlso best IsNot Nothing
    Dim couponMsg As String = TryCast(TempData("CouponMsg"), String)
    Dim couponOk As String = TryCast(TempData("CouponOk"), String)

    ' Saving the next tier would give, worked out on the cart's average item price
    Dim nextSave As Decimal = 0D
    Dim nextNeed As Integer = 0
    If nextOffer IsNot Nothing AndAlso itemCount > 0 Then
        nextNeed = nextOffer.MinItems - itemCount
        nextSave = nextOffer.DiscountOn(subtotal / itemCount * nextOffer.MinItems)
    End If

    Dim suggestions = If(TryCast(ViewBag.Suggestions, List(Of IndiaBobbles.Product)), New List(Of IndiaBobbles.Product)())

    ' ---- Delivery estimate: SET THESE to your real dispatch + courier days ----
    Dim deliverMinDays As Integer = 5
    Dim deliverMaxDays As Integer = 8
    Dim dFrom = DateTime.Today.AddDays(deliverMinDays)
    Dim dTo = DateTime.Today.AddDays(deliverMaxDays)
    Dim deliveryText As String = If(dFrom.Month = dTo.Month,
        dFrom.Day.ToString() & "–" & dTo.ToString("d MMM", inr),
        dFrom.ToString("d MMM", inr) & " – " & dTo.ToString("d MMM", inr))
End Code

<style>
    /* ---- Cart offers (page-specific) ---- */
    .ib-cart .offer {
        margin: 18px 0 4px;
    }

    .ib-cart .tkt {
        display: flex;
        align-items: stretch;
        border-radius: 14px;
        overflow: hidden;
        border: 2px dashed #330B3F;
        background: #fff;
    }

        .ib-cart .tkt .l {
            background: #330B3F;
            color: #fff;
            padding: 12px 14px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            min-width: 108px;
            text-align: center;
        }

            .ib-cart .tkt .l .v {
                color: #ffc107;
                font-weight: 800;
                font-size: 20px;
                line-height: 1.1;
            }

            .ib-cart .tkt .l .c {
                font-family: Consolas, Menlo, monospace;
                font-size: 12px;
                letter-spacing: 1px;
                margin-top: 4px;
                opacity: .9;
            }

        .ib-cart .tkt .r {
            flex: 1;
            padding: 10px 12px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

            .ib-cart .tkt .r .t {
                flex: 1;
                min-width: 0;
            }

            .ib-cart .tkt .r b {
                display: block;
                color: #1f1724;
                font-size: 15px;
                line-height: 1.3;
            }

            .ib-cart .tkt .r small {
                display: block;
                color: #157a3c;
                font-weight: 600;
                margin-top: 2px;
            }

    .ib-cart .btn-offer {
        background: #ffc107;
        color: #330B3F;
        font-weight: 800;
        border: 0;
        border-radius: 10px;
        padding: 9px 18px;
        white-space: nowrap;
    }

        .ib-cart .btn-offer:hover {
            background: #ffb300;
            color: #330B3F;
        }

    .ib-cart .nudge {
        margin-top: 10px;
        background: #f3ecf6;
        color: #330B3F;
        border-radius: 10px;
        padding: 9px 12px;
        font-size: 14px;
    }

        .ib-cart .nudge i {
            color: #e23744;
            margin-right: 4px;
        }

    .ib-cart .other {
        background: none;
        border: 0;
        padding: 0;
        margin-top: 10px;
        color: #6b5f72;
        font-size: 14px;
        text-decoration: underline;
        cursor: pointer;
    }

    .ib-cart .coupon-ok {
        margin-top: 10px;
        background: #e9f7ee;
        border: 1px solid #b7e4c7;
        color: #157a3c;
        border-radius: 10px;
        padding: 9px 12px;
        font-size: 14px;
        font-weight: 600;
    }

    .ib-cart .coupon-msg {
        margin-top: 10px;
        background: #fff4e5;
        border: 1px solid #ffd59a;
        color: #7a4b00;
        border-radius: 10px;
        padding: 9px 12px;
        font-size: 14px;
    }

    .ib-cart .upgrade {
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

        .ib-cart .upgrade .btn-offer {
            padding: 6px 14px;
        }

    .ib-cart .totals .grand s {
        color: #8a7f90;
        font-weight: 400;
        font-size: .7em;
        margin-right: 6px;
    }

    .ib-cart .contact.pulse {
        animation: ibPulse 1s ease 2;
    }

    @@keyframes ibPulse {
        0%, 100% {
            box-shadow: 0 0 0 0 rgba(255,193,7,0);
        }

        50% {
            box-shadow: 0 0 0 6px rgba(255,193,7,.55);
        }
    }

    /* delivery estimate */
    .ib-cart .eta {
        margin-top: 14px;
        display: flex;
        align-items: center;
        gap: 10px;
        background: #f6f1f8;
        border-radius: 12px;
        padding: 12px 14px;
        color: #330B3F;
        font-size: 15px;
    }

        .ib-cart .eta i {
            font-size: 20px;
        }

    /* make it a pair */
    .ib-cart .pair {
        margin-top: 22px;
        border: 1px solid #ece6ef;
        border-radius: 16px;
        padding: 18px;
        background: #fff;
    }

        .ib-cart .pair h3 {
            font-size: 20px;
            font-weight: 800;
            color: #330B3F;
            margin: 0;
        }

        .ib-cart .pair .sub {
            color: #6b5f72;
            margin: 4px 0 14px;
            font-size: 14px;
        }

            .ib-cart .pair .sub b {
                color: #157a3c;
            }

        .ib-cart .pair .grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 12px;
        }

        .ib-cart .pair .p {
            border: 1px solid #ece6ef;
            border-radius: 12px;
            overflow: hidden;
            transition: transform .15s, box-shadow .15s;
        }

            .ib-cart .pair .p:hover {
                transform: translateY(-2px);
                box-shadow: 0 8px 18px rgba(51,11,63,.12);
            }

            .ib-cart .pair .p a {
                text-decoration: none;
            }

        .ib-cart .pair .img {
            display: block;
        }

        .ib-cart .pair .img {
            aspect-ratio: 1 / 1;
            background: #f6f1f8;
        }

            .ib-cart .pair .img img {
                width: 100%;
                height: 100%;
                object-fit: cover;
                display: block;
            }

        .ib-cart .pair .b {
            padding: 10px;
        }

        .ib-cart .pair .n {
            font-weight: 700;
            font-size: 14px;
            color: #1f1724;
            line-height: 1.3;
            display: -webkit-box;
            -webkit-line-clamp: 2;
            -webkit-box-orient: vertical;
            overflow: hidden;
            min-height: 2.6em;
        }

        .ib-cart .pair .pr {
            margin-top: 4px;
            font-weight: 800;
            color: #330B3F;
        }

            .ib-cart .pair .pr s {
                color: #8a7f90;
                font-weight: 400;
                font-size: 13px;
                margin-left: 4px;
            }

        .ib-cart .pair .go:hover {
            background: #4a1a5a;
            color: #fff;
        }

        .ib-cart .pair .go {
            display: block;
            margin-top: 8px;
            text-align: center;
            background: #330B3F;
            color: #fff;
            border-radius: 8px;
            padding: 7px 0;
            font-weight: 700;
            font-size: 13px;
        }

    @@media (max-width: 575px) {
        .ib-cart .pair {
            padding: 14px;
        }

            .ib-cart .pair .grid {
                gap: 8px;
            }

            .ib-cart .pair .b {
                padding: 8px;
            }

            .ib-cart .pair .n {
                font-size: 13px;
            }

        .ib-cart .tkt .l {
            min-width: 88px;
            padding: 10px;
        }

            .ib-cart .tkt .l .v {
                font-size: 17px;
            }
    }
</style>

@Section scripts
    <script>
        (function () {
            var saveUrl = '@Url.Content("~/cart/updatecontact")';
            var status = $('#ibSaveStatus');
            var phoneMsg = $('#ibPhoneMsg');
            var phoneMsgDefault = phoneMsg.text();
            var timer = null, lastSaved = '';

            function phoneOk(v) { return /^[6-9][0-9]{9}$/.test(v); }
            function emailOk(v) { return v === '' || /^[^\s@@]+@@[^\s@@]+\.[^\s@@]+$/.test(v); }

            // Returns a promise so the offer button can wait for the number to be saved
            function save() {
                var name = $('#cName').val().trim();
                var email = $('#cEmail').val().trim();
                var phone = $('#cPhone').val().trim();
                if (!phoneOk(phone) || !emailOk(email)) return $.Deferred().resolve().promise();
                var key = name + '|' + email + '|' + phone;
                if (key === lastSaved) return $.Deferred().resolve().promise();
                status.text('Saving…').attr('class', 'save-status');
                return $.get(saveUrl, { name: name, email: email, Phone: phone })
                    .done(function () { lastSaved = key; status.html('<i class="fa fa-check"></i> Saved').attr('class', 'save-status ok'); })
                    .fail(function () { status.text('Couldn\'t save. We\'ll ask again at checkout.').attr('class', 'save-status err'); });
            }

            function askForPhone(msg) {
                phoneMsg.text(msg).removeClass('d-none');
                $('#ibContact').removeClass('pulse');
                void document.getElementById('ibContact').offsetWidth;   // restart the animation
                $('#ibContact').addClass('pulse');
                $('#cPhone').focus();
            }

            // Save automatically shortly after typing stops, and when leaving a field
            $('#ibContact input').on('input', function () { clearTimeout(timer); timer = setTimeout(save, 900); })
                                 .on('blur', save);

            // Phone: digits only
            $('#cPhone').on('input', function () {
                this.value = this.value.replace(/\D/g, '').slice(0, 10);
                if (phoneOk(this.value)) phoneMsg.addClass('d-none');
            });

            // Offer buttons: need the WhatsApp number first (that's the "unlock")
            $('form.ib-offer-form').on('submit', function (e) {
                var form = this;
                e.preventDefault();
                if (!phoneOk($('#cPhone').val().trim())) {
                    askForPhone('Enter your 10-digit WhatsApp number to unlock this offer.');
                    return;
                }
                $(form).find('button').prop('disabled', true).text('Applying…');
                save().always(function () { form.submit(); });
            });

            // "Have a different code?"
            $('#ibOtherCode').on('click', function () {
                $(this).addClass('d-none');
                $('#ibCodeForm').removeClass('d-none').find('input[name=coupon]').focus();
            });

            // Checkout: phone needed for delivery updates
            $('#ibCheckout').on('click', function (e) {
                var link = this.href;
                if (!phoneOk($('#cPhone').val().trim())) {
                    e.preventDefault();
                    askForPhone(phoneMsgDefault);
                    return;
                }
                e.preventDefault();
                save().always(function () { window.location = link; });
            });
        })();
    </script>
End Section

<div class="ib-cart container fullbody py-4">

    <div class="d-flex justify-content-between align-items-end flex-wrap gap-2 mb-3">
        <h1 class="m-0">Your cart <span class="count @(If(itemCount = 0, "d-none", ""))">@itemCount @(If(itemCount = 1, "item", "items"))</span></h1>
        <a href="~/tag/collectibles" class="keep"><i class="fa fa-arrow-left" aria-hidden="true"></i> Continue shopping</a>
    </div>

    @If items.Count = 0 Then
        @<div class="empty">
            <i class="fa fa-shopping-cart" aria-hidden="true"></i>
            <h2>Your cart is empty</h2>
            <p>Our hand-painted bobbleheads are waiting to meet you.</p>
            <a class="btn btn-ibp" href="~/tag/collectibles">Shop bobbleheads</a>
        </div>
    Else
        @<div class="row g-4">

            <!-- ============ Items ============ -->
            <div class="col-lg-7">
                @For Each o In items
                    @<div class="item">
                        <img src="@o.ProductImg" alt="@o.ProductName" />
                        <div class="info">
                            <div class="name">@o.ProductName</div>
                            <div class="code">Code: @o.ProductCode</div>
                            <div class="unit">₹@(o.Price.ToString("#,##0.##", inr)) each</div>
                            <div class="d-flex align-items-center gap-3 mt-2 flex-wrap">
                                <div class="qty" role="group" aria-label="Quantity">
                                    <a href="~/cart/subtractquantity/@o.ID" aria-label="Decrease quantity">−</a>
                                    <span>@o.Quantity</span>
                                    <a href="~/cart/addquantity/@o.ID" aria-label="Increase quantity">+</a>
                                </div>
                                <form method="post" action="@Url.Content("~/cart/remove/" & o.ID)" class="m-0">
                                    @Html.AntiForgeryToken()
                                    <button type="submit" class="remove"><i class="fa fa-trash-o" aria-hidden="true"></i> Remove</button>
                                </form>
                            </div>
                        </div>
                        <div class="amt">₹@(o.Amount.ToString("#,##0.##", inr))</div>
                    </div>
                Next

                <div class="perks">
                    <span><i class="fa fa-truck" aria-hidden="true"></i> Free shipping across India</span>
                    <span><i class="fa fa-paint-brush" aria-hidden="true"></i> Hand-painted</span>
                    <span><i class="fa fa-undo" aria-hidden="true"></i> 7-day returns</span>
                </div>

                <!-- Delivery estimate -->
                <div class="eta">
                    <i class="fa fa-calendar-check-o" aria-hidden="true"></i>
                    <span>Order today, arrives by <b>@deliveryText</b></span>
                </div>

                <!-- Make it a pair / suggestions -->
                @If suggestions.Count > 0 Then
                    @<div class="pair">
                        @If nextOffer IsNot Nothing Then
                            @<h3>Make it a pair, save ₹@(nextSave.ToString("#,##0", inr))</h3>
                            @<p class="sub">Add @nextNeed more @(If(nextNeed = 1, "bobblehead", "bobbleheads")) and <b>@nextOffer.Code</b> takes <b>@nextOffer.Label()</b> your whole order.</p>
                        Else
                            @<h3>Complete the gang</h3>
                            @<p class="sub">More hand-painted favourites, with free shipping.</p>
                        End If
                        <div class="grid">
                            @For Each p In suggestions
                                Dim pUrl As String = Url.Content("~/product/" & p.ID & "/" & IndiaBobbles.Utility.Slugify(p.Name))
                                Dim onSale As Boolean = p.MRP > p.SalePrice AndAlso p.MRP > 0 AndAlso p.SalePrice > 0
                                Dim price As Decimal = If(p.SalePrice > 0, p.SalePrice, p.MRP)   ' Cart/Add charges SalePrice
                                Dim mrpHtml As String = If(onSale, "<s>₹" & p.MRP.ToString("#,##0", inr) & "</s>", "")
                                @<div class="p">
                                    <a class="img" href="@pUrl">
                                        @If Not String.IsNullOrWhiteSpace(p.ThumbPath) Then
                                            @<img src="@p.ThumbPath" alt="@p.Name" loading="lazy" />
                                        End If
                                    </a>
                                    <div class="b">
                                        <a class="n" href="@pUrl">@p.Name</a>
                                        <div class="pr">₹@(price.ToString("#,##0", inr)) @Html.Raw(mrpHtml)</div>
                                        <a class="go" href="@Url.Content("~/cart/add/" & p.ID)" rel="nofollow"><i class="fa fa-plus" aria-hidden="true"></i> Add</a>
                                    </div>
                                </div>
                            Next
                        </div>
                    </div>
                End If
            </div>

            <!-- ============ Summary ============ -->
            <div class="col-lg-5">
                <div class="summary">

                    <!-- Contact capture -->
                    <div class="contact" id="ibContact">
                        @If showOffer Then
                            @<h3><i class="fa fa-whatsapp" aria-hidden="true"></i> Unlock @best.Label() + get order updates on WhatsApp</h3>
                            @<p class="sub">Add your number to use the offer below. We'll also save your cart and send delivery updates.</p>
                        Else
                            @<h3><i class="fa fa-whatsapp" aria-hidden="true"></i> Where should we send your order updates?</h3>
                            @<p class="sub">We'll save your cart and send delivery updates on WhatsApp.</p>
                        End If

                        <label for="cPhone" class="form-label">WhatsApp number</label>
                        <div class="input-group mb-1">
                            <span class="input-group-text">+91</span>
                            <input type="tel" id="cPhone" class="form-control" inputmode="numeric" maxlength="10" autocomplete="tel-national" placeholder="10-digit mobile number" value="@cPhone" />
                        </div>
                        <div id="ibPhoneMsg" class="text-danger small mb-2 d-none">Please enter your 10-digit mobile number so we can update you about delivery.</div>

                        <div class="row g-2 mt-1">
                            <div class="col-sm-6">
                                <label for="cName" class="form-label">Name</label>
                                <input type="text" id="cName" class="form-control" maxlength="100" autocomplete="name" placeholder="Your name" value="@cName" />
                            </div>
                            <div class="col-sm-6">
                                <label for="cEmail" class="form-label">Email <span class="opt">(optional)</span></label>
                                <input type="email" id="cEmail" class="form-control" maxlength="250" autocomplete="email" placeholder="you@example.com" value="@cEmail" />
                            </div>
                        </div>

                        <div class="d-flex justify-content-between align-items-center mt-2">
                            <span class="privacy"><i class="fa fa-lock" aria-hidden="true"></i> Only for your order. No spam. <a href="~/privacy-policy">Privacy</a></span>
                            <span id="ibSaveStatus" class="save-status"></span>
                        </div>
                    </div>

                    <!-- Coupon / offer -->
                    @If hasCoupon Then
                        @<div class="coupon applied">
                            <span><i class="fa fa-tag" aria-hidden="true"></i> <b>@Model.Coupon</b> applied · you save ₹@(currentSave.ToString("#,##0.##", inr))</span>
                            <form method="post" action="@Url.Content("~/cart/RemoveCoupon")" class="m-0">
                                @Html.AntiForgeryToken()
                                <input type="hidden" name="returnto" value="Index" />
                                <button type="submit" class="link">Remove</button>
                            </form>
                        </div>
                        If upgrade IsNot Nothing Then
                            @<div class="upgrade">
                                <span><i class="fa fa-arrow-circle-up" aria-hidden="true"></i> Save ₹@((bestSave - currentSave).ToString("#,##0", inr)) more with @upgrade.Code</span>
                                <form method="post" action="@Url.Content("~/cart/applycoupon")" class="m-0 ib-offer-form">
                                    @Html.AntiForgeryToken()
                                    <input type="hidden" name="returnto" value="Index" />
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
                                        <small>You pay ₹@((subtotal - bestSave).ToString("#,##0", inr)) with free shipping</small>
                                    </div>
                                    <form method="post" action="@Url.Content("~/cart/applycoupon")" class="m-0 ib-offer-form">
                                        @Html.AntiForgeryToken()
                                        <input type="hidden" name="returnto" value="Index" />
                                        <input type="hidden" name="coupon" value="@best.Code" />
                                        <button type="submit" class="btn btn-offer">Apply</button>
                                    </form>
                                </div>
                            </div>

                            @If nextOffer IsNot Nothing Then
                                @<div class="nudge"><i class="fa fa-gift" aria-hidden="true"></i> Or add @nextNeed more and save <b>₹@(nextSave.ToString("#,##0", inr))</b> with @nextOffer.Code.</div>
                            End If

                            <button type="button" class="other" id="ibOtherCode">Have a different code?</button>
                            <form method="post" action="@Url.Content("~/cart/applycoupon")" class="coupon d-none" id="ibCodeForm">
                                @Html.AntiForgeryToken()
                                <input type="hidden" name="returnto" value="Index" />
                                <input type="text" name="coupon" maxlength="100" class="form-control" placeholder="Enter coupon code" aria-label="Coupon code" />
                                <button type="submit" class="btn btn-apply">Apply</button>
                            </form>
                        </div>
                    Else
                        @<form method="post" action="@Url.Content("~/cart/applycoupon")" class="coupon">
                            @Html.AntiForgeryToken()
                            <input type="hidden" name="returnto" value="Index" />
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

                    <!-- Totals -->
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
                            <span>Total</span>
                            <span>
                                @If hasCoupon AndAlso Model.Discount > 0 Then
                                    @<s>₹@(subtotal.ToString("#,##0", inr))</s>
                                End If
                                ₹@(total.ToString("#,##0.##", inr))
                            </span>
                        </div>
                        <div class="tax">Inclusive of all taxes</div>
                    </div>

                    <a id="ibCheckout" href="@Url.Content("~/cart/address")" class="btn btn-checkout w-100">Proceed to Checkout <i class="fa fa-arrow-right" aria-hidden="true"></i></a>
                    <p class="secure"><i class="fa fa-lock" aria-hidden="true"></i> Secure payments: UPI, cards, net banking</p>
                </div>
            </div>
        </div>
    End If
</div>