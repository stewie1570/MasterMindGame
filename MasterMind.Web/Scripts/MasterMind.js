var constants = {
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
            .fail(function () { self.isCommunicating(false); });
        pubsub.publish("thinkquick:sendguess", { width: self.guessWidth() });
    }

    this.setupGame = function (width)
    {
        self.isCommunicating(true);
        self.guessWidth(width);
        self.level(width - 3);
        $.post("Home/Setup", { width: self.guessWidth() })
            .success(function (data)
            {
                self.isCommunicating(false);
                if (data["Message"] != undefined)
                    alert(data.Message);
                else
                {
                    self.binders.bindServerResults(data);
                    self.isSetup(true);
                }
            })
            .fail(function () { self.isCommunicating(false); });
        pubsub.publish("thinkquick:setup", { width: width });
    }

    this.shareScore = function ()
    {
        var vm = self.serverVm();
        vm.Level = self.level();
        vm.width = self.guessWidth();
        pubsub.publish("thinkquick:sharescore", vm);
    }

    this.binders = {
        bindServerResults: function (data)
        {
            self.isCommunicating(false);
            var filler = {
                Result: self.helpers.padRight([], self.guessWidth(), 2),
                Guess: self.helpers.padRight([], self.guessWidth(), 0)
            };
            data.Results = self.helpers.padRight(data.Results, data.MaxAttempts, filler);
            self.serverVm(data);
            self.currentGuess([]);

            if (data.IsOver)
                pubsub.publish(data.IsAWin ? "thinkquick:win" : "thinkquick:lost", { width: self.guessWidth() });
        }
    };

    this.helpers = {
        padRight: function (array, length, filler)
        {
            var currentLength = array == null ? 0 : array.length;
            var newArray = [];
            for (var i = 0; i < length; i++)
            {
                newArray[i] = i < currentLength ? array[i] : filler;
            }
            return newArray;
        },

        isNullOrEmpty: function (str)
        {
            return str == null || str.length == 0;
        },

        select: function (array, del)
        {
            var ret = [];
            for (var i = 0; i < array.length; i++)
            {
                ret.push(del(array[i]));
            }
            return ret;
        },

        replaceAll: function (str, find, replacement)
        {
            return str.split(find).join(replacement);
        },

        pegArrayToGuessString: function (array)
        {
            var guessCSV = self
                .helpers
                .select(array, function (guess) { return guess[0]; }).toString();
            return self.helpers.replaceAll(guessCSV, ',', '');
        }
    };
};