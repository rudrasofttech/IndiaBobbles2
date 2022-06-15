@ModelType IndiaBobbles.Order

@Code
    Layout = Nothing
End Code

<!DOCTYPE html>

<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <title>India Bobbles Order ID @Model.ID Receipt</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3" crossorigin="anonymous" />
</head>
<body>
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
        @<span class="badge bg-warning">Cash on Delivery</span>
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
                <br /><br/>
                      <span>
                          <strong>Seller:</strong> Rudra Softtech LLP<br/>
                          <strong>PAN No:</strong> AAMFR0653C<br/>
                          <strong>GST Registration No:</strong> 07AAMFR0653C2ZD
                      </span>
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
                                    <td>₹ @oi.Price.ToString("###0.00")</td>
                                    <td>₹ @oi.Amount.ToString("###0.00")</td>
                                </tr>
                            Next
                            @<tr>
                                <td colspan="4" class="text-right">Sub Total</td>
                                <td>₹ @Model.Amount.ToString("###0.00")</td>
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
                                    <td>- ₹ @Model.Discount.ToString("###0.00")</td>
                                </tr>
                            End If
                            @<tr>
                                <td colspan="4" class="text-right">Shipping Fee</td>
                                <td>₹ @Model.ShippingPrice.ToString("###0.00")</td>
                            </tr>
                            @if (Model.PaymentMode = "COD") Then
                                @<tr>
                                    <td colspan="4" class="text-right">Cash On Delivery Fee</td>
                                    <td>₹ @Model.COD.ToString("###0.00")</td>
                                </tr>
                            End If
                            @<tr>
                                <td colspan="4" class="text-right">Total</td>
                                <td>₹ @Model.Total.ToString("###0.00")</td>
                            </tr>
                        End If
                    </tbody>
                </table>
                <div class="row">
                    <div class="col-md-4">
                        <h3>Billing Address:</h3>
                        <address>
                            <strong>@Model.Name, @Model.Phone </strong>
                            <br />
                            @Model.BillingAddress <br /> 
                            @Model.BillingCity, @Model.BillingState <br /> 
                            @Model.BillingCountry <br />
                            @Model.BillingZip
                        </address>
                    </div>
                    <div class="col-md-4">
                        <h3>Shipping Address:</h3>
                        <address>
                            <strong>@Model.ShippingFirstName @Model.ShippingLastName , @Model.ShippingPhone </strong>
                            <br />
                            @Model.ShippingAddress <br/>
                            @Model.ShippingCity , @Model.ShippingState <br/> 
                            @Model.ShippingCountry <br />
                            @Model.ShippingZip 
                        </address>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>
