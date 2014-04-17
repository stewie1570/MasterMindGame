$(document).ready(function ()
{
    var pubsub = new Pubsub();

    var initial = { "Results": [], "IsOver": false, "IsAWin": false };
    ko.applyBindings(new GameViewModel(initial, pubsub));
    new Facebook(pubsub);
});