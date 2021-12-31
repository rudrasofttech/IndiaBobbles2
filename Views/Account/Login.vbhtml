@ModelType IndiaBobbles.LoginDTO
@Code
    ViewData("Title") = "Login"

End Code

<div class="container fullbody bg-white">
    <div class="row">
        <div class="col-6 p-3">
            <h2>Login</h2>
            @If ViewBag.Error IsNot Nothing Then
                @<div Class="alert alert-danger" role="alert">
                    @ViewBag.Error
                </div>
            End If
            @Using (Html.BeginForm("Login", "Account"))
                @Html.AntiForgeryToken()
                @Html.ValidationSummary(True, "", New With {.class = "text-danger"})
                @<div class="mb-3">
                    @Html.LabelFor(Function(model) model.Email, htmlAttributes:=New With {.class = "form-label"})
                    @Html.EditorFor(Function(model) model.Email, New With {.htmlAttributes = New With {.class = "form-control", .required = "required"}})
                    @Html.ValidationMessageFor(Function(model) model.Email, "", New With {.class = "text-danger"})
                </div>
                @<div class="mb-3">
                    @Html.LabelFor(Function(model) model.Password, htmlAttributes:=New With {.class = "form-label"})
                    @Html.PasswordFor(Function(model) model.Password, htmlAttributes:=New With {.class = "form-control", .required = "required"})
                    @Html.ValidationMessageFor(Function(model) model.Password, "", New With {.class = "text-danger"})
                </div>
                @<div class="mb-3">
                    <input type="submit" value="Login" class="btn btn-success mt-2" />
                    <a href="~/account/generateotp" class="btn btn-dark float-end">Get OTP</a>
                </div>
            End Using
        </div>
    </div>
</div>
