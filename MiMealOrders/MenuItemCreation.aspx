<%@ Page Title="" Language="C#" MasterPageFile="~/Master.Master" AutoEventWireup="true" CodeBehind="MenuItemCreation.aspx.cs" Inherits="MiMealOrders.MenuItemCreation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style type="text/css">
    .nutrition-panel {
        display: none;
        border: 1px solid #507CD1;
        background-color: #EFF3FB;
        padding: 10px 14px;
        margin: 6px 0 10px 20px;
        width: 620px;
        font-size: 11pt;
    }
    .nutrition-panel.visible { display: block; }
    .nutrition-panel h4 {
        margin: 0 0 6px 0;
        color: #2461BF;
        font-size: 11pt;
    }
    .nutrition-row { margin: 3px 0; }
    .nutrition-label { font-weight: bold; display: inline-block; width: 140px; }
    .nutrition-error { color: red; font-style: italic; }
    .ai-badge {
        font-size: 9pt;
        color: #888;
        margin-left: 8px;
    }
    #lookupStatus { margin-left: 10px; font-style: italic; color: #555; }
</style>
<script type="text/javascript">
    function lookupNutrition() {
        var itemName = document.getElementById('<%= tbMItemName.ClientID %>').value;
        if (!itemName || itemName.trim() === '') {
            alert('Enter a Menu Item Name before looking up nutrition.');
            return;
        }
        var statusEl = document.getElementById('lookupStatus');
        var panelEl  = document.getElementById('nutritionPanel');
        statusEl.innerText = 'Looking up...';

        var xhr = new XMLHttpRequest();
        xhr.open('GET', 'NutritionLookup.ashx?itemName=' + encodeURIComponent(itemName.trim()), true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                statusEl.innerText = '';
                if (xhr.status === 200) {
                    var data = JSON.parse(xhr.responseText);
                    populateNutritionPanel(data);
                } else {
                    statusEl.innerText = 'Lookup request failed (' + xhr.status + ')';
                }
            }
        };
        xhr.send();
    }

    function populateNutritionPanel(data) {
        var panelEl = document.getElementById('nutritionPanel');
        if (data.HasError || data.ErrorMessage) {
            document.getElementById('nutritionContent').innerHTML =
                '<span class="nutrition-error">Error: ' + (data.ErrorMessage || 'Unknown error') + '</span>';
            panelEl.className = 'nutrition-panel visible';
            return;
        }

        // Populate hidden fields so values are submitted with the form
        setValue('hfCalories',      data.Calories);
        setValue('hfTotalFatG',     data.TotalFatG);
        setValue('hfSaturatedFatG', data.SaturatedFatG);
        setValue('hfCholesterolMg', data.CholesterolMg);
        setValue('hfSodiumMg',      data.SodiumMg);
        setValue('hfTotalCarbG',    data.TotalCarbG);
        setValue('hfDietaryFiberG', data.DietaryFiberG);
        setValue('hfSugarsG',       data.SugarsG);
        setValue('hfProteinG',      data.ProteinG);
        setValue('hfIsAIEstimate',  data.IsAIEstimate ? '1' : '0');

        var badge = data.IsAIEstimate ? '<span class="ai-badge">(AI Estimate)</span>' : '';
        var html =
            '<h4>Nutrition Facts — Per Serving ' + badge + '</h4>' +
            '<div class="nutrition-row"><span class="nutrition-label">Calories:</span> ' + (data.Calories || '—') + '</div>' +
            '<div class="nutrition-row"><span class="nutrition-label">Total Fat:</span> ' + fmt(data.TotalFatG) + 'g</div>' +
            '<div class="nutrition-row"><span class="nutrition-label">Saturated Fat:</span> ' + fmt(data.SaturatedFatG) + 'g</div>' +
            '<div class="nutrition-row"><span class="nutrition-label">Cholesterol:</span> ' + fmt(data.CholesterolMg) + 'mg</div>' +
            '<div class="nutrition-row"><span class="nutrition-label">Sodium:</span> ' + fmt(data.SodiumMg) + 'mg</div>' +
            '<div class="nutrition-row"><span class="nutrition-label">Total Carbohydrate:</span> ' + fmt(data.TotalCarbG) + 'g</div>' +
            '<div class="nutrition-row"><span class="nutrition-label">Dietary Fiber:</span> ' + fmt(data.DietaryFiberG) + 'g</div>' +
            '<div class="nutrition-row"><span class="nutrition-label">Sugars:</span> ' + fmt(data.SugarsG) + 'g</div>' +
            '<div class="nutrition-row"><span class="nutrition-label">Protein:</span> ' + fmt(data.ProteinG) + 'g</div>';

        document.getElementById('nutritionContent').innerHTML = html;
        panelEl.className = 'nutrition-panel visible';
    }

    function fmt(val) { return (val !== null && val !== undefined) ? val : '—'; }
    function setValue(id, val) {
        var el = document.getElementById(id);
        if (el) el.value = (val !== null && val !== undefined) ? val : '';
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p style="text-align:right; padding-right: 20px;">
        <asp:Button ID="btHome" runat="server" OnClick="btHome_Click" style="height: 26px" Text="Home" Width="109px" />
    </p>

    <table style="margin-left: 20px; width: 920px; border-collapse: collapse;">
        <tr>
            <td style="width: 200px; padding: 6px 4px;"><strong>Item(s) for Menu:</strong></td>
            <td style="padding: 6px 4px;">
                <asp:DropDownList ID="ddlMenu" runat="server" Width="235px"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="padding: 6px 4px;"><strong>Menu Item Name:</strong></td>
            <td style="padding: 6px 4px;">
                <asp:TextBox ID="tbMItemName" runat="server" Width="235px"></asp:TextBox>
                &nbsp;
                <input type="button" value="Lookup Nutrition" onclick="lookupNutrition();" style="height:26px;" />
                <span id="lookupStatus"></span>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div id="nutritionPanel" class="nutrition-panel">
                    <div id="nutritionContent"></div>
                </div>
                <%-- Hidden fields carry nutrition values through the form postback --%>
                <asp:HiddenField ID="hfCalories"      runat="server" />
                <asp:HiddenField ID="hfTotalFatG"     runat="server" />
                <asp:HiddenField ID="hfSaturatedFatG" runat="server" />
                <asp:HiddenField ID="hfCholesterolMg" runat="server" />
                <asp:HiddenField ID="hfSodiumMg"      runat="server" />
                <asp:HiddenField ID="hfTotalCarbG"    runat="server" />
                <asp:HiddenField ID="hfDietaryFiberG" runat="server" />
                <asp:HiddenField ID="hfSugarsG"       runat="server" />
                <asp:HiddenField ID="hfProteinG"      runat="server" />
                <asp:HiddenField ID="hfIsAIEstimate"  runat="server" />
            </td>
        </tr>
        <tr>
            <td style="padding: 6px 4px;"><strong>Menu Item Short Name:</strong></td>
            <td style="padding: 6px 4px;">
                <asp:TextBox ID="tbMItemNameShort" runat="server" Width="235px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="padding: 6px 4px;"><strong>Menu Item Cost (Tier 1):</strong></td>
            <td style="padding: 6px 4px;">
                <asp:TextBox ID="tbMItemCost1" runat="server" Width="73px"></asp:TextBox>
                &nbsp;&nbsp; <strong>Cost (Tier 2):</strong> &nbsp;
                <asp:TextBox ID="tbMItemCost2" runat="server" Width="73px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="padding: 6px 4px;"><strong>Menu Item Type:</strong></td>
            <td style="padding: 6px 4px;">
                <asp:DropDownList ID="ddlMItemType" runat="server" Width="235px">
                    <asp:ListItem Selected="True" Value="0">Entree</asp:ListItem>
                    <asp:ListItem Value="1">Ala Carte</asp:ListItem>
                    <asp:ListItem Value="2">Type A</asp:ListItem>
                    <asp:ListItem Value="3">Type B</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="padding: 6px 4px;"><strong>Preferred Vendor:</strong></td>
            <td style="padding: 6px 4px;">
                <asp:DropDownList ID="ddlMItemVendorID" runat="server" Width="235px"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="padding: 10px 4px;">
                <asp:Label ID="lblMessage" runat="server" ForeColor="Red" Visible="False"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="2" style="padding: 4px; text-align: right; padding-right: 300px;">
                <asp:Button ID="btSave" runat="server" OnClick="btSave_Click" ForeColor="Red" Text="Save" Width="109px" />
            </td>
        </tr>
    </table>

    <div style="margin: 20px;">
        <asp:GridView ID="gvMenuItems" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" Width="921px" AutoGenerateColumns="False">
            <AlternatingRowStyle BackColor="White" />
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <Columns>
                <asp:BoundField DataField="vMItemName"      HeaderText="Item Name" />
                <asp:BoundField DataField="vMItemNameShort" HeaderText="Short Name" />
                <asp:BoundField DataField="decMItemCost1"   HeaderText="Cost T1" DataFormatString="{0:C}" />
                <asp:BoundField DataField="decMItemCost2"   HeaderText="Cost T2" DataFormatString="{0:C}" />
                <asp:BoundField DataField="intCalories"     HeaderText="Calories" NullDisplayText="—" />
                <asp:BoundField DataField="decProteinG"     HeaderText="Protein (g)" NullDisplayText="—" />
                <asp:BoundField DataField="decTotalCarbG"   HeaderText="Carbs (g)" NullDisplayText="—" />
                <asp:BoundField DataField="decSodiumMg"     HeaderText="Sodium (mg)" NullDisplayText="—" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
