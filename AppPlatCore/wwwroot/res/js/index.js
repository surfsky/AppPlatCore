
// plaintree - 树菜单
// tree - 智能树菜单
var _menuStyle = F.cookie('MenuStyle') || 'tree';
// 主选项卡标签页
var _mainTabs = F.cookie('MainTabs') || 'multi';

var SIDEBAR_WIDTH_CONSTANT = 260;
// _sidebarWidth变量会随着用户拖动分隔条而改变
var _sidebarWidth = SIDEBAR_WIDTH_CONSTANT;


// 基础版下载
function onBaseDownloadClick() {
    // 删除按钮的徽标
    this.setBadge(false);
    window.open('http://fineui.com/fans/', '_blank');
}

// 点击企业版试用
function onApplyTrialClick(event) {
    var windowApplyTrial = F(PARAMS.windowApplyTrial);
    windowApplyTrial.show();
}

// 点击主题仓库
function onThemeSelectClick(event) {
    var windowThemeRoller = F(PARAMS.windowThemeRoller);
    windowThemeRoller.show();
}

// 点击加载动画
function onLoadingSelectClick(event) {
    var windowLoadingSelector = F(PARAMS.windowLoadingSelector);
    windowLoadingSelector.show();
}

// 设置长期存在的Cookie
function setCookie(name, value) {
    F.cookie(name, value, {
        expires: 100  // 单位：天
    });
}


// 点击折叠/展开按钮
function onFoldClick(event) {
    toggleSidebar();
}

// 设置折叠按钮的状态
function setFoldButtonStatus(collapsed) {
    var foldButton = F(PARAMS.btnCollapseSidebar);
    if (collapsed) {
        foldButton.setIconFont('f-iconfont-unfold');
    } else {
        foldButton.setIconFont('f-iconfont-fold');
    }
}

// 获取折叠按钮的状态
function getFoldButtonStatus() {
    var foldButton = F(PARAMS.btnCollapseSidebar);
    return foldButton.iconFont === 'f-iconfont-unfold';
}

// 展开侧边栏
function expandSidebar() {
    toggleSidebar(false);
}

// 折叠侧边栏
function collapseSidebar() {
    toggleSidebar(true);
}

// 折叠/展开侧边栏
function toggleSidebar(collapsed) {
    var sidebarRegion = F(PARAMS.sidebarRegion);
    var treeMenu = F(PARAMS.treeMenu);
    var logoEl = sidebarRegion.el.find('.logo');

    var currentCollapsed = getFoldButtonStatus();
    if (F.isUND(collapsed)) {
        collapsed = !currentCollapsed;
    } else {
        if (currentCollapsed === collapsed) {
            return;
        }
    }

    F.noAnimation(function () {

        setFoldButtonStatus(collapsed);

        if (!collapsed) {
            if (_menuStyle === 'tree') {
                logoEl.removeClass('short').text(logoEl.attr('title'));
                sidebarRegion.setWidth(_sidebarWidth);
                // 启用分隔条拖动
                sidebarRegion.setSplitDraggable(true);

                // 禁用树微型模式
                treeMenu.miniMode = false;
                // 重新加载树菜单
                treeMenu.loadData();
            } else {
                sidebarRegion.expand();
            }
        } else {
            if (_menuStyle === 'tree') {
                logoEl.addClass('short').text('F');
                sidebarRegion.setWidth(60);
                // 禁用分隔条拖动
                sidebarRegion.setSplitDraggable(false);

                // 启用树微型模式
                treeMenu.miniMode = true;
                // 重新加载树菜单
                treeMenu.loadData();
            } else {
                sidebarRegion.collapse();
            }
        }
    });
}

// 侧边栏分隔条拖动事件
function onSidebarSplitDrag(event) {
    _sidebarWidth = this.width;
}


// 点击仅显示基础版示例
function onShowOnlyBaseClick(event) {
    var checked = this.isChecked();

    setCookie('ShowOnlyBase', checked);
    top.window.location.reload();
}


function onSearchTrigger1Click(event) {
    F.removeCookie('SearchText');
    top.window.location.reload();
}

function onSearchTrigger2Click(event) {
    var ttbxSearch = this;
    if (ttbxSearch.el.hasClass('collapsed')) {
        ttbxSearch.el.removeClass('collapsed').addClass('expanded').outerWidth(200);
    } else {
        var ttbxSearchValue = ttbxSearch.getValue();
        if (ttbxSearchValue) {
            setCookie('SearchText', this.getValue());
            top.window.location.reload();
        }
    }
}

