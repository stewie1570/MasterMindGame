﻿var constants = {
    guessColors: ["empty", "red", "blue", "green", "yellow", "purple"],
    resultColors: ["red", "white", "empty"]
};

var GameViewModel = function (serverVm, pubsub)
{
    var self = this;
    this.initialServerVM = serverVm;
    this.serverVm = ko.observable(serverVm);
    this.currentGuess = ko.observableArray([]);
    this.guessWidth = ko.observable(null);
    this.isSetup = ko.observable(false);
    this.isCommunicating = ko.observable(false);
    this.level = ko.observable(0);
    this.resultLogic = ko.observable("perpeg");

    this.pegAction = function (peg)
    {
        if (constants.guessColors.indexOf(peg) >= 0)
        {
            if (self.currentGuess().length < self.guessWidth())
                self.currentGuess.push(peg);
        }
        else
            self.currentGuess.pop();
    };

    this.reset = function ()
    {
        self.serverVm(self.initialServerVM);
        self.currentGuess([]);
        self.isSetup(false);
    }

    this.sendGuess = function ()
    {
        self.isCommunicating(true);
        $.post("Home/Guess", { guess: self.helpers.pegArrayToGuessString(self.currentGuess()) })
            .success(self.binders.bindServerResults)
            .fail(self.binders.communicationFail);
        pubsub.publish("thinkquick:sendguess", { width: self.guessWidth() });
    }

    this.setupGame = function (width)
    {
        self.isCommunicating(true);
        self.guessWidth(width);
        self.level(width - 3);
        $.post("Home/Setup", { width: self.guessWidth(), resultLogic: self.resultLogic() })
            .success(self.binders.setupSuccess)
            .fail(self.binders.communicationFail);
        pubsub.publish("thinkquick:setup", { width: width });
    }

    this.shareScore = function ()
    {
        var vm = self.serverVm();
        vm.level = self.level();
        vm.width = self.guessWidth();
        pubsub.publish("thinkquick:sharescore", vm);
    }

    this.binders = {
        setupSuccess: function (data)
        {
            self.isCommunicating(false);
            if (data["Message"] != undefined)
                alert(data.Message);
            else
            {
                self.binders.bindServerResults(data);
                self.isSetup(true);
            }
        },

        communicationFail: function () { self.isCommunicating(false); },

        bindServerResults: function (data)
        {
            data = JSONCasing.toCamel(data);
            self.isCommunicating(false);
            self.helpers.addTimeLapsePercentagesTo(data);
            data.results = (data.results || []).padRight(data.maxAttempts, {
                result: [].padRight(self.guessWidth(), constants.resultColors.indexOf("empty")),
                guess: [].padRight(self.guessWidth(), constants.guessColors.indexOf("empty")),
                timeLapsePercent: 0
            });
            self.serverVm(data);
            self.currentGuess([]);

            if (data.isOver)
                pubsub.publish(data.isAWin ? "thinkquick:win" : "thinkquick:lost", { width: self.guessWidth() });
        }
    };

    this.helpers = {
        addTimeLapsePercentagesTo: function (data)
        {
            if (data.results)
            {
                var maxTimeLapse = data.results.select(function (result)
                {
                    return result.timeLapse.seconds;
                }).max();
                data.results = data.results.select(function (result)
                {
                    result.timeLapsePercent = maxTimeLapse == 0 ? 0 : (result.timeLapse.seconds / maxTimeLapse) * 100;
                    return result;
                });
            }
        },

        pegArrayToGuessString: function (pegArray)
        {
            return pegArray
                .select(function (guess) { return guess[0]; })
                .toString()
                .replaceAll(',', '');
        }
    };
};

String.prototype.replaceAll = function (find, replacement)
{
    return this.split(find).join(replacement);
}

Array.prototype.max = function ()
{
    var ret = this[0];
    this.forEach(function (i) { ret = ret > i ? ret : i; });
    return ret;
}

Array.prototype.select = function (del)
{
    var ret = [];
    this.forEach(function (i) { ret.push(del(i)); });
    return ret;
}

Array.prototype.padRight = function (length, filler)
{
    var currentLength = this == null ? 0 : this.length;
    var newArray = [];
    for (var i = 0; i < length; i++)
    {
        newArray[i] = i < currentLength ? this[i] : filler;
    }
    return newArray;
}