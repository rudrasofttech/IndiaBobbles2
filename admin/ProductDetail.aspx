<%@ Page Title="Product" Language="vb" AutoEventWireup="false" MasterPageFile="~/admin/Admin.Master" CodeBehind="ProductDetail.aspx.vb" Inherits="IndiaBobbles.ProductDetail" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Body" runat="server">

    <%-- Tags on this product --%>
    <asp:SqlDataSource ID="ProductTagDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>"
        SelectCommand="SELECT CT.ID, CT.DisplayName FROM ProductTag AS PT INNER JOIN CategoryTag AS CT ON CT.ID = PT.TagID WHERE PT.ProductID = @ProductID ORDER BY CT.DisplayName">
        <SelectParameters>
            <asp:QueryStringParameter Name="ProductID" QueryStringField="id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>

    <%-- Tags that can still be added (already-assigned tags are left out, so no duplicates) --%>
    <asp:SqlDataSource ID="TagDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:indiabobblesConnectionString %>"
        SelectCommand="SELECT ID, DisplayName FROM CategoryTag WHERE ID NOT IN (SELECT TagID FROM ProductTag WHERE ProductID = @ProductID) ORDER BY DisplayName">
        <SelectParameters>
            <asp:QueryStringParameter Name="ProductID" QueryStringField="id" Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:Panel ID="NotFoundPanel" runat="server" Visible="false" CssClass="adm-panel text-center py-5">
        <i class="fa fa-question-circle fa-3x text-muted"></i>
        <h1 class="h4 mt-3">Product not found</h1>
        <p class="text-muted">It may have been removed.</p>
        <a runat="server" href="~/admin/products.aspx" class="btn btn-ib">Back to products</a>
    </asp:Panel>

    <asp:Panel ID="DetailPanel" runat="server">

        <a runat="server" href="~/admin/products.aspx" class="adm-back"><i class="fa fa-angle-left"></i> All products</a>

        <!-- ============ Hero ============ -->
        <div class="pd-hero">
            <div class="pd-thumb">
                <asp:Image ID="HeroImage" runat="server" AlternateText="" />
                <asp:PlaceHolder ID="HeroEmpty" runat="server"><span class="empty"><i class="fa fa-image"></i></span></asp:PlaceHolder>
            </div>
            <div class="pd-main">
                <div class="pd-badges"><asp:Literal ID="BadgesLiteral" runat="server" /> <span class="pd-id">ID <asp:Literal ID="IDLiteral" runat="server" /></span></div>
                <h1><asp:Literal ID="NameLiteral" runat="server" /></h1>
                <div class="pd-price"><asp:Literal ID="PriceLiteral" runat="server" /></div>
                <div class="pd-actions">
                    <asp:HyperLink ID="EditLink" runat="server" CssClass="btn btn-ib"><i class="fa fa-pencil"></i> Edit product</asp:HyperLink>
                    <asp:HyperLink ID="ViewLink" runat="server" CssClass="btn btn-ib-o" Target="_blank" rel="noopener"><i class="fa fa-external-link"></i> View on website</asp:HyperLink>
                </div>
            </div>
            <div class="pd-quick">
                <div class="lbl">Quick actions</div>
                <asp:LinkButton ID="StockButton" runat="server" CssClass="qa" CausesValidation="false" />
                <asp:LinkButton ID="StatusButton" runat="server" CssClass="qa" CausesValidation="false"
                    OnClientClick="return confirm('Change whether this product is shown on the website?');" />
            </div>
        </div>

        <asp:Label ID="MessageLabel" runat="server" CssClass="adm-flash" Visible="false" EnableViewState="false" />

        <div class="row g-3">
            <!-- ============ Left column ============ -->
            <div class="col-xl-8">

                <!-- Photos -->
                <div class="adm-panel" id="photos">
                    <div class="pd-ph-head">
                        <h2><i class="fa fa-picture-o"></i> Photos <span class="pd-count"><asp:Literal ID="PhotoCountLiteral" runat="server" /></span></h2>
                        <asp:HyperLink ID="PhotosEditLink" runat="server" CssClass="pd-link"><i class="fa fa-upload"></i> Upload photos</asp:HyperLink>
                    </div>

                    <div class="gallery pd-gallery">
                        <asp:Repeater ID="PhotoRepeater" runat="server">
                            <ItemTemplate>
                                <div class="ph">
                                    <a href='<%#: Eval("ImagePath") %>' target="_blank" rel="noopener" title="Open full size">
                                        <img src='<%#: Eval("ImagePath") %>' alt='<%# "Photo " & Eval("Sequence") %>' loading="lazy" />
                                    </a>
                                    <span class='<%# If(CBool(Eval("IsFirst")), "tag-main", "tag-seq") %>'><%# If(CBool(Eval("IsFirst")), "Main", Eval("Sequence").ToString()) %></span>
                                    <%# If(CBool(Eval("IsThumb")), "<span class=""tag-thumb"" title=""Used as thumbnail""><i class=""fa fa-star""></i></span>", "") %>
                                    <div class="tools">
                                        <asp:LinkButton runat="server" CommandName="Left" CommandArgument='<%# Eval("ID") %>' ToolTip="Move left" Enabled='<%# Not CBool(Eval("IsFirst")) %>' CausesValidation="false"><i class="fa fa-chevron-left"></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" CommandName="Thumb" CommandArgument='<%# Eval("ID") %>' ToolTip="Use as thumbnail" Visible='<%# Not CBool(Eval("IsThumb")) %>' CausesValidation="false"><i class="fa fa-star-o"></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" CommandName="First" CommandArgument='<%# Eval("ID") %>' ToolTip="Make main photo" Visible='<%# Not CBool(Eval("IsFirst")) %>' CausesValidation="false"><i class="fa fa-angle-double-left"></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" CommandName="Remove" CommandArgument='<%# Eval("ID") %>' ToolTip="Remove from gallery" CssClass="del" CausesValidation="false"
                                            OnClientClick="return confirm('Remove this photo from the product? The file stays in Drive.');"><i class="fa fa-trash"></i></asp:LinkButton>
                                        <asp:LinkButton runat="server" CommandName="Right" CommandArgument='<%# Eval("ID") %>' ToolTip="Move right" Enabled='<%# Not CBool(Eval("IsLast")) %>' CausesValidation="false"><i class="fa fa-chevron-right"></i></asp:LinkButton>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>

                    <asp:Panel ID="NoPhotosPanel" runat="server" CssClass="adm-empty" Visible="false">
                        <i class="fa fa-picture-o"></i> No photos yet. Upload some or add one by path below.
                    </asp:Panel>

                    <div class="pd-add">
                        <label class="form-label" for="<%= PhotoPathTextBox.ClientID %>">Add a photo from Drive</label>
                        <div class="url-row mb-0">
                            <asp:TextBox ID="PhotoPathTextBox" runat="server" CssClass="form-control" placeholder="/drive/products/photo.jpg or https://…" MaxLength="500" />
                            <button type="button" class="btn btn-ib-o text-nowrap" data-bs-toggle="modal" data-bs-target="#driveModal" title="Browse Drive"><i class="fa fa-folder-open"></i></button>
                            <asp:Button ID="SaveButton" runat="server" Text="Add" CssClass="btn btn-ib" ValidationGroup="photogrp" />
                        </div>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="PhotoPathTextBox" ValidationGroup="photogrp"
                            ErrorMessage="Enter the photo path" CssClass="val" Display="Dynamic" />
                        <div class="hint">Hover a photo to reorder, set it as the thumbnail ★ or remove it. The first photo is the main one on the website.</div>
                    </div>
                </div>

                <!-- Description -->
                <div class="adm-panel">
                    <h2><i class="fa fa-align-left"></i> Description</h2>
                    <div class="pd-desc" id="pdDesc"><asp:Literal ID="DescLiteral" runat="server" Mode="PassThrough" /></div>
                    <button type="button" class="pd-more d-none" id="pdMore">Show more <i class="fa fa-angle-down"></i></button>
                </div>
            </div>

            <!-- ============ Right column ============ -->
            <div class="col-xl-4">

                <!-- Tags -->
                <div class="adm-panel">
                    <h2><i class="fa fa-tags"></i> Category tags <span class="pd-count"><asp:Literal ID="TagCountLiteral" runat="server" /></span></h2>
                    <div class="pd-tags">
                        <asp:Repeater ID="TagRepeater" runat="server" DataSourceID="ProductTagDataSource">
                            <ItemTemplate>
                                <span class="pd-chip"><%#: Eval("DisplayName") %>
                                    <asp:LinkButton runat="server" CommandName="RemoveTag" CommandArgument='<%# Eval("ID") %>' ToolTip="Remove tag" CausesValidation="false"
                                        OnClientClick="return confirm('Remove this tag?');"><i class="fa fa-times"></i></asp:LinkButton>
                                </span>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:PlaceHolder ID="NoTagsPanel" runat="server" Visible="false">
                            <span class="adm-warn d-block"><i class="fa fa-exclamation-circle"></i> No tags – this product won't appear on any category page.</span>
                        </asp:PlaceHolder>
                    </div>
                    <div class="pd-tag-add">
                        <asp:DropDownList ID="TagDropDown" runat="server" CssClass="form-select" DataSourceID="TagDataSource"
                            DataTextField="DisplayName" DataValueField="ID" AppendDataBoundItems="true" aria-label="Tag to add" />
                        <asp:Button ID="SaveTagButton" runat="server" Text="Add" CssClass="btn btn-ib" CausesValidation="false" />
                    </div>
                </div>

                <!-- Specifications -->
                <div class="adm-panel">
                    <h2><i class="fa fa-list-ul"></i> Specifications</h2>
                    <dl class="pd-spec">
                        <asp:Repeater ID="SpecRepeater" runat="server">
                            <ItemTemplate>
                                <dt><%#: Eval("Key") %></dt>
                                <dd><%# If(String.IsNullOrWhiteSpace(Convert.ToString(Eval("Value"))), "<span class=""missing"">Not set</span>", HttpUtility.HtmlEncode(Convert.ToString(Eval("Value")))) %></dd>
                            </ItemTemplate>
                        </asp:Repeater>
                    </dl>
                </div>

                <!-- History -->
                <div class="adm-panel">
                    <h2><i class="fa fa-clock-o"></i> History</h2>
                    <dl class="pd-spec mb-0">
                        <dt>Created</dt><dd><asp:Literal ID="CreatedLiteral" runat="server" /></dd>
                        <dt>Last updated</dt><dd><asp:Literal ID="ModifiedLiteral" runat="server" /></dd>
                    </dl>
                </div>
            </div>
        </div>
    </asp:Panel>

    <script>
        (function () {
            // Collapse long descriptions
            var d = document.getElementById('pdDesc'), b = document.getElementById('pdMore');
            if (d && b && d.scrollHeight > 340) {
                d.classList.add('clip'); b.classList.remove('d-none');
                b.addEventListener('click', function () {
                    var open = d.classList.toggle('clip') === false;
                    b.innerHTML = open ? 'Show less <i class="fa fa-angle-up"></i>' : 'Show more <i class="fa fa-angle-down"></i>';
                });
            }
            // "e" opens the editor
            document.addEventListener('keydown', function (e) {
                if (e.key === 'e' && !/INPUT|TEXTAREA|SELECT/.test(document.activeElement.tagName)) {
                    location.href = document.getElementById('<%= EditLink.ClientID %>').href;
                }
            });
        })();
    </script>
</asp:Content>