//-------------------------------------------
// DOM Basic
//-------------------------------------------
// DOM 操作辅助类
var dom = new function () {
    // 获取元素
    function getEl(cssSelector = '', id = '') {
        if (id != '')          return document.getElementById(id);
        if (cssSelector != '') return document.querySelector(cssSelector);
    }
    // 获取元素列表
    function getEls(name = '', className = '', tagName = '', cssSelector = '') {
        if (name != '') return document.getElementsByName(name);
        if (className != '') return document.getElementsByClassName(className);
        if (tagName != '') return document.getElementsByTagName(tagName);
        if (cssSelector != '') return document.querySelectorAll(cssSelector);
    }


    // 属性及样式
    function getAttr(el, name)         { return el.name; }
    function setAttr(el, name, value)  { el.name = value; }
    function getStyle(el, name)        { return el.style.name; }
    function setStyle(el, name, value) { el.style.name = value; }

    // 事件
    function on(el, eventName, handler) {
        el.addEventLisenter(eventName, handler);
    }

    // 获取根对象
    function getRoot(item) {
        if (item.parent != 'undefined')
            return getRoot(item.parent);
    }

}();


//-------------------------------------------
// DOM Tool
//-------------------------------------------
// 切换全屏(ie8不支持)
function switchFullScreen() {
    if (isFullscreen()) exitFullScreen();
    else enterFullScreen();
}
// 是否全屏
function isFullscreen() {
    return document.fullscreenElement ||
        document.msFullscreenElement ||
        document.mozFullScreenElement ||
        document.webkitFullscreenElement ||
        false;
}
//进入全屏
function enterFullScreen() {
    var de = document.documentElement;
    if (de.requestFullscreen) de.requestFullscreen();
    else if (de.mozRequestFullScreen) de.mozRequestFullScreen();
    else if (de.webkitRequestFullScreen) de.webkitRequestFullScreen();
    else if (de.msRequestFullscreen) de.msRequestFullscreen();
}
//退出全屏
function exitFullScreen() {
    var de = document;
    if (de.exitFullscreen) de.exitFullscreen();
    else if (de.mozCancelFullScreen) de.mozCancelFullScreen();
    else if (de.webkitCancelFullScreen) de.webkitCancelFullScreen();
    else if (de.msExitFullscreen) de.msExitFullscreen();
}

//-------------------------------------------
// 语言特性
//-------------------------------------------
// 获取属性
function getProperty(o, propertyName) {
    if (o.hasOwnProperty(propertyName))
        return o.propertyName;
    return null;
}




//-------------------------------------------
// IO
//-------------------------------------------
// 根据相对路径获取绝对路径
function getPath(relativePath, absolutePath) {
    var reg = new RegExp("\\.\\./", "g");
    var uplayCount = 0;     // 相对路径中返回上层的次数。
    var m = relativePath.match(reg);
    if (m) uplayCount = m.length;

    var lastIndex = absolutePath.length - 1;
    for (var i = 0; i <= uplayCount; i++) {
        lastIndex = absolutePath.lastIndexOf("/", lastIndex);
    }
    return absolutePath.substr(0, lastIndex + 1) + relativePath.replace(reg, "");
}      



//-------------------------------------------
// Tools
//-------------------------------------------
// 生成 GUID
function newGuid() {
    var guid = "";
    for (var i = 1; i <= 32; i++) {
        var n = Math.floor(Math.random() * 16.0).toString(16);
        guid += n;
        if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
            guid += "-";
    }
    return guid;
}
