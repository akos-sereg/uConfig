function ScreenHandler() {
    this.screens = [
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
        case 'devices':
            break;
        case 'configuration':
            document.app.screens.configurationScreen.device =
                document.app.state.devices.find(function (device) { return device.deviceID == context });
            document.app.screens.configurationScreen.load();
            break;
    }
}