Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports IndiaBobbles
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Data
Public Class Orders
    Inherits AdminPage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        If e.Row.RowIndex > -1 Then
            Select Case e.Row.Cells(3).Text
                Case "1"
                    e.Row.Cells(3).Text = "New"
                    e.Row.Cells(3).ForeColor = System.Drawing.Color.Red
                Case "2"
                    e.Row.Cells(3).Text = "Process"
                    e.Row.Cells(3).ForeColor = System.Drawing.Color.Green
                Case "3"
                    e.Row.Cells(3).Text = "Card by Paid"
                Case "4"
                    e.Row.Cells(3).Text = "Cash on Delivery"
                Case "5"
                    e.Row.Cells(3).Text = "Shipped"
                Case "6"
                    e.Row.Cells(3).Text = "Complete"
                Case "7"
                    e.Row.Cells(3).Text = "Refund"
                Case "8"
                    e.Row.Cells(3).Text = "Deleted"
            End Select
        End If
    End Sub

    Protected Sub GridView1_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
        If e.CommandName = "Remove" Then
            Dim om As OrderManager = New OrderManager()
            om.DeleteOrder(Convert.ToInt32(e.CommandArgument.ToString()))
            GridView1.DataBind()
        End If

    End Sub

End Class