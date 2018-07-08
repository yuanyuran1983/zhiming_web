using System;
using System.Data;
using System.Data.SqlClient;

namespace zhiming_web
{
    public class GlobalConfig
    {
        private static GlobalConfig myconfig;
        private String qiyonghunhepaizhong = "";
		private String qiyongshangpinpaisongjiezhang = "";
        private int chedanshijian = 0;
        private GlobalConfig()
        {
            
        }

        public static int val(string s)
        {
            return Convert.ToInt32(s);
        }

        public static double dval(string s)
        {
            return Convert.ToDouble(s);
        }

        public static GlobalConfig GetInstance()
        {
            if (myconfig == null)
            {
                myconfig = new GlobalConfig();
                string sql = "select * from xitong";
                DataSet ds = new DataSet();
                MSSQL mssql = new MSSQL();
                ds = mssql.exeDs(sql, "xitong");
                myconfig.qiyonghunhepaizhong = ds.Tables[0].Rows[0]["启用混合排钟"].ToString();
				myconfig.qiyongshangpinpaisongjiezhang = ds.Tables[0].Rows[0]["启用商品派送结账"].ToString();
                myconfig.chedanshijian = GlobalConfig.val(ds.Tables[0].Rows[0]["撤单时间"].ToString());
            }
            return myconfig;
        }

        static public int GetChedanshijian()
        {
            GlobalConfig myconfig = GetInstance();
            if (myconfig == null)
            {
                return 0;
            }
            return myconfig.chedanshijian;
        }

        static public string GetQiyonghunhepaizhong()
        {
            GlobalConfig myconfig = GetInstance();
            if (myconfig == null)
            {
                return null;
            }
            return myconfig.qiyonghunhepaizhong;
        }

		static public string GetQiyongshangpinpaisongjiezhang()
        {
            GlobalConfig myconfig = GetInstance();
            if (myconfig == null)
            {
                return null;
            }
            return myconfig.qiyongshangpinpaisongjiezhang;
        }

        

        public static int getfenzhong(DateTime dt1, DateTime dt2)
        {
            TimeSpan ts = dt1 - dt2;
            return GlobalConfig.val(ts.TotalMinutes.ToString("f0"));
        }

    }
}