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
        apiUrl: '/api',
        consoleLogPollInterval: 10, // seconds
        detailsPollInterval: 10, // seconds

        // if lastSeen seconds exceeds this value, device is considered as disconnected
        deviceInactiveInterval: 86400,

        // read-only demo account's JWT token
        demoAccountJwt: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxMjQwIiwiZW1haWwiOiJkZW1vdXNlckBkZW1vLmRlIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvc2lkIjoiNTkwZGRlNTktNGI4My00M2NiLThlY2UtMWM3MWQ1ZGNiN2M3Iiwicm9sZSI6ImRlbW8iLCJuYmYiOjE2NDc1NDQ3ODgsImV4cCI6MTY3OTA4MDc4OCwiaWF0IjoxNjQ3NTQ0Nzg4LCJpc3MiOiJodHRwOi8vbXlzaXRlLmNvbSIsImF1ZCI6Imh0dHA6Ly9teWF1ZGllbmNlLmNvbSJ9.bvomBIkfN3Aymum6MxQY6jdujHrCff1pn3to9jOGqo8'
    };
}

App.prototype.init = function () {
    this.screens.loginScreen.init();
    this.screens.devicesScreen.init();

    this.screens.screenHandler.showScreen('login');
    if (document.location.hash == '#signup') {
        this.screens.loginScreen.showCreateAccount();
    }
    if (document.location.hash == '#autologinDemo') {
        this.screens.loginScreen.autologinDemoAccount();
    }
}
