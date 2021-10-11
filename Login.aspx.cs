using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApplication1
{
    public partial class Login : System.Web.UI.Page
    {
        string connString = System.Configuration.ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string query = @" SELECT * FROM tblUser WHERE UserName=@UserName AND Password=@Password ";
            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 600;
            cmd.Parameters.AddWithValue("@UserName", txtUserName.Text);
            cmd.Parameters.AddWithValue("@Password", txtPassword.Text);

            DataTable dt = new DataTable();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                InsertUserLog(dt);
            }

        }

        private void InsertUserLog(DataTable dt)
        {
            Session["Userid"] = dt.Rows[0]["Userid"].ToString();
            string query = @"IF NOT EXISTS (SELECT TOP 1 'NE' FROM tblUserStatus WHERE Userid=@Userid)
        BEGIN
 
        INSERT INTO [dbo].[tblUserStatus]
        ([Userid]
        ,[UserStatus]
        ,[lastLoginTime]
        )
        VALUES
        (@Userid , 'Online', GETDATE())

        END
        ELSE
        BEGIN

        UPDATE [dbo].[tblUserStatus]
        SET UserStatus='Online', [lastLoginTime]=GETDATE()
        WHERE Userid=@Userid

        END";

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.CommandType = CommandType.Text;
            cmd.CommandTimeout = 600;
            cmd.Parameters.AddWithValue("@Userid", dt.Rows[0]["Userid"].ToString());

            conn.Open();
            int i = (int)cmd.ExecuteNonQuery();
            conn.Close();
            if (i > 0)
            {
                Response.Redirect("DashBoard.aspx");
            }
        }
    }
}