function App() {
}

App.prototype.getConfig = function () {
    return {
        apiUrl: 'http://127.0.0.1:8080/api'
    };
}

App.prototype.submitNewDevice = function () {
    var form = new AddDeviceForm();
    var formData = form.getFormData();

    new BackendService().registerDevice(formData, function () {
        console.log('Device is now registered');
        form.clear();

        toastr["success"]("Device registered successfully");
    });
}