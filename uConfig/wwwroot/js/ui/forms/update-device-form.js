function UpdateDeviceForm() {

}

UpdateDeviceForm.prototype.getFormData = function () {
    return {
        name: $('#config_device_name').val(),
        platform: $('#config_device_platform').val()
    }
}