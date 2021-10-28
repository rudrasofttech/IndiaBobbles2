@ModelType List(Of IndiaBobbles.Order)
@Code
    ViewData("Title") = "My Orders"
End Code

<div class="container bg-white pt-2 fullbody">
    <div class="row">
        <div class="col-12 text-center">
            <h1>My Orders</h1>
            <p>Search your previous orders by Order Id or your email or your phone.</p>
            <div class="pt-2 pb-2">
                <form class="row row-cols-lg-auto g-3 align-items-center" method="get">
                    <div class="col-12">
                        <div class="input-group">
                            <input type="text" value="@Request.QueryString("q")" required class="form-control" name="q" style="width:300px;" placeholder="Search by Order id or Email or Phone" maxlength="250" />
                        </div>
                    </div>
                    <div class="col-12">
                        <button type="submit" class="btn btn-light">Search</button>
                    </div>
                </form>
                <div class="table-responsive">
                    <table class="table">
                        <thead>
                            <tr>
                                <th scope="col">#</th>
                                <th scope="col">Name</th>
                                <th scope="col">Contact</th>
                                <th scope="col">Item Count</th>
                                <th scope="col">Total</th>
                                <th scope="col">Date</th>
                                <th scope="col">Status</th>
                                <th scope="col"></th>
                            </tr>
                        </thead>
                        <tbody>
                            @If Model IsNot Nothing Then
                                @For Each o In Model
                                    @<tr>
                                        <th scope="row">@o.ID</th>
                                        <td>@o.Name</td>
                                        <td>
                                            @o.Email <br /> @o.Phone
                                        </td>
                                        <td>@o.OrderItems.Count</td>
                                        <td>@o.Total</td>
                                        <td>@o.DateCreated</td>
                                        <td>
                                            @Select Case o.Status
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
                                        </td>
                                        <td>
                                            <a href="~/orders/detail/@o.ID">Detail</a>
                                        </td>
                                    </tr>
                                Next
                                If Model.Count = 0 Then
                                    @<tr>
                                        <td colspan="8">No Orders Found</td>
                                    </tr>
                                End If
                            End If
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>
</div>


