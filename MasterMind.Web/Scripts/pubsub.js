Pubsub = function ()
{
    this.pubSubCallBacks = [];
    this.askTellCallBacks = [];

    this.askFor = function (topic)
    {
        for (var key in this.askTellCallBacks)
        {
            if (topic == this.askTellCallBacks[key].topic)
                return this.askTellCallBacks[key]
                    .callback
                    .apply(window, Array.prototype.splice.call(arguments, 1, arguments.length - 1));
        }

        throw "No available answer for '" + topic + "'.";
    };

    this.answerFor = function (topic, provider)
    {
        for (var key in this.askTellCallBacks)
        {
            if (this.askTellCallBacks[key].topic == topic)
            {
                this.askTellCallBacks[key].callback = provider;
                return;
            }
        }

        this.askTellCallBacks.push({ topic: topic, callback: provider });
    };

    this.subscribe = function (topic, callback)
    {
        this.pubSubCallBacks.push({ topic: topic, callback: callback });
    };

    this.publish = function (topic, arg)
    {
        for (var key in this.pubSubCallBacks)
        {
            if (topic == this.pubSubCallBacks[key].topic)
                this.pubSubCallBacks[key].callback(arg);
        }
    };
}