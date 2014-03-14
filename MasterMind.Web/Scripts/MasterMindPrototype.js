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
        var guessCSV = self
                    .helpers
                    .select(self.currentGuess(), function (guess)
                    {
                        return guess[0];
                    }).toString();
        var guess = self.helpers.replaceAll(guessCSV, ',', '');

        $.post("Home/Guess", { guess: guess })
            .done(function (data)
            {
                self.serverVm(data); self.currentGuess([]);
            });
    }

    this.setupGame = function ()
    {
        $.post("Home/Setup", { width: self.guessWidth(), maxAttempts: self.maxAttempts() })
            .fail(function (xhr)
            {
                alert($.parseJSON(xhr.responseText).Message);
            })
            .success(function () { self.isSetup(true); });
    }

    this.helpers = {
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
        }
    };
};


$(document).ready(function ()
{
    var initial = { "Results": [], "IsOver": false, "IsAWin": false };

    ko.applyBindings(new GameViewModel(initial));
});