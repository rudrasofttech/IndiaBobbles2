<%@ Page Title="" Language="vb" ValidateRequest="false" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="ManageArticle.aspx.vb" Inherits="IndiaBobbles.ManageArticle" %>

<%@ Register Src="~/admin/controls/Message.ascx" TagPrefix="uc1" TagName="Message" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">
    <asp:SqlDataSource ID="CategorySource" runat="server" CacheExpirationPolicy="Sliding"
        ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>" DataSourceMode="DataReader"
        SelectCommand="SELECT ID, Name FROM Category WHere Status=0"></asp:SqlDataSource>
    <div class="row">
        <div class="col-md-12">
            <uc1:Message ID="message1" Visible="false" runat="server" />

            <h1>
                <asp:Literal ID="HeadingLit" runat="server">Create</asp:Literal></h1>
            <div class="mb-3">
                <label class="form-label" for="<%: TitleTextBox.ClientID %>">
                    Article Title</label>
                <asp:TextBox CssClass="form-control" ID="TitleTextBox" MaxLength="250" runat="server" AutoPostBack="True"
                    OnTextChanged="TitleTextBox_TextChanged"></asp:TextBox><asp:RequiredFieldValidator
                        ID="TitleReqVal" ValidationGroup="VideoGrp" ControlToValidate="TitleTextBox"
                        runat="server" ErrorMessage="Required" CssClass="validate" Display="Dynamic"
                        SetFocusOnError="True"></asp:RequiredFieldValidator>

            </div>
            <div class="mb-3">
                <label class="form-label" for="<%: MetaTitleTextBox.ClientID %>">
                    Meta Title</label>

                <asp:TextBox CssClass="form-control" ID="MetaTitleTextBox" MaxLength="250" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator
                    ID="RequiredFieldValidator2" ValidationGroup="VideoGrp" ControlToValidate="MetaTitleTextBox"
                    runat="server" ErrorMessage="Required" CssClass="validate" Display="Dynamic"
                    SetFocusOnError="True"></asp:RequiredFieldValidator>

            </div>
            <div class="mb-3">
                <label class="form-label" for="<%: URLTextBox.ClientID %>">
                    URL</label>

                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                    <ContentTemplate>
                        <div class="input-group mb-3">
                            <span class="input-group-text" id="basic-addon3"><%= IndiaBobbles.Utility.SiteURL %>/blog/</span>
                            <asp:TextBox CssClass="form-control" aria-describedby="basic-addon3" ID="URLTextBox" MaxLength="250" runat="server"
                                AutoPostBack="True" OnTextChanged="URLTextBox_TextChanged"></asp:TextBox>
                        </div>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ValidationGroup="VideoGrp"
                            ControlToValidate="URLTextBox" runat="server" ErrorMessage="Required" CssClass="validate"
                            Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator><asp:CustomValidator
                                ID="CustomValidator1" runat="server" ValidationGroup="VideoGrp" ControlToValidate="URLTextBox"
                                ErrorMessage="Duplicate URL, Please change the title or modify the url." CssClass="validate"
                                Display="Dynamic" OnServerValidate="CustomValidator1_ServerValidate" SetFocusOnError="True"></asp:CustomValidator>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="TitleTextBox" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>

            </div>
            <div class="mb-3">
                <label class="form-label" for="<%: TagTextBox.ClientID %>">
                    Tag</label>

                <asp:TextBox CssClass="form-control" ID="TagTextBox" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                    ID="TagReqVal" ValidationGroup="VideoGrp" ControlToValidate="TagTextBox" runat="server"
                    ErrorMessage="Required" CssClass="validate" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>

            </div>
            <div class="form-check mb-3">
                <label class="form-check-label" for="<%: SitemapCheckBox.ClientID %>">
                    Add To Sitemap</label>
                <asp:CheckBox ID="SitemapCheckBox" CssClass="form-check-input" Checked="true" runat="server" />
            </div>
            <div class="mb-3">
                <label class="form-label" for="<%: WriterTextBox.ClientID %>">
                    Writer Name</label>

                <asp:TextBox CssClass="form-control" ID="WriterTextBox" MaxLength="250" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                    ID="WriterReqVal" ValidationGroup="VideoGrp" ControlToValidate="WriterTextBox"
                    runat="server" ErrorMessage="Required" CssClass="validate" Display="Dynamic"
                    SetFocusOnError="True"></asp:RequiredFieldValidator>

            </div>
            <div class="mb-3">
                <label class="form-label" for="<%: WriterEmailTextBox.ClientID %>">
                    Writer Email</label>

                <asp:TextBox CssClass="form-control" ID="WriterEmailTextBox" MaxLength="250" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                    ID="WriterEmailReqVal" ValidationGroup="VideoGrp" ControlToValidate="WriterEmailTextBox"
                    runat="server" ErrorMessage="Required" CssClass="validate" Display="Dynamic"
                    SetFocusOnError="True"></asp:RequiredFieldValidator>

            </div>
            <div class="mb-3">
                <label class="form-label" for="<%: CategoryDropDown.ClientID %>">
                    Category</label>
                <asp:DropDownList ID="CategoryDropDown" CssClass="form-select" runat="server" DataMember="DefaultView" DataSourceID="CategorySource"
                    DataTextField="Name" DataValueField="ID">
                    <asp:ListItem Selected="True" Value="">--Select--</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="CategoryReqVal" ValidationGroup="VideoGrp" ControlToValidate="CategoryDropDown"
                    runat="server" ErrorMessage="Required" CssClass="validate" Display="Dynamic"
                    SetFocusOnError="True"></asp:RequiredFieldValidator>
            </div>
            <div class="mb-3">
                <label class="form-label" for="<%: FacebookImageTextBox.ClientID %>">
                    Facebook Image (<a role="button" class="btn btn-link" data-bs-toggle="modal" data-bs-target="#driveModal">View Drive</a>)</label>
                <asp:TextBox CssClass="form-control" ID="FacebookImageTextBox" MaxLength="250" runat="server"></asp:TextBox>
            </div>
            <div class="mb-3">
                <label class="form-label" for="<%: FacebookDescTextBox.ClientID %>">
                    Facebook Description</label>
                <asp:TextBox CssClass="form-control" ID="FacebookDescTextBox" MaxLength="250" runat="server"></asp:TextBox>
            </div>
            <div class="mb-3">
                <label class="form-label" for="<%: StatusDropDown.ClientID %>">
                    Status</label>

                <asp:DropDownList ID="StatusDropDown" CssClass="form-select" runat="server">
                    <asp:ListItem Value="1">Draft</asp:ListItem>
                    <asp:ListItem Selected="True" Value="2">Publish</asp:ListItem>
                    <asp:ListItem Value="3">Inactive</asp:ListItem>
                </asp:DropDownList>

            </div>
            <div class="mb-3">
                <div class="form-check">
                    <label class="form-check-label" for="SlideShowCheckBox">Slide Show</label>
                        <asp:CheckBox CssClass="form-check-input" ID="SlideShowCheckBox" ClientIDMode="Static" Text="" runat="server" />
                </div>
                <div class="form-check">
                    <label class="form-check-label" for="QuestionCheckBox">Question</label>
                        <asp:CheckBox CssClass="form-check-input" ID="QuestionCheckBox" ClientIDMode="Static" Text="" runat="server" />
                </div>
            </div>
            <div class="mb-3">
                <label class="form-label" for="<%: DescTextBox.ClientID %>">
                    Small Description</label>
                    <asp:TextBox CssClass="form-control" ID="DescTextBox" TextMode="MultiLine" Rows="5" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                        ID="DescReqVal" ValidationGroup="VideoGrp" ControlToValidate="DescTextBox" runat="server"
                        ErrorMessage="Required" CssClass="validate" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>
            </div>
            <div class="mb-3">
                <label class="form-label" for="<%: TextTextBox.ClientID %>">
                    Text (<a href="#driveModal" data-toggle="modal" role="button">Open Drive</a>)</label>

                <asp:TextBox CssClass="form-control" ID="TextTextBox" TextMode="MultiLine" Rows="20" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                    ID="TextReqVal" ValidationGroup="VideoGrp" ControlToValidate="TextTextBox" runat="server"
                    ErrorMessage="Required" CssClass="validate" Display="Dynamic" SetFocusOnError="True"></asp:RequiredFieldValidator>

            </div>
            <div class="mb-3">
                <asp:Button ID="SubmitButton" ValidationGroup="VideoGrp" class="btn btn-primary"
                    runat="server" Text="Save" OnClick="SubmitButton_Click" />
                <a href="Default.aspx" class="btn">Cancel</a>
            </div>
        </div>
    </div>
</asp:Content>
