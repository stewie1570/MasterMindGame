var MasterMindCore = function (pubsub)
{
    var self = this;
    self.pubsub = pubsub;
    self.constants = {
        guessColors: ["empty", "red", "blue", "green", "yellow", "purple"],
        resultColors: ["red", "white", "empty"]
    };

    self.pubsub.subscribe("mastermind:core:setup", function (setupData)
    {
        self.pubsub.publish("mastermind:comm:setup", setupData);
    });

    self.pubsub.subscribe("mastermind:core:start", function ()
    {
        self.pubsub.publish("mastermind:ui:show", { Results: [], IsOver: false, IsAWin: false });
    });

    self.pubsub.subscribe("mastermind:core:guess", function (guessNumberArray)
    {
        var guessCSV = self
                    .helpers
                    .arraySelect(guessNumberArray, function (guess)
                    {
                        return self.constants.guessColors[guess].charAt(0);
                    }).toString();
        var guess = self.helpers.replaceAll(guessCSV, ',', '');

        self.pubsub.publish("mastermind:comm:guess", guess);
    });

    self.pubsub.subscribe("mastermind:core:show:results", function (vm)
    {
        self.pubsub.publish("mastermind:ui:bind", vm);
    });

    self.helpers = {
        arraySelect: function (array, del)
        {
            var results = [];
            for (var i = 0; i < array.length; i++)
                results.push(del(array[i]));
            return results;
        },
        
        replaceAll: function (str, find, replacement)
        {
            return str.split(find).join(replacement);
        }
    };
}