Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Text
Imports IndiaBobbles

' Admin › Orders – stats, search, status filters, CSV export, soft delete.
Public Class Orders
    Inherits AdminPage

    Private Shared ReadOnly INR As New CultureInfo("en-IN")

    Private ReadOnly Property ConnStr As String
        Get
            Return ConfigurationManager.ConnectionStrings("indiabobblesConnectionString").ConnectionString
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Allow links like orders.aspx?status=ship or orders.aspx?q=84117
        If Not IsPostBack Then
            Dim st As String = Request.QueryString("status")
            If Not String.IsNullOrEmpty(st) AndAlso StatusDropDown.Items.FindByValue(st) IsNot Nothing Then StatusDropDown.SelectedValue = st
            If Not String.IsNullOrEmpty(Request.QueryString("q")) Then SearchTextBox.Text = Request.QueryString("q").Trim()
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        LoadStats()
    End Sub

    ' ------------------------------------------------------------------
    '  Stats cards
    ' ------------------------------------------------------------------
    Private Sub LoadStats()
        Dim monthStart As New DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)
        Const sql As String =
            "SELECT " &
            " SUM(CASE WHEN Status IN (3,4) THEN 1 ELSE 0 END) AS ToShip," &
            " SUM(CASE WHEN Status = 5 THEN 1 ELSE 0 END) AS Shipped," &
            " SUM(CASE WHEN Status = 1 AND DateCreated >= DATEADD(day,-30,GETDATE()) AND (ISNULL(Email,'') <> '' OR ISNULL(Phone,'') <> '') THEN 1 ELSE 0 END) AS Leads," &
            " ISNULL(SUM(CASE WHEN Status IN (3,4,5,6) AND DateCreated >= @m THEN Total ELSE 0 END),0) AS MonthSales," &
            " SUM(CASE WHEN Status IN (3,4,5,6) AND DateCreated >= @m THEN 1 ELSE 0 END) AS MonthOrders " &
            "FROM [Order]"
        Try
            Using con As New SqlConnection(ConnStr), cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@m", monthStart)
                con.Open()
                Using r = cmd.ExecuteReader()
                    If r.Read() Then
                        ToShipLiteral.Text = ToInt(r("ToShip")).ToString()
                        ShippedLiteral.Text = ToInt(r("Shipped")).ToString()
                        LeadLiteral.Text = ToInt(r("Leads")).ToString()
                        MonthSalesLiteral.Text = Money(r("MonthSales"))
                        Dim n As Integer = ToInt(r("MonthOrders"))
                        MonthLabelLiteral.Text = "Sales in " & monthStart.ToString("MMMM") & " · " & n & If(n = 1, " order", " orders")
                    End If
                End Using
            End Using
        Catch ex As Exception
            ' Stats are a convenience – never break the page because of them
        End Try
    End Sub

    Private Shared Function ToInt(o As Object) As Integer
        If o Is Nothing OrElse IsDBNull(o) Then Return 0
        Return Convert.ToInt32(o)
    End Function

    ' ------------------------------------------------------------------
    '  Helpers used by the grid markup
    ' ------------------------------------------------------------------
    Protected Shared Function StatusText(o As Object) As String
        Select Case Convert.ToString(o)
            Case "1" : Return "New / unpaid"
            Case "2" : Return "Processing"
            Case "3" : Return "Card paid"
            Case "4" : Return "Cash on delivery"
            Case "5" : Return "Shipped"
            Case "6" : Return "Complete"
            Case "7" : Return "Refund"
            Case "8" : Return "Deleted"
            Case Else : Return Convert.ToString(o)
        End Select
    End Function

    Protected Shared Function StatusCss(o As Object) As String
        Select Case Convert.ToString(o)
            Case "1" : Return "os-new"
            Case "2" : Return "os-proc"
            Case "3", "4" : Return "os-paid"
            Case "5" : Return "os-ship"
            Case "6" : Return "os-done"
            Case "7" : Return "os-ref"
            Case "8" : Return "os-del"
            Case Else : Return "os-new"
        End Select
    End Function

    Protected Shared Function Money(o As Object) As String
        If o Is Nothing OrElse IsDBNull(o) Then Return "₹0"
        Return "₹" & Convert.ToDecimal(o).ToString("#,##0.##", INR)
    End Function

    ''' <summary>Phone as a call link plus a WhatsApp link (Indian numbers get +91).</summary>
    Protected Shared Function PhoneHtml(phone As Object, orderId As Object) As String
        Dim p As String = Convert.ToString(phone).Trim()
        If p = "" Then Return ""
        Dim digits As String = New String(p.Where(AddressOf Char.IsDigit).ToArray())
        If digits.Length = 11 AndAlso digits.StartsWith("0") Then digits = digits.Substring(1)
        If digits.Length = 10 Then digits = "91" & digits

        Dim html As New StringBuilder("<div class=""cline"">")
        html.AppendFormat("<a href=""tel:+{0}""><i class=""fa fa-phone""></i> {1}</a>", digits, HttpUtility.HtmlEncode(p))
        If digits.Length >= 11 Then
            Dim msg As String = "Hi, this is India Bobbles about your order #" & Convert.ToString(orderId) & "."
            html.AppendFormat(" · <a class=""wa"" target=""_blank"" rel=""noopener"" href=""https://wa.me/{0}?text={1}"" title=""Message on WhatsApp""><i class=""fa fa-whatsapp""></i> WhatsApp</a>",
                              digits, HttpUtility.UrlEncode(msg))
        End If
        html.Append("</div>")
        Return html.ToString()
    End Function

    ' ------------------------------------------------------------------
    '  Grid events
    ' ------------------------------------------------------------------
    Protected Sub GridView1_RowDataBound(sender As Object, e As GridViewRowEventArgs) Handles GridView1.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then Return
        Dim row = CType(e.Row.DataItem, DataRowView)
        Dim st As String = Convert.ToString(row("Status"))
        If st = "3" OrElse st = "4" Then
            e.Row.CssClass = "r-ship"           ' paid, needs shipping – yellow edge
        ElseIf st = "1" AndAlso String.IsNullOrWhiteSpace(Convert.ToString(row("Email"))) AndAlso String.IsNullOrWhiteSpace(Convert.ToString(row("Phone"))) Then
            e.Row.CssClass = "r-cart"           ' empty cart – faded
        End If
    End Sub

    Protected Sub GridView1_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView1.RowCommand
        If e.CommandName <> "Remove" Then Return
        Dim id As Integer
        If Not Integer.TryParse(Convert.ToString(e.CommandArgument), id) Then Return

        ' Same delete logic as before
        Dim om As New OrderManager()
        om.DeleteOrder(id)
        MessageLabel.Text = "<i class=""fa fa-check""></i> Order #" & id & " removed."
        MessageLabel.Visible = True
        GridView1.DataBind()
    End Sub

    Protected Sub OrderDataSource_Selected(sender As Object, e As SqlDataSourceStatusEventArgs) Handles OrderDataSource.Selected
        If e.Exception Is Nothing Then
            CountLiteral.Text = e.AffectedRows.ToString("#,##0") & If(e.AffectedRows = 1, " order", " orders") & " in this view"
        End If
    End Sub

    ' ------------------------------------------------------------------
    '  Toolbar
    ' ------------------------------------------------------------------
    Protected Sub SearchButton_Click(sender As Object, e As EventArgs) Handles SearchButton.Click
        SearchTextBox.Text = SearchTextBox.Text.Trim()
        GridView1.PageIndex = 0
        GridView1.DataBind()
    End Sub

    Protected Sub StatusDropDown_SelectedIndexChanged(sender As Object, e As EventArgs) Handles StatusDropDown.SelectedIndexChanged
        GridView1.PageIndex = 0
    End Sub

    Protected Sub HideCartsCheckBox_CheckedChanged(sender As Object, e As EventArgs) Handles HideCartsCheckBox.CheckedChanged
        GridView1.PageIndex = 0
    End Sub

    Protected Sub ClearButton_Click(sender As Object, e As EventArgs) Handles ClearButton.Click
        SearchTextBox.Text = ""
        StatusDropDown.SelectedValue = "0"
        HideCartsCheckBox.Checked = True
        GridView1.PageIndex = 0
        GridView1.DataBind()
    End Sub

    ''' <summary>Downloads every order in the current view (all pages) as CSV for Excel.</summary>
    Protected Sub ExportButton_Click(sender As Object, e As EventArgs) Handles ExportButton.Click
        Dim view = CType(OrderDataSource.Select(DataSourceSelectArguments.Empty), DataView)
        Dim csv As New StringBuilder()
        csv.AppendLine("Order ID,Date,Status,Name,Email,Phone,Ship to,Items,Total,Coupon,Track code,Transaction code")
        For Each r As DataRowView In view
            csv.AppendLine(String.Join(",", {
                Q(r("ID")), Q(Convert.ToDateTime(r("DateCreated")).ToString("yyyy-MM-dd HH:mm")), Q(StatusText(r("Status"))),
                Q(r("Name")), Q(r("Email")), Q(r("Phone")), Q(r("Shipping")), Q(r("ItemCount")),
                Q(If(IsDBNull(r("Total")), "0", Convert.ToDecimal(r("Total")).ToString("0.00", CultureInfo.InvariantCulture))),
                Q(r("Coupon")), Q(r("ShippingTrackCode")), Q(r("TransactionCode"))}))
        Next

        Response.Clear()
        Response.ContentType = "text/csv"
        Response.ContentEncoding = Encoding.UTF8
        Response.AddHeader("Content-Disposition", "attachment; filename=orders-" & DateTime.Now.ToString("yyyyMMdd-HHmm") & ".csv")
        Response.BinaryWrite(Encoding.UTF8.GetPreamble())   ' so Excel shows ₹ and Hindi names correctly
        Response.Write(csv.ToString())
        Response.Flush()
        Response.SuppressContent = True
        HttpContext.Current.ApplicationInstance.CompleteRequest()
    End Sub

    ''' <summary>CSV-safe value (quotes, and blocks Excel formula injection).</summary>
    Private Shared Function Q(o As Object) As String
        Dim s As String = If(o Is Nothing OrElse IsDBNull(o), "", Convert.ToString(o))
        If s.Length > 0 AndAlso "=+-@".IndexOf(s(0)) >= 0 Then s = "'" & s
        Return """" & s.Replace("""", """""") & """"
    End Function

End Class