function onSearchBlur(event) {
    var ttbxSearch = this;
    if (!ttbxSearch.getValue()) {
        ttbxSearch.el.removeClass('expanded').addClass('collapsed').outerWidth(24);
    }
}

// 点击标题栏工具图标 - 查看源代码
function onToolSourceCodeClick(event) {
    var mainTabStrip = F(PARAMS.mainTabStrip);
    var windowSourceCode = F(PARAMS.windowSourceCode);


    var activeTab = mainTabStrip.getActiveTab();
    var iframeWnd, iframeUrl;
    if (activeTab.iframe) {
        iframeWnd = activeTab.getIFrameWindow();
        iframeUrl = activeTab.getIFrameUrl();
    }

    var files = [iframeUrl];
    var sourcefilesNode = $(iframeWnd.document).find('head meta[name=sourcefiles]');
    if (sourcefilesNode.length) {
        $.merge(files, sourcefilesNode.attr('content').split(';'));
    }
    windowSourceCode.show(PARAMS.sourceUrl + '?files=' + encodeURIComponent(files.join(';')));

}

// 点击标题栏工具图标 - 刷新
function onToolRefreshClick(event) {
    var mainTabStrip = F(PARAMS.mainTabStrip);

    var activeTab = mainTabStrip.getActiveTab();
    if (activeTab.iframe) {
        var iframeWnd = activeTab.getIFrameWindow();
        iframeWnd.location.reload();
    }
}

// 点击标题栏工具图标 - 在新标签页中打开
function onToolNewWindowClick(event) {
    var mainTabStrip = F(PARAMS.mainTabStrip);

    var activeTab = mainTabStrip.getActiveTab();
    if (activeTab.iframe) {
        var iframeUrl = activeTab.getIFrameUrl();
        iframeUrl = PARAMS.processNewWindowUrl(iframeUrl);
        window.open(iframeUrl, '_blank');
    }
}


// 添加示例标签页（通过href在树中查找）
// href: 选项卡对应的网址
// actived: 是否激活选项卡（默认为true）
function addExampleTabByHref(href, actived) {
    var mainTabStrip = F(PARAMS.mainTabStrip);
    var treeMenu = F(PARAMS.treeMenu);

    F.addMainTabByHref(mainTabStrip, treeMenu, href, actived);
}


// 添加示例标签页
// tabOptions: 选项卡参数
// tabOptions.id： 选项卡ID
// tabOptions.iframeUrl: 选项卡IFrame地址 
// tabOptions.title： 选项卡标题
// tabOptions.icon： 选项卡图标
// tabOptions.createToolbar： 创建选项卡前的回调函数（接受tabOptions参数）
// tabOptions.refreshWhenExist： 添加选项卡时，如果选项卡已经存在，是否刷新内部IFrame
// tabOptions.iconFont： 选项卡图标字体
// actived: 是否激活选项卡（默认为true）
function addExampleTab(tabOptions, actived) {

    if (typeof (tabOptions) === 'string') {
        tabOptions = {
            id: arguments[0],
            iframeUrl: arguments[1],
            title: arguments[2],
            icon: arguments[3],
            createToolbar: arguments[4],
            refreshWhenExist: arguments[5],
            iconFont: arguments[6]
        };
    }

    F.addMainTab(F(PARAMS.mainTabStrip), tabOptions, actived);
}


// 移除选中标签页
function removeActiveTab() {
    var mainTabStrip = F(PARAMS.mainTabStrip);

    var activeTab = mainTabStrip.getActiveTab();
    activeTab.hide();
}

// 获取当前激活选项卡的ID
function getActiveTabId() {
    var mainTabStrip = F(PARAMS.mainTabStrip);

    var activeTab = mainTabStrip.getActiveTab();
    if (activeTab) {
        return activeTab.id;
    }
    return '';
}

// 激活选项卡，并刷新其中的内容，示例：表格控件->杂项->在新标签页中打开（关闭后刷新父选项卡）
function activeTabAndRefresh(tabId) {
    var mainTabStrip = F(PARAMS.mainTabStrip);
    var targetTab = mainTabStrip.getTab(tabId);
    var oldActiveTabId = getActiveTabId();

    if (targetTab) {
        mainTabStrip.activeTab(targetTab);
        targetTab.refreshIFrame();

        // 删除之前的激活选项卡
        mainTabStrip.removeTab(oldActiveTabId);
    }
}

