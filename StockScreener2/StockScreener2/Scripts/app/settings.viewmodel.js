function SettingsViewModel(app, dataModel) {
    var self = this;

    Sammy(function () {
        this.get('#settings', function () {
            app.view(app.Views["Loading"]);
            console.log("settings are loaded.")
            console.log(app.Views);
            app.view(app.Views["Settings"]);
            console.log(app.view().constructor.name);
        });
        this.get('/Settings', function () { this.app.runRoute('get', '#settings') });
    });

    return self;
}

app.addViewModel({
    name: "Settings",
    bindingMemberName: "settings",
    factory: SettingsViewModel
});
