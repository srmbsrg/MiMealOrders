using System;
using System.Configuration;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace MiMealOrders
{
    public partial class MainOrder : System.Web.UI.Page
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
                LoadStudentDropDown();
            }
        }

        private void LoadStudentDropDown()
        {
            object districtId = Session["DistrictID"];
            object userName   = Session["UserName"];
            if (districtId == null) return;

            // Load students/children linked to this user account
            DataTable dt = Db.DataSelect(
                "SELECT intID, vStudentName FROM Students " +
                "WHERE intUserID = (SELECT intID FROM UserLoginInfo WHERE vGenUserName = '" +
                    (userName != null ? userName.ToString().Replace("'", "''") : "") + "') " +
                "ORDER BY vStudentName");

            ddlStudents.Items.Clear();
            ddlStudents.Items.Add(new ListItem("-- Select Student --", "0"));
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                    ddlStudents.Items.Add(new ListItem(row["vStudentName"].ToString(), row["intID"].ToString()));
            }
        }

        protected void ddlStudents_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStudents.SelectedValue == "0")
            {
                pnlMenuItems.Visible = false;
                return;
            }

            LoadMenuItemsForStudent(ddlStudents.SelectedValue);
        }

        private void LoadMenuItemsForStudent(string studentId)
        {
            // Get today's menu items for the student's campus
            DataTable dt = Db.DataSelect(
                "SELECT mi.intID, mi.vMItemName, mi.vMItemNameShort, mi.decMItemCost1, " +
                "       mi.intCalories, mi.decTotalFatG, mi.decSaturatedFatG, mi.decCholesterolMg, " +
                "       mi.decSodiumMg, mi.decTotalCarbG, mi.decDietaryFiberG, mi.decSugarsG, " +
                "       mi.decProteinG, mi.bitIsAIEstimate " +
                "FROM MenuItems mi " +
                "INNER JOIN Menus m ON mi.intMenuID = m.intID " +
                "INNER JOIN Students s ON s.intCampusID = m.intCampusID " +
                "WHERE s.intID = " + studentId +
                "  AND GETDATE() BETWEEN m.dtMenuStart AND m.dtMenuEnd " +
                "ORDER BY mi.intMItemType, mi.vMItemName");

            pnlMenuItems.Visible = true;

            if (dt.Rows.Count == 0)
            {
                rptMenuItems.Visible = false;
                lblNoItems.Visible = true;
                return;
            }

            lblNoItems.Visible = false;
            rptMenuItems.Visible = true;
            rptMenuItems.DataSource = dt;
            rptMenuItems.DataBind();
        }

        /// <summary>Called from ASPX databinding expression to check if nutrition data is available.</summary>
        protected bool HasNutrition(object caloriesValue)
        {
            return caloriesValue != null && caloriesValue != DBNull.Value;
        }

        /// <summary>Builds the HTML nutrition panel for a menu item row. Called from ASPX databinding.</summary>
        protected string BuildNutritionPanel(
            string itemId,
            object calories, object fatG, object satFatG, object cholMg, object sodiumMg,
            object carbG, object fiberG, object sugarsG, object proteinG, object isAIEst)
        {
            var sb = new StringBuilder();
            sb.Append("<div id='nf_").Append(HttpUtility.HtmlEncode(itemId)).Append("' class='nutrition-facts'>");
            sb.Append("<div class='nf-grid'>");
            AppendNfItem(sb, "Calories",    calories,  "");
            AppendNfItem(sb, "Total Fat",   fatG,      "g");
            AppendNfItem(sb, "Sat. Fat",    satFatG,   "g");
            AppendNfItem(sb, "Cholesterol", cholMg,    "mg");
            AppendNfItem(sb, "Sodium",      sodiumMg,  "mg");
            AppendNfItem(sb, "Total Carb",  carbG,     "g");
            AppendNfItem(sb, "Fiber",       fiberG,    "g");
            AppendNfItem(sb, "Sugars",      sugarsG,   "g");
            AppendNfItem(sb, "Protein",     proteinG,  "g");
            sb.Append("</div>");

            bool aiEst = (isAIEst != null && isAIEst != DBNull.Value && Convert.ToBoolean(isAIEst));
            if (aiEst)
                sb.Append("<div class='ai-note'>* AI-estimated values. Not a substitute for verified nutrition information.</div>");

            sb.Append("</div>");
            return sb.ToString();
        }

        private static void AppendNfItem(StringBuilder sb, string label, object val, string unit)
        {
            string display = (val != null && val != DBNull.Value)
                ? HttpUtility.HtmlEncode(val.ToString()) + unit
                : "&mdash;";
            sb.Append("<div class='nf-item'>")
              .Append("<div class='nf-label'>").Append(label).Append("</div>")
              .Append("<div class='nf-val'>").Append(display).Append("</div>")
              .Append("</div>");
        }

        protected void btHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
    }
}
