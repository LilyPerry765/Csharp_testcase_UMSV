/*
Written By : Mohammad Dayyan
Weblog: http://mds-soft.persianblog.ir
*/

var today = getTodayPersian();
var year = today[0];
var month = today[1];
var day = today[2];

var targetElement = null;
var previousCalendarElement = null;
var mainDivID = "MDS_DateTimePicker";

function HideDateTimePicker() {
    try {
        var previousCalendarElement = document.getElementById(mainDivID);
        previousCalendarElement.style.visibility = "hidden";
        previousCalendarElement.style.display = "none";
    }
    catch (e) { }
}

function ShowDateTimePicker(targetElementID) {
    targetElement = document.getElementById(targetElementID);
    previousCalendarElement = document.getElementById(mainDivID);

    var x = targetElement.offsetLeft;
    var y = targetElement.offsetTop + targetElement.offsetHeight;

    var parent = targetElement;
    while (parent.offsetParent) {
        parent = parent.offsetParent;
        x += parent.offsetLeft;
        y += parent.offsetTop;
    }

    //var weekDay = WeekDay(today[3]);
    var numberOfDaysInCurrentMonth = 31;
    if (month > 6 && month < 12)
        numberOfDaysInCurrentMonth = 30;
    else if (month == 12)
        numberOfDaysInCurrentMonth = leap_persian(year) ? 30 : 29;

    var numberOfDaysInPreviousMonth = 31;
    if (month - 1 > 6 && month - 1 < 12)
        numberOfDaysInPreviousMonth = 30;
    else if (month - 1 == 12)
        numberOfDaysInPreviousMonth = leap_persian(year - 1) ? 30 : 29;

    var MonthYear = Month(month) + "  " + ToPersianNumber(year);

    ///////////////////////////////////////////////////////////

    var j = persian_to_jd(year, month, 01);
    var fistWeekDay = jwday(j);

    var tableString = "";
    var div = null;

    if (previousCalendarElement == null) {
        div = document.createElement('div');
        div.id = mainDivID;
        div.style.zIndex = "110";
        div.style.left = x + "px";
        div.style.top = y + "px";
        div.style.position = "absolute";
        //tableString = '<div id="' + mainDivID + '" style="left:' + x + 'px; top:' + y + 'px; z-index:2; margin: 0px auto; position: absolute;">';
    }
    else {
        previousCalendarElement.style.left = x + "px";
        previousCalendarElement.style.top = y + "px";
        previousCalendarElement.style.visibility = "visible";
        previousCalendarElement.style.display = "block";
    }
    tableString += '<table border="0" cellpadding="0" cellspacing="0" id="CalendarTable">';
    tableString += '<tr>';
    tableString += '<td><input id="PreviousMonthButton" type="button" value="&lt;" onclick="showPreviousMonth();" /></td>';
    tableString += '<td id="calendar_header" colspan="5">' + MonthYear + '</td>';
    tableString += '<td><input id="NextMonthButton" type="button" value="&gt;" onclick="showNextMonth()" /></td>';
    tableString += '</tr>';
    tableString += '<tr id="WeekDayNames"><td width="14%">ش</td><td width="14.3%">ی</td><td width="14.3%">د</td><td width="14.3%">س</td><td width="14.3%">چ</td><td width="14.3%">پ</td><td width="14%">ج</td></tr>';

    var cellNumber = 0;
    var td_Number = 0;

    if (fistWeekDay != 6)
        for (var i =  fistWeekDay; i >= 0; i--) 
		{
        	tableString += '<td class="previousMonth">' + ToPersianNumber(numberOfDaysInPreviousMonth - i) + '</td>';
        	cellNumber++;
        	td_Number++;
    	}

    var todayTemp = getTodayPersian();
    yearTemp = todayTemp[0];
    monthTemp = todayTemp[1];
    dayTemp = todayTemp[2];

    for (var i = 1; i <= numberOfDaysInCurrentMonth; i++) {
        if (td_Number == 7) {
            td_Number = 0;
            tableString += '</tr><tr>';
        }
        if (i == dayTemp && month == monthTemp && year == yearTemp)
            tableString += '<td id="today_td" onclick="td_click(this)">' + ToPersianNumber(i) + '</td>';
        else
            tableString += '<td onclick="td_click(this)">' + ToPersianNumber(i) + '</td>';
        td_Number++;
        cellNumber++;
    }

    if (cellNumber < 42) {
        for (var i = 1; i <= 42 - cellNumber; i++) {
            if (td_Number == 7) {
                td_Number = 0;
                tableString += '</tr><tr>';
            }
            tableString += '<td class="previousMonth">' + ToPersianNumber(i) + '</td>';
            td_Number++;
        }
    }

    tableString += '<tr>';
    tableString += '<td id="calendar_footer" colspan="7"><input id="today_button" type="button" value="امروز" onclick="ShowCurrentMonth()" />&nbsp;&nbsp;&nbsp;&nbsp;<input id="close_button" type="button" value="بستن" onclick="HideDateTimePicker(\'CalendarTable\');" /></td>';
    tableString += '</tr>';
    tableString += '</table>';
    tableString += '</div>';

    if (previousCalendarElement != null)
        previousCalendarElement.innerHTML = tableString;
    else {
        div.innerHTML += tableString;
        document.body.appendChild(div);
    }
}

