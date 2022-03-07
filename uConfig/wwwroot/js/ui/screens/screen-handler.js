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
    $('#app_navbar').hide();
    this.context = context;

    console.log('[screen] Changed to "' + screenName + '" with context: ' + context);

    switch (screenName) {
        case 'login':
            document.app.screens.loginScreen.autologin();
            break;
        case 'devices':
            $('#app_navbar').show();
            break;
        case 'configuration':
            $('#app_navbar').show();
            document.app.screens.configurationScreen.device =
                document.app.state.devices.find(function (device) { return device.deviceID == context });
            document.app.screens.configurationScreen.load();
            break;
    }
}