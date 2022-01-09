@Imports IndiaBobbles
@Code
    ViewData("Title") = "Buy Bobble Heads & Figurines Online in India – India Bobbles"

End Code

@section meta
    <meta name="description" content="Shop the best range of Bollywood and Indian bobble heads each piece hand painted by master artists. Start collecting now, Free Shipping all over India." />
    <meta name="Keywords" Content="buy bobblehead online india, buy bobbleheads online, bobble head figurines, bobble heads, indian bobble heads, india bobbles, indian figurines" />
End Section
@section head
    <meta property="og:type" content="article" />
    <meta property="og:url" content="//www.indiabobbles.com/" />
    <meta property="og:site_name" content="IndiaBobbles" />
    <meta property="og:title" content="India Bobbles - Collectible Indian bobble heads, figurines and games." />
    <meta property="og:description" content="Shop the best range of Bollywood and Indian bobble heads each piece hand painted by master artists. Start collecting now, Free Shipping all over India." />
    <meta property="og:image" content="//www.indiabobbles.com/drive/theme/khichdi/img/Agni-poster.jpg" />
End Section
@If ViewBag.Highlights.Count > 0 Then
    @<div class="container-fluid">
        <div class="row  align-items-center" style="min-height: calc(100vh - 132px);">
            <div class="col-12">
                <div id="highlightCarousel" Class="carousel carousel-dark slide" data-bs-ride="carousel">
                    <div Class="carousel-indicators">
                        @For index = 0 To ViewBag.Highlights.Count - 1
                            @<Button type="button" data-bs-target="#highlightCarousel" data-bs-slide-To="@index" class="@Html.Raw(IIf(index = 0, "active", ""))" aria-current="true" aria-label="Slide @(index + 1)"></Button>
                        Next
                    </div>
                    <div Class="carousel-inner">
                        @For index = 0 To ViewBag.Highlights.Count - 1
                            Dim p As Product = ViewBag.Highlights(index)
                            @<div class="carousel-item @Html.Raw(IIf(index = 0, "active", ""))" data-bs-interval="10000">
                                <div class="row  align-items-center">
                                    <div class="col-md-7 mb-3 mb-md-0 text-center text-md-end">
                                        <h2>@p.Name</h2>
                                        <h3 class="text-success">@p.SalePrice.ToString("C")</h3>
                                        @If p.Description.Length > 100 Then
                                            @<p>@p.Description.Substring(0, 97) ...</p>
                                        Else
                                            @<p>@p.Description</p>
                                        End If
                                        <a class="btn btn-dark mt-3" href="@Url.Content("~/product/" & p.ID & "/" & IndiaBobbles.Utility.Slugify(p.Name))">View Detail</a>
                                        @If Not p.OutofStock Then
                                            @<form method="get" class="d-inline" action="@Url.Content("~/cart/add/" & p.ID)">
                                                <button class="btn btn-success mt-3">Buy Now</button>
                                            </form>
                                        End If
                                    </div>
                                    <div class="col-md-5 text-center text-md-start"><img src="@p.ThumbPath" alt="..." class="img-fluid" /></div>
                                </div>
                            </div>
                        Next
                        @*<div class="carousel-item" data-bs-interval="5000">
                                <div class="row  align-items-center">
                                    <div class="col-md-7 text-end">
                                        <h5>Agni</h5>
                                        <p>Save New Delhi from incoming missile attacks.</p>
                                        <p><a Class="btn btn-lg btn-primary" href="~/games">Play Game</a></p>
                                    </div>
                                    <div class="col-md-5 text-start">
                                        <img src="~/theme/khichdi/img/Agni%20Promo%20Art.jpg" class="img-fluid" alt="..." />
                                    </div>
                                </div>
                            </div>
                            <div class="carousel-item" data-bs-interval="5000">
                                <div class="row  align-items-center">
                                    <div class="col-md-7 text-end">
                                        <h5>Meteor Treasure</h5>
                                        <p> Navigate the Mars rover save it from ongoing meteor fall.</p>
                                        <p> <a Class="btn btn-lg btn-primary" href="~/games">Play Game</a></p>
                                    </div>
                                    <div class="col-md-5 text-start">
                                        <img src="~/theme/khichdi/img/meteor-new-poster-comp.jpg" class="img-fluid" alt="..." />
                                    </div>
                                </div>
                            </div>*@
                    </div>
                    <Button Class="carousel-control-prev" type="button" data-bs-target="#highlightCarousel" data-bs-slide="prev">
                        <Span Class="carousel-control-prev-icon" aria-hidden="true"></Span>
                        <Span Class="visually-hidden">Previous</Span>
                    </Button>
                    <Button Class="carousel-control-next" type="button" data-bs-target="#highlightCarousel" data-bs-slide="next">
                        <Span Class="carousel-control-next-icon" aria-hidden="true"></Span>
                        <Span Class="visually-hidden">Next</Span>
                    </Button>
                </div>
            </div>
        </div>
    </div>
End If
@*<div Class="container-fluid">
        <div Class="row">
            <div Class="col-sm-6 text-center">
                <a href="~/collectibles" Class="link-dark">
                    <img src="~/theme/khichdi/img/collectibles-Sketchy.jpg" alt="collectibles" Class="img-fluid img-rounded p-4" />
                    <h3> Collectibles</h3>
                </a>
            </div>
            <div Class="col-sm-6 text-center">
                <a href="~/games" Class="link-dark">
                    <img src="~/theme/khichdi/img/agni-game.jpg" alt="agni-game" Class="img-fluid rounded p-4">
                    <h3> Games</h3>
                </a>
            </div>
        </div>
    </div>*@
