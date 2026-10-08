@ModelType IndiaBobbles.Member
@Code
    ViewData("Title") = "My Profile – India Bobbles"
    Dim countrylist As New List(Of SelectListItem)
    countrylist.Add(New SelectListItem() With {.Text = "Select", .Value = ""})
    countrylist.Add(New SelectListItem() With {.Text = "India", .Value = "IN"})

    Dim genderlist As New List(Of SelectListItem)
    genderlist.Add(New SelectListItem() With {.Text = "Select", .Value = ""})
    genderlist.Add(New SelectListItem() With {.Text = "Male", .Value = "M"})
    genderlist.Add(New SelectListItem() With {.Text = "Female", .Value = "F"})
    genderlist.Add(New SelectListItem() With {.Text = "Other", .Value = "O"})

    Dim displayName As String = Trim(If(Model.MemberName, "") & " " & If(Model.LastName, ""))
    If displayName = "" Then displayName = "India Bobbles member"
    Dim initial As String = If(String.IsNullOrEmpty(Model.MemberName), "IB", Model.MemberName.Substring(0, 1).ToUpper())
End Code


@section scripts
    <script>
        (function () {
            var mob = document.getElementById('Mobile');
            if (mob) mob.addEventListener('input', function () { this.value = this.value.replace(/\D/g, '').slice(0, 10); });

            // Warn before leaving with unsaved changes
            var form = document.getElementById('ibProfileForm');
            var dirty = false;
            if (form) {
                form.addEventListener('input', function () { dirty = true; });
                form.addEventListener('change', function () { dirty = true; });
                form.addEventListener('submit', function () {
                    dirty = false;
                    var b = document.getElementById('ibSaveBtn');
                    setTimeout(function () { b.disabled = true; b.textContent = 'Saving…'; }, 0);
                });
                window.addEventListener('beforeunload', function (e) { if (dirty) { e.preventDefault(); e.returnValue = ''; } });
            }
        })();
    </script>
End Section

