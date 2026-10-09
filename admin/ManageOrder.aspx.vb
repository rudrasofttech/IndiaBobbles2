Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Data.SqlClient
Imports System.Globalization
Imports IndiaBobbles

Public Class ManageOrder
    Inherits AdminPage

    Private om As OrderManager = New OrderManager()
    Private o As Order
    Private Shared ReadOnly INR As New CultureInfo("en-IN")

    ' ------------------------------------------------------------------
    '  Values used by the markup
    ' ------------------------------------------------------------------
    Protected ReadOnly Property Ord As Order
        Get
            Return o
        End Get
    End Property

    Protected ReadOnly Property StatusNum As Integer
        Get
            Return If(o Is Nothing, 0, Convert.ToInt32(o.Status))
        End Get
    End Property

    Protected Property ItemQty As Integer
    Protected Property WaNumber As String = ""
    Protected Property FirstName As String = ""
    Protected Property ShipLines As String = ""
    Protected Property BillLines As String = ""

    Private Sub ManageOrder_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim id As Integer
        If Integer.TryParse(Request.QueryString("orderid"), id) Then
            o = om.GetOrderDetail(id)
        End If
        If o Is Nothing Then
            NotFoundPanel.Visible = True
            OrderPanel.Visible = False
            Return
        End If

        Page.Title = "Order #" & o.ID
        PrepareDisplay()

        If Not Page.IsCallback AndAlso Not Page.IsPostBack Then
            ReceiptLink.NavigateUrl = Page.ResolveUrl("~/orders/detail/" & o.ID)
            Dim st As String = StatusNum.ToString()
            If StatusDropDown.Items.FindByValue(st) IsNot Nothing Then StatusDropDown.SelectedValue = st
            TrackTextBox.Text = o.ShippingTrackCode
            ShippingServiceTextBox.Text = If(String.IsNullOrWhiteSpace(o.ShippingService), "Delhivery", o.ShippingService)
            ShippingNotesTextBox.Text = o.ShippingNotes
            DetailTextBox.Text = o.TransactionDetail

            If Session("OrderFlash") IsNot Nothing Then
                MessageLabel.Text = "<i class=""fa fa-check""></i> " & CStr(Session("OrderFlash"))
                MessageLabel.Visible = True
                Session.Remove("OrderFlash")
            End If
        End If
    End Sub

    Private Sub PrepareDisplay()
        ' Item count
        Try
            Using con As New SqlConnection(ConfigurationManager.ConnectionStrings("indiabobblesConnectionString").ConnectionString),
                  cmd As New SqlCommand("SELECT ISNULL(SUM(Quantity),0) FROM OrderItem WHERE OrderID = @id", con)
                cmd.Parameters.AddWithValue("@id", o.ID)
                con.Open()
                ItemQty = Convert.ToInt32(cmd.ExecuteScalar())
            End Using
        Catch ex As Exception
            ItemQty = 0
        End Try

        ' Addresses (HTML-encoded, one line each)
        ShipLines = AddressHtml(o.ShippingAddress, o.ShippingCity, o.ShippingState, o.ShippingZip, o.ShippingCountry)
        BillLines = AddressHtml(o.BillingAddress, o.BillingCity, o.BillingState, o.BillingZip, o.BillingCountry)

        ' WhatsApp / phone number in international format (Indian numbers get 91)
        Dim digits As String = New String(Convert.ToString(o.Phone).Where(AddressOf Char.IsDigit).ToArray())
        If digits.Length = 11 AndAlso digits.StartsWith("0") Then digits = digits.Substring(1)
        If digits.Length = 10 Then digits = "91" & digits
        WaNumber = If(digits.Length >= 11, digits, "")

        Dim nm As String = Convert.ToString(o.Name).Trim()
        FirstName = If(nm = "", "there", nm.Split(" "c)(0))
    End Sub

    Private Function AddressHtml(addr As String, city As String, state As String, zip As String, country As String) As String
        Dim enc = Function(s As String) HttpUtility.HtmlEncode(If(s, "").Trim())
        Dim cityLine As String = String.Join(", ", {enc(city), enc(state)}.Where(Function(x) x <> ""))
        If enc(zip) <> "" Then cityLine = (cityLine & " " & enc(zip)).Trim()
        Dim lines = {enc(addr), cityLine, enc(country)}.Where(Function(x) x <> "").ToList()
        ' A country on its own is not an address
        If lines.Count = 0 OrElse (lines.Count = 1 AndAlso lines(0) = enc(country)) Then Return ""
        Return String.Join("<br />", lines)
    End Function

    Protected Function StatusName(n As Integer) As String
        Select Case n
            Case 1 : Return "New / unpaid"
            Case 2 : Return "Processing"
            Case 3 : Return "Card paid"
            Case 4 : Return "Cash on delivery"
            Case 5 : Return "Shipped"
            Case 6 : Return "Complete"
            Case 7 : Return "Refund"
            Case 8 : Return "Deleted"
            Case Else : Return n.ToString()
        End Select
    End Function

    ''' <summary>Progress step: 1 Placed, 2 Paid, 3 Shipped, 4 Complete.</summary>
    Protected Function StepCss(stepNo As Integer) As String
        Dim reached As Integer
        Select Case StatusNum
            Case 1, 2 : reached = 1
            Case 3, 4 : reached = 2
            Case 5 : reached = 3
            Case 6 : reached = 4
            Case Else : reached = 1
        End Select
        If stepNo < reached Then Return "done"
        If stepNo = reached Then Return "done now"
        Return ""
    End Function

    Protected Shared Function N(v As Object) As Decimal
        If v Is Nothing OrElse IsDBNull(v) Then Return 0D
        Dim d As Decimal
        Decimal.TryParse(Convert.ToString(v, CultureInfo.InvariantCulture), NumberStyles.Any, CultureInfo.InvariantCulture, d)
        Return d
    End Function

    Protected Shared Function M(v As Object) As String
        Return "₹" & N(v).ToString("#,##0.##", INR)
    End Function

    Protected Shared Function D(v As Object) As String
        If v Is Nothing OrElse IsDBNull(v) Then Return "–"
        Dim dt As DateTime = Convert.ToDateTime(v)
        If dt = DateTime.MinValue Then Return "–"
        Return dt.ToString("d MMM yyyy, h:mm tt")
    End Function

    Protected Shared Function OrDash(v As Object) As String
        Dim s As String = Convert.ToString(v)
        Return If(String.IsNullOrWhiteSpace(s), "<span class=""mo-muted"">–</span>", HttpUtility.HtmlEncode(s))
    End Function

    ' ------------------------------------------------------------------
    '  Actions (same OrderManager calls as before)
    ' ------------------------------------------------------------------
    Protected Sub SubmitButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        om.UpdateOrderStatus(o.ID, CType([Enum].Parse(GetType(OrderStatusType), StatusDropDown.SelectedValue), OrderStatusType), ShippingNotesTextBox.Text.Trim())
        om.UpdateOrderShippingService(o.ID, ShippingServiceTextBox.Text.Trim())
        om.UpdateShippingTrackingCode(o.ID, TrackTextBox.Text.Trim())
        om.UpdateOrderTransactionDetail(o.ID, DetailTextBox.Text.Trim())
        Session("OrderFlash") = "Order updated."
        Response.Redirect("~/admin/manageorder.aspx?orderid=" & o.ID)
    End Sub

    Protected Sub DeleteButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        om.DeleteOrder(o.ID)
        Response.Redirect("~/admin/orders.aspx")
    End Sub
End Class