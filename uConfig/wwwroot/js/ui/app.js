function App() {
    this.backendService = new BackendService();
    this.addDeviceForm = new AddDeviceForm();

    // ui
    this.registeredDeviceList = new RegisteredDeviceList('#registered-device-list', '#registered-device-template');
}

App.prototype.getConfig = function () {
    return {
        apiUrl: 'http://127.0.0.1:8080/api'
    };
}

App.prototype.submitNewDevice = function () {
    var formData = this.addDeviceForm.getFormData();
    var self = this;

    this.backendService.registerDevice(formData, function () {
        console.log('Device is now registered');
        self.addDeviceForm.clear();

        toastr["success"]("Device registered successfully");
        document.app.fetchDevices();
    });
}

App.prototype.fetchDevices = function () {
    var self = this;
    this.backendService.getDevices(function (devices) {
        self.registeredDeviceList.refresh(devices);
    });
}

App.prototype.init = function () {
    // empty-device-list indicator should be hidden by default
    this.registeredDeviceList.hideNone();

    // make sure that device list is loaded upon page init
    this.fetchDevices();

    // hide add device form by default
    this.addDeviceForm.hide();
}
