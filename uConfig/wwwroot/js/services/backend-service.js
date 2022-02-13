function BackendService() {
    this.token = null;
}

// ---------------------------------------------------------------------------------
// Login
// ---------------------------------------------------------------------------------
BackendService.prototype.login = function (loginData, onSuccess) {
    var self = this;
    $.ajax({
        type: "POST",
        url: document.app.getConfig().apiUrl + '/login',
        data: JSON.stringify(loginData),
        success: function (response) {
            self.token = response.token;
            onSuccess(response);
        },
        error: function (xhr) {
            console.error('Backend service call failure');
            console.log(xhr);
        },
        dataType: 'json',
        contentType: "application/json"
    });
}

// ---------------------------------------------------------------------------------
// Device
// ---------------------------------------------------------------------------------
BackendService.prototype.registerDevice = function (device, onSuccess) {
    $.ajax({
        type: "POST",
        url: document.app.getConfig().apiUrl + '/device',
        data: JSON.stringify(device),
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
    $.ajax({
        type: "GET",
        url: document.app.getConfig().apiUrl + '/device',
        success: function (data) {
            onSuccess(data);
        },
        dataType: 'json',
        contentType: "application/json"
    });
}

// ---------------------------------------------------------------------------------
// Device Config
// ---------------------------------------------------------------------------------
BackendService.prototype.getDeviceConfig = function (deviceId, onSuccess) {
    $.ajax({
        type: "GET",
        url: document.app.getConfig().apiUrl + '/device/' + deviceId + '/config',
        success: function (data) {
            onSuccess(data);
        },
        dataType: 'json',
        contentType: "application/json"
    });
}