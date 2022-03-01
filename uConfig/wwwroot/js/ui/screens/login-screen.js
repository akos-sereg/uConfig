function LoginScreen() {
    this.name = 'login';
}

LoginScreen.prototype.init = function () {
    $('#login_password').on('keyup', function (e) {
        if (e.key === 'Enter' || e.keyCode === 13) {
            document.app.screens.loginScreen.login();
        }
    });
}

LoginScreen.prototype.login = function () {
    var username = $('#login_username').val();
    var password = $('#login_password').val();

    document.app.services.backendService.login({ username: username, password: password }, function (loggedInUser) {
        document.app.state.loggedInUser = loggedInUser;

        // user is now logged in, we can fetch devices now
        $("#screen-login").effect('fade', {}, 1000, function () {
            $("#screen-login").hide();
        });
        setTimeout(function () {
            document.app.screens.screenHandler.showScreen('devices');
            document.app.screens.devicesScreen.fetchDevices();
        }, 1000);
        

        toastr["success"]("Logged in successfully.");
    },
    function () {
        toastr["warning"]("Login failed, try with different credentials.");
    });
}