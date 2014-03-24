var constants = {
    guessColors: ["empty", "red", "blue", "green", "yellow", "purple"],
    resultColors: ["red", "white", "empty"]
};

var GameViewModel = function (serverVm)
{
    var self = this;
    this.initialServerVM = serverVm;
    this.serverVm = ko.observable(serverVm);
    this.currentGuess = ko.observableArray([]);
    this.guessWidth = ko.observable(null);
    this.isSetup = ko.observable(false);

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
        $.post("Home/Guess", { guess: self.helpers.pegArrayToGuessString(self.currentGuess()) })
            .done(self.bindHelpers.bindServerResults);
    }

    this.setupGame = function (width)
    {
        self.guessWidth(width);
        $.post("Home/Setup", { width: self.guessWidth() })
            .success(function (data)
            {
                if (data["Message"] != undefined)
                    alert(data.Message);
                else
                {
                    self.bindHelpers.bindServerResults(data);
                    self.isSetup(true);
                }
            });
    }

    this.bindHelpers = {
        bindServerResults: function (data)
        {
            var filler = {
                Result: self.helpers.padRight([], self.guessWidth(), 2),
                Guess: self.helpers.padRight([], self.guessWidth(), 0)
            };
            data.Results = self.helpers.padRight(data.Results, data.MaxAttempts, filler);
            self.serverVm(data);
            self.currentGuess([]);
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


$(document).ready(function ()
{
    var initial = { "Results": [], "IsOver": false, "IsAWin": false };

    ko.applyBindings(new GameViewModel(initial));
});