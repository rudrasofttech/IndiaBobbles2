Public Class Tags
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        UserIDHidden.Value = CurrentUser.ID
    End Sub

    Protected Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        SqlDataSource1.Insert()
        Response.Redirect("~/admin/tags.aspx")
    End Sub
End Class