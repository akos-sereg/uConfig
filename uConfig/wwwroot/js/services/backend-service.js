function BackendService() {

}

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
            console.log(data);
            onSuccess(data);
        },
        dataType: 'json',
        contentType: "application/json"
    });
}