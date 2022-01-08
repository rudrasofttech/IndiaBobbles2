<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ViewDrive.aspx.vb" Inherits="IndiaBobbles.ViewDrive" %>

<%@ Register Src="~/admin/controls/Message.ascx" TagPrefix="uc1" TagName="Message" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-1BmE4kWBq78iYhFldvKuhfTAU6auU8tT94WrHftjDbrCEXSU1oBoqyl2QvZ6jIW3" crossorigin="anonymous" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js" integrity="sha384-ka7Sk0Gln4gmtz2MlQnikT1wXgYsOg+OMhuP+IlRH9sENBO0LRn5q+8nbTov4+1p" crossorigin="anonymous"></script>
    <link href="style.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <uc1:Message ID="message4" Visible="false" runat="server" />
        <div class="row">
            <div class="col-12">
                <nav aria-label="breadcrumb">
                    <ol class="breadcrumb">
                        <li class="breadcrumb-item"><a href="~/admin/viewdrive.aspx">Home</a></li>
                        <%
                            Dim temp As New StringBuilder()
                            For Each i In FolderList
                                If Not String.IsNullOrEmpty(i) Then
                                    temp.Append(i)
                                    temp.Append("/")
                        %>
                        <li class="breadcrumb-item"><a href="viewdrive.aspx?folderpath=<%= temp %>">
                            <%= i%></a></li>
                        <%End If
                            Next %>
                    </ol>
                </nav>
            </div>
            <asp:Repeater ID="FolderTableRepeater" runat="server">
                <HeaderTemplate>
                    <table id="folderitemtable" class="table table-hover table-condensed cursor-pointer">
                        <thead>
                            <tr>
                                <th></th>
                                <th class="type-string cursor-pointer">Name
                                </th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <img src='<%# DataBinder.Eval(Container.DataItem, "ThumbNail")%>' alt="" style="max-width: 40px;" />
                        </td>
                        <td>
                            <a href="viewdrive.aspx?folderpath=<%# DataBinder.Eval(Container.DataItem, "Location").ToString().Replace("\\", "/")%>">
                                <%# DataBinder.Eval(Container.DataItem, "Name")%></a>
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
            <asp:Repeater ID="FileItemRepeater" runat="server">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td>
                            <img src='<%# DataBinder.Eval(Container.DataItem, "ThumbNail")%>' alt="" style="max-width: 40px;" />
                        </td>
                        <td>
                            <input type="text" value='<%# DataBinder.Eval(Container.DataItem, "WebPath")%>' style='border: none; width: 100%;' />
                        </td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody> </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
        </div>
    </form>
</body>
</html>
