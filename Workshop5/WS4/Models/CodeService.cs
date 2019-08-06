using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WS4.Models
{
    public class CodeService
    {/// <summary>
     /// 取得DB連線字串
     /// </summary>
     /// <returns></returns>
        private string GetDBConnectionString()
        {
            return
                System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
        }

        /// <summary>
        /// BookClass Dropdownlist 需要 class_id 、class_name 
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetBookClassTable()
        {
            DataTable dt = new DataTable();  //宣告資料表物件
            string sql = @"SELECT BC.BOOK_CLASS_ID AS BOOK_CLASS_ID,BC.BOOK_CLASS_NAME AS BOOK_CLASS_NAME 
                                        FROM BOOK_CLASS BC;";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);         
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                conn.Open();
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapBookClassToList(dt);
        }

        /// <summary>
        ///  User Dropdownlist 需要 USER_id(BOOK_KEEPER) 、USER_name 
        /// </summary>
        public List<SelectListItem> GetUserNameTable()
        {
            DataTable dt = new DataTable();  //宣告資料表物件
            //用有英文名字重複
            string sql = @" SELECT M.USER_ENAME  AS  USER_ENAME ,M.[USER_ID]  AS  [USER_ID]
                                         FROM MEMBER_M M
                                         ORDER BY M.USER_ENAME ASC;     ";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                conn.Open();
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapMemberToList(dt);
        }

        ///這沒用到??
        /// <summary>
        ///修改頁面單本書的圖書類別
        /// </summary>
        public List<SelectListItem> GetBookClassTable(int BookId)
        {
            DataTable dt = new DataTable();  //宣告資料表物件
            string sql = @"SELECT BC.BOOK_CLASS_ID,BC.BOOK_CLASS_NAME FROM BOOK_CLASS BC;";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                conn.Open();
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapBookClassToList(dt);
        }

        ///BookStutus Dropdownlist 需要 class_id 、class_name 
        public List<SelectListItem> GetBookStutusTable()  //連資料庫跑sql
        {
            DataTable dt = new DataTable();  //宣告資料表物件
            string sql = @"SELECT BC.CODE_ID AS CODE_ID,BC.CODE_NAME AS CODE_NAME
                                        FROM BOOK_CODE BC 
                                        WHERE CODE_TYPE='BOOK_STATUS';";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                conn.Open();
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapBookCodeToList(dt);
        }

        /// <summary>
        /// Maping 資料 給dropdownlist
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private List<SelectListItem> MapBookClassToList(DataTable dt) //資料庫表格轉成SelectListItem
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Text = row["BOOK_CLASS_NAME"].ToString(),
                    Value = row["BOOK_CLASS_ID"].ToString()
                });
            }
            return result;
        }

     
        public List<SelectListItem> GetUserFullNameTable()
        {
            DataTable dt = new DataTable();  //宣告資料表物件
            string sql = @"
                                        SELECT (M.USER_ENAME+'-'+M.USER_CNAME) AS USER_FULL_NAME,M.[USER_ID] AS [USER_ID]
                                        FROM MEMBER_M M 
                                        ORDER BY M.USER_ENAME ASC;   ";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                conn.Open();
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapMemberToFullNameList(dt);
        }

        private List<SelectListItem> MapMemberToList(DataTable dt) //資料庫表格轉成SelectListItem
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Text = row["USER_ENAME"].ToString(),
                    Value = row["USER_ID"].ToString()
                });
            }
            return result;
        }

        private List<SelectListItem> MapMemberToFullNameList(DataTable dt) //資料庫表格轉成SelectListItem
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Text = row["USER_FULL_NAME"].ToString(),
                    Value = row["USER_ID"].ToString()
                });
            }
            return result;
        }

        private List<SelectListItem> MapBookCodeToList(DataTable dt) //將找到的資料庫表格轉成SelectListItem
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Text = row["CODE_NAME"].ToString(),
                    Value = row["CODE_ID"].ToString()
                });
            }
            return result;
        }
    }
}