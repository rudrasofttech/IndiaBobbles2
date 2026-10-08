Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Mail
Imports System.Web.UI
Imports IndiaBobbles

Public Class BulkEmail
    Inherits AdminPage

    Private ReadOnly dc As New indiabobblesEntities()

    Private Class RecipientInfo
        Public Property Email As String
        Public Property Name As String
    End Class

    Private Class OrderRecipientView
        Public Property Name As String
        Public Property Email As String
        Public Property Products As String
    End Class

    Private Sub BulkEmail_Load(sender As Object, e As EventArgs) Handles Me.Load
        If ForbidUserAccess(MemberTypeType.Admin) Then
            Response.Redirect("default.aspx")
        End If

        If Not Page.IsPostBack AndAlso Not Page.IsCallback Then
            BindMembers()
            BindOrders(Nothing)
        End If
    End Sub

    Private Sub BindMembers()
        Try
            Dim members = dc.Members.
                Where(Function(m) Not String.IsNullOrEmpty(m.Email)).
                Select(Function(m) New With {
                    .MemberName = If(String.IsNullOrEmpty(m.MemberName), "(No Name)", m.MemberName),
                    .Email = m.Email
                }).
                ToList()

            Dim distinctMembers = members.
                GroupBy(Function(m) m.Email.Trim().ToLower()).
                Select(Function(g) g.First()).
                OrderBy(Function(m) m.MemberName).
                ThenBy(Function(m) m.Email).
                ToList()

            MemberRepeater.DataSource = distinctMembers
            MemberRepeater.DataBind()
            MemberCountLiteral.Text = distinctMembers.Count.ToString()
        Catch ex As Exception
            Trace.Write("Unable to fetch members for bulk email.")
            Trace.Write(ex.Message)
            Trace.Write(ex.StackTrace)
            MemberCountLiteral.Text = "0"
        End Try
    End Sub

    Private Sub BindOrders(productFilters As List(Of String))
        Try
            Dim orders = dc.Orders.
                Where(Function(o) Not String.IsNullOrEmpty(o.Email)).
                Select(Function(o) New With {
                    .ID = o.ID,
                    .Name = If(String.IsNullOrEmpty(o.Name), "(No Name)", o.Name),
                    .Email = o.Email
                }).
                ToList()

            Dim orderIds = orders.Select(Function(o) o.ID).ToList()

            Dim items = dc.OrderItems.
                Where(Function(i) orderIds.Contains(i.OrderID)).
                Select(Function(i) New With {
                    .OrderID = i.OrderID,
                    .ProductName = i.ProductName,
                    .ProductCode = i.ProductCode
                }).
                ToList()

            Dim itemLookup = items.
                GroupBy(Function(i) i.OrderID).
                ToDictionary(
                    Function(g) g.Key,
                    Function(g) g.Select(Function(x) (If(x.ProductName, "") & " " & If(x.ProductCode, "")).Trim()).
                                  Where(Function(x) x <> "").
                                  Distinct().
                                  ToList()
                )

            Dim expanded = orders.Select(
                Function(o)
                    Dim p As New List(Of String)()
                    If itemLookup.ContainsKey(o.ID) Then
                        p = itemLookup(o.ID)
                    End If

                    Return New OrderRecipientView With {
                        .Name = o.Name,
                        .Email = o.Email,
                        .Products = String.Join(", ", p)
                    }
                End Function).ToList()

            If productFilters IsNot Nothing AndAlso productFilters.Count > 0 Then
                expanded = expanded.
                    Where(Function(x) productFilters.Any(Function(f) x.Products.ToLower().Contains(f))).
                    ToList()
            End If

            Dim distinctOrders = expanded.
                GroupBy(Function(x) x.Email.Trim().ToLower()).
                Select(Function(g)
                           Dim first = g.First()
                           Dim allProducts = g.
                               Select(Function(z) z.Products).
                               Where(Function(z) Not String.IsNullOrEmpty(z)).
                               ToList()
                           Dim mergedProducts = String.Join(", ", allProducts).
                               Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries).
                               Select(Function(s) s.Trim()).
                               Where(Function(s) s <> "").
                               Distinct().
                               ToList()

                           Return New OrderRecipientView With {
                               .Name = first.Name,
                               .Email = first.Email,
                               .Products = String.Join(", ", mergedProducts)
                           }
                       End Function).
                OrderBy(Function(x) x.Name).
                ThenBy(Function(x) x.Email).
                ToList()

            OrderRepeater.DataSource = distinctOrders
            OrderRepeater.DataBind()
            OrderCountLiteral.Text = distinctOrders.Count.ToString()
        Catch ex As Exception
            Trace.Write("Unable to fetch order emails for bulk email.")
            Trace.Write(ex.Message)
            Trace.Write(ex.StackTrace)
            OrderCountLiteral.Text = "0"
        End Try
    End Sub

    Protected Sub ApplyOrderFilterButton_Click(sender As Object, e As EventArgs)
        BindOrders(GetProductFilters())
        ShowOrderModal()
    End Sub

    Protected Sub ClearOrderFilterButton_Click(sender As Object, e As EventArgs)
        OrderProductFilterTextBox.Text = ""
        BindOrders(Nothing)
        ShowOrderModal()
    End Sub

    Private Function GetProductFilters() As List(Of String)
        Return OrderProductFilterTextBox.Text.
            Replace(vbCrLf, ",").
            Replace(vbLf, ",").
            Split(","c).
            Select(Function(x) x.Trim().ToLower()).
            Where(Function(x) x <> "").
            Distinct().
            ToList()
    End Function

    Private Sub ShowOrderModal()
        ScriptManager.RegisterStartupScript(
            Me,
            Me.GetType(),
            "showOrderModal",
            "var el=document.getElementById('orderPickerModal'); if(el){ bootstrap.Modal.getOrCreateInstance(el).show(); }",
            True)
    End Sub

    Protected Sub SubmitButton_Click(ByVal sender As Object, ByVal e As EventArgs)
        Page.Validate("BulkMailGrp")
        If Not Page.IsValid Then Return

        Try
            Dim invalidRecipients As List(Of String) = Nothing
            Dim recipients As List(Of RecipientInfo) = ParseRecipients(RecipientsTextBox.Text, invalidRecipients)

            If recipients.Count = 0 Then
                ShowStatus("alert alert-danger", "No valid recipient found.")
                Return
            End If

            If invalidRecipients.Count > 0 Then
                ShowStatus("alert alert-danger", "Invalid recipient(s): " & String.Join(", ", invalidRecipients.Distinct(StringComparer.OrdinalIgnoreCase)))
                Return
            End If

            Dim em As New EmailManager()
            Dim sentCount As Integer = 0

            For Each recipient As RecipientInfo In recipients
                Dim receiverName As String = If(String.IsNullOrWhiteSpace(recipient.Name), recipient.Email, recipient.Name)

                Dim personalizedMessage As String = MessageTextBox.Text.Trim()
                personalizedMessage = personalizedMessage.Replace("{{name}}", receiverName)
                personalizedMessage = personalizedMessage.Replace("{{email}}", recipient.Email)

                Dim personalizedSubject As String = SubjectTextBox.Text.Trim()
                personalizedSubject = personalizedSubject.Replace("{{name}}", receiverName)
                personalizedSubject = personalizedSubject.Replace("{{email}}", recipient.Email)

                If em.SendMail(Utility.NewsletterEmail,
                               recipient.Email,
                               Utility.AdminName,
                               receiverName,
                               personalizedMessage,
                               personalizedSubject,
                               EmailMessageType.Communication,
                               EmailGroupTextBox.Text.Trim()) Then
                    sentCount += 1
                End If
            Next

            ShowStatus("alert alert-success", String.Format("Email sent to {0} of {1} recipients.", sentCount, recipients.Count))
        Catch ex As Exception
            Trace.Write("Unable to send bulk email.")
            Trace.Write(ex.Message)
            Trace.Write(ex.StackTrace)
            ShowStatus("alert alert-danger", "Unable to send email. Please check logs.")
        End Try
    End Sub

    Private Function ParseRecipients(rawRecipients As String, ByRef invalidRecipients As List(Of String)) As List(Of RecipientInfo)
        invalidRecipients = New List(Of String)()
        Dim recipientMap As New Dictionary(Of String, RecipientInfo)(StringComparer.OrdinalIgnoreCase)

        Dim normalized As String = rawRecipients.Replace(vbCrLf, ",").Replace(vbLf, ",").Replace(";", ",")

        For Each token As String In normalized.Split(","c)
            Dim raw As String = token.Trim()
            If String.IsNullOrEmpty(raw) Then Continue For

            Try
                Dim addr As New MailAddress(raw)
                Dim email As String = addr.Address
                Dim displayName As String = addr.DisplayName

                If String.IsNullOrWhiteSpace(displayName) Then
                    displayName = email
                End If

                If recipientMap.ContainsKey(email) Then
                    If recipientMap(email).Name = recipientMap(email).Email AndAlso displayName <> email Then
                        recipientMap(email).Name = displayName
                    End If
                Else
                    recipientMap.Add(email, New RecipientInfo With {.Email = email, .Name = displayName})
                End If
            Catch
                invalidRecipients.Add(raw)
            End Try
        Next

        Return recipientMap.Values.ToList()
    End Function

    Private Sub ShowStatus(cssClass As String, message As String)
        StatusPanel.Visible = True
        StatusPanel.CssClass = cssClass
        StatusLiteral.Text = message
    End Sub
End Class