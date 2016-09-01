function HomeViewModel(app, dataModel) {
    var self = this;

    self.myHometown = ko.observable("");
    self.stockName = ko.observable("");

    Sammy(function () {
        //this.get('#home', function () {
        //    // Make a call to the protected Web API by passing in a Bearer Authorization Header
        //    $.ajax({
        //        method: 'get',
        //        url: app.dataModel.userInfoUrl,
        //        contentType: "application/json; charset=utf-8",
        //        headers: {
        //            'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
        //        },
        //        success: function (data) {
        //            console.log("Testing get hometown");
        //            self.myHometown('Your Hometown is : ' + data.hometown);
        //        }
        //    });
        //});
        this.get('#home', function () {
            $.ajax({
                method: 'get',
                url: '/api/Stocks',
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    console.log("success:");
                    console.log(data);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert(xhr.status);
                    alert(thrownError);
                }
                 });
        });
        this.get('/', function () { this.app.runRoute('get', '#home') });
    });

    return self;
}

app.addViewModel({
    name: "Home",
    bindingMemberName: "home",
    factory: HomeViewModel
});
