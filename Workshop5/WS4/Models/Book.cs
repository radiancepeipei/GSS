using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WS4.Models
{
    public class Book
    {
        [DisplayName("書籍編號")]
        public int BOOKID { get; set; }

        [DisplayName("書名")]
        [Required(ErrorMessage ="請填寫書名")]//Model裡為required的欄位的資料在資料庫裡不得為Null，否則會有錯誤
        public string BOOK_NAME { get; set; }

        [DisplayName("類別代號")]
        [Required(ErrorMessage = "請選擇書籍類別")]
        public string BOOK_CLASS_ID { get; set; }

        [DisplayName("圖書類別")]
        public string BOOK_CLASS_NAME { get; set; }

        [DisplayName("借閱人")]
        public string USER_ENAME { get; set; }

        [DisplayName("借閱人中文姓名")]
        public string USER_CNAME { get; set; }

        [DisplayName("借閱人")]
        public string USER_ID { get; set; }

        [DisplayName("作者")]
        [Required(ErrorMessage = "請填寫作者")]
        public string BOOK_AUTHOR { get; set; }

        [DisplayName("購書日期")]
        [Required(ErrorMessage = "請選擇日期")]
        public string BOOK_BOUGHT_DATE { get; set; }

        [DisplayName("出版商")]
        [Required(ErrorMessage = "請填寫出版商")]
        public string BOOK_PUBLISHER { get; set; }

        [DisplayName("內容簡介")]
        [Required(ErrorMessage = "請填寫簡介")]
        public string BOOK_NOTE { get; set; }

        [DisplayName("借閱狀態")]
        public string BOOK_STATUS { get; set; }  //=CODE_ID

        [DisplayName("借閱狀態")]
      
        public string CODE_NAME { get; set; }  

        [DisplayName("借閱人")]
        public string BOOK_KEEPER { get; set; }

        [DisplayName("書籍數量")]
        public int BOOK_AMOUNT { get; set; }

        [DisplayName("建立時間")]
        public DateTime CREATE_DATE { get; set; }

        [DisplayName("建立使用者")]
        public int CREATE_USER { get; set; }

        [DisplayName("修改時間")]
        public DateTime MODIFY_DATE { get; set; }

        [DisplayName("修改使用者")]
        public string MODIFY_USER { get; set; }
    }
}