@ModelType IndiaBobbles.OrderAddressDTO
@Code
    ViewData("Title") = "Address"

    Dim statesarr() As String = {"", "Andaman and Nicobar Islands", "Andhra Pradesh", "Arunachal Pradesh",
                              "Assam", "Bihar", "Chandigarh", "Chhattisgarh", "Dadra and Nagar Haveli",
                              "Daman and Diu", "Delhi", "Goa", "Gujarat", "Haryana", "Himachal Pradesh",
                              "Jammu and Kashmir", "Jharkhand", "Karnataka", "Kerala", "Lakshadweep", "Madhya Pradesh",
                                      "Maharashtra", "Manipur", "Meghalaya", "Mizoram", "Nagaland", "Odisha",
                                              "Puducherry", "Punjab", "Rajasthan", "Sikkim", "Tamil Nadu", "Tripura",
                                              "Uttar Pradesh", "Uttarakhand", "West Bengal", "Telangana"}
    Dim statelist As New SelectList(statesarr)

End Code

<div class="container bg-white pt-2 fullbody">
    <div class="row">
        <div class="col-12 ">
            <h1 class="text-center">Address</h1>
            @Using (Html.BeginForm())
                @Html.AntiForgeryToken()
                @Html.ValidationSummary(True, "", New With {.class = "text-danger"})
                @<div class="form-horizontal">
                    <div class="row">
                        <div class="col-sm-6">
                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.Email, htmlAttributes:=New With {.class = "control-label col-md-3"})
                                <div class="col-md-9">
                                    @Html.EditorFor(Function(model) model.Email, New With {.htmlAttributes = New With {.class = "form-control"}})
                                    @Html.ValidationMessageFor(Function(model) model.Email, "", New With {.class = "text-danger"})
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-6">
                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.Name, htmlAttributes:=New With {.class = "control-label col-md-3"})
                                <div class="col-md-9">
                                    @Html.EditorFor(Function(model) model.Name, New With {.htmlAttributes = New With {.class = "form-control", .onchange = "copyData(this, 'ShippingName')"}})
                                    @Html.ValidationMessageFor(Function(model) model.Name, "", New With {.class = "text-danger"})
                                </div>
                            </div>
                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.Phone, htmlAttributes:=New With {.class = "control-label col-md-3"})
                                <div class="col-md-9">
                                    @Html.EditorFor(Function(model) model.Phone, New With {.htmlAttributes = New With {.class = "form-control", .onchange = "copyData(this, 'ShippingPhone')"}})
                                    @Html.ValidationMessageFor(Function(model) model.Phone, "", New With {.class = "text-danger"})
                                </div>
                            </div>
                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.BillingAddress, htmlAttributes:=New With {.class = "control-label col-md-3"})
                                <div class="col-md-9">
                                    @Html.EditorFor(Function(model) model.BillingAddress, New With {.htmlAttributes = New With {.class = "form-control", .onchange = "copyData(this, 'ShippingAddress')"}})
                                    @Html.ValidationMessageFor(Function(model) model.BillingAddress, "", New With {.class = "text-danger"})
                                </div>
                            </div>
                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.BillingCity, htmlAttributes:=New With {.class = "control-label col-md-3"})
                                <div class="col-md-9">
                                    @Html.EditorFor(Function(model) model.BillingCity, New With {.htmlAttributes = New With {.class = "form-control", .onchange = "copyData(this, 'ShippingCity')"}})
                                    @Html.ValidationMessageFor(Function(model) model.BillingCity, "", New With {.class = "text-danger"})
                                </div>
                            </div>
                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.BillingState, htmlAttributes:=New With {.class = "control-label col-md-3"})
                                <div class="col-md-9">
                                    @Html.DropDownListFor(Function(model) model.BillingState, statelist, New With {.class = "form-select", .onchange = "copyData(this, 'ShippingState')"})
                                    @Html.ValidationMessageFor(Function(model) model.BillingState, "", New With {.class = "text-danger"})
                                </div>
                            </div>
                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.BillingCountry, htmlAttributes:=New With {.class = "control-label col-md-3"})
                                <div class="col-md-9">
                                    @Html.EditorFor(Function(model) model.BillingCountry, New With {.value = "India", .htmlAttributes = New With {.class = "form-control", .onchange = "copyData(this, 'ShippingCountry')", .readonly = ""}})
                                    @Html.ValidationMessageFor(Function(model) model.BillingCountry, "", New With {.class = "text-danger"})
                                </div>
                            </div>
                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.BillingPinCode, htmlAttributes:=New With {.class = "control-label col-md-3"})
                                <div class="col-md-9">
                                    @Html.EditorFor(Function(model) model.BillingPinCode, New With {.htmlAttributes = New With {.class = "form-control", .onchange = "copyData(this, 'ShippingPincode')"}})
                                    @Html.ValidationMessageFor(Function(model) model.BillingPinCode, "", New With {.class = "text-danger"})
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.ShippingName, htmlAttributes:=New With {.class = "control-label col-md-4"})
                                <div class="col-md-9">
                                    @Html.EditorFor(Function(model) model.ShippingName, New With {.htmlAttributes = New With {.class = "form-control"}})
                                    @Html.ValidationMessageFor(Function(model) model.ShippingName, "", New With {.class = "text-danger"})
                                </div>
                            </div>

                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.ShippingPhone, htmlAttributes:=New With {.class = "control-label col-md-4"})
                                <div class="col-md-9">
                                    @Html.EditorFor(Function(model) model.ShippingPhone, New With {.htmlAttributes = New With {.class = "form-control"}})
                                    @Html.ValidationMessageFor(Function(model) model.ShippingPhone, "", New With {.class = "text-danger"})
                                </div>
                            </div>

                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.ShippingAddress, htmlAttributes:=New With {.class = "control-label col-md-4"})
                                <div class="col-md-9">
                                    @Html.EditorFor(Function(model) model.ShippingAddress, New With {.htmlAttributes = New With {.class = "form-control"}})
                                    @Html.ValidationMessageFor(Function(model) model.ShippingAddress, "", New With {.class = "text-danger"})
                                </div>
                            </div>

                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.ShippingCity, htmlAttributes:=New With {.class = "control-label col-md-3"})
                                <div class="col-md-9">
                                    @Html.EditorFor(Function(model) model.ShippingCity, New With {.htmlAttributes = New With {.class = "form-control"}})
                                    @Html.ValidationMessageFor(Function(model) model.ShippingCity, "", New With {.class = "text-danger"})
                                </div>
                            </div>

                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.ShippingState, htmlAttributes:=New With {.class = "control-label col-md-3"})
                                <div class="col-md-9">
                                    @Html.DropDownListFor(Function(model) model.ShippingState, statelist, New With {.class = "form-select"})
                                    @Html.ValidationMessageFor(Function(model) model.ShippingState, "", New With {.class = "text-danger"})
                                </div>
                            </div>

                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.ShippingCountry, htmlAttributes:=New With {.class = "control-label col-md-3"})
                                <div class="col-md-9">
                                    @Html.EditorFor(Function(model) model.ShippingCountry, New With {.htmlAttributes = New With {.class = "form-control", .readonly = ""}})
                                    @Html.ValidationMessageFor(Function(model) model.ShippingCountry, "", New With {.class = "text-danger"})
                                </div>
                            </div>

                            <div class="form-group">
                                @Html.LabelFor(Function(model) model.ShippingPincode, htmlAttributes:=New With {.class = "control-label col-md-3"})
                                <div class="col-md-9">
                                    @Html.EditorFor(Function(model) model.ShippingPincode, New With {.htmlAttributes = New With {.class = "form-control"}})
                                    @Html.ValidationMessageFor(Function(model) model.ShippingPincode, "", New With {.class = "text-danger"})
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="form-group mb-5 mt-5">
                        <div class="col-md-12 text-end">
                            <input type="submit" value="Proceed to Checkout" class="btn btn-primary" />
                        </div>
                    </div>
                </div>
            End Using
        </div>
    </div>
</div>

@section scripts
    <script>
        function copyData(src, trg) {
            $("#" + trg).val($(src).val())
        }
    </script>
End Section