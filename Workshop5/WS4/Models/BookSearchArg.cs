using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WS4.Models
{
    public class BookSearchArg   
    {
        [DisplayName("書籍名稱")]
        public string BOOK_NAME { get; set; }

        [DisplayName("圖書類別")]
        public string BOOK_CLASS_ID { get; set; }  //電腦要用

        [DisplayName("借閱狀態")]
        public string BOOK_STATUS { get; set; }

        [DisplayName("借閱人")]
        public string BOOK_KEEPER { get; set; }

    }
}