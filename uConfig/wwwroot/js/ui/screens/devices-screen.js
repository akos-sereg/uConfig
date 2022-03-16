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
        self.addDeviceForm.hide();
        self.fetchDevices();

        // on production sometimes we have a bit of a delay (probably because of data propagation delay), so it is
        // better to just reload the data once again after 1500 ms
        setTimeout(self.fetchDevices, 1500);
    });
}

DevicesScreen.prototype.fetchDevices = function () {
    $('#registered-device-list-loading').show();
    document.app.services.backendService.getDevices(function (devicesResponse) {
        document.app.state.devices = devicesResponse.devices;
        document.app.state.deviceIdLastSeen = devicesResponse.deviceIdLastSeen;
        document.app.screens.devicesScreen.registeredDeviceList.refresh(document.app.state.devices);
        $('#registered-device-list-loading').hide();
    });
}