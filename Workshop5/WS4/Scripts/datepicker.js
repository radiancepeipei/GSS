function DatepickerInitial(datepickerDivItem,dateFormat) {
    $(datepickerDivItem).kendoDatePicker(
        {
            value: new Date(),
            format: dateFormat,
            disableDates: function (date) { //禁用大於今天的日期
                return date <= kendo.date.today() ? false : true;
            }
        }
    );
}


