EventTracking = function (pubsub)
{
    var self = this;

    pubsub.subscribe("thinkquick:setup", function (data) { self.track("Setup", data.width); });
    pubsub.subscribe("thinkquick:sendguess", function (data) { self.track("Guess", data.width); });
    pubsub.subscribe("thinkquick:win", function (data) { self.track("Win", data.width); });
    pubsub.subscribe("thinkquick:lost", function (data) { self.track("Lost", data.width); });
    pubsub.subscribe("thinkquick:sharescore", function (data) { self.track("Share", data.width); });
    pubsub.subscribe("thinkquick:sharescore:published", function (data) { self.track("Facebook Publish", data.width); });
    pubsub.subscribe("thinkquick:authenticate", function (name)
    {
        ga('send', 'event', 'Think Quick Game', "Authenticate", name, 1);
    });

    this.track = function (action, width)
    {
        ga('send', 'event', 'Think Quick Game', action, "Game Width: " + width, 1);
    };
};