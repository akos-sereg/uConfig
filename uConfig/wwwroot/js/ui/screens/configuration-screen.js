function ConfigurationScreen() {
    this.name = 'configuration';
    this.device = {};
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

    // details tab
    $('#config_device_name').val(this.device.name);
    $('#config_device_platform').val(this.device.platform);

    // access tab
    $('#config_access_url').val(document.app.getConfig().apiUrl + '/device/' + this.device.deviceID + '/config?apiKey=' + document.app.state.loggedInUser.apiKey);
    $('#config_access_apikey').val(document.app.state.loggedInUser.apiKey);
}