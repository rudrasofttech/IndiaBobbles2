Imports System.Web.Mvc


Namespace Controllers
    Public Class AccountController
        Inherits Controller

        Private ReadOnly db As New indiabobblesEntities

        ' GET: Account/Login
        Function Login() As ActionResult
            Return View(New LoginDTO())
        End Function

        ' POST: Account/Login
        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Function Login(ByVal dto As LoginDTO) As ActionResult
            If Not ModelState.IsValid Then
                ViewBag("error") = "Invalid input"
            End If

            Try
                ' TODO: Add insert logic here

                Return RedirectToAction("Index")
            Catch
                Return View()
            End Try
        End Function

        ' GET: Account/Edit/5
        Function Edit(ByVal id As Integer) As ActionResult
            Return View()
        End Function

        ' POST: Account/Edit/5
        <HttpPost()>
        Function Edit(ByVal id As Integer, ByVal collection As FormCollection) As ActionResult
            Try
                ' TODO: Add update logic here

                Return RedirectToAction("Index")
            Catch
                Return View()
            End Try
        End Function

        ' GET: Account/Delete/5
        Function Delete(ByVal id As Integer) As ActionResult
            Return View()
        End Function

        ' POST: Account/Delete/5
        <HttpPost()>
        Function Delete(ByVal id As Integer, ByVal collection As FormCollection) As ActionResult
            Try
                ' TODO: Add delete logic here

                Return RedirectToAction("Index")
            Catch
                Return View()
            End Try
        End Function
    End Class
End Namespace