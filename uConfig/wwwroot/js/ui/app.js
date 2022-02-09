function App() {
    this.backendService = new BackendService();
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
    this.backendService.getDevices(function (data) {
    });
}