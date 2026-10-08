Public Class ManageCoupons
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If ForbidUserAccess(MemberTypeType.Admin) Then
            Response.Redirect("default.aspx")
        End If
    End Sub

    Protected Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Page.Validate("CouponGrp")
        If Not Page.IsValid Then Return

        CouponDataSource.Insert()
        Response.Redirect("~/admin/managecoupons.aspx")
    End Sub
End Class