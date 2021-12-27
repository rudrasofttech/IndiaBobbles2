Imports System.Net.Mail
Imports System.Threading.Tasks
Imports SendGrid
Imports SendGrid.Helpers.Mail
Public Enum EmailMessageType
    Activation = 1
    Unsubscribe = 2
    Newsletter = 3
    ChangePassword = 4
    Reminder = 5
    Communication = 6
End Enum

Public Class EmailManager

    Private ReadOnly dc As New indiabobblesEntities
    Public Sub New()
    End Sub

    Public Async Function SendMailAsync(ByVal fromAddress As String, ByVal toAddress As String, ByVal senderName As String, ByVal recieverName As String, ByVal body As String, ByVal subject As String, ByVal messageType As EmailMessageType, ByVal emailGroup As String) As Task(Of Boolean)
        Return Await SendMailAsync(fromAddress, toAddress, senderName, recieverName, body, subject, String.Empty, messageType, emailGroup)
    End Function

    Public Async Function SendMailAsync(ByVal em As EmailMessage) As Task(Of Boolean)
        Try
            'Dim mail As MailMessage = New MailMessage()
            'mail.[To].Add(New MailAddress(em.ToAddress, em.ToName))

            'If em.CCAdress.Trim() <> String.Empty Then
            '    mail.CC.Add(em.CCAdress)
            'End If

            'mail.From = New MailAddress(em.FromAddress, em.FromName)
            'mail.Subject = em.Subject
            'mail.Body = em.Message
            'mail.IsBodyHtml = True
            'Dim client As System.Net.Mail.SmtpClient = New SmtpClient()
            'client.Send(mail)
            Dim apiKey = "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"
            Dim client = New SendGridClient(apiKey)
            Dim from = New EmailAddress(em.FromAddress, em.FromName)
            Dim subject = em.Subject
            Dim [to] = New EmailAddress(em.ToAddress, em.ToName)
            Dim plainTextContent = em.Message
            Dim htmlContent = em.Message
            Dim msg = MailHelper.CreateSingleEmail(from, [to], subject, plainTextContent, htmlContent)
            Dim response = Await client.SendEmailAsync(msg)

            Try
                If response.StatusCode = Net.HttpStatusCode.Accepted Then
                    em.IsSent = True
                    em.SentDate = DateTime.UtcNow
                Else
                    em.IsSent = False
                End If
                em.LastAttempt = DateTime.UtcNow
                UpdateMessage(em)
            Catch
            End Try

            Return True
        Catch ex As Exception
            Try
                em.LastAttempt = DateTime.Now
                em.IsSent = False
                UpdateMessage(em)
            Catch
            End Try

            HttpContext.Current.Trace.Write("Unable to send email.")
            HttpContext.Current.Trace.Write(ex.Message)
            HttpContext.Current.Trace.Write(ex.StackTrace)
            Return False
        End Try
    End Function

    Public Function SendMail(ByVal em As EmailMessage, ByVal attachments As List(Of String)) As Boolean
        Try
            Dim mail As MailMessage = New MailMessage()
            mail.[To].Add(New MailAddress(em.ToAddress, em.ToName))

            If em.CCAdress.Trim() <> String.Empty Then
                mail.CC.Add(em.CCAdress)
            End If

            mail.From = New MailAddress(em.FromAddress, em.FromName)
            mail.Subject = em.Subject
            mail.Body = em.Message
            mail.IsBodyHtml = True

            'For Each path As String In attachments
            '    mail.Attachments.Add(New Attachment(HttpContext.Current.Server.MapPath(path)))
            'Next

            Dim client As System.Net.Mail.SmtpClient = New SmtpClient()
            client.Send(mail)

            Try
                em.LastAttempt = DateTime.Now
                em.IsSent = True
                em.SentDate = DateTime.Now
                UpdateMessage(em)
            Catch
            End Try

            Return True
        Catch ex As Exception

            Try
                em.LastAttempt = DateTime.Now
                em.IsSent = False
                UpdateMessage(em)
            Catch
            End Try

            HttpContext.Current.Trace.Write("Unable to send email.")
            HttpContext.Current.Trace.Write(ex.Message)
            HttpContext.Current.Trace.Write(ex.StackTrace)
            Return False
        End Try
    End Function

    Public Async Function SendMailAsync(ByVal fromAddress As String, ByVal toAddress As String, ByVal senderName As String, ByVal recieverName As String, ByVal body As String, ByVal subject As String, ByVal ccaddresses As String, ByVal messageType As EmailMessageType, ByVal emailGroup As String) As Task(Of Boolean)
        Try
            Dim em As EmailMessage = New EmailMessage()
            em.ID = Guid.NewGuid()
            Dim emessage As String = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailWrapper.html"))
            emessage = emessage.Replace("[root]", Utility.SiteURL)
            emessage = emessage.Replace("[newsletteremail]", Utility.NewsletterEmail)
            emessage = emessage.Replace("[message]", body)
            emessage = emessage.Replace("[id]", em.ID.ToString())
            emessage = emessage.Replace("[toaddress]", toAddress)
            emessage = emessage.Replace("[sitename]", Utility.SiteName)
            emessage = emessage.Replace("[sitetitle]", Utility.SiteTitle)
            emessage = emessage.Replace("[address]", Utility.Address)
            emessage = emessage.Replace("[emailsignature]", Utility.GetSiteSetting("EmailSignature"))
            em.Message = emessage
            em = AddMessage(em.ID, toAddress, fromAddress, subject, emessage, messageType, emailGroup, ccaddresses, recieverName, senderName)

            If em IsNot Nothing Then
                Return Await SendMailAsync(em)
            Else
                Return False
            End If

        Catch ex As Exception
            HttpContext.Current.Trace.Write("Unable to send email.")
            HttpContext.Current.Trace.Write(ex.Message)
            HttpContext.Current.Trace.Write(ex.StackTrace)
            Return False
        End Try
    End Function

    Public Function UpdateMessage(ByVal em As EmailMessage) As Boolean
        Try

            Dim item As EmailMessage = dc.EmailMessages.FirstOrDefault(Function(u) u.ID = em.ID)
            item.CreateDate = em.CreateDate
            item.EmailGroup = em.EmailGroup
            item.EmailType = em.EmailType
            item.FromAddress = em.FromAddress
            item.IsRead = em.IsRead
            item.IsSent = em.IsSent
            item.Message = em.Message
            item.SentDate = em.SentDate
            item.Subject = em.Subject
            item.ToAddress = em.ToAddress
            item.ReadDate = em.ReadDate
            dc.SaveChanges()
            Return True
        Catch ex As Exception
            HttpContext.Current.Trace.Write("Unable to save EmailMessage object to database.")
            HttpContext.Current.Trace.Write(ex.Message)
            HttpContext.Current.Trace.Write(ex.StackTrace)
            Return False
        End Try
    End Function

    Public Function GetMessage(ByVal id As Guid) As EmailMessage
        Try
            Dim em As EmailMessage = dc.EmailMessages.FirstOrDefault(Function(u) u.ID = id)
            Return em
        Catch ex As Exception
            HttpContext.Current.Trace.Write("Unable to get email message database.")
            HttpContext.Current.Trace.Write(ex.Message)
            HttpContext.Current.Trace.Write(ex.StackTrace)
            Return Nothing
        End Try
    End Function

    Public Function GetUnsentMessage() As EmailMessage
        Try
            Dim em As EmailMessage = dc.EmailMessages.Where(Function(u) u.IsSent = False AndAlso u.ToAddress <> "").OrderBy(Function(u) u.LastAttempt).FirstOrDefault()
            Return em
        Catch ex As Exception
            HttpContext.Current.Trace.Write("Unable to get email message database.")
            HttpContext.Current.Trace.Write(ex.Message)
            HttpContext.Current.Trace.Write(ex.StackTrace)
            Return Nothing
        End Try
    End Function

    Public Function AddMessage(ByVal id As Guid, ByVal toaddress As String, ByVal fromaddress As String, ByVal subject As String, ByVal body As String, ByVal messagetype As EmailMessageType, ByVal emailGroup As String, ByVal ccaddress As String, ByVal toname As String, ByVal fromname As String) As EmailMessage
        Try


            Dim em As EmailMessage = New EmailMessage() With {
                    .ID = id,
                    .Message = body,
                    .FromAddress = fromaddress,
                    .EmailType = CByte(messagetype),
                    .Subject = subject,
                    .ToAddress = toaddress,
                    .SentDate = DateTime.Now,
                    .CreateDate = DateTime.Now,
                    .IsRead = False,
                    .IsSent = False,
                    .EmailGroup = emailGroup,
                    .CCAdress = ccaddress,
                    .ToName = toname,
                    .FromName = fromname,
                    .LastAttempt = DateTime.Now
                }
            dc.EmailMessages.Add(em)
            dc.SaveChanges()
            Return em

        Catch ex As Exception
            HttpContext.Current.Trace.Write("Unable to save EmailMessage object to database.")
            HttpContext.Current.Trace.Write(ex.Message)
            HttpContext.Current.Trace.Write(ex.StackTrace)
            Return Nothing
        End Try
    End Function
End Class
