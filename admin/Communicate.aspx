<%@ Page Title="" Language="vb" ValidateRequest="false" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="Communicate.aspx.vb" Inherits="IndiaBobbles.Communicate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <h1>Email Communication</h1>
    <div class="p-3 mb-3 bg-light">
        <div class="mb-3">
            <label for="EmailGroupTextBox" class="form-label">Email Group</label>
            <asp:TextBox ClientIDMode="Static" ID="EmailGroupTextBox" MaxLength="100" CssClass="form-control" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="EmailGroupReqVal" ControlToValidate="EmailGroupTextBox" ValidationGroup="EmailGrp" CssClass="text-danger" runat="server" ErrorMessage="Required" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>
        <div class="mb-3">
            <label for="SubjectTextBox" class="form-label">Subject</label>
            <asp:TextBox ClientIDMode="Static" ID="SubjectTextBox" MaxLength="100" CssClass="form-control" runat="server"></asp:TextBox>
            <asp:RequiredFieldValidator ID="SubjectReqVal" ControlToValidate="SubjectTextBox" ValidationGroup="EmailGrp" CssClass="text-danger" runat="server" ErrorMessage="Required" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>
        <div class="mb-3">
            <label for="EmailTextBox" class="form-label">Email</label>
            <asp:TextBox ClientIDMode="Static" ID="EmailTextBox" TextMode="MultiLine" CssClass="form-control" runat="server" Rows="10" AutoPostBack="true" OnTextChanged="EmailTextBox_TextChanged"></asp:TextBox>
            <asp:RequiredFieldValidator ID="EmailReqVal" ControlToValidate="EmailTextBox" ValidationGroup="EmailGrp" CssClass="text-danger" runat="server" ErrorMessage="Required" Display="Dynamic"></asp:RequiredFieldValidator>
        </div>
        <div class="border p-2 my-2">
            <asp:Literal ID="PreviewLiteral" Mode="PassThrough" runat="server"></asp:Literal>
        </div>
        <div class="mb-3 form-check">
            <input type="checkbox" class="form-check-input" id="OrderEmailsChk" runat="server" />
            <label class="form-check-label" for="OrderEmailsChk">Emails from Order</label>
        </div>
        <div class="mb-3 form-check">
            <input type="checkbox" class="form-check-input" id="RegisterMemberChk" runat="server" />
            <label class="form-check-label" for="RegisterMemberChk">Register Member</label>
        </div>

        <asp:Button ID="SubmitButton" CssClass="btn btn-primary" runat="server" Text="Save Emails" ValidationGroup="EmailGrp" OnClick="SubmitButton_Click" />
    </div>
</asp:Content>
