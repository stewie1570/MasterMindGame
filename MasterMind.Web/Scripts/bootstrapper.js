$(document).ready(function ()
{
    window.pubsub = new Pubsub();

    var initial = { "Results": [], "IsOver": false, "IsAWin": false };
    ko.applyBindings(new GameViewModel(initial, window.pubsub));
    new Facebook(window.pubsub);
    new EventTracking(window.pubsub);
    window.pubsub.subscribe("thinkquick:facebook:init", function () {
        window.pubsub.askFor("thinkquick:userfullname").then(function (name) {
            if (name)
                pubsub.publish("thinkquick:authenticate", name);
        });
    });
});