function ShowCurrentMonth() {
    today = getTodayPersian();
    year = today[0];
    month = today[1];
    day = today[2];
    ShowDateTimePicker(targetElement.id);
}

function showPreviousMonth() {
    month = month - 1;
    if (month < 1) {
        month = 12;
        year--;
    }
    ShowDateTimePicker(targetElement.id);
}

function showNextMonth() {
    month = month + 1;
    if (month > 12) {
        month = 1;
        year++;
    }
    ShowDateTimePicker(targetElement.id);
}

function td_click(td_element) {
    var str = ToPersianNumber(year + "/" + Zero_Pad(month, 10) + "/" + Zero_Pad(td_element.innerHTML, 10));
    var textbox_element = document.getElementById(targetElement.id);
    textbox_element.value = str;
    HideDateTimePicker();
}


function ToPersianNumber(inputNumber1) {
    /* ۰ ۱ ۲ ۳ ۴ ۵ ۶ ۷ ۸ ۹ */
    var str1 = Trim(inputNumber1.toString());
    if (str1 == "") return;
    str1 = Trim(str1);
//    str1 = str1.replace(/0/g, '۰');
//    str1 = str1.replace(/1/g, '۱');
//    str1 = str1.replace(/2/g, '۲');
//    str1 = str1.replace(/3/g, '۳');
//    str1 = str1.replace(/4/g, '۴');
//    str1 = str1.replace(/5/g, '۵');
//    str1 = str1.replace(/6/g, '۶');
//    str1 = str1.replace(/7/g, '۷');
//    str1 = str1.replace(/8/g, '۸');
//    str1 = str1.replace(/9/g, '۹');
    return str1;
}

function ToEnglishNumber(inputNumber2) {
    var str = Trim(inputNumber2.toString());
    if (str == "") return;
    str = str.replace(/۰/g, '0');
    str = str.replace(/۱/g, '1');
    str = str.replace(/۲/g, '2');
    str = str.replace(/۳/g, '3');
    str = str.replace(/۴/g, '4');
    str = str.replace(/۵/g, '5');
    str = str.replace(/۶/g, '6');
    str = str.replace(/۷/g, '7');
    str = str.replace(/۸/g, '8');
    str = str.replace(/۹/g, '9');
    return str;
}

function Trim(input_string) {
    return input_string.replace(/^\s+|\s+$/g, "");
}

function WeekDay(weekDayNumber) {
    switch (weekDayNumber) {
        case 0:
            return "یک شنبه";
            break;
        case 1:
            return "دو شنبه";
            break;
        case 2:
            return "سه شنبه";
            break;
        case 3:
            return "چهار شنبه";
            break;
        case 4:
            return "پنج شنبه";
            break;
        case 5:
            return "جمعه";
            break;
        case 6:
            return "شنبه";
            break;
    }
}

function Month(monthNumber) {
    switch (monthNumber) {
        case 1:
            return "فروردین";
            break;
        case 2:
            return "اردیبهشت";
            break;
        case 3:
            return "خرداد";
            break;
        case 4:
            return "تیر";
            break;
        case 5:
            return "مرداد";
            break;
        case 6:
            return "شهریور";
            break;
        case 7:
            return "مهر";
            break;
        case 8:
            return "آبان";
            break;
        case 9:
            return "آذر";
            break;
        case 10:
            return "دی";
            break;
        case 11:
            return "بهمن";
            break;
        case 12:
            return "اسفند";
            break;
    }
}

function Zero_Pad(nr, base) {
    var len = (String(base).length - String(nr).length) + 1;
    return len > 0 ? new Array(len).join('0') + nr : nr;
}

