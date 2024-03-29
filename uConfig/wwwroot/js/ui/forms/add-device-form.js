﻿function AddDeviceForm() {
    this.isDisplayed = false;
}

AddDeviceForm.prototype.getFormData = function () {
    var formData = {
        name: $('#add_device_name').val(),
        platform: $('#add_device_platform').val(),
    }

    return formData;
}

AddDeviceForm.prototype.clear = function () {
    $('#add_device_name').val('');
    $('#add_device_platform').val('');
}

AddDeviceForm.prototype.show = function () {
    $('#add_device_container').show();
    this.isDisplayed = true;
    $('#add_device_link').html('&lt; Add new device ...');
}

AddDeviceForm.prototype.validate = function () {
    var formData = this.getFormData();

    if (formData.name == '') {
        toastr["warning"]("Device name is required field");
        return false;
    }

    if (formData.platform == '') {
        toastr["warning"]("Device platform is required field");
        return false;
    }

    return true;
}

AddDeviceForm.prototype.hide = function () {
    $('#add_device_container').hide();
    this.isDisplayed = false;
    $('#add_device_link').html('&gt; Add new device ...');
}

AddDeviceForm.prototype.toggle = function () {
    if (this.isDisplayed) {
        this.hide();
    } else {
        this.show();
    }
}