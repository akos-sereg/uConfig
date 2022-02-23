function App() {
    // services
    this.services = {
        backendService: new BackendService()
    }

    // screens
    this.screens = {
        screenHandler: new ScreenHandler(),
        loginScreen: new LoginScreen(),
        devicesScreen: new DevicesScreen(),
        configurationScreen: new ConfigurationScreen()
    }
    
    // state
    this.state = {
        loggedInUser: null,
        devices: []
    };
}

App.prototype.getConfig = function () {
    return {
        apiUrl: 'http://127.0.0.1:8080/api',
        consoleLogPollInterval: 10, // seconds
        detailsPollInterval: 10, // seconds
    };
}

App.prototype.init = function () {
    var self = this;

    // default screen
    this.screens.screenHandler.showScreen('login');
    this.screens.devicesScreen.init();
}
