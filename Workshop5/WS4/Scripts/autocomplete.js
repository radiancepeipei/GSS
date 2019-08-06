function AutoCompleteInitial(autocompleteDivItem,width,getURL){
    $(autocompleteDivItem).width(width).kendoAutoComplete({
        dataSource: {
            transport: {
                read: {
                    type: "post",
                    dataType: "json",
                    url: getURL
                  
                }
            }
        }
    });

}