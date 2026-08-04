<%@ Page Title="Manage Sitemap" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="ManageSitemap.aspx.vb" Inherits="IndiaBobbles.ManageSitemap" %>

<%@ Register Src="~/admin/controls/Message.ascx" TagPrefix="uc1" TagName="Message" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <h1>Manage Sitemap</h1>
    <uc1:Message runat="server" ID="message" Visible="false" />

    <div class="row bg-light p-3 mb-3">
        <div class="col-md-12">
            <asp:Button ID="GenerateButton" runat="server" Text="Generate &amp; Save sitemap.xml"
                CssClass="btn btn-primary me-2" CausesValidation="false" OnClick="GenerateButton_Click" />
            <a href="/sitemap.xml" target="_blank" class="btn btn-secondary me-2">
                <i class="fa fa-eye"></i> View sitemap.xml
            </a>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <h4>Sitemap Entries Preview</h4>
            <div class="table-responsive">
                <asp:GridView ID="SitemapGridView" runat="server"
                    AutoGenerateColumns="False"
                    CssClass="table table-striped table-bordered table-condensed"
                    EmptyDataText="No entries found.">
                    <Columns>
                        <asp:BoundField DataField="Type" HeaderText="Type" />
                        <asp:BoundField DataField="Loc" HeaderText="URL" />
                        <asp:BoundField DataField="ChangeFreq" HeaderText="Change Freq" />
                        <asp:BoundField DataField="Priority" HeaderText="Priority" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>