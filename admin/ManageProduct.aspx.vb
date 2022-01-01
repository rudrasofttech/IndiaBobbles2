Public Class ManageProduct
    Inherits AdminPage
    Private ReadOnly db As New indiabobblesEntities
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack And Not Page.IsCallback Then
            LoadData()
        End If
    End Sub

    Private Sub LoadData()
        If TargetID <> 0 Then
            HeadingLiteral.Text = "Edit Product"
            Dim product = db.Products.FirstOrDefault(Function(m) m.ID = TargetID)
            If product IsNot Nothing Then
                NameTextBox.Text = product.Name
                URLTextBox.Text = product.URL
                DescTextBox.Text = product.Description
                MRPTextBox.Text = product.MRP
                SaleTextBox.Text = product.SalePrice
                DimensionTextBox.Text = product.Dimension
                ColorTextBox.Text = product.Color
                WeightTextBox.Text = product.Weight
                MaterialTextBox.Text = product.Material
                ManufacturerTextBox.Text = product.Manufacturer
                CareTextBox.Text = product.CareInstructions
                RecommendAgeTextBox.Text = product.RecommendedAge
                CountryOriginTextBox.Text = product.CountryofOrigin
                FragileCheckBox.Checked = product.Fragile
                HandmadeCheckBox.Checked = product.Handmade
                OutofStockCheckBox.Checked = product.OutofStock
                ShippingTimeTextBox.Text = product.ShippingTime

            End If
        End If
    End Sub

    Protected Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Page.Validate()
        If Not Page.IsValid Then
            Return
        End If
        Dim product As New Product()
        If TargetID <> 0 Then
            product = db.Products.FirstOrDefault(Function(m) m.ID = TargetID)
        End If
        product.Name = NameTextBox.Text.Trim()
        product.URL = URLTextBox.Text.Trim()
        product.Description = DescTextBox.Text.Trim()
        product.MRP = Decimal.Parse(MRPTextBox.Text.Trim())
        product.SalePrice = Decimal.Parse(SaleTextBox.Text.Trim())
        product.Dimension = DimensionTextBox.Text.Trim()
        product.Color = ColorTextBox.Text.Trim()
        product.Weight = WeightTextBox.Text.Trim()
        product.Material = MaterialTextBox.Text.Trim()
        product.Manufacturer = ManufacturerTextBox.Text.Trim()
        product.CareInstructions = CareTextBox.Text.Trim()
        product.RecommendedAge = RecommendAgeTextBox.Text.Trim()
        product.CountryofOrigin = CountryOriginTextBox.Text.Trim()
        product.Fragile = FragileCheckBox.Checked
        product.Handmade = HandmadeCheckBox.Checked
        product.OutofStock = OutofStockCheckBox.Checked
        product.ShippingTime = ShippingTimeTextBox.Text.Trim()

        If TargetID = 0 Then
            db.Products.Add(product)
        End If
        db.SaveChanges()
        Response.Redirect("~/admin/productdetail.aspx?id=" & product.ID)
    End Sub
End Class