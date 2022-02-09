function App() {
    this.backendService = new BackendService();

    // ui
    this.registeredDeviceList = new RegisteredDeviceList('#registered-device-list', '#registered-device-template');
}

App.prototype.getConfig = function () {
    return {
        apiUrl: 'http://127.0.0.1:8080/api'
    };
}

App.prototype.submitNewDevice = function () {
    var form = new AddDeviceForm();
    var formData = form.getFormData();

    this.backendService.registerDevice(formData, function () {
        console.log('Device is now registered');
        form.clear();

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
