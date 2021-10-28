@ModelType IndiaBobbles2.CategoryTag
@Code
    ViewData("Title") = "Details"
End Code

<h2>Details</h2>

<div>
    <h4>CategoryTag</h4>
    <hr />
    <dl class="dl-horizontal">
        <dt>
            @Html.DisplayNameFor(Function(model) model.UrlName)
        </dt>

        <dd>
            @Html.DisplayFor(Function(model) model.UrlName)
        </dd>

        <dt>
            @Html.DisplayNameFor(Function(model) model.Description)
        </dt>

        <dd>
            @Html.DisplayFor(Function(model) model.Description)
        </dd>

        <dt>
            @Html.DisplayNameFor(Function(model) model.ImagePath)
        </dt>

        <dd>
            @Html.DisplayFor(Function(model) model.ImagePath)
        </dd>

        <dt>
            @Html.DisplayNameFor(Function(model) model.CreateDate)
        </dt>

        <dd>
            @Html.DisplayFor(Function(model) model.CreateDate)
        </dd>

        <dt>
            @Html.DisplayNameFor(Function(model) model.CreatedBy)
        </dt>

        <dd>
            @Html.DisplayFor(Function(model) model.CreatedBy)
        </dd>

        <dt>
            @Html.DisplayNameFor(Function(model) model.ModifiedBy)
        </dt>

        <dd>
            @Html.DisplayFor(Function(model) model.ModifiedBy)
        </dd>

        <dt>
            @Html.DisplayNameFor(Function(model) model.ModifyDate)
        </dt>

        <dd>
            @Html.DisplayFor(Function(model) model.ModifyDate)
        </dd>

        <dt>
            @Html.DisplayNameFor(Function(model) model.Status)
        </dt>

        <dd>
            @Html.DisplayFor(Function(model) model.Status)
        </dd>

        <dt>
            @Html.DisplayNameFor(Function(model) model.DisplayName)
        </dt>

        <dd>
            @Html.DisplayFor(Function(model) model.DisplayName)
        </dd>

    </dl>
</div>
<p>
    @Html.ActionLink("Edit", "Edit", New With { .id = Model.ID }) |
    @Html.ActionLink("Back to List", "Index")
</p>
