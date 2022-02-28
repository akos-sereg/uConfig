function LoginScreen() {
    this.name = 'login';
}

LoginScreen.prototype.login = function () {
    var username = $('#login_username').val();
    var password = $('#login_password').val();

    document.app.services.backendService.login({ username: username, password: password }, function (loggedInUser) {
        document.app.state.loggedInUser = loggedInUser;

        // user is now logged in, we can fetch devices now
        document.app.screens.screenHandler.showScreen('devices');
        document.app.screens.devicesScreen.fetchDevices();

        toastr["success"]("Logged in successfully.");
    },
    function () {
        toastr["warning"]("Login failed, try with different credentials.");
    });
}