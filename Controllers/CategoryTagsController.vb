Imports System
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.Entity
Imports System.Linq
Imports System.Threading.Tasks
Imports System.Net
Imports System.Web
Imports System.Web.Mvc

Namespace Controllers
    Public Class CategoryTagsController
        Inherits System.Web.Mvc.Controller

        Private ReadOnly db As New indiabobblesEntities

        ' GET: CategoryTags
        Async Function Index() As Task(Of ActionResult)
            Return View(Await db.CategoryTags.ToListAsync())
        End Function

        ' GET: CategoryTags/Details/5
        Async Function Details(ByVal id As Integer?) As Task(Of ActionResult)
            If IsNothing(id) Then
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
            Dim categoryTag As CategoryTag = Await db.CategoryTags.FindAsync(id)
            If IsNothing(categoryTag) Then
                Return HttpNotFound()
            End If
            Return View(categoryTag)
        End Function

        ' GET: CategoryTags/Create
        Function Create() As ActionResult
            Return View()
        End Function

        ' POST: CategoryTags/Create
        'To protect from overposting attacks, enable the specific properties you want to bind to, for 
        'more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Async Function Create(<Bind(Include:="ID,UrlName,Description,ImagePath,CreateDate,CreatedBy,ModifiedBy,ModifyDate,Status,DisplayName")> ByVal categoryTag As CategoryTag) As Task(Of ActionResult)
            If ModelState.IsValid Then
                db.CategoryTags.Add(categoryTag)
                Await db.SaveChangesAsync()
                Return RedirectToAction("Index")
            End If
            Return View(categoryTag)
        End Function

        ' GET: CategoryTags/Edit/5
        Async Function Edit(ByVal id As Integer?) As Task(Of ActionResult)
            If IsNothing(id) Then
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
            Dim categoryTag As CategoryTag = Await db.CategoryTags.FindAsync(id)
            If IsNothing(categoryTag) Then
                Return HttpNotFound()
            End If
            Return View(categoryTag)
        End Function

        ' POST: CategoryTags/Edit/5
        'To protect from overposting attacks, enable the specific properties you want to bind to, for 
        'more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        <HttpPost()>
        <ValidateAntiForgeryToken()>
        Async Function Edit(<Bind(Include:="ID,UrlName,Description,ImagePath,CreateDate,CreatedBy,ModifiedBy,ModifyDate,Status,DisplayName")> ByVal categoryTag As CategoryTag) As Task(Of ActionResult)
            If ModelState.IsValid Then
                db.Entry(categoryTag).State = EntityState.Modified
                Await db.SaveChangesAsync()
                Return RedirectToAction("Index")
            End If
            Return View(categoryTag)
        End Function

        ' GET: CategoryTags/Delete/5
        Async Function Delete(ByVal id As Integer?) As Task(Of ActionResult)
            If IsNothing(id) Then
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
            Dim categoryTag As CategoryTag = Await db.CategoryTags.FindAsync(id)
            If IsNothing(categoryTag) Then
                Return HttpNotFound()
            End If
            Return View(categoryTag)
        End Function

        ' POST: CategoryTags/Delete/5
        <HttpPost()>
        <ActionName("Delete")>
        <ValidateAntiForgeryToken()>
        Async Function DeleteConfirmed(ByVal id As Integer) As Task(Of ActionResult)
            Dim categoryTag As CategoryTag = Await db.CategoryTags.FindAsync(id)
            db.CategoryTags.Remove(categoryTag)
            Await db.SaveChangesAsync()
            Return RedirectToAction("Index")
        End Function

        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If (disposing) Then
                db.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub
    End Class
End Namespace
