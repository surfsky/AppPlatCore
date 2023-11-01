
function queryString(key) {
    var regex_str = "^.+\\?.*?\\b" + key + "=(.*?)(?:(?=&)|$|#)"
    var regex = new RegExp(regex_str, "i");
    var url = window.location.toString();
    if (regex.test(url)) return RegExp.$1;
    return undefined;
}


function isCardNo(card) {
    var reg = /(^\d{15}$)|(^\d{18}$)|(^\d{17}(\d|X|x)$)/;
    if (reg.test(card) === false) {
        return false;
    }
    return true;
}

function isEmailNo(email) {
    var myreg = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
    if (!myreg.test(email)) {
        return false;
    }
    return true;
}


function isMobileNo(mobile) {
    if (!/^1(3|4|5|7|8)\d{9}$/.test(mobile)) {
        return false;
    }
    return true;
}

function addDays(date, days) {
    var nd = new Date(date);
    nd = nd.valueOf();
    nd = nd + days * 24 * 60 * 60 * 1000;
    nd = new Date(nd);
    var y = nd.getFullYear();
    var m = nd.getMonth() + 1;
    var d = nd.getDate();
    if (m <= 9) m = "0" + m;
    if (d <= 9) d = "0" + d;
    var cdate = y + "-" + m + "-" + d;
    return cdate;
}
function getWeek(myDate) {
    var str = "";
    if (myDate) {
        var Week = ['日', '一', '二', '三', '四', '五', '六'];
        str += '周' + Week[myDate.getDay()];
    }
    return str;
}

function getDateStr(month) {
    month = month + '';
    var str = month;
    if (month.length == 1)
        str = ("0" + (month + ''));
    return str;
}

function phone(data) {
    if (data != '')
        window.location.href = 'tel:' + data;
}
 //对浏览器的UserAgent进行正则匹配，不含有微信独有标识的则为其他浏览器
//var useragent = navigator.userAgent;
//if (useragent.match(/MicroMessenger/i) != 'MicroMessenger') {
//    // 这里警告框会阻塞当前页面继续加载
//    alert('已禁止本次访问：您必须使用微信内置浏览器访问本页面！');
//    // 以下代码是用javascript强行关闭当前页面
//    var opened = window.open('about:blank', '_self');
//    opened.opener = null;
//    opened.close();
//} 

function noData() {
    $("<div style=\"position: relative; width: 100%; display: table; position: absolute; top: 40%; left: 0;\"><p style=\"position: absolute; top:40%; left: 0; text-align: center; width: 100%; top: 0;color:#c6c6c6;font-size:16px\"><i class=\"weui-icon-info weui-icon_msg\" style=\"color:#cecece;font-size:45px\"></i><br/><br/>暂无数据</p></div>").css({ display: "block" }).appendTo("body");

}


