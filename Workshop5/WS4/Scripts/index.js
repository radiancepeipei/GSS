
$(document).ready(function () {
    //初始化首頁dropdownlist
    DropDownListInitial("#search_book_class_dropdownlist","GetBookClassDropdownList");
    DropDownListInitial("#search_book_keeper_dropdownlist","GetKeeperDropDownList");
    DropDownListInitial("#search_book_status_dropdownlist","GetBookStatusDropDownList");
    DropDownListInitial("#update_book_class_dropdownlist", "GetBookClassDropdownList");
    DropDownListInitial("#update_book_keeper_dropdownlist", "GetKeeperDropDownList");
    DropDownListInitial("#update_book_status_dropdownlist", "GetBookStatusDropDownList");
    //初始化編輯window的datepicker
    DatepickerInitial("#update_book_bought_datepicker", "yyyy/MM/dd");
    //初始化書名auto
    AutoCompleteInitial("#search_book_name", 255, "GetBookNameAutoComplete");



    //建立grid 
    $("#book_grid").kendoGrid({  //初始化kendogrid
        dataSource: {
              transport: {
                        read: {
                            url: "GetBookSearchResult",
                            type: "Post",
                            dataType: "json"

                        }
                    }, pageSize: 20
        },
        height: 550,
        sortable: true,
        scrollable: true,
        pageable: true,
        columns:
            [
            { field: "BOOKID", title: "編號", hidden: true},   // hidden 把這個欄位隱藏
            { field: "BOOK_CLASS_NAME", title: "圖書類別", width: "15%" },
            { field: "BOOK_NAME", title: "書名", width: "30%" },
            { field: "BOOK_BOUGHT_DATE", title: "購書日期", format: "{0:yyyy-mm-dd}", width: "15%" },
            { field: "CODE_NAME", title: "借閱狀態", width: "10%" },
            { field: "USER_ENAME", title: "借閱人", width: "10%" },
                { command: { text: "編輯", click: updateBook, width: "10%"  } },  // inline click事件
                { command: { text: "刪除", click: deleteBook, width: "10%"  } }
            ]
    });

    //搜尋書籍
    $("#search_book_btn").click(function (e) {
        e.preventDefault();
        var SearchBookArg =
        {//抓搜尋欄位填入的value
            BOOK_NAME: $("#search_book_name").val( ),
            BOOK_CLASS_ID: $("#search_book_class_dropdownlist").data("kendoDropDownList").value(),
            BOOK_KEEPER: $("#search_book_keeper_dropdownlist").data("kendoDropDownList").value(),
            BOOK_STATUS: $("#search_book_status_dropdownlist").data("kendoDropDownList").value()     
        };
        var dataSource = new kendo.data.DataSource({
            transport: {
                read: {
                    type: "Post",
                    dataType: "json",
                    url: "GetBookSearchResult",
                    data: SearchBookArg
                }, pageSize: 20
            }
        });
        //把dataSource的data綁到search grid 的col
        $("#book_grid").data("kendoGrid").setDataSource(dataSource);    
    });

    //初始編輯kendoWindow
    $("#update_book_window").kendoWindow({
        width: "400px",
        modal: true, //overlay page
        visible: false,
        title: "編輯書籍",
        actions: ["Close"]
    });


    //??
    //編輯window按下存檔
    $("#update_book_save").click(function (e) {
        e.preventDefault();
        var updateBookData = {
            BOOKID: $("update_book_id").val(),
            BOOK_NAME: $("#update_book_name").val(),
            BOOK_AUTHOR: $("#update_book_author").val(),
            BOOK_PUBLISHER: $("#update_book_publisher").val(),
            BOOK_NOTE: $("#update_book_note").val(),
            BOOK_BOUGHT_DATE: $("#update_book_bought_datepicker").data("kendoDatePicker").value(),
            BOOK_CLASS_ID: $("#update_book_class_dropdownlist").data("kendoDropDownList").value(),
            BOOK_STATUS: $("#update_book_status_dropdownlist").data("kendoDropDownList").value(),
            USER_ID: $("#update_book_keeper_dropdownlist").data("kendoDropDownList").value()
        };

        //判斷書籍狀態與借閱人關係是否正確
       /* if () {
        }*/

        //$.ajax({
        //    type: " Post",
        //    dataType: "json",
        //    url: "UpdateBookInDB",
        //    data: updateBookData
        //}).done(function (data) {
        //    alert("已更改成功");
        //    $("#book_grid").data("kendoGrid").dataSource.read(); // 更改後 grid 重 load
        //});



});




/*------------------------------------*/
    //??
    //按下新增跳轉至新增頁面
    $("#insert_book_btn").click(function (e) {
        e.preventDefault();
        window.location = 'InsertBook';
    });


    //首頁清除輸入欄位
    $("#clear_book_btn").click(function (e) {
        e.preventDefault();
        $("#search_book_name").val(''); 
        $("#search_book_class_dropdownlist").data("kendoDropDownList").select(-1);//回到預設
        $("#search_book_keeper_dropdownlist").data("kendoDropDownList").select(-1);
        $("#search_book_status_dropdownlist").data("kendoDropDownList").select(-1);
    });




});//document.ready end tag
    

  
//抓預設資料
function updateBook(e) {
        e.preventDefault();
        var bookData = this.dataItem($(e.target).closest("tr"));//找最近一列的BookId
        $.ajax({
            type: "POST",
            url: " UpdateBookDefault",
            data: { "bookId": bookData.BOOKID },  //對應updateBook controller參數:grid field
            success: function (response) {  //避免連線錯誤
                //response.Model欄位資料放進 前端欄位
                $("#update_book_id").val(response.BOOKID);
                $("#update_book_name").val(response.BOOK_NAME);
                $("#update_book_author").val(response.BOOK_AUTHOR);
                $("#update_book_publisher").val(response.BOOK_PUBLISHER);
                $("#update_book_note").val(response.BOOK_NOTE);
                $("#update_book_bought_datepicker").data("kendoDatePicker").value(response.BOOK_BOUGHT_DATE);
                $("#update_book_class_dropdownlist").data("kendoDropDownList").value(response.BOOK_CLASS_ID);
                $("#update_book_status_dropdownlist").data("kendoDropDownList").value(response.BOOK_STATUS);
                $("#update_book_keeper_dropdownlist").data("kendoDropDownList").value(response.USER_ID);

                var bookstatusvalue = response.BOOK_STATUS;
                if (bookstatusvalue == "A" || bookstatusvalue == "U") {  //A是可借出，U是不可借出
                    $("#update_book_keeper_dropdownlist").data("kendoDropDownList").enable(false);//keeper無法使用
                    $("#update_book_delete").attr("disable", false);//可刪除
                }
                else if (bookstatusvalue == "B" || bookstatusvalue == "C") {  //B是已借出，U是已借出未領
                    $("#update_book_keeper_dropdownlist").data("kendoDropDownList").enable(true);//keeper可使用
                    $("#update_book_delete").attr("disable", true);//不可刪除
                }
            }, error: function(error) {
                    alert("系統發生錯誤");
                }
        });

    $("#update_book_window").data("kendoWindow").center().open();
}

