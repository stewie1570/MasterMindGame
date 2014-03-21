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
    this.maxAttempts = ko.observable(null);
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
            .done(function (data)
            {
                self.serverVm(data); self.currentGuess([]);
            });
    }

    this.setupGame = function ()
    {
        $.post("Home/Setup", { width: self.guessWidth(), maxAttempts: self.maxAttempts() })
            .success(function (data)
            {
                if (data["Message"] != undefined)
                    alert(data.Message);
                else
                    self.isSetup(true);
            });
    }

    this.helpers = {
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