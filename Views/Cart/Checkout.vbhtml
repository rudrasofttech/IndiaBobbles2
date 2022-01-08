@ModelType IndiaBobbles.Order
@Code
    ViewData("Title") = "Checkout"
End Code

<div class="container bg-white pt-2 fullbody">
    <div class="row">
        <div class="col-12 ">
            <h1 class="text-center">Check Out</h1>
            <div class="pt-2 pb-2 mb-2">
                <div class="table-responsive">
                    <table class="table">
                        <thead>
                            <tr>
                                <th scope="col"></th>
                                <th scope="col">Product Name</th>
                                <th scope="col">Product Code</th>
                                <th scope="col">Quantity</th>
                                <th scope="col">Price</th>
                                <th scope="col">Amount</th>

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
                                        <td>
                                            @o.Quantity
                                        </td>
                                        <td>@o.Price.ToString("##00.00")</td>
                                        <td>@o.Amount.ToString("##00.00")</td>
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
                                                <input type="hidden" name="returnto" value="Checkout" />
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
                                                <input type="hidden" name="returnto" value="Checkout" />
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
                                @<tr>
                                    <td colspan="5" class="text-end">
                                        Shipping Fee
                                    </td>
                                    <td>0</td>
                                    <td></td>
                                </tr>
                                @<tr>
                                    <td colspan="5" class="text-end">
                                        Total
                                    </td>
                                    <td>@Model.Total.ToString("##00.00")</td>
                                    <td></td>
                                </tr>
                            End If
                        </tbody>
                    </table>
                </div>
                @If Model IsNot Nothing Then
                    @<div class="row">
                        <div class="col-md-4">
                            Billing Address:<br />
                            <a href="address" class="btn btn-sm btn-link">Change</a><br />
                            @Model.Name<br />
                            @Model.Email<br />
                            @Model.Phone<br />
                            <address>
                                @Model.BillingAddress<br />
                                @Model.BillingCity<br />
                                @Model.BillingState<br />
                                @Model.BillingZip
                            </address>
                        </div>
                        <div Class="col-md-4">
                            Shipping Address:<br />
                            <a href="address" class="btn btn-sm btn-link">Change</a><br />
                            @Model.ShippingFirstName @Model.ShippingLastName<br />
                            @Model.ShippingPhone<br />
                            <address>
                                @Model.ShippingAddress<br />
                                @Model.ShippingCity<br />
                                @Model.ShippingState<br />
                                @Model.ShippingZip
                            </address>
                        </div>
                        <div Class="col-md-4">
                            <form action="@ViewBag.action1" method="post">
                                <input type="hidden" name="hash" value="@ViewBag.hash1" />
                                <input type="hidden" name="key" value="@ViewBag.key" />
                                <input type="hidden" name="txnid" value="@Model.ID" />
                                <input type="hidden" name="amount" value="@Model.Total.ToString("g29")" />
                                <input type="hidden" name="firstname" value="@Model.Name" />
                                <input type="hidden" name="lastname" value="" />
                                <input type="hidden" name="email" value="@Model.Email" />
                                <input type="hidden" name="phone" value="@Model.Phone" />
                                <input type="hidden" name="productinfo" value="IndiaBobblesProducts" />
                                <input type="hidden" name="surl" value="https://www.indiabobbles.com/cart/payumoneyresponse?id=@Model.ID" />
                                <input type="hidden" name="furl" value="https://www.indiabobbles.com/cart/checkout?id=@Model.ID" />
                                <input type="hidden" name="curl" value="https://www.indiabobbles.com/cart/checkout?id=@Model.ID" />
                                <input type="hidden" name="address1" value="@Model.BillingAddress" />
                                <input type="hidden" name="address2" value="" />
                                <input type="hidden" name="city" value="@Model.BillingCity" />
                                <input type="hidden" name="state" value="@Model.BillingState" />
                                <input type="hidden" name="country" value="@Model.BillingCountry" />
                                <input type="hidden" name="zipcode" value="@Model.BillingZip" />
                                <input type="hidden" name="udf1" value="" />
                                <input type="hidden" name="udf2" value="" />
                                <input type="hidden" name="udf3" value="" />
                                <input type="hidden" name="udf4" value="" />
                                <input type="hidden" name="udf5" value="" />
                                <input type="hidden" name="pg" value="" />
                                <input type="hidden" name="service_provider" value="payu_paisa" />
                                <input type="image" src="~/theme/khichdi/img/Paynow.png" />
                            </form>
                        </div>
                    </div>
                End If
            </div>
        </div>
    </div>
</div>
