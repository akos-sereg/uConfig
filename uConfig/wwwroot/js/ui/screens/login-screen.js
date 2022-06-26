function LoginScreen() {
    this.name = 'login';
}

LoginScreen.prototype.init = function () {
    $('#login_password').on('keyup', function (e) {
        if (e.key === 'Enter' || e.keyCode === 13) {
            document.app.screens.loginScreen.login();
        }
    });

    $('#login_password_confirm').on('keyup', function (e) {
        if (e.key === 'Enter' || e.keyCode === 13) {
            document.app.screens.loginScreen.signup();
        }
    });
}

LoginScreen.prototype.logout = function (e) {
    localStorage.removeItem('jwt_token');
    document.location.href = '/';
    e.preventDefault();
}

LoginScreen.prototype.showLogin = function () {
    $('#signup_password_confirm').hide();
    $('#signup_link').show();
    $('#login_link').hide();
    $('#login_signup_btn').html('Login');
    $('#login_signup_btn').data('cmd', 'login');
}

LoginScreen.prototype.showCreateAccount = function () {
    $('#signup_password_confirm').show();
    $('#signup_link').hide();
    $('#login_link').show();
    $('#login_signup_btn').html('Signup');
    $('#login_signup_btn').data('cmd', 'signup');
}

LoginScreen.prototype.autologin = function () {
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
}

LoginScreen.prototype.autologinDemoAccount = function () {
    // log in with demo user and empty password: login endpoint will return a valid JWT token for the demo user,
    // but with read-only access (eg. role is "demo")
    document.app.services.backendService.login({ username: 'demouser@demo.de', password: '' }, function (loggedInUser) {
        document.app.screens.loginScreen.onSuccessfulLogin(loggedInUser);
    },
    function () {
        toastr["warning"]("Login failed with demo account");
    });
}

LoginScreen.prototype.loginOrSignup = function () {
    var buttonCommand = $('#login_signup_btn').data('cmd');
    if (buttonCommand == 'login') {
        this.login();
    } else if (buttonCommand == 'signup') {
        this.signup();
    }
}

LoginScreen.prototype.signup = function () {
    var username = $('#login_username').val();
    var password = $('#login_password').val();
    var password_confirm = $('#login_password_confirm').val();

    if (username == '' || username.indexOf('@') == -1) {
        toastr["warning"]("Email is invalid");
        return;
    }

    if (password.length < 6) {
        toastr["warning"]("Password length must be minimum 6 characters");
        return;
    }

    if (password != password_confirm) {
        toastr["warning"]("Password mismatch");
        return;
    }

    document.app.services.backendService.registerUser(
        username,
        password,
        function (loggedInUser) {
            toastr["success"]("Signed up successfully");
            document.app.screens.loginScreen.onSuccessfulLogin(loggedInUser);
        },
        function (statusCode) {
            switch (statusCode) {
                case 400:
                    toastr["warning"]("Registration data is not okay: password length or password mismatch");
                    break;
                case 409:
                    toastr["warning"]("User is already registered with this email, try with different email address.");
                    break;
                default:
                    toastr["warning"]("An error occurred while registering. Please try again later.");
                    break;
            }
            
        });
}

LoginScreen.prototype.login = function () {
    var username = $('#login_username').val();
    var password = $('#login_password').val();

    document.app.services.backendService.login({ username: username, password: password }, function (loggedInUser) {
        document.app.screens.loginScreen.onSuccessfulLogin(loggedInUser);
    },
    function () {
        toastr["warning"]("Login failed, try with different credentials.");
    });
}

LoginScreen.prototype.onSuccessfulLogin = function (loggedInUser) {
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
}