<div class="ib-acc container fullbody">
    <div class="row g-4">

        <!-- ============ Sidebar ============ -->
        <div class="col-lg-3">
            <div class="side">
                <div class="top">
                    <span class="avatar">@initial</span>
                    <h5>@displayName</h5>
                    <small>@Model.Email</small>
                </div>
                <nav>
                    <a href="~/account/manageprofile" class="active"><i class="fa fa-user" aria-hidden="true"></i> My profile</a>
                    <a href="~/orders"><i class="fa fa-shopping-bag" aria-hidden="true"></i> My orders</a>
                    <a href="~/tag/collectibles"><i class="fa fa-star" aria-hidden="true"></i> Shop bobbleheads</a>
                    <a href="https://indiabobbles.tawk.help" target="_blank" rel="noopener"><i class="fa fa-question-circle" aria-hidden="true"></i> Help centre</a>
                    <a href="~/account/logout" class="logout"><i class="fa fa-sign-out" aria-hidden="true"></i> Log out</a>
                </nav>
            </div>
        </div>

        <!-- ============ Form ============ -->
        <div class="col-lg-9">
            <h1>My profile</h1>
            <p class="sub">Keep your details up to date for smooth deliveries and offers made for you.</p>

            @If ViewBag.Success IsNot Nothing Then
                @<div class="msg msg-ok" role="status"><i class="fa fa-check-circle mt-1" aria-hidden="true"></i><span>@ViewBag.Success</span></div>
            End If
            @If ViewBag.Error IsNot Nothing Then
                @<div class="msg msg-err" role="alert"><i class="fa fa-exclamation-circle mt-1" aria-hidden="true"></i><span>@ViewBag.Error</span></div>
            End If

            @Using (Html.BeginForm(Nothing, Nothing, FormMethod.Post, New With {.id = "ibProfileForm"}))
                @Html.AntiForgeryToken()
                @Html.ValidationSummary(True, "", New With {.class = "text-danger small"})

                @* Personal details *@
                @<div class="card-ib">
                    <h2><i class="fa fa-user" aria-hidden="true"></i> Personal details</h2>
                    <p class="desc">How we greet you, and a little birthday surprise if you tell us your date of birth. 🎂</p>
                    <div class="row g-3">
                        <div class="col-md-6">
                            @Html.LabelFor(Function(model) model.MemberName, "First name", htmlAttributes:=New With {.class = "form-label"})
                            @Html.EditorFor(Function(model) model.MemberName, New With {.htmlAttributes = New With {.class = "form-control", .autocomplete = "given-name"}})
                            @Html.ValidationMessageFor(Function(model) model.MemberName, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-6">
                            @Html.LabelFor(Function(model) model.LastName, "Last name", htmlAttributes:=New With {.class = "form-label"})
                            @Html.EditorFor(Function(model) model.LastName, New With {.htmlAttributes = New With {.class = "form-control", .autocomplete = "family-name"}})
                            @Html.ValidationMessageFor(Function(model) model.LastName, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-6">
                            @Html.LabelFor(Function(model) model.DOB, "Date of birth", htmlAttributes:=New With {.class = "form-label"})
                            @Html.TextBoxFor(Function(model) model.DOB, "{0:yyyy-MM-dd}", New With {.class = "form-control", .type = "date", .min = DateTime.Now.AddYears(-100).ToString("yyyy-MM-dd"), .max = DateTime.Now.AddYears(-13).ToString("yyyy-MM-dd")})
                            @Html.ValidationMessageFor(Function(model) model.DOB, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-6">
                            @Html.LabelFor(Function(model) model.Gender, "Gender", htmlAttributes:=New With {.class = "form-label"})
                            @Html.DropDownListFor(Function(model) model.Gender, genderlist, htmlAttributes:=New With {.class = "form-select"})
                            @Html.ValidationMessageFor(Function(model) model.Gender, "", New With {.class = "text-danger small"})
                        </div>
                    </div>
                </div>

                @* Contact details *@
                @<div class="card-ib">
                    <h2><i class="fa fa-phone" aria-hidden="true"></i> Contact details</h2>
                    <p class="desc">Used for login codes, order updates and delivery calls.</p>
                    <div class="row g-3">
                        <div class="col-md-6">
                            @Html.LabelFor(Function(model) model.Email, "Email address", htmlAttributes:=New With {.class = "form-label"})
                            <div class="input-group">
                                @Html.EditorFor(Function(model) model.Email, New With {.htmlAttributes = New With {.class = "form-control", .readonly = "readonly"}})
                                <span class="input-group-text" title="Email can't be changed"><i class="fa fa-lock" aria-hidden="true"></i></span>
                            </div>
                            <div class="hint">Your email is your login, so it can't be changed here.</div>
                            @Html.ValidationMessageFor(Function(model) model.Email, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-6">
                            @Html.LabelFor(Function(model) model.Mobile, "Mobile number", htmlAttributes:=New With {.class = "form-label"})
                            <div class="input-group">
                                <span class="input-group-text">+91</span>
                                @Html.EditorFor(Function(model) model.Mobile, New With {.htmlAttributes = New With {.class = "form-control", .type = "tel", .inputmode = "numeric", .maxlength = "10", .autocomplete = "tel-national"}})
                            </div>
                            @Html.ValidationMessageFor(Function(model) model.Mobile, "", New With {.class = "text-danger small"})
                        </div>
                        <div class="col-md-6">
                            @Html.LabelFor(Function(model) model.Country, "Country", htmlAttributes:=New With {.class = "form-label"})
                            @Html.DropDownListFor(Function(model) model.Country, countrylist, htmlAttributes:=New With {.class = "form-select"})
                            @Html.ValidationMessageFor(Function(model) model.Country, "", New With {.class = "text-danger small"})
                        </div>
                    </div>
                </div>

                @* Communication *@
                @<div class="card-ib">
                    <h2><i class="fa fa-envelope" aria-hidden="true"></i> Email preferences</h2>
                    <p class="desc">You're in control. Change this anytime.</p>
                    <div class="news form-switch">
                        @Html.EditorFor(Function(model) model.Newsletter, New With {.htmlAttributes = New With {.class = "form-check-input", .role = "switch"}})
                        <label for="Newsletter">
                            <b>Offers &amp; new launches</b>
                            Festival deals, new bobbleheads and members-only discounts. Order updates are always sent.
                        </label>
                    </div>
                    @Html.ValidationMessageFor(Function(model) model.Newsletter, "", New With {.class = "text-danger small"})
                </div>

                @<div class="savebar">
                    <span class="hint"><i class="fa fa-shield" aria-hidden="true"></i> We never share your details. <a href="~/privacy-policy" style="color:#330B3F;font-weight:700;">Privacy Policy</a></span>
                    <button type="submit" id="ibSaveBtn" class="btn btn-main">Save changes</button>
                </div>
            End Using
        </div>
    </div>
</div>