'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated from a template.
'
'     Manual changes to this file may cause unexpected behavior in your application.
'     Manual changes to this file will be overwritten if the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic

Partial Public Class UserRole
    Public Property ID As Integer
    Public Property Name As String

    Public Overridable Property UserInRoles As ICollection(Of UserInRole) = New HashSet(Of UserInRole)

End Class