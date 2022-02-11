function App() {
    // services
    this.services = {
        backendService: new BackendService()
    }

    // ui
    this.ui = {
        registeredDeviceList: new RegisteredDeviceList('#registered-device-list', '#registered-device-template'),
        addDeviceForm: new AddDeviceForm(),
        screenHandler: new ScreenHandler()
    }
    
    // state
    this.state = {
        devices: []
    };
}

App.prototype.getConfig = function () {
    return {
        apiUrl: 'http://127.0.0.1:8080/api'
    };
}

App.prototype.submitNewDevice = function () {
    var formData = this.ui.addDeviceForm.getFormData();
    var self = this;

    this.services.backendService.registerDevice(formData, function () {
        console.log('Device is now registered');
        self.ui.addDeviceForm.clear();

        toastr["success"]("Device registered successfully");
        document.app.fetchDevices();
    });
}

App.prototype.fetchDevices = function () {
    var self = this;
    this.services.backendService.getDevices(function (devices) {
        self.state.devices = devices;
        self.ui.registeredDeviceList.refresh(devices);
    });
}

App.prototype.init = function () {
    // default screen
    this.ui.screenHandler.showScreen('devices');

    // empty-device-list indicator should be hidden by default
    this.ui.registeredDeviceList.hideNone();

    // make sure that device list is loaded upon page init
    this.fetchDevices();

    // hide add device form by default
    this.ui.addDeviceForm.hide();
}
