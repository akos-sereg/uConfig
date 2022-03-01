function ScreenHandler() {
    this.screens = [
        new LoginScreen().name,
        new DevicesScreen().name,
        new ConfigurationScreen().name
    ];
}

ScreenHandler.prototype.showScreen = function (screenName, context) {
    this.screens.forEach(function (name) {
        $('#screen-' + name).hide();
    });

    $('#screen-' + screenName).show();
    this.context = context;

    console.log('[screen] Changed to "' + screenName + '" with context: ' + context);

    switch (screenName) {
        case 'login':
            var jwtToken = localStorage.getItem('jwt_token');
            if (jwtToken) {
                document.app.services.backendService.validateJwt(jwtToken, function (loggedInUser) {
                    document.app.state.loggedInUser = loggedInUser;

                    $("#screen-login").effect('fade', {}, 1000, function () {
                        $("#screen-login").hide();
                    });
                    setTimeout(function () {
                        document.app.screens.screenHandler.showScreen('devices');
                        document.app.screens.devicesScreen.fetchDevices();
                    }, 1000);
                    
                });
            }
            break;
        case 'devices':
            break;
        case 'configuration':
            document.app.screens.configurationScreen.device =
                document.app.state.devices.find(function (device) { return device.deviceID == context });
            document.app.screens.configurationScreen.load();
            break;
    }
}