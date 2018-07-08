using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Security.Cryptography;
using System.Text;

namespace zhiming_web
{
    /// <summary>
    /// WebService1 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {
	
		[WebMethod]
		//获取技师上钟状态，供报钟使用
        public string CheckAlarm()
    	{
			string sql = "";
            MSSQL mssql = new MSSQL();
            DataSet ds = new DataSet();
            sql = "select * from xiaofeixiangmu where 完成情况='进行中'";
            sql = sql + " order by 剩余时长";
            ds = mssql.exeDs(sql, "xiaofeixiangmu");

			string retStr = "";

			if(ds.Tables[0].Rows.Count == 0) {
				mssql.close();
				return " ";
			}else {
				retStr += "{\"已上钟技师\":[";
			
	            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
	            {
	                retStr += "{\"技师\":\"";
					retStr += ds.Tables[0].Rows[i]["技师工号"].ToString();
					retStr += "\",";
					
					retStr += "\"剩余时间\":\"";
					retStr += ds.Tables[0].Rows[i]["剩余时长"].ToString();
		
					retStr += "\"},";
	            }
			}
			retStr = retStr.TrimEnd(',');
			retStr += "]}";
            mssql.close();
			return retStr;
    	}

		[WebMethod]
        public string ChangePwd(string id, string oldPwd, string newPwd)//修改密码
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] old_intput = Encoding.Default.GetBytes(oldPwd.ToCharArray());
            byte[] old_output = md5.ComputeHash(old_intput);
            string old_EncryptPwd = BitConverter.ToString(old_output).Replace("-", "");
            byte[] new_intput = Encoding.Default.GetBytes(newPwd.ToCharArray());
            byte[] new_output = md5.ComputeHash(new_intput);
            string new_EncryptPwd = BitConverter.ToString(new_output).Replace("-", "");
            string sqlstr = "select 密码 from renyuan where 工号=" + id;
            try
            {
                MSSQL mysql = new MSSQL();
                DataSet ds = mysql.exeDs(sqlstr, "renyuan");
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return "false";
                }
                else
                {
                	/*
                    if (old_EncryptPwd == ds.Tables[0].Rows[0]["密码"].ToString())
                    {
                        sqlstr = "update renyuan set 密码='"+ new_EncryptPwd + "' where 工号=" + id;
                        try
                        {
                            mysql.exesqlCmd(sqlstr);
                            mysql.close();
                            return "true";
                        }
                        catch (Exception e){
                            return "false";
                        }
                        
                    }
                    else
                    {
                        return "false";
                    }
                    */
                    //暂时使用
                    if (oldPwd == ds.Tables[0].Rows[0]["密码"].ToString())
                    {
                        sqlstr = "update renyuan set 密码='"+ newPwd + "' where 工号=" + id;
                        try
                        {
                            mysql.exesqlCmd(sqlstr);
                            mysql.close();
                            return "true";
                        }
                        catch (Exception e){
                            return "false";
                        }
                        
                    }
                    else
                    {
                        return "false";
                    }
                }
            }
            catch
            {
                return "false";
            }
        }

	#if true
		[WebMethod]
		//上钟之前获取派工信息
        public string CheckroomByGuest(string guest_no, string bCheckDaizhong)
    	{
			if (guest_no == "")
            {
                return "请输入客人手牌号";
            }

			string sql = sql = "select * from shoupai where 芯片号='" + guest_no + "'" + " or 手牌号='" + guest_no + "'"; ;
            DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();
            ds = mssql.exeDs(sql, "sql");
            mssql.close();
			string fanghao = "";

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["流水号"].ToString() != "")
                {
                    if (ds.Tables[0].Rows[0]["房号"].ToString() == "")
                    {
                    	if(bCheckDaizhong == "0") {
                        	return "成功 " + ds.Tables[0].Rows[0]["房号"].ToString();
                    	} else {//查询daizhong表
							sql = "select * from daizhong where 客人手牌号='" + guest_no + "' order by 时间 desc";
			                DataSet ds3 = new DataSet();
			                MSSQL mssql3 = new MSSQL();
			                ds3 = mssql3.exeDs(sql, "sql");
			                mssql3.close();
							if (ds3.Tables[0].Rows.Count > 0)
							{
								if(ds3.Tables[0].Rows[0]["包厢"].ToString() == ""){
									return "成功 " + ds3.Tables[0].Rows[0]["包厢"].ToString();
								} else {
									fanghao = ds3.Tables[0].Rows[0]["包厢"].ToString();
								}
							} else {
									return "成功 " + ds.Tables[0].Rows[0]["房号"].ToString();
							}
                    	}
                    }else {
						fanghao = ds.Tables[0].Rows[0]["房号"].ToString();
                    }
                    sql = "select * from fangjian where 房号='" + fanghao + "'";
	                DataSet ds2 = new DataSet();
	                MSSQL mssql2 = new MSSQL();
	                ds2 = mssql2.exeDs(sql, "sql");
	                mssql2.close();

	                if (ds2.Tables[0].Rows.Count > 0)
	                {
	                    return "成功 "+fanghao + " 有效";
	                }
	                else
	                {
	                    return "成功 "+fanghao + " 无效";
	                }
                    //return "成功 "+ds.Tables[0].Rows[0]["房号"].ToString();
                }
                else
                {
                    return "客人手牌未发出";
                }
            }
            else
            {
                return "客人手牌无效";
            }
    	}
#else
		[WebMethod]
		//上钟之前获取派工信息
        public string CheckroomByGuest(string guest_no)
    	{
			if (guest_no == "")
            {
                return "请输入客人手牌号";
            }

			string sql = "select * from xiaofeixiangmu a,  shoupai b where (b.芯片号='" + guest_no + "'" + " or b.手牌号='" + guest_no + "')"; ;
            sql += " and a.完成情况='等待中'";
			sql += " and a.流水号=b.流水号";
			sql += " order by a.日期";
			DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();
            ds = mssql.exeDs(sql, "sql");
            mssql.close();
			string fanghao = "";

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["流水号"].ToString() != "")
                {
                    if (ds.Tables[0].Rows[0]["房号"].ToString() == "")
                    {
                        return "成功 " + ds.Tables[0].Rows[0]["房号"].ToString();
                    	
                    }else {
						fanghao = ds.Tables[0].Rows[0]["房号"].ToString();
                    }
                    sql = "select * from fangjian where 房号='" + fanghao + "'";
	                DataSet ds2 = new DataSet();
	                MSSQL mssql2 = new MSSQL();
	                ds2 = mssql2.exeDs(sql, "sql");
	                mssql2.close();

	                if (ds2.Tables[0].Rows.Count > 0)
	                {
	                    return "成功 "+fanghao + " 有效";
	                }
	                else
	                {
	                    return "成功 "+fanghao + " 无效";
	                }
                    //return "成功 "+ds.Tables[0].Rows[0]["房号"].ToString();
                }
                else
                {
                    return "客人手牌未发出";
                }
            }
            else
            {
                return "客人手牌无效";
            }
    	}
