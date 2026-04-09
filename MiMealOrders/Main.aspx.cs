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
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnReset_Click1(object sender, EventArgs e)
        {
            tbInfo.Visible = false;
            tbInfo.Text = "";
            tbUserID.Text = "";
            tbPassword.Text = "";
        }

        protected void btLogin_Click(object sender, EventArgs e)
        {
            DataLAyer.DataConnector dat = new DataLAyer.DataConnector("Server=localhost;Database=MiMealOrdering;Trusted_Connection=True;");
            ///decide which table to query
            if (cbAdmin.Checked == true)
            {
                DataTable dt = dat.DataSelect("Select intID, intDistrictID From  AdminloginInfo Where vAdminUserName = '" + tbUserID.Text + "' AND vAdminPassword = '" + tbPassword.Text + "'");

                if (dt.Rows.Count > 0)
                {
                    Session["DistrictID"] = dt.Rows[0]["intDistrictID"];
                    Session["UserName"] = tbUserID;
                    Response.Redirect("AdminHome.aspx");
                }
                else
                {
                    tbInfo.Visible = true;
                    tbInfo.Text = "Log in attempt failed";
                }
            }
            else
            {
                DataTable dt = dat.DataSelect("Select intID, intGenUserDistrictID From UserLoginInfo Where vGenUserName = '" + tbUserID.Text + "' AND vGenUserPassword = '" + tbPassword.Text + "'");

                if (dt.Rows.Count > 0)
                {
                    Session["DistrictID"] = dt.Rows[0]["intGenUserDistrictID"];
                    Session["UserName"] = tbUserID;
                    Response.Redirect("UserMain.aspx");
                }
                else
                {
                    tbInfo.Visible = true;
                    tbInfo.Text = "Log in attempt failed";
                }
            }
        }

        protected void btAdminRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminRegistration.aspx");
        }

        protected void btUserRegister_Click(object sender, EventArgs e)
        {
            Response.Redirect("UserRegistration.aspx");
        }
        
    }
}