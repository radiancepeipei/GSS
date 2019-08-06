using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WS4.Controllers
{
    public class BookController : Controller    
    {  // 初始化物件，下面要用到 dropdownlistService、bookService的屬性
        Models.CodeService dropdownlistService = new Models.CodeService();  // Models.CodeService( ) 建構子 
        Models.BookService bookService = new Models.BookService();  // Models.BookService( ) 建構子

        /// Get :Book
        /// <summary>
        /// 首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 書籍類別 dropdonwlist
        /// </summary>
        [HttpGet()]
        public JsonResult GetBookClassDropdownList()
        {
            List<SelectListItem> bookClass = dropdownlistService.GetBookClassTable();
            return Json(bookClass, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 借閱人 dropdownlist
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public JsonResult GetKeeperDropDownList()
        {
            List<SelectListItem> user = dropdownlistService.GetUserNameTable();
            return Json(user,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 書籍狀態 dropdownlist
        /// </summary>
        [HttpGet()]
        public JsonResult GetBookStatusDropDownList()
        {
            List<SelectListItem> bookStatus = dropdownlistService.GetBookStutusTable();
            return Json(bookStatus,JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        ///  搜尋結果
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost()]
        public JsonResult GetBookSearchResult(Models.BookSearchArg bookSearch)
        {
            List<Models.Book> searchResult = bookService.GetBookByCondtioin(bookSearch);
            return Json(searchResult);
        }

        ///查詢頁點新增進入
        /// <summary>
        /// 新增書籍
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult InsertBookToDB(Models.Book book) {
            bookService.InsertBook(book);
            return View("InsertBook");
        }

        /// <summary>
        /// 首頁刪除書籍
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        [HttpPost()]
        public JsonResult DeleteBook(int BookId)
        {
            try
            {
                bookService.DeleteBookById(BookId);
                return this.Json(true);  //this ->類別物件
            }
            catch (Exception ex)
            {
                return this.Json(false);
            }
        }

        /// <summary>
        /// 修改書籍
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        [HttpPost()] 
        public JsonResult UpdateBookDefault(int bookId)  //傳入該本書的 bookId
        {
            Models.Book SelectedBook=bookService.GetBookById(bookId);  //準備修改書籍的資料
            return Json(SelectedBook);
        }

        [HttpPost()]//修改頁面按下存檔，確認修改後跑POST
        public JsonResult UpdateBookInDB(Models.Book book)   //傳入參數為使用者輸入，只有傳入的資料欄位會被更新，其他欄位維持不變
        {
            bookService.UpdateBook(book);//做完Update的book
            return Json(true);
        }

        [HttpGet]//書名顯示明細
        public ActionResult GetBookDetail(int bookId)
        {
            Models.Book SelectedBook = bookService.GetBookById(bookId);
            return View(SelectedBook);
        }

        [HttpPost()]
        public JsonResult GetBookNameAutoComplete()
        {
            List<string> bookNameList = bookService.GetAllBookName();
            return Json(bookNameList);
        }
    }
}