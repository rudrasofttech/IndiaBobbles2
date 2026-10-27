@ModelType IndiaBobbles.Product

@Code
    ViewData("Title") = Model.Name

    ' Photos in display order (safe even if no photo has Sequence = 1)
    Dim photos = Model.ProductPhotoes.OrderBy(Function(t) t.Sequence).ToList()
    Dim mainPhoto As String = If(photos.Count > 0, photos(0).ImagePath, "")

    ' Indian number format: 1,000 / 1,00,000
    Dim inr = New System.Globalization.CultureInfo("en-IN")
    Dim salePrice As String = Model.SalePrice.ToString("#,##0.##", inr)
    Dim hasDiscount As Boolean = Model.MRP > Model.SalePrice
    Dim discountPct As Integer = If(hasDiscount AndAlso Model.MRP > 0, CInt(Math.Round((Model.MRP - Model.SalePrice) * 100 / Model.MRP)), 0)

    ' Short teaser from the description (HTML tags removed)
    Dim plainDesc As String = System.Text.RegularExpressions.Regex.Replace(If(Model.Description, ""), "<[^>]+>", " ")
    plainDesc = System.Web.HttpUtility.HtmlDecode(System.Text.RegularExpressions.Regex.Replace(plainDesc, "\s+", " ")).Trim()
    Dim teaser As String = If(plainDesc.Length > 170, plainDesc.Substring(0, 170).TrimEnd() & "…", plainDesc)

    Dim waUrl As String = Url.Action("WhatsAppRedirect", "Home", New With {.message = "Hi, I have a question about " & Model.Name})

    ' ---- Coupon offers (coupons marked "Offer it on the cart page" in admin) ----
    Dim offers As List(Of IndiaBobbles.CouponCode) = If(TryCast(ViewBag.Offers, List(Of IndiaBobbles.CouponCode)), New List(Of IndiaBobbles.CouponCode)())
    If Model.OutofStock Then offers = New List(Of IndiaBobbles.CouponCode)()
    offers = offers.Where(Function(o) Not o.ReturningOnly) _
                   .OrderBy(Function(o) Math.Max(o.MinItems, 1)) _
                   .ThenByDescending(Function(o) o.DiscountOn(Model.SalePrice * Math.Max(o.MinItems, 1))) _
                   .ToList()
    Dim offerCount As Integer = offers.Count
    ' Best price for ONE piece (e.g. FIRST100 → ₹900)
    Dim bestOne = offers.Where(Function(o) o.FitsCart(1, Model.SalePrice)) _
                        .OrderByDescending(Function(o) o.DiscountOn(Model.SalePrice)) _
                        .FirstOrDefault()
    Dim bestOnePrice As Decimal = If(bestOne Is Nothing, Model.SalePrice, Model.SalePrice - bestOne.DiscountOn(Model.SalePrice))
    ' Biggest saving any offer gives (each at its own minimum quantity), for "Up to ₹400 off"
    Dim maxSave As Decimal = If(offerCount = 0, 0D, offers.Max(Function(x) x.DiscountOn(Model.SalePrice * Math.Max(x.MinItems, 1))))

    ' SEO + social
    Dim siteUrl As String = IndiaBobbles.Utility.SiteURL.TrimEnd("/"c)
    Dim productPath As String = Url.Content("~/product/" & Model.ID & "/" & IndiaBobbles.Utility.Slugify(Model.Name))
    Dim productUrl As String = siteUrl & productPath
    ViewBag.CanonicalUrl = productUrl

    Dim metaTitle As String = Model.Name & " | India Bobbles"
    Dim metaDescription As String = If(String.IsNullOrWhiteSpace(teaser), "Buy hand-painted bobbleheads from India Bobbles.", teaser)
    Dim metaImage As String = siteUrl & "/theme/khichdi/img/ib-logo.png"

    If Not String.IsNullOrWhiteSpace(mainPhoto) Then
        If mainPhoto.StartsWith("//") Then
            metaImage = "https:" & mainPhoto
        ElseIf mainPhoto.StartsWith("http://", StringComparison.OrdinalIgnoreCase) OrElse mainPhoto.StartsWith("https://", StringComparison.OrdinalIgnoreCase) Then
            metaImage = mainPhoto
        Else
            metaImage = siteUrl & If(mainPhoto.StartsWith("/"), mainPhoto, "/" & mainPhoto)
        End If
    End If
