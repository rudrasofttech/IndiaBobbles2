@ModelType IEnumerable(Of IndiaBobbles.Product)
@Code
    ViewData("Title") = ViewBag.Tag
End Code

<div class="container pt-2 fullbody">
    <div class="row row-cols-1 row-cols-lg-4 row-cols-md-3 g-4">
        @For Each item In Model
            @<div Class="col">
                <div Class="card h-100">
                    @If Not String.IsNullOrEmpty(item.ThumbPath) Then
                        @<a href="@Url.Content("~/product/" & item.ID & "/" & IndiaBobbles.Utility.Slugify(item.Name))">
                            <img src="@item.ThumbPath" Class="card-img-top" alt="Photo of @item.Name" />
                        </a>
                    End If
                    <div Class="card-body">
                        <h5 Class="card-title">@item.Name</h5>
                    </div>
                    <ul class="list-group list-group-flush">
                        <li Class="list-group-item">MRP: <span class="badge text-danger">@item.MRP.ToString("C")</span></li>
                        <li Class="list-group-item">Our Price: <span class="badge bg-success">@item.SalePrice.ToString("C")</span></li>
                    </ul>
                    <div class="card-body">
                        <div class="row">
                            <div class="col-6"><a class="btn btn-primary" href="@Url.Content("~/product/" & item.ID & "/" & IndiaBobbles.Utility.Slugify(item.Name))">View Detail</a></div>
                            <div class="col-6 text-end">
                                @If Not item.OutofStock Then
                                    @<form method="get" action="@Url.Content("~/cart/add/" & item.ID)">
                                        <button class="btn btn-warning ">Buy Now</button>
                                    </form>
                                Else
                                    @<button disabled class="btn btn-warning">Out of Stock</button>
                                End If
                                                    </div>
                        </div>
                        
                        
                    </div>
                </div>
            </div>
        Next
    </div>

</div>