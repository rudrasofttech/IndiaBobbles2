@ModelType List(Of IndiaBobbles.Post)
@Code
    ViewData("Title") = "Meteor Treasure | Agni Game Online - India Bobbles"
End Code

@section meta
    <meta name="description" content="Play free online games like as Agni and Meteor treasure. These games are popular and are in more demand on windows, android operating systems. Download now." />
    <meta name="Keywords" Content="agni game online, meteor treasure game online" />
End Section
@section head
    <style>
        body {
            background-image: url(//www.indiabobbles.com/drive/theme/khichdi/img/Agni-Screenshot-1.jpg);
            background-attachment: fixed;
            background-position: bottom;
            background-size: cover;
            background-repeat: no-repeat;
        }

        .gamesnippet {
            color: #fff;
        }

            .gamesnippet p {
                font-weight: normal;
                font-size: 15px;
            }
    </style>
End Section
<div class="container pt-2">
    <div class="row row-cols-1 row-cols-md-3 g-4">
        <div class="col">
            <div class="card">
                <div style="max-height:200px;overflow-y:hidden;">
                    <img src="//www.indiabobbles.com/drive/theme/khichdi/img/agni-game-icon.JPG" class="card-img-top" alt="" />
                </div>
                <div class="card-body">
                    <h5 class="card-title">Agni</h5>
                    <p class="card-text">Get ready to defend your city against a foe who is hell bent on destroying it. It seems the end of days are near, as no one talks of peace anymore. Once the onslaught begins you won't have much time to react. Every single shot should be perfectly timed, no room for any miscalculation, one miss and millions perish. </p>
                    <a href="~/agni/index.html" target="_blank" ><img src="//www.indiabobbles.com/drive/theme/khichdi/img/play-on-web.png" alt="play-on-web" style="padding-top:10px; height:70px;" /></a>
                </div>
            </div>
        </div>
        <div class="col">
            <div class="card">
                <div style="max-height:200px;overflow-y:hidden;">
                    <img src="//www.indiabobbles.com/drive/theme/khichdi/img/meteor-icon.png" class="card-img-top" alt="" />
                </div>
                <div class="card-body">
                    <h5 class="card-title">Meteor Treasure</h5>
                    <p class="card-text">
                        Welcome to the Red Planet, you have been assigned to navigate the surface of the planet and collect soil samples. Unfortunately your rover is in the midst of a meteor shower which doesn't seem to stop. Now it's your responsibility to save the rover.
                        And while at it, collect the buried treasure inside the collided meteors and score your best.
                    </p>
                    <a href="~/meteortreasure/index.html" target="_blank"><img src="//www.indiabobbles.com/drive/theme/khichdi/img/play-on-web.png" alt="play-on-web" style="padding-top:10px; height:70px;" /></a>
                </div>
            </div>
        </div>
        @If Model IsNot Nothing Then
            @For Each m In Model
                @<div class="col">
                    <div class="card">
                        <div style="max-height:200px;overflow-y:hidden;">
                            <img src="@m.OGImage" class="card-img-top" alt="" />
                        </div>
                        <div class="card-body">
                            <h5 class="card-title">@m.Title</h5>
                            <p class="card-text">@Html.Raw(m.Description)</p>
                            <a href="@Url.Content("~/blog/" & m.URL)"><img src="//www.indiabobbles.com/drive/theme/khichdi/img/play-on-web.png" alt="play-on-web" style="padding-top:10px; height:70px;" /></a>
                        </div>
                    </div>
                </div>
            Next
        End If
    </div>
</div>

