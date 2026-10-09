@ModelType IndiaBobbles.UnsubscribeDTO
@Imports IndiaBobbles

@Code
    ViewData("Title") = "Unsubscribe from Emails"

    Dim inr = New System.Globalization.CultureInfo("en-IN")
    Dim highlights As New List(Of Product)()
    If ViewBag.Highlights IsNot Nothing Then
        highlights = CType(ViewBag.Highlights, IEnumerable(Of Product)).Take(4).ToList()
    End If

    Dim isDone As Boolean = ViewBag.Success IsNot Nothing
    Dim emailShown As String = If(Model IsNot Nothing, Model.Email, "")
End Code

<style>
    
</style>

<div class="container fullbody py-4 ib-unsub">

    <div class="u-card">
        @If isDone Then
            @* ---------- Done state ---------- *@
            @<div class="u-top">
                <div class="u-ico"><i class="fa fa-check"></i></div>
                <h1>You're unsubscribed</h1>
                <p>@ViewBag.Success</p>
                @If Not String.IsNullOrWhiteSpace(emailShown) Then
                    @<span class="u-email">@emailShown</span>
                End If
            </div>
            @<div class="u-body">
                <div class="u-note">
                    <b>No more promotional emails.</b> It can take up to 48 hours for any emails already on their way to stop.
                    You'll still get emails about orders you place, like receipts and shipping updates.
                </div>
                <p class="mb-3" style="color:#3e3445;">
                    Unsubscribed by mistake? Just write to
                    <a href="mailto:indiabobbles@rudrasofttech.com?subject=Please%20resubscribe%20me" style="color:#330B3F;font-weight:700;">indiabobbles@rudrasofttech.com</a>
                    and we'll add you back.
                </p>
                <div class="u-actions">
                    <a class="a1" href="@Url.Content("~/")"><i class="fa fa-shopping-bag"></i> Continue shopping</a>
                    <a class="a2" href="@Url.Content("~/order-custom-bobbleheads")"><i class="fa fa-paint-brush"></i> Custom bobblehead</a>
                </div>
            </div>
        Else
            @* ---------- Form state ---------- *@
            @<div class="u-top">
                <div class="u-ico"><i class="fa fa-envelope-o"></i></div>
                <h1>Sorry to see you go</h1>
                <p>Confirm your email to stop our promotional emails.</p>
            </div>
            @<div class="u-body">
                @If ViewBag.Error IsNot Nothing Then
                    @<div class="u-err" role="alert"><i class="fa fa-exclamation-circle"></i> @ViewBag.Error</div>
                End If

                <ul class="u-list">
                    <li class="no"><i class="fa fa-times"></i><span>You'll stop getting new arrivals, offers and newsletters.</span></li>
                    <li class="yes"><i class="fa fa-check"></i><span>You'll still get receipts and shipping updates for your orders.</span></li>
                </ul>

                @Using (Html.BeginForm())
                    @Html.AntiForgeryToken()
                    @Html.ValidationSummary(True, "", New With {.class = "u-err d-block"})

                    @<div class="mb-3">
                        @Html.LabelFor(Function(m) m.Email, htmlAttributes:=New With {.class = "form-label"})
                        @Html.TextBoxFor(Function(m) m.Email, New With {.class = "form-control", .maxlength = "250", .placeholder = "you@example.com", .type = "email", .autocomplete = "email", .required = "required"})
                        @Html.ValidationMessageFor(Function(m) m.Email, "", New With {.class = "text-danger small"})
                    </div>

                    @<button type="submit" class="u-btn">Unsubscribe me</button>
                End Using

                <a class="u-ghost" href="@Url.Content("~/")">No, keep me subscribed</a>
            </div>
        End If
    </div>

    @If highlights.Count > 0 Then
        @<div class="u-more">
            <h2>@(If(isDone, "Before you go, our favourites", "Still here? See what's new"))</h2>
            <p class="sub">Hand-painted by our artists, with free shipping across India.</p>
            <div class="row row-cols-2 row-cols-md-4 g-3">
                @For Each item In highlights
                    Dim productUrl As String = Url.Content("~/product/" & item.ID & "/" & Utility.Slugify(item.Name))
                    Dim onSale As Boolean = item.MRP > item.SalePrice AndAlso item.MRP > 0
                    Dim price As Decimal = If(onSale, item.SalePrice, item.MRP)
                    @<div class="col">
                        <a class="u-prod" href="@productUrl">
                            <div class="img">
                                @If Not String.IsNullOrWhiteSpace(item.ThumbPath) Then
                                    @<img src="@item.ThumbPath" alt="@item.Name" loading="lazy" />
                                End If
                                @If onSale Then
                                    @<span class="off">@(Math.Round((item.MRP - item.SalePrice) * 100 / item.MRP))% OFF</span>
                                End If
                            </div>
                            <div class="b">
                                <div class="n">@item.Name</div>
                                <div class="p">
                                    ₹@(price.ToString("#,##0.##", inr))
                                    @If onSale Then
                                        @<s>₹@(item.MRP.ToString("#,##0.##", inr))</s>
                                    End If
                                </div>
                            </div>
                        </a>
                    </div>
                Next
            </div>
        </div>
    End If
</div>