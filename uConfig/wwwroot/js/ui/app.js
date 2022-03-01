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
        // apiUrl: 'http://127.0.0.1:8080/api',
        // apiUrl: 'https://uconfy.herokuapp.com/api',
        apiUrl: '/api',
        consoleLogPollInterval: 10, // seconds
        detailsPollInterval: 10, // seconds
    };
}

App.prototype.init = function () {
    this.screens.loginScreen.init();
    this.screens.devicesScreen.init();

    this.screens.screenHandler.showScreen('login');
}
