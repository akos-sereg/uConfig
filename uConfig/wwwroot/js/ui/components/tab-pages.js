function TabPages(containerId, navigationId, tabs) {
    this.containerId = containerId;
    this.navigationId = navigationId;
    this.tabs = tabs;

    $(this.containerId).hide();

    this.tabItemTemplate = '#tab_item_template';

    document.tabs[this.containerId] = this;
}

TabPages.prototype.render = function () {

    var self = this;
    this.eraseNavigation();

    // build navigation
    let tabItemTemplate = document.querySelector(this.tabItemTemplate);
    var navigation = document.querySelector(this.navigationId);
    this.tabs.forEach(function (tab) {
        let tabItem = tabItemTemplate.content.cloneNode(true).querySelector('span');
        tabItem.innerHTML = tab.label;
        tabItem.classList.add("tab_item_not_selected");
        tabItem.attributes.onclick.nodeValue =
            tabItem.attributes.onclick.nodeValue
                .replace('[tabComponent]', 'document.tabs[\'' + self.containerId + '\']')
                .replace('{tabId}', tab.pageId);

        tabItem.attributes['data-tab-id'].nodeValue = tabItem.attributes['data-tab-id'].nodeValue.replace('[tabId]', tab.pageId);
        navigation.append(tabItem);
    });

    let paddingLine = document.createElement('div');
    paddingLine.style.width = '100%';
    paddingLine.style.borderBottom = 'solid 1px #aaaaaa';
    navigation.append(paddingLine);

    if (this.tabs.length > 0) {
        navigation.children[0].classList.remove("tab_item_not_selected");
        navigation.children[0].classList.add("tab_item_selected");
        this.switch(this.tabs[0].pageId);
    }

    $(this.containerId).show();
}

TabPages.prototype.eraseNavigation = function () {
    document.querySelector(this.navigationId).innerHTML = '';
}

TabPages.prototype.switch = function (tabId) {
    var navigation = document.querySelector(this.navigationId);

    navigation.childNodes.forEach(function (tabHeader) {
        tabHeader.classList.remove('tab_item_selected');
        tabHeader.classList.remove('tab_item_not_selected');

        if (tabHeader.attributes['data-tab-id']) {
            if (tabHeader.attributes['data-tab-id'].nodeValue == tabId) {
                tabHeader.classList.add('tab_item_selected');
                $(tabHeader.attributes['data-tab-id'].nodeValue).show();
            } else {
                tabHeader.classList.add('tab_item_not_selected');
                $(tabHeader.attributes['data-tab-id'].nodeValue).hide();
            }
        }
        
    });
}

// ---------------------------------------------------------------------
// globals, used by this component
// ---------------------------------------------------------------------
document.tabs = {};