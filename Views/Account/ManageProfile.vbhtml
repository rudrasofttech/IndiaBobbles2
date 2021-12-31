@ModelType IndiaBobbles.Member
@Code
    ViewData("Title") = "Manage Profile"
    Dim countrylist As New List(Of SelectListItem)
    countrylist.Add(New SelectListItem() With {.Text = "Select", .Value = ""})
    countrylist.Add(New SelectListItem() With {.Text = "India", .Value = "IND"})

    Dim genderlist As New List(Of SelectListItem)
    genderlist.Add(New SelectListItem() With {.Text = "Select", .Value = ""})
    genderlist.Add(New SelectListItem() With {.Text = "Male", .Value = "M"})
    genderlist.Add(New SelectListItem() With {.Text = "Female", .Value = "F"})
    genderlist.Add(New SelectListItem() With {.Text = "Other", .Value = "O"})
End Code
<div class="container fullbody bg-white">
    <div class="row">
        <div class="col-6 p-3">
            <h2>Profile</h2>
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
                        @Html.EditorFor(Function(model) model.Email, New With {.htmlAttributes = New With {.class = "form-control", .readonly = "readonly"}})
                        @Html.ValidationMessageFor(Function(model) model.Email, "", New With {.class = "text-danger"})
                    </div>
                    <div class="mb-3">
                        @Html.LabelFor(Function(model) model.Newsletter, htmlAttributes:=New With {.class = "form-label"})
                        @Html.EditorFor(Function(model) model.Newsletter)
                        @Html.ValidationMessageFor(Function(model) model.Newsletter, "", New With {.class = "text-danger"})
                    </div>
                    <div class="mb-3">
                        @Html.LabelFor(Function(model) model.MemberName, htmlAttributes:=New With {.class = "form-label"})
                        @Html.EditorFor(Function(model) model.MemberName, New With {.htmlAttributes = New With {.class = "form-control"}})
                        @Html.ValidationMessageFor(Function(model) model.MemberName, "", New With {.class = "text-danger"})
                    </div>
                    <div class="mb-3">
                        @Html.LabelFor(Function(model) model.LastName, htmlAttributes:=New With {.class = "form-label"})
                        @Html.EditorFor(Function(model) model.LastName, New With {.htmlAttributes = New With {.class = "form-control"}})
                        @Html.ValidationMessageFor(Function(model) model.LastName, "", New With {.class = "text-danger"})
                    </div>
                    <div class="mb-3">
                        @Html.LabelFor(Function(model) model.DOB, htmlAttributes:=New With {.class = "form-label"})
                        @Html.EditorFor(Function(model) model.DOB, New With {.htmlAttributes = New With {.class = "form-control", .type = "date", .min = DateTime.Now.AddYears(-100).ToString("yyyy-MM-dd"), .max = DateTime.Now.AddYears(-13).ToString("yyyy-MM-dd")}})
                        @Html.ValidationMessageFor(Function(model) model.DOB, "", New With {.class = "text-danger"})
                    </div>
                    <div class="mb-3">
                        @Html.LabelFor(Function(model) model.Country, htmlAttributes:=New With {.class = "form-label"})
                        @Html.DropDownListFor(Function(model) model.Country, countrylist, htmlAttributes:=New With {.class = "form-select"})
                        @Html.ValidationMessageFor(Function(model) model.Country, "", New With {.class = "text-danger"})
                    </div>
                    <div class="mb-3">
                        @Html.LabelFor(Function(model) model.Mobile, htmlAttributes:=New With {.class = "form-label"})
                        @Html.EditorFor(Function(model) model.Mobile, New With {.htmlAttributes = New With {.class = "form-control"}})
                        @Html.ValidationMessageFor(Function(model) model.Mobile, "", New With {.class = "text-danger"})
                    </div>
                    <div Class="mb-3">
                        @Html.LabelFor(Function(model) model.Gender, htmlAttributes:=New With {.class = "form-label"})
                        @Html.DropDownListFor(Function(model) model.Gender, genderlist, htmlAttributes:=New With {.class = "form-select"})
                        @Html.ValidationMessageFor(Function(model) model.Gender, "", New With {.class = "text-danger"})
                    </div>
                    <div Class="p-3 bg-light">
                        <input type="submit" value="Save" Class="btn btn-primary" />
                    </div>
                </div>
            End Using
        </div>
    </div>
</div>