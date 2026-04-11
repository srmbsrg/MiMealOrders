using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MiMealOrders.Services;

namespace MiMealOrders
{
    public partial class MenuItemCreation : System.Web.UI.Page
    {
        private DataLAyer.DataConnector Db
        {
            get
            {
                string cs = ConfigurationManager.ConnectionStrings["MiMealOrdering"] != null
                    ? ConfigurationManager.ConnectionStrings["MiMealOrdering"].ConnectionString
                    : "Server=localhost;Database=MiMealOrdering;Trusted_Connection=True;";
                return new DataLAyer.DataConnector(cs);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadMenuDropDown();
                LoadVendorDropDown();
                LoadMenuItemsGrid();
            }
        }

        private void LoadMenuDropDown()
        {
            object districtId = Session["DistrictID"];
            if (districtId == null) return;

            DataTable dt = Db.DataSelect(
                "SELECT intID, vMenuName FROM Menus WHERE intDistrictID = " + districtId);

            ddlMenu.Items.Clear();
            ddlMenu.Items.Add(new ListItem("-- Select Menu --", "0"));
            foreach (DataRow row in dt.Rows)
                ddlMenu.Items.Add(new ListItem(row["vMenuName"].ToString(), row["intID"].ToString()));
        }

        private void LoadVendorDropDown()
        {
            object districtId = Session["DistrictID"];
            if (districtId == null) return;

            DataTable dt = Db.DataSelect(
                "SELECT intID, vVendorName FROM Vendors WHERE intDistrictID = " + districtId);

            ddlMItemVendorID.Items.Clear();
            ddlMItemVendorID.Items.Add(new ListItem("-- Select Vendor --", "0"));
            foreach (DataRow row in dt.Rows)
                ddlMItemVendorID.Items.Add(new ListItem(row["vVendorName"].ToString(), row["intID"].ToString()));
        }

        private void LoadMenuItemsGrid()
        {
            object districtId = Session["DistrictID"];
            if (districtId == null) return;

            DataTable dt = Db.DataSelect(
                "SELECT mi.intID, mi.vMItemName, mi.vMItemNameShort, mi.decMItemCost1, mi.decMItemCost2, " +
                "       mi.intCalories, mi.decProteinG, mi.decTotalCarbG, mi.decSodiumMg " +
                "FROM MenuItems mi " +
                "INNER JOIN Menus m ON mi.intMenuID = m.intID " +
                "WHERE m.intDistrictID = " + districtId +
                " ORDER BY mi.vMItemName");

            gvMenuItems.DataSource = dt;
            gvMenuItems.DataBind();
        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbMItemName.Text))
            {
                ShowMessage("Menu Item Name is required.");
                return;
            }

            if (ddlMenu.SelectedValue == "0")
            {
                ShowMessage("Please select a Menu.");
                return;
            }

            // Collect nutrition hidden-field values (may be empty if no lookup was performed)
            int?    calories      = ParseNullableInt(hfCalories.Value);
            decimal? totalFatG    = ParseNullableDecimal(hfTotalFatG.Value);
            decimal? satFatG      = ParseNullableDecimal(hfSaturatedFatG.Value);
            decimal? cholMg       = ParseNullableDecimal(hfCholesterolMg.Value);
            decimal? sodiumMg     = ParseNullableDecimal(hfSodiumMg.Value);
            decimal? carbG        = ParseNullableDecimal(hfTotalCarbG.Value);
            decimal? fiberG       = ParseNullableDecimal(hfDietaryFiberG.Value);
            decimal? sugarsG      = ParseNullableDecimal(hfSugarsG.Value);
            decimal? proteinG     = ParseNullableDecimal(hfProteinG.Value);
            bool     isAIEstimate = (hfIsAIEstimate.Value == "1");

            string sql =
                "INSERT INTO MenuItems " +
                "(vMItemName, vMItemNameShort, decMItemCost1, decMItemCost2, intMItemType, intMItemVendorID, intMenuID, " +
                " intCalories, decTotalFatG, decSaturatedFatG, decCholesterolMg, decSodiumMg, " +
                " decTotalCarbG, decDietaryFiberG, decSugarsG, decProteinG, bitIsAIEstimate, dtNutritionLookedUp) " +
                "VALUES (" +
                SqlStr(tbMItemName.Text) + ", " +
                SqlStr(tbMItemNameShort.Text) + ", " +
                SqlDecimal(tbMItemCost1.Text) + ", " +
                SqlDecimal(tbMItemCost2.Text) + ", " +
                ddlMItemType.SelectedValue + ", " +
                ddlMItemVendorID.SelectedValue + ", " +
                ddlMenu.SelectedValue + ", " +
                SqlNullableInt(calories) + ", " +
                SqlNullableDecimal(totalFatG) + ", " +
                SqlNullableDecimal(satFatG) + ", " +
                SqlNullableDecimal(cholMg) + ", " +
                SqlNullableDecimal(sodiumMg) + ", " +
                SqlNullableDecimal(carbG) + ", " +
                SqlNullableDecimal(fiberG) + ", " +
                SqlNullableDecimal(sugarsG) + ", " +
                SqlNullableDecimal(proteinG) + ", " +
                (calories.HasValue ? (isAIEstimate ? "1" : "0") : "NULL") + ", " +
                (calories.HasValue ? "GETDATE()" : "NULL") +
                ")";

            int result = Db.DataInsert(sql);

            if (result > 0)
            {
                ShowMessage("Menu item saved successfully.", false);
                ClearForm();
                LoadMenuItemsGrid();
            }
            else
            {
                ShowMessage("Error saving menu item. Please try again.");
            }
        }

        private void ClearForm()
        {
            tbMItemName.Text = "";
            tbMItemNameShort.Text = "";
            tbMItemCost1.Text = "";
            tbMItemCost2.Text = "";
            ddlMItemType.SelectedIndex = 0;
            hfCalories.Value = "";
            hfTotalFatG.Value = "";
            hfSaturatedFatG.Value = "";
            hfCholesterolMg.Value = "";
            hfSodiumMg.Value = "";
            hfTotalCarbG.Value = "";
            hfDietaryFiberG.Value = "";
            hfSugarsG.Value = "";
            hfProteinG.Value = "";
            hfIsAIEstimate.Value = "";
        }

        private void ShowMessage(string message, bool isError = true)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = isError
                ? System.Drawing.Color.Red
                : System.Drawing.Color.Green;
            lblMessage.Visible = true;
        }

        protected void btHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminHome.aspx");
        }

        // --- SQL value helpers (matching existing pattern — no DataLAyer parameterization available) ---

        private static string SqlStr(string val)
        {
            if (string.IsNullOrEmpty(val)) return "''";
            return "'" + val.Replace("'", "''") + "'";
        }

        private static string SqlDecimal(string val)
        {
            decimal d;
            return decimal.TryParse(val, out d) ? d.ToString(System.Globalization.CultureInfo.InvariantCulture) : "0";
        }

        private static string SqlNullableInt(int? val)
        {
            return val.HasValue ? val.Value.ToString() : "NULL";
        }

        private static string SqlNullableDecimal(decimal? val)
        {
            return val.HasValue ? val.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "NULL";
        }

        private static int? ParseNullableInt(string val)
        {
            int i;
            return int.TryParse(val, out i) ? (int?)i : null;
        }

        private static decimal? ParseNullableDecimal(string val)
        {
            decimal d;
            return decimal.TryParse(val, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out d) ? (decimal?)d : null;
        }
    }
}
