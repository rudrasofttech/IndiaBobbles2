<!DOCTYPE html>
<html>
<head>
    @RenderSection("meta", required:=False)
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="google-site-verification" content="rtXgIUyaLbBtd_tsln3F6ZB9ZboSPHZe7K_zB6uRgv8" />
    <link rel="shortcut icon" href="~/theme/khichdi/favicon.jpg" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3" crossorigin="anonymous" />
    <link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.6.3/css/font-awesome.min.css" rel="stylesheet" />
    @Scripts.Render("~/bundles/modernizr")
    <style>
        /*body {
            background-image: url( @@Url.Content("~/theme/khichdi/img/India-Bobbles-package-1300.jpg") );
        }*/

        .fullbody {
            min-height: calc(100vh - 150px);
        }
    </style>
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
    <div class="container-fluid bg-warning">
        <header class="d-flex flex-wrap align-items-center justify-content-center justify-content-md-between py-3">
            <a href="~/" class="d-flex align-items-center col-md-3 mb-2 mb-md-0 text-dark text-decoration-none">
                <img alt="India Bobbles Logo" src="~/theme/khichdi/img/ib-logo.png" />
            </a>
            <ul class="nav col-12 col-md-auto mb-2 justify-content-center mb-md-0">
                <li><a href="~/tag/collectibles" class="nav-link px-2 link-dark">Bobbles</a></li>
                <li><a href="~/blog" class="nav-link px-2 link-dark">Blog</a></li>
                <li><a href="~/orders" class="nav-link px-2 link-dark">My Orders</a></li>
                @If o.OrderItems.Count > 0 Then
                    @<li><a href="@Url.Content("~/cart")" Class="nav-link px-2 link-dark">Cart (@o.OrderItems.Count)</a></li>
                Else
                    @<li><a href="@Url.Content("~/cart")" Class="nav-link px-2 link-dark">Cart</a></li>
                End If
            </ul>
            <div Class="col-md-3 text-end">
                @If Request.IsAuthenticated Then
                    @<a href="~/account/manageprofile" class="btn btn-success me-2">Profile</a>
                    @<a href="~/account/logout" class="btn btn-secondary me-2">Logout</a>
                Else
                    @<a href="~/account/generateotp" class="btn btn-secondary me-2">Login</a>
                    @<a href="~/account/register" class="btn btn-secondary me-2">Signup</a>
                End If

            </div>
        </header>
    </div>
    @If o.OrderItems.Count > 0 AndAlso (String.IsNullOrEmpty(o.Name) OrElse String.IsNullOrEmpty(o.Email) OrElse String.IsNullOrEmpty(o.Phone)) Then
        @<div class="alert alert-primary rounded-0 text-center" role="alert">
            Your order is missing Name, Email and Phone . <a href="#" data-bs-toggle="modal" data-bs-target="#orderContactModal">Add Now</a>
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
    <footer Class="footer mt-auto py-3 bg-light">
        <div Class="container">
            <div>
                <a href="~/about" Class="px-2 link-dark">Our Story</a>
                @If User.Identity.IsAuthenticated Then
                    @<a href="~/account/myaccount" class="px-2 link-dark">My Account</a>
                    @<a href="~/account/logout" class="px-2 link-dark">Logout</a>
                Else
                    @<a href="~/account/register" class="px-2 link-dark">Register</a>
                    @<a href="~/account/login" class="px-2 link-dark">Login</a>
                End If
                <a href="~/contact" class="px-2 link-dark">Contact</a>
                <a href="~/privacy-policy" class="px-2 link-dark">Privacy</a>
                <a href="~/terms-and-conditions" class="px-2 link-dark">Terms &amp; Conditions</a>
                <a href="~/shipping-policy" class="px-2 link-dark">Shipping &amp; Refund</a>
                <a href="~/payment-options" class="px-2 link-dark">Payment Options</a>
                <a href="~/order-custom-bobbleheads" class="px-2 link-dark">Custom Bobbleheads</a>
                <a href="~/collectibles" class="px-2 link-dark">Collectibles</a>
                <a href="~/games" class="px-2 link-dark">Games</a>
                <a href="https://www.facebook.com/IndiaBobbles" class="px-2 link-dark" title="IndiaBobbles Facebook" target="_blank">
                    <i class="fa fa-facebook" aria-hidden="true"></i>
                </a>
                <a href="https://www.instagram.com/indiabobbles" class="px-2 link-dark" title="IndiaBobbles Instagram" target="_blank">
                    <i class="fa fa-instagram" aria-hidden="true"></i>
                </a>
            </div>
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
        var vt_root = "//rudrasofttech.com/vtracker/";
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
</body>
</html>
