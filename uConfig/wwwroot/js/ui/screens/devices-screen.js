function DevicesScreen() {
    this.name = 'devices';
    this.registeredDeviceList = new RegisteredDeviceList('#registered-device-list', '#registered-device-template');
    this.addDeviceForm = new AddDeviceForm();
}

DevicesScreen.prototype.init = function () {
    this.registeredDeviceList.hideNone();
    this.addDeviceForm.hide();
    this.fetchDevices();
}

DevicesScreen.prototype.submitNewDevice = function () {
    var formData = this.addDeviceForm.getFormData();
    var self = this;

    document.app.services.backendService.registerDevice(formData, function () {
        console.log('Device is now registered');
        self.addDeviceForm.clear();

        toastr["success"]("Device registered successfully");
        self.fetchDevices();
    });
}

DevicesScreen.prototype.fetchDevices = function () {
    var self = this;
    document.app.services.backendService.getDevices(function (devices) {
        document.app.state.devices = devices;
        self.registeredDeviceList.refresh(devices);
    });
}