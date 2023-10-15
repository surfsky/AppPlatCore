
function renderGender(value) {
    return value == 1 ? '男' : '女';
}

// https://fineui.com/js/api/global.html##F_Grid_cardRenderer
// params.rowData: 当前行数据
// params.renderer: 获取列渲染值的回调函数（如果列定义了渲染函数，通过此参数获取列渲染之后的值）
function renderCard(params) {
    var rowId = params.rowData.id;
    var renderer = params.renderer;

    var COLUMNIDS = {
        'AtSchool': 'AtSchool',
        'Name': 'Name',
        'Gender': 'Gender',
        'EntranceYear': 'EntranceYear',
        'EntranceDate': 'EntranceDate',
        'Major': 'Major'
    };

    // 纯JS示例中使用的列名和 FineUIPro/Mvc/Core 中使用的列名不大相同
    if (F.product === 'F.js') {
        COLUMNIDS = {
            'AtSchool': 'atSchool',
            'Name': 'name',
            'Gender': 'gender',
            'EntranceYear': 'entranceYear',
            'EntranceDate': 'entranceDate',
            'Major': 'major'
        };
    }

    // 是否在校（图标）
    var atSchool = params.rowData.values[COLUMNIDS.AtSchool] ?
        '<i class="f-icon f-iconfont f-grid-static-checkbox f-checked"></i>' : '<i class="f-icon f-iconfont f-grid-static-checkbox"></i>';

    return '<div class="card"><table>' +
        '<tr><td>编号：</td><td>' + rowId + '</td></tr>' +
        '<tr><td>姓名：</td><td>' + renderer(COLUMNIDS.Name) + '</td></tr>' +
        '<tr><td>性别：</td><td>' + renderer(COLUMNIDS.Gender) + '</td></tr>' +
        '<tr><td>入学年份：</td><td>' + renderer(COLUMNIDS.EntranceYear) + '</td></tr>' +
        '<tr><td>入学日期：</td><td>' + renderer(COLUMNIDS.EntranceDate) + '</td></tr>' +
        '<tr><td>是否在校：</td><td>' + atSchool + '</td></tr>' +
        '<tr><td>所学专业：</td><td>' + renderer(COLUMNIDS.Major) + '</td></tr>' +
        '</table></div>';
}