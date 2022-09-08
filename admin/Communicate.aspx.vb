Public Class Communicate
    Inherits AdminPage
    Dim emaildict As New Dictionary(Of String, Tuple(Of String, String))
    Dim dc As New indiabobblesEntities

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub SubmitButton_Click(sender As Object, e As EventArgs) Handles SubmitButton.Click
        If OrderEmailsChk.Checked Then
            Dim orders = dc.Orders.Where(Function(m) Not String.IsNullOrEmpty(m.Email)).ToList()
            For Each o As Order In orders
                If Not emaildict.ContainsKey(o.Email.ToLower()) Then
                    emaildict.Add(o.Email.ToLower(), New Tuple(Of String, String)(o.Email.ToLower(), o.Name))
                End If
            Next
        End If

        If RegisterMemberChk.Checked Then
            Dim members = dc.Members.Where(Function(m) m.Newsletter).ToList()
            For Each m As Member In members
                If Not emaildict.ContainsKey(m.Email.ToLower()) Then
                    emaildict.Add(m.Email.ToLower(), New Tuple(Of String, String)(m.Email.ToLower(), m.MemberName))
                End If
            Next
        End If

        For Each obj In emaildict
            Dim eman As New EmailManager()
            eman.SendMail(Utility.NewsletterEmail, obj.Value.Item1, Utility.AdminName,
                          obj.Value.Item2, EmailTextBox.Text.Trim(),
                          SubjectTextBox.Text, EmailMessageType.Communication, EmailGroupTextBox.Text.Trim())
        Next
    End Sub

    Protected Sub EmailTextBox_TextChanged(sender As Object, e As EventArgs)
        Dim emessage As String = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailWrapper.html"))
        emessage = emessage.Replace("[root]", Utility.SiteURL)
        emessage = emessage.Replace("[newsletteremail]", Utility.NewsletterEmail)
        emessage = emessage.Replace("[message]", EmailTextBox.Text.Trim())
        emessage = emessage.Replace("[id]", Guid.Empty.ToString())
        'emessage = emessage.Replace("[toaddress]", "")
        'emessage = emessage.Replace("[toname]", recieverName)
        emessage = emessage.Replace("[sitename]", Utility.SiteName)
        emessage = emessage.Replace("[sitetitle]", Utility.SiteTitle)
        emessage = emessage.Replace("[address]", Utility.Address)
        emessage = emessage.Replace("[emailsignature]", Utility.GetSiteSetting("EmailSignature"))
        PreviewLiteral.Text = emessage
    End Sub
End Class