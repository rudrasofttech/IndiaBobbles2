@Imports IndiaBobbles
@Code
    ViewData("Title") = "Buy Bobble Heads & Figurines Online in India – India Bobbles"

    Dim inr = New System.Globalization.CultureInfo("en-IN")

    ' Featured products (ViewBag.Highlights) as a typed list
    Dim highlights As New List(Of Product)()
    If ViewBag.Highlights IsNot Nothing Then
        highlights = CType(ViewBag.Highlights, IEnumerable(Of Product)).ToList()
    End If

    ' Up to 3 photos for the hero collage
    Dim heroPics = highlights.Where(Function(x) Not String.IsNullOrEmpty(x.ThumbPath)).Take(3).ToList()
End Code

@Functions
    Public Shared Function ProductLink(p As Product, helper As System.Web.Mvc.UrlHelper) As String
        Return helper.Content("~/product/" & p.ID & "/" & IndiaBobbles.Utility.Slugify(p.Name))
    End Function
End Functions

@section meta
    <meta name="description" content="Shop the best range of Bollywood and Indian bobble heads each piece hand painted by master artists. Start collecting now, Free Shipping all over India." />
    <meta name="Keywords" Content="buy bobblehead online india, buy bobbleheads online, bobble head figurines, bobble heads, indian bobble heads, india bobbles, indian figurines" />
End Section

@section head
    <meta property="og:type" content="website" />
    <meta property="og:url" content="https://www.indiabobbles.com/" />
    <meta property="og:site_name" content="India Bobbles" />
    <meta property="og:title" content="India Bobbles - Hand-painted Bollywood & Indian bobbleheads" />
    <meta property="og:description" content="Shop the best range of Bollywood and Indian bobble heads each piece hand painted by master artists. Start collecting now, Free Shipping all over India." />
    <meta property="og:image" content="https://www.indiabobbles.com/theme/khichdi/img/ib-logo.png" />
End Section

@section scripts
    <script>
        function ibSubscribe() {
            var email = $('#subEmail').val().trim();
            var msg = $('#subMsg');
            var btn = $('#subBtn');
            if (!/^[^\s@@]+@@[^\s@@]+\.[^\s@@]+$/.test(email)) {
                msg.text('Please enter a valid email address.').css('color', '#ffb3b3');
                return;
            }
            btn.prop('disabled', true).text('Subscribing...');
            $.ajax({
                url: '@Url.Action("Subscribe", "Home")',
                type: 'POST',
                data: {
                    email: email,
                    honeypot: $('#subPhone').val(),
                    __RequestVerificationToken: $('#ibNews input[name="__RequestVerificationToken"]').val()
                },
                success: function (r) {
                    if (r.success) {
                        $('#subEmail').val('');
                        msg.text(r.message || 'Thank you! You are subscribed.').css('color', '#ffc107');
                        btn.text('Subscribed');
                    } else {
                        msg.text(r.message || 'Something went wrong.').css('color', '#ffb3b3');
                        btn.prop('disabled', false).text('Subscribe');
                    }
                },
                error: function () {
                    msg.text('Something went wrong. Please try again.').css('color', '#ffb3b3');
                    btn.prop('disabled', false).text('Subscribe');
                }
            });
        }
    </script>
End Section

