Facebook = function (pubsub)
{
    var self = this;

    pubsub.answerFor("thinkquick:userfullname", function ()
    {
        var deferred = Q.defer();
        FB.api("/me", function (response)
        {
            deferred.resolve(response.name);
        });
        
        return deferred.promise;
    });

    pubsub.subscribe("thinkquick:sharescore", function (data)
    {
        FB.getLoginStatus(function (response)
        {
            if (response.status === 'connected')
                self.postToFacebook(data);
            else
                FB.login(function (response)
                {
                    if (response.authResponse) self.postToFacebook(data);
                });
        });
    });

    this.postToFacebook = function (data)
    {
        FB.ui(
        {
            method: 'feed',
            name: 'Think Quick',
            caption: 'Puzzle Solved',
            description: (
                "Level " + data.level + " solved in " + data.totalTimeLapse.totalSeconds.toFixed(1) + ' seconds.\n' +
                data.colorCount + " distinct colors.\n" +
                data.score + " POINTS"
            ),
            link: document.location.href
        }, function (response)
        {
            if (response && response.post_id)
                pubsub.publish("thinkquick:sharescore:published", data);
        });
    }
}