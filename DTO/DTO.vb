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
    Public Property ShippingPincode() As String
        Get
            Return _shippingPincode
        End Get
        Set(ByVal value As String)
            _shippingPincode = value
        End Set
    End Property
End Class
