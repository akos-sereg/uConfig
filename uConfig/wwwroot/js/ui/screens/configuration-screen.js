function ConfigurationScreen() {
    this.name = 'configuration';
    this.context = {};
    this.tabPages = new TabPages(
        '#tab_pages_configuration',
        '#tab_pages_config_nav',
        [
            {
                pageId: '#tab_pages_config_details',
                label: 'Details'
            },
            {
                pageId: '#tab_pages_config_access',
                label: 'Access'
            },
            {
                pageId: '#tab_pages_config_params',
                label: 'Params'
            },
            {
                pageId: '#tab_pages_config_console',
                label: 'Console'
            },
            {
                pageId: '#tab_pages_config_activity',
                label: 'Activity'
            }
        ]
    );
}

ConfigurationScreen.prototype.load = function () {
    this.tabPages.render();
}