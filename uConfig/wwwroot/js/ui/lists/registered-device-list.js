function RegisteredDeviceList(placeholderId, templateId) {
    this.placeholderId = placeholderId;
    this.templateId = templateId;
}

RegisteredDeviceList.prototype.clear = function () {
    document.querySelector(this.placeholderId).innerHTML = '';
}

RegisteredDeviceList.prototype.refresh = function (devices) {

    this.clear();

    let app = document.querySelector(this.placeholderId);
    let listItem = document.querySelector(this.templateId);
    let list = document.createElement('div');
    list.classList.add("registered_device_container");

    for (let registeredDevice of devices) {
        let record = listItem.content.cloneNode(true).querySelector('div');

        record.classList.add("registered_device_item");
        record.querySelector('#device-card-title').textContent = registeredDevice.name;
        list.append(record);

        app.append(list);
    }

    devices.length > 0 ? this.hideNone() : this.showNone();
}

RegisteredDeviceList.prototype.showNone = function () {
    $('#add_device_none_yet').show();
}

RegisteredDeviceList.prototype.hideNone = function () {
    $('#add_device_none_yet').hide();
}