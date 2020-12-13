using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Disconnected_data_access_ASP.Net
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void GetDataFromDB()
        {
            string CS = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(CS);
            string strSelectQuery = "select * from tblStudents";
            SqlDataAdapter da = new SqlDataAdapter(strSelectQuery, con);

            DataSet ds = new DataSet();
            da.Fill(ds, "Students");

            ds.Tables["Students"].PrimaryKey = new DataColumn[] { ds.Tables["Students"].Columns["ID"] };
            Cache.Insert("DATASET", ds, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);
            gvStudents.DataSource = ds;
            gvStudents.DataBind();

            lblMessage.Text = "Data Loaded from Database";
        }
        private void GetDataFromCache()
        {
            if (Cache["DATASET"] != null)
            {
                gvStudents.DataSource = (DataSet)Cache["DATASET"];
                gvStudents.DataBind();
            }
        }
        

        protected void btnGetDataFromDB_Click(object sender, EventArgs e)
        {
            GetDataFromDB();
        }

        protected void gvStudents_RowEditing(object sender, GridViewEditEventArgs e)
        { // Set row in editing mode
            gvStudents.EditIndex = e.NewEditIndex;
            GetDataFromCache();
        }

        protected void gvStudents_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (Cache["DATASET"] != null)
            {
               DataSet ds = (DataSet)Cache["DATASET"];
              DataRow dr = ds.Tables["Students"].Rows.Find(e.Keys["ID"]);
                dr["Name"] = e.NewValues["Name"];
                dr["Gender"] = e.NewValues["Gender"];
                dr["TotalMarks"] = e.NewValues["TotalMarks"];
                Cache.Insert("DATASET", ds, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);

                gvStudents.EditIndex = -1;
                GetDataFromCache();

            }
        }

        protected void gvStudents_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvStudents.EditIndex = -1;
            GetDataFromCache();
        }

        protected void gvStudents_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (Cache["DATASET"] != null)
            {
                DataSet ds = (DataSet)Cache["DATASET"];
                DataRow dr = ds.Tables["Students"].Rows.Find(e.Keys["ID"]);
                dr.Delete();
                Cache.Insert("DATASET", ds, null, DateTime.Now.AddHours(24), System.Web.Caching.Cache.NoSlidingExpiration);
                GetDataFromCache();

            }

        }

        protected void btnUpdateDB_Click(object sender, EventArgs e)
        {
            string CS = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            SqlConnection con = new SqlConnection(CS);
            string strSelectQuery = "select * from tblStudents";
            SqlDataAdapter da = new SqlDataAdapter(strSelectQuery, con);

            DataSet ds = (DataSet)Cache["DATASET"];

            string strUpdateCommand = "Update tblStudents set Name = @Name, Gender = @Gender, TotalMarks = @TotalMarks where ID = @ID";
            SqlCommand updateCommand = new SqlCommand(strUpdateCommand, con);

            updateCommand.Parameters.Add("@Name", SqlDbType.NVarChar, 50, "Name");
            updateCommand.Parameters.Add("@Gender", SqlDbType.NVarChar, 50, "Gender");
            updateCommand.Parameters.Add("@TotalMarks", SqlDbType.Int, 50, "TotalMarks");
            updateCommand.Parameters.Add("@Id", SqlDbType.Int, 50, "Id");

            da.UpdateCommand = updateCommand;

            string strDeleteCommand = "delete from tblStudents where ID = @ID";
            SqlCommand deleteCommand = new SqlCommand(strDeleteCommand, con);
            deleteCommand.Parameters.Add("@Id", SqlDbType.Int, 50, "Id");
            da.DeleteCommand = deleteCommand;

            da.Update(ds, "Students");
            lblMessage.Text = "Database Table UPdated";

        }
    }
}