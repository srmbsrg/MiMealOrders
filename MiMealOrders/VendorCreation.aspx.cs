using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiMealOrders
{
    public partial class VendorCreation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminHome.aspx");
        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            if (tbName.Text.Length < 6)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Name must be a minimum of 6 characters";
                return;
            }

            /*Save Vendor*/
            DataLAyer.DataConnector dat = new DataLAyer.DataConnector("Server=127.0.0.1\\sqlexpress;Database=MiMealOrdering;Trusted_Connection=True;");
            int DataReturn = new int();
            int intDID = Convert.ToInt32(Session["DistrictID"].ToString());

            DataReturn = dat.DataInsert(@"INSERT INTO VendorinInfo  
                                                ([Vendor Name], [Vendor Contact], [Vendor Phone], [Vendor Email], intVendorDistrictID)
                                                VALUES ('" + tbName.Text + "','" + tbContact.Text + "','" + tbPhone.Text + "','" + tbEmail.Text + "'," + intDID + ") ");
            if (DataReturn > 0)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Vendor profile created successfully.";
                tbName.Text = "";
                tbContact.Text = "";
                tbPhone.Text = "";
                tbEmail.Text = "";
                return;
            }
            else
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Error saving vendor profile";
                return;
            }
        }
    }
}