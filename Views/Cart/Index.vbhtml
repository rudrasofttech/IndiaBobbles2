@ModelType IndiaBobbles.Order
@Code
    ViewData("Title") = "Cart"
End Code

<div class="container bg-white pt-2 fullbody">
    <div class="row">
        <div class="col-12 ">
            <h1 class="text-center">Cart</h1>
            <div class="pt-2 pb-2 mb-2">
                <div class="table-responsive">
                    <table class="table">
                        <thead>
                            <tr>
                                <th scope="col"></th>
                                <th scope="col">Product Name</th>
                                <th scope="col">Product Code</th>
                                <th scope="col" class="text-start">Quantity</th>
                                <th scope="col">Price</th>
                                <th scope="col">Amount</th>
                                <th scope="col"></th>
                            </tr>
                        </thead>
                        <tbody>
                            @If Model IsNot Nothing Then
                                @For Each o In Model.OrderItems
                                    @<tr>
                                        <th scope="row"><img src="@o.ProductImg" style="height:80px" /></th>
                                        <td>@o.ProductName</td>
                                        <td>
                                            @o.ProductCode
                                        </td>
                                        <td class="text-left">
                                            <ul class="pagination">
                                                <li class="page-item"><a class="page-link" href="~/cart/subtractquantity/@o.ID">-</a></li>
                                                <li class="page-item disabled"><a class="page-link">@o.Quantity</a></li>
                                                <li class="page-item"><a class="page-link" href="~/cart/addquantity/@o.ID">+</a></li>
                                            </ul>
                                        </td>
                                        <td>@o.Price.ToString("##00.00")</td>
                                        <td>@o.Amount.ToString("##00.00")</td>
                                        <td>
                                            <form method="post" action="@Url.Content("~/cart/remove/" & o.ID)">
                                                @Html.AntiForgeryToken()
                                                <input type="submit" value="Remove" Class="btn btn-dark btn-sm" />
                                            </form>
                                        </td>
                                    </tr>
                                Next

                                If Model.OrderItems.Count = 0 Then
                                    @<tr>
                                        <td colspan="7">No products in your cart.</td>
                                    </tr>
                                Else
                                    @<tr>
                                        <td colspan="4" style="text-align:left">
                                            <form method="post" action="@Url.Content("~/cart/applycoupon")">
                                                @Html.AntiForgeryToken()
                                                <input type="text" name="coupon" maxlength="100" placeholder="Coupon Code" />
                                                <input type="submit" value="Apply" Class="btn btn-light btn-sm" />
                                            </form>
                                        </td>
                                        <td style="text-align:right;">Sub Total</td>
                                        <td>@Model.Amount.ToString("##00.00")</td>
                                        <td></td>
                                    </tr>
                                End If

                                If Model.Coupon <> "" Then
                                    @<tr>
                                        <td colspan="5" style="text-align:right">
                                            Coupon Code
                                            <form method="post" action="@Url.Content("~/cart/RemoveCoupon")">
                                                @Html.AntiForgeryToken()
                                                <input type="submit" value="Remove" Class="btn btn-link btn-sm" />
                                            </form>
                                        </td>
                                        <td>@Model.Coupon</td>
                                        <td></td>
                                    </tr>
                                    @<tr>
                                        <td colspan="5" class="text-end">
                                            Discount
                                        </td>
                                        <td>- @Model.Discount.ToString("##00.00")</td>
                                        <td></td>
                                    </tr>
                                End If


                            End If
                        </tbody>
                    </table>
                </div>
            </div>
            @If Model.OrderItems.Count > 0 Then
                @<form method="post" action="~/cart/setaddress">
                    @Html.AntiForgeryToken()
                    <div class="row">
                        <div class="col-sm-5 text-left">
                            <h4>Billing Details</h4>
                            <div class="mb-3">
                                <label for="nametxt" class="form-label">Name</label>
                                <input type="text" name="Name" maxlength="150" required class="form-control" id="nametxt" onchange="copyData('nametxt', 'snametxt');" />
                            </div>
                            <div class="mb-3">
                                <label for="emailtxt" class="form-label">Email</label>
                                <input type="email" name="Email" maxlength="250" required class="form-control" id="emailtxt" />
                            </div>
                            <div class="mb-3">
                                <label for="phonetxt" class="form-label">Phone</label>
                                <input type="text" name="Phone" maxlength="15" required class="form-control" id="phonetxt" onchange="copyData('phonetxt', 'sphonetxt');" />
                            </div>
                            <div class="mb-3">
                                <label for="addresstxt" class="form-label">Address</label>
                                <input type="text" name="BillingAddress" maxlength="250" required class="form-control" id="addresstxt" onchange="copyData('addresstxt', 'saddresstxt');" />
                            </div>
                            <div class="mb-3">
                                <label for="citytxt" class="form-label">City</label>
                                <input type="text" name="BillingCity" maxlength="150" required class="form-control" id="citytxt" onchange="copyData('citytxt', 'scitytxt');" />
                            </div>
                            <div class="mb-3">
                                <label for="pintxt" class="form-label">Pincode</label>
                                <input type="text" name="BillingPinCode" maxlength="10" required class="form-control" id="pintxt" onchange="copyData('pintxt', 'spintxt');" />
                            </div>
                            <div class="mb-3">
                                <label for="stateselect" class="form-label">State</label>
                                <select class="form-select" name="BillingState" id="stateselect" aria-label="Default select example" onchange="copyData('stateselect', 'sstateselect');">
                                    <option selected></option>
                                    <option value="Andaman and Nicobar Islands">
                                        Andaman and Nicobar Islands
                                    </option>
                                    <option value="Andhra Pradesh">
                                        Andhra Pradesh
                                    </option>
                                    <option value="Arunachal Pradesh">
                                        Arunachal Pradesh
                                    </option>
                                    <option value="Assam">
                                        Assam
                                    </option>
                                    <option value="Bihar">
                                        Bihar
                                    </option>
                                    <option value="Chandigarh">
                                        Chandigarh
                                    </option>
                                    <option value="Chhattisgarh">
                                        Chhattisgarh
                                    </option>
                                    <option value="Dadra and Nagar Haveli">
                                        Dadra and Nagar Haveli
                                    </option>
                                    <option value="Daman and Diu">
                                        Daman and Diu
                                    </option>
                                    <option value="Delhi" selected>
                                        Delhi
                                    </option>
                                    <option value="Goa">
                                        Goa
                                    </option>
                                    <option value="Gujarat">
                                        Gujarat
                                    </option>
                                    <option value="Haryana">
                                        Haryana
                                    </option>
                                    <option value="Himachal Pradesh">
                                        Himachal Pradesh
                                    </option>
                                    <option value="Jammu and Kashmir">
                                        Jammu and Kashmir
                                    </option>
                                    <option value="Jharkhand">
                                        Jharkhand
                                    </option>
                                    <option value="Karnataka">
                                        Karnataka
                                    </option>
                                    <option value="Kerala">
                                        Kerala
                                    </option>
                                    <option value="Lakshadweep">
                                        Lakshadweep
                                    </option>
                                    <option value="Madhya Pradesh">
                                        Madhya Pradesh
                                    </option>
                                    <option value="Maharashtra">
                                        Maharashtra
                                    </option>
                                    <option value="Manipur">
                                        Manipur
                                    </option>
                                    <option value="Meghalaya">
                                        Meghalaya
                                    </option>
                                    <option value="Mizoram">
                                        Mizoram
                                    </option>
                                    <option value="Nagaland">
                                        Nagaland
                                    </option>
                                    <option value="Odisha">
                                        Odisha
                                    </option>
                                    <option value="Puducherry">
                                        Puducherry
                                    </option>
                                    <option value="Punjab">
                                        Punjab
                                    </option>
                                    <option value="Rajasthan">
                                        Rajasthan
                                    </option>
                                    <option value="Sikkim">
                                        Sikkim
                                    </option>
                                    <option value="Tamil Nadu">
                                        Tamil Nadu
                                    </option>
                                    <option value="Tripura">
                                        Tripura
                                    </option>
                                    <option value="Uttar Pradesh">
                                        Uttar Pradesh
                                    </option>
                                    <option value="Uttarakhand">
                                        Uttarakhand
                                    </option>
                                    <option value="West Bengal">
                                        West Bengal
                                    </option>
                                    <option value="Telangana">Telangana</option>
                                </select>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Country</label>
                                <select class="form-select" name="BillingCountry" aria-label="Default select example">
                                    <option selected value="India">India</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-sm-5">
                            <h4>Shipping Details</h4>
                            <div class="mb-3">
                                <label for="snametxt" class="form-label">Name</label>
                                <input type="text" name="ShippingName" maxlength="150" required class="form-control" id="snametxt" />
                            </div>
                            <div class="mb-3">
                                <label for="sphonetxt" class="form-label">Phone</label>
                                <input type="text" name="ShippingPhone" maxlength="15" required class="form-control" id="sphonetxt" />
                            </div>
                            <div class="mb-3">
                                <label for="saddresstxt" class="form-label">Address</label>
                                <input type="text" name="ShippingAddress" maxlength="250" required class="form-control" id="saddresstxt" />
                            </div>
                            <div class="mb-3">
                                <label for="scitytxt" class="form-label">City</label>
                                <input type="text" name="ShippingCity" maxlength="150" required class="form-control" id="scitytxt" />
                            </div>
                            <div class="mb-3">
                                <label for="spintxt" class="form-label">Pincode</label>
                                <input type="text" name="ShippingPinCode" maxlength="10" required class="form-control" id="spintxt" />
                            </div>
                            <div class="mb-3">
                                <label for="sstateselect" class="form-label">State</label>
                                <select class="form-select" name="ShippingState" id="sstateselect" aria-label="Default select example">
                                    <option selected></option>
                                    <option value="Andaman and Nicobar Islands">
                                        Andaman and Nicobar Islands
                                    </option>
                                    <option value="Andhra Pradesh">
                                        Andhra Pradesh
                                    </option>
                                    <option value="Arunachal Pradesh">
                                        Arunachal Pradesh
                                    </option>
                                    <option value="Assam">
                                        Assam
                                    </option>
                                    <option value="Bihar">
                                        Bihar
                                    </option>
                                    <option value="Chandigarh">
                                        Chandigarh
                                    </option>
                                    <option value="Chhattisgarh">
                                        Chhattisgarh
                                    </option>
                                    <option value="Dadra and Nagar Haveli">
                                        Dadra and Nagar Haveli
                                    </option>
                                    <option value="Daman and Diu">
                                        Daman and Diu
                                    </option>
                                    <option value="Delhi" selected>
                                        Delhi
                                    </option>
                                    <option value="Goa">
                                        Goa
                                    </option>
                                    <option value="Gujarat">
                                        Gujarat
                                    </option>
                                    <option value="Haryana">
                                        Haryana
                                    </option>
                                    <option value="Himachal Pradesh">
                                        Himachal Pradesh
                                    </option>
                                    <option value="Jammu and Kashmir">
                                        Jammu and Kashmir
                                    </option>
                                    <option value="Jharkhand">
                                        Jharkhand
                                    </option>
                                    <option value="Karnataka">
                                        Karnataka
                                    </option>
                                    <option value="Kerala">
                                        Kerala
                                    </option>
                                    <option value="Lakshadweep">
                                        Lakshadweep
                                    </option>
                                    <option value="Madhya Pradesh">
                                        Madhya Pradesh
                                    </option>
                                    <option value="Maharashtra">
                                        Maharashtra
                                    </option>
                                    <option value="Manipur">
                                        Manipur
                                    </option>
                                    <option value="Meghalaya">
                                        Meghalaya
                                    </option>
                                    <option value="Mizoram">
                                        Mizoram
                                    </option>
                                    <option value="Nagaland">
                                        Nagaland
                                    </option>
                                    <option value="Odisha">
                                        Odisha
                                    </option>
                                    <option value="Puducherry">
                                        Puducherry
                                    </option>
                                    <option value="Punjab">
                                        Punjab
                                    </option>
                                    <option value="Rajasthan">
                                        Rajasthan
                                    </option>
                                    <option value="Sikkim">
                                        Sikkim
                                    </option>
                                    <option value="Tamil Nadu">
                                        Tamil Nadu
                                    </option>
                                    <option value="Tripura">
                                        Tripura
                                    </option>
                                    <option value="Uttar Pradesh">
                                        Uttar Pradesh
                                    </option>
                                    <option value="Uttarakhand">
                                        Uttarakhand
                                    </option>
                                    <option value="West Bengal">
                                        West Bengal
                                    </option>
                                    <option value="Telangana">Telangana</option>
                                </select>
                            </div>
                            <div class="mb-3">
                                <label class="form-label">Country</label>
                                <select class="form-select" name="ShippingCountry" aria-label="Default select example">
                                    <option selected value="India">India</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="text-end">
                        <button type="submit" class="btn btn-primary">Proceed to Checkout</button>
                    </div>
                </form>
            End If
        </div>
    </div>
</div>

@section scripts
    <script>
        function copyData(src, trg) {
            $("#" + trg).val($("#" + src).val())
        }
    </script>
End Section