End Code

@Functions
    ' True when a field has something worth showing
    Public Shared Function Has(v As Object) As Boolean
        Return v IsNot Nothing AndAlso v.ToString().Trim() <> ""
    End Function
End Functions

@section head

    @* Product info for Google search results *@
    <script type="application/ld+json">
    {
      "@@context": "https://schema.org",
      "@@type": "Product",
      "name": "@Html.Raw(System.Web.HttpUtility.JavaScriptStringEncode(Model.Name))",
      "image": "@Html.Raw(System.Web.HttpUtility.JavaScriptStringEncode(mainPhoto))",
      "description": "@Html.Raw(System.Web.HttpUtility.JavaScriptStringEncode(teaser))",
      "brand": { "@@type": "Brand", "name": "India Bobbles" },
      "offers": {
        "@@type": "Offer",
        "priceCurrency": "INR",
        "price": "@(Model.SalePrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture))",
        "availability": "@(If(Model.OutofStock, "https://schema.org/OutOfStock", "https://schema.org/InStock"))"
      }
    }
    </script>

    <style>
        /* ---- Offers box ---- */
        .ib-pdp .lowest {
            margin: 2px 0 4px;
            font-size: 15px;
            color: #157a3c;
            font-weight: 700;
        }

            .ib-pdp .lowest b {
                font-size: 17px;
            }

        .ib-pdp .offers {
            border: 1px solid #ece6ef;
            border-radius: 14px;
            padding: 12px 14px;
            margin: 0 0 18px;
            background: #fff;
            scroll-margin-top: 90px;
            transition: box-shadow .3s;
        }

            .ib-pdp .offers.flash {
                box-shadow: 0 0 0 4px rgba(255,193,7,.55);
            }

            .ib-pdp .offers .oh {
                font-weight: 800;
                color: #330B3F;
                font-size: 15px;
                margin-bottom: 8px;
            }

                .ib-pdp .offers .oh i {
                    color: #e23744;
                    margin-right: 4px;
                }

            .ib-pdp .offers .of {
                display: flex;
                align-items: center;
                gap: 12px;
                padding: 9px 0;
                border-top: 1px dashed #e4dbe9;
            }

                .ib-pdp .offers .of:first-of-type {
                    border-top: 0;
                    padding-top: 2px;
                }

            .ib-pdp .offers .cd {
                flex: none;
                font-family: Consolas, Menlo, monospace;
                font-weight: 700;
                font-size: 12px;
                letter-spacing: 1px;
                color: #330B3F;
                background: #fff4cc;
                border: 1px dashed #e0b400;
                border-radius: 8px;
                padding: 4px 8px;
            }

            .ib-pdp .offers .tx {
                flex: 1;
                min-width: 0;
                font-size: 14px;
                color: #3e3445;
                line-height: 1.35;
            }

                .ib-pdp .offers .tx b {
                    color: #1f1724;
                }

                .ib-pdp .offers .tx small {
                    display: block;
                    color: #157a3c;
                    font-weight: 600;
                    font-size: 12.5px;
                    margin-top: 1px;
                }

            .ib-pdp .offers .ft {
                font-size: 12px;
                color: #6b5f72;
                margin-top: 6px;
            }

        /* ---- Mobile sticky bar: price + coupon count ---- */
        .ib-mbar .pb {
            display: flex;
            flex-direction: column;
            line-height: 1.15;
            min-width: 0;
        }

            .ib-mbar .pb .price {
                font-size: 20px;
                font-weight: 800;
                color: #330B3F;
            }

            .ib-mbar .pb .mcoup {
                font-size: 12px;
                font-weight: 700;
                color: #157a3c;
                text-decoration: none;
                margin-top: 2px;
                line-height: 1.25;
            }

                .ib-mbar .pb .mcoup i {
                    margin-right: 3px;
                }

                .ib-mbar .pb .mcoup u {
                    text-decoration-style: dotted;
                }

        /* ---- Phones only: smaller photo + one scrolling row of thumbnails, so name & price show sooner ---- */
        @@media (max-width: 767.98px) {
            .ib-pdp .main-img img {
                max-height: 40vh;
                width: auto;
                max-width: 100%;
                object-fit: contain;
            }

            .ib-pdp .thumbs {
                display: flex !important;
                flex-wrap: nowrap !important;
                overflow-x: auto;
                gap: 8px;
                justify-content: flex-start !important;
                padding-bottom: 4px;
                scroll-snap-type: x proximity;
                -webkit-overflow-scrolling: touch;
                scrollbar-width: none;
            }

                .ib-pdp .thumbs::-webkit-scrollbar {
                    display: none;
                }

                .ib-pdp .thumbs button {
                    flex: 0 0 58px !important;
                    width: 58px !important;
                    height: 58px !important;
                    scroll-snap-align: start;
                    padding: 2px;
                }

                    .ib-pdp .thumbs button img {
                        width: 100%;
                        height: 100%;
                        object-fit: cover;
                    }

            .ib-pdp .offers .of {
                gap: 10px;
            }
        }
    </style>