// 激活选项卡，并刷新其中的内容，示例：表格控件->杂项->在新标签页中打开（关闭后更新父选项卡中的表格）
function activeTabAndUpdate(tabId, param1) {
    var mainTabStrip = F(PARAMS.mainTabStrip);
    var targetTab = mainTabStrip.getTab(tabId);
    var oldActiveTabId = getActiveTabId();

    if (targetTab) {
        mainTabStrip.activeTab(targetTab);
        targetTab.getIFrameWindow().updatePage(param1);

        // 删除之前的激活选项卡
        mainTabStrip.removeTab(oldActiveTabId);
    }
}

// 通知框
function notify(msg) {
    F.notify({
        message: msg,
        messageIcon: 'information',
        target: '_top',
        header: false,
        displayMilliseconds: 3 * 1000,
        positionX: 'center',
        positionY: 'center'
    });
}

// 点击菜单样式
function onMenuStyleCheckChange(event, item, checked) {
    var menuStyle = item.getAttr('data-tag');

    setCookie('MenuStyle', menuStyle);
    top.window.location.reload();
}

// 点击显示模式
function onMenuDisplayModeCheckChange(event, item, checked) {
    var displayMode = item.getAttr('data-tag');

    setCookie('DisplayMode', displayMode);
    top.window.location.reload();
}

// 点击语言
function onMenuLangCheckChange(event, item, checked) {
    var lang = item.getAttr('data-tag');

    setCookie('Language', lang);
    top.window.location.reload();
}

// 点击选项卡标签页
function onMenuMainTabsCheckChange(event, item, checked) {
    var mainTabs = item.getAttr('data-tag');

    setCookie('MainTabs', mainTabs);
    top.window.location.reload();
}



// 示例数
function getExamplesCount() {
    var hfExamplesCount = F(PARAMS.hfExamplesCount);
    if(hfExamplesCount) {
        return hfExamplesCount.getValue();
    } else {
        return F(PARAMS.treeMenu).getNodeCount(true);
    }
}

var THEMES = ["Pure_Black", "Pure_Green", "Pure_Blue", "Pure_Purple", "Pure_Orange", "Pure_Red", "Default", "Metro_Blue", "Metro_Dark_Blue", "Metro_Gray", "Metro_Green", "Metro_Orange", "Black_Tie", "Blitzer", "Cupertino", "Dark_Hive", "Dot_Luv", "Eggplant", "Excite_Bike", "Flick", "Hot_Sneaks", "Humanity", "Le_Frog", "Mint_Choc", "Overcast", "Pepper_Grinder", "Redmond", "Smoothness", "South_Street", "Start", "Sunny", "Swanky_Purse", "Trontastic", "UI_Darkness", "UI_Lightness", "Vader", "custom_default", "bootstrap_pure", "custom_light_green", "image_black_sky", "image_green_rain", "image_green_drip", "image_green_poppy", "image_green_lotus", "image_blue_sky", "image_blue_star", "image_blue_moon", "image_blue_drip", "image_purple_fog", "image_orange_light", "image_red_dawn"];
// 转到下一个主题
function nextThemePlease() {
    var currentTheme = F.cookie('Theme');
    var currentThemeIndex = $.inArray(currentTheme, THEMES);
    if (currentThemeIndex != -1) {
        currentThemeIndex++;
        if (currentThemeIndex >= THEMES.length) {
            currentThemeIndex = 0;
        }
        F.cookie('Theme', THEMES[currentThemeIndex], {
            expires: 100  // 单位：天
        });
        window.location.reload();
    }
}

function generateBreadcrumbHtml(treeMenu, nodeId) {
    var result = [];
    var nodePaths = treeMenu.getNodePath(nodeId).split('/');
    if (nodePaths && nodePaths.length) {
        var nodePathLength = nodePaths.length;
        $.each(nodePaths, function (index, item) {
            if (item === 'root') {
                //result.push('<span class="breadcrumb-root">首页</span>');
            } else {
                var cls = 'breadcrumb-text';
                if (index === nodePathLength - 1) {
                    cls = 'breadcrumb-last';
                }
                var itemData = treeMenu.getNodeData(item);
                result.push('<span class="' + cls + '">' + itemData.text + '</span>');
            }
        });
    }
    return result.join('<span class="breadcrumb-separator">/</span>');
}


