@ModelType IndiaBobbles.Product
@Code
    ViewData("Title") = Model.Name
End Code
@section head
    <link href="//www.rudrasofttech.com/js-tools/modal/modal.css" rel="Stylesheet" />
    <link href="//www.rudrasofttech.com/js-tools/modal/modal-darktheme.css" id="modaltheme" rel="Stylesheet" />
    <style>
        .thumb img {
            height: 100px;
            margin: 3px;
            padding: 0px;
        }
    </style>
End Section
@section scripts
    <script src="//www.rudrasofttech.com/js-tools/modal/modal.js" type="text/javascript"></script>
    <script>
        $(document).ready(function () {
            $(".thumb").modalbox({
                Type: 'image',
                height: 700
            });
        });
    </script>
End Section
<div class="container bg-white p-md-5">
    <h1 class="text-center">@Model.Name</h1>
    <div class="row">
        @If Model.ProductPhotoes.Count > 0 Then
            @<div Class="col-md-4 text-center">
                <img src="@Model.ProductPhotoes.FirstOrDefault(Function(t) t.Sequence = 1).ImagePath" alt="" Class="img-responsive" style="max-height:500px">
                <h5><b>Gallery</b></h5>
                <ul class="gallery list-inline">
                    @For Each pp In Model.ProductPhotoes
                        @<li Class="gallerycolumn list-inline-item">
                            <a href="@pp.ImagePath" Class="thumb" rel="group1">
                                <img src="@pp.ImagePath">
                            </a>
                        </li>
                    Next
                </ul>
            </div>
        End If
        <div Class="col-md-8">
            <p>@Html.Raw(Model.Description)</p>
            <table class="table table-bordered">
                <tbody>
                    <tr>
                        <td class="text-left">MRP</td>
                        <td>@Model.MRP.ToString("##00.00") ₹</td>
                    </tr>
                    <tr>
                        <td class="text-left fw-bold text-success">Sale Price</td>
                        <td class="fw-bold text-success" style="font-size:1.25em;">@Model.SalePrice.ToString("##00.00") ₹</td>
                    </tr>
                </tbody>
            </table>
            @If Not Model.OutofStock Then
                @<form method="get" action="@Url.Content("~/cart/add/" & Model.ID)">
                    <button class="btn btn-primary mb-3">Add to Cart</button>
                </form>
            End If
            <h4> More Details</h4>
            <Table Class="table table-bordered">
                <tbody>
                    <tr>
                        <td class="text-left">Fragile</td>
                        @If Model.Fragile Then
                            @<td class="text-danger">Yes</td>
                        Else
                            @<td>No</td>
                        End If

                    </tr>
                    <tr>
                        <td class="text-left">Handmade</td>
                        @If Model.Handmade Then
                            @<td>Yes</td>
                        Else
                            @<td>No</td>
                        End If
                    </tr>
                    <tr>
                        <td Class="text-left">Dimension</td>
                        <td>@Model.Dimension</td>
                    </tr>
                    <tr>
                        <td class="text-left">Color</td>
                        <td>@Model.Color</td>
                    </tr>
                    <tr>
                        <td class="text-left">Weight</td>
                        <td>@Model.Weight</td>
                    </tr>
                    <tr>
                        <td class="text-left">Material</td>
                        <td>@Model.Material</td>
                    </tr>
                    <tr>
                        <td class="text-left">Manufacturer</td>
                        <td>@Model.Manufacturer</td>
                    </tr>
                    <tr>
                        <td class="text-left">Care Instructions</td>
                        <td>@Model.CareInstructions</td>
                    </tr>
                    <tr>
                        <td class="text-left">Recommended Age</td>
                        <td>@Model.RecommendedAge</td>
                    </tr>
                    <tr>
                        <td class="text-left">Country of Origin</td>
                        <td>@Model.CountryofOrigin</td>
                    </tr>

                    <tr>
                        <td class="text-left">Shipping Time</td>
                        <td>@Model.ShippingTime</td>
                    </tr>
                </tbody>
            </Table>
        </div>
    </div>
</div>

