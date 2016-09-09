function HomeViewModel(app, dataModel) {
    var self = this;

    //Array of stocks
    self.stocks = ko.observableArray();

    //For adding a new symbol
    self.stockSymbol = ko.observable("");
    self.addStock = function () {
        console.log(self.stockSymbol());
        $.ajax({
            method: 'post',
            url: '/api/Stocks?stockSymbol=' + self.stockSymbol(),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (data) {
                console.log("success:");
                console.log(data);
                self.stocks.push(
                            {
                                name: data.name,
                                symbol: data.symbol,
                                change: data.stockPrices[0].change,
                                bid: data.stockPrices[0].bid,
                                ask: data.stockPrices[0].ask,
                                daysLow: data.stockPrices[0].daysLow,
                                daysHigh: data.stockPrices[0].daysHigh,
                                open: data.stockPrices[0].open,
                                close: data.stockPrices[0].close,
                                lastUpdate: new Date(data.stockPrices[0].created).toUTCString().slice(16, 25)
                            }
                            );
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert(xhr.status);
                alert(thrownError);
            }
        });
    }

    Sammy(function () {
        this.get('#home', function () {
            $.ajax({
                method: 'get',
                url: '/api/Stocks',
                contentType: "application/json; charset=utf-8",
                headers: {
                    'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
                },
                success: function (data) {
                    console.log("success:");
                    console.log(data);

                    data.forEach(function (stock) {
                        self.stocks.push(
                            {
                                name: stock.name,
                                symbol: stock.symbol,
                                change: stock.stockPrices[0].change,
                                bid: stock.stockPrices[0].bid,
                                ask: stock.stockPrices[0].ask,
                                daysLow: stock.stockPrices[0].daysLow,
                                daysHigh: stock.stockPrices[0].daysHigh,
                                open: stock.stockPrices[0].open,
                                close: stock.stockPrices[0].close,
                                lastUpdate: new Date(stock.stockPrices[0].created).toString().slice(16, 25)
                            }
                            );
                    });
                    self.stocks().forEach(function (stock) {
                        console.log(stock);
                    });
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
