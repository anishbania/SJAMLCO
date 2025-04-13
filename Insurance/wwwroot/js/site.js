// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
$("[data-val-required]").not(":hidden").not(":radio").not(":checkbox").before('<span style="color:red;max-width:10px;min-height:30px;">*</span>');
// Write your JavaScript code.
function GetNepaliCurrentDate() {
    var selectedDate = NepaliFunctions.GetCurrentBsDate();
    var month = (selectedDate.month.toString().length == 2) ? selectedDate.month : '0' + selectedDate.month;
    var day = (selectedDate.day.toString().length == 2) ? selectedDate.day : '0' + selectedDate.day;
    var date = selectedDate.year + '-' + month + '-' + day;
    return date;
}

function NepaliDatePickerCal(date) {
    var cDate = GetNepaliCurrentDate();
    date.nepaliDatePicker({
        ndpYear: true,
        ndpMonth: true,
        readOnlyInput: true,
        disableAfter: cDate,
    //    unicodeDate: true
    });
    date.value = date.value || cDate;
}

//function NepaliDatePickerCal(date) {
//    date.nepaliDatePicker({
//        ndpYear: true,
//        ndpMonth: true,
//        readOnlyInput: true,
//        disableAfter: GetNepaliCurrentDate(),
//    });
//}


var npDate = document.getElementsByClassName("NepaliDate");
if (npDate.length > 0) {
    NepaliDatePickerCal(npDate);
}

var element = document.getElementsByClassName("Print_Date");
if (element) {
    element.textContent = EnglishToNepali(GetNepaliCurrentDate());
}
