function RegisteredDeviceList(placeholderId, templateId) {
    this.placeholderId = placeholderId;
    this.templateId = templateId;
}

RegisteredDeviceList.prototype.clear = function () {
    document.querySelector(this.placeholderId).innerHTML = '';
}

RegisteredDeviceList.prototype.refresh = function (devices) {

    this.clear();

    let app = document.querySelector(this.placeholderId); // '#registered-device-list', #registered-device-template
    let listItem = document.querySelector(this.templateId);
    let list = document.createElement('div');

    for (let registeredDevice of devices) {
        let li = listItem.content.cloneNode(true).querySelector('div');

        li.textContent = registeredDevice.name;
        list.append(li);

        app.append(list);
    }

}