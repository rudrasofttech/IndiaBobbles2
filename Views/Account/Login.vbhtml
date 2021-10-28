@ModelType IndiaBobbles.LoginDTO
@Code
    ViewData("Title") = "Login"

End Code

<div class="container fullbody bg-white">
    <div class="row">
        <div class="col-12 p-2">
            <h2>Login</h2>

            @Using (Html.BeginForm())
                @Html.AntiForgeryToken()

                @<div class="form-horizontal">
                    @Html.ValidationSummary(True, "", New With {.class = "text-danger"})
                    <div class="form-group">
                        @Html.LabelFor(Function(model) model.Email, htmlAttributes:=New With {.class = "control-label col-md-2"})
                        <div class="col-md-10">
                            @Html.EditorFor(Function(model) model.Email, New With {.htmlAttributes = New With {.class = "form-control"}})
                            @Html.ValidationMessageFor(Function(model) model.Email, "", New With {.class = "text-danger"})
                        </div>
                    </div>

                    <div class="form-group">
                        @Html.LabelFor(Function(model) model.Password, htmlAttributes:=New With {.class = "control-label col-md-2"})
                        <div class="col-md-10">
                            @Html.EditorFor(Function(model) model.Password, New With {.htmlAttributes = New With {.class = "form-control"}})
                            @Html.ValidationMessageFor(Function(model) model.Password, "", New With {.class = "text-danger"})
                        </div>
                    </div>

                    <div class="form-group">
                        <div class="col-md-offset-2 col-md-10">
                            <input type="submit" value="Login" class="btn btn-primary mt-2" />
                        </div>
                    </div>
                </div>
            End Using
        </div>
    </div>
</div>
