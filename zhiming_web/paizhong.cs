using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace zhiming_web
{
    public static class paizhong
    {
        public static int nandaxiangrenshu = 0;
        public static int nanxiaoxiangrenshu = 0;
        public static int nvdaxiangrenshu = 0;
        public static int nvxiaoxiangrenshu = 0;
        public static int hunhedaxiangrenshu = 0;
        public static int hunhexiaoxiangrenshu = 0;
        public static int baojianrenshu = 0;
        public static int caierrenshu = 0;
        public static int xiujiaorenshu = 0;
        public static MSSQL mssql = new MSSQL();

        public static string Getxingbie(string gonghao)
        {
            string sql = "select * from renyuan where 工号='" + gonghao + "'";
            DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();
            ds = mssql.exeDs(sql, "renyuan");
            mssql.close();
            return ds.Tables[0].Rows[0]["性别"].ToString();
        }

        //获取当前时间应该的班次
        public static string GetCurBanCi()
        {
            string s = "";
            string sql = "select * from 报钟界面";

            DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();
            ds = mssql.exeDs(sql, "报钟界面");
            mssql.close();

            DateTime zao1 = Convert.ToDateTime(ds.Tables[0].Rows[0]["早班开始时间"].ToString());
            DateTime zao2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["早班结束时间"].ToString());
            DateTime curTime = DateTime.Now;

            if ((DateTime.Compare(curTime, zao1) >= 0) && (DateTime.Compare(curTime, zao2) <= 0))
            {
                s = "早班";
            }
            else
            {
                s = "晚班";
            }
            return s;
        }

        //获取甩尾模式
        public static string GetShuaiWeiMoShi()
        {
            string s = "";
            string sql = "select * from 报钟界面";
            DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();
            ds = mssql.exeDs(sql, "报钟界面");
            s = ds.Tables[0].Rows[0]["甩尾模式"].ToString();
            mssql.close();
            return s;
        }

        public static string Getxuanzhong()
        {
            string s = "";
            string sql = "select * from 报钟界面";
            DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();
            ds = mssql.exeDs(sql, "报钟界面");
            s = ds.Tables[0].Rows[0]["选钟甩尾"].ToString();
            mssql.close();

            return s;
        }

        //移到最尾
        public static void yiwei(string banci, string gonghao, string fuwushuxing, string xingbie)
        {
            if (fuwushuxing == "")
            {
                return;
            }

            string s = "";
            string sql = "";
            string sql2 = "select * from renyuan where 1=1";
            if (banci == "早班")
            {
                sql2 = sql2 + " and 早班='是'";
            }

            if (banci == "晚班")
            {
                sql2 = sql2 + " and 早班='否'";
            }

            if (GlobalConfig.GetQiyonghunhepaizhong() == "否")
            {
                #region
                if (xingbie == "男")
                {
                    switch (fuwushuxing)
                    {
                        case "大项类":
                            sql = sql2 + " and 性别='男'";
                            sql = sql + " and charindex('大项类',服务类别)>0";
                            sql = sql + " order by 男大项顺序";
                            break;

                        case "小项类":
                            sql = sql2 + " and 性别='男'";
                            sql = sql + " and charindex('小项类',服务类别)>0";
                            sql = sql + " order by 男小项顺序";
                            break;

                        case "修脚类":
                            //sql = sql2 + " and 性别='男'";
                            sql = sql2 + " and charindex('修脚类',服务类别)>0";
                            sql = sql + " order by 修脚顺序";
                            break;

                        case "采耳类":
                            //sql = sql2 + " and 性别='男'";
                            sql = sql2 + " and charindex('采耳类',服务类别)>0";
                            sql = sql + " order by 采耳顺序";
                            break;

                        case "保健类":
                            //sql = sql2 + " and 性别='男'";
                            sql = sql2 + " and charindex('保健类',服务类别)>0";
                            sql = sql + " order by 中式保健顺序";
                            break;
                    }

                    DataSet ds = new DataSet();
                    ds = mssql.exeDs(sql, "renyuan");
                    ArrayList a = new ArrayList();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        a.Add(ds.Tables[0].Rows[i]["工号"].ToString());
                        s = s + ds.Tables[0].Rows[i]["工号"].ToString() + ",";
                    }
                    //MyClass.ri("排钟男前 ", "工号" + gonghao + " " + s, "");
                    a.Remove(gonghao);
                    a.Add(gonghao);

                    s = "";
                    for (int i = 0; i < a.Count; i++)
                    {
                        s = s + a[i].ToString() + ",";
                        switch (fuwushuxing)
                        {
                            case "大项类":
                                sql = "update renyuan set 男大项顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "小项类":
                                sql = "update renyuan set 男小项顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "修脚类":
                                sql = "update renyuan set 修脚顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                sql = sql + " order by 修脚顺序";
                                mssql.exesqlCmd(sql);
                                break;

                            case "采耳类":
                                sql = "update renyuan set 采耳顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                sql = sql + " order by 采耳顺序";
                                mssql.exesqlCmd(sql);
                                break;

                            case "保健类":
                                sql = "update renyuan set 中式保健顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                sql = sql + " order by 中式保健顺序";
                                mssql.exesqlCmd(sql);
                                break;
                        }
                    }
                    //MyClass.ri("排钟男后 ", "工号" + gonghao + " " + s, "");
                }

                if (xingbie == "女")
                {
                    switch (fuwushuxing)
                    {
                        case "大项类":
                            sql = sql2 + " and 性别='女'";
                            sql = sql + " and charindex('大项类',服务类别)>0";
                            sql = sql + " order by 女大项顺序";
                            break;

                        case "小项类":
                            sql = sql2 + " and 性别='女'";
                            sql = sql + " and charindex('小项类',服务类别)>0";
                            sql = sql + " order by 女小项顺序";
                            break;

                        case "修脚类":
                            //sql = sql2 + " and 性别='女'";
                            sql = sql2 + " and charindex('修脚类',服务类别)>0";
                            sql = sql + " order by 修脚顺序";
                            break;

                        case "采耳类":
                            //sql = sql2 + " and 性别='女'";
                            sql = sql2 + " and charindex('采耳类',服务类别)>0";
                            sql = sql + " order by 采耳顺序";
                            break;

                        case "保健类":
                            //sql = sql2 + " and 性别='女'";
                            sql = sql2 + " and charindex('保健类',服务类别)>0";
                            sql = sql + " order by 中式保健顺序";
                            break;
                    }

                    DataSet ds = new DataSet();
                    ds = mssql.exeDs(sql, "renyuan");
                    ArrayList a = new ArrayList();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        a.Add(ds.Tables[0].Rows[i]["工号"].ToString());
                        s = s + ds.Tables[0].Rows[i]["工号"].ToString() + ",";
                    }
                    //MyClass.ri("排钟女前", s, "");
                    a.Remove(gonghao);
                    a.Add(gonghao);

                    s = "";
                    for (int i = 0; i < a.Count; i++)
                    {
                        s = s + a[i].ToString() + ",";
                        switch (fuwushuxing)
                        {
                            case "大项类":
                                sql = "update renyuan set 女大项顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "小项类":
                                sql = "update renyuan set 女小项顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "修脚类":
                                sql = "update renyuan set 修脚顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "采耳类":
                                sql = "update renyuan set 采耳顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "保健类":
                                sql = "update renyuan set 中式保健顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;
                        }
                    }

                    //MyClass.ri("排钟女后 ", "工号" + gonghao + " " + s, "");
                }

                #endregion
            }

            if (GlobalConfig.GetQiyonghunhepaizhong() == "是")
            {
                #region
                switch (fuwushuxing)
                {
                    case "大项类":
                        sql = sql2 + " and charindex('大项类',服务类别)>0";
                        sql = sql + " order by 混合大项顺序";
                        break;

                    case "小项类":
                        sql = sql2 + " and charindex('小项类',服务类别)>0";
                        sql = sql + " order by 混合小项顺序";
                        break;

                    case "修脚类":
                        sql = sql2 + " and charindex('修脚类',服务类别)>0";
                        sql = sql + " order by 修脚顺序";
                        break;

                    case "采耳类":
                        sql = sql2 + " and charindex('采耳类',服务类别)>0";
                        sql = sql + " order by 采耳顺序";
                        break;

                    case "保健类":
                        sql = sql2 + " and charindex('保健类',服务类别)>0";
                        sql = sql + " order by 中式保健顺序";
                        break;
                }

                DataSet ds = new DataSet();
                ds = mssql.exeDs(sql, "renyuan");
                ArrayList a = new ArrayList();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    a.Add(ds.Tables[0].Rows[i]["工号"].ToString());
                    s = s + ds.Tables[0].Rows[i]["工号"].ToString() + ",";
                }
                //MyClass.ri("排钟男女前", s, "");
                a.Remove(gonghao);
                a.Add(gonghao);

                s = "";
                for (int i = 0; i < a.Count; i++)
                {
                    s = s + a[i].ToString() + ",";
                    switch (fuwushuxing)
                    {
                        case "大项类":
                            sql = "update renyuan set 混合大项顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                            mssql.exesqlCmd(sql);
                            break;

                        case "小项类":
                            sql = "update renyuan set 混合小项顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                            mssql.exesqlCmd(sql);
                            break;

                        case "修脚类":
                            sql = "update renyuan set 修脚顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                            mssql.exesqlCmd(sql);
                            break;

                        case "采耳类":
                            sql = "update renyuan set 采耳顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                            mssql.exesqlCmd(sql);
                            break;

                        case "保健类":
                            sql = "update renyuan set 中式保健顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                            mssql.exesqlCmd(sql);
                            break;
                    }
                }
                //MyClass.ri("排钟男女后", s, "");
                #endregion
            }
        }

        //移动最前
        public static void yishou(string gonghao, string fuwushuxing, string xingbie)
        {
            string s = "";
            string banci = GetCurBanCi();
            string sql = "";
            string sql2 = "select * from renyuan where 1=1";
            if (banci == "早班")
            {
                sql2 = sql2 + " and 早班='是'";
            }

            if (banci == "晚班")
            {
                sql2 = sql2 + " and 早班='否'";
            }

            if (GlobalConfig.GetQiyonghunhepaizhong() == "否")
            {
                #region
                if (xingbie == "男")
                {
                    switch (fuwushuxing)
                    {
                        case "大项类":
                            sql = sql2 + " and 性别='男'";
                            sql = sql + " and charindex('大项类',服务类别)>0";
                            sql = sql + " order by 男大项顺序";
                            break;

                        case "小项类":
                            sql = sql2 + " and 性别='男'";
                            sql = sql + " and charindex('小项类',服务类别)>0";
                            sql = sql + " order by 男小项顺序";
                            break;

                        case "修脚类":
                            sql = sql2 + " and charindex('修脚类',服务类别)>0";
                            sql = sql + " order by 修脚顺序";
                            break;

                        case "采耳类":
                            sql = sql2 + " and charindex('采耳类',服务类别)>0";
                            sql = sql + " order by 采耳顺序";
                            break;

                        case "保健类":
                            sql = sql2 + " and charindex('保健类',服务类别)>0";
                            sql = sql + " order by 中式保健顺序";
                            break;
                    }

                    DataSet ds = new DataSet();
                    ds = mssql.exeDs(sql, "renyuan");
                    ArrayList a = new ArrayList();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        a.Add(ds.Tables[0].Rows[i]["工号"].ToString());
                        s = s + ds.Tables[0].Rows[i]["工号"].ToString() + ",";
                    }
                    //MyClass.ri("排钟男前 ", "工号" + gonghao + " " + s, "");
                    a.Remove(gonghao);
                    a.Insert(0, gonghao);

                    s = "";
                    for (int i = 0; i < a.Count; i++)
                    {
                        s = s + a[i].ToString() + ",";
                        switch (fuwushuxing)
                        {
                            case "大项类":
                                sql = "update renyuan set 男大项顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "小项类":
                                sql = "update renyuan set 男小项顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "修脚类":
                                sql = "update renyuan set 修脚顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "采耳类":
                                sql = "update renyuan set 采耳顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "保健类":
                                sql = "update renyuan set 中式保健顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;
                        }
                    }
                    //MyClass.ri("排钟男后 ", "工号" + gonghao + " " + s, "");
                }

                if (xingbie == "女")
                {
                    switch (fuwushuxing)
                    {
                        case "大项类":
                            sql = sql2 + " and 性别='女'";
                            sql = sql + " and charindex('大项类',服务类别)>0";
                            sql = sql + " order by 女大项顺序";
                            break;

                        case "小项类":
                            sql = sql2 + " and 性别='女'";
                            sql = sql + " and charindex('小项类',服务类别)>0";
                            sql = sql + " order by 女小项顺序";
                            break;

                        case "修脚类":
                            sql = sql2 + " and charindex('修脚类',服务类别)>0";
                            sql = sql + " order by 修脚顺序";
                            break;

                        case "采耳类":
                            sql = sql2 + " and charindex('采耳类',服务类别)>0";
                            sql = sql + " order by 采耳顺序";
                            break;

                        case "保健类":
                            sql = sql2 + " and charindex('保健类',服务类别)>0";
                            sql = sql + " order by 中式保健顺序";
                            break;
                    }

                    DataSet ds = new DataSet();
                    ds = mssql.exeDs(sql, "renyuan");
                    ArrayList a = new ArrayList();
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        a.Add(ds.Tables[0].Rows[i]["工号"].ToString());
                        s = s + ds.Tables[0].Rows[i]["工号"].ToString() + ",";
                    }
                    //MyClass.ri("排钟女前", s, "");
                    a.Remove(gonghao);
                    a.Insert(0, gonghao);

                    s = "";
                    for (int i = 0; i < a.Count; i++)
                    {
                        s = s + a[i].ToString() + ",";
                        switch (fuwushuxing)
                        {
                            case "大项类":
                                sql = "update renyuan set 女大项顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "小项类":
                                sql = "update renyuan set 女小项顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "修脚类":
                                sql = "update renyuan set 修脚顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "采耳类":
                                sql = "update renyuan set 采耳顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;

                            case "保健类":
                                sql = "update renyuan set 中式保健顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                                mssql.exesqlCmd(sql);
                                break;
                        }
                    }

                    //MyClass.ri("排钟女后 ", "工号" + gonghao + " " + s, "");
                }
                #endregion
            }

            if (GlobalConfig.GetQiyonghunhepaizhong() == "是")
            {
                #region
                switch (fuwushuxing)
                {
                    case "大项类":
                        sql = sql2 + " and charindex('大项类',服务类别)>0";
                        sql = sql + " order by 混合大项顺序";
                        break;

                    case "小项类":
                        sql = sql2 + " and charindex('小项类',服务类别)>0";
                        sql = sql + " order by 混合小项顺序";
                        break;

                    case "修脚类":
                        sql = sql2 + " and charindex('修脚类',服务类别)>0";
                        sql = sql + " order by 修脚顺序";
                        break;

                    case "采耳类":
                        sql = sql2 + " and charindex('采耳类',服务类别)>0";
                        sql = sql + " order by 采耳顺序";
                        break;

                    case "保健类":
                        sql = sql2 + " and charindex('保健类',服务类别)>0";
                        sql = sql + " order by 中式保健顺序";
                        break;
                }

                DataSet ds = new DataSet();
                ds = mssql.exeDs(sql, "renyuan");
                ArrayList a = new ArrayList();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    a.Add(ds.Tables[0].Rows[i]["工号"].ToString());
                    s = s + ds.Tables[0].Rows[i]["工号"].ToString() + ",";
                }
                //MyClass.ri("排钟男女前", s, "");
                a.Remove(gonghao);
                a.Insert(0, gonghao);

                s = "";
                for (int i = 0; i < a.Count; i++)
                {
                    s = s + a[i].ToString() + ",";
                    switch (fuwushuxing)
                    {
                        case "大项类":
                            sql = "update renyuan set 混合大项顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                            mssql.exesqlCmd(sql);
                            break;

                        case "小项类":
                            sql = "update renyuan set 混合小项顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                            mssql.exesqlCmd(sql);
                            break;

                        case "修脚类":
                            sql = "update renyuan set 修脚顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                            mssql.exesqlCmd(sql);
                            break;

                        case "采耳类":
                            sql = "update renyuan set 采耳顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                            mssql.exesqlCmd(sql);
                            break;

                        case "保健类":
                            sql = "update renyuan set 中式保健顺序=" + (i + 1).ToString() + " where 工号='" + a[i].ToString() + "'";
                            mssql.exesqlCmd(sql);
                            break;
                    }
                }
                //MyClass.ri("排钟男女后", s, "");
                #endregion
            }
        }
    }
}