using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiMealOrders
{
    public partial class AdminHome : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btCampus_Click(object sender, EventArgs e)
        {
            Response.Redirect("MenuCreation.aspx");
        }

        protected void btVendor_Click(object sender, EventArgs e)
        {
            Response.Redirect("VendorCreation.aspx");
        }

        protected void btMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("MenuCreation.aspx");
        }

        protected void btMenuItems_Click(object sender, EventArgs e)
        {
            Response.Redirect("MenuItemCreation.aspx");
        }

        protected void btMenuCreation_Click(object sender, EventArgs e)
        {
            Response.Redirect("MenuAdminView.aspx");
        }

        protected void btHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
    }
}