End Section

@section scripts
    <script>
        // Thumbnails: swap the main photo
        $(document).on('click', '.ib-pdp .thumbs button', function () {
            var src = $(this).data('src');
            $('#ibMainImg').attr('src', src);
            $('.ib-pdp .thumbs button').removeClass('sel');
            $(this).addClass('sel');
        });

        // Click main photo: open large view
        $(document).on('click', '#ibMainWrap', function () {
            $('#ibZoomImg').attr('src', $('#ibMainImg').attr('src'));
            bootstrap.Modal.getOrCreateInstance(document.getElementById('ibZoom')).show();
        });

        // "Read the full story" link: open the Story tab and scroll to it
        $(document).on('click', '.js-open-story', function (e) {
            e.preventDefault();
            bootstrap.Tab.getOrCreateInstance(document.getElementById('tab-story-btn')).show();
            document.getElementById('ibTabs').scrollIntoView({ behavior: 'smooth' });
        });

        // "{n} coupon codes" (mobile bar / price line): scroll to the offers box and highlight it
        $(document).on('click', '.js-offers', function (e) {
            var box = document.getElementById('ibOffers');
            if (!box) return;
            e.preventDefault();
            box.scrollIntoView({ behavior: 'smooth', block: 'center' });
            box.classList.add('flash');
            setTimeout(function () { box.classList.remove('flash'); }, 1600);
        });

        function notifyMe(productId) {
            var email = $('#notifyEmail').val().trim();
            var honeypot = $('#notifyPhone').val();
            var btn = $('#notifyBtn');
            var msg = $('#notifyMsg');

            if (email === '') {
                msg.removeClass('text-success text-danger').addClass('text-danger').text('Please enter your email address.');
                return;
            }

            // Basic client-side email format check
            var emailRegex = /^[^\s@@]+@@[^\s@@]+\.[^\s@@]+$/;
            if (!emailRegex.test(email)) {
                msg.removeClass('text-success text-danger').addClass('text-danger').text('Please enter a valid email address.');
                return;
            }

            // Disable button to prevent double submission
            btn.prop('disabled', true).text('Sending...');
            msg.text('');

            $.ajax({
                url: '@Url.Action("NotifyMe", "Product")',
                type: 'POST',
                data: {
                    productId: productId,
                    email: email,
                    honeypot: honeypot,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (response) {
                    if (response.success) {
                        $('#notifyEmail').val('');
                        msg.removeClass('text-danger').addClass('text-success').text(response.message);
                        btn.prop('disabled', true).text('Notified');
                    } else {
                        msg.removeClass('text-success').addClass('text-danger').text(response.message);
                        btn.prop('disabled', false).text('Notify me');
                    }
                },
                error: function () {
                    msg.removeClass('text-success').addClass('text-danger').text('Something went wrong. Please try again.');
                    btn.prop('disabled', false).text('Notify me');
                }
            });
        }
    </script>
End Section

@section meta
    <meta name="description" content="@metaDescription" />
    <meta name="robots" content="index,follow,max-image-preview:large" />
    <meta name="keywords" content="@Model.Name, bobblehead, collectible, India Bobbles" />

    <meta property="og:locale" content="en_IN" />
    <meta property="og:type" content="product" />
    <meta property="og:site_name" content="India Bobbles" />
    <meta property="og:title" content="@metaTitle" />
    <meta property="og:description" content="@metaDescription" />
    <meta property="og:url" content="@productUrl" />
    <meta property="og:image" content="@metaImage" />
    <meta property="og:image:alt" content="@Model.Name" />

    <meta property="product:price:amount" content="@Model.SalePrice.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)" />
    <meta property="product:price:currency" content="INR" />
    <meta property="product:availability" content="@(If(Model.OutofStock, "out of stock", "in stock"))" />

    <meta name="twitter:card" content="summary_large_image" />
    <meta name="twitter:title" content="@metaTitle" />
    <meta name="twitter:description" content="@metaDescription" />
    <meta name="twitter:image" content="@metaImage" />
End Section

<div class="ib-pdp container bg-white py-4 px-3 px-md-4">

    <!-- Breadcrumb -->
    <nav class="crumb mb-3" aria-label="breadcrumb">
        <a href="~/">Home</a> &rsaquo; <a href="~/tag/collectibles">Bobbles</a> &rsaquo; <span>@Model.Name</span>
    </nav>

    <div class="row g-4 g-lg-5">

        <!-- ============ Photos ============ -->
        <div class="col-md-6">
            @If photos.Count > 0 Then
                @<div>
                    <div class="main-img" id="ibMainWrap" title="Click to enlarge">
                        @If Model.Handmade Then
                            @<span class="img-badge badge-hp"><i class="fa fa-paint-brush" aria-hidden="true"></i> Hand-painted</span>
                        End If
                        @If Model.OutofStock Then
                            @<span class="img-badge badge-oos">Out of stock</span>
                        End If
                        <img id="ibMainImg" src="@mainPhoto" alt="@Model.Name" />
                        <span class="zoom-hint"><i class="fa fa-search-plus" aria-hidden="true"></i> Click to zoom</span>
                    </div>

                    @If photos.Count > 1 Then
                        @<div class="thumbs">
                            @For i As Integer = 0 To photos.Count - 1
                                Dim pp = photos(i)
                                @<button type="button" class="@(If(i = 0, "sel", ""))" data-src="@pp.ImagePath" aria-label="Photo @(i + 1)">
                                    <img src="@pp.ImagePath" alt="@Model.Name photo @(i + 1)" loading="lazy" />
                                </button>
                            Next
                        </div>
                    End If
                </div>
            End If
        </div>

        <!-- ============ Buy box ============ -->
        <div class="col-md-6">
            @If Has(Model.Material) Then
                @<span class="chip">@Model.Material</span>
            End If
            @If Model.Handmade Then
                @<span class="chip">Hand-painted</span>
            End If

            <h1 class="mt-1 mb-2">@Model.Name</h1>

            @If teaser <> "" Then
                @<p class="teaser mb-3">@teaser <a href="#ibTabs" class="js-open-story">Read the full story</a></p>
            End If

            <div class="mb-1">
                <span class="price">₹@(salePrice)</span>
                @If hasDiscount Then
                    @<s class="mrp">₹@(Model.MRP.ToString("#,##0.##", inr))</s>
                    @<span class="off">@discountPct% off</span>
                End If
            </div>
            @*@If bestOne IsNot Nothing AndAlso bestOnePrice < Model.SalePrice Then
                @<div class="lowest"><i class="fa fa-tag" aria-hidden="true"></i> Get it for <b>₹@(bestOnePrice.ToString("#,##0", inr))</b> with @bestOne.Code</div>
            End If*@
            <div class="tax mb-3">Inclusive of all taxes · Free shipping across India</div>

            <!-- Offers -->
            @If offerCount > 0 Then
                @<div class="offers" id="ibOffers">
                    <div class="oh"><i class="fa fa-ticket" aria-hidden="true"></i> @offerCount @(If(offerCount = 1, "offer", "offers")) for you</div>
                    @For Each o In offers
                        Dim qty As Integer = Math.Max(o.MinItems, 1)
                        Dim cartValue As Decimal = Model.SalePrice * qty
                        Dim save As Decimal = o.DiscountOn(cartValue)
                        Dim pitchText As String = If(String.IsNullOrWhiteSpace(o.Pitch), If(qty > 1, "when you buy " & qty & " or more", "on your order"), o.Pitch)
                        Dim note As String
                        If qty = 1 Then
                            note = "You pay ₹" & (Model.SalePrice - save).ToString("#,##0", inr)
                        Else
                            note = "Save ₹" & save.ToString("#,##0", inr) & " on " & qty & " bobbleheads"
                        End If
                        If o.MinAmount > 0 AndAlso cartValue < o.MinAmount Then note = "On orders of ₹" & o.MinAmount.ToString("#,##0", inr) & " or more"
                        If o.ValidTo.HasValue Then note &= " · till " & o.ValidTo.Value.ToString("d MMM", inr)
                        @<div class="of">
                            <span class="cd">@o.Code</span>
                            <div class="tx"><b>@o.Label()</b> @pitchText<small>@note</small></div>
                        </div>
                    Next
                    <div class="ft"><i class="fa fa-info-circle" aria-hidden="true"></i> Apply in one tap on the cart page. One coupon per order.</div>
                </div>
            End If

            <div class="trust mb-4">
                <div><i class="fa fa-truck" aria-hidden="true"></i>Free delivery across India</div>
                <div><i class="fa fa-clock-o" aria-hidden="true"></i>Dispatched in 2 business days</div>
                <div><i class="fa fa-paint-brush" aria-hidden="true"></i>Hand-painted by artists</div>
                <div><i class="fa fa-lock" aria-hidden="true"></i>UPI, cards, net banking</div>
            </div>

            @If Not Model.OutofStock Then
                @<form method="get" action="@Url.Content("~/cart/add/" & Model.ID)" class="mb-3">
                    <button class="btn btn-ib w-100"><i class="fa fa-shopping-cart" aria-hidden="true"></i> Add to Cart</button>
                </form>
            Else
                @<div class="mb-3">
                    @Html.AntiForgeryToken()
                    <button type="button" class="btn btn-outline-secondary w-100 mb-2" disabled>Out of stock</button>
                    <p class="small mb-2">Get an email as soon as it's back in stock:</p>
                    <div class="d-flex">
                        <input type="email" id="notifyEmail" class="form-control me-2" placeholder="Your email" maxlength="250" />
                        <button type="button" class="btn btn-dark" id="notifyBtn" onclick="notifyMe(@Model.ID)">Notify me</button>
                    </div>
                    <input type="text" id="notifyPhone" name="phone" style="display:none;" tabindex="-1" autocomplete="off" />
                    <div id="notifyMsg" class="mt-2 small"></div>
                </div>
            End If

            <a target="_blank" rel="noopener" class="btn btn-wa w-100 mb-3" href="@waUrl">
                <i class="fa fa-whatsapp" aria-hidden="true"></i> Questions? Chat with us on WhatsApp
            </a>

            <p class="small-links mb-0">
                <i class="fa fa-undo" aria-hidden="true"></i> 7-day returns (store credit) ·
                <a href="~/shipping-policy">Shipping &amp; Returns</a> ·
                <a href="https://indiabobbles.tawk.help" target="_blank" rel="noopener">Help Centre</a>
            </p>
        </div>
    </div>

    <!-- ============ Tabs ============ -->
    <div class="mt-5" id="ibTabs">
        <ul class="nav nav-tabs" role="tablist">
            <li class="nav-item" role="presentation">
                <button class="nav-link active" data-bs-toggle="tab" data-bs-target="#tab-details" type="button" role="tab">Details</button>
            </li>
            <li class="nav-item" role="presentation">
                <button class="nav-link" id="tab-story-btn" data-bs-toggle="tab" data-bs-target="#tab-story" type="button" role="tab">The Story</button>
            </li>
            <li class="nav-item" role="presentation">
                <button class="nav-link" data-bs-toggle="tab" data-bs-target="#tab-shipping" type="button" role="tab">Shipping &amp; Returns</button>
            </li>
            @If Has(Model.CareInstructions) Then
                @<li class="nav-item" role="presentation">
                    <button class="nav-link" data-bs-toggle="tab" data-bs-target="#tab-care" type="button" role="tab">Care</button>
                </li>
            End If
        </ul>

        <div class="tab-content pt-4">
            <!-- Details -->
            <div class="tab-pane fade show active" id="tab-details" role="tabpanel">
                <div class="spec">
                    @If Has(Model.Dimension) Then
                        @<div><span>Dimensions</span><span>@Model.Dimension</span></div>
                    End If
                    @If Has(Model.Material) Then
                        @<div><span>Material</span><span>@Model.Material</span></div>
                    End If
                    @If Has(Model.Weight) Then
                        @<div><span>Weight</span><span>@Model.Weight</span></div>
                    End If
                    @If Has(Model.Color) Then
                        @<div><span>Colour</span><span>@Model.Color</span></div>
                    End If
                    <div><span>Handmade</span><span>@(If(Model.Handmade, "Yes, hand-painted", "No"))</span></div>
                    <div><span>Fragile</span><span>@(If(Model.Fragile, "Yes, packed with care", "No"))</span></div>
                    @If Has(Model.RecommendedAge) Then
                        @<div><span>Recommended age</span><span>@Model.RecommendedAge</span></div>
                    End If
                    @If Has(Model.Manufacturer) Then
                        @<div><span>Manufacturer</span><span>@Model.Manufacturer</span></div>
                    End If
                    @If Has(Model.CountryofOrigin) Then
                        @<div><span>Country of origin</span><span>@Model.CountryofOrigin</span></div>
                    End If
                    @If Has(Model.ShippingTime) Then
                        @<div><span>Shipping time</span><span>@Model.ShippingTime</span></div>
                    End If
                </div>
            </div>

            <!-- Story (full description, HTML allowed) -->
            <div class="tab-pane fade" id="tab-story" role="tabpanel">
                <div class="story">@Html.Raw(Model.Description)</div>
            </div>

            <!-- Shipping & returns -->
            <div class="tab-pane fade" id="tab-shipping" role="tabpanel">
                <ul class="story ps-3">
                    <li><b>Free shipping</b> on every order across India.</li>
                    <li>Dispatched within <b>2 business days</b>. Delivery in 2–3 days to metro cities, up to 8–10 working days elsewhere.</li>
                    <li><b>7-day returns</b> from shipment, refunded as store credit. Items must be unused and in original packaging.</li>
                    <li><b>Damaged in transit?</b> Email photos to <a href="mailto:indiabobbles@rudrasofttech.com">indiabobbles@rudrasofttech.com</a> within 4 hours of delivery for a replacement or refund.</li>
                </ul>
                <a href="~/shipping-policy">Read the full Shipping &amp; Refund policy &rsaquo;</a>
            </div>

            @If Has(Model.CareInstructions) Then
                @<div class="tab-pane fade" id="tab-care" role="tabpanel">
                    <p class="story">@Model.CareInstructions</p>
                </div>
            End If
        </div>
    </div>
</div>

<!-- Zoom modal -->
<div class="modal fade" id="ibZoom" tabindex="-1" aria-label="@Model.Name photo" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered modal-lg">
        <div class="modal-content border-0">
            <div class="modal-header border-0 pb-0">
                <h5 class="modal-title">@Model.Name</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body text-center">
                <img id="ibZoomImg" src="" alt="@Model.Name" style="max-width:100%; max-height:75vh;" />
            </div>
        </div>
    </div>
</div>

<!-- Mobile sticky Add to Cart (price + coupon count) -->
<div class="ib-mbar">
    <div class="pb">
        <span class="price">₹@(salePrice)</span>
        @If offerCount > 0 Then
            @<a href="#ibOffers" class="mcoup js-offers"><i class="fa fa-ticket" aria-hidden="true"></i><u>Up to ₹@(maxSave.ToString("#,##0", inr)) off via @offerCount coupon @(If(offerCount = 1, "code", "codes"))</u></a>
        End If
    </div>
    @If Not Model.OutofStock Then
        @<form method="get" action="@Url.Content("~/cart/add/" & Model.ID)" class="m-0">
            <button class="btn btn-ib" style="background:#330B3F;color:#fff;font-weight:700;border-radius:8px;padding:.6rem 1.4rem;">Add to Cart</button>
        </form>
    Else
        @<span class="badge bg-secondary p-2">Out of stock</span>
    End If
</div>