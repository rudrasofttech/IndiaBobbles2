Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net.Mail
Imports System.Text.RegularExpressions
Imports System.Web.Script.Serialization
Imports System.Web.UI
Imports IndiaBobbles

Public Class BulkEmail
    Inherits AdminPage

    Private ReadOnly dc As New indiabobblesEntities()

    Private Class RecipientInfo
        Public Property Email As String
        Public Property Name As String
        Public Property Data As Dictionary(Of String, String)
    End Class

    Private Class MemberRecipientView
        Public Property MemberName As String
        Public Property Email As String
        Public Property DOB As String
        Public Property Mobile As String
        Public Property Country As String
        Public Property Createdate As String
    End Class

    Private Class OrderRecipientView
        Public Property Name As String
        Public Property Email As String
        Public Property Products As String
        Public Property OrderCount As Integer
        Public Property OrderDate As String
        Public Property OrderTotal As String
        Public Property LifetimeOrderTotal As String
    End Class

    Private Function NormalizeEmail(ByVal email As String) As String
        Return If(email, String.Empty).Trim().ToLowerInvariant()
    End Function

    Private Function SafeDateString(ByVal dt As Nullable(Of Date)) As String
        If dt.HasValue Then
            Return dt.Value.ToString("dd MMM yyyy")
        End If
        Return String.Empty
    End Function

    Private Function GetUnsubscribedEmailSet() As HashSet(Of String)
        Try
            Dim emails = dc.UnsubscribedEmails.
                Where(Function(u) u.Email IsNot Nothing AndAlso u.Email <> "").
                Select(Function(u) u.Email).
                ToList().
                Select(Function(e) NormalizeEmail(e)).
                Where(Function(e) e <> "").
                Distinct().
                ToList()

            Return New HashSet(Of String)(emails, StringComparer.OrdinalIgnoreCase)
        Catch ex As Exception
            Trace.Write("Unable to fetch unsubscribed email list.")
            Trace.Write(ex.Message)
            Trace.Write(ex.StackTrace)
            Return New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        End Try
    End Function

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
            Dim unsubscribed = GetUnsubscribedEmailSet()

            Dim membersRaw = dc.Members.
                Where(Function(m) Not String.IsNullOrEmpty(m.Email)).
                Select(Function(m) New With {
                    .MemberName = m.MemberName,
                    .Email = m.Email,
                    .DOB = m.DOB,
                    .Mobile = m.Mobile,
                    .Country = m.Country,
                    .Createdate = m.Createdate
                }).
                ToList()

            Dim members = membersRaw.Select(
                Function(m) New MemberRecipientView With {
                    .MemberName = If(String.IsNullOrEmpty(m.MemberName), "(No Name)", m.MemberName),
                    .Email = If(m.Email, String.Empty),
                    .DOB = SafeDateString(m.DOB),
                    .Mobile = If(m.Mobile, String.Empty),
                    .Country = If(m.Country, String.Empty),
                    .Createdate = m.Createdate.ToString("dd MMM yyyy")
                }).
                ToList()

            Dim distinctMembers = members.
                Where(Function(m) Not unsubscribed.Contains(NormalizeEmail(m.Email))).
                GroupBy(Function(m) NormalizeEmail(m.Email)).
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
            Dim unsubscribed = GetUnsubscribedEmailSet()

            Dim orders = dc.Orders.
                Where(Function(o) Not String.IsNullOrEmpty(o.Email)).
                Select(Function(o) New With {
                    .ID = o.ID,
                    .Name = o.Name,
                    .Email = o.Email,
                    .DateCreated = o.DateCreated,
                    .Total = o.Total
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

                    Return New With {
                        .OrderID = o.ID,
                        .Name = If(String.IsNullOrEmpty(o.Name), "(No Name)", o.Name),
                        .Email = If(o.Email, String.Empty),
                        .Products = String.Join(", ", p),
                        .DateCreated = o.DateCreated,
                        .Total = o.Total
                    }
                End Function).ToList()

            If productFilters IsNot Nothing AndAlso productFilters.Count > 0 Then
                expanded = expanded.
                    Where(Function(x) productFilters.Any(Function(f) x.Products.ToLower().Contains(f))).
                    ToList()
            End If

            Dim distinctOrders = expanded.
                Where(Function(x) Not unsubscribed.Contains(NormalizeEmail(x.Email))).
                GroupBy(Function(x) NormalizeEmail(x.Email)).
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

                           Dim latestOrder = g.OrderByDescending(Function(x) x.DateCreated).First()
                           Dim lifetimeTotal = g.Sum(Function(x) x.Total)

                           Return New OrderRecipientView With {
                               .Name = first.Name,
                               .Email = first.Email,
                               .Products = String.Join(", ", mergedProducts),
                               .OrderCount = g.Count(),
                               .OrderDate = latestOrder.DateCreated.ToString("dd MMM yyyy"),
                               .OrderTotal = latestOrder.Total.ToString("0.##"),
                               .LifetimeOrderTotal = lifetimeTotal.ToString("0.##")
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

    Private Function GetValueCaseInsensitive(ByVal dict As Dictionary(Of String, String), ByVal key As String) As String
        If dict Is Nothing Then Return String.Empty
        Dim value As String = Nothing
        If dict.TryGetValue(key, value) Then
            Return If(value, String.Empty)
        End If
        Return String.Empty
    End Function

    Private Function ReplaceTemplateTokens(ByVal templateText As String, ByVal tokenData As Dictionary(Of String, String)) As String
        If String.IsNullOrEmpty(templateText) Then Return String.Empty
        If tokenData Is Nothing Then tokenData = New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        Return Regex.Replace(
            templateText,
            "\{\{\s*([A-Za-z0-9_]+)\s*\}\}",
            Function(m)
                Dim key = m.Groups(1).Value
                Dim value As String = Nothing
                If tokenData.TryGetValue(key, value) Then
                    Return If(value, String.Empty)
                End If
                Return String.Empty
            End Function,
            RegexOptions.IgnoreCase)
    End Function

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

            Dim unsubscribed = GetUnsubscribedEmailSet()
            Dim unsubscribedCount = recipients.Where(Function(r) unsubscribed.Contains(NormalizeEmail(r.Email))).Count()
            recipients = recipients.Where(Function(r) Not unsubscribed.Contains(NormalizeEmail(r.Email))).ToList()

            If recipients.Count = 0 Then
                ShowStatus("alert alert-danger", "All recipients are unsubscribed. No email sent.")
                Return
            End If

            Dim em As New EmailManager()
            Dim sentCount As Integer = 0

            For Each recipient As RecipientInfo In recipients
                Dim receiverName As String = If(String.IsNullOrWhiteSpace(recipient.Name), recipient.Email, recipient.Name)

                Dim tokenData As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
                If recipient.Data IsNot Nothing Then
                    For Each kv In recipient.Data
                        tokenData(kv.Key) = If(kv.Value, String.Empty)
                    Next
                End If

                tokenData("Email") = recipient.Email
                tokenData("Name") = receiverName
                If String.IsNullOrWhiteSpace(GetValueCaseInsensitive(tokenData, "MemberName")) Then
                    tokenData("MemberName") = receiverName
                End If

                Dim personalizedMessage As String = ReplaceTemplateTokens(MessageTextBox.Text.Trim(), tokenData)
                Dim personalizedSubject As String = ReplaceTemplateTokens(SubjectTextBox.Text.Trim(), tokenData)

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

            Dim message = String.Format("Email sent to {0} of {1} eligible recipients.", sentCount, recipients.Count)
            If unsubscribedCount > 0 Then
                message &= String.Format(" Skipped {0} unsubscribed recipient(s).", unsubscribedCount)
            End If

            ShowStatus("alert alert-success", message)
        Catch ex As Exception
            Trace.Write("Unable to send bulk email.")
            Trace.Write(ex.Message)
            Trace.Write(ex.StackTrace)
            ShowStatus("alert alert-danger", "Unable to send email. Please check logs.")
        End Try
    End Sub

    Private Function ParseRecipientFromJsonObject(ByVal obj As Object, ByVal rawLabel As String, ByRef invalidRecipients As List(Of String)) As RecipientInfo
        Try
            Dim dictionaryObj = TryCast(obj, Dictionary(Of String, Object))
            If dictionaryObj Is Nothing Then
                invalidRecipients.Add(rawLabel)
                Return Nothing
            End If

            Dim data As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            For Each kv In dictionaryObj
                data(kv.Key) = If(kv.Value, String.Empty).ToString()
            Next

            Dim email As String = NormalizeEmail(GetValueCaseInsensitive(data, "Email"))
            If String.IsNullOrEmpty(email) Then
                invalidRecipients.Add(rawLabel)
                Return Nothing
            End If

            Dim name As String = GetValueCaseInsensitive(data, "Name")
            If String.IsNullOrWhiteSpace(name) Then
                name = GetValueCaseInsensitive(data, "MemberName")
            End If
            If String.IsNullOrWhiteSpace(name) Then
                name = email
            End If

            Return New RecipientInfo With {
                .Email = email,
                .Name = name,
                .Data = data
            }
        Catch
            invalidRecipients.Add(rawLabel)
            Return Nothing
        End Try
    End Function

    Private Function ParseRecipients(rawRecipients As String, ByRef invalidRecipients As List(Of String)) As List(Of RecipientInfo)
        invalidRecipients = New List(Of String)()
        Dim recipientMap As New Dictionary(Of String, RecipientInfo)(StringComparer.OrdinalIgnoreCase)
        Dim serializer As New JavaScriptSerializer()

        Dim rawText As String = If(rawRecipients, String.Empty).Trim()
        If rawText = "" Then
            Return New List(Of RecipientInfo)()
        End If

        ' Mode 1: Full JSON array
        If rawText.StartsWith("[") AndAlso rawText.EndsWith("]") Then
            Try
                Dim arrObj = TryCast(serializer.DeserializeObject(rawText), Object())
                If arrObj IsNot Nothing Then
                    For i As Integer = 0 To arrObj.Length - 1
                        Dim info = ParseRecipientFromJsonObject(arrObj(i), "JSON item #" & (i + 1).ToString(), invalidRecipients)
                        If info IsNot Nothing Then
                            If recipientMap.ContainsKey(info.Email) Then
                                recipientMap(info.Email) = info
                            Else
                                recipientMap.Add(info.Email, info)
                            End If
                        End If
                    Next
                End If

                Return recipientMap.Values.ToList()
            Catch
                invalidRecipients.Add("Invalid JSON array")
                Return recipientMap.Values.ToList()
            End Try
        End If

        ' Mode 2: JSON line by line OR legacy "Name <email>"
        Dim lines = rawText.Replace(vbCrLf, vbLf).Split(New String() {vbLf}, StringSplitOptions.RemoveEmptyEntries).
            Select(Function(x) x.Trim()).
            Where(Function(x) x <> "").
            ToList()

        For Each line In lines
            If line.StartsWith("{") AndAlso line.EndsWith("}") Then
                Try
                    Dim obj = serializer.DeserializeObject(line)
                    Dim info = ParseRecipientFromJsonObject(obj, line, invalidRecipients)
                    If info IsNot Nothing Then
                        If recipientMap.ContainsKey(info.Email) Then
                            recipientMap(info.Email) = info
                        Else
                            recipientMap.Add(info.Email, info)
                        End If
                    End If
                Catch
                    invalidRecipients.Add(line)
                End Try
            Else
                ' Legacy support
                Try
                    Dim addr As New MailAddress(line)
                    Dim email As String = NormalizeEmail(addr.Address)
                    Dim displayName As String = addr.DisplayName

                    If String.IsNullOrWhiteSpace(displayName) Then
                        displayName = email
                    End If

                    Dim data As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase) From {
                        {"Email", email},
                        {"Name", displayName},
                        {"MemberName", displayName}
                    }

                    Dim info As New RecipientInfo With {
                        .Email = email,
                        .Name = displayName,
                        .Data = data
                    }

                    If recipientMap.ContainsKey(email) Then
                        recipientMap(email) = info
                    Else
                        recipientMap.Add(email, info)
                    End If
                Catch
                    invalidRecipients.Add(line)
                End Try
            End If
        Next

        Return recipientMap.Values.ToList()
    End Function

    Private Sub ShowStatus(cssClass As String, message As String)
        StatusPanel.Visible = True
        StatusPanel.CssClass = cssClass
        StatusLiteral.Text = message
    End Sub
End Class