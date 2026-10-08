@ModelType IndiaBobbles.OrderAddressDTO
@Code
    ViewData("Title") = "Delivery Address – India Bobbles"

    Dim statesarr() As String = {"", "Andaman and Nicobar Islands", "Andhra Pradesh", "Arunachal Pradesh",
                              "Assam", "Bihar", "Chandigarh", "Chhattisgarh", "Dadra and Nagar Haveli and Daman and Diu",
                              "Delhi", "Goa", "Gujarat", "Haryana", "Himachal Pradesh",
                              "Jammu and Kashmir", "Jharkhand", "Karnataka", "Kerala", "Ladakh", "Lakshadweep", "Madhya Pradesh",
                              "Maharashtra", "Manipur", "Meghalaya", "Mizoram", "Nagaland", "Odisha",
                              "Puducherry", "Punjab", "Rajasthan", "Sikkim", "Tamil Nadu", "Telangana", "Tripura",
                              "Uttar Pradesh", "Uttarakhand", "West Bengal"}
    Dim statelist As New SelectList(statesarr)

    ' Open the "billing" block only if a different billing address was saved before
    Dim billingDiffers As Boolean = Not String.IsNullOrEmpty(Model.BillingAddress) AndAlso
                                    (Model.BillingAddress <> Model.ShippingAddress OrElse Model.BillingPinCode <> Model.ShippingPincode)
    ' Open the "gift" block only if a different recipient was saved before
    Dim giftDiffers As Boolean = Not String.IsNullOrEmpty(Model.ShippingName) AndAlso Model.ShippingName <> Model.Name
End Code

@section scripts
    <script>
        (function () {
            var form = $('#ibAddrForm');
            var same = $('#ibSameBilling');
            var gift = $('#ibGift');
            var pairs = [['ShippingAddress', 'BillingAddress'], ['ShippingCity', 'BillingCity'], ['ShippingState', 'BillingState'],
            ['ShippingCountry', 'BillingCountry'], ['ShippingPincode', 'BillingPinCode']];

            // Billing same as delivery
            function syncBilling() {
                var on = same.is(':checked');
                $('#ibBilling').toggleClass('d-none', on);
                $('#ibBilling').find('[data-req]').prop('required', !on);
                if (on) pairs.forEach(function (p) { $('#' + p[1]).val($('#' + p[0]).val()); });
            }

            // Recipient: me, or someone else (gift)
            function syncGift() {
                var on = gift.is(':checked');
                $('#ibRecipient').toggleClass('d-none', !on);
                $('#ShippingName, #ShippingPhone').prop('required', on);
                if (!on) { $('#ShippingName').val($('#Name').val()); $('#ShippingPhone').val($('#Phone').val()); }
            }

            same.on('change', syncBilling);
            gift.on('change', syncGift);

            // Digits only for phone and PIN code
            $('#Phone, #ShippingPhone').on('input', function () { this.value = this.value.replace(/\D/g, '').slice(0, 10); });
            $('#ShippingPincode, #BillingPinCode').on('input', function () { this.value = this.value.replace(/\D/g, '').slice(0, 6); });

            // Auto-fill city & state from PIN code (India Post public API). Fails silently.
            function pinLookup(pinId, cityId, stateId, hintId) {
                $('#' + pinId).on('input', function () {
                    var pin = this.value, hint = $('#' + hintId);
                    if (pin.length !== 6) { hint.text(''); return; }
                    hint.text('Looking up…');
                    $.getJSON('https://api.postalpincode.in/pincode/' + pin)
                        .done(function (r) {
                            var po = r && r[0] && r[0].Status === 'Success' && r[0].PostOffice && r[0].PostOffice[0];
                            if (!po) { hint.text('Please check this PIN code.'); return; }
                            if (!$('#' + cityId).val()) $('#' + cityId).val(po.District);
                            var st = $('#' + stateId);
                            if (st.find('option').filter(function () { return this.value === po.State; }).length) st.val(po.State);
                            hint.html('<i class="fa fa-check"></i> ' + po.District + ', ' + po.State);
                        })
                        .fail(function () { hint.text(''); });
                });
            }
            pinLookup('ShippingPincode', 'ShippingCity', 'ShippingState', 'ibPinHint');
            pinLookup('BillingPinCode', 'BillingCity', 'BillingState', 'ibBPinHint');

            // Before submit: copy whatever is hidden, prevent double submit
            form.on('submit', function () {
                if (!gift.is(':checked')) { $('#ShippingName').val($('#Name').val()); $('#ShippingPhone').val($('#Phone').val()); }
                if (same.is(':checked')) pairs.forEach(function (p) { $('#' + p[1]).val($('#' + p[0]).val()); });
                var b = $('#ibAddrBtn');
                setTimeout(function () { b.prop('disabled', true).text('Please wait…'); }, 0);
            });

            syncBilling();
            syncGift();
        })();
    </script>
