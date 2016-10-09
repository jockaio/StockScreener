function SettingsViewModel(app, dataModel) {
    var self = this;

    self.settings = ko.observableArray();
    self.setting = ko.observable();

    self.getSettings = function () {
        console.log("getSettings");
        $.ajax({
            method: 'get',
            url: '/api/Settings',
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (data) {
                data.forEach(function (setting) {
                    self.settings.push(
                        new Setting(setting)
                        );
                });
                console.log(self.settings());
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr);
                self.error(xhr.responseText);
                self.showError(true);
                setTimeout(function () { self.showError(false) }, 3000);
            }
        });
    }

    self.saveSetting = function () {
        $.ajax({
            method: 'post',
            data: self.setting(),
            url: '/api/Settings',
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (data) {
                console.log(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr);
                self.error(xhr.responseText);
                self.showError(true);
                setTimeout(function () { self.showError(false) }, 3000);
            }
        });
    }

    Sammy(function () {
        this.get('#settings', function () {
            app.view(app.Views["Loading"]);
            self.getSettings();
            app.view(app.Views["Settings"]);
        });
        this.get('/Settings', function () { this.app.runRoute('get', '#settings') });
    });

    return self;
}

function Setting(data) {
    self = this;
    self.id = ko.observable(data.id),
    self.calculationType = ko.observable(data.calculationType)
    self.operator = ko.observable(data.operator),
    self.targetValue = ko.observable(data.targetValue)
}

app.addViewModel({
    name: "Settings",
    bindingMemberName: "settings",
    factory: SettingsViewModel
});
