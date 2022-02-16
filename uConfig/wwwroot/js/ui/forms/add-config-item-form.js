function AddConfigItemForm() {

}

AddConfigItemForm.prototype.getFormData = function () {
    return {
        key: $('#config_add_key').val(),
        value: $('#config_add_value').val(),
    };
}

AddConfigItemForm.prototype.submit = function () {
    var formData = this.getFormData();
    var existingDeviceConfig = document.app.screens.configurationScreen.deviceConfig;

    var existingItem = existingDeviceConfig.items.find(function (deviceConfigItem) {
        return deviceConfigItem.key == formData.key;
    });

    if (existingItem != null) {
        toastr["warning"]("Key already exist");
        return;
    }

    existingDeviceConfig.items.push(formData);
    document.app.services.backendService.createOrUpdateDeviceConfig(
        document.app.screens.configurationScreen.device.deviceID,
        existingDeviceConfig,
        function () {
            document.app.services.backendService.getDeviceConfig(document.app.screens.configurationScreen.device.deviceID, function (deviceConfig) {
                document.app.screens.configurationScreen.deviceConfig = deviceConfig;
                document.app.screens.configurationScreen.renderKeyValueList();
                document.app.screens.configurationScreen.refreshSampleCode();
            });
        }
    );
}