End Section

<div class="ib-addr container fullbody py-4">

    <!-- Progress -->
    <ol class="progress-steps">
        <li class="done"><a href="~/cart"><span>1</span> Cart</a></li>
        <li class="on"><span>2</span> Address</li>
        <li><span>3</span> Payment</li>
    </ol>

    <div class="row justify-content-center">
        <div class="col-lg-9 col-xl-8">

            <h1>Where should we deliver?</h1>
            <p class="sub">Free shipping across India. Usually delivered in 2–10 working days.</p>

            @If ViewBag.Exception IsNot Nothing Then
                @<div class="msg-err" role="alert"><i class="fa fa-exclamation-circle" aria-hidden="true"></i> Sorry, something went wrong while saving your address. Please check the details and try again, or <a href="https://indiabobbles.tawk.help" target="_blank" rel="noopener">contact us</a>.</div>
            End If

            @Using (Html.BeginForm(Nothing, Nothing, FormMethod.Post, New With {.id = "ibAddrForm"}))
                @Html.AntiForgeryToken()
                @Html.ValidationSummary(True, "", New With {.class = "text-danger small"})

                @* ===== 1. Contact ===== *@
                @<div class="card-ib">
                    <h2><span class="n">1</span> Contact details</h2>
                    <div class="row g-3">
                        <div class="col-12">
                            <label for="Email" class="form-label">Email</label>
                            @Html.TextBoxFor(Function(m) m.Email, New With {.class = "form-control", .type = "email", .required = "required", .autocomplete = "email", .placeholder = "you@example.com"})
                            <div class="hint">Your order confirmation will be sent here.</div>
                            @Html.ValidationMessageFor(Function(m) m.Email, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-6">
                            <label for="Name" class="form-label">Full name</label>
                            @Html.TextBoxFor(Function(m) m.Name, New With {.class = "form-control", .required = "required", .autocomplete = "name", .placeholder = "e.g. Priya Sharma"})
                            @Html.ValidationMessageFor(Function(m) m.Name, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-6">
                            <label for="Phone" class="form-label">Mobile number</label>
                            <div class="input-group">
                                <span class="input-group-text">+91</span>
                                @Html.TextBoxFor(Function(m) m.Phone, New With {.class = "form-control", .type = "tel", .required = "required", .inputmode = "numeric", .maxlength = "10", .pattern = "[6-9][0-9]{9}", .title = "Please enter a valid 10-digit mobile number", .autocomplete = "tel-national", .placeholder = "10-digit mobile"})
                            </div>
                            <div class="hint">For delivery updates on WhatsApp.</div>
                            @Html.ValidationMessageFor(Function(m) m.Phone, "", New With {.class = "text-danger small"})
                        </div>
                    </div>
                </div>

                @* ===== 2. Delivery address ===== *@
                @<div class="card-ib">
                    <h2><span class="n">2</span> Delivery address</h2>
                    <div class="row g-3">
                        <div class="col-md-4">
                            <label for="ShippingPincode" class="form-label">PIN code</label>
                            @Html.TextBoxFor(Function(m) m.ShippingPincode, New With {.class = "form-control", .required = "required", .inputmode = "numeric", .maxlength = "6", .pattern = "[1-9][0-9]{5}", .title = "Please enter a 6-digit PIN code", .autocomplete = "postal-code", .placeholder = "6-digit PIN"})
                            <div class="hint" id="ibPinHint"></div>
                            @Html.ValidationMessageFor(Function(m) m.ShippingPincode, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-8">
                            <label for="ShippingAddress" class="form-label">House no., building, street, area</label>
                            @Html.TextBoxFor(Function(m) m.ShippingAddress, New With {.class = "form-control", .required = "required", .autocomplete = "street-address", .placeholder = "e.g. H104, Ajnara Daffodil, Sector 137"})
                            @Html.ValidationMessageFor(Function(m) m.ShippingAddress, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-4">
                            <label for="ShippingCity" class="form-label">City / District</label>
                            @Html.TextBoxFor(Function(m) m.ShippingCity, New With {.class = "form-control", .required = "required", .autocomplete = "address-level2"})
                            @Html.ValidationMessageFor(Function(m) m.ShippingCity, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-4">
                            <label for="ShippingState" class="form-label">State</label>
                            @Html.DropDownListFor(Function(m) m.ShippingState, statelist, New With {.class = "form-select", .required = "required"})
                            @Html.ValidationMessageFor(Function(m) m.ShippingState, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-4">
                            <label for="ShippingCountry" class="form-label">Country</label>
                            @Html.TextBoxFor(Function(m) m.ShippingCountry, New With {.class = "form-control", .readonly = "readonly"})
                        </div>
                    </div>

                    <!-- Gift / someone else -->
                    <div class="toggle mt-3">
                        <input type="checkbox" id="ibGift" class="form-check-input" @(If(giftDiffers, "checked", "")) />
                        <label for="ibGift"><b>🎁 Sending to someone else?</b> Add the recipient's name and phone for the courier.</label>
                    </div>
                    <div id="ibRecipient" class="row g-3 mt-1 d-none">
                        <div class="col-md-6">
                            <label for="ShippingName" class="form-label">Recipient's name</label>
                            @Html.TextBoxFor(Function(m) m.ShippingName, New With {.class = "form-control", .autocomplete = "off"})
                            @Html.ValidationMessageFor(Function(m) m.ShippingName, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-6">
                            <label for="ShippingPhone" class="form-label">Recipient's mobile</label>
                            <div class="input-group">
                                <span class="input-group-text">+91</span>
                                @Html.TextBoxFor(Function(m) m.ShippingPhone, New With {.class = "form-control", .type = "tel", .inputmode = "numeric", .maxlength = "10", .autocomplete = "off"})
                            </div>
                            @Html.ValidationMessageFor(Function(m) m.ShippingPhone, "", New With {.class = "text-danger small"})
                        </div>
                    </div>
                </div>

                @* ===== 3. Billing ===== *@
                @<div class="card-ib">
                    <h2><span class="n">3</span> Billing address</h2>
                    <div class="toggle">
                        <input type="checkbox" id="ibSameBilling" class="form-check-input" @(If(billingDiffers, "", "checked")) />
                        <label for="ibSameBilling"><b>Same as delivery address</b></label>
                    </div>

                    <div id="ibBilling" class="row g-3 mt-1">
                        <div class="col-md-4">
                            <label for="BillingPinCode" class="form-label">PIN code</label>
                            @Html.TextBoxFor(Function(m) m.BillingPinCode, New With {.class = "form-control", .data_req = "1", .inputmode = "numeric", .maxlength = "6", .autocomplete = "billing postal-code"})
                            <div class="hint" id="ibBPinHint"></div>
                            @Html.ValidationMessageFor(Function(m) m.BillingPinCode, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-8">
                            <label for="BillingAddress" class="form-label">House no., building, street, area</label>
                            @Html.TextBoxFor(Function(m) m.BillingAddress, New With {.class = "form-control", .data_req = "1", .autocomplete = "billing street-address"})
                            @Html.ValidationMessageFor(Function(m) m.BillingAddress, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-4">
                            <label for="BillingCity" class="form-label">City / District</label>
                            @Html.TextBoxFor(Function(m) m.BillingCity, New With {.class = "form-control", .data_req = "1", .autocomplete = "billing address-level2"})
                            @Html.ValidationMessageFor(Function(m) m.BillingCity, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-4">
                            <label for="BillingState" class="form-label">State</label>
                            @Html.DropDownListFor(Function(m) m.BillingState, statelist, New With {.class = "form-select", .data_req = "1"})
                            @Html.ValidationMessageFor(Function(m) m.BillingState, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-4">
                            <label for="BillingCountry" class="form-label">Country</label>
                            @Html.TextBoxFor(Function(m) m.BillingCountry, New With {.class = "form-control", .readonly = "readonly"})
                        </div>
                    </div>
                </div>

                @<div class="actions">
                    <a href="~/cart" class="back"><i class="fa fa-arrow-left" aria-hidden="true"></i> Back to cart</a>
                    <button type="submit" id="ibAddrBtn" class="btn btn-main">Continue to Payment <i class="fa fa-arrow-right" aria-hidden="true"></i></button>
                </div>
                @<p class="secure"><i class="fa fa-lock" aria-hidden="true"></i> Your details are safe with us and used only for this order. <a href="~/privacy-policy">Privacy Policy</a></p>
            End Using
        </div>
    </div>
</div>