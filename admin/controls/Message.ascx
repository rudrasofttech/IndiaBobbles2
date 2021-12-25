<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Message.ascx.vb" Inherits="IndiaBobbles.Message" %>

<div class="alert alert-dismissible fade show <%: Block %>">
    <%if not HideClose Then %>
    <a class="close" data-dismiss="alert" href="#">×</a>
    <%End If %>
    <h4 class="alert-heading">
        <asp:Literal ID="HeadingLit" runat="server"></asp:Literal></h4>
    <p>
        <asp:Literal ID="TextLit" runat="server"></asp:Literal>
    </p>
</div>
