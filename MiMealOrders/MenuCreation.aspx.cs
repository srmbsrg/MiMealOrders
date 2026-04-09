using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MiMealOrders
{
    public partial class MenuCreation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("AdminHome.aspx");
        }

        protected void Calendar1_SelectionChanged(object sender, EventArgs e)
        {
            tbSDate.Text = Calendar1.SelectedDate.ToLongDateString();
            Calendar1.Visible = false;

        }

        protected void lbSDate_Click1(object sender, EventArgs e)
        {
            Calendar1.Visible = true;
        }

        protected void lbEDate_Click(object sender, EventArgs e)
        {
            Calendar2.Visible = true;
        }
        protected void Calendar2_SelectionChanged(object sender, EventArgs e)
        {
            tbEDate.Text = Calendar2.SelectedDate.ToLongDateString();
            Calendar2.Visible = false;

        }
        protected void Calendar3_SelectionChanged(object sender, EventArgs e)
        {
            tbBDate.Text = Calendar3.SelectedDate.ToLongDateString();
            Calendar3.Visible = false;

        }

        protected void lbOBDate_Click(object sender, EventArgs e)
        {
            Calendar3.Visible = true;
        }

        protected void Calendar4_SelectionChanged(object sender, EventArgs e)
        {
            tbOEDate.Text = Calendar4.SelectedDate.ToLongDateString();
            Calendar4.Visible = false;

        }


        protected void lbOEDate_Click1(object sender, EventArgs e)
        {
            Calendar4.Visible = true;
        }
    }
}