//動態變更借閱人跟書籍狀態的Dropdownlist
//changeBookStatus();
//function changeBookStatus() {
//    var bookStatus = $("#update_book_status_dropdownlist").data("kendoDropDownList").value();
//    console.log(bookStatus);
//    $("#update_book_keeper_dropdownlist").change(function () {
//        if (bookstatusvalue == "A" || bookstatusvalue == "U") {  //A是可借出，U是不可借出
//            $("#update_book_keeper_dropdownlist").data("kendoDropDownList").enable(false);//keeper無法使用
//            $("#update_book_delete").attr("disable", false);//可刪除
//        }
//        else if (bookstatusvalue == "B" || bookstatusvalue == "C") {  //B是已借出，U是已借出未領
//            $("#update_book_keeper_dropdownlist").data("kendoDropDownList").enable(true);//keeper可使用
//            $("#update_book_delete").attr("disable", true);//不可刪除
//        }

//    });





//}

function changeBookStatus() {
    $("select#update_book_status_dropdownlist").change(function () {
        var nvalue = $("#update_book_status_dropdownlist").data("kendoDropDownList").value()
        console.log(nvalue)
        if (nvalue == "A" || nvalue == "U") {
            $("#update_book_delete").data("kendoDropDownList").enable(false);
            $('#update_book_delete').attr("disabled", false);
        }
        else {
            $("#update_book_delete").data("kendoDropDownList").enable(true);
            $('#update_book_delete').attr("disabled", true);
        }
    });

}



    //刪除單本書
    function deleteBook(e) {
        e.preventDefault();
        var grid = $("#book_grid").data("kendoGrid");
        var target = $(e.target).closest("tr");
        var msg = "確定要刪除嗎?";
        kendo.confirm(msg).then(function () {
            $.ajax({
                type: "POST",
                url: "DeleteBook",
                data: "BookId=" + grid.dataItem(target).BOOKID,
                datatype: "json",
                success: function (response) {
                    if (response) {
                        grid.removeRow(target);
                        alert("已成功刪除");
                    } else {
                        alert("書已借出，無法刪除");
                    }
                }, error: function (error) {
                    alert("系統發生錯誤");
                }
            });

        });
    }











