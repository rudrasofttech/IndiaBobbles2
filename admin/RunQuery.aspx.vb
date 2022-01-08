Imports System.Data.SqlClient

Public Class RunQuery
    Inherits System.Web.UI.Page
    Private ReadOnly dc As New indiabobblesEntities
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub SubmitButton_Click(sender As Object, e As EventArgs) Handles SubmitButton.Click
        Try
            Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("indiabobblesConnectionString").ConnectionString)
                conn.Open()
                Using comm As New SqlCommand(QueryTextBox.Text.Trim(), conn)
                    Dim rows = comm.ExecuteNonQuery()
                    Message1.Text = String.Format("Command executed successfully. {0} rows affected.", rows)
                    Message1.Indicate = AlertType.Success
                    Message1.Visible = True
                End Using
            End Using
        Catch ex As Exception
            Message1.Text = String.Format("{0}<br/>{1}<br/>{2}", ex.Message, ex.Source, ex.StackTrace)
            message1.Indicate = AlertType.[Error]
            message1.Visible = True
        End Try
    End Sub
End Class