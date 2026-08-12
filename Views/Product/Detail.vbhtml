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

        function notifyMe(productId) {
    var email = $('#notifyEmail').val().trim();
    var honeypot = $('#notifyPhone').val();
    var btn = $('#notifyBtn');
    var msg = $('#notifyMsg');

    if (email === '') {
        msg.removeClass('text-success text-danger').addClass('text-danger').text('Please enter your email address.');
        return;
    }

    // Basic client-side email format check
    var emailRegex = /^[^\s@@]+@@[^\s@@]+\.[^\s@@]+$/;
    if (!emailRegex.test(email)) {
        msg.removeClass('text-success text-danger').addClass('text-danger').text('Please enter a valid email address.');
        return;
    }

    // Disable button to prevent double submission
    btn.prop('disabled', true).text('Sending...');
    msg.text('');

    $.ajax({
        url: '@Url.Action("NotifyMe", "Product")',
        type: 'POST',
        data: {
            productId: productId,
            email: email,
            honeypot: honeypot,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response.success) {
                $('#notifyEmail').val('');
                msg.removeClass('text-danger').addClass('text-success').text(response.message);
                btn.prop('disabled', true).text('Notified');
            } else {
                msg.removeClass('text-success').addClass('text-danger').text(response.message);
                btn.prop('disabled', false).text('Notify me');
            }
        },
        error: function () {
            msg.removeClass('text-success').addClass('text-danger').text('Something went wrong. Please try again.');
            btn.prop('disabled', false).text('Notify me');
        }
    });
}
    </script>
End Section
<div class="container bg-white p-md-5">
    <h1 class="text-center">@Model.Name</h1>
    <div class="row">
        @If Model.ProductPhotoes.Count > 0 Then
            @<div Class="col-md-4 text-center">
                <div class="position-relative d-inline-block">
                    <img src="@Model.ProductPhotoes.FirstOrDefault(Function(t) t.Sequence = 1).ImagePath" alt="" Class="img-responsive" style="max-height:500px">
                    @If Model.OutofStock Then
                        @<span class="badge bg-secondary position-absolute top-0 start-0 m-2" style="font-size:0.9em;">Out of stock</span>
                    End If
                </div>
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
                    @If Model.MRP > Model.SalePrice Then
                        @<tr>
                            <td class="text-left">MRP</td>
                            <td><s>@Model.MRP.ToString("##00.00") ₹</s></td>
                        </tr>
                    End If
                    <tr>
                        <td class="text-left fw-bold text-success">Price</td>
                        <td class="fw-bold text-success" style="font-size:1.25em;">@Model.SalePrice.ToString("##00.00") ₹</td>
                    </tr>
                </tbody>
            </table>
            @If Not Model.OutofStock Then
                @<form method="get" action="@Url.Content("~/cart/add/" & Model.ID)">
                    <button class="btn btn-primary mb-3">Add to Cart</button>
                </form>
            Else
                @<div class="mb-3">
                    @Html.AntiForgeryToken()
                    <button type="button" class="btn btn-outline-secondary" disabled>Out of stock</button>
                    <div class="mt-2" style="max-width:400px;">
                        <div class="d-flex">
                            <input type="email" id="notifyEmail" class="form-control me-2" placeholder="Your email" maxlength="250" />
                            <button type="button" class="btn btn-dark" id="notifyBtn" onclick="notifyMe(@Model.ID)">Notify me</button>
                        </div>
                        <input type="text" id="notifyPhone" name="phone" style="display:none;" tabindex="-1" autocomplete="off" />
                        <div id="notifyMsg" class="mt-2 small"></div>
                    </div>
                </div>
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

