using System;
using System.Data;
using System.Data.SqlClient;

namespace zhiming_web
{
    public class MSSQL
    {
        public SqlConnection conn = new SqlConnection();
        public SqlCommand sqlCmd = new SqlCommand();
        public string connstr = System.Configuration.ConfigurationManager.AppSettings["conn"];

        public MSSQL()
        {
            conn.ConnectionString = connstr;
            conn.Open();
        }

        public SqlCommand exeCmd(string sql)
        {
            sqlCmd.Connection = conn;
            sqlCmd.CommandTimeout = 15;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sql;
            return sqlCmd;
        }

        public int exesqlCmd(string sql)   //删除 修改
        {
            sqlCmd = exeCmd(sql);
            return sqlCmd.ExecuteNonQuery();
        }

        public SqlDataReader exeDr(string sql)   //查询
        {
            sqlCmd = exeCmd(sql);
            SqlDataReader dr;
            dr = sqlCmd.ExecuteReader();
            return dr;        //  while (dr.Read())   dr.GetValue(2).ToString() 
        }

        public DataSet exeDs(string sql, string tbl)   //查询
        {
            sqlCmd = exeCmd(sql);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd);
            sqlAdapter.SelectCommand = sqlCmd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, tbl);
            return ds;             //dataGridView1.DataSource = ds.Tables[0].DefaultView;
        }

        public DateTime getSvrDate()    //获取日期
        {
            string sql = "SELECT convert(char(10),getdate(),21)";
            DataSet ds = exeDs(sql, "table1");
            return DateTime.ParseExact(ds.Tables[0].Rows[0][0].ToString(), "yyyy-MM-dd", null);
        }

        public DateTime getSvrDateTime()    //获取日期+时间
        {
            string sql = "SELECT GETDATE() AS sys_Sqlser_time";
            DataSet ds = exeDs(sql, "table1");
            return (DateTime)ds.Tables[0].Rows[0][0];
        }

        public string Getxulie()
        {
            DateTime dt = getSvrDateTime();
            string s = dt.Year.ToString("0000") + dt.Month.ToString("00") + dt.Day.ToString("00");
            s = s + dt.Hour.ToString("00") + dt.Minute.ToString("00") + dt.Second.ToString("00");
            return s;
        }

        public void close()
        {
            if (conn.State != ConnectionState.Closed)
            {
                try
                {
                    conn.Close();
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

    }
    
    public class MSSQL2
    {
        public SqlConnection conn = new SqlConnection();
        public SqlCommand sqlCmd = new SqlCommand();
        public string connstr = System.Configuration.ConfigurationManager.AppSettings["conn2"];

        //private string connstr = "server=" + MyClass.ip + ";database=我知足足浴用品2015;integrated security=SSPI";

        public MSSQL2()
        {
            try
            {
                conn.ConnectionString = connstr;
                conn.Open();
            }
            catch
            {

            }
        }

        public SqlCommand exeCmd(string sql)
        {
            sqlCmd.Connection = conn;
            sqlCmd.CommandTimeout = 15;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = sql;
            return sqlCmd;
        }

        public int exesqlCmd(string sql)   //删除 修改
        {
            sqlCmd = exeCmd(sql);
            return sqlCmd.ExecuteNonQuery();
        }

        public SqlDataReader exeDr(string sql)   //查询
        {
            sqlCmd = exeCmd(sql);
            SqlDataReader dr;
            dr = sqlCmd.ExecuteReader();
            return dr;        //  while (dr.Read())   dr.GetValue(2).ToString() 
        }

        public DataSet exeDs(string sql, string tbl)   //查询
        {
            sqlCmd = exeCmd(sql);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd);
            sqlAdapter.SelectCommand = sqlCmd;
            DataSet ds = new DataSet();
            sqlAdapter.Fill(ds, tbl);
            return ds;             //dataGridView1.DataSource = ds.Tables[0].DefaultView;
        }

        public DateTime getSvrDate()    //获取日期
        {
            string sql = "SELECT convert(char(10),getdate(),21)";
            DataSet ds = exeDs(sql, "table1");
            return DateTime.ParseExact(ds.Tables[0].Rows[0][0].ToString(), "yyyy-MM-dd", null);
        }

        public DateTime getSvrDateTime()    //获取日期+时间
        {
            string sql = "SELECT GETDATE() AS sys_Sqlser_time";
            DataSet ds = exeDs(sql, "table1");
            return (DateTime)ds.Tables[0].Rows[0][0];
        }

        public void close()
        {
            if (conn.State != ConnectionState.Closed)
            {
                try
                {
                    conn.Close();
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

    }
    
}
