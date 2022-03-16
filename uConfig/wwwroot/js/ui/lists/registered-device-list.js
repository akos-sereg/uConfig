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
        deviceLastSeen = document.app.state.deviceIdLastSeen[registeredDevice.deviceID];
        if (deviceLastSeen == -1) {
            record.querySelector('#device-card-online-indicator').classList.add('dot-gray');
            record.querySelector('#device-card-subtitle').textContent = "Device not connected";
        } else if (deviceLastSeen < 84000) {
            record.querySelector('#device-card-online-indicator').classList.add('dot-green');
            record.querySelector('#device-card-subtitle').textContent = "Device was active in the last 24 hours";
        } else {
            record.querySelector('#device-card-online-indicator').classList.add('dot-yellow');
            record.querySelector('#device-card-subtitle').textContent = "Device seen " + Math.floor(deviceLastSeen / 84000) + " days ago";
        }
        record.querySelector('#configure_device_btn').attributes.onclick.nodeValue =
            record.querySelector('#configure_device_btn')
                .attributes
                .onclick
                .nodeValue
                .replace('{deviceId}', registeredDevice.deviceID);
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