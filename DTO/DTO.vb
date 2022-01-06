Imports System.ComponentModel.DataAnnotations

Public Class LoginDTO
    Private _email As String = String.Empty
    <Required>
    <EmailAddress>
    Public Property Email() As String
        Get
            Return _email
        End Get
        Set(ByVal value As String)
            _email = value
        End Set
    End Property

    Private _password As String = String.Empty
    <Required>
    Public Property Password() As String
        Get
            Return _password
        End Get
        Set(ByVal value As String)
            _password = value
        End Set
    End Property
End Class

Public Class RegisterDTO
    <Required>
    <EmailAddress>
    Public Property Email As String = String.Empty
    Public Property Newsletter As Boolean
    <Required>
    <MaxLength(200)>
    Public Property Name As String = String.Empty
    <MaxLength(20)>
    Public Property Mobile As String = String.Empty
End Class

Public Class OTPDTO
    <Required>
    <EmailAddress>
    Public Property Email As String = String.Empty
End Class

Public Class ProfileDTO
    <Required>
    <MaxLength(200)>
    Public Property MemberName As String = String.Empty

    <MaxLength(200)>
    Public Property LastName As String = String.Empty
    Public Property DOB As DateTime
    Public Property Newsletter As Boolean
    <MaxLength(20)>
    Public Property Mobile As String = String.Empty
    <MaxLength(10)>
    Public Property Country As String = String.Empty
    <MaxLength(1)>
    Public Property Gender As String = String.Empty
End Class

Public Class OrderAddressDTO
    Private _email As String
    <Required>
    <EmailAddress>
    <MaxLength(250)>
    Public Property Email() As String
        Get
            Return _email
        End Get
        Set(ByVal value As String)
            _email = value
        End Set
    End Property


    Private _phone As String
    <Required>
    <MaxLength(15)>
    <Display(Name:="Billing Contact Phone")>
    Public Property Phone() As String
        Get
            Return _phone
        End Get
        Set(ByVal value As String)
            _phone = value
        End Set
    End Property

    Private _name As String
    <Required>
    <MaxLength(150)>
    <Display(Name:="Billing Contact Name")>
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Private _billingAddress As String
    <Required>
    <MaxLength(250)>
    <Display(Name:="Billing Address")>
    Public Property BillingAddress() As String
        Get
            Return _billingAddress
        End Get
        Set(ByVal value As String)
            _billingAddress = value
        End Set
    End Property

    Private _billingCity As String
    <Required>
    <MaxLength(150)>
    <Display(Name:="Billing City")>
    Public Property BillingCity() As String
        Get
            Return _billingCity
        End Get
        Set(ByVal value As String)
            _billingCity = value
        End Set
    End Property

    Private _billingState As String
    <Required>
    <MaxLength(50)>
    <Display(Name:="Billing State")>
    Public Property BillingState() As String
        Get
            Return _billingState
        End Get
        Set(ByVal value As String)
            _billingState = value
        End Set
    End Property

    Private _billingCountry As String
    <Required>
    <MaxLength(50)>
    <Display(Name:="Billing Country")>
    Public Property BillingCountry() As String
        Get
            Return _billingCountry
        End Get
        Set(ByVal value As String)
            _billingCountry = value
        End Set
    End Property

    Private _billingPincode As String
    <Required>
    <MaxLength(15)>
    <Display(Name:="Billing PinCode")>
    Public Property BillingPinCode() As String
        Get
            Return _billingPincode
        End Get
        Set(ByVal value As String)
            _billingPincode = value
        End Set
    End Property

    Private _shippingName As String
    <Required>
    <MaxLength(250)>
    <Display(Name:="Shipping Contact Name")>
    Public Property ShippingName() As String
        Get
            Return _shippingName
        End Get
        Set(ByVal value As String)
            _shippingName = value
        End Set
    End Property

    Private _shippingPhone As String
    <Required>
    <MaxLength(15)>
    <Display(Name:="Shipping Contact Phone")>
    Public Property ShippingPhone() As String
        Get
            Return _shippingPhone
        End Get
        Set(ByVal value As String)
            _shippingPhone = value
        End Set
    End Property

    Private _shippingAddress As String
    <Required>
    <MaxLength(300)>
    <Display(Name:="Shipping Address")>
    Public Property ShippingAddress() As String
        Get
            Return _shippingAddress
        End Get
        Set(ByVal value As String)
            _shippingAddress = value
        End Set
    End Property

    Private _shippingCity As String
    <Required>
    <MaxLength(50)>
    <Display(Name:="Shipping City")>
    Public Property ShippingCity() As String
        Get
            Return _shippingCity
        End Get
        Set(ByVal value As String)
            _shippingCity = value
        End Set
    End Property

    Private _shippingState As String
    <Required>
    <MaxLength(50)>
    <Display(Name:="Shipping State")>
    Public Property ShippingState() As String
        Get
            Return _shippingState
        End Get
        Set(ByVal value As String)
            _shippingState = value
        End Set
    End Property

    Private _shippingCountry As String
    <Required>
    <MaxLength(50)>
    <Display(Name:="Shipping Country")>
    Public Property ShippingCountry() As String
        Get
            Return _shippingCountry
        End Get
        Set(ByVal value As String)
            _shippingCountry = value
        End Set
    End Property

    Private _shippingPincode As String
    <Required>
    <MaxLength(10)>
    <Display(Name:="Shipping Pincode")>
    Public Property ShippingPincode() As String
        Get
            Return _shippingPincode
        End Get
        Set(ByVal value As String)
            _shippingPincode = value
        End Set
    End Property

    Public Sub New()

    End Sub

    Public Sub New(o As Order)
        If o IsNot Nothing Then
            Name = o.Name
            Email = o.Email
            Phone = o.Phone
            BillingAddress = o.BillingAddress
            BillingCity = o.BillingCity
            If String.IsNullOrEmpty(o.BillingCountry) Then
                o.BillingCountry = "India"
            End If
            BillingCountry = o.BillingCountry
            BillingState = o.BillingState
            BillingPinCode = o.BillingZip
            ShippingAddress = o.ShippingAddress
            ShippingCity = o.ShippingCity
            If String.IsNullOrEmpty(o.ShippingCountry) Then
                o.ShippingCountry = "India"
            End If
            ShippingCountry = o.ShippingCountry
            ShippingName = o.ShippingFirstName
                ShippingPhone = o.ShippingPhone
                ShippingState = o.ShippingState
                ShippingPincode = o.ShippingZip
            End If
    End Sub
End Class

