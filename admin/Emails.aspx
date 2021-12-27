<%@ Page Title="Emails" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="Emails.aspx.vb" Inherits="IndiaBobbles.Emails" %>

<%@ Import Namespace="IndiaBobbles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>"
        SelectCommand="SELECT DISTINCT [EmailGroup] FROM [EmailMessage]"></asp:SqlDataSource>
    <h1>Emails</h1>
    <div class="row p-3 mb-3 bg-light">
        <div class="col">
            <label class="form-label" for="<%: TypeDropDown.ClientID %>">
                Find Anywhere</label>
            <asp:TextBox cssclass="form-control" ID="KeywordTextBox" MaxLength="200" runat="server"></asp:TextBox>
        </div>
        <div class="col">
            <label class="form-label" for="<%: TypeDropDown.ClientID %>">
                Type</label>
            <asp:DropDownList cssclass="form-select" ID="TypeDropDown" runat="server">
                <asp:ListItem Selected="True" Value="">--Select--</asp:ListItem>
                <asp:ListItem Value="1">Activation</asp:ListItem>
                <asp:ListItem Value="2">Unsubscribe</asp:ListItem>
                <asp:ListItem Value="3">Newsletter</asp:ListItem>
                <asp:ListItem Value="4">ChangePassword</asp:ListItem>
                <asp:ListItem Value="5">Reminder</asp:ListItem>
                <asp:ListItem Value="6">Communication</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="col">
            <label class="form-label" for="<%: GroupDropDown.ClientID %>">
                Group</label>
            <asp:DropDownList ID="GroupDropDown" cssclass="form-select" runat="server"
                DataSourceID="SqlDataSource1" DataTextField="EmailGroup"
                DataValueField="EmailGroup">
            </asp:DropDownList>
        </div>
        <div class="col">
            <label class="form-label" for="<%: SentDropDown.ClientID %>">
                Sent</label>
            <asp:DropDownList ID="SentDropDown" runat="server" cssclass="form-select">
                <asp:ListItem Selected="True" Value="">--Select--</asp:ListItem>
                <asp:ListItem Value="1">Yes</asp:ListItem>
                <asp:ListItem Value="0">No</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="col">
            <label class="form-label" for="<%: ReadDropDown.ClientID %>">
                Read</label>
            <asp:DropDownList ID="ReadDropDown" runat="server" cssclass="form-select">
                <asp:ListItem Selected="True" Value="">--Select--</asp:ListItem>
                <asp:ListItem Value="1">Yes</asp:ListItem>
                <asp:ListItem Value="0">No</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="col">
            <asp:Button ID="SubmitButton" class="btn btn-primary" runat="server" Text="Filter"
                OnClick="SubmitButton_Click" />
            <asp:Button ID="DeleteButton" runat="server" Text="Remove" OnClientClick="return confirm('You are about to delete emails, are you sure?')" CssClass="btn btn-dark float-end"
                CausesValidation="false" OnClick="DeleteButton_Click" />
        </div>
    </div>
    <div class="table-responsive">
        <asp:GridView ID="EmailGrid" AllowPaging="True" PageSize="30"
            AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-condensed"
            GridLines="None" runat="server" DataKeyNames="ID"
            OnPageIndexChanging="EmailGrid_PageIndexChanging" EmptyDataText="No Data Found.">
            <columns>
                <asp:TemplateField>
                    <headertemplate>
                        <input type="checkbox" id="selectchk" onchange="onSelectCheck();" />
                        Select
                    </headertemplate>
                    <itemtemplate>
                        <asp:CheckBox ID="cbSelect" CssClass="gridCB" runat="server"></asp:CheckBox>
                        <asp:Literal ID="EmailIDLt" Text='<%# Eval("ID") %>' Visible="false" runat="server"></asp:Literal>
                    </itemtemplate>
                </asp:TemplateField>
                <asp:HyperLinkField DataNavigateUrlFields="ID" HeaderText="Email" DataNavigateUrlFormatString="~/account/email/{0}" Text="View" Target="_blank" />
                <asp:TemplateField ShowHeader="False" HeaderText="To">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "ToAddress") %>, <%# DataBinder.Eval(Container.DataItem, "ToName") %>
                        
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="LastAttempt" HeaderText="LastAttempt" SortExpression="Attempt"
                    DataFormatString="{0:d MMM y}" />
                <asp:BoundField DataField="ReadDate" DataFormatString="{0:d MMM y}" HeaderText="Read"
                    SortExpression="ReadDate" />
                <asp:BoundField DataField="EmailGroup" HeaderText="EmailGroup" SortExpression="EmailGroup" />
                <asp:CheckBoxField DataField="IsSent" HeaderText="IsSent" SortExpression="IsSent" />
                <asp:CheckBoxField DataField="IsRead" HeaderText="IsRead" SortExpression="IsRead" />
                <asp:BoundField DataField="CreateDate" DataFormatString="{0:d MMM y}" HeaderText="Create"
                    SortExpression="CreateDate" />
                <asp:BoundField DataField="SentDate" DataFormatString="{0:d MMM y}" HeaderText="Sent"
                    SortExpression="SentDate" />
                <asp:BoundField DataField="Subject" HeaderText="Subject" SortExpression="Subject" />
            </columns>
            <PagerSettings Position="TopAndBottom" />
            <PagerStyle CssClass="paging" HorizontalAlign="Center" />
        </asp:GridView>
    </div>
    <script>
        function onSelectCheck() {
            if ($('#selectchk').is(':checked')) {
                $(".gridCB input").attr("checked", "checked");
            } else {
                $(".gridCB input").removeAttr("checked", "checked");
            }
        }
    </script>
</asp:Content>
