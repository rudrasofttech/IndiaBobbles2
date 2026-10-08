Imports System.Data.SqlClient
Imports System.Globalization

' Admin › Product detail – read-only overview + quick actions, photos and tags.
' Full editing (text, prices, photo upload) stays in ManageProduct.aspx.
Public Class ProductDetail
    Inherits AdminPage

    Private ReadOnly db As New indiabobblesEntities
    Private Shared ReadOnly INR As New CultureInfo("en-IN")

    Private ReadOnly Property ConnStr As String
        Get
            Return ConfigurationManager.ConnectionStrings("indiabobblesConnectionString").ConnectionString
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If TargetID = 0 Then
            Response.Redirect("~/admin/products.aspx")
            Return
        End If
        If Not IsPostBack Then BindAll()
    End Sub

    ' ------------------------------------------------------------------
    '  Binding
    ' ------------------------------------------------------------------
    Private Function LoadProduct() As Product
        Return db.Products.FirstOrDefault(Function(m) m.ID = TargetID)
    End Function

    Private Sub BindAll()
        Dim p = LoadProduct()
        If p Is Nothing Then
            NotFoundPanel.Visible = True
            DetailPanel.Visible = False
            Return
        End If
        BindHeader(p)
        BindInfo(p)
        BindPhotos(p)
        BindTags()
    End Sub

    Private Sub BindHeader(p As Product)
        Dim enc = Function(s As String) Server.HtmlEncode(If(s, ""))
        NameLiteral.Text = enc(p.Name)
        IDLiteral.Text = p.ID.ToString()
        Page.Title = p.Name & " – Product"

        HeroImage.ImageUrl = If(String.IsNullOrWhiteSpace(p.ThumbPath), "", p.ThumbPath)
        HeroImage.Visible = Not String.IsNullOrWhiteSpace(p.ThumbPath)
        HeroEmpty.Visible = Not HeroImage.Visible

        ' Status + stock badges
        Dim stName As String = If(p.Status = 0, "Active", If(p.Status = 1, "Inactive", "Deleted"))
        BadgesLiteral.Text =
            "<span class=""badge-ib st-" & stName.ToLower() & """>" & stName & "</span> " &
            If(p.OutofStock, "<span class=""badge-ib oos"">Out of stock</span>", "<span class=""badge-ib ins"">In stock</span>")

        ' Price
        Dim html As String = "<b>₹" & p.SalePrice.ToString("#,##0.##", INR) & "</b>"
        If p.MRP > p.SalePrice AndAlso p.MRP > 0 Then
            html &= " <s>₹" & p.MRP.ToString("#,##0.##", INR) & "</s> <span class=""off"">" &
                    Math.Round((p.MRP - p.SalePrice) * 100 / p.MRP) & "% off</span>"
        End If
        PriceLiteral.Text = html

        ' Links
        EditLink.NavigateUrl = "~/admin/manageproduct.aspx?id=" & p.ID
        PhotosEditLink.NavigateUrl = "~/admin/manageproduct.aspx?id=" & p.ID & "#photos"
        ViewLink.NavigateUrl = "~/product/" & p.ID & "/" & IndiaBobbles.Utility.Slugify(p.Name)

        ' Quick actions
        StockButton.Text = If(p.OutofStock, "<i class=""fa fa-check""></i> Mark in stock", "<i class=""fa fa-ban""></i> Mark out of stock")
        StatusButton.Text = If(p.Status = 0, "<i class=""fa fa-eye-slash""></i> Hide from website", "<i class=""fa fa-eye""></i> Make active")
    End Sub

    Private Sub BindInfo(p As Product)
        ' Description is admin-written HTML, shown as on the website
        DescLiteral.Text = If(String.IsNullOrWhiteSpace(p.Description), "<span class=""text-muted"">No description yet.</span>", p.Description)

        Dim rows As New List(Of KeyValuePair(Of String, String)) From {
            New KeyValuePair(Of String, String)("Dimension", p.Dimension),
            New KeyValuePair(Of String, String)("Weight", p.Weight),
            New KeyValuePair(Of String, String)("Material", p.Material),
            New KeyValuePair(Of String, String)("Colour", p.Color),
            New KeyValuePair(Of String, String)("Manufacturer", p.Manufacturer),
            New KeyValuePair(Of String, String)("Country of origin", p.CountryofOrigin),
            New KeyValuePair(Of String, String)("Recommended age", p.RecommendedAge),
            New KeyValuePair(Of String, String)("Ships in", p.ShippingTime),
            New KeyValuePair(Of String, String)("Handmade", If(p.Handmade, "Yes", "No")),
            New KeyValuePair(Of String, String)("Fragile", If(p.Fragile, "Yes", "No")),
            New KeyValuePair(Of String, String)("Care instructions", p.CareInstructions)
        }
        SpecRepeater.DataSource = rows
        SpecRepeater.DataBind()

        ' Dates (ModifyDate may be nullable – boxing handles both cases)
        CreatedLiteral.Text = p.CreateDate.ToString("d MMM yyyy, h:mm tt")
        Dim md As Object = p.ModifyDate
        ModifiedLiteral.Text = If(md Is Nothing, "–", CDate(md).ToString("d MMM yyyy, h:mm tt"))
    End Sub

    Private Sub BindPhotos(p As Product)
        Dim photos = p.ProductPhotoes.OrderBy(Function(x) x.Sequence).ToList()
        PhotoCountLiteral.Text = photos.Count.ToString()
        PhotoRepeater.DataSource = photos.Select(Function(x, i) New With {
            .ID = x.ID, .ImagePath = x.ImagePath, .Sequence = i + 1,
            .IsFirst = (i = 0), .IsLast = (i = photos.Count - 1),
            .IsThumb = String.Equals(x.ImagePath, p.ThumbPath, StringComparison.OrdinalIgnoreCase)}).ToList()
        PhotoRepeater.DataBind()
        NoPhotosPanel.Visible = photos.Count = 0
    End Sub

    Private Sub BindTags()
        TagRepeater.DataBind()          ' ProductTagDataSource
        TagDropDown.Items.Clear()
        TagDropDown.Items.Add(New ListItem("Choose a tag…", ""))
        TagDropDown.DataBind()          ' TagDataSource (only tags not yet assigned)
        NoTagsPanel.Visible = TagRepeater.Items.Count = 0
        TagCountLiteral.Text = TagRepeater.Items.Count.ToString()
    End Sub

    Private Sub Flash(msg As String, Optional isError As Boolean = False)
        MessageLabel.Text = If(isError, "<i class=""fa fa-exclamation-triangle""></i> ", "<i class=""fa fa-check""></i> ") & msg
        MessageLabel.CssClass = If(isError, "adm-error d-block", "adm-flash")
        MessageLabel.Visible = True
    End Sub

    Private Sub Touch(p As Product)
        p.ModifyDate = DateTime.Now
        db.SaveChanges()
    End Sub

    ' ------------------------------------------------------------------
    '  Quick actions
    ' ------------------------------------------------------------------
    Protected Sub StockButton_Click(sender As Object, e As EventArgs) Handles StockButton.Click
        Dim p = LoadProduct()
        If p Is Nothing Then Return
        p.OutofStock = Not p.OutofStock
        Touch(p)
        Flash(If(p.OutofStock, "Marked out of stock.", "Marked in stock."))
        BindAll()
    End Sub

    Protected Sub StatusButton_Click(sender As Object, e As EventArgs) Handles StatusButton.Click
        Dim p = LoadProduct()
        If p Is Nothing Then Return
        p.Status = If(p.Status = 0, CByte(1), CByte(0))
        Touch(p)
        Flash(If(p.Status = 0, "Product is now active on the website.", "Product is now hidden from the website."))
        BindAll()
    End Sub

    ' ------------------------------------------------------------------
    '  Photos: add by path, reorder, set thumbnail, remove
    ' ------------------------------------------------------------------
    Protected Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Page.Validate("photogrp")
        If Not Page.IsValid Then Return
        Dim p = LoadProduct()
        If p Is Nothing Then Return

        Dim path As String = PhotoPathTextBox.Text.Trim()
        If p.ProductPhotoes.Any(Function(x) String.Equals(x.ImagePath, path, StringComparison.OrdinalIgnoreCase)) Then
            Flash("That photo is already in the gallery.", True)
            Return
        End If

        Dim nextSeq As Integer = If(p.ProductPhotoes.Any(), p.ProductPhotoes.Max(Function(x) x.Sequence) + 1, 1)
        p.ProductPhotoes.Add(New ProductPhoto With {.ImagePath = path, .Sequence = nextSeq})
        If String.IsNullOrWhiteSpace(p.ThumbPath) Then p.ThumbPath = path
        Touch(p)

        PhotoPathTextBox.Text = ""
        Flash("Photo added.")
        BindAll()
    End Sub

    Protected Sub PhotoRepeater_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles PhotoRepeater.ItemCommand
        Dim p = LoadProduct()
        If p Is Nothing Then Return
        Dim photoId As Integer
        If Not Integer.TryParse(Convert.ToString(e.CommandArgument), photoId) Then Return

        Dim list = p.ProductPhotoes.OrderBy(Function(x) x.Sequence).ToList()
        Dim idx As Integer = list.FindIndex(Function(x) x.ID = photoId)
        If idx < 0 Then Return
        Dim photo = list(idx)

        Select Case e.CommandName
            Case "Left"
                If idx > 0 Then list.RemoveAt(idx) : list.Insert(idx - 1, photo)
            Case "Right"
                If idx < list.Count - 1 Then list.RemoveAt(idx) : list.Insert(idx + 1, photo)
            Case "First"
                list.RemoveAt(idx) : list.Insert(0, photo)
            Case "Thumb"
                p.ThumbPath = photo.ImagePath
            Case "Remove"
                list.RemoveAt(idx)
                If String.Equals(p.ThumbPath, photo.ImagePath, StringComparison.OrdinalIgnoreCase) Then
                    p.ThumbPath = If(list.Count > 0, list(0).ImagePath, "")
                End If
                db.ProductPhotoes.Remove(photo)   ' file stays in Drive
            Case Else
                Return
        End Select

        ' Re-number 1..n so sequences never have gaps or duplicates
        For i As Integer = 0 To list.Count - 1
            list(i).Sequence = i + 1
        Next
        Touch(p)

        Flash(PhotoMessage(e.CommandName))
        BindAll()
    End Sub

    Private Shared Function PhotoMessage(cmd As String) As String
        Select Case cmd
            Case "Thumb" : Return "Thumbnail updated."
            Case "Remove" : Return "Photo removed from the gallery (the file is still in Drive)."
            Case Else : Return "Photo order updated."
        End Select
    End Function

    ' ------------------------------------------------------------------
    '  Tags
    ' ------------------------------------------------------------------
    Protected Sub SaveTagButton_Click(sender As Object, e As EventArgs) Handles SaveTagButton.Click
        Dim tagId As Integer
        If Not Integer.TryParse(TagDropDown.SelectedValue, tagId) Then
            Flash("Choose a tag first.", True)
            Return
        End If

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand(
                "IF NOT EXISTS (SELECT 1 FROM ProductTag WHERE ProductID = @p AND TagID = @t) " &
                "INSERT INTO ProductTag (ProductID, TagID) VALUES (@p, @t)", con)
                cmd.Parameters.AddWithValue("@p", TargetID)
                cmd.Parameters.AddWithValue("@t", tagId)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        Flash("Tag added.")
        BindTags()
    End Sub

    Protected Sub TagRepeater_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles TagRepeater.ItemCommand
        If e.CommandName <> "RemoveTag" Then Return
        Dim tagId As Integer
        If Not Integer.TryParse(Convert.ToString(e.CommandArgument), tagId) Then Return

        Using con As New SqlConnection(ConnStr)
            Using cmd As New SqlCommand("DELETE FROM ProductTag WHERE ProductID = @p AND TagID = @t", con)
                cmd.Parameters.AddWithValue("@p", TargetID)
                cmd.Parameters.AddWithValue("@t", tagId)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        Flash("Tag removed.")
        BindTags()
    End Sub

End Class