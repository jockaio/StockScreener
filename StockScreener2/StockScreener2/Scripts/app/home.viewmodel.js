function HomeViewModel(app, dataModel) {
    var self = this;

    //For error handling
    self.error = ko.observable("");
    self.showError = ko.observable(false);

    //Array of stocks
    self.stocks = ko.observableArray();

    //Check LowSpreadPercentage
    self.lowSpreadPercentageChecked = ko.observable(false);
    self.checkLowSpreadPercentage = function () {
        var tempArray = [];
        if (self.lowSpreadPercentageChecked() == true) {
            self.updateStocks(false);
            self.lowSpreadPercentageChecked(false);
        } else {
            self.stocks().forEach(function (stock) {
                if (stock.lowSpreadPercentage() < 0.1) {
                    stock.highlight(true);
                    tempArray.push(stock);
                    self.stocks.remove(stock);
                }
            });
            //Reverse the order to have them in alphbetical order when unshifting stock-array.
            tempArray.reverse();
            tempArray.forEach(function (stock) {
                self.stocks.unshift(stock);
            });

            self.lowSpreadPercentageChecked(true);
        }
    }

    //For adding a new symbol
    self.stockSymbol = ko.observable("");
    self.addStock = function () {
        $.ajax({
            method: 'post',
            url: '/api/Stocks?stockSymbol=' + self.stockSymbol(),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (data) {
                self.stocks.push(
                                new Stock(data)       
                            );
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr);
                self.error(xhr.responseText);
                self.showError(true);
                setTimeout(function () { self.showError(false) }, 3000);
            }
        });
    }

    self.deleteStock = function () {
        var stock = this;
        $.ajax({
            method: 'delete',
            url: '/api/Stocks/' + this.id(),
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (data) {
                self.stocks.remove(stock);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                self.error(xhr.responseText);
                self.showError(true);
                setTimeout(function () { self.showError(false) }, 3000);
            }
        });
    }

    //show stock chart
    self.showChart = ko.observable(false);
    self.hideChart = function () {
        self.showChart(false);
    }

    self.getChart = function (days) {
        self.showChart(false);
        self.loadingChart(true);

        if (days == undefined) {
            days = "";
        }
        if (this.id != undefined) {
            self.stockId(this.id());
        }

        $.ajax({
            method: 'get',
            url: '/api/Stocks/GetHistoricalQuotes/' + self.stockId() + "/",
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (data) {
                var result = [];
                data.forEach(function (p) {
                    result.push(
                        [
                          new Date(p.date).getTime(),
                          p.close
                        ]
                        );
                    console.log(new Date(p.date).getTime());
                })
                
                $('#highstockChart').highcharts('StockChart', {


                    rangeSelector: {
                        selected: 1
                    },

                    title: {
                        text: data[0].symbol
                    },

                    series: [{
                        name: data[0].symbol,
                        data: result,
                        tooltip: {
                            valueDecimals: 2
                        }
                    }]
                });
                self.loadingChart(false);
                self.showChart(true);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr);
                self.error(xhr.responseText);
                self.showError(true);
                setTimeout(function () { self.showError(false) }, 3000);
                self.loadingChart(false);
            }
        });
    }

    //chart
    self.loadingChart = ko.observable(false);
    self.stockId = ko.observable();

    self.refreshStocks = function () {
        self.updateStocks(true);
    }

    self.updateStocks = function (forceUpdate) {
        //Clean the collection of stocks before updating the data.
        self.stocks.removeAll();
        $.ajax({
            method: 'get',
            url: '/api/Stocks?forceRefresh=' + forceUpdate,
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (data) {
                data.forEach(function (stock) {
                    self.stocks.push(
                            new Stock(stock)
                        );
                });
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
        this.get('#home', function () {
            app.view(app.Views["Loading"]);
            self.updateStocks(false);
            app.view(app.Views["Home"]);
        });
        this.get('/', function () { this.app.runRoute('get', '#home') });
    });

    return self;
}

function Stock(data) {

    //Calculations
    var calculations = data.stockPrices[0].calculations;
    var lowSP = "";
    calculations.forEach(function (calc){
        if (calc.calculationType == 0) {
            lowSP = calc.value;
        }
    });

    var self = this;
    self.id = ko.observable(data.id),
    self.name = ko.observable(data.name),
    self.symbol = ko.observable(data.symbol),
    self.change = ko.observable(data.stockPrices[0].change),
    self.bid = ko.observable(data.stockPrices[0].bid),
    self.ask = ko.observable(data.stockPrices[0].ask),
    self.daysLow = ko.observable(data.stockPrices[0].daysLow),
    self.daysHigh = ko.observable(data.stockPrices[0].daysHigh),
    self.open = ko.observable(data.stockPrices[0].open),
    self.close = ko.observable(data.stockPrices[0].close),
    self.last = ko.observable(data.stockPrices[0].last),
    self.lowSpreadPercentage = ko.observable(lowSP),
    self.lastUpdate = ko.observable(new Date(data.stockPrices[0].created).toString().slice(16, 25)),
    self.highlight = ko.observable(false)
}

app.addViewModel({
    name: "Home",
    bindingMemberName: "home",
    factory: HomeViewModel
});
