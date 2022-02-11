function ScreenHandler() {
    // container div ids: 'screen-<screen-name>', eg. 'screen-devices' or 'screen-configuration'
    this.screens = ['devices', 'configuration'];

    this.context = '';
    this.contextObject = {};
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
            this.contextObject = document.app.state.devices.find(function (device) { return device.deviceID == context });
            break;
    }
}