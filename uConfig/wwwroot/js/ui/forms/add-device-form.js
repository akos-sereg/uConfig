function AddDeviceForm() {
}

AddDeviceForm.prototype.getFormData = function () {
    var formData = {
        name: $('#add_device_name').val(),
        platform: $('#add_device_platform').val(),
    }

    console.log(formData);
    return formData;
}

AddDeviceForm.prototype.clear = function () {
    $('#add_device_name').val('');
    $('#add_device_platform').val('');
}