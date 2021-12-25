@ModelType IndiaBobbles.Order
@Code
    ViewData("Title") = "Detail"
End Code
<div class="container bg-white p-2 fullbody">
    <div class="row">
        <div class="col-12">
            <h1>Order Number : @Model.ID</h1>
            <span>Order Date : @Model.DateCreated.ToShortDateString()</span><br /> <span>
                Status : @Select Case Model.Status
                    Case 1
    @<span class="badge bg-light text-dark">New</span>
                    Case 2
    @<span class="badge bg-primary">In Process</span>
                    Case 3
    @<span class="badge bg-warning">Paid</span>
                    Case 4
    @<span class="badge bg-warning">Paid</span>
                    Case 5
    @<span class="badge bg-info">Shipped</span>
                    Case 6
    @<span class="badge bg-success">Complete</span>
                    case 7
    @<span>Refunded</span>
                    Case 8
                        @<span class="badge bg-dark">Deleted</span>
                End Select
            </span>
            <span>Payment Mode: @Model.PaymentMode</span><br />
            @If (Model.ShippingService <> "") Then
                @<span>Shipping Service: @Model.ShippingService</span>
                @<br />
            End If
            @If (Model.ShippingTrackCode <> "") Then
                @<span>
                    Tracking Code: @Model.ShippingTrackCode
                </span>
                @<br />
            End If
            @If (Model.TransactionCode <> "") Then
                @<span>
                    Transaction Code: @Model.TransactionCode
                </span>
                @<br />
            End If
            <table class="table">
                <thead>
                    <tr>
                        <th scope="col">Name</th>
                        <th scope="col">Code</th>
                        <th scope="col">Quantity</th>
                        <th scope="col">Price</th>
                        <th scope="col">Amount</th>
                    </tr>
                </thead>
                <tbody>
                    @If Model IsNot Nothing Then
                        @For Each oi In Model.OrderItems
                            @<tr>
                                <th scope="row">@oi.ProductName</th>
                                <td>@oi.ProductCode</td>
                                <td>
                                    @oi.Quantity
                                </td>
                                <td>@oi.Price.ToString("###0.00")</td>
                                <td>@oi.Amount.ToString("###0.00")</td>
                            </tr>
                        Next
                        @<tr>
                            <td colspan="4" class="text-right">Sub Total</td>
                            <td>@Model.Amount.ToString("###0.00")</td>
                        </tr>
                        @if Model.Coupon <> "" Then
                            @<tr>
                                <td colspan="4" class="text-right">Coupon</td>
                                <td>@Model.Coupon</td>
                            </tr>
                        End If
                        @if Model.Discount > 0 Then
                            @<tr>
                                <td colspan="4" class="text-right">Discount</td>
                                <td>- @Model.Discount.ToString("###0.00")</td>
                            </tr>
                        End If
                        @<tr>
                            <td colspan="4" class="text-right">Shipping Fee</td>
                            <td>@Model.ShippingPrice.ToString("###0.00")</td>
                        </tr>
                        @if (Model.PaymentMode = "COD") Then
                            @<tr>
                                <td colspan="4" class="text-right">Cash On Delivery Fee</td>
                                <td>@Model.COD.ToString("###0.00")</td>
                            </tr>
                        End If
                        @<tr>
                            <td colspan="4" class="text-right">Total</td>
                            <td>@Model.Total.ToString("###0.00")</td>
                        </tr>
                    End If
                </tbody>
            </table>
            <p>
                Thanks for your Purchase. If you have any question please contact us at @Html.Raw("iborder." & Model.ID & "@rudrasofttech.com") or call us at 9871500276.
            </p>
            <form method="post" action="@Url.Content("~/orders/receipt/" & Model.ID)" target="_blank">
                @Html.AntiForgeryToken()
                <input type="submit" value="Print Receipt" Class="btn btn-primary m-2" />
            </form>
            @If Not String.IsNullOrEmpty(Model.Email) Then
                @<a href="@Url.Content("~/orders/email/" & Model.ID)" Class="btn btn-secondary m-2">Email Receipt</a>
            End If

        </div>
    </div>
</div>