<div class="ib-home">

    <!-- ============ Hero ============ -->
    <section class="hero">
        <div class="container py-5">
            <div class="row align-items-center g-4">
                <div class="col-lg-6">
                    <div class="kick">Hand-painted · Made in India</div>
                    <h1>Bollywood on <em>your desk.</em></h1>
                    <p class="mb-4">Iconic heroes, legendary villains and national icons, each bobblehead hand-painted by master artists. Free shipping across India.</p>
                    <div class="d-flex flex-wrap gap-2">
                        <a class="btn btn-iby" href="~/tag/collectibles">Shop Bobbleheads <i class="fa fa-arrow-right" aria-hidden="true"></i></a>
                        <a class="btn btn-ibw" href="~/order-custom-bobbleheads">Make a Custom One</a>
                    </div>
                </div>
                @If heroPics.Count > 0 Then
                    @<div class="col-lg-6">
                        <div class="trio">
                            @For i As Integer = 0 To heroPics.Count - 1
                                Dim hp = heroPics(i)
                                @<a class="t@(i)" href="@(ProductLink(hp, Url))" title="@hp.Name"><img src="@hp.ThumbPath" alt="@hp.Name" /></a>
                            Next
                        </div>
                    </div>
                End If
            </div>
        </div>
    </section>

    <!-- ============ Trust strip ============ -->
    <div class="strip">
        <div class="container d-flex flex-wrap justify-content-around gap-3 py-3">
            <span><i class="fa fa-truck" aria-hidden="true"></i>Free delivery</span>
            <span><i class="fa fa-paint-brush" aria-hidden="true"></i>Hand-painted</span>
            <span><i class="fa fa-lock" aria-hidden="true"></i>UPI · Cards · Net banking</span>
            <span><i class="fa fa-undo" aria-hidden="true"></i>7-day returns</span>
        </div>
    </div>

    <!-- ============ Products ============ -->
    @If highlights.Count > 0 Then
        @<section class="sec">
            <div class="container">
                <div class="d-flex justify-content-between align-items-end flex-wrap gap-2 mb-4">
                    <div>
                        <h2>Shop the collection</h2>
                        <p class="lead-sm">Our bestsellers, each one a little piece of filmy history.</p>
                    </div>
                    <a href="~/tag/collectibles" class="fw-bold text-decoration-none" style="color:#330B3F">View all <i class="fa fa-arrow-right" aria-hidden="true"></i></a>
                </div>

                <div class="row row-cols-2 row-cols-md-3 row-cols-lg-4 g-3 g-md-4">
                    @For Each item In highlights
                        Dim productUrl As String = ProductLink(item, Url)
                        Dim hasDiscount As Boolean = item.MRP > item.SalePrice
                        Dim offPct As Integer = If(hasDiscount AndAlso item.MRP > 0, CInt(Math.Round((item.MRP - item.SalePrice) * 100 / item.MRP)), 0)
                        Dim showPrice = If(hasDiscount, item.SalePrice, item.MRP)

                        @<div class="col">
                            <div class="ib-card @(If(item.OutofStock, "is-oos", ""))">
                                <a class="ib-img" href="@productUrl">
                                    @If Not String.IsNullOrEmpty(item.ThumbPath) Then
                                        @<img src="@item.ThumbPath" alt="Photo of @item.Name" loading="lazy" />
                                    End If
                                    @If hasDiscount Then
                                        @<span class="ib-off">@offPct% OFF</span>
                                    End If
                                    @If item.OutofStock Then
                                        @<span class="ib-oos">Out of stock</span>
                                    End If
                                </a>
                                <div class="ib-body">
                                    <a class="ib-name" href="@productUrl">@item.Name</a>
                                    <div class="ib-price">
                                        <b>₹@(showPrice.ToString("#,##0.##", inr))</b>
                                        @If hasDiscount Then
                                            @<s>₹@(item.MRP.ToString("#,##0.##", inr))</s>
                                        End If
                                    </div>
                                    <div class="ib-ship"><i class="fa fa-check" aria-hidden="true"></i> Free shipping</div>
                                    <div class="ib-actions">
                                        <a class="ib-btn ib-btn-o" href="@productUrl">View</a>
                                        @If Not item.OutofStock Then
                                            @<form method="get" action="@Url.Content("~/cart/add/" & item.ID)">
                                                <button class="ib-btn ib-btn-p">Add to Cart</button>
                                            </form>
                                        Else
                                            @<a class="ib-btn ib-btn-g" href="@productUrl"><i class="fa fa-bell" aria-hidden="true"></i> Notify me</a>
                                        End If
                                    </div>
                                </div>
                            </div>
                        </div>
                    Next
                </div>
            </div>
        </section>
    End If

    <!-- ============ Custom bobbleheads ============ -->
    <section class="sec pt-0">
        <div class="container">
            <div class="custom row g-4 align-items-center mx-0">
                <div class="col-lg-6">
                    <h3>Your face. Our artists. One-of-a-kind.</h3>
                    <p class="mb-3">Send us a photo, and we'll hand-paint a custom bobblehead of you, your partner or your boss.</p>
                    <div class="occ mb-3"><span>💍 Weddings</span><span>🎂 Birthdays</span><span>🪔 Diwali</span><span>🏢 Corporate gifts</span></div>
                    <a class="btn btn-ibp" href="~/order-custom-bobbleheads">Order a Custom Bobblehead</a>
                </div>
                <div class="col-lg-6">
                    <div class="demo">
                        <div class="photo"><i class="fa fa-camera" aria-hidden="true"></i></div>
                        <div class="arrow"><i class="fa fa-long-arrow-right" aria-hidden="true"></i></div>
                        @If heroPics.Count > 0 Then
                            @<img src="@(heroPics(0).ThumbPath)" alt="Hand-painted bobblehead" />
                        Else
                            @<div class="photo"><i class="fa fa-paint-brush" aria-hidden="true"></i></div>
                        End If
                    </div>
                </div>
            </div>
        </div>
    </section>

    <!-- ============ Newsletter ============ -->
    @*<section class="sec pt-0">
        <div class="container">
            <div class="news d-flex justify-content-between align-items-center flex-wrap gap-3" id="ibNews">
                <div>
                    <h3>Join the India Bobbles club</h3>
                    <p>New launches, festival offers and members-only discounts in your inbox.</p>
                </div>
                <div>
                    @Html.AntiForgeryToken()
                    <div class="d-flex flex-wrap gap-2">
                        <input type="email" id="subEmail" placeholder="Your email address" maxlength="250" aria-label="Email address" />
                        <input type="text" id="subPhone" style="display:none;" tabindex="-1" autocomplete="off" />
                        <button type="button" id="subBtn" class="btn btn-iby" onclick="ibSubscribe()">Subscribe</button>
                    </div>
                    <div id="subMsg" class="small mt-2"></div>
                    <div class="small mt-1" style="color:#cbb8d4">You can unsubscribe anytime. See our <a href="~/privacy-policy" style="color:#ffc107">Privacy Policy</a>.</div>
                </div>
            </div>
        </div>
    </section>*@
</div>