@ModelType IndiaBobbles.RegisterDTO

@Code
    ViewData("Title") = "Create your account – India Bobbles"
End Code

@section head
    <style>
        
    </style>
End Section

@section scripts
    <script>
        (function () {
            // Mobile: digits only
            var mob = document.getElementById('Mobile');
            if (mob) mob.addEventListener('input', function () { this.value = this.value.replace(/\D/g, '').slice(0, 10); });

            // Prevent double submit
            var form = document.getElementById('ibRegForm');
            var btn = document.getElementById('ibRegBtn');
            if (form && btn) {
                form.addEventListener('submit', function () {
                    if (form.checkValidity()) { setTimeout(function () { btn.disabled = true; btn.textContent = 'Creating your account…'; }, 0); }
                });
            }
        })();
    </script>
End Section

<div class="ib-auth container fullbody">
    <div class="wrap">

        <!-- ============ Brand panel (desktop) ============ -->
        <div class="brand">
            <img src="~/theme/khichdi/img/ib-logo.png" alt="India Bobbles" />
            <h2>Join the <em>India Bobbles</em> family 🎉</h2>
            <p>Create a free account in under a minute. No password needed, you'll log in with a code sent to your email.</p>
            <ul>
                <li><i class="fa fa-truck" aria-hidden="true"></i> Track all your orders in one place</li>
                <li><i class="fa fa-gift" aria-hidden="true"></i> Members-only offers &amp; early launches</li>
                <li><i class="fa fa-bolt" aria-hidden="true"></i> Faster checkout next time</li>
                <li><i class="fa fa-bell" aria-hidden="true"></i> Back-in-stock alerts for your favourites</li>
            </ul>
        </div>

        <!-- ============ Form panel ============ -->
        <div class="form-side">
            <h1>Create your account</h1>
            <p class="sub">It's free, and it only takes a minute.</p>

            @If ViewBag.Error IsNot Nothing Then
                @<div class="msg-err" role="alert"><i class="fa fa-exclamation-circle mt-1" aria-hidden="true"></i><span>@ViewBag.Error</span></div>
            End If

            @Using (Html.BeginForm(Nothing, Nothing, FormMethod.Post, New With {.id = "ibRegForm"}))
                @Html.AntiForgeryToken()
                @Html.ValidationSummary(True, "", New With {.class = "text-danger small"})

                @<div class="mb-3">
                    @Html.LabelFor(Function(model) model.Name, "Full name", htmlAttributes:=New With {.class = "form-label"})
                    @Html.TextBoxFor(Function(model) model.Name, New With {.class = "form-control", .required = "required", .maxlength = "50", .autocomplete = "name", .placeholder = "e.g. Priya Sharma", .autofocus = "autofocus"})
                    @Html.ValidationMessageFor(Function(model) model.Name, "", New With {.class = "text-danger small"})
                </div>

                @<div class="mb-3">
                    @Html.LabelFor(Function(model) model.Email, "Email address", htmlAttributes:=New With {.class = "form-label"})
                    @Html.TextBoxFor(Function(model) model.Email, New With {.class = "form-control", .type = "email", .required = "required", .maxlength = "250", .autocomplete = "email", .placeholder = "you@example.com"})
                    <div class="hint">We'll send your login codes and order updates here.</div>
                    @Html.ValidationMessageFor(Function(model) model.Email, "", New With {.class = "text-danger small"})
                </div>

                @<div class="mb-3">
                    @Html.LabelFor(Function(model) model.Mobile, "Mobile number", htmlAttributes:=New With {.class = "form-label"})
                    <div class="input-group">
                        <span class="input-group-text">+91</span>
                        @Html.TextBoxFor(Function(model) model.Mobile, New With {.class = "form-control", .type = "tel", .required = "required", .maxlength = "10", .inputmode = "numeric", .pattern = "[0-9]{10}", .title = "Please enter your 10-digit mobile number", .autocomplete = "tel-national", .placeholder = "10-digit mobile number"})
                    </div>
                    <div class="hint">For delivery updates. We never share your number.</div>
                    @Html.ValidationMessageFor(Function(model) model.Mobile, "", New With {.class = "text-danger small"})
                </div>

                @<div class="news-box mb-3">
                    @Html.EditorFor(Function(model) model.Newsletter)
                    <label for="Newsletter">
                        <b>Yes, send me offers &amp; new launches</b>
                        Festival deals, new bobbleheads and members-only discounts. Unsubscribe anytime.
                    </label>
                </div>

                @<div class="mb-3">
                    <label class="form-label" for="CaptchaValue">Type the characters you see</label>
                    <div class="captcha">
                        <img src="@Model.CaptchaImage" alt="Captcha image" />
                        <div>
                            <input type="hidden" name="CaptchaKey" value="@Model.CaptchaKey" />
                            <input type="text" class="form-control" required="required" id="CaptchaValue" name="CaptchaValue" maxlength="20" autocomplete="off" autocapitalize="off" spellcheck="false" placeholder="Enter captcha" />
                        </div>
                    </div>
                    <div class="hint">Can't read it? <button type="button" class="link-btn" onclick="location.reload()">Get a new one</button></div>
                </div>

                @<button type="submit" id="ibRegBtn" class="btn btn-main mb-2">Create my account <i class="fa fa-arrow-right" aria-hidden="true"></i></button>
                @<p class="terms mb-0">By creating an account, you agree to our <a href="~/terms-and-conditions">Terms &amp; Conditions</a> and <a href="~/privacy-policy">Privacy Policy</a>.</p>
            End Using

            <div class="divider"></div>
            <p class="small-note mb-0">Already have an account? <a href="~/account/otplogin">Log in</a></p>
        </div>
    </div>
</div>