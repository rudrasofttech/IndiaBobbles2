<!DOCTYPE html>
<html>
<head>
    @RenderSection("meta", required:=False)
    @If Not String.IsNullOrEmpty(ViewBag.CanonicalUrl) Then
        @<link rel="canonical" href="@ViewBag.CanonicalUrl" />
    End If
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="google-site-verification" content="rtXgIUyaLbBtd_tsln3F6ZB9ZboSPHZe7K_zB6uRgv8" />
    <link rel="shortcut icon" href="~/theme/khichdi/favicon.jpg" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3" crossorigin="anonymous" />
    <link href="~/theme/khichdi/css/indiabobbles.css" rel="stylesheet" />
    <link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.6.3/css/font-awesome.min.css" rel="stylesheet" />
    @Scripts.Render("~/bundles/modernizr")
    @RenderSection("head", required:=False)
</head>
<body>
    @Code
        Dim om = New IndiaBobbles.OrderManager()
        Dim o As IndiaBobbles.Order = om.GetCart()
    End Code

    <div id="fb-root"></div>
    <script type="text/javascript">
        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) return;
            js = d.createElement(s); js.id = id;
            js.src = "//connect.facebook.net/en_US/sdk.js#xfbml=1&version=v2.7&appId=490407121012387";
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));</script>
    <!-- Announcement bar -->
    <div class="ib-topbar text-center py-2 px-2">
        <i class="fa fa-truck" aria-hidden="true"></i>Free shipping across India
        <span class="d-none d-md-inline">&nbsp;·&nbsp;<i class="fa fa-paint-brush" aria-hidden="true"></i>Every piece hand-painted by artists</span>
        &nbsp;·&nbsp;<a href="https://indiabobbles.tawk.help" target="_blank" rel="noopener">Need help?</a>
    </div>

    <!-- Main header -->
    <header class="ib-header sticky-top">
        <nav class="navbar navbar-expand-lg container py-2">

            <a class="navbar-brand" href="~/"><img alt="India Bobbles Logo" src="~/theme/khichdi/img/ib-logo.png" /></a>

            <!-- Cart + menu button (mobile) -->
            <div class="d-flex align-items-center d-lg-none ms-auto">
                <a href="@Url.Content("~/cart")" class="ib-cart me-2" title="Cart">
                    <i class="fa fa-shopping-cart" aria-hidden="true"></i>
                    @If o.OrderItems.Count > 0 Then
                        @<span class="ib-cart-badge">@o.OrderItems.Count</span>
                    End If
                </a>
                <button class="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#ibNav" aria-controls="ibNav" aria-expanded="false" aria-label="Toggle menu">
                    <span class="navbar-toggler-icon"></span>
                </button>
            </div>

            <div class="collapse navbar-collapse" id="ibNav">
                <ul class="navbar-nav mx-lg-auto">
                    <li class="nav-item"><a href="~/tag/collectibles" class="nav-link">Bobbles</a></li>
                    <li class="nav-item"><a href="~/order-custom-bobbleheads" class="nav-link">Custom</a></li>
                    <li class="nav-item"><a href="~/blog" class="nav-link">Blog</a></li>
                    <li class="nav-item"><a href="~/orders" class="nav-link">My Orders</a></li>
                    <li class="nav-item"><a href="https://indiabobbles.tawk.help" target="_blank" rel="noopener" class="nav-link">Help</a></li>
                </ul>

                <div class="ib-account d-flex align-items-center gap-2">
                    <!-- Cart (desktop) -->
                    <a href="@Url.Content("~/cart")" class="ib-cart me-2 d-none d-lg-inline-block" title="Cart">
                        <i class="fa fa-shopping-cart" aria-hidden="true"></i>
                        @If o.OrderItems.Count > 0 Then
                            @<span class="ib-cart-badge">@o.OrderItems.Count</span>
                        End If
                    </a>

                    @If Request.IsAuthenticated Then
                        @<a href="~/account/manageprofile" class="btn btn-ib btn-sm px-3"><i class="fa fa-user" aria-hidden="true"></i> Profile</a>
                        @<a href="~/account/logout" class="btn btn-ib-outline btn-sm px-3">Logout</a>
                    Else
                        @<a href="~/account/otplogin" class="btn btn-ib-outline btn-sm px-3">Login</a>
                        @<a href="~/account/register" class="btn btn-ib btn-sm px-3">Sign up</a>
                    End If
                </div>
            </div>
        </nav>
    </header>
    @If o.OrderItems.Count > 0 AndAlso Not Request.Path.ToLower().StartsWith("/cart") AndAlso (String.IsNullOrEmpty(o.Name) OrElse String.IsNullOrEmpty(o.Email) OrElse String.IsNullOrEmpty(o.Phone)) Then
        @<div class="alert alert-primary rounded-0 text-center" role="alert">
            Your order is missing Name, Email and Phone. <button type="button" id="addcontactorderanchor" data-bs-toggle="modal" data-bs-target="#orderContactModal" class="btn btn-link">Add Now</button>
        </div>
    End If
    @RenderBody()
    @If o IsNot Nothing Then
        @<div Class="modal fade" id="orderContactModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div Class="modal-dialog">
                <div Class="modal-content">
                    <div Class="modal-header">
                        <h5 Class="modal-title" id="exampleModalLabel">Contact Information</h5>
                        <Button type="button" Class="btn-close" data-bs-dismiss="modal" aria-label="Close"></Button>
                    </div>
                    <div Class="modal-body">
                        <p>
                            Your order is missing crucial contact information.
                        </p>
                        <form method="get" action="@Url.Content("~/cart/updatecontact")">
                            <div class="mb-2">
                                <label for="ordercontactnametxt" class="form-label">Name</label>
                                <input type="text" maxlength="100" value="@o.Name" name="name" class="form-control" id="ordercontactnametxt" />
                            </div>
                            <div class="mb-2">
                                <label for="ordercontactemailtxt" class="form-label">Email</label>
                                <input type="email" maxlength="250" value="@o.Email" name="email" class="form-control" id="ordercontactemailtxt" />
                            </div>
                            <div class="mb-2">
                                <label for="ordercontactphonetxt" class="form-label">Phone</label>
                                <input type="text" maxlength="15" name="Phone" value="@o.Phone" class="form-control" id="ordercontactphonetxt" />
                            </div>
                            <div class="text-end">
                                <button type="submit" class="btn btn-primary">Save</button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    End If
    <footer class="ib-footer mt-auto">
        <div class="container py-5">
            <div class="row g-4">

                <!-- Brand -->
                <div class="col-12 col-lg-4">
                    <a href="~/"><img src="~/theme/khichdi/img/ib-logo.png" alt="India Bobbles" style="max-height:56px;" /></a>
                    <p class="ib-tagline mt-3 mb-3">Bollywood and Indian bobbleheads, each piece hand-painted by master artists.</p>
                    <div class="ib-social">
                        <a href="https://www.facebook.com/IndiaBobbles" target="_blank" rel="noopener" title="India Bobbles on Facebook"><i class="fa fa-facebook" aria-hidden="true"></i></a>
                        <a href="https://www.instagram.com/indiabobbles" target="_blank" rel="noopener" title="India Bobbles on Instagram"><i class="fa fa-instagram" aria-hidden="true"></i></a>
                    </div>
                </div>

                <!-- Shop -->
                <div class="col-6 col-md-4 col-lg-2">
                    <h6>Shop</h6>
                    <ul>
                        <li><a href="~/tag/collectibles">All Bobbles</a></li>
                        <li><a href="~/collectibles">Collectibles</a></li>
                        <li><a href="~/order-custom-bobbleheads">Custom Bobbleheads</a></li>
                        <li><a href="~/games">Games</a></li>
                        <li><a href="~/blog">Blog</a></li>
                    </ul>
                </div>

                <!-- Help -->
                <div class="col-6 col-md-4 col-lg-3">
                    <h6>Help</h6>
                    <ul>
                        <li><a href="https://indiabobbles.tawk.help" target="_blank" rel="noopener">Help Centre &amp; FAQs</a></li>
                        <li><a href="~/orders">Track My Order</a></li>
                        <li><a href="~/shipping-policy">Shipping &amp; Refund</a></li>
                        <li><a href="~/payment-options">Payment Options</a></li>
                        <li><a href="~/contact">Contact Us</a></li>
                    </ul>
                </div>

                <!-- Account & company -->
                <div class="col-6 col-md-4 col-lg-3">
                    <h6>India Bobbles</h6>
                    <ul>
                        <li><a href="~/about">Our Story</a></li>
                        @If User.Identity.IsAuthenticated Then
                            @<li><a href="~/account/manageprofile">My Account</a></li>
                        Else
                            @<li><a href="~/account/login">Login</a> / <a href="~/account/register">Register</a></li>
                        End If
                        <li><a href="~/privacy-policy">Privacy Policy</a></li>
                        <li><a href="~/terms-and-conditions">Terms &amp; Conditions</a></li>
                    </ul>
                    <a href="https://indiabobbles.tawk.help" target="_blank" rel="noopener" class="ib-help-btn mt-2"><i class="fa fa-question-circle" aria-hidden="true"></i> Need help?</a>
                </div>
            </div>
        </div>

        <!-- Trust strip -->
        <div class="ib-perks">
            <div class="container py-3 d-flex flex-wrap justify-content-center gap-4">
                <span><i class="fa fa-truck" aria-hidden="true"></i>Free shipping across India</span>
                <span><i class="fa fa-paint-brush" aria-hidden="true"></i>Hand-painted by artists</span>
                <span><i class="fa fa-lock" aria-hidden="true"></i>Secure payments: UPI, cards, net banking</span>
                <span><i class="fa fa-clock-o" aria-hidden="true"></i>Mon–Sat, 10 AM–6 PM IST</span>
            </div>
        </div>

        <!-- Bottom bar -->
        <div class="container py-3 ib-bottom d-flex flex-column flex-md-row justify-content-between gap-2 text-center text-md-start">
            <span>&copy; @DateTime.Now.Year India Bobbles. All rights reserved.</span>
            <span>Noida, Uttar Pradesh, India · <a href="mailto:indiabobbles@rudrasofttech.com">indiabobbles@rudrasofttech.com</a></span>
        </div>
    </footer>

    <script type="text/javascript">
        //Google analytics code
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
                m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

        ga('create', 'UA-421743-7', 'indiabobbles.com');
        ga('send', 'pageview');

    </script>
    <script type="text/javascript">
        var vt_root = "//www.webstats.co.in/";
        var vt_website_id = "2";
        var vt_wvri = "";
        var VTInit = (function () {
            function VTInit() { }
            VTInit.prototype.initialize = function () {
                var seed = document.createElement("script");
                seed.setAttribute("src", vt_root + "rv/getjs/" + vt_website_id);
                if (document.getElementsByTagName("head").length > 0) {
                    document.getElementsByTagName("head")[0].appendChild(seed);
                }
            };
            return VTInit;
        }());
        var _vtInit = new VTInit();
        _vtInit.initialize();
    </script>
    @Scripts.Render("~/bundles/jquery")
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-ka7Sk0Gln4gmtz2MlQnikT1wXgYsOg+OMhuP+IlRH9sENBO0LRn5q+8nbTov4+1p" crossorigin="anonymous"></script>
    @RenderSection("scripts", required:=False)
    <!--Start of Tawk.to Script-->
    <script type="text/javascript">
        var Tawk_API = Tawk_API || {}, Tawk_LoadStart = new Date();
        (function () {
            var s1 = document.createElement("script"), s0 = document.getElementsByTagName("script")[0];
            s1.async = true;
            s1.src = 'https://embed.tawk.to/6ac5dd435dbfea34c7146470/1k4aegkak';
            s1.charset = 'UTF-8';
            s1.setAttribute('crossorigin', '*');
            s0.parentNode.insertBefore(s1, s0);
        })();
    </script>
    <!--End of Tawk.to Script-->
</body>
</html>
