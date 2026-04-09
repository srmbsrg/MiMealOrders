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
    public partial class UserRegistration : System.Web.UI.Page
    {

        DataLAyer.DataConnector dat = new DataLAyer.DataConnector("Server=127.0.0.1\\sqlexpress;Database=MiMealOrdering;Trusted_Connection=True;");
        int DataReturn = new int();

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

            DataTable dt = dat.DataSelect("SELECT intID FROM DistrictInfo WHERE vDistrictName ='" +ddlDistrict.Text+ "' ");

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

            DataTable dt1 = dat.DataSelect("SELECT intID FROM UserLoginInfo WHERE vGenUserName ='" + tbName.Text + "' ");
            if (dt1.Rows.Count > 0)
            {
                lblMessage.Visible = true;
                lblMessage.Text = "Login name already in use";
                return;
            }
            else
            { 
                ///finally save it
                DataReturn = dat.DataInsert(@"INSERT INTO UserLoginInfo  
                                                (vGenUserName, vGenUserPassword, vGenUserEMail, intGenUserDistrictID) 
                                                VALUES ('" +tbName.Text+ "','" +tbPass1.Text+ "','" + tbEMailAddress.Text+ "'," +DistrictID+ ") ");
                if (DataReturn > 0)
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = "User Login created successfully.";
                    btSave.Visible = false;
                    ddlDistrict.Enabled = false;
                    btSaveAccounts.Visible = true;
                    return;
                }
                else
                {
                    lblMessage.Visible = true;
                    lblMessage.Text = "Error saving user information";
                    return;
                }

            }
        }

        protected void btLookup_Click(object sender, EventArgs e)
        {
            string DBName = null;
            string vConnString = "Server=SCOTTDEV\\SQLEXPRESS;Database=MiMealOrdering; Trusted_Connection = True"; 
            int intUserID = 0;

            DataTable dt = dat.DataSelect("SELECT vDistrictDBName FROM DistrictInfo WHERE vDistrictName ='" + ddlDistrict.Text + "' ");

            if (dt.Rows.Count > 0)
            {
                DBName = Convert.ToString(dt.Rows[0]["vDistrictDBName"]);

                // connect to student database
                DataLAyer.DataConnector dlStudent = new DataLAyer.DataConnector(vConnString);

                DataTable dtStudent = dlStudent.DataSelect("SELECT [ID], [First], [Last], Campus, Grade, Homeroom  FROM Acct WHERE [ID] ='" + tbStudentID.Text + "' AND [Last] = '" + tbSLName.Text.ToUpper() + "'");

                if (dtStudent.Rows.Count > 0)
                {
                    lbMatch.Visible = false;
                    tbStudentID.Text = "";
                    tbSLName.Text = "";

                    DataTable dtUserID  = dat.DataSelect("SELECT intID FROM UserLoginInfo WHERE vGenUserName ='" + tbName.Text + "' ");

                    if (dtUserID.Rows.Count > 0)
                    {
                        intUserID = Convert.ToInt32(dtUserID.Rows[0]["intID"]);
                    }

                    //no duplicates
                    DataReturn = dat.DataDelete("DELETE FROM UserrStudentInfo WHERE intUserID = '" + intUserID + "' AND [Student ID] = '" +dtStudent.Rows[0]["ID"]+ "' ");


                    DataReturn = dat.DataInsert(@"INSERT INTO UserStudentInfo
                                                (intUserID, [Student ID], Campus,[First Name], [Last Name], HomeRoom)
                                                VALUES     (" + intUserID + ",'" + dtStudent.Rows[0]["ID"] + "','" + dtStudent.Rows[0]["Campus"] + "','" + dtStudent.Rows[0]["First"] + "','" + dtStudent.Rows[0]["Last"] + "','" + dtStudent.Rows[0]["HomeRoom"] + "')");

                    if (DataReturn > 0)
                    {
                        DataTable dtUserStudents = dat.DataSelect("SELECT DISTINCT [Student ID], Campus,[First Name], [Last Name], HomeRoom FROM  UserStudentInfo WHERE intUserID =" + intUserID + "");

                        //populate grid with info
                        gvStudents.DataSource = dtUserStudents;
                        gvStudents.DataBind();
                        return;
                    }

                }
                else
                {
                    lbMatch.Visible = true;
                    tbStudentID.Text = "";
                    tbSLName.Text = "";
                    return;
                }

            }

        }

        protected void btSaveAccounts_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }

    }
}