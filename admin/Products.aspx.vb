Imports System.Data.SqlClient

' Products admin page – adds the one-click "Mark in stock / out of stock" toggle.
' If your existing Products.aspx.vb already has code, keep it and just add the
' ProductsGridView_RowCommand handler below.
Public Class Products
    Inherits System.Web.UI.Page

    Protected Sub ProductsGridView_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles ProductsGridView.RowCommand
        If e.CommandName <> "ToggleStock" Then Return   ' let paging/sorting work as usual

        Dim id As Integer
        If Not Integer.TryParse(Convert.ToString(e.CommandArgument), id) Then Return

        Dim cs As String = ConfigurationManager.ConnectionStrings("indiabobblesConnectionString").ConnectionString
        Using con As New SqlConnection(cs)
            Using cmd As New SqlCommand("UPDATE Product SET OutofStock = CASE WHEN OutofStock = 1 THEN 0 ELSE 1 END, ModifyDate = GETDATE() WHERE ID = @id", con)
                cmd.Parameters.AddWithValue("@id", id)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        MessageLabel.Text = "<i class=""fa fa-check""></i> Stock status updated."
        MessageLabel.Visible = True
        ProductsGridView.DataBind()
    End Sub

End Class