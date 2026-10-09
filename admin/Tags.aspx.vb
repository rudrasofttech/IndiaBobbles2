Imports System.Data
Imports System.Data.SqlClient

' Admin › Tags – list with product counts, add/edit in one form, hide/delete/restore.
Public Class Tags
    Inherits AdminPage

    ' Tags the website itself relies on (home page sections etc.)
    Private Shared ReadOnly SystemTags As String() = {"highlight", "trending"}

    Private ReadOnly Property ConnStr As String
        Get
            Return ConfigurationManager.ConnectionStrings("indiabobblesConnectionString").ConnectionString
        End Get
    End Property

    Protected ReadOnly Property EditingId As Integer
        Get
            Dim id As Integer
            Integer.TryParse(EditIdHidden.Value, id)
            Return id
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        UserIDHidden.Value = CurrentUser.ID
        If Not IsPostBack AndAlso Session("TagFlash") IsNot Nothing Then
            Flash(CStr(Session("TagFlash")))
            Session.Remove("TagFlash")
        End If
    End Sub

    Protected Sub Page_PreRender(sender As Object, e As EventArgs) Handles Me.PreRender
        BindList()
        Dim editing As Boolean = EditingId > 0
        FormTitleLiteral.Text = If(editing, "Edit tag", "New tag")
        SaveButton.Text = If(editing, "Save changes", "Create tag")
        CancelButton.Visible = editing
        EditingNote.Visible = editing
    End Sub

    Private Sub BindList()
        Dim dt As New DataTable()
        Using con As New SqlConnection(ConnStr)
            Using da As New SqlDataAdapter(
                "SELECT CT.ID, CT.UrlName, CT.Description, CT.Status, CT.DisplayName, COUNT(PT.ID) AS ProductCount " &
                "FROM CategoryTag AS CT LEFT OUTER JOIN ProductTag AS PT ON CT.ID = PT.TagID " &
                "GROUP BY CT.ID, CT.UrlName, CT.Description, CT.Status, CT.DisplayName " &
                "ORDER BY CASE WHEN CT.Status = 2 THEN 1 ELSE 0 END, CT.DisplayName", con)
                da.Fill(dt)
            End Using
        End Using
        TagRepeater.DataSource = dt
        TagRepeater.DataBind()
    End Sub

    ' ------------------------------------------------------------------
    '  Helpers used in the markup
    ' ------------------------------------------------------------------
    Protected Shared Function StatusText(o As Object) As String
        Select Case Convert.ToString(o)
            Case "0" : Return "Active"
            Case "1" : Return "Inactive"
            Case "2" : Return "Deleted"
            Case Else : Return Convert.ToString(o)
        End Select
    End Function

    Protected Shared Function IsSystemTag(urlName As Object) As Boolean
        Return SystemTags.Contains(Convert.ToString(urlName).Trim().ToLowerInvariant())
    End Function

    Protected Shared Function DeleteConfirm(urlName As Object, count As Object) As String
        Dim msg As String = "Delete this tag?"
        Dim n As Integer = Convert.ToInt32(count)
        If IsSystemTag(urlName) Then
            msg = "This tag is used by the website (e.g. the home page). Delete it anyway?"
        ElseIf n > 0 Then
            msg = "This tag is on " & n & " product(s). Its page will stop working. Delete it?"
        End If
        Return "return confirm('" & msg.Replace("'", "\'") & "');"
    End Function

    ' ------------------------------------------------------------------
    '  Row actions
    ' ------------------------------------------------------------------
    Protected Sub TagRepeater_ItemCommand(source As Object, e As RepeaterCommandEventArgs) Handles TagRepeater.ItemCommand
        Dim id As Integer
        If Not Integer.TryParse(Convert.ToString(e.CommandArgument), id) Then Return

        Select Case e.CommandName
            Case "EditTag"
                LoadIntoForm(id)
            Case "Toggle"
                Exec("UPDATE CategoryTag SET Status = CASE WHEN Status = 0 THEN 1 ELSE 0 END WHERE ID = @ID AND Status IN (0,1)", id)
                Flash("Tag status updated.")
            Case "DeleteTag"
                Exec("UPDATE CategoryTag SET Status = 2 WHERE ID = @ID", id)   ' soft delete, as before
                If EditingId = id Then ResetForm()
                Flash("Tag deleted. You can restore it from the Deleted tab.")
            Case "Restore"
                Exec("UPDATE CategoryTag SET Status = 1 WHERE ID = @ID", id)
                Flash("Tag restored as inactive. Activate it when you're ready.")
        End Select
    End Sub

    Private Sub LoadIntoForm(id As Integer)
        Using con As New SqlConnection(ConnStr), cmd As New SqlCommand("SELECT UrlName, DisplayName, Description, Status FROM CategoryTag WHERE ID = @ID", con)
            cmd.Parameters.AddWithValue("@ID", id)
            con.Open()
            Using r = cmd.ExecuteReader()
                If Not r.Read() Then Return
                EditIdHidden.Value = id.ToString()
                UrlNameTextBox.Text = Convert.ToString(r("UrlName"))
                DisplayNameTextBox.Text = Convert.ToString(r("DisplayName"))
                DescriptionTextBox.Text = Convert.ToString(r("Description")).Trim()
                StatusDropdown.SelectedValue = Convert.ToString(r("Status"))
            End Using
        End Using
    End Sub

    Private Sub ResetForm()
        EditIdHidden.Value = "0"
        UrlNameTextBox.Text = ""
        DisplayNameTextBox.Text = ""
        DescriptionTextBox.Text = ""
        StatusDropdown.SelectedValue = "0"
    End Sub

    Protected Sub CancelButton_Click(sender As Object, e As EventArgs) Handles CancelButton.Click
        ResetForm()
    End Sub

    ' ------------------------------------------------------------------
    '  Save (create or update)
    ' ------------------------------------------------------------------
    Protected Sub SaveButton_Click(sender As Object, e As EventArgs) Handles SaveButton.Click
        Page.Validate("valgrp")
        If Not Page.IsValid Then Return

        Dim id As Integer = EditingId
        Dim urlName As String = MakeSlug(UrlNameTextBox.Text)
        Dim displayName As String = DisplayNameTextBox.Text.Trim()
        Dim description As String = DescriptionTextBox.Text.Trim()
        Dim status As Byte = Byte.Parse(StatusDropdown.SelectedValue)

        If urlName = "" Then
            UrlErrorLabel.Text = "Use letters or numbers in the URL name." : UrlErrorLabel.Visible = True : Return
        End If

        Using con As New SqlConnection(ConnStr)
            con.Open()
            Using chk As New SqlCommand("SELECT COUNT(*) FROM CategoryTag WHERE LOWER(LTRIM(RTRIM(UrlName))) = @Url AND Status <> 2 AND ID <> @ID", con)
                chk.Parameters.AddWithValue("@Url", urlName)
                chk.Parameters.AddWithValue("@ID", id)
                If CInt(chk.ExecuteScalar()) > 0 Then
                    UrlNameTextBox.Text = urlName
                    UrlErrorLabel.Text = "Another tag already uses /tag/" & Server.HtmlEncode(urlName) & "." : UrlErrorLabel.Visible = True : Return
                End If
            End Using

            Dim sql As String = If(id = 0,
                "INSERT INTO CategoryTag (UrlName, Description, ImagePath, CreateDate, CreatedBy, Status, DisplayName) VALUES (@UrlName, @Description, N'', GETDATE(), @CreatedBy, @Status, @DisplayName)",
                "UPDATE CategoryTag SET UrlName = @UrlName, Description = @Description, DisplayName = @DisplayName, Status = @Status WHERE ID = @ID")
            Using cmd As New SqlCommand(sql, con)
                cmd.Parameters.AddWithValue("@UrlName", urlName)
                cmd.Parameters.AddWithValue("@Description", If(description = "", " ", description))   ' old page saved " " when empty
                cmd.Parameters.AddWithValue("@DisplayName", displayName)
                cmd.Parameters.AddWithValue("@Status", status)
                cmd.Parameters.AddWithValue("@CreatedBy", UserIDHidden.Value)
                cmd.Parameters.AddWithValue("@ID", id)
                cmd.ExecuteNonQuery()
            End Using
        End Using

        ' Redirect after saving so a page refresh can't create the tag twice
        Session("TagFlash") = "Tag " & Server.HtmlEncode(displayName) & If(id = 0, " created.", " updated.")
        Response.Redirect("~/admin/tags.aspx", False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub

    ''' <summary>"Bollywood Legends!" → "bollywood-legends"</summary>
    Private Shared Function MakeSlug(s As String) As String
        Dim t As String = If(s, "").Trim().ToLowerInvariant().Replace("&", " and ")
        t = System.Text.RegularExpressions.Regex.Replace(t, "[^a-z0-9]+", "-").Trim("-"c)
        If t.Length > 80 Then t = t.Substring(0, 80).Trim("-"c)
        Return t
    End Function

    ' ------------------------------------------------------------------
    Private Sub Exec(sql As String, id As Integer)
        Using con As New SqlConnection(ConnStr), cmd As New SqlCommand(sql, con)
            cmd.Parameters.AddWithValue("@ID", id)
            con.Open()
            cmd.ExecuteNonQuery()
        End Using
    End Sub

    Private Sub Flash(msg As String)
        MessageLabel.Text = "<i class=""fa fa-check""></i> " & msg
        MessageLabel.Visible = True
    End Sub
End Class