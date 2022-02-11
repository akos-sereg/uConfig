function ScreenHandler() {
    // container div ids: 'screen-<screen-name>', eg. 'screen-devices' or 'screen-configuration'
    this.screens = ['devices', 'configuration'];

    this.context = '';
}

ScreenHandler.prototype.showScreen = function (screenName, context) {
    this.screens.forEach(function (name) {
        $('#screen-' + name).hide();
    });

    $('#screen-' + screenName).show();
    this.context = context;

    console.log('[screen] Changed to "' + screenName + '" with context: ' + context);
}