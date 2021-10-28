@ModelType IEnumerable(Of IndiaBobbles2.CategoryTag)
@Code
ViewData("Title") = "Index"
End Code

<h2>Index</h2>

<p>
    @Html.ActionLink("Create New", "Create")
</p>
<table class="table">
    <tr>
        <th>
            @Html.DisplayNameFor(Function(model) model.UrlName)
        </th>
        <th>
            @Html.DisplayNameFor(Function(model) model.Description)
        </th>
        <th>
            @Html.DisplayNameFor(Function(model) model.ImagePath)
        </th>
        <th>
            @Html.DisplayNameFor(Function(model) model.CreateDate)
        </th>
        <th>
            @Html.DisplayNameFor(Function(model) model.CreatedBy)
        </th>
        <th>
            @Html.DisplayNameFor(Function(model) model.ModifiedBy)
        </th>
        <th>
            @Html.DisplayNameFor(Function(model) model.ModifyDate)
        </th>
        <th>
            @Html.DisplayNameFor(Function(model) model.Status)
        </th>
        <th>
            @Html.DisplayNameFor(Function(model) model.DisplayName)
        </th>
        <th></th>
    </tr>

@For Each item In Model
    @<tr>
        <td>
            @Html.DisplayFor(Function(modelItem) item.UrlName)
        </td>
        <td>
            @Html.DisplayFor(Function(modelItem) item.Description)
        </td>
        <td>
            @Html.DisplayFor(Function(modelItem) item.ImagePath)
        </td>
        <td>
            @Html.DisplayFor(Function(modelItem) item.CreateDate)
        </td>
        <td>
            @Html.DisplayFor(Function(modelItem) item.CreatedBy)
        </td>
        <td>
            @Html.DisplayFor(Function(modelItem) item.ModifiedBy)
        </td>
        <td>
            @Html.DisplayFor(Function(modelItem) item.ModifyDate)
        </td>
        <td>
            @Html.DisplayFor(Function(modelItem) item.Status)
        </td>
        <td>
            @Html.DisplayFor(Function(modelItem) item.DisplayName)
        </td>
        <td>
            @Html.ActionLink("Edit", "Edit", New With {.id = item.ID }) |
            @Html.ActionLink("Details", "Details", New With {.id = item.ID }) |
            @Html.ActionLink("Delete", "Delete", New With {.id = item.ID })
        </td>
    </tr>
Next

</table>
