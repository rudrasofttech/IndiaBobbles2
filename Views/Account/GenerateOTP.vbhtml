@ModelType IndiaBobbles.OTPDTO
@Code
    ViewData("Title") = "Generate OTP"
End Code
<div class="container fullbody bg-white">
    <div class="row">
        <div class="col-md-6 p-3">
            <h2>Generate OTP</h2>
            @If ViewBag.Success IsNot Nothing Then
                @<div Class="alert alert-success" role="alert">
                    @ViewBag.Success
                    <br />
                    <a href="~/account/login">Login Here</a>
                </div>
            End If
            @If ViewBag.Error IsNot Nothing Then
                @<div Class="alert alert-danger" role="alert">
                    @ViewBag.Error
                    
                </div>
            End If
            @Using (Html.BeginForm())
                @Html.AntiForgeryToken()

                @Html.ValidationSummary(True, "", New With {.class = "text-danger"})
                @<div Class="mb-3">
                    @Html.LabelFor(Function(model) model.Email, htmlAttributes:=New With {.class = "form-label"})
                    @Html.EditorFor(Function(model) model.Email, New With {.htmlAttributes = New With {.class = "form-control", .required = "required"}})
                    @Html.ValidationMessageFor(Function(model) model.Email, "", New With {.class = "text-danger"})
                </div>
                @<div Class="p-3 bg-light">
                    <input type="submit" value="Get OTP" Class="btn btn-primary" />
                    <a href="~/account/login" class="btn btn-dark float-end">I have OTP</a>
                </div>

            End Using
        </div>
    </div>
</div>