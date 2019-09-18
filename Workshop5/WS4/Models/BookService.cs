using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WS4.Models
{
    public class BookService
    {   /// 取得DB連線字串
        private string GetDBConnectionString()
        {
            return
                System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
        }



        /// <summary>
        /// 跑搜尋結果呼叫MapBOOKDATAToList
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public List<Models.Book> GetBookByCondtioin(Models.BookSearchArg arg)
        {
            DataTable dt = new DataTable();  //宣告dataTable物件
            string sql = @"
                                    SELECT 
                                            BD.BOOK_ID AS BookId,
                                            BD.BOOK_NAME AS BookName,
                                            BD.BOOK_AUTHOR AS BookAuthor,
                                            BD.BOOK_PUBLISHER AS BookPublisher,
                                            BD.BOOK_NOTE AS BookNote,
                                            BD.BOOK_KEEPER AS BookKeeper,
                                            BD.BOOK_PUBLISHER AS BookPublisher,
		                                    BD.BOOK_NOTE AS BookNote,
                                            BD.BOOK_STATUS AS BookStatus,
                                            CONVERT(VARCHAR,BD.BOOK_BOUGHT_DATE, 111)  AS BookBoughtDate,
                                            BC.BOOK_CLASS_NAME AS BookClassName,
	                                        M.USER_ENAME  AS UserEname,
                                            M.USER_CNAME  AS UserCname,
		                                    BookCode.CODE_NAME AS CodeName,
                                            BC.BOOK_CLASS_ID  AS BookClassId,
                                            M.USER_ID AS UserId
                                            FROM BOOK_DATA BD
                                            INNER JOIN BOOK_CLASS BC ON BD.BOOK_CLASS_ID=BC.BOOK_CLASS_ID
                                            LEFT JOIN  MEMBER_M M ON BD.BOOK_KEEPER=M.[USER_ID]
                                            INNER JOIN BOOK_CODE BookCode on BD.BOOK_STATUS=BookCode.CODE_ID
                                            WHERE
		                                    (BookCode.CODE_TYPE='BOOK_STATUS')AND
		                                    (BD.BOOK_NAME LIKE ('%' + @BookName+'%') OR @BookName='' ) AND
                                            (BC.BOOK_CLASS_ID = @BookClassId OR @BookClassId ='' )AND
                                            (BD.BOOK_KEEPER = @BookKeeper OR @BookKeeper='')AND
                                            (BD.BOOK_STATUS = @BookStatus OR   @BookStatus='')
                                            ORDER BY BookBoughtDate DESC ";
                    
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);  //宣告sql command 物件
                cmd.Parameters.Add(new SqlParameter("@BookName", arg.BOOK_NAME == null ? string.Empty : arg.BOOK_NAME));  //指定Parameter
                cmd.Parameters.Add(new SqlParameter("@BookClassId", arg.BOOK_CLASS_ID == null ? string.Empty : arg.BOOK_CLASS_ID));
                cmd.Parameters.Add(new SqlParameter("@BookKeeper", arg.BOOK_KEEPER == null ? string.Empty : arg.BOOK_KEEPER));
                cmd.Parameters.Add(new SqlParameter("@BookStatus", arg.BOOK_STATUS == null ? string.Empty : arg.BOOK_STATUS));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd); //宣告sql Adapter 物件 並傳入Sql command物件
                sqlAdapter.Fill(dt);  //填入dataTable物件
                conn.Close();
            }
            return this.MapBookDataToList(dt);//把 搜尋資料傳到data table 
        }

        /// <summary>
        /// 把搜尋結果裝到list回傳給 GetBookByCondtioin (為了準備搜尋結果的View)
        /// </summary>
        /// <param name="BOOKDATA"></param>
        /// <returns></returns>
        private List<Models.Book> MapBookDataToList(DataTable BookData) //從sql抓欄位給Book Model
        {
            List<Models.Book> result = new List<Book>();
            foreach (DataRow row in BookData.Rows)  //把BookData轉成 list
            {
                result.Add(new Book()
                {
                    BOOK_CLASS_ID = row["BookClassId"].ToString(),
                    USER_ID = row["UserId"].ToString(),
                    BOOK_BOUGHT_DATE = row["BookBoughtDate"].ToString(),
                    BOOK_NOTE=row["BookNote"].ToString(),
                    BOOK_STATUS=row["BookStatus"].ToString(),
                    BOOK_KEEPER=row["BookKeeper"].ToString(),
                    BOOKID = (int)row["BookId"],
                    BOOK_NAME = row["BookName"].ToString(),
                    BOOK_CLASS_NAME = row["BookClassName"].ToString(),
                    BOOK_AUTHOR = row["BookAuthor"].ToString(),
                    BOOK_PUBLISHER = row["BookPublisher"].ToString(),
                    USER_ENAME = row["UserEname"].ToString(),
                    USER_CNAME = row["UserCname"].ToString(),
                    CODE_NAME=row["CodeName"].ToString()
                }) ;
            }
            return result;
        }

        /// <summary>
        /// 跑新增書籍
        /// </summary>
        /// <param name="Book"></param>
        /// <returns></returns>
        public void InsertBook(Models.Book Book) {

          // DataTable dt = new DataTable();  //宣告dataTable物件
            string sql = @"INSERT INTO BOOK_DATA
                         (BOOK_NAME,
                          BOOK_AUTHOR,
                          BOOK_PUBLISHER,
                          BOOK_NOTE,
                          BOOK_BOUGHT_DATE,
                          BOOK_CLASS_ID,
                          BOOK_STATUS,
                          BOOK_KEEPER,
                          BOOK_AMOUNT
                         )
                         VALUES(@BOOK_NAME,@BOOK_AUTHOR,@BOOK_PUBLISHER,@BOOK_NOTE,@BOOK_BOUGHT_DATE,@BOOK_CLASS_ID,@BOOK_STATUS,'',1);";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlTransaction tran = conn.BeginTransaction();
                cmd.Transaction = tran;
                //防止sql injection
                cmd.Parameters.Add(new SqlParameter("@BOOK_NAME", Book.BOOK_NAME));
                cmd.Parameters.Add(new SqlParameter("@BOOK_AUTHOR", Book.BOOK_AUTHOR));
                cmd.Parameters.Add(new SqlParameter("@BOOK_PUBLISHER", Book.BOOK_PUBLISHER));
                cmd.Parameters.Add(new SqlParameter("@BOOK_NOTE", Book.BOOK_NOTE));
                cmd.Parameters.Add(new SqlParameter("@BOOK_BOUGHT_DATE", Book.BOOK_BOUGHT_DATE));
                cmd.Parameters.Add(new SqlParameter("@BOOK_CLASS_ID", Book.BOOK_CLASS_ID));
                cmd.Parameters.Add(new SqlParameter("@BOOK_STATUS", 'A'));
                conn.Open();
                try
                {
                    cmd.ExecuteScalar();//把異動的資料第一筆第一項傳回(BookId)  做validate
                    //cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception)
                {
                    tran.Rollback();//try若有錯誤catch rollback取消交易
                    throw;
                }
            }
          
        }

        /// <summary>
        /// 跑刪除書籍
        /// </summary>
        /// <param name="BookId"></param>
        public void DeleteBookById(int BookId)
        {
           
            string sql = "DELETE FROM BOOK_DATA WHERE BOOK_ID=@BookId";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlTransaction tran = conn.BeginTransaction();
                cmd.Transaction = tran;
                cmd.Parameters.Add(new SqlParameter("@BookId",BookId));
                try
                {
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 修改前顯示單一本書的資訊
        /// 傳入BookId搜尋單本書結果呼叫MapUPDATEBOOKDATAToList
        /// 因為只有一本所以使用[0]抓搜尋單本書的結果
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public Models.Book GetBookById(int bookId)
        {
            DataTable dt = new DataTable();  //宣告dataTable物件 
            //select所有book欄位
            string sql = @"SELECT    
                            bd.BOOK_ID AS BookId,
                            bc.BOOK_CLASS_NAME AS BookClassName,
		                    bd.BOOK_NAME AS BookName,
							bd.BOOK_PUBLISHER AS BookPublisher,
							bd.BOOK_NOTE AS BookNote,
                            CONVERT(varchar, bd.BOOK_BOUGHT_DATE, 23) AS BookBoughtDate,
	                    	bd.BOOK_STATUS AS BookStatus,
	                    	M.USER_ENAME  AS UserEname,
                            M.USER_CNAME  AS UserCname,
							BookCode.CODE_NAME AS CodeName,
                            bc.BOOK_CLASS_ID  AS BookClassId,
                            M.USER_ID AS UserId,
                            bd.BOOK_AUTHOR AS BookAuthor,
                            bd.BOOK_BOUGHT_DATE AS BookBoughtDate,
                            bd.BOOK_PUBLISHER AS BookPublisher,
                            bd.BOOK_NOTE AS BookNote,
                            bd.BOOK_KEEPER AS BookKeeper
                            FROM BOOK_DATA bd
                            INNER JOIN BOOK_CLASS bc ON bd.BOOK_CLASS_ID=bc.BOOK_CLASS_ID
                            LEFT JOIN  MEMBER_M M ON bd.BOOK_KEEPER=M.[USER_ID]
                            INNER JOIN BOOK_CODE BookCode on bd.BOOK_STATUS=BookCode.CODE_ID AND BookCode.CODE_TYPE='BOOK_STATUS'
                            WHERE  
                            bd.BOOK_ID=@BOOK_ID
                            ";//book code也要inner join 跟book class同概念 每本書會對到一種class ; Left join 是因為不需要沒借書的Member 
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);  //與資料庫連線並執行sql command
                cmd.Parameters.Add(new SqlParameter("@BOOK_ID", bookId));  //指定Parameter 傳遞參數
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd); //宣告sql Adapter 物件 並傳入Sql command
                sqlAdapter.Fill(dt);  //填入dataTable
                conn.Close();
            }
            return this.MapBookDataToList(dt).FirstOrDefault();  //找出的 bookId 只有一個 //*
        }

        //修改書籍內容
        public void UpdateBook(Models.Book book)
        {
            string sql = @"UPDATE BOOK_DATA 
                               SET BOOK_NAME=@BOOK_NAME,
                               BOOK_AUTHOR=@BOOK_AUTHOR,
                               BOOK_PUBLISHER=@BOOK_PUBLISHER,
                               BOOK_NOTE=@BOOK_NOTE,
                               BOOK_BOUGHT_DATE=@BOOK_BOUGHT_DATE,
                               BOOK_STATUS=@BOOK_STATUS,
                               BOOK_KEEPER=@BOOK_KEEPER,
                               BOOK_CLASS_ID=@BOOK_CLASS_ID
                               WHERE BOOK_ID=@BOOK_ID;";
           
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlTransaction tran = conn.BeginTransaction();
                cmd.Transaction = tran;

                cmd.Parameters.Add(new SqlParameter("@BOOK_ID", book.BOOKID));
                cmd.Parameters.Add(new SqlParameter("@BOOK_NAME", book.BOOK_NAME));
                cmd.Parameters.Add(new SqlParameter("@BOOK_AUTHOR", book.BOOK_AUTHOR));
                cmd.Parameters.Add(new SqlParameter("@BOOK_PUBLISHER", book.BOOK_PUBLISHER));
                cmd.Parameters.Add(new SqlParameter("@BOOK_NOTE", book.BOOK_NOTE));
                cmd.Parameters.Add(new SqlParameter("@BOOK_BOUGHT_DATE", book.BOOK_BOUGHT_DATE));
                cmd.Parameters.Add(new SqlParameter("@BOOK_CLASS_ID", book.BOOK_CLASS_ID));
                cmd.Parameters.Add(new SqlParameter("@BOOK_STATUS", book.BOOK_STATUS));
                cmd.Parameters.Add(new SqlParameter("@BOOK_KEEPER", book.BOOK_KEEPER == null ? string.Empty:book.BOOK_KEEPER));
                try
                {
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    throw;
                }
                finally
                {
                    conn.Close();
                }
            }
        }


        //Book name autocomplete的Book Name Data
        public List<string> GetAllBookName( )
        {
            DataTable dt = new DataTable( );
            string sql = @"SELECT DISTINCT 
                                        BD.BOOK_NAME AS BOOK_NAME
                                        FROM BOOK_DATA BD
                                        ORDER BY BOOK_NAME;";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);  
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd); 
                sqlAdapter.Fill(dt);  
                conn.Close();
            }
            return MapAllBookNameToList(dt);

        }

        private List<string> MapAllBookNameToList(DataTable BookNameData) 
        {
           List<string> result = new List<string>();
            foreach (DataRow row in BookNameData.Rows)  //把BookData轉成 list
            {
                result.Add(row["BOOK_NAME"].ToString( ));
            }
            return result;
        }
    }
}
 