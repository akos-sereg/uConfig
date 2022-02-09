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