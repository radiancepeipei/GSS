function DropDownListInitial(dropDownListDivItem, getDataURL) {
    $(dropDownListDivItem).kendoDropDownList({
            optionLabel: "請選擇...",
            dataTextField: "Text",
            dataValueField: "Value",
            dataSource: {
                    transport: {
                        read: {
                            dataType: "json",
                            url: getDataURL,
                            type: "Get"
                        }
                    }
            }
    });
}
