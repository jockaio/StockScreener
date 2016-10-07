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
        console.log("check");
        if (self.lowSpreadPercentageChecked() == true) {
            self.updateStocks();
            self.lowSpreadPercentageChecked(false);
        } else {
            self.stocks().forEach(function (stock) {
                console.log(stock.name() + ": " + stock.lowSpreadPercentage());
                if (stock.lowSpreadPercentage() < 0.1) {
                    stock.highlight(true);
                    tempArray.push(stock);
                    self.stocks.remove(stock);
                }
            });
            //Reverse the order to have them in alphbetical order when unshifting stock-array.
            tempArray.reverse();
            tempArray.forEach(function (stock) {
                console.log(stock.name());
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
            url: '/api/Stocks/' + this.id(),
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
    self.chartLength = ko.observable(7);
    self.showChart = ko.observable(false);
    self.hideChart = function () {
        self.showChart(false);
    }
    
    self.getChart = function (days) {
        self.showChart(false);
        self.resetChart();
        self.loadingChart(true);

        if (days == undefined) {
            days = "";
        }
        if (this.id != undefined) {
            self.stockId(this.id());
        }
        
        $.ajax({
            method: 'get',
            url: '/api/Stocks/GetHistoricalQuotes/'+self.stockId()+'/'+days,
            contentType: "application/json; charset=utf-8",
            headers: {
                'Authorization': 'Bearer ' + app.dataModel.getAccessToken()
            },
            success: function (data) {
                var ds = [];
                var cQs = [];
                data.forEach(function (quote) {
                    self.dates.push(new Date(quote.date).toString("dd/MM"));
                    self.closingQuotes.push(quote.adjClose);
                });
                self.stockName(data[0].symbol)
                self.loadingChart(false);
                self.showChart(true);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log(xhr);
                self.error(xhr.responseJSON);
                self.showError(true);
                setTimeout(function () { self.showError(false) }, 3000);
                self.loadingChart(false);
            }
        });
    }

    //chart
    self.stockName = ko.observable("");
    self.stockId = ko.observable("");
    self.dates = ko.observableArray();
    self.closingQuotes = ko.observableArray();
    self.loadingChart = ko.observable(false);

    self.resetChart = function () {
        self.stockName("");
        self.dates.removeAll();
        self.closingQuotes.removeAll();
    };

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
                self.error(xhr.responseJSON);
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
    self.lowSpreadPercentage = ko.observable(data.stockPrices[0].lowSpreadPercentage),
    self.lastUpdate = ko.observable(new Date(data.stockPrices[0].created).toString().slice(16, 25)),
    self.highlight = ko.observable(false)
}

app.addViewModel({
    name: "Home",
    bindingMemberName: "home",
    factory: HomeViewModel
});