#endif

		
	   [WebMethod]
        //点钟
        public string DianZhong(string buzhang_id, string jishi_id, 
        string guest_id, string service_name, string room_id)
        {
            if(buzhang_id.Equals("") || jishi_id.Equals("") || guest_id.Equals("") || room_id.Equals(""))
            {
                return "缺少必要的信息，请检查查询条件是否完整";
            }

				
            string sql = "select * from renyuan where 工号='" + buzhang_id + "'";
            sql = sql + " and 类型='部长'";
            try
            {
                MSSQL mysql = new MSSQL();
                DataSet ds = mysql.exeDs(sql, "renyuan");
                //mysql.close();
                if (ds.Tables[0].Rows.Count == 0)
                {
                    mysql.close();
                    return "部长手牌无效";
                }
                else
                {
                    try
                    {
						sql = "select * from fangjian where 房号='" + room_id + "'";
						ds = mysql.exeDs(sql, "fangjian");
						if (ds.Tables[0].Rows.Count == 0)
						{
							mysql.close();
							return "房号不是有效";
						}
					
                        sql = "select * from renyuan where 工号='" + jishi_id + "'";
                        sql = sql + " and 类型='技师'";
                        ds = mysql.exeDs(sql, "renyuan");
                        string xingbie = ds.Tables[0].Rows[0]["性别"].ToString();

                        string buzhanggonghao = buzhang_id;
                        sql = "select * from shoupai where 手牌号='" + guest_id + "'";
                        sql = sql + " and 流水号<>''";
                        ds = mysql.exeDs(sql, "keren");
                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            mysql.close();
                            return "客人手牌未领用";
                        }

                        string kerenliushuihao = ds.Tables[0].Rows[0]["流水号"].ToString();
                        string kerenshoupaihao = ds.Tables[0].Rows[0]["手牌号"].ToString();
                        string fuwushuxing = "";
                        string fuwuxiangmu = "";

                        sql = "select * from fuwuxiangmu where 服务项目名称='" + service_name + "'";
                        ds = mysql.exeDs(sql, "fuwuxiangmubianhao");
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            fuwushuxing = ds.Tables[0].Rows[0]["服务属性"].ToString();
                            fuwuxiangmu = ds.Tables[0].Rows[0]["服务项目名称"].ToString();
                        }

                        DateTime dt = DateTime.Now;

                        sql = "insert into daizhong (时间,包厢,项目名称,男,女,不限,客人手牌号,部长,服务属性,技师工号,点轮) values ('";
                        sql = sql + dt.ToString() + "','";
                        sql = sql + room_id + "','";
                        sql = sql + fuwuxiangmu + "','";
                        sql = sql + "" + "','";
                        sql = sql + "" + "','";
                        sql = sql + "" + "','";
                        sql = sql + kerenshoupaihao + "','";
                        sql = sql + buzhanggonghao + "','";
                        sql = sql + fuwushuxing + "','";
                        sql = sql + jishi_id + "','";
                        sql = sql + "点" + "')";
                        mysql.exesqlCmd(sql);
                        //string tmp = "房号:" + fanghao + " 技师:" + jishigonghao + " 项目:" + textBox3.Text
                            //+ " 客人:" + kerenshoupaihao + " 部长:" + MyClass.caozuoyuan;
                        //MyClass.ri("消费项目--点钟", tmp, MyClass.buzhangshoupai);
                        mysql.close();
                        return "点钟操作成功";
                    }
                    catch
                    {
                        mysql.close();
                        return "点钟操作失败";
                    }
                }
            }
            catch
            {
                return "点钟操作失败";
            }
        }
	
		[WebMethod]
		public string GetFangHaoList()
		{
			string sql = "select * from fangjian order by 房号";
			DataSet ds = new DataSet();
			ds = paizhong.mssql.exeDs(sql,"fanghao");
			string retStr = "[";
			for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
			{
				retStr += "{";
            	retStr += "\"房号\":\"";
				retStr += ds.Tables[0].Rows[i]["房号"].ToString();
				retStr += "\"},";
			}
			retStr = retStr.TrimEnd(',');
			retStr += "]";
			return retStr;
	   }

	   	[WebMethod]
		//获取技师状态
		public string GetJishiStatus(string jishi_id)
		{
			if (jishi_id == "")
			{
				return "请填写技师ID";
			}

			string sql = "";


			DataSet ds = new DataSet();

			sql = "select * from renyuan where (工号='" + jishi_id + "'";
			sql = sql + " or 芯片号='" + jishi_id + "')";
			sql = sql + " and 类型='技师'";
			ds = paizhong.mssql.exeDs(sql, "renyuan");
			if (ds.Tables[0].Rows.Count == 0)
			{
				return "手牌不在系统内";
			}

			return ds.Tables[0].Rows[0]["技师状态"].ToString();

		}

		
    	[WebMethod]
        public string HelloWorld()
    	{
    		return "Hello World";
        }
	
		[WebMethod]
		//[ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
		public string Kaifang(string guest_no, string room_no)
		{
			string sql = "";
            string s = guest_no;
            string shoupaihao = "";
            sql = "select * from shoupai where (手牌号='" + s + "' or 芯片号='" + s + "'" + " or 串号='" + s + "'" + ")";
            sql = sql + " and 串号<>''";
            DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();
            ds = mssql.exeDs(sql, "shoupai");
            if (ds.Tables[0].Rows.Count > 0)
            {
				shoupaihao = ds.Tables[0].Rows[0]["手牌号"].ToString();
				
				/*
                if (ds.Tables[0].Rows[0]["房号"].ToString() != room_no)
                {
                    mssql.close();
                    return "手牌房号与开房房号不符";
                }
				*/

                if (ds.Tables[0].Rows[0]["发牌时间"].ToString() != "")
                {
                    //textBox2.Text = ds.Tables[0].Rows[0]["手牌号"].ToString();
                }
                else
                {
                    mssql.close();
                    return "手牌未发出";
                }
            }
            else
            {
                mssql.close();
                return "手牌号不在系统里";
            }

            //shoupaihao = ds.Tables[0].Rows[0]["手牌号"].ToString();

            sql = "select * from fangjian where 手牌号='" + shoupaihao + "'";
            ds = mssql.exeDs(sql, "fangjian");
            if (ds.Tables[0].Rows.Count > 0)
            {
                mssql.close();
                return "此手牌已绑定房号" + ds.Tables[0].Rows[0]["房号"].ToString();
            }


            string dt = mssql.getSvrDateTime().ToString("yyyy-MM-dd HH:mm:ss");
            sql = "select * from fangjian where 房号='" + room_no + "'";
            ds = mssql.exeDs(sql, "fangjian");
			if (ds.Tables[0].Rows.Count == 0)
            {
                mssql.close();
                return "房号无效";
            }
            if (ds.Tables[0].Rows[0]["开房时间"].ToString() == "")
            {
                sql = "update fangjian set 开房时间='" + dt + "'";
                sql = sql + ",状态='使用'";
				sql = sql + ",手牌号='" + shoupaihao + "'";
                sql = sql + " where 房号='" + room_no + "'";
                mssql.exesqlCmd(sql);
            }

			/*
            sql = "update shoupai set 房号='" + room_no + "'";
            sql = sql + " where 手牌号='" + shoupaihao + "'";
            mssql.exesqlCmd(sql);
            mssql.close();
			*/

            //MyClass.ri("开房", "房号:" + textBox1.Text + " 客人:" + shoupaihao, MyClass.gonghao);
            //this.Close();
            return "开房成功";
		}
	

        [WebMethod]
        public string PaiZhong(string buzhang_id, string keren_id, string service_name
            , string jishi_type, string room_no, string renshu, string sex)//选钟
        {
            if (keren_id.Equals(""))
            {
                return "请输入客人手牌号";
            }
            if (buzhang_id.Equals(""))
            {
                return "请填写部长信息";
            }
            if (jishi_type.Equals("") && service_name.Equals(""))
            {
                return "请选择项目或技师类型";
            }
            string sql = "select * from renyuan where 工号='" + buzhang_id + "'";
            sql = sql + " and 类型='部长'";
            MSSQL mysql = new MSSQL();
            try
            {
                DataSet ds = mysql.exeDs(sql, "renyuan");
                //mysql.close();
                if (ds.Tables[0].Rows.Count == 0)
                {
                    mysql.close();
                    return "部长手牌无效";
                }
            }
            catch
            {
                mysql.close();
                return "排钟操作失败";
            }

            if (room_no != "")
            {
                sql = "select * from fangjian where 房号='" + room_no + "'";
                DataSet ds = new DataSet();
                ds = mysql.exeDs(sql, "fangjian");
                if (ds.Tables[0].Rows.Count == 0)
                {
                    mysql.close();
                    return "房号不是有效";
                }
            }

            return pai1(buzhang_id, keren_id, service_name, jishi_type, room_no, renshu, sex);
        }

        private string pai1(string buzhang_id, string keren_id,  string service_name
            , string jishi_type, string room_no, string renshu, string sex)
        {
            string sql = "";
            MSSQL mysql = new MSSQL();
            DataSet ds = new DataSet();     //项目表
            DataSet ds1 = new DataSet();    //女表
            DataSet ds2 = new DataSet();    //男表
            DataSet ds3 = new DataSet();    //不限

            int nvrenshu = 0;
            int nanrenshu = 0;
            int buxianrenshu = 0;
            if (sex.Equals("女"))
            {
                nvrenshu = GlobalConfig.val(renshu);
            }
            if (sex.Equals("男"))
            {
                nanrenshu = GlobalConfig.val(renshu);
            }
            if (sex.Equals("不限"))
            {
                buxianrenshu = GlobalConfig.val(renshu);
            }

            string xiangmu = service_name;

            string fanghao = room_no;   //房号

            string buzhanggonghao = buzhang_id;   //部长工号

            sql = "select * from shoupai where 手牌号='" + keren_id + "'";
            sql = sql + " and 流水号<>''";
            ds = mysql.exeDs(sql, "keren");
            if (ds.Tables[0].Rows.Count == 0)
            {
                mysql.close();
                return "客人卡未发牌";
            }

            string kerenliushuihao = ds.Tables[0].Rows[0]["流水号"].ToString();   //客人流水号
            string kerenshoupaihao = ds.Tables[0].Rows[0]["手牌号"].ToString();   //客人手牌号
            string fuwuxiangmu = "";
            string fuwushuxing = "";

            try
            {
                string yuanfanghao = ds.Tables[0].Rows[0]["房号"].ToString();
                if (yuanfanghao == "")
                {
                    sql = "update shoupai set 房号='" + fanghao + "'" + " where 手牌号='" + kerenshoupaihao + "'";
                    mysql.exesqlCmd(sql);
                }

                if (service_name != "")
                {
                    #region
                    sql = "select * from fuwuxiangmu where 服务项目名称='" + xiangmu + "'";
                    ds = mysql.exeDs(sql, "fuwuxiangmu");
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        fuwuxiangmu = ds.Tables[0].Rows[0]["服务项目名称"].ToString();
                        fuwushuxing = ds.Tables[0].Rows[0]["服务属性"].ToString();
                    }

                    for (int k = 0; k < nvrenshu; k++)
                    {
                        sql = "insert into daizhong (时间,包厢,项目名称,服务属性,男,女,不限,客人手牌号,点轮,技师工号,部长) values ('";
                        sql = sql + DateTime.Now.ToString() + "','";
                        sql = sql + fanghao + "','";
                        sql = sql + fuwuxiangmu + "','";
                        sql = sql + fuwushuxing + "','";
                        sql = sql + "否" + "','";
                        sql = sql + "是" + "','";
                        sql = sql + "否" + "','";
                        sql = sql + kerenshoupaihao + "','";
                        sql = sql + "轮" + "','";
                        sql = sql + "" + "','";
                        sql = sql + buzhanggonghao + "')";
                        mysql.exesqlCmd(sql);
                    }

                    for (int k = 0; k < nanrenshu; k++)
                    {
                        sql = "insert into daizhong (时间,包厢,项目名称,服务属性,男,女,不限,客人手牌号,点轮,技师工号,部长) values ('";
                        sql = sql + DateTime.Now.ToString() + "','";
                        sql = sql + fanghao + "','";
                        sql = sql + fuwuxiangmu + "','";
                        sql = sql + fuwushuxing + "','";
                        sql = sql + "是" + "','";
                        sql = sql + "否" + "','";
                        sql = sql + "否" + "','";
                        sql = sql + kerenshoupaihao + "','";
                        sql = sql + "轮" + "','";
                        sql = sql + "" + "','";
                        sql = sql + buzhanggonghao + "')";
                        mysql.exesqlCmd(sql);
                    }

                    for (int k = 0; k < buxianrenshu; k++)
                    {
                        sql = "insert into daizhong (时间,包厢,项目名称,服务属性,男,女,不限,客人手牌号,点轮,技师工号,部长) values ('";
                        sql = sql + DateTime.Now.ToString() + "','";
                        sql = sql + fanghao + "','";
                        sql = sql + fuwuxiangmu + "','";
                        sql = sql + fuwushuxing + "','";
                        sql = sql + "否" + "','";
                        sql = sql + "否" + "','";
                        sql = sql + "是" + "','";
                        sql = sql + kerenshoupaihao + "','";
                        sql = sql + "轮" + "','";
                        sql = sql + "" + "','";
                        sql = sql + buzhanggonghao + "')";
                        mysql.exesqlCmd(sql);
                    }

                    //MyClass.ri("触摸屏自动排钟", "房号:" + fanghao + " 项目:" + fuwuxiangmu + " " + " 男:" + nanrenshu.ToString()
                    //   + " 女:" + nvrenshu.ToString() + " 不限:" + buxianrenshu.ToString(), MyClass.buzhangshoupai);
                    #endregion
                }

                if (jishi_type != "")
                {
                    fuwuxiangmu = "";
                    fuwushuxing = jishi_type;

                    for (int k = 0; k < nvrenshu; k++)
                    {
                        sql = "insert into daizhong (时间,包厢,项目名称,服务属性,男,女,不限,客人手牌号,点轮,技师工号,部长) values ('";
                        sql = sql + DateTime.Now.ToString() + "','";
                        sql = sql + fanghao + "','";
                        sql = sql + fuwuxiangmu + "','";
                        sql = sql + fuwushuxing + "','";
                        sql = sql + "否" + "','";
                        sql = sql + "是" + "','";
                        sql = sql + "否" + "','";
                        sql = sql + kerenshoupaihao + "','";
                        sql = sql + "轮" + "','";
                        sql = sql + "" + "','";
                        sql = sql + buzhanggonghao + "')";
                        mysql.exesqlCmd(sql);
                    }

                    for (int k = 0; k < nanrenshu; k++)
                    {
                        sql = "insert into daizhong (时间,包厢,项目名称,服务属性,男,女,不限,客人手牌号,点轮,技师工号,部长) values ('";
                        sql = sql + DateTime.Now.ToString() + "','";
                        sql = sql + fanghao + "','";
                        sql = sql + fuwuxiangmu + "','";
                        sql = sql + fuwushuxing + "','";
                        sql = sql + "是" + "','";
                        sql = sql + "否" + "','";
                        sql = sql + "否" + "','";
                        sql = sql + kerenshoupaihao + "','";
                        sql = sql + "轮" + "','";
                        sql = sql + "" + "','";
                        sql = sql + buzhanggonghao + "')";
                        mysql.exesqlCmd(sql);
                    }

                    for (int k = 0; k < buxianrenshu; k++)
                    {
                        sql = "insert into daizhong (时间,包厢,项目名称,服务属性,男,女,不限,客人手牌号,点轮,技师工号,部长) values ('";
                        sql = sql + DateTime.Now.ToString() + "','";
                        sql = sql + fanghao + "','";
                        sql = sql + fuwuxiangmu + "','";
                        sql = sql + fuwushuxing + "','";
                        sql = sql + "否" + "','";
                        sql = sql + "否" + "','";
                        sql = sql + "是" + "','";
                        sql = sql + kerenshoupaihao + "','";
                        sql = sql + "轮" + "','";
                        sql = sql + "" + "','";
                        sql = sql + buzhanggonghao + "')";
                        mysql.exesqlCmd(sql);
                    }
                }
                mysql.close();
                return "安排成功";
            }
            catch
            {
                mysql.close();
                return "安排失败";
            }
        }

       
        [WebMethod]
        public string Login(string id, string pwd, string device_id, string current_time)//登录
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] intput = Encoding.Default.GetBytes(pwd.ToCharArray());
            byte[] output = md5.ComputeHash(intput);
            string EncryptPwd = BitConverter.ToString(output).Replace("-", "");
            string sqlstr = "select 密码 from renyuan where 工号=" + id;
            try
            {
                MSSQL mysql = new MSSQL();
                DataSet ds = mysql.exeDs(sqlstr, "renyuan");
                mysql.close();
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return "false";
                }
                else
                {
                    //if(EncryptPwd == ds.Tables[0].Rows[0]["密码"].ToString())
					if(pwd == ds.Tables[0].Rows[0]["密码"].ToString())//暂时使用
                    {
                        return "true";
                    }
                    else
                    {
                        return "false";
                    }
                }
            }
            catch
            {
                return "false";
            }
        }



        [WebMethod]
        //[ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public string Select(string sqlStr)
        {
            string retStr = "";
            try
            {
                MSSQL mysql = new MSSQL();

                DataSet ds = mysql.exeDs(sqlStr, "renyuan");
                mysql.close();
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return null;
                }
                else
                {
                    retStr += "[";
                    for (int i=0;i< ds.Tables[0].Rows.Count; i++)
                    {
                        retStr += "{";
                        for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                        {
                            retStr += "\"";
                            retStr += ds.Tables[0].Columns[j].ColumnName.ToString();
                            retStr += "\":\"";
                            retStr += ds.Tables[0].Rows[i][j].ToString();
                            retStr += "\",";
                        }
                        retStr = retStr.TrimEnd(',');
                        retStr += "},";
                    }
                    retStr = retStr.TrimEnd(',');
                    retStr += "]";
                    return retStr;
                }

            }
            catch
            {
                return null;
            }
        }


		

        [WebMethod]
        public string Shangzhong_Paigong(string jishi_id)
        {
            if (jishi_id == "")
            {
                return "请填写技师ID";
            }

            string s = jishi_id;
            //textBox1.Text = "";
            string jishigonghao = "";

            string sql = "";
            try
            {
                DataSet ds = new DataSet();

                sql = "select * from renyuan where (工号='" + s + "'";
                sql = sql + " or 芯片号='" + s + "')";
                sql = sql + " and 类型='技师'";
                ds = paizhong.mssql.exeDs(sql, "renyuan");
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return "手牌不在系统内";
                }
                jishigonghao = ds.Tables[0].Rows[0]["工号"].ToString();
                string xingbie = ds.Tables[0].Rows[0]["性别"].ToString();

                if (ds.Tables[0].Rows[0]["技师状态"].ToString() == "进行中")
                {
                    return "技师未下钟";
                }


                string fanghao = "";
                string liushuihao = "";
                string kerenshoupai = "";

                sql = "select * from xiaofeixiangmu where";
                sql = sql + " 技师工号='" + jishigonghao + "'";
                sql = sql + " and (完成情况='等待中' or 完成情况='暂停')";
                sql = sql + " and 服务项目名称<>''";
                ds = paizhong.mssql.exeDs(sql, "jishi");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string lunzhong = ds.Tables[0].Rows[0]["轮钟"].ToString();
                    string xuanzhongshuaiwei = paizhong.Getxuanzhong();
                    string xuanzhong = ds.Tables[0].Rows[0]["选钟"].ToString();
                    string fuwushuxing = ds.Tables[0].Rows[0]["服务属性"].ToString();
                    fanghao = ds.Tables[0].Rows[0]["房号"].ToString();
                    liushuihao = ds.Tables[0].Rows[0]["流水号"].ToString();
                    kerenshoupai = ds.Tables[0].Rows[0]["客人手牌号"].ToString();

                    string xiangmu = ds.Tables[0].Rows[0]["服务项目名称"].ToString();
                    string id = ds.Tables[0].Rows[0]["id"].ToString();
                    if (ds.Tables[0].Rows[0]["完成情况"].ToString() == "等待中")
                    {
                        sql = "update xiaofeixiangmu set 完成情况='进行中'";
                        sql = sql + ",上钟时间='" + paizhong.mssql.getSvrDateTime() + "'";
                        sql = sql + " where id=" + id;
                        paizhong.mssql.exesqlCmd(sql);
                    }

                    //换技师的上钟
                    if (ds.Tables[0].Rows[0]["完成情况"].ToString() == "暂停")
                    {
                        sql = "update xiaofeixiangmu set 完成情况='进行中'";
                        sql = sql + " where id=" + id;
                        paizhong.mssql.exesqlCmd(sql);
                    }

                    sql = "update renyuan set 流水号='" + liushuihao + "'";
                    sql = sql + ",当前项目='" + ds.Tables[0].Rows[0]["服务项目名称"].ToString() + "'";
                    sql = sql + ",房号='" + fanghao + "'";
                    sql = sql + ",技师状态='进行中'";
                    sql = sql + " where 工号='" + jishigonghao + "'";
                    paizhong.mssql.exesqlCmd(sql);

                    string CurBanCi = paizhong.GetCurBanCi();

                    if ((xuanzhongshuaiwei == "是") && (xuanzhong == "是"))
                    {
                        paizhong.yiwei(CurBanCi, jishigonghao, fuwushuxing, xingbie);
                    }

                    if (lunzhong == "是")
                    {
                        paizhong.yiwei(CurBanCi, jishigonghao, fuwushuxing, xingbie);
                    }

                    string tmp = "房号:" + fanghao + " 项目:" + ds.Tables[0].Rows[0]["服务项目名称"].ToString()
                        + "客人:" + kerenshoupai + " 技师:" + jishigonghao;
                    //MyClass.ri("上钟", tmp, jishigonghao);

                    return "上钟成功 "+ds.Tables[0].Rows[0]["时长"].ToString();

                }
                else
                {
                    sql = "select * from xiaofeixiangmu where";
                    sql = sql + " 技师工号='" + jishigonghao + "'";
                    sql = sql + " and (完成情况='等待中' or 完成情况='暂停')";
                    sql = sql + " and 服务项目名称=''";
                    ds = paizhong.mssql.exeDs(sql, "xiaofeixiangmu");
                    if (ds.Tables[0].Rows.Count >= 0)
                    {
                        //无项目
                        return "无项目,请选择自主上钟";
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "上钟失败";
        }

        [WebMethod]
        public string Shangzhong_Zizhu(string jishi_id, string guest_no, 
        		string service_name, string room_no, string leixing)
        {
            if (jishi_id == "")
            {
                return "请输入技师ID";
            }

            if (guest_no == "")
            {
                return "请输入客人手牌号";
            }

			if (service_name == "")
            {
                return "请选择服务项目";
            }

            string sql = sql = "select * from shoupai where 芯片号='" + guest_no + "'" + " or 手牌号='" + guest_no + "'"; ;
            DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();
            ds = mssql.exeDs(sql, "sql");
            mssql.close();

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0]["流水号"].ToString() != "")
                {
                    //textBox2.Text = ds.Tables[0].Rows[0]["手牌号"].ToString();
                    //textBox6.Text = ds.Tables[0].Rows[0]["房号"].ToString();
                    //if (textBox6.Text != "")
                    //{
                    //    textBox6.Enabled = false;
                    //}
                    //label4.Text = "有效";
                }
                else
                {
                    return "客人手牌未发出";
                }
            }
            else
            {
                return "客人手牌无效";
            }


            sql = "";
            ds = new DataSet();

            string dt = paizhong.mssql.getSvrDateTime().ToString("yyyy-MM-dd HH:mm:ss");
            string jishigonghao = jishi_id;
            string kerenshoupaihao = guest_no;
            string kerenliushuihao = "";
            string xiangmu = service_name;
            string fanghao = room_no;
			string xiangmushichang = "";

            if (fanghao != "")
            {
                sql = "select * from fangjian where 房号='" + fanghao + "'";
                ds = paizhong.mssql.exeDs(sql, "fangjian");
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return "房号不是有效";
                }
            }
            sql = "select * from renyuan where 工号='" + jishi_id + "'";
            ds = paizhong.mssql.exeDs(sql, "renyuan");
            string shuxing1 = ds.Tables[0].Rows[0]["服务类别"].ToString();

            sql = "select * from fuwuxiangmu where 服务项目名称='" + service_name + "'";
            ds = paizhong.mssql.exeDs(sql, "renyuan");
            string shuxing2 = ds.Tables[0].Rows[0]["服务属性"].ToString();
            if (!shuxing1.Contains(shuxing2))
            {
                return "不可做该项目";
            }

            if (leixing == "加钟")
            {
                //加钟
                #region
                sql = "select * from renyuan where 工号='" + jishigonghao + "'";
                sql = sql + " and 类型='技师'";
                ds = paizhong.mssql.exeDs(sql, "renyuan");
                if (ds.Tables[0].Rows[0]["技师状态"].ToString() != "进行中")
                {
                    return "技师状态不是进行中";
                }

                sql = "select * from shoupai where 手牌号='" + kerenshoupaihao + "'";
                sql = sql + " and 流水号<>''";
                ds = paizhong.mssql.exeDs(sql, "keren");

                kerenliushuihao = ds.Tables[0].Rows[0]["流水号"].ToString();

                sql = "select * from xiaofeixiangmu where 完成情况='进行中' and 客人手牌号='";
                sql = sql + kerenshoupaihao + "'";
                sql = sql + " and 技师工号='" + jishigonghao + "'";
                ds = paizhong.mssql.exeDs(sql, "xiaofeixiangmu");
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return "技师没有为客人做第一个项目";
                }

                if (GlobalConfig.val(ds.Tables[0].Rows[0]["剩余时长"].ToString()) > 0)
                {
                    string s = "加钟失败：还有" + ds.Tables[0].Rows[0]["剩余时长"].ToString() + "分钟";
                    return s;
                }

                sql = "select * from fuwuxiangmu where 服务项目名称='" + xiangmu + "'";
                ds = paizhong.mssql.exeDs(sql, "fuwuxiangmubianhao");

                sql = "insert into xiaofeixiangmu (流水号,客人手牌号,日期,服务类别名称,服务项目编号,服务项目名称,售价,售价2,进价,促销价,会员价,提成,点钟提成,";
                sql = sql + "加钟提成,时长,剩余时长,促销时段1,促销时段2,结账单价,结账金额,上钟时间,技师工号,上钟方式,点钟,加钟,轮钟,";
                sql = sql + "呼叫,结账,交班,备注,完成情况,点评,会员卡,结账流水号,交班流水号,房号,数量,单位,是商品,服务属性,";
                sql = sql + "发牌默认,减发牌默认,减房费,允许超过分钟,超过多少分钟,每超时价格,";
                sql = sql + "操作员) values ('";
                sql = sql + kerenliushuihao + "','";
                sql = sql + kerenshoupaihao + "','";
                sql = sql + dt.ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["服务类别名称"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["服务项目编号"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["服务项目名称"].ToString() + "',";
                sql = sql + ds.Tables[0].Rows[0]["售价"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["售价2"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["进价"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["促销价"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["会员价"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["提成"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["点钟提成"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["加钟提成"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["时长"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["时长"].ToString() + ",'";
                sql = sql + ds.Tables[0].Rows[0]["促销时段1"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["促销时段2"].ToString() + "',";
                sql = sql + "0" + ",";
                sql = sql + "0" + ",'";
                sql = sql + dt.ToString() + "','";
                sql = sql + jishigonghao + "','";
                sql = sql + "技师上钟" + "','";
                sql = sql + "否" + "','";
                sql = sql + "是" + "','";
                sql = sql + "否" + "','";
                sql = sql + "否" + "','";
                sql = sql + "否" + "','";
                sql = sql + "否" + "','";
                sql = sql + "" + "','";    //备注
                sql = sql + "进行中" + "','";
                sql = sql + "" + "','";
                sql = sql + "" + "','";
                sql = sql + "" + "','";
                sql = sql + "" + "','";
                sql = sql + fanghao + "',1,'";
                sql = sql + ds.Tables[0].Rows[0]["单位"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["是商品"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["服务属性"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["发牌默认"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["减发牌默认"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["减房费"].ToString() + "',";
                sql = sql + GlobalConfig.val(Convert.ToString(ds.Tables[0].Rows[0]["允许超过分钟"])).ToString() + ",";
                sql = sql + GlobalConfig.val(Convert.ToString(ds.Tables[0].Rows[0]["超过多少分钟"])).ToString() + ",";
                sql = sql + GlobalConfig.val(Convert.ToString(ds.Tables[0].Rows[0]["每超时价格"])).ToString() + ",'";
                sql = sql + jishigonghao + "')";
                paizhong.mssql.exesqlCmd(sql);
				xiangmushichang = ds.Tables[0].Rows[0]["时长"].ToString();

                //string tmp = "房号:" + fanghao + " 项目:" + xiangmu
                    //+ " 客人:" + kerenshoupaihao + " 技师:" + jishigonghao;
                //MyClass.ri("技师上钟--加钟", tmp, jishigonghao);
                //return "加钟成功";
                #endregion
            }


            if ((leixing == "点钟") || (leixing == "轮钟"))
            {
                //点钟  轮钟
                #region
                sql = "select * from renyuan where 工号='" + jishigonghao + "'";
                sql = sql + " and 类型='技师'";
                ds = paizhong.mssql.exeDs(sql, "renyuan");
                if (ds.Tables[0].Rows[0]["技师状态"].ToString() == "进行中")
                {
                    return "技师正在上钟，请先下钟";
                }

                string xingbie = ds.Tables[0].Rows[0]["性别"].ToString();
                sql = "select * from shoupai where 手牌号='" + kerenshoupaihao + "'";
                sql = sql + " and 流水号<>''";
                ds = paizhong.mssql.exeDs(sql, "keren");

                kerenliushuihao = ds.Tables[0].Rows[0]["流水号"].ToString();

                sql = "delete from xiaofeixiangmu where 流水号='" + kerenliushuihao + "'";
                sql = sql + " and 客人手牌号='" + kerenshoupaihao + "'";
                sql = sql + " and 服务项目名称=''";
                sql = sql + " and 技师工号='" + jishigonghao + "'";
                paizhong.mssql.exesqlCmd(sql);

                sql = "select * from fuwuxiangmu where 服务项目名称='" + xiangmu + "'";
                ds = paizhong.mssql.exeDs(sql, "fuwuxiangmubianhao");
                string fuwushuxing = ds.Tables[0].Rows[0]["服务属性"].ToString();

                sql = "insert into xiaofeixiangmu (流水号,客人手牌号,日期,服务类别名称,服务项目编号,服务项目名称,售价,售价2,进价,促销价,会员价,提成,点钟提成,";
                sql = sql + "加钟提成,时长,剩余时长,促销时段1,促销时段2,结账单价,结账金额,上钟时间,技师工号,上钟方式,选钟,点钟,加钟,轮钟,";
                sql = sql + "呼叫,结账,交班,备注,完成情况,点评,会员卡,结账流水号,交班流水号,房号,数量,单位,是商品,服务属性,";
                sql = sql + "发牌默认,减发牌默认,减房费,允许超过分钟,超过多少分钟,每超时价格,";
                sql = sql + "操作员) values ('";
                sql = sql + kerenliushuihao + "','";
                sql = sql + kerenshoupaihao + "','";
                sql = sql + dt.ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["服务类别名称"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["服务项目编号"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["服务项目名称"].ToString() + "',";
                sql = sql + ds.Tables[0].Rows[0]["售价"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["售价2"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["进价"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["促销价"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["会员价"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["提成"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["点钟提成"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["加钟提成"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["时长"].ToString() + ",";
                sql = sql + ds.Tables[0].Rows[0]["时长"].ToString() + ",'";
                sql = sql + ds.Tables[0].Rows[0]["促销时段1"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["促销时段2"].ToString() + "',";
                sql = sql + "0" + ",";
                sql = sql + "0" + ",'";
                sql = sql + dt.ToString() + "','";
                sql = sql + jishigonghao + "','";
                sql = sql + "技师上钟" + "','";
                sql = sql + "否" + "','";
                if (leixing == "点钟")
                {
                    sql = sql + "是" + "','";
                }
                else
                {
                    sql = sql + "否" + "','";
                }

                sql = sql + "否" + "','";
                if (leixing == "轮钟")
                {
                    sql = sql + "是" + "','";
                }
                else
                {
                    sql = sql + "否" + "','";
                }
                sql = sql + "否" + "','";
                sql = sql + "否" + "','";
                sql = sql + "否" + "','";
                sql = sql + "" + "','";    //备注
                sql = sql + "进行中" + "','";
                sql = sql + "" + "','";
                sql = sql + "" + "','";
                sql = sql + "" + "','";
                sql = sql + "" + "','";
                sql = sql + fanghao + "',1,'";
                sql = sql + ds.Tables[0].Rows[0]["单位"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["是商品"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["服务属性"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["发牌默认"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["减发牌默认"].ToString() + "','";
                sql = sql + ds.Tables[0].Rows[0]["减房费"].ToString() + "',";
                sql = sql + GlobalConfig.val(Convert.ToString(ds.Tables[0].Rows[0]["允许超过分钟"])).ToString() + ",";
                sql = sql + GlobalConfig.val(Convert.ToString(ds.Tables[0].Rows[0]["超过多少分钟"])).ToString() + ",";
                sql = sql + GlobalConfig.val(Convert.ToString(ds.Tables[0].Rows[0]["每超时价格"])).ToString() + ",'";
                sql = sql + jishigonghao + "')";
                paizhong.mssql.exesqlCmd(sql);
				xiangmushichang = ds.Tables[0].Rows[0]["时长"].ToString();

                sql = "update shoupai set 房号='" + room_no + "'";
                sql = sql + " where 手牌号='" + guest_no + "'";
                paizhong.mssql.exesqlCmd(sql);

                string shuaiweimoshi = paizhong.GetShuaiWeiMoShi();
                if (shuaiweimoshi == "上钟甩尾")
                {
                    if (leixing == "轮钟")
                    {
                        string CurBanCi = paizhong.GetCurBanCi();
                        paizhong.yiwei(CurBanCi, jishigonghao, fuwushuxing, xingbie);
                    }
                }

                //string tmp = "房号:" + fanghao + " 项目:" + xiangmu
                    //+ " 客人:" + kerenshoupaihao + " 技师:" + jishigonghao;
                //MyClass.ri("技师上钟--" + leixing, tmp, jishigonghao);
                
                #endregion
            }

            sql = "update renyuan set 流水号='" + kerenliushuihao + "'";
            sql = sql + ",当前项目='" + xiangmu + "'";
            sql = sql + ",房号='" + fanghao + "'";
            sql = sql + ",技师状态='进行中'";
            sql = sql + " where 工号='" + jishigonghao + "'";
            paizhong.mssql.exesqlCmd(sql);
            return leixing + "成功 "+xiangmushichang;
    	}

		[WebMethod]
		//上钟查询， 自主上钟时，查询状态，自动填写房号等信息
		public string ShangzhongChaxun(string jishi_id)
		{
			string sql = "";
            sql = "select * from xiaofeixiangmu where";
            sql = sql + " 技师工号='" + jishi_id + "'";
            sql = sql + " and (完成情况='等待中' or 完成情况='暂停')";
            DataSet ds = new DataSet();
            ds = paizhong.mssql.exeDs(sql,"xiaofeixiangmu");
			String resJson = "{";
            if (ds.Tables[0].Rows.Count == 0)
            {
                return " ";
            }
            if (ds.Tables[0].Rows.Count > 0)
            {
                resJson += "\"客人手牌号\":\"";
				resJson += ds.Tables[0].Rows[0]["客人手牌号"].ToString();
				resJson += "\",\"服务项目名称\":\"";
				resJson += ds.Tables[0].Rows[0]["服务项目名称"].ToString();
				resJson += "\",\"点钟\":\"";
				resJson += ds.Tables[0].Rows[0]["点钟"].ToString();
				resJson += "\",\"轮钟\":\"";
				resJson += ds.Tables[0].Rows[0]["轮钟"].ToString();
				resJson += "\",\"房号\":\"";
				resJson += ds.Tables[0].Rows[0]["房号"].ToString();
                resJson += "\"}";
            }
			return resJson;
		}
		
		[WebMethod]
		public string Jiazhong(string jishi_id, string guest_no, string service_name, string room_no)
		{
            return Shangzhong_Zizhu(jishi_id, guest_no, service_name,
                room_no, "加钟");
		}

		[WebMethod]
		public string Xiazhong(string jishi_id, string btiqian)
		{
	        if (jishi_id=="")
	        {
	            return "请输入技师ID";
	        }

	        string sql = "";
	        sql = "select * from renyuan where 工号='" + jishi_id + "'";
	        sql = sql + " or 芯片号='" + jishi_id + "'";
	        sql = sql + " and 类型='技师'";

	        DataSet ds = new DataSet();
	        MSSQL mssql = new MSSQL();
	        string dt = mssql.getSvrDateTime().ToString("yyyy-MM-dd HH:mm:ss");

	        ds = mssql.exeDs(sql, "shoupai");
	        if (ds.Tables[0].Rows.Count==0)
	        {
	            mssql.close();
	            return "手牌号不在系统内";
	        }

	        string jishigonghao = ds.Tables[0].Rows[0]["工号"].ToString();
	        string kerenliushuihao = ds.Tables[0].Rows[0]["流水号"].ToString();
	        if (kerenliushuihao != "")
	        {
	            sql = "select * from xiaofeixiangmu where";
	            sql = sql + " 完成情况='进行中'";
	            sql = sql + " and 技师工号='" + jishigonghao + "'";
	            ds = mssql.exeDs(sql, "xiaofeixiangmu");
	            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
	            {
	                string id = ds.Tables[0].Rows[i]["id"].ToString();
	                string xiangmu = ds.Tables[0].Rows[0]["服务项目名称"].ToString();

	                if (GlobalConfig.val(ds.Tables[0].Rows[i]["剩余时长"].ToString()) > 0)
	                {
	                	if(btiqian == "否")
                		{
		                    mssql.close();
		                    return xiangmu + "下钟时间未到," + GlobalConfig.val(ds.Tables[0].Rows[i]["剩余时长"].ToString());                       //不到时间可以下钟
                		}
					}

	                sql = "update renyuan set 流水号='',当前项目='',技师状态='空闲',房号=''";
	                sql = sql + " where 工号='" + jishigonghao + "'";
	                mssql.exesqlCmd(sql);

	                sql = "update xiaofeixiangmu set 完成情况='已完成'";
	                sql = sql + ",下钟时间='" + dt + "'";
	                sql = sql + " where id=" + id;
	                mssql.exesqlCmd(sql);

	                sql = "insert into 下钟打印 (pid) values ('" + id + "')";
	                mssql.exesqlCmd(sql);

	                //string tmp = " 项目:" + xiangmu + " 客人:" + kerenliushuihao + " 技师:" + jishigonghao;
	                //MyClass.ri("下钟", tmp,jishigonghao);
	            }

	            mssql.close();
				return "下钟成功";
	        }
	        else
	        {
	            mssql.close();
	            return "手牌未上钟";
	        }
	    }

		[WebMethod]
		public string GetGuestItem(string guest_id)
		{
            if (guest_id == "")
            {
                return "手牌号不能为空";
            }
            string sql = "";
            MSSQL mssql = new MSSQL();
            DataSet ds = new DataSet();

            sql = "select * from shoupai where (手牌号='" + guest_id + "' or 芯片号='" + guest_id + "')";
            ds = mssql.exeDs(sql, "shoupai");
            if (ds.Tables[0].Rows.Count == 0)
            {
                mssql.close();
                return "该手牌不在系统内";
            }
            string kerenshoupaihao = ds.Tables[0].Rows[0]["手牌号"].ToString();

            //dataGridView1.Rows.Clear();
            sql = "select * from xiaofeixiangmu where 客人手牌号='" + kerenshoupaihao + "'";
            sql = sql + " and 完成情况='进行中' and 是商品='否' order by id desc";
            ds = mssql.exeDs(sql, "shoupai");
			mssql.close();
			if (ds.Tables[0].Rows.Count == 0)
            {
                return "没有消费项目";
            }
            else
            {
                string retStr = "[";
                for (int i=0;i< ds.Tables[0].Rows.Count; i++)
                {
                    retStr += "{";
                    for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                    {
                        retStr += "\"";
                        retStr += ds.Tables[0].Columns[j].ColumnName.ToString();
                        retStr += "\":\"";
                        retStr += ds.Tables[0].Rows[i][j].ToString();
                        retStr += "\",";
                    }
                    retStr = retStr.TrimEnd(',');
                    retStr += "},";
                }
                retStr = retStr.TrimEnd(',');
                retStr += "]";
                return retStr;
            }
        }


		[WebMethod]
		//日结统计
		public string Rijietongji(string begin_time, string end_time)
		{
			try
            {
                DateTime dt1;
				DateTime dt2;

				dt1 = Convert.ToDateTime(end_time + ":59");
	            dt2 = Convert.ToDateTime(begin_time + ":00");

                string sql = "";
                MSSQL mssql = new MSSQL();
                MSSQL2 mssql2 = new MSSQL2();
                DataSet ds = new DataSet();    //模版
                DataSet ds1 = new DataSet();   //付款方式
                DataSet ds2 = new DataSet();

                double xianjin = 0;             //会员卡充值现金
                double yinlian = 0;             //会员卡充值银联卡
                double weixin = 0;             //会员卡充值微信收
                double zhifubao = 0, qita=0;             //会员卡充值支付宝收
                

                sql = "select * from fukuanfangshi order by 顺序";
                ds1 = mssql.exeDs(sql, "fukuanfangshi");
                int hang = ds1.Tables[0].Rows.Count;
                string[] fangshi = new string[hang];
                string[] caozuo = new string[hang];
                double[] jine = new double[hang];
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                    fangshi[i] = ds1.Tables[0].Rows[i]["付款方式"].ToString();
                    caozuo[i] = ds1.Tables[0].Rows[i]["操作"].ToString();
                    jine[i] = 0;
                }

                sql = "select * from jiezhang2 where 日期>='" + dt2.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                sql = sql + " and 日期<='" + dt1.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                //if (comboBox1.Text != "全部")
                //{
                    //sql = sql + " and 操作员='" + comboBox1.Text + "'";
                //}
                sql = sql + " order by id";
                ds2 = mssql.exeDs(sql, "jiezhang2");
                for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < fangshi.Length; j++)
                    {
                        if (ds2.Tables[0].Rows[i]["付款方式"].ToString() == fangshi[j])
                        {
                            jine[j] = jine[j] + GlobalConfig.dval(ds2.Tables[0].Rows[i]["金额"].ToString());
                            break;
                        }
                    }
                }

				string retStr = "{\"日结统计\":[";

                for (int i = 0; i < fangshi.Length; i++)
                {
					retStr += "{\"项目\":\"";
					retStr += fangshi[i].ToString();
					retStr += "\",";
					retStr += "\"金额\":\"";
					retStr += jine[i].ToString();
					retStr += "\"},";
                }
				

                sql = "select * from chongzhi where 类型='充值' and 日期>='" + dt2.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                sql = sql + " and 日期<='" + dt1.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                //if (comboBox1.Text != "全部")
                //{
                    //sql = sql + " and 操作员='" + comboBox1.Text + "'";
                //}
                sql = sql + " order by id";
                ds2 = mssql2.exeDs(sql, "chongzhi");
                for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                {
                    xianjin = xianjin + GlobalConfig.dval(ds2.Tables[0].Rows[i]["实收现金"].ToString());
                    yinlian = yinlian + GlobalConfig.dval(ds2.Tables[0].Rows[i]["实收银联卡"].ToString());

                    weixin = weixin + GlobalConfig.dval(ds2.Tables[0].Rows[i]["实收微信"].ToString());
                    zhifubao = zhifubao + GlobalConfig.dval(ds2.Tables[0].Rows[i]["实收支付宝"].ToString());
                    qita = qita + GlobalConfig.dval(ds2.Tables[0].Rows[i]["实收其它"].ToString());
                }

                mssql.close();
                mssql2.close();

				retStr += "{\"项目\":\"";
				retStr += "";
				retStr += "\",";
				retStr += "\"金额\":\"";
				retStr += "";
				retStr += "\"},";

				retStr += "{\"项目\":\"";
				retStr += "会员卡充值实收现金";
				retStr += "\",";
				retStr += "\"金额\":\"";
				retStr += xianjin.ToString();
				retStr += "\"},";

                retStr += "{\"项目\":\"";
				retStr += "会员卡充值实收银联卡";
				retStr += "\",";
				retStr += "\"金额\":\"";
				retStr += yinlian.ToString();
				retStr += "\"},";

                retStr += "{\"项目\":\"";
                retStr += "会员卡充值实收微信";
                retStr += "\",";
                retStr += "\"金额\":\"";
                retStr += weixin.ToString();
                retStr += "\"},";
                retStr += "{\"项目\":\"";
                retStr += "会员卡充值实收支付宝";
                retStr += "\",";
                retStr += "\"金额\":\"";
                retStr += zhifubao.ToString();
                retStr += "\"},";
                retStr += "{\"项目\":\"";
				retStr += "会员卡充值实收银其他";
				retStr += "\",";
				retStr += "\"金额\":\"";
				retStr += qita.ToString();
				retStr += "\"},";

                retStr += "{\"项目\":\"";
                retStr += "合计";
                retStr += "\",";
                retStr += "\"金额\":\"";
                retStr += (xianjin + yinlian + weixin + zhifubao + qita).ToString();
                retStr += "\"},";

				/*
                sql = "select sum(cast(总消费金额 as decimal(18,2))) as 总消费金额,sum(cast(打折后金额 as decimal(18,2))) as 打折后金额,sum(cast(房费 as int)) as 房费  from jiezhang1 where 日期>='" + dt1.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                sql = sql + " and 日期<='" + dt2.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                if (comboBox1.Text != "全部")
                {
                    sql = sql + " and 操作员='" + comboBox1.Text + "'";
                }
                ds2 = mssql.exeDs(sql, "jiezhang1");

                jj = dataGridView1.Rows.Add();
                dataGridView1.Rows[jj].Cells["项目"].Value = "";
                dataGridView1.Rows[jj].Cells["金额"].Value = "";

                jj = dataGridView1.Rows.Add();
                dataGridView1.Rows[jj].Cells["项目"].Value = "项目总消费金额";
                dataGridView1.Rows[jj].Cells["金额"].Value = ds2.Tables[0].Rows[0]["总消费金额"].ToString();

                jj = dataGridView1.Rows.Add();
                dataGridView1.Rows[jj].Cells["项目"].Value = "房费";
                dataGridView1.Rows[jj].Cells["金额"].Value = ds2.Tables[0].Rows[0]["房费"].ToString();

                jj = dataGridView1.Rows.Add();
                dataGridView1.Rows[jj].Cells["项目"].Value = "打折后金额";
                dataGridView1.Rows[jj].Cells["金额"].Value = ds2.Tables[0].Rows[0]["打折后金额"].ToString();
                */
				retStr = retStr.TrimEnd(',');
				retStr += "]}";
				return retStr;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

		}
		

/*
		[WebMethod]
		//日结统计
		public string Rijietongji(string begin_time, string end_time)
		{
            DateTime dt1;
			DateTime dt2;

			dt1 = Convert.ToDateTime(end_time + ":59");
            dt2 = Convert.ToDateTime(begin_time + ":00");


			MSSQL mssql = new MSSQL();
            string sql = "select * from rijie where 时间>='" + dt2.ToString() + "'";
            sql = sql + " and 时间<='" + dt1.ToString() + "'";
            sql = sql + " order by 付款方式 desc, 时间";
            DataSet ds1 = new DataSet();  
            ds1 = mssql.exeDs(sql,"rijie");


			string JsonStr = "";
			string title = "";
			//JsonStr+="{\"项目业绩\":[";
			//bool bchangetitle = false;

            if (0 == ds1.Tables[0].Rows.Count)
            {
				mssql.close();
				return " ";
            }
            else
            {
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {
                	if(title == "") {
						title = ds1.Tables[0].Rows[i]["付款方式"].ToString();
						JsonStr+="{\""+title+"\":[";
                	}
                	else if(title != ds1.Tables[0].Rows[i]["付款方式"].ToString()) {
						title = ds1.Tables[0].Rows[i]["付款方式"].ToString();
						JsonStr = JsonStr.TrimEnd(',');
						JsonStr += "],";
						JsonStr+="\""+title+"\":[";
                	}

                    JsonStr += "{\"日结流水号\":\"";
                    JsonStr += ds1.Tables[0].Rows[i]["日结流水号"].ToString();
                    JsonStr += "\",\"时间\":\"";
                    JsonStr += ds1.Tables[0].Rows[i]["时间"].ToString();
                    JsonStr += "\",\"付款方式\":\"";
                    JsonStr += ds1.Tables[0].Rows[i]["付款方式"].ToString();
                    JsonStr += "\",\"金额\":\"";
                    JsonStr += ds1.Tables[0].Rows[i]["金额"].ToString();
                    JsonStr += "\",\"操作人\":\"";
                    JsonStr += ds1.Tables[0].Rows[i]["操作人"].ToString();
                    JsonStr += "\"},";

                }
				JsonStr = JsonStr.TrimEnd(',');
				JsonStr += "]}";
            }
 
            mssql.close();
			return JsonStr;
		}
*/
		
		[WebMethod]
		public string Jishiyeji(string jishi_id, string begin_time, string end_time)
		{
			if (jishi_id == "")
            {
                return "请输入技师ID";
            }

            DateTime dt;
            DateTime dt1;
			DateTime dt2;

            string sql = "";

            sql = "select * from renyuan where 芯片号='" + jishi_id + "'" + " or 工号='" + jishi_id + "'";
            sql = sql + " and 类型='技师'";
            DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();
            dt = mssql.getSvrDate();
            ds = mssql.exeDs(sql, "shoupai");

            if (ds.Tables[0].Rows.Count > 0)
            {
                //textBox1.Text = ds.Tables[0].Rows[0]["工号"].ToString();
                //label2.Text = "验证成功";
                mssql.close();
                dt1 = Convert.ToDateTime(end_time + " 23:59:59");
                dt2 = Convert.ToDateTime(begin_time + " 00:00:00");
            }
            else
            {
                mssql.close();
                return "手牌无效";
            }

			mssql = new MSSQL();
            sql="";
            ds = new DataSet();
            sql = "select * from xitong";
            ds = mssql.exeDs(sql,"xitong");

            sql = "select * from xiaofeixiangmu where 日期>='" + dt2.ToString() + "'";
            sql = sql + " and 日期<='" + dt1.ToString() + "'";
            //sql = sql + " and 结账='是' and 完成情况='已完成'";
            sql = sql + " and 是商品='否'";
            sql=sql + " and 技师工号='" + jishi_id + "'";
            sql = sql + " order by id desc";
            DataSet ds1 = new DataSet();   //消费项目
            DataSet ds2 = new DataSet();   //消费商品

            ds1 = mssql.exeDs(sql,"xiaofeifuwu");



            sql = "select * from xiaofeixiangmu where 日期>='" + dt2.ToString() + "'";
            sql = sql + " and 日期<='" + dt1.ToString() + "'";
            //sql = sql + " and 结账='是'";
            sql = sql + " and 是商品='是'";
            sql = sql + " and 销售员='" + jishi_id + "'";
            sql = sql + " order by id desc";
            ds2 = mssql.exeDs(sql,"xiaofei");


            double xiangmushu=0;
            double xiangmuticheng=0;
            double dianzhongshu=0;
            double dianzhongticheng=0;
            double jiazhongshu=0;
            double jiazhongticheng=0;
            double shangpinshuliang = 0;  //商品数量
            double shangpinticheng=0;
            int chidaofenzhong=0;
            double chongzhi = 0;

			string JsonStr = "";
			JsonStr+="{\"项目业绩\":[";

            if (0 == ds1.Tables[0].Rows.Count)
            {
                JsonStr += "{\"日期\":\"";
                JsonStr += " ";
                JsonStr += "\",\"服务项目名称\":\"";
                JsonStr += " ";
                JsonStr += "\",\"数量\":\"";
                JsonStr += "0";
                JsonStr += "\",\"提成\":\"";
                JsonStr += "0";
                JsonStr += "\",\"点钟提成\":\"";
                JsonStr += "0";
                JsonStr += "\",\"加钟提成\":\"";
                JsonStr += "0";
                JsonStr += "\",\"点评\":\"";
                JsonStr += " ";
                JsonStr += "\"}],";
            }
            else
            {
                for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
                {

                    double shuliang = GlobalConfig.dval(ds1.Tables[0].Rows[i]["数量"].ToString());
                    JsonStr += "{\"日期\":\"";
                    JsonStr += ds1.Tables[0].Rows[i]["日期"].ToString();
                    JsonStr += "\",\"服务项目名称\":\"";
                    JsonStr += ds1.Tables[0].Rows[i]["服务项目名称"].ToString();
                    JsonStr += "\",\"数量\":\"";
                    JsonStr += ds1.Tables[0].Rows[i]["数量"].ToString();
                    double value = GlobalConfig.dval(ds1.Tables[0].Rows[i]["提成"].ToString()) * shuliang;
                    JsonStr += "\",\"提成\":\"";
                    JsonStr += value.ToString();
                    if (ds1.Tables[0].Rows[i]["点钟"].ToString() == "是")
                    {
                        if (ds.Tables[0].Rows[0]["小项上钟排除"].ToString() == "是")
                        {
                            if (ds1.Tables[0].Rows[i]["服务属性"].ToString() != "小项类")
                            {
                                value = GlobalConfig.dval(ds1.Tables[0].Rows[i]["点钟提成"].ToString()) * shuliang;
                                JsonStr += "\",\"点钟提成\":\"";
                                JsonStr += value.ToString();
                            }
                            else
                            {
								JsonStr += "\",\"点钟提成\":\"";
								JsonStr += "0";
                            }
                        }
                        else
                        {
                            value = GlobalConfig.dval(ds1.Tables[0].Rows[i]["点钟提成"].ToString()) * shuliang;
                            JsonStr += "\",\"点钟提成\":\"";
                            JsonStr += value.ToString();
                        }

                    }
                    else
                    {
                        JsonStr += "\",\"点钟提成\":\"";
                        JsonStr += "0";
                    }

                    if (ds1.Tables[0].Rows[i]["加钟"].ToString() == "是")
                    {
                        if (ds.Tables[0].Rows[0]["小项上钟排除"].ToString() == "是")
                        {
                            if (ds1.Tables[0].Rows[i]["服务属性"].ToString() != "小项类")
                            {
                                value = GlobalConfig.dval(ds1.Tables[0].Rows[i]["加钟提成"].ToString()) * shuliang;
                                JsonStr += "\",\"加钟提成\":\"";
                                JsonStr += value.ToString();
                            }
                            else
                            {
								JsonStr += "\",\"加钟提成\":\"";
								JsonStr += "0";
                            }
                        }
                        else
                        {
                            value = GlobalConfig.dval(ds1.Tables[0].Rows[i]["加钟提成"].ToString()) * shuliang;
                            JsonStr += "\",\"加钟提成\":\"";
                            JsonStr += value.ToString();
                        }
                    }
                    else
                    {
                        JsonStr += "\",\"加钟提成\":\"";
                        JsonStr += "0";
                    }
                    JsonStr += "\",\"点评\":\"";
                    JsonStr += ds1.Tables[0].Rows[i]["点评"].ToString();
                    JsonStr += "\"},";

                    ///////////////
                    xiangmushu = xiangmushu + shuliang;
                    xiangmuticheng = xiangmuticheng + GlobalConfig.dval(ds1.Tables[0].Rows[i]["提成"].ToString()) * shuliang;
                    if (ds1.Tables[0].Rows[i]["点钟"].ToString() == "是")
                    {
                        if ((ds.Tables[0].Rows[0]["小项上钟排除"].ToString() == "是")
                                && (ds1.Tables[0].Rows[i]["服务属性"].ToString() != "小项类"))
                        {
                            dianzhongshu = dianzhongshu + shuliang;
                            dianzhongticheng = dianzhongticheng + GlobalConfig.dval(ds1.Tables[0].Rows[i]["点钟提成"].ToString()) * shuliang;
                        }
                    }
                    if (ds1.Tables[0].Rows[i]["加钟"].ToString() == "是")
                    {
                        if ((ds.Tables[0].Rows[0]["小项上钟排除"].ToString() == "是")
                                && (ds1.Tables[0].Rows[i]["服务属性"].ToString() != "小项类"))
                        {
                            jiazhongshu = jiazhongshu + shuliang;
                            jiazhongticheng = jiazhongticheng + GlobalConfig.dval(ds1.Tables[0].Rows[i]["加钟提成"].ToString()) * shuliang;
                        }
                    }
                }
				JsonStr = JsonStr.TrimEnd(',');
				JsonStr += "],";
            }

			

			JsonStr+="\"商品业绩\":[";

            if (0 == ds2.Tables[0].Rows.Count)
            {
                JsonStr += "{\"日期\":\"";
                JsonStr += " ";
                JsonStr += "\",\"服务项目名称\":\"";
                JsonStr += " ";
                JsonStr += "\",\"数量\":\"";
                JsonStr += "0";
                JsonStr += "\",\"提成\":\"";
                JsonStr += "0";
                JsonStr += "\"}],";
            }
            else
            {

                for (int i = 0; i < ds2.Tables[0].Rows.Count; i++)
                {
                    JsonStr += "{\"日期\":\"";
                    JsonStr += ds2.Tables[0].Rows[i]["日期"].ToString();
                    JsonStr += "\",\"服务项目名称\":\"";
                    JsonStr += ds2.Tables[0].Rows[i]["服务项目名称"].ToString();
                    JsonStr += "\",\"数量\":\"";
                    JsonStr += ds2.Tables[0].Rows[i]["数量"].ToString();
                    double value = GlobalConfig.dval(ds2.Tables[0].Rows[i]["提成"].ToString()) * GlobalConfig.dval(ds2.Tables[0].Rows[i]["数量"].ToString());
                    JsonStr += "\",\"提成\":\"";
                    JsonStr += value.ToString();
                    JsonStr += "\"},";


                    shangpinshuliang = shangpinshuliang + GlobalConfig.dval(ds2.Tables[0].Rows[i]["数量"].ToString());
                    shangpinticheng = shangpinticheng + GlobalConfig.dval(ds2.Tables[0].Rows[i]["提成"].ToString())
                         * GlobalConfig.dval(ds2.Tables[0].Rows[i]["数量"].ToString());
                }
				JsonStr = JsonStr.TrimEnd(',');
				JsonStr += "],";
            }
			

			/*
            if (MyClass.qiyongchidao == "是")
            {
                string shangbanshijian = Convert.ToDateTime(ds.Tables[0].Rows[0]["上班时间"]).ToString("HH:mm:ss");
                for (int i = 0; i < ds3.Tables[0].Rows.Count; i++)   //迟到
                {
                    string shijishijian = Convert.ToDateTime(ds3.Tables[0].Rows[0]["时间"]).ToString("HH:mm:ss");
                    DateTime shijian1 = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + shangbanshijian);
                    DateTime shijian2 = Convert.ToDateTime(dt.ToString("yyyy-MM-dd") + " " + shijishijian);
                    TimeSpan ts = shijian2 - shijian1;
                    int xiaoshi = ts.Hours;
                    int fenzhong = ts.Minutes;
                    if ((xiaoshi > 0) || (fenzhong > 0))
                    {
                        chidaofenzhong = chidaofenzhong + xiaoshi * 60 + fenzhong;
                    }
                }
            }
            

            for (int i = 0; i < ds4.Tables[0].Rows.Count; i++)   //充值
            {
                chongzhi = chongzhi + MyClass.dval(ds4.Tables[0].Rows[i]["提成"].ToString());
            }
            */

			JsonStr+="\"总业绩\":[{";

            JsonStr += "\"项目数\":\"";
			JsonStr += xiangmushu;
			JsonStr += "\",\"项目提成\":\"";
			JsonStr += xiangmuticheng;
			JsonStr += "\",\"点钟数\":\"";
			JsonStr += dianzhongshu;
			JsonStr += "\",\"点钟提成\":\"";
			JsonStr += dianzhongticheng;
			JsonStr += "\",\"加钟数\":\"";
			JsonStr += jiazhongshu;
			JsonStr += "\",\"加钟提成\":\"";
			JsonStr += jiazhongticheng;


			JsonStr += "\",\"商品数\":\"";
			JsonStr += shangpinshuliang;
			JsonStr += "\",\"商品提成\":\"";
			JsonStr += shangpinticheng;

            double heji = xiangmuticheng + dianzhongticheng + jiazhongticheng + shangpinticheng + chongzhi - chidaofenzhong ;

			JsonStr += "\",\"合计\":\"";
			JsonStr += heji.ToString();
			JsonStr += "\"}]}";

            mssql.close();
			return JsonStr;
		}

		
		[WebMethod]
		//prepare for 更换项目
        public string ChaXunXiaofeiXiangmu_byGuestID(string guest_id, string jishi_id)
        {
            string sql = "'";

			sql = "select * from xiaofeixiangmu where 客人手牌号='" + guest_id + "'";
            sql = sql + " and 技师工号='" + jishi_id + "'";
            sql = sql + " and 完成情况='进行中'";
            sql = sql + " order by id desc";

            
            DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();
            ds = mssql.exeDs(sql, "xiaofeixiangmu");
            mssql.close();

            if (ds.Tables[0].Rows.Count > 0)
            {
                mssql.close();
                return ds.Tables[0].Rows[0]["服务项目名称"].ToString();
            }
            else
            {
                mssql.close();
				return "该手牌没有进行中项目";
            }
        }
		

		[WebMethod]
		public string Huanxiangmu(string jishi_id, string guest_id, string jiuxiangmu, string newxiangmu)
		{
			if (newxiangmu == "")
            {
                return "请输入服务项目";
            }

            if (jiuxiangmu == "")
            {
                return "没有原项目";
            }

            DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();
            string sql = "";
            sql = "select * from xiaofeixiangmu where 客人手牌号='" + guest_id + "'";
            sql = sql + " and 技师工号='" + jishi_id + "'";
            sql = sql + " and 完成情况='进行中'";
            sql = sql + " order by id desc";
            ds = mssql.exeDs(sql, "xiaofeixiangmu");

            if (ds.Tables[0].Rows.Count == 0)
            {
                mssql.close();
				return "技师没有为该客人服务";
            }

            string id = ds.Tables[0].Rows[0]["id"].ToString();
            string jishigonghao = jishi_id;
            string oldxiangmu = ds.Tables[0].Rows[0]["服务项目名称"].ToString();
			int yiyongshichang = GlobalConfig.val(ds.Tables[0].Rows[0]["时长"].ToString()) - GlobalConfig.val(ds.Tables[0].Rows[0]["剩余时长"].ToString());

            sql = "select * from fuwuxiangmu where 服务项目名称='" + newxiangmu + "'";
            ds = mssql.exeDs(sql, "fuwuxiangmu");

            sql = "update renyuan set 当前项目='" + ds.Tables[0].Rows[0]["服务项目名称"].ToString() + "'";
            sql = sql + " where 工号='" + jishigonghao + "'";
            mssql.exesqlCmd(sql);

            string beizhu = " " + oldxiangmu + " 换成 " + newxiangmu;

            sql = "update xiaofeixiangmu set 服务类别名称='" + ds.Tables[0].Rows[0]["服务类别名称"].ToString() + "'";
            sql = sql + ",服务项目编号='" + ds.Tables[0].Rows[0]["服务项目编号"].ToString() + "'";
            sql = sql + ",服务项目名称='" + ds.Tables[0].Rows[0]["服务项目名称"].ToString() + "'";
            sql = sql + ",促销时段1='" + ds.Tables[0].Rows[0]["促销时段1"].ToString() + "'";
            sql = sql + ",促销时段2='" + ds.Tables[0].Rows[0]["促销时段2"].ToString() + "'";
            sql = sql + ",备注=备注+'" + beizhu + "'";
            sql = sql + ",售价=" + ds.Tables[0].Rows[0]["售价"].ToString();
            sql = sql + ",进价=" + ds.Tables[0].Rows[0]["进价"].ToString();
            sql = sql + ",会员价=" + ds.Tables[0].Rows[0]["会员价"].ToString();
            sql = sql + ",促销价=" + ds.Tables[0].Rows[0]["促销价"].ToString();
            sql = sql + ",提成=" + ds.Tables[0].Rows[0]["提成"].ToString();
            sql = sql + ",点钟提成=" + ds.Tables[0].Rows[0]["点钟提成"].ToString();
            sql = sql + ",加钟提成=" + ds.Tables[0].Rows[0]["加钟提成"].ToString();
            sql = sql + ",时长=" + ds.Tables[0].Rows[0]["时长"].ToString();
            sql = sql + ",剩余时长=" + (GlobalConfig.val( ds.Tables[0].Rows[0]["时长"].ToString()) - yiyongshichang).ToString();
            sql = sql + ",发牌默认='" + ds.Tables[0].Rows[0]["发牌默认"].ToString() + "'";
            sql = sql + ",减发牌默认='" + ds.Tables[0].Rows[0]["减发牌默认"].ToString() + "'";
            sql = sql + ",减房费='" + ds.Tables[0].Rows[0]["减房费"].ToString() + "'";
            sql = sql + ",允许超过分钟=" + GlobalConfig.val(Convert.ToString(ds.Tables[0].Rows[0]["允许超过分钟"])).ToString() + "";
            sql = sql + ",超过多少分钟=" + GlobalConfig.val(Convert.ToString(ds.Tables[0].Rows[0]["超过多少分钟"])).ToString() + "";
            sql = sql + ",每超时价格=" + GlobalConfig.val(Convert.ToString(ds.Tables[0].Rows[0]["每超时价格"])).ToString() + "";
            sql = sql + " where id=" + id;
            mssql.exesqlCmd(sql);

            mssql.close();
			return "换项目成功";

            //MyClass.ri("换项目","技师:" + textBox1.Text + " 客人:" + textBox2.Text + " 原项目:" + oldxiangmu + " 新项目:" + textBox3.Text,MyClass.jishishoupai);
		}

		[WebMethod]
		//获取当前客人被服务状态
		public string PrepareForHuanjishi(string guest_id)
		{
			if (guest_id == "")
            {
                return "请输入手牌信息";
            }
            string sql = "";
            MSSQL mssql = new MSSQL();
            DataSet ds1 = new DataSet();

            sql = "select * from shoupai where (手牌号='" + guest_id + "' or 芯片号='" + guest_id + "')";
            ds1 = mssql.exeDs(sql, "shoupai");
            if (ds1.Tables[0].Rows.Count == 0)
            {
                mssql.close();
                return "该手牌不在系统内";
            }
            string kerenshoupaihao = ds1.Tables[0].Rows[0]["手牌号"].ToString();

            sql = "select * from xiaofeixiangmu where 客人手牌号='" + kerenshoupaihao + "'";
            sql = sql + " and 完成情况='进行中' and 是商品='否' order by id desc";
            ds1 = mssql.exeDs(sql, "shoupai");
			if(ds1.Tables[0].Rows.Count == 0) {
				return "没有服务项目";
			}
			string retStr = "[";
            for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
            {
                retStr += "{";
            	retStr += "\"id\":\"";
                retStr +=  ds1.Tables[0].Rows[i]["id"].ToString();
				retStr += "\",\"项目名称\":\"";
                retStr += ds1.Tables[0].Rows[i]["服务项目名称"].ToString();
				retStr += "\",\"技师\":\"";
                retStr +=  ds1.Tables[0].Rows[i]["技师工号"].ToString();
				retStr += "\"},";
            }
			retStr = retStr.TrimEnd(',');
			retStr += "]";

            mssql.close();
			return retStr;
		}

		[WebMethod]
		//换技师
		public string Huanjishi(string oldjishi_id, string guest_id, string newjishi_id, string item_id)
		{
            if (guest_id == "")
            {
                return "请输入客人手牌号";
            }

            if (newjishi_id == "")
            {
                return "请输入新技师工号";
            }


            if (oldjishi_id != "")
            {
                string sql = "";
                MSSQL mssql = new MSSQL();
                DataSet ds = new DataSet();
                //int Rowindex = dataGridView1.CurrentCell.RowIndex;

                string id = item_id;
                string yuanjishi = oldjishi_id;
                //string oldxiangmu = dataGridView1.Rows[Rowindex].Cells["项目名称"].Value.ToString();

                sql = "select * from renyuan where 工号='" + newjishi_id + "'";
                sql = sql + " and 类型='技师'";
                ds = mssql.exeDs(sql, "renyuan");
                if (ds.Tables[0].Rows.Count == 0)
                {
                    mssql.close();
                    return "新技师工号不存在";
                }

                if (ds.Tables[0].Rows[0]["上班状态"].ToString() == "下班")
                {
                    mssql.close();
                    return "新技师未上班";
                }

                if (ds.Tables[0].Rows[0]["停用状态"].ToString() == "是")
                {
                    mssql.close();
                    return "新技师已停用";
                }

                if (ds.Tables[0].Rows[0]["技师状态"].ToString() == "进行中")
                {
                    string tmp = "新技师状态不是空闲：在房间" + ds.Tables[0].Rows[0]["房号"].ToString() + ds.Tables[0].Rows[0]["当前项目"].ToString();
                    mssql.close();
                    return tmp;
                }

                if (ds.Tables[0].Rows[0]["技师状态"].ToString() == "等待中")
                {
                    string tmp = "新技师状态不是空闲,已派工等待中,请先取消该技师的派工";
                    mssql.close();
                    return tmp;
                }

                sql = "select * from renyuan where 工号='" + yuanjishi + "'";
                ds = mssql.exeDs(sql, "xiaofei");

                sql = "update renyuan set 房号='',技师状态='空闲',当前项目='',流水号='' where 工号='" + yuanjishi + "'";
                mssql.exesqlCmd(sql);

                string beizhu = " " + yuanjishi + " 换成 " + newjishi_id;
                sql = "update xiaofeixiangmu set 技师工号='" + newjishi_id + "'";
                sql = sql + ",备注=备注+'" + beizhu + "'";
                sql = sql + " where id=" + id;
                mssql.exesqlCmd(sql);

                sql = "update renyuan set 房号='" + ds.Tables[0].Rows[0]["房号"].ToString() + "'";
                sql = sql + ",流水号='" + ds.Tables[0].Rows[0]["流水号"].ToString() + "'";
                sql = sql + ",当前项目='" + ds.Tables[0].Rows[0]["当前项目"].ToString() + "'";
                sql = sql + ",技师状态='进行中'";
                sql = sql + " where 工号='" + newjishi_id + "'";
                mssql.exesqlCmd(sql);
                mssql.close();

                //MyClass.ri("换技师", "手牌号:" + textBox1.Text + " 原技师:" + yuanjishi + " 新技师:" + newjishi_id,MyClass.gonghao);
				return "换技师成功";
            } else
            {
                return "请输入原技师工号";
            }
        }

		[WebMethod]
		public string AllXiaofeichaxun(string guest_id)
		{

			if (guest_id == "")
            {
                return "请输入客人ID";
            }
            string sql = "";
            DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();

            sql = "select * from shoupai a,xiaofeixiangmu b where a.流水号=b.流水号";
            sql = sql + " and a.手牌号='" + guest_id + "'";
            sql = sql + " and (b.完成情况<>'等待中'";
            if (GlobalConfig.GetQiyongshangpinpaisongjiezhang() == "是")
            {
                sql = sql + " or b.派送='已派送')";
            }
            else
            {
                sql = sql + " or b.派送='已派送' or 派送='未派送')";
            }

            sql = sql + " order by a.手牌号";
            ds = mssql.exeDs(sql, "xiangmu");
			string JsonStr = "";
			JsonStr+="[";
            for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
            {
                
				JsonStr += "{\"客人\":\"";
				JsonStr += guest_id;
				JsonStr += "\",\"服务项目名称\":\"";
				JsonStr += ds.Tables[0].Rows[k]["服务项目名称"].ToString();
				JsonStr += "\",\"售价\":\"";
				JsonStr += ds.Tables[0].Rows[k]["售价"].ToString();
				JsonStr += "\",\"数量\":\"";
				JsonStr += ds.Tables[0].Rows[k]["数量"].ToString();
				JsonStr += "\",\"技师工号\":\"";
				JsonStr += ds.Tables[0].Rows[k]["技师工号"].ToString();
				JsonStr += "\",\"房号\":\"";
				JsonStr += ds.Tables[0].Rows[k]["房号1"].ToString();
                JsonStr += "\"},";
            }
			JsonStr = JsonStr.TrimEnd(',');
			JsonStr+="]";
			mssql.close();
			return JsonStr;
        }

		[WebMethod]
		public string QuxiaoPaigong(string buzhang_id, string guest_id)
		{
            string s = buzhang_id;
            if (s == "")
            {
                return "请输入部长ID";
            }

			if (guest_id == "")
            {
                return "请输入客人ID";
            }

            string sql = "";
           

            sql = "select * from renyuan where 芯片号='" + s + "'";
            sql = sql + " or 工号='" + s + "'";
            sql = sql + " and 芯片号<>''";
            sql = sql + " and 类型='部长'";

            DataSet ds1 = new DataSet();
            MSSQL mssql = new MSSQL();
            ds1 = mssql.exeDs(sql, "sql");
            mssql.close();

            if (ds1.Tables[0].Rows.Count > 0)
            {
            	sql = "select * from shoupai where 芯片号='" + guest_id + "'";
	            sql = sql + " or 手牌号='" + guest_id + "'";
	            sql = sql + " and 芯片号<>''";

	            DataSet ds2 = new DataSet();
	            mssql = new MSSQL();
	            ds2 = mssql.exeDs(sql, "sql");

	            if (ds2.Tables[0].Rows.Count > 0)
	            {
	                if (ds2.Tables[0].Rows[0]["发牌时间"].ToString() == "")
	                {
	                    mssql.close();
	                    return "客人手牌未发牌";
	                }

	                DataSet ds = new DataSet();
		            sql = "select * from shoupai where 手牌号='" + guest_id + "'";
		            ds = paizhong.mssql.exeDs(sql, "renyuan");
		            if (ds.Tables[0].Rows.Count == 0)
		            {
		                return "取消派工成功";
		            }

		            string kerenshoupaihao = ds.Tables[0].Rows[0]["手牌号"].ToString();

		            sql = "delete from daizhong where 客人手牌号='" + guest_id + "'";
		            int m = paizhong.mssql.exesqlCmd(sql);

		            sql = "select * from xiaofeixiangmu where 完成情况='等待中' and 客人手牌号='" + guest_id + "'";
		            sql = sql + " order by id desc";
		            ds = paizhong.mssql.exeDs(sql, "xiaofeixiangmu");

		            if (ds.Tables[0].Rows.Count == 0)
		            {
		                //如果没有已派的项目，提示删除待钟区，返回
		                return "已删除" + "0个等待项目 " + m.ToString() + "个待钟项目";
		            }

		            string shuaiweimoshi = paizhong.GetShuaiWeiMoShi();
		            string xuanzhongshuaiwei = paizhong.Getxuanzhong();

		            for (int k = 0; k < ds.Tables[0].Rows.Count; k++)
		            {
		                #region
		                sql = "insert into tui (技师,房号,客人,项目) values ('"
		                    + ds.Tables[0].Rows[k]["技师工号"].ToString() + "','"
		                    + ds.Tables[0].Rows[k]["房号"].ToString() + "','"
		                    + ds.Tables[0].Rows[k]["客人手牌号"].ToString() + "','"
		                    + ds.Tables[0].Rows[k]["服务项目名称"].ToString() + "')";
		                paizhong.mssql.exesqlCmd(sql);

		                string lunzhong = ds.Tables[0].Rows[k]["轮钟"].ToString();
		                string xuanzhong = ds.Tables[0].Rows[k]["选钟"].ToString();
		                string jishigonghao = ds.Tables[0].Rows[k]["技师工号"].ToString();
		                string fuwushuxing = ds.Tables[0].Rows[k]["服务属性"].ToString();

		                sql = "select * from renyuan where 工号='" + jishigonghao + "'";
		                DataSet jishiDS = new DataSet();
		                jishiDS = paizhong.mssql.exeDs(sql, "renyuan");
		                string xingbie = jishiDS.Tables[0].Rows[0]["性别"].ToString();
		                string jishizhuangtai = jishiDS.Tables[0].Rows[0]["技师状态"].ToString();

		                string id = ds.Tables[0].Rows[k]["id"].ToString();
		                

		                sql = "delete from xiaofeixiangmu where id=" + id;
		                int i = paizhong.mssql.exesqlCmd(sql);

		                if (jishizhuangtai == "等待中")
		                {
		                    sql = "update renyuan set 流水号='',房号='',技师状态='空闲',当前项目='',剩余时长=0";
		                    sql = sql + " where 工号='" + jishigonghao + "'";
		                    paizhong.mssql.exesqlCmd(sql);
		                }

		                if (fuwushuxing != "")
		                {
		                    if ((shuaiweimoshi == "派工甩尾") && (lunzhong == "是"))
		                    {
		                        paizhong.yishou(jishigonghao, fuwushuxing, xingbie);
		                    }

		                    if ((shuaiweimoshi == "派工甩尾") && (xuanzhong == "是") && (xuanzhongshuaiwei == "是"))
		                    {
		                        paizhong.yishou(jishigonghao, fuwushuxing, xingbie);
		                    }
		                }

		                #endregion
		            }

		            //string tmp =  " 客人:" + kerenshoupaihao 
		                //+ " 等待项目数:" + ds.Tables[0].Rows.Count.ToString() + " 待钟数:" + m.ToString();
		            //MyClass.ri("取消派工", tmp,"");

	                mssql.close();
					return "取消派工成功";

	            }
	            else
	            {
	                mssql.close();  
					return "客人手牌无效";
	            }           
            }
            else
            {
				return "部长手牌无效";
            }
		}
		
		[WebMethod]
		public string Tianjiashangpin(string jishi_id, string guest_id, string shangpin, string count)
		{
            if (jishi_id=="")
            {
                return "请刷技师手牌";
            }

            if (guest_id=="")
            {
                return "请输入客人手牌";
            }

            if (shangpin == "")
            {
                return "请选择商品";
            }

            if (count.Equals("0"))
            {
                return "数量应大于0";
            }

            string sql = "";
            DataSet ds = new DataSet();
            DataSet ds2 = new DataSet();
            MSSQL mssql = new MSSQL();

            sql = "select * from shoupai where 手牌号='" + guest_id + "'";
            ds = mssql.exeDs(sql, "renyuan");
            if (ds.Tables[0].Rows.Count > 0)
            {
                string zuming = ds.Tables[0].Rows[0]["组名"].ToString();
                if (ds.Tables[0].Rows[0]["发牌时间"].ToString() == "")
                {
                    mssql.close();
                    return "客人手牌未发牌";
                }

                string fanghao = ds.Tables[0].Rows[0]["房号"].ToString();
                string kerenshoupaihao = ds.Tables[0].Rows[0]["手牌号"].ToString();
                string liushuihao = ds.Tables[0].Rows[0]["流水号"].ToString();
                sql = "select * from fuwuxiangmu where 服务项目名称='" + shangpin + "'";
                sql = sql + " and 是商品='是'";
                ds2 = mssql.exeDs(sql, "shangpin");
                if (ds2.Tables[0].Rows.Count > 0)
                {
                    string guidstr = System.Guid.NewGuid().ToString();
                    sql = "insert into xiaofeixiangmu (日期,流水号,会员卡,客人手牌号,服务类别名称,服务项目编号,服务项目名称,单位,进价,售价,促销价,";
                    sql = sql + "会员价";
                    sql = sql + ",促销时段1,促销时段2,提成,数量,结账,交班,销售员,结账流水号,交班流水号,是商品,减库存,派送,呼叫次数,房号,技师工号,";
                    sql = sql + "发牌默认,减发牌默认,减房费,允许超过分钟,超过多少分钟,每超时价格,操作员,guidstr) values ('";
                    sql = sql + mssql.getSvrDateTime().ToString("yyyy-MM-dd HH:mm:ss") + "','";
                    sql = sql + liushuihao + "','";
                    sql = sql + "" + "','";
                    sql = sql + kerenshoupaihao + "','";
                    sql = sql + ds2.Tables[0].Rows[0]["服务类别名称"].ToString() + "','";
                    sql = sql + ds2.Tables[0].Rows[0]["服务项目编号"].ToString() + "','";
                    sql = sql + ds2.Tables[0].Rows[0]["服务项目名称"].ToString() + "','";
                    sql = sql + ds2.Tables[0].Rows[0]["单位"].ToString() + "',";
                    sql = sql + ds2.Tables[0].Rows[0]["进价"].ToString() + ",";
                    sql = sql + ds2.Tables[0].Rows[0]["售价"].ToString() + ",";
                    sql = sql + ds2.Tables[0].Rows[0]["促销价"].ToString() + ",";
                    sql = sql + ds2.Tables[0].Rows[0]["会员价"].ToString() + ",'";
                    sql = sql + ds2.Tables[0].Rows[0]["促销时段1"].ToString() + "','";
                    sql = sql + ds2.Tables[0].Rows[0]["促销时段2"].ToString() + "',";
                    sql = sql + ds2.Tables[0].Rows[0]["提成"].ToString() + ",";
                    sql = sql + GlobalConfig.val(count).ToString() + ",'";
                    sql = sql + "否" + "','";
                    sql = sql + "否" + "','";
                    sql = sql + jishi_id + "','";
                    sql = sql + "" + "','";
                    sql = sql + "" + "','";
                    sql = sql + ds2.Tables[0].Rows[0]["是商品"].ToString() + "','";
                    sql = sql + ds2.Tables[0].Rows[0]["减库存"].ToString() + "','";
                    sql = sql + "未派送" + "',0,'";
                    sql = sql + fanghao + "','";
                    sql = sql + jishi_id + "','";
                    sql = sql + ds2.Tables[0].Rows[0]["发牌默认"].ToString() + "','";
                    sql = sql + ds2.Tables[0].Rows[0]["减发牌默认"].ToString() + "','";
                    sql = sql + ds2.Tables[0].Rows[0]["减房费"].ToString() + "',";
                    sql = sql + GlobalConfig.val(ds2.Tables[0].Rows[0]["允许超过分钟"].ToString()).ToString() + ",";
                    sql = sql + GlobalConfig.val(ds2.Tables[0].Rows[0]["超过多少分钟"].ToString()).ToString() + ",";
                    sql = sql + GlobalConfig.val(ds2.Tables[0].Rows[0]["每超时价格"].ToString()).ToString() + ",'";
                    sql = sql + jishi_id + "','";
                    sql = sql + guidstr + "')";
                    mssql.exesqlCmd(sql);

                    sql = "insert into 下钟打印 (pid) values ('" + guidstr + "')";
                    mssql.exesqlCmd(sql);

                    mssql.close();

                   // MyClass.ri("点商品", "房号:" + fanghao + " 客人:" + textBox2.Text 
                        //+ " 销售人:" + textBox1.Text + " 名称:" +textBox3.Text + " 数量:" + textBox6.Text,MyClass.gonghao);

      
                    //this.Close();
                    return "操作成功";
                } else {
					mssql.close();
					return "没有改商品";
                }
            }else {
            	mssql.close();
				return "该手牌无效";
            }
        }

		[WebMethod]
		//为退商品准备
		public string ChaXunXiaofeiShangpin(string guest_id)
		{
            if (guest_id == "")
            {
                return "请输入客人手牌";
            }
            string sql = "";
            MSSQL mssql = new MSSQL();
            DataSet ds1 = new DataSet();
          
            sql = "select * from shoupai where (手牌号='" + guest_id + "' or 芯片号='" + guest_id + "')";
            ds1 = mssql.exeDs(sql, "shoupai");
            if (ds1.Tables[0].Rows.Count == 0)
            {
                mssql.close();
                return "该手牌不在系统内";
            }
            string kerenshoupaihao = ds1.Tables[0].Rows[0]["手牌号"].ToString();

            //dataGridView1.Rows.Clear();
            sql = "select * from xiaofeixiangmu where 客人手牌号='" + kerenshoupaihao + "'";
            sql = sql + " and 是商品='是'";
            sql = sql + " and 结账流水号=''";
            sql=sql + " order by id desc";
            ds1 = mssql.exeDs(sql, "shoupai");
			string JsonStr = "";
			JsonStr+="[";
            for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
            {
                JsonStr += "{\"id\":\"";
				JsonStr += ds1.Tables[0].Rows[i]["id"].ToString();
				JsonStr += "\",\"商品名称\":\"";
				JsonStr += ds1.Tables[0].Rows[i]["服务项目名称"].ToString();
				JsonStr += "\",\"数量\":\"";
				JsonStr += ds1.Tables[0].Rows[i]["数量"].ToString();
                JsonStr += "\"},";
            }
			JsonStr = JsonStr.TrimEnd(',');
			JsonStr+="]";

            mssql.close();
			return JsonStr;
        }


		[WebMethod]
		public string TuiShangpin(string guest_id, string id, string wood_name, string wood_cnt)
		{
            if (guest_id == "")
            {
                return "请输入客人手牌号";
            }

            string sql = "update xiaofeixiangmu set 数量=数量-" + GlobalConfig.val(wood_cnt);
            sql = sql + " where id=" + id;
            MSSQL mssql = new MSSQL();
            mssql.exesqlCmd(sql);
            mssql.close();

            //MyClass.ri("退商品", "客人:" + textBox1.Text + " 商品:" + xiangmu
                //+ " 原数量:" + shuliang.ToString() + " 退数量:" + textBox2.Text, MyClass.gonghao);

			return "操作成功";
            
        }

        [WebMethod]
        //为退项目准备
        public string ChaXunXiaofeiXiangmu(string jishi_id)
        {
            if (jishi_id == "")
            {
                return "请输入技师手牌";
            }
            string sql = "";
            MSSQL mssql = new MSSQL();
            DataSet ds = new DataSet();
            sql = sql = "select * from xiaofeixiangmu where 技师工号='" + jishi_id + "'";
            sql = sql + " and 完成情况='进行中'";
            sql = sql + " and 是商品='否'";
            ds = mssql.exeDs(sql, "xiaofeixiangmu");
            if (ds.Tables[0].Rows.Count == 0)
            {
                mssql.close();
                return "技师没有正在进行的项目";
            }
            else
            {
                string JsonStr = "";
                JsonStr += "[";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    JsonStr += "{\"id\":\"";
                    JsonStr += ds.Tables[0].Rows[i]["id"].ToString();
                    JsonStr += "\",\"项目名称\":\"";
                    JsonStr += ds.Tables[0].Rows[i]["服务项目名称"].ToString();
                    JsonStr += "\",\"客人\":\"";
                    JsonStr += ds.Tables[0].Rows[i]["客人手牌号"].ToString();
                    JsonStr += "\"},";
                }
                JsonStr = JsonStr.TrimEnd(',');
                JsonStr += "]";
                return JsonStr;
            }
        }

        [WebMethod]
        public string TuiXiangmu(string jishi_id, string xiangmu)
        {
            if (jishi_id == "")
            {
                return "请输入技师手牌号";
            }

            string jishi = jishi_id;
            string sql = "";
            DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();
            string liushuihao = "";

            sql = "select * from xiaofeixiangmu where";
            sql = sql + " 技师工号='" + jishi + "'";
            sql = sql + " and 服务项目名称='" + xiangmu + "'";
            sql = sql + " and 完成情况='进行中'";
            ds = mssql.exeDs(sql, "xiaofeixiangmu");
            if (ds.Tables[0].Rows.Count == 0)
            {
                mssql.close();
                return "退项目失败";
            }


            string kerenshoupai = ds.Tables[0].Rows[0]["客人手牌号"].ToString();
            string fanghao = ds.Tables[0].Rows[0]["房号"].ToString();
            liushuihao = ds.Tables[0].Rows[0]["流水号"].ToString();
            bool b = false;
            if (ds.Tables[0].Rows[0]["上钟时间"].ToString() != "")
            {
                DateTime dt1 = mssql.getSvrDateTime();
                DateTime dt2 = Convert.ToDateTime(ds.Tables[0].Rows[0]["上钟时间"]);
                int fenzhong = GlobalConfig.getfenzhong(dt1, dt2);
                if (fenzhong < GlobalConfig.GetChedanshijian())
                {
                    b = true;
                }
                else
                {
                    mssql.close();
                    return "已超过撤单时间，不能删除";
                }
            }
            else
            {
                b = true;
            }

            if (b)   //删除
            {
                sql = "delete from xiaofeixiangmu where 流水号='" + liushuihao + "'";
                sql = sql + " and 技师工号='" + jishi + "'";
                sql = sql + " and 服务项目名称='" + xiangmu + "'";
                mssql.exesqlCmd(sql);

                sql = "select * from renyuan where 工号='" + jishi + "'";
                ds = mssql.exeDs(sql, "shoupai");
                if (ds.Tables[0].Rows[0]["流水号"].ToString() == kerenshoupai)
                {
                    sql = "update renyuan set 流水号='',房号='',技师状态='空闲',当前项目=''";
                    sql = sql + " where 工号='" + ds.Tables[0].Rows[0]["工号"].ToString() + "'";
                    mssql.exesqlCmd(sql);
                }

                string tmp = "房号:" + fanghao + " 技师:" + jishi + " 客人:" + kerenshoupai + " 项目:" + xiangmu;
                //MyClass.ri("退项目", tmp, jishi);
                mssql.close();
                return "退项目成功";
            }

            mssql.close();
            return "已超过撤单时间，不能删除";

        }


		[WebMethod]
		//获取服务或商品类别
        public string GetWufuLeibei(string wufuOrshangpin)
    	{
            string sql = "";
            if (wufuOrshangpin == "1")
            {
                sql = "select distinct a.服务类别名称 from fuwuleibie a,fuwuxiangmu b where a.服务类别名称=b.服务类别名称 and b.前台显示='是' and b.是商品='否'";
            }
			else
            {
                sql = "select distinct a.服务类别名称 from fuwuleibie a,fuwuxiangmu b where a.服务类别名称=b.服务类别名称 and b.前台显示='是' and b.是商品='是' and b.售价>0";
            }

			DataSet ds = new DataSet();
            MSSQL mssql = new MSSQL();

            ds = mssql.exeDs(sql, "fuwuxiangmu");
            mssql.close();

			string retStr = "[";

            int maxrecord = ds.Tables[0].Rows.Count;
			
			for(int i=0; i<maxrecord; i++)
            {
            	retStr += "{";
            	retStr += "\"服务类别\":\"";
                retStr += ds.Tables[0].Rows[i]["服务类别名称"].ToString();
				retStr += "\"},";
            }

			retStr = retStr.TrimEnd(',');
			retStr += "]";
			return retStr;
        }

		[WebMethod]
		//获取服务类别中的所有服务项目
        public string GetXiangmuFromLeibie(string wufuleibie, string wufuOrshangpin)
    	{
			string sql = "";

            if (wufuOrshangpin == "1")
            {
                sql = "select * from fuwuxiangmu where 是商品='否' and 服务类别名称='" + wufuleibie + "'" + " order by 服务项目名称";  
            } else {
				sql = "select * from fuwuxiangmu where 是商品='是' and 前台显示='是' and 售价>0 and 服务类别名称='" + wufuleibie + "'" + "order by 服务项目名称";
            }


            MSSQL mssql = new MSSQL();
			DataSet ds = new DataSet();
            ds = mssql.exeDs(sql, "fuwuxiangmu");
            mssql.close();

			string retStr = "[";

            int maxrecord = ds.Tables[0].Rows.Count;
			
			for(int i=0; i<maxrecord; i++)
			{
				retStr += "{";
            	retStr += "\"服务项目\":\"";
				retStr += ds.Tables[0].Rows[i]["服务项目名称"].ToString() + " " + GlobalConfig.dval(ds.Tables[0].Rows[i]["售价"].ToString()).ToString("0.0");
				retStr += "\"},";
			}

            retStr = retStr.TrimEnd(',');
			retStr += "]";
			return retStr;
    	}

		[WebMethod]
		//获取服务类别中的所有服务项目
        public string XiaZhongChaXun()
    	{
			string sql = "";
            MSSQL mssql = new MSSQL();
            DataSet ds = new DataSet();
            sql = "select * from xiaofeixiangmu where 完成情况='进行中'";
            sql = sql + " order by 剩余时长";
            ds = mssql.exeDs(sql, "xiaofeixiangmu");

			string retStr = "{\"已上钟技师\":[";

			if(ds.Tables[0].Rows.Count == 0) {
				retStr += "{\"技师\":\"";
				retStr += "";
				retStr += "\",";
				retStr += "\"项目\":\"";
				retStr += "";
				retStr += "\",";
				retStr += "\"位置\":\"";
				retStr += "";
				retStr += "\",";
				retStr += "\"剩余时间\":\"";
				retStr += "";
				retStr += "\",";
				retStr += "\"点钟\":\"";
				retStr += "";
				retStr += "\"},";

			}else {
			
	            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
	            {
	                retStr += "{\"技师\":\"";
					retStr += ds.Tables[0].Rows[i]["技师工号"].ToString();
					retStr += "\",";
					retStr += "\"项目\":\"";
					retStr += ds.Tables[0].Rows[i]["服务项目名称"].ToString();
					retStr += "\",";
					retStr += "\"位置\":\"";
					retStr += ds.Tables[0].Rows[i]["房号"].ToString();
					retStr += "\",";
					retStr += "\"剩余时间\":\"";
					retStr += ds.Tables[0].Rows[i]["剩余时长"].ToString();
					retStr += "\",";
					retStr += "\"点钟\":\"";
					retStr += ds.Tables[0].Rows[i]["点钟"].ToString();
					retStr += "\"},";
	            }
			}
			retStr = retStr.TrimEnd(',');
			retStr += "],";

            sql = "select * from renyuan a";
            sql = sql + " where a.技师状态='空闲'";
            sql = sql + " and a.上班状态='上班' and a.停用状态='否'";
            sql = sql + " and 类型='技师'";
            //sql = sql + " and 性别='女'";
            sql = sql + " order by 工号";
            ds = paizhong.mssql.exeDs(sql, "paizhong");
            string  s = "";
			retStr += "\"未上钟技师\":[";
			if(ds.Tables[0].Rows.Count == 0) {
				retStr += "{\"工号\":\"";
				retStr += "";
				retStr += "\",";
				retStr += "\"姓名\":\"";
				retStr += "";
				retStr += "\",";
				retStr += "\"性别\":\"";
				retStr += "";
				retStr += "\",";
				retStr += "\"服务类别\":\"";
				retStr += "";
				retStr += "\"},";
			} else {
	            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
	            {
	                if (s != ds.Tables[0].Rows[i]["工号"].ToString())
	                {
	                    s = ds.Tables[0].Rows[i]["工号"].ToString();
	                    retStr += "{\"工号\":\"";
						retStr += ds.Tables[0].Rows[i]["工号"].ToString();
						retStr += "\",";
						retStr += "\"姓名\":\"";
						retStr += ds.Tables[0].Rows[i]["姓名"].ToString();
						retStr += "\",";
						retStr += "\"性别\":\"";
						retStr += ds.Tables[0].Rows[i]["性别"].ToString();
						retStr += "\",";
						retStr += "\"服务类别\":\"";
						retStr += ds.Tables[0].Rows[i]["服务类别"].ToString();
						retStr += "\"},";
	                }
	            }
			}
			retStr = retStr.TrimEnd(',');
			retStr += "]}";
            mssql.close();
			return retStr;
    	}

		[WebMethod]
        //选钟
        public string XuanZhong(string buzhang_id, string jishi_id, string guest_id, 
        	string service_name, string room_id)
        {
            if (buzhang_id.Equals("") || jishi_id.Equals("") || guest_id.Equals("") 
				|| room_id.Equals(""))
            {
                return "缺少必要的信息，请检查查询条件是否完整";
            }
            string sql = "select * from renyuan where 工号='" + buzhang_id + "'";
            sql = sql + " and 类型='部长'";
            try
            {
                MSSQL mysql = new MSSQL();
                DataSet ds = mysql.exeDs(sql, "renyuan");
                //mysql.close();
                if (ds.Tables[0].Rows.Count == 0)
                {
                    mysql.close();
                    return "部长手牌无效";
                }
                else
                {
                    try
                    {
                    	sql = "select * from fangjian where 房号='" + room_id + "'";
						ds = mysql.exeDs(sql, "fangjian");
						if (ds.Tables[0].Rows.Count == 0)
						{
							mysql.close();
							return "房号不是有效";
						}
						
                        sql = "select * from renyuan where 工号='" + jishi_id + "'";
                        sql = sql + " and 类型='技师'";
                        ds = mysql.exeDs(sql, "renyuan");
                        string xingbie = ds.Tables[0].Rows[0]["性别"].ToString();

                        string buzhanggonghao = buzhang_id;
                        sql = "select * from shoupai where 手牌号='" + guest_id + "'";
                        sql = sql + " and 流水号<>''";
                        ds = mysql.exeDs(sql, "keren");
                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            mysql.close();
                            return "客人手牌未领用";
                        }

                        string kerenliushuihao = ds.Tables[0].Rows[0]["流水号"].ToString();
                        string kerenshoupaihao = ds.Tables[0].Rows[0]["手牌号"].ToString();
						string yuanfanghao = ds.Tables[0].Rows[0]["房号"].ToString();
	                    if (yuanfanghao == "")
	                    {
	                        sql = "update shoupai set 房号='" + room_id + "'" + " where 手牌号='" + kerenshoupaihao + "'";
	                        mysql.exesqlCmd(sql);
	                    }
                        string fuwushuxing = "";
                        string fuwuxiangmu = "";

                        sql = "select * from fuwuxiangmu where 服务项目名称='" + service_name + "'";
                        ds = mysql.exeDs(sql, "fuwuxiangmubianhao");
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            fuwushuxing = ds.Tables[0].Rows[0]["服务属性"].ToString();
                            fuwuxiangmu = ds.Tables[0].Rows[0]["服务项目名称"].ToString();
                        }

                        DateTime dt = DateTime.Now;

                        sql = "insert into daizhong (时间,包厢,项目名称,男,女,不限,客人手牌号,部长,服务属性,技师工号,点轮) values ('";
                        sql = sql + dt.ToString() + "','";
                        sql = sql + room_id + "','";
                        sql = sql + fuwuxiangmu + "','";
                        sql = sql + "" + "','";
                        sql = sql + "" + "','";
                        sql = sql + "" + "','";
                        sql = sql + kerenshoupaihao + "','";
                        sql = sql + buzhanggonghao + "','";
                        sql = sql + fuwushuxing + "','";
                        sql = sql + jishi_id + "','";
                        sql = sql + "选" + "')";
                        mysql.exesqlCmd(sql);
                        //string tmp = "房号:" + fanghao + " 技师:" + jishigonghao + " 项目:" + textBox3.Text
                        //+ " 客人:" + kerenshoupaihao + " 部长:" + MyClass.caozuoyuan;
                        //MyClass.ri("消费项目--点钟", tmp, MyClass.buzhangshoupai);
                        mysql.close();
                        return "选钟操作成功";
                    }
                    catch
                    {
                        mysql.close();
                        return "选钟操作失败";
                    }
                }
            }
            catch
            {
                return "选钟操作失败";
            }
        }

        [WebMethod]
        //传入客户端时间，判断有效期
        public string PanduanShijian(string limit_time, string client_time)
        {
            DateTime dt = DateTime.Now;
            DateTime dt1 = Convert.ToDateTime(client_time);
            DateTime dt2 = Convert.ToDateTime(limit_time + " 23:59:59");
            TimeSpan ts = dt - dt1;
            if (DateTime.Compare(dt, dt2) > 0 || ts.TotalMinutes > 30 || ts.TotalMinutes < -30)
                return "false";
            else
                return "true";
        }

    }
}
