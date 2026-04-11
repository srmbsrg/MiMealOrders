<%@ Page Title="" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="UserMain.aspx.cs" Inherits="MiMealOrders.MainOrder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
    .menu-items-header {
        font-size: 13pt;
        font-weight: bold;
        margin: 14px 0 8px 20px;
        color: #2461BF;
    }
    .menu-item-row {
        border: 1px solid #ccd;
        background-color: #EFF3FB;
        margin: 6px 20px;
        padding: 8px 12px;
    }
    .menu-item-row:nth-child(even) { background-color: #fff; }
    .item-name {
        font-weight: bold;
        font-size: 12pt;
    }
    .item-price {
        float: right;
        font-weight: bold;
        color: #336699;
    }
    .nutrition-toggle {
        font-size: 9pt;
        color: #336699;
        cursor: pointer;
        text-decoration: underline;
        margin-top: 4px;
        display: inline-block;
    }
    .nutrition-facts {
        display: none;
        background-color: #f9f9ff;
        border-left: 3px solid #507CD1;
        margin-top: 6px;
        padding: 6px 10px;
        font-size: 10pt;
    }
    .nutrition-facts.open { display: block; }
    .nf-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 4px 12px; }
    .nf-item .nf-label { font-weight: bold; font-size: 9pt; color: #555; }
    .nf-item .nf-val   { font-size: 10pt; }
    .ai-note { font-size: 8pt; color: #999; font-style: italic; margin-top: 4px; }
    .no-items { margin: 20px; color: #777; }
</style>
<script type="text/javascript">
    function toggleNutrition(id) {
        var el = document.getElementById('nf_' + id);
        if (!el) return;
        el.className = (el.className.indexOf('open') >= 0) ? 'nutrition-facts' : 'nutrition-facts open';
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p style="text-align:right; padding-right: 20px;">
        <asp:Button ID="btHome" runat="server" OnClick="btHome_Click" Text="Home" Width="109px" />
    </p>

    <p style="margin-left: 20px;">
        <strong>Select the child you are ordering for:</strong>&nbsp;&nbsp;
        <asp:DropDownList ID="ddlStudents" runat="server"
            AutoPostBack="true"
            OnSelectedIndexChanged="ddlStudents_SelectedIndexChanged">
        </asp:DropDownList>
    </p>

    <asp:Panel ID="pnlMenuItems" runat="server" Visible="false">
        <div class="menu-items-header">Today's Menu Items</div>

        <asp:Repeater ID="rptMenuItems" runat="server">
            <ItemTemplate>
                <div class="menu-item-row">
                    <span class="item-price">$<%# Eval("decMItemCost1", "{0:F2}") %></span>
                    <span class="item-name"><%# System.Web.HttpUtility.HtmlEncode(Eval("vMItemName").ToString()) %></span>
                    <%# Eval("vMItemNameShort") != DBNull.Value && !string.IsNullOrEmpty(Eval("vMItemNameShort").ToString())
                        ? "<br /><span style='font-size:9pt;color:#777;'>" + System.Web.HttpUtility.HtmlEncode(Eval("vMItemNameShort").ToString()) + "</span>"
                        : "" %>

                    <%# HasNutrition(Eval("intCalories")) ? "<span class='nutrition-toggle' onclick='toggleNutrition(" + Eval("intID") + ");'>[+ Nutrition Info]</span>" : "" %>

                    <%# HasNutrition(Eval("intCalories")) ? BuildNutritionPanel(
                        Eval("intID").ToString(),
                        Eval("intCalories"), Eval("decTotalFatG"), Eval("decSaturatedFatG"),
                        Eval("decCholesterolMg"), Eval("decSodiumMg"), Eval("decTotalCarbG"),
                        Eval("decDietaryFiberG"), Eval("decSugarsG"), Eval("decProteinG"),
                        Eval("bitIsAIEstimate")) : "" %>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <asp:Label ID="lblNoItems" runat="server" CssClass="no-items" Visible="false"
            Text="No menu items are available for the selected student at this time."></asp:Label>
    </asp:Panel>
</asp:Content>
