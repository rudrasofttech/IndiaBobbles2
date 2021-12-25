Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports IndiaBobbles

Public Class ManageOrder
    Inherits AdminPage

    Private om As OrderManager = New OrderManager()
    Private o As Order


    Protected Sub OrderDetailView_DataBound(ByVal sender As Object, ByVal e As EventArgs)
        If OrderDetailView.CurrentMode = DetailsViewMode.[ReadOnly] Then

            If OrderDetailView.DataItem IsNot Nothing Then

                Select Case OrderDetailView.Rows(17).Cells(1).Text
                    Case "1"
                        OrderDetailView.Rows(17).Cells(1).Text = "New"
                    Case "2"
                        OrderDetailView.Rows(17).Cells(1).Text = "Process"
                    Case "3"
                        OrderDetailView.Rows(17).Cells(1).Text = "Paid By Card"
                    Case "4"
                        OrderDetailView.Rows(17).Cells(1).Text = "Paid By COD"
                    Case "5"
                        OrderDetailView.Rows(17).Cells(1).Text = "Shipped"
                    Case "6"
                        OrderDetailView.Rows(17).Cells(1).Text = "Complete"
                    Case "7"
                        OrderDetailView.Rows(17).Cells(1).Text = "Refund"
                End Select
            End If
        End If
    End Sub

    Protected Sub SubmitButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        om.UpdateOrderStatus(o.ID, CType([Enum].Parse(GetType(OrderStatusType), StatusDropDown.SelectedValue), OrderStatusType), ShippingNotesTextBox.Text.Trim())
        om.UpdateOrderShippingService(o.ID, ShippingServiceTextBox.Text.Trim())
        om.UpdateShippingTrackingCode(o.ID, TrackTextBox.Text)
        om.UpdateOrderTransactionDetail(o.ID, DetailTextBox.Text.Trim())
        Response.Redirect("~/admin/manageorder.aspx?orderid=" & o.ID)
    End Sub

    Protected Sub DeleteButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        om.DeleteOrder(o.ID)
        Response.Redirect("~/admin/orders.aspx")
    End Sub

    Private Sub ManageOrder_Load(sender As Object, e As EventArgs) Handles Me.Load
        o = om.GetOrderDetail(Integer.Parse(Request.QueryString("orderid")))

        If Not Page.IsCallback AndAlso Not Page.IsPostBack Then
            ReceiptLink.NavigateUrl = Page.ResolveUrl("~/orders/detail/" & o.ID)
            StatusDropDown.SelectedValue = o.Status.ToString()
            TrackTextBox.Text = o.ShippingTrackCode
            ShippingServiceTextBox.Text = o.ShippingService
            ShippingNotesTextBox.Text = o.ShippingNotes
            DetailTextBox.Text = o.TransactionDetail
        End If
    End Sub
End Class