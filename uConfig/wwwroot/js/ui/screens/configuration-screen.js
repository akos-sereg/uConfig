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
            }
        ]
    );
}

ConfigurationScreen.prototype.load = function () {
    this.tabPages.render();
}