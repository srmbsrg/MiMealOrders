using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

namespace MiMealOrders
{
    public partial class AdminRegistration : System.Web.UI.Page
    {
        DataLAyer.DataConnector dat = new DataLAyer.DataConnector("Server=127.0.0.1\\sqlexpress;Database=MiMealOrdering;Trusted_Connection=True;");

        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt = dat.DataSelect("SELECT DISTINCT vDistrictName FROM DistrictInfo");

            if (dt.Rows.Count > 0)
            {
                using (dt)
                    foreach (DataRow dr in dt.Rows)
                    {
                        ddlDistrict.Items.Add(dr["vDistrictName"].ToString());
                    }

            }
        }

        protected void btHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            int DistrictID = new int();
            int DataReturn = new int();

            if (tbName.Text.Length < 6)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Name must be a minimum of 6 characters";
                return;
            }
            if (tbPass1.Text.Length < 6 || tbPass2.Text.Length < 6)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Password must be a minimum of 6 characters";
                return;
            }
            if (tbPass1.Text != tbPass2.Text)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Password fields do not match";
                return;
            }

            DataTable dt = dat.DataSelect("SELECT intID FROM DistrictInfo WHERE vDistrictName ='" + ddlDistrict.Text + "' ");

            if (dt.Rows.Count > 0)
            {
                DistrictID = Convert.ToInt32(dt.Rows[0]["intID"]);
            }
            else
            {
                lblMessage.Visible = true;
                lblMessage.Text = "A District must be selected";
                return;
            }

            DataTable dt1 = dat.DataSelect("SELECT intID FROM AdminLoginInfo WHERE vAdminUserName ='" + tbName.Text + "' ");
            if (dt1.Rows.Count > 0)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Login name already in use";
                return;
            }
            else
            {
                ///finally save it
                DataReturn = dat.DataInsert("INSERT INTO AdminLoginInfo  (vAdminUserName, vAdminPassword, vUserEMail, intDistrictID) VALUES ('" + tbName.Text + "','" + tbPass1.Text + "','" + tbEMailAddress.Text + "'," + DistrictID + ") ");
               
                if (DataReturn > 0)
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = "Admin Login created successfully";
                    btSave.Enabled = false;
                    ddlDistrict.Enabled = false;
                    return;
                }
                else
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = "Error saving admin information";
                    return;
                }
            }
        }
    }
}
