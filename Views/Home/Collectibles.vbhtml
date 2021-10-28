@ModelType List(Of IndiaBobbles.Product)

@Code
    ViewData("Title") = "Collectibles"
End Code

<div class="container pt-2 fullbody">
    <div class="row">
        <h1 class="text-center"><img src="//www.indiabobbles.com/drive/theme/khichdi/img/Cult-India-collectibles-Log.png" alt="Cult India Collectibles"></h1>
        @If Model IsNot Nothing Then
            @For Each p In Model
                @<div Class="col-md-4">
                    <a href="~/@IndiaBobbles.Utility.Slugify(p.Name)">
                        <img src="@p.ThumbPath" class="img-fluid p-2" />
                    </a>
                </div>
            Next
        End If
    </div>
</div>

