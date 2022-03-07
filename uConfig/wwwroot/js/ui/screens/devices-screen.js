function DevicesScreen() {
    this.name = 'devices';
    this.registeredDeviceList = new RegisteredDeviceList('#registered-device-list', '#registered-device-template');
    this.addDeviceForm = new AddDeviceForm();
}

DevicesScreen.prototype.init = function () {
    this.registeredDeviceList.hideNone();
    this.addDeviceForm.hide();
    $('#registered-device-list-loading').hide();
}

DevicesScreen.prototype.submitNewDevice = function () {

    if (!this.addDeviceForm.validate()) {
        return;
    }

    var formData = this.addDeviceForm.getFormData();
    var self = this;

    document.app.services.backendService.registerDevice(formData, function () {
        console.log('Device is now registered');
        self.addDeviceForm.clear();

        toastr["success"]("Device registered successfully");
        self.fetchDevices();
        self.addDeviceForm.hide();
    });
}

DevicesScreen.prototype.fetchDevices = function () {
    var self = this;
    $('#registered-device-list-loading').show();
    document.app.services.backendService.getDevices(function (devicesResponse) {
        document.app.state.devices = devicesResponse.devices;
        document.app.state.deviceIdLastSeen = devicesResponse.deviceIdLastSeen;
        self.registeredDeviceList.refresh(document.app.state.devices);
        $('#registered-device-list-loading').hide();
    });
}