<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="RunQuery.aspx.vb" Inherits="IndiaBobbles.RunQuery" %>

<%@ Register Src="~/admin/controls/Message.ascx" TagPrefix="uc1" TagName="Message" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <div class="row">
        <div class="col-12">
            <uc1:Message runat="server" ID="Message1" />
            <h2>Run Simple Query</h2>
            <div class="mb-3">
                <label class="form-label">
                    Query Text
                </label>

                <asp:TextBox MaxLength="300" CssClass="form-control" Rows="7" ID="QueryTextBox" TextMode="MultiLine"
                    runat="server"></asp:TextBox>

            </div>
            <div class="p-3">
                <asp:Button ID="SubmitButton" class="btn btn-primary" runat="server" Text="Run" />
            </div>
        </div>
    </div>
</asp:Content>
