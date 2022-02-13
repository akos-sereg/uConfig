function ConfigurationScreen() {
    this.name = 'configuration';
    this.device = {};
    this.deviceConfig = { "Items": [] };
    this.addConfigItemForm = new AddConfigItemForm();
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
    var self = this;
    this.tabPages.render();

    // details tab
    $('#config_device_name').val(this.device.name);
    $('#config_device_platform').val(this.device.platform);

    // access tab
    $('#config_access_url').val(document.app.getConfig().apiUrl + '/device/' + this.device.deviceID + '/config?apiKey=' + document.app.state.loggedInUser.apiKey);
    $('#config_access_apikey').val(document.app.state.loggedInUser.apiKey);

    // params tab
    document.app.services.backendService.getDeviceConfig(this.device.deviceID, function (deviceConfig) {
        self.deviceConfig = deviceConfig;
        self.renderKeyValueList();
    });
}

ConfigurationScreen.prototype.renderKeyValueList = function () {
    var paramsContainer = document.querySelector('#config_list_container_tbody');
    paramsContainer.innerHTML = '';

    let kvPairTemplate = document.querySelector('#config_key_value');
    this.deviceConfig.items.forEach(function (kvPair) {
        let kvPairItem = kvPairTemplate.content.cloneNode(true).querySelector('tr');
        kvPairItem.querySelector('#config_kv_template_key').innerHTML = kvPair.key;
        kvPairItem.querySelector('#config_kv_template_value').innerHTML = kvPair.value;
        paramsContainer.append(kvPairItem);
    });

    let kvPairAddTemplate = document.querySelector('#config_key_value_add');
    let kvPairAdd = kvPairAddTemplate.content.cloneNode(true).querySelector('tr');
    paramsContainer.append(kvPairAdd);
}