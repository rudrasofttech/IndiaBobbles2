@ModelType IndiaBobbles.RegisterDTO

@Code
    ViewData("Title") = "Register"
End Code

<div class="container fullbody bg-white">
    <div class="row">
        <div class="col-md-6 p-3">
            <h2>Register</h2>
            @If ViewBag.Error IsNot Nothing Then
                @<div Class="alert alert-danger" role="alert">
                    @ViewBag.Error
                </div>
            End If
            @Using (Html.BeginForm())
                @Html.AntiForgeryToken()
                @<div>
                    @Html.ValidationSummary(True, "", New With {.class = "text-danger"})
                    <div class="mb-3">
                        @Html.LabelFor(Function(model) model.Email, htmlAttributes:=New With {.class = "form-label"})
                        @Html.EditorFor(Function(model) model.Email, New With {.htmlAttributes = New With {.class = "form-control", .required = "required", .maxlength = "250"}})
                        @Html.ValidationMessageFor(Function(model) model.Email, "", New With {.class = "text-danger"})
                    </div>
                    <div class="mb-3">
                        @Html.LabelFor(Function(model) model.Name, htmlAttributes:=New With {.class = "form-label"})
                        @Html.EditorFor(Function(model) model.Name, New With {.htmlAttributes = New With {.class = "form-control", .required = "required", .maxlength = "50"}})
                        @Html.ValidationMessageFor(Function(model) model.Name, "", New With {.class = "text-danger"})
                    </div>
                    <div class="mb-3">
                        @Html.LabelFor(Function(model) model.Mobile, htmlAttributes:=New With {.class = "form-label"})
                        @Html.EditorFor(Function(model) model.Mobile, New With {.htmlAttributes = New With {.class = "form-control", .required = "required", .maxlength = "10"}})
                        @Html.ValidationMessageFor(Function(model) model.Mobile, "", New With {.class = "text-danger"})
                    </div>
                    <div class="mb-3 form-check">
                        @Html.LabelFor(Function(model) model.Newsletter, htmlAttributes:=New With {.class = "form-check-label"})
                        @Html.EditorFor(Function(model) model.Newsletter, New With {.htmlAttributes = New With {.class = "form-check-input"}})
                    </div>
                    <div class="mb-3">
                        <input type="submit" value="Submit" class="btn btn-secondary" />
                    </div>
                </div>
            End Using
        </div>
    </div>
</div>