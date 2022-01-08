Public Class ProductDetail
    Inherits AdminPage
    Private ReadOnly db As New indiabobblesEntities
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack And Not Page.IsCallback Then
            PopulatePhotos()
        End If
    End Sub

    Private Sub PopulatePhotos()
        PhotoGridView.DataSource = db.ProductPhotoes.Where(Function(m) m.ProductID = TargetID).ToList
        PhotoGridView.DataBind()
    End Sub

    'Protected Sub PhotoListView_ItemCommand(sender As Object, e As ListViewCommandEventArgs) Handles PhotoListView.ItemCommand
    '    If e.CommandName = "Remove" Then
    '        db.ProductPhotoes.RemoveRange(db.ProductPhotoes.Where(Function(m) m.ID = Integer.Parse(e.CommandArgument.ToString())))
    '        db.SaveChanges()
    '    End If
    'End Sub

    Protected Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Page.Validate("photogrp")
        If Not Page.IsValid Then
            Return
        End If
        Dim pp As New ProductPhoto()
        pp.ImagePath = PhotoPathTextBox.Text.Trim()
        pp.Sequence = Integer.Parse(SequenceTextBox.Text)
        pp.Product = db.Products.FirstOrDefault(Function(m) m.ID = TargetID)
        db.ProductPhotoes.Add(pp)
        db.SaveChanges()
        PhotoPathTextBox.Text = String.Empty
        SequenceTextBox.Text = String.Empty
        PopulatePhotos()
    End Sub

    Protected Sub PhotoGridView_RowUpdating(sender As Object, e As GridViewUpdateEventArgs) Handles PhotoGridView.RowUpdating
        Dim ImagePathTextBox = CType(PhotoGridView.Rows(e.RowIndex).FindControl("ImagePathTextBox"), TextBox)
        Dim SequenceTextBox = CType(PhotoGridView.Rows(e.RowIndex).FindControl("SequenceTextBox"), TextBox)
        Dim PhotoIDLabel = CType(PhotoGridView.Rows(e.RowIndex).FindControl("PhotoIDLabel"), Label)
        Dim photoid As Integer = Integer.Parse(PhotoIDLabel.Text)
        Dim pp = db.ProductPhotoes.FirstOrDefault(Function(m) m.ID = photoid)
        If pp IsNot Nothing Then
            pp.ImagePath = ImagePathTextBox.Text.Trim()
            pp.Sequence = Integer.Parse(SequenceTextBox.Text.Trim())
            db.SaveChanges()
        End If
        PhotoGridView.EditIndex = -1
        PopulatePhotos()
    End Sub

    Protected Sub PhotoGridView_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles PhotoGridView.RowEditing
        PhotoGridView.EditIndex = e.NewEditIndex
        PopulatePhotos()
    End Sub

    Protected Sub PhotoGridView_RowCancelingEdit(sender As Object, e As GridViewCancelEditEventArgs) Handles PhotoGridView.RowCancelingEdit
        PhotoGridView.EditIndex = -1
        PopulatePhotos()
    End Sub

    Protected Sub PhotoGridView_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles PhotoGridView.RowDeleting
        Dim PhotoIDLabel = CType(PhotoGridView.Rows(e.RowIndex).FindControl("PhotoIDLabel"), Label)
        Dim photoid As Integer = Integer.Parse(PhotoIDLabel.Text)
        Dim pp = db.ProductPhotoes.FirstOrDefault(Function(m) m.ID = photoid)
        If pp IsNot Nothing Then
            db.ProductPhotoes.Remove(pp)
            db.SaveChanges()
        End If
        PopulatePhotos()
    End Sub

    Protected Sub SaveTagButton_Click(sender As Object, e As EventArgs) Handles SaveTagButton.Click
        ProductTagDataSource.Insert()
    End Sub
End Class