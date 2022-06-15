@ModelType IndiaBobbles.Order
@Code
    ViewData("Title") = "Cart"
End Code

<div Class="container bg-white pt-2 fullbody">
    <div Class="row">
        <div Class="col-12 ">
            <h1 Class="text-center">Cart</h1>
            @If Model IsNot Nothing Then
                @<div class="p-2 text-center">
                    <a href="#" data-bs-toggle="modal" data-bs-target="#orderContactModal"><strong>@Model.Name</strong></a>, <a href="#" data-bs-toggle="modal" data-bs-target="#orderContactModal"><i>@Model.Email</i></a>, <a href="#" data-bs-toggle="modal" data-bs-target="#orderContactModal"><i>@Model.Phone</i></a>
</div>
            End If
            <div Class="pt-2 pb-2 mb-2">
                <div Class="table-responsive">
                    <Table Class="table">
                        <thead>
                            <tr>
                                <th scope="col"></th>
                                <th scope="col"> Product Name</th>
                                <th scope="col"> Product Code</th>
                                <th scope="col" Class="text-start">Quantity</th>
                                <th scope="col"> Price</th>
                                <th scope="col"> Amount</th>
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
                                        <td>₹ @o.Price.ToString("##00.00")</td>
                                        <td>₹ @o.Amount.ToString("##00.00")</td>
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
                                                <input type="hidden" name="returnto" value="Index" />
                                                <input type="submit" value="Apply" Class="btn btn-light btn-sm" />
                                            </form>
                                        </td>
                                        <td style="text-align:right;">Sub Total</td>
                                        <td>₹ @Model.Amount.ToString("##00.00")</td>
                                        <td></td>
                                    </tr>
                                End If

                                If Model.Coupon <> "" Then
                                    @<tr>
                                        <td colspan="5" style="text-align:right">
                                            Coupon Code
                                            <form method="post" action="@Url.Content("~/cart/RemoveCoupon")">
                                                @Html.AntiForgeryToken()
                                                <input type="hidden" name="returnto" value="Index" />
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
                                        <td>- ₹ @Model.Discount.ToString("##00.00")</td>
                                        <td></td>
                                    </tr>
                                End If


                            End If
                        </tbody>
                    </Table>
                </div>
            </div>
            @If Model.OrderItems.Count > 0 Then
                @<div class="text-end">
                    <a href="@Url.Content("~/cart/address")" class="btn btn-primary">Proceed To Checkout</a>
                </div>
            End If
        </div>
    </div>
</div>

