Pubsub = function ()
{
    this.callbacks = [];

    this.subscribe = function (topic, callback)
    {
        this.callbacks.push({ topic: topic, callback: callback });
    };

    this.publish = function (topic, arg)
    {
        for (var key in this.callbacks)
        {
            if (topic == this.callbacks[key].topic)
                this.callbacks[key].callback(arg);
        }
    };
}