F.ready(function () {
    var mainTabStrip = F(PARAMS.mainTabStrip);
    var treeMenu = F(PARAMS.treeMenu);
    if (!treeMenu) return;

    // 纯JS示例只支持智能树菜单
    if (F.product === 'F.js') {
        _menuStyle = 'tree';
    }


    // 初始化主框架中的树(或者Accordion+Tree)和选项卡互动，以及地址栏的更新
    // treeMenu： 主框架中的树控件实例，或者内嵌树控件的手风琴控件实例
    // mainTabStrip： 选项卡实例
    // options: 参数
    // options.updateHash： 切换Tab时，是否更新地址栏Hash值（默认值：true）
    // options.refreshWhenExist： 添加选项卡时，如果选项卡已经存在，是否刷新内部IFrame（默认值：false）
    // options.refreshWhenTabChange: 切换选项卡时，是否刷新内部IFrame（默认值：false）
    // options.maxTabCount: 最大允许打开的选项卡数量
    // options.maxTabMessage: 超过最大允许打开选项卡数量时的提示信息
    // options.beforeNodeClick: 节点点击事件之前执行（返回false则不执行点击事件）
    // options.beforeTabAdd: 添加选项卡之前执行（返回false则不添加选项卡）
    // options.moveToEnd: 将选项卡移到尾部（如果选项卡已存在，则不改变位置，默认值：false）
    var initOptions = {
        moveToEnd: true,
        maxTabCount: 10,
        maxTabMessage: '请先关闭一些选项卡（最多允许打开 10 个）！',
        beforeNodeClick: function (event, treeNodeId) {
            var nodeEl = treeMenu.getNodeEl(treeNodeId);
            var nodeTag = nodeEl.attr('data-tag');
            var nodeData = treeMenu.getNodeData(treeNodeId);
            if (nodeTag === 'pop-window1') {
                F(PARAMS.windowThemeRoller).show();
                return false;
            } else if (nodeTag === 'newtab') {
                window.open(nodeData.href, '_blank');
                return false;
            }
        },
        beforeTabAdd: function (event, tabOptions, treeNodeId) {
            // 手工调用F.addMainTab也会运行到这里，由于不是点击左侧树节点触发的，所以此时treeNodeId为空
            if (!treeNodeId) {
                return;
            }
            var nodeEl = treeMenu.getNodeEl(treeNodeId);
            var nodeTag = nodeEl.attr('data-tag');
            if (nodeTag === 'custom-title') {
                var parentNode = treeMenu.getParentData(treeNodeId);
                tabOptions.title = parentNode.text + ' - ' + nodeEl.text();
            }

            // 单标签页 - 显示当前页面所在的路径
            if (_mainTabs === 'single') {
                $('#breadcrumb .breadcrumb-inner').html(generateBreadcrumbHtml(treeMenu, treeNodeId));
            }
        }
    };

    // 是否单标签页
    if (_mainTabs === 'single') {
        $('body').addClass('maintabs-single');

        // 1. 单标签页标识符  2. 标签页存在则直接更新（因为只有这一个标签页）
        $.extend(initOptions, {
            singleTabId: PARAMS.tabHomepage,
            refreshWhenExist: true
        });
    }

    F.initTreeTabStrip(treeMenu, mainTabStrip, initOptions);


    // 如果地址哈希值不存在，则添加响应式首页
    var hashFragment = window.location.hash.substr(1);
    if (!hashFragment || hashFragment.indexOf(PARAMS.mainUrl) >= 0) {
        addExampleTabByHref(PARAMS.dashboardUrl);
    }


    //addExampleTabByHref("/iframe/grid_iframe.aspx");
    //addExampleTabByHref("/toolbar/toolbar_multi.aspx");
    //addExampleTabByHref("/layout/vbox.aspx");
    //addExampleTabByHref("/button/button.aspx");

    // 下一个主题
    //:: 页面按键事件
    $(document).on('keydown', function (event) {
        //:: Shift + L
        if (event.shiftKey && event.keyCode === 76) {
            nextThemePlease();
        }
    });

});

