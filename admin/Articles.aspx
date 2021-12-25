<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="Articles.aspx.vb" Inherits="IndiaBobbles.Articles" %>

<%@ Register Src="~/admin/controls/Message.ascx" TagPrefix="uc1" TagName="Message" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">

    <asp:SqlDataSource ID="ArticleSource" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>"
        SelectCommand="SELECT P.ID, P.Title, P.DateCreated, P.WriterName, P.Viewed, C.Name AS Category, PS.Name AS Status, P.URL, P.Sitemap FROM Category AS C INNER JOIN Post AS P ON C.ID = P.Category INNER JOIN PostStatus AS PS ON P.Status = PS.ID WHERE (P.Status = @Status) or @Status = '0'">
        <SelectParameters>
            <asp:ControlParameter ControlID="StatusDropDown" Name="Status" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="CategorySource" runat="server" CacheExpirationPolicy="Sliding"
        ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>" DataSourceMode="DataReader"
        SelectCommand="SELECT ID, Name FROM Category"></asp:SqlDataSource>

    <h1>Article List</h1>
    <uc1:Message ID="message1" Visible="false" EnableViewState="false" runat="server" />
    <div class="row bg-light">
        <div class="col-md-4">
            <div class="p-3">
                <asp:DropDownList ID="StatusDropDown" CssClass="form-select" runat="server">
                    <asp:ListItem Value="0">Status</asp:ListItem>
                    <asp:ListItem Value="1">Draft</asp:ListItem>
                    <asp:ListItem Value="2">Publish</asp:ListItem>
                    <asp:ListItem Value="3">Inactive</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>
        <div class="col-md-8">
            <div class="p-3">
                <asp:Button ID="SubmitButton" class="btn btn-primary" runat="server" Text="Filter" />
            </div>
        </div>
    </div>

    <div class="table-responsive">
        <asp:GridView ID="ArticleGridView" runat="server" AllowPaging="True" AllowSorting="True"
            AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-condensed"
            DataKeyNames="ID" DataMember="DefaultView" DataSourceID="ArticleSource" GridLines="None"
            PageSize="20" EmptyDataText="No Article found for your selected filter criteria"
            OnRowCommand="ArticleGridView_RowCommand">
            <Columns>
                <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True"
                    SortExpression="ID" />
                <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" />
                <asp:BoundField DataField="DateCreated" DataFormatString="{0:d MMM y}" HeaderText="DateCreated"
                    SortExpression="DateCreated" />
                <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                <asp:BoundField DataField="WriterName" HeaderText="WriterName" SortExpression="WriterName" />
                <asp:TemplateField ShowHeader="False" HeaderText="Viewed">
                    <ItemTemplate>
                        <a class="viewed" target="_blank" href='<%#String.Format("http://vtracker.rudrasofttech.com/report/WebpagePublicStats?id=2&path={0}{1}", HttpContext.Current.Server.UrlEncode("http://www.indiabobbles.com/blog/"), DataBinder.Eval(Container.DataItem, "URL")) %>'
                            vhref='<%# string.Format("http://www.indiabobbles.com/blog/{0}", DataBinder.Eval(Container.DataItem, "URL")) %>'></a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Category" HeaderText="Category" SortExpression="Category" />
                <asp:CheckBoxField DataField="Sitemap" HeaderText="Sitemap" SortExpression="Sitemap" ReadOnly="true" />
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="SetTopStoryButton" runat="server" CausesValidation="False" CommandName="TopStory"
                            CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>' Text="Top Story"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:HyperLinkField DataNavigateUrlFields="ID" DataNavigateUrlFormatString="managearticle.aspx?id={0}&amp;mode=edit"
                    Text="Edit" />
                <asp:TemplateField ShowHeader="False">
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CommandArgument='<%# DataBinder.Eval(Container.DataItem, "ID") %>'
                            CausesValidation="False" CommandName="DeleteCommand" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete this article?');"></asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:HyperLinkField DataNavigateUrlFields="URL" DataNavigateUrlFormatString="http://www.indiabobbles.com/blog/{0}?preview=true"
                    Target="_blank" Text="Preview" />
            </Columns>
            <PagerSettings FirstPageText="First" LastPageText="Last" Mode="NumericFirstLast" NextPageText="Next" Position="TopAndBottom" PreviousPageText="Previous" />
            <PagerStyle CssClass="paging" />
        </asp:GridView>

    </div>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".viewed").each(function () {
                var ele = $(this);
                $.get("../handlers/visitsinfo.ashx?u=" + encodeURI(ele.attr("vhref")), function (data) { ele.html(data); });
            });
        });
    </script>
</asp:Content>
