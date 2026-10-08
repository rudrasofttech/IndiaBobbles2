@ModelType IndiaBobbles.Order
@Code
    ViewData("Title") = "Your Cart – India Bobbles"
    Dim inr = New System.Globalization.CultureInfo("en-IN")
    Dim items = Model.OrderItems.ToList()
    Dim itemCount As Integer = items.Sum(Function(x) x.Quantity)
    Dim hasCoupon As Boolean = Model IsNot Nothing AndAlso Not String.IsNullOrEmpty(Model.Coupon)
    ' NOTE: assumes Model.Amount is the subtotal before discount (as on the old page). Adjust if Amount already includes the discount.
    Dim total As Decimal = If(Model IsNot Nothing, Model.Amount - If(hasCoupon, Model.Discount, 0D), 0D)
    Dim cName As String = If(Model IsNot Nothing, Model.Name, "")
    Dim cEmail As String = If(Model IsNot Nothing, Model.Email, "")
    Dim cPhone As String = If(Model IsNot Nothing, Model.Phone, "")
End Code

@Section scripts
    <script>
        (function () {
            var saveUrl = '@Url.Content("~/cart/updatecontact")';
            var status = $('#ibSaveStatus');
            var timer = null, lastSaved = '';

            function phoneOk(v) { return /^[6-9][0-9]{9}$/.test(v); }
            function emailOk(v) { return v === '' || /^[^\s@@]+@@[^\s@@]+\.[^\s@@]+$/.test(v); }

            function save() {
                var name = $('#cName').val().trim();
                var email = $('#cEmail').val().trim();
                var phone = $('#cPhone').val().trim();
                if (!phoneOk(phone) || !emailOk(email)) return;      // only save valid details
                var key = name + '|' + email + '|' + phone;
                if (key === lastSaved) return;
                status.text('Saving…').attr('class', 'save-status');
                $.get(saveUrl, { name: name, email: email, Phone: phone })
                    .done(function () { lastSaved = key; status.html('<i class="fa fa-check"></i> Saved').attr('class', 'save-status ok'); })
                    .fail(function () { status.text('Couldn\'t save. We\'ll ask again at checkout.').attr('class', 'save-status err'); });
            }

            // Save automatically shortly after typing stops, and when leaving a field
            $('#ibContact input').on('input', function () { clearTimeout(timer); timer = setTimeout(save, 900); })
                                 .on('blur', save);

            // Phone: digits only
            $('#cPhone').on('input', function () { this.value = this.value.replace(/\D/g, '').slice(0, 10); });

            // Checkout: phone needed for delivery updates
            $('#ibCheckout').on('click', function (e) {
                var phone = $('#cPhone').val().trim();
                if (!phoneOk(phone)) {
                    e.preventDefault();
                    $('#ibPhoneMsg').removeClass('d-none');
                    $('#cPhone').focus();
                    return;
                }
                save();
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
            </div>

            <!-- ============ Summary ============ -->
            <div class="col-lg-5">
                <div class="summary">

                    <!-- Contact capture -->
                    <div class="contact" id="ibContact">
                        <h3><i class="fa fa-whatsapp" aria-hidden="true"></i> Where should we send your order updates?</h3>
                        <p class="sub">We'll save your cart and send delivery updates on WhatsApp.</p>

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

                    <!-- Coupon -->
                    @If hasCoupon Then
                        @<div class="coupon applied">
                            <span><i class="fa fa-tag" aria-hidden="true"></i> <b>@Model.Coupon</b> applied</span>
                            <form method="post" action="@Url.Content("~/cart/RemoveCoupon")" class="m-0">
                                @Html.AntiForgeryToken()
                                <input type="hidden" name="returnto" value="Index" />
                                <button type="submit" class="link">Remove</button>
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

                    <!-- Totals -->
                    <div class="totals">
                        <div><span>Subtotal</span><span>₹@(Model.Amount.ToString("#,##0.##", inr))</span></div>
                        @If hasCoupon Then
                            @<div class="disc"><span>Coupon discount</span><span>− ₹@(Model.Discount.ToString("#,##0.##", inr))</span></div>
                        End If
                        <div><span>Shipping</span><span class="free">FREE</span></div>
                        <div class="grand"><span>Total</span><span>₹@(total.ToString("#,##0.##", inr))</span></div>
                        <div class="tax">Inclusive of all taxes</div>
                    </div>

                    <a id="ibCheckout" href="@Url.Content("~/cart/address")" class="btn btn-checkout w-100">Proceed to Checkout <i class="fa fa-arrow-right" aria-hidden="true"></i></a>
                    <p class="secure"><i class="fa fa-lock" aria-hidden="true"></i> Secure payments: UPI, cards, net banking</p>
                </div>
            </div>
        </div>
    End If
</div>