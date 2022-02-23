﻿function ConfigurationScreen() {
    var self = this;
    this.name = 'configuration';
    this.device = {};
    this.deviceConfig = { "Items": [] };
    this.addConfigItemForm = new AddConfigItemForm();
    this.updateDeviceForm = new UpdateDeviceForm();
    this.consolePoller = {
        deviceId: null,
        poller: null,
    };
    this.detailsPoller = {
        deviceId: null,
        poller: null,
    };

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
        ],
        function (tabId) {
            if (tabId == '#tab_pages_config_details') {
                if (self.detailsPoller.deviceId == self.device.deviceID) {
                    return;
                } else {
                    if (self.detailsPoller.poller) {
                        clearInterval(self.detailsPoller.poller);
                    }
                }

                function pollerJob() {
                    document.app.services.backendService.getDeviceActivity(self.device.deviceID, function (lastSeenInSeconds) {
                        if (lastSeenInSeconds == -1) {
                            $('#device_not_connected').show();
                            $('#device_connected').hide();
                        } else {
                            $('.device_last_seen').html(lastSeenInSeconds);
                            $('#device_connected').show();
                            $('#device_not_connected').hide();
                        }
                    });
                }

                pollerJob();
                self.detailsPoller.deviceId = self.device.deviceID;
                self.detailsPoller.poller = setInterval(pollerJob, document.app.getConfig().detailsPollInterval * 1000);
                
            } else if (tabId == '#tab_pages_config_console') {
                if (self.consolePoller.deviceId == self.device.deviceID) {
                    // poller is already running for the target device
                    return;
                } else {
                    // poller is running for a different device, canceling poller
                    if (self.consolePoller.poller) {
                        clearInterval(self.consolePoller.poller);
                    }
                }

                function pollerJob() {
                    document.app.services.backendService.getDeviceLogs(self.device.deviceID, function (logs) {
                        $('#console_logs').html(logs.map(function (logEntry) { return logEntry + '<br/>'; }));
                        var consoleLogs = document.getElementById('console_logs');
                        consoleLogs.scrollTop = consoleLogs.scrollHeight - consoleLogs.clientHeight;
                    });
                }

                pollerJob();
                self.consolePoller.deviceId = self.device.deviceID;
                self.consolePoller.poller = setInterval(pollerJob, document.app.getConfig().consoleLogPollInterval * 1000);
                
            }
        }
    );

    // polling for console logs
    
    
}

ConfigurationScreen.prototype.load = function () {
    var self = this;
    this.tabPages.render();

    // details tab
    $('#config_device_name').val(this.device.name);
    $('#config_device_platform').val(this.device.platform);
    $('#device_not_connected').hide();
    $('#device_connected').hide();

    // access tab
    $('#config_access_device_id').val(this.device.deviceID);
    $('#config_access_apikey').val(document.app.state.loggedInUser.apiKey);

    // params tab
    document.app.services.backendService.getDeviceConfig(this.device.deviceID, function (deviceConfig) {
        self.deviceConfig = deviceConfig;
        self.renderKeyValueList();

        // update sample code on access tab
        self.refreshSampleCode(deviceConfig);
    });
}

ConfigurationScreen.prototype.refreshSampleCode = function () {
    var sampleCode = $('#sample_code_template').html()
        .replace('{deviceId}', this.device.deviceID)
        .replace('{apiKey}', document.app.state.loggedInUser.apiKey);

    var paramsCode = this.deviceConfig.items.length == 0 ? '    // no key-value pair added yet. use Params tab to add config.\n' : '';
    this.deviceConfig.items.forEach(function (item) {
        const parsed = parseInt(item.value, 10);
        if (!isNaN(parsed)) {
            paramsCode += '    printf("' + item.key + ': %d\\n", uconfig_get_int_param("' + item.key + '", 1234));\n';
        }
        else {
            paramsCode += '    printf("' + item.key + ': %s\\n", uconfig_get_string_param("' + item.key + '", "aaaa"));\n';
        }
    });
    var sampleCode = sampleCode.replace('{getParamsCode}', paramsCode);
    $('#sample_code').html(sampleCode);
}

ConfigurationScreen.prototype.renderKeyValueList = function () {
    var paramsContainer = document.querySelector('#config_list_container_tbody');
    paramsContainer.innerHTML = '';

    let kvPairTemplate = document.querySelector('#config_key_value');
    this.deviceConfig.items.forEach(function (kvPair) {
        let kvPairItem = kvPairTemplate.content.cloneNode(true).querySelector('tr');
        kvPairItem.querySelector('#config_kv_template_key').innerHTML = kvPair.key;
        kvPairItem.querySelector('#config_kv_template_value').innerHTML = kvPair.value;
        kvPairItem.querySelector('#config_kv_template_remove').attributes.onclick.nodeValue = 
            kvPairItem.querySelector('#config_kv_template_remove').attributes.onclick.nodeValue.replace('{configKey}', kvPair.key);
        paramsContainer.append(kvPairItem);
    });

    let kvPairAddTemplate = document.querySelector('#config_key_value_add');
    let kvPairAdd = kvPairAddTemplate.content.cloneNode(true).querySelector('tr');
    paramsContainer.append(kvPairAdd);
}

ConfigurationScreen.prototype.deleteConfigItem = function (key) {
    var newDeviceConfig = JSON.parse(JSON.stringify(this.deviceConfig));

    newDeviceConfig.items = [];
    this.deviceConfig.items.forEach(function (item) {
        if (item.key != key) {
            newDeviceConfig.items.push(item);
        }
    });

    var self = this;
    document.app.services.backendService.createOrUpdateDeviceConfig(
        this.device.deviceID,
        newDeviceConfig,
        function () {
            document.app.services.backendService.getDeviceConfig(self.device.deviceID, function (deviceConfig) {
                self.deviceConfig = deviceConfig;
                self.renderKeyValueList();
            });
        }
    );
}

ConfigurationScreen.prototype.updateDevice = function () {
    var deviceData = this.updateDeviceForm.getFormData();
    deviceData.deviceId = this.device.deviceID;

    document.app.services.backendService.updateDevice(
        this.device.deviceID,
        deviceData,
        function () {
            toastr["success"]("Device updated successfully");
            document.app.screens.devicesScreen.fetchDevices();
        }
    );
}

ConfigurationScreen.prototype.deleteDevice = function () {
    document.app.services.backendService.deleteDevice(
        this.device.deviceID,
        function () {
            toastr["success"]("Device deleted successfully");

            document.app.screens.screenHandler.showScreen('devices', null);
            document.app.screens.devicesScreen.fetchDevices();
        }
    );
}