@ModelType IndiaBobbles.OtpLoginDTO
@Code
    ViewData("Title") = "Login – India Bobbles"
    Dim isStep2 As Boolean = (Model IsNot Nothing AndAlso Model.StepNo = 2)

    ' Mask the email for display: rajkiran.singh@x.com -> ra•••••@x.com
    Dim maskedEmail As String = ""
    If isStep2 AndAlso Not String.IsNullOrEmpty(Model.Email) AndAlso Model.Email.Contains("@") Then
        Dim at As Integer = Model.Email.IndexOf("@")
        Dim namePart As String = Model.Email.Substring(0, at)
        maskedEmail = If(namePart.Length > 2, namePart.Substring(0, 2), namePart) & "•••••" & Model.Email.Substring(at)
    End If
End Code

@section head
    <style>
        
    </style>
End Section

@section scripts
    <script>
        (function () {
            // OTP box: digits only, auto-submit not forced (user taps Verify)
            var otp = document.getElementById('OTP');
            if (otp) {
                otp.focus();
                otp.addEventListener('input', function () { this.value = this.value.replace(/\D/g, ''); });
            }

            // Resend countdown (30 seconds)
            var resend = document.getElementById('ibResend');
            var label = document.getElementById('ibResendLabel');
            if (resend) {
                var secs = 30;
                resend.disabled = true;
                var timer = setInterval(function () {
                    secs--;
                    label.textContent = 'Resend code in ' + secs + 's';
                    if (secs <= 0) {
                        clearInterval(timer);
                        resend.disabled = false;
                        label.textContent = 'Resend code';
                    }
                }, 1000);
            }

            // Prevent double submits
            var form = document.getElementById('ibOtpForm');
            if (form) {
                form.addEventListener('submit', function (e) {
                    var btn = e.submitter;
                    if (btn && btn.id === 'ibMainBtn') {
                        setTimeout(function () { btn.disabled = true; btn.textContent = 'Please wait…'; }, 0);
                    }
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
            <h2>Welcome to the <em>India Bobbles</em> family 👋</h2>
            <p>Log in with just your email. No password to remember.</p>
            <ul>
                <li><i class="fa fa-truck" aria-hidden="true"></i> Track your orders anytime</li>
                <li><i class="fa fa-gift" aria-hidden="true"></i> Members-only offers &amp; early launches</li>
                <li><i class="fa fa-bolt" aria-hidden="true"></i> Faster checkout next time</li>
                <li><i class="fa fa-paint-brush" aria-hidden="true"></i> Hand-painted bobbleheads, free shipping</li>
            </ul>
        </div>

        <!-- ============ Form panel ============ -->
        <div class="form-side">
            <div class="steps"><span class="on"></span><span class="@(If(isStep2, "on", ""))"></span></div>

            @If isStep2 Then
                @<h1>Enter your code</h1>
                @<p class="sub">We emailed a one-time code to <b>@maskedEmail</b>. It may take a minute, so check your spam folder too.</p>
            Else
                @<h1>Log in or sign up</h1>
                @<p class="sub">Enter your email and we'll send you a one-time login code.</p>
            End If

            @If ViewBag.Success IsNot Nothing Then
                @<div class="msg msg-ok" role="status"><i class="fa fa-check-circle mt-1" aria-hidden="true"></i><span>@ViewBag.Success</span></div>
            End If
            @If ViewBag.Error IsNot Nothing Then
                @<div class="msg msg-err" role="alert"><i class="fa fa-exclamation-circle mt-1" aria-hidden="true"></i><span>@ViewBag.Error</span></div>
            End If

            @Using (Html.BeginForm("OtpLogin", "Account", FormMethod.Post, New With {.id = "ibOtpForm"}))
                @Html.AntiForgeryToken()
                @Html.HiddenFor(Function(model) model.StepNo)
                @Html.ValidationSummary(True, "", New With {.class = "text-danger small"})

                If isStep2 Then
                    ' Email kept (read-only) so Verify and Resend still post it
                    @Html.HiddenFor(Function(model) model.Email)

                    @<div class="mb-3">
                        <label for="OTP" class="form-label">One-time code</label>
                        @Html.TextBoxFor(Function(model) model.OTP, New With {.class = "form-control otp-input", .required = "required", .inputmode = "numeric", .autocomplete = "one-time-code", .maxlength = "10", .placeholder = "••••••"})
                        @Html.ValidationMessageFor(Function(model) model.OTP, "", New With {.class = "text-danger small"})
                    </div>

                    @<button type="submit" name="command" value="verify" id="ibMainBtn" class="btn btn-main mb-3">Verify &amp; Log in</button>

                    @<div class="d-flex justify-content-between align-items-center flex-wrap gap-2 small">
                        <span class="text-muted">
                            Didn't get it?
                            <button type="submit" name="command" value="send" id="ibResend" class="btn-link-ib" formnovalidate="formnovalidate"><span id="ibResendLabel">Resend code in 30s</span></button>
                        </span>
                        <a href="@Url.Action("OtpLogin", "Account")" class="btn-link-ib">Change email</a>
                    </div>
                Else
                    @<div class="mb-3">
                        @Html.LabelFor(Function(model) model.Email, "Email address", htmlAttributes:=New With {.class = "form-label"})
                        @Html.TextBoxFor(Function(model) model.Email, New With {.class = "form-control", .type = "email", .required = "required", .autocomplete = "email", .placeholder = "you@example.com", .autofocus = "autofocus"})
                        @Html.ValidationMessageFor(Function(model) model.Email, "", New With {.class = "text-danger small"})
                    </div>

                    @<button type="submit" name="command" value="send" id="ibMainBtn" class="btn btn-main">Send login code <i class="fa fa-arrow-right" aria-hidden="true"></i></button>
                End If
            End Using

            <div class="divider"></div>
            <p class="small-note mb-1">New to India Bobbles? <a href="~/account/register">Create an account</a></p>
            <p class="small-note mb-0">Trouble logging in? <a href="https://indiabobbles.tawk.help" target="_blank" rel="noopener">Get help</a></p>
        </div>
    </div>
</div>