function BackendService() {
    this.token = null;
}

// ---------------------------------------------------------------------------------
// Login
// ---------------------------------------------------------------------------------
BackendService.prototype.login = function (loginData, onSuccess, onError) {
    var self = this;
    $.ajax({
        type: "POST",
        url: document.app.getConfig().apiUrl + '/login',
        data: JSON.stringify(loginData),
        success: function (response) {
            self.token = response.token;
            localStorage.setItem('jwt_token', response.token);
            onSuccess(response);
        },
        error: function (xhr) {
            onError();
        },
        dataType: 'json',
        contentType: "application/json"
    });
};

BackendService.prototype.validateJwt = function (jwtToken, onSuccess) {
    var self = this;
    $.ajax({
        type: "POST",
        url: document.app.getConfig().apiUrl + '/login/jwt',
        data: JSON.stringify({ JwtToken: jwtToken }),
        success: function (response) {
            self.token = jwtToken;
            onSuccess(response);
        },
        dataType: 'json',
        contentType: "application/json"
    });
};

BackendService.prototype.registerUser = function (email, password, onSuccess, onError) {
    var self = this;
    $.ajax({
        type: "POST",
        url: document.app.getConfig().apiUrl + '/login/signup',
        data: JSON.stringify({ email: email, password: password }),
        success: function (response) {
            self.token = response.token;
            localStorage.setItem('jwt_token', response.token);
            onSuccess(response);
        },
        error: function (xhr) {
            onError(xhr.status);
        },
        dataType: 'json',
        contentType: "application/json"
    });
}

// ---------------------------------------------------------------------------------
// Device
// ---------------------------------------------------------------------------------
BackendService.prototype.registerDevice = function (device, onSuccess) {
    var self = this;
    $.ajax({
        type: "POST",
        url: document.app.getConfig().apiUrl + '/device',
        data: JSON.stringify(device),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', self.token);
        },
        success: function () {
            onSuccess();
        },
        error: function (xhr) {

            if (xhr.status != 201) {
                console.error('Backend service call failure');
                console.log(xhr);
            }

            if (xhr.status == 201) {
                onSuccess();
            }
        },
        dataType: 'json',
        contentType: "application/json"
    });
}

BackendService.prototype.getDevices = function (onSuccess) {
    var self = this;
    $.ajax({
        type: "GET",
        url: document.app.getConfig().apiUrl + '/device',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', self.token);
        },
        success: function (data) {
            onSuccess(data);
        },
        dataType: 'json',
        contentType: "application/json"
    });
}

BackendService.prototype.getDeviceActivity = function (deviceId, onSuccess) {
    var self = this;
    $.ajax({
        type: "GET",
        url: document.app.getConfig().apiUrl + '/device/' + deviceId + '/activity',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', self.token);
        },
        success: function (lastSeen, resp) {
            onSuccess(resp == 'nocontent' ? -1 : lastSeen);
        },
        dataType: 'json',
        contentType: "application/json"
    });
}

BackendService.prototype.getDeviceLogs = function (deviceId, onSuccess) {
    var self = this;
    $.ajax({
        type: "GET",
        url: document.app.getConfig().apiUrl + '/device/' + deviceId + '/logs',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', self.token);
        },
        success: function (logs) {
            onSuccess(logs);
        },
        dataType: 'json',
        contentType: "application/json"
    });
}

// ---------------------------------------------------------------------------------
// Device Config
// ---------------------------------------------------------------------------------
BackendService.prototype.getDeviceConfig = function (deviceId, onSuccess) {
    var self = this;
    $.ajax({
        type: "GET",
        url: document.app.getConfig().apiUrl + '/device/' + deviceId + '/config?origin=web',
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', self.token);
        },
        success: function (data) {
            onSuccess(data);
        },
        dataType: 'json',
        contentType: "application/json"
    });
}

BackendService.prototype.createOrUpdateDeviceConfig = function (deviceId, deviceConfig, onSuccess) {
    var self = this;
    $.ajax({
        type: "PUT",
        url: document.app.getConfig().apiUrl + '/device/' + deviceId + '/config',
        data: JSON.stringify(deviceConfig),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', self.token);
        },
        success: function () {
            onSuccess();
        },
        error: function (xhr) {
            console.log(xhr);
        },
        dataType: 'json',
        contentType: "application/json"
    });
}

BackendService.prototype.updateDevice = function (deviceId, device, onSuccess) {
    var self = this;
    $.ajax({
        type: "PUT",
        url: document.app.getConfig().apiUrl + '/device/' + deviceId,
        data: JSON.stringify(device),
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', self.token);
        },
        success: function () {
            onSuccess();
        },
        error: function (xhr) {
            console.log(xhr);
        },
        dataType: 'json',
        contentType: "application/json"
    });
}

BackendService.prototype.deleteDevice = function (deviceId, onSuccess) {
    var self = this;
    $.ajax({
        type: "DELETE",
        url: document.app.getConfig().apiUrl + '/device/' + deviceId,
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Authorization', self.token);
        },
        success: function () {
            onSuccess();
        },
        error: function (xhr) {
            console.log(xhr);
        },
        dataType: 'json',
        contentType: "application/json"
    });
}