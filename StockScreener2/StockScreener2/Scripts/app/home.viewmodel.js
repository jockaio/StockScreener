﻿function HomeViewModel(app, dataModel) {
    var self = this;

    //For error handling
    self.error = ko.observable("");
    self.showError = ko.observable(false);

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
                                id: data.id,
                                name: data.name,
                                symbol: data.symbol,
                                change: data.stockPrices[0].change,
                                bid: data.stockPrices[0].bid,
                                ask: data.stockPrices[0].ask,
                                daysLow: data.stockPrices[0].daysLow,
                                daysHigh: data.stockPrices[0].daysHigh,
                                open: data.stockPrices[0].open,
                                close: data.stockPrices[0].close,
                                last: data.stockPrices[0].last,
                                lastUpdate: new Date(data.stockPrices[0].created).toString().slice(16, 25)
                            }
                            );
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log("Error: ");
                console.log(xhr);
                console.log(ajaxOptions);
                console.log(thrownError);
                self.error(xhr.responseJSON);
                self.showError(true);
                setTimeout(function () { self.showError(false) }, 3000);
            }
        });
    }

    self.deleteStock = function () {
        var stock = this;
        $.ajax({
            method: 'delete',
            url: '/api/Stocks/' + this.id,
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (data) {
                self.stocks.remove(stock);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                self.error(thrownError);
                self.showError(true);
            }
        });
    }

    //show stock chart
    self.showChart = function () {
        $.ajax({
            method: 'get',
            url: '/api/Stocks/GetHistoricalQuotes/' + this.id,
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (data) {
                console.log("success");
                console.log(data);
                self.dates.removeAll();
                self.closingQuotes.removeAll();
                var ds = [];
                var cQs = [];
                data.forEach(function (quote) {
                    self.dates.push(new Date(quote.date).toString("dd/MM"));
                    self.closingQuotes.push(quote.adjClose);
                });
                self.stockName(data[0].symbol)
            },
            error: function (xhr, ajaxOptions, thrownError) {
                self.error(thrownError);
                self.showError(true);
            }
        });
    }

    //chart
    self.stockName = ko.observable("");
    self.dates = ko.observableArray();
    self.closingQuotes = ko.observableArray();

    self.SimpleLineData = {
        labels: self.dates,
        datasets: [
            {
                label: self.stockName,
                lineTension: 0,
                pointStyle: "rectRot",
                pointHitRadius: 20,
                backgroundColor: "rgba(220,220,220,0.2)",
                borderColor: "rgba(220,220,220,1)",
                pointColor: "rgba(220,220,220,1)",
                pointStrokeColor: "#fff",
                pointHighlightFill: "#fff",
                pointHighlightStroke: "rgba(220,220,220,1)",
                data: self.closingQuotes
            }
        ]
    };

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
                                id: stock.id,
                                name: stock.name,
                                symbol: stock.symbol,
                                change: stock.stockPrices[0].change,
                                bid: stock.stockPrices[0].bid,
                                ask: stock.stockPrices[0].ask,
                                daysLow: stock.stockPrices[0].daysLow,
                                daysHigh: stock.stockPrices[0].daysHigh,
                                open: stock.stockPrices[0].open,
                                close: stock.stockPrices[0].close,
                                last: stock.stockPrices[0].last,
                                lastUpdate: new Date(stock.stockPrices[0].created).toString().slice(16, 25)
                            }
                            );
                    });
                    self.stocks().forEach(function (stock) {
                        console.log(stock);
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    self.error(thrownError);
                    self.showError(true);
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
