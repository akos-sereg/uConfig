function App() {
    // services
    this.services = {
        backendService: new BackendService()
    }

    // screens
    this.screens = {
        screenHandler: new ScreenHandler(),
        devicesScreen: new DevicesScreen(),
        configurationScreen: new ConfigurationScreen()
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

App.prototype.init = function () {
    // default screen
    this.screens.screenHandler.showScreen('devices');

    // init device screen
    this.screens.devicesScreen.init();
}
