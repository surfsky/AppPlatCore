
//-------------------------------------------
// IO
//-------------------------------------------
// 在a.js中直接使用 include("../js/common.js");
function include(jssrc){
    // 先获取当前a.js的src。a.js中调用include,直接获取最后1个script标签就是a.js的引用。
    var scripts = document.getElementsByTagName("script");
    var lastScript = scripts[scripts.length-1];
    var src = lastScript.src;
    if(src.indexOf("http://")!=0 && src.indexOf("/") !=0){     
        // a.js使用相对路径,先替换成绝对路径
        var url = location.href;
        var index = url.indexOf("?");
        if(index != -1){
            url = url.substring(0, index-1);
        }
         
        src = getPath(src,url);
    }
    var jssrcs = jssrc.split("|");  // 可以include多个js，用|隔开
    for(var i=0;i<jssrcs.length;i++){
        // 使用juqery的同步ajax加载js.
        // 使用document.write 动态添加的js会在当前js的后面，可能会有js引用问题
        // 动态创建script脚本，是非阻塞下载，也会出现引用问题
        $.ajax({type:'GET', url:getPath(jssrc,src), async:false, dataType:'script'});
    }
}
 

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

