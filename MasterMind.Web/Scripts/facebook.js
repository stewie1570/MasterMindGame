Facebook = function (pubsub)
{
    pubsub.subscribe("thinkquick:sharescore", function (data)
    {
        FB.ui(
        {
            method: 'feed',
            name: 'ThinkQuick',
            caption: 'Puzzle Solved',
            description: (
                "Game Solved in " + data.TotalTimeLapse.TotalSeconds.toFixed(1) + ' seconds.\n' +
                data.ColorCount + " distinct colors.\n" +
                data.Score + " POINTS"
            ),
            link: document.location.href
        }, function (response) { });
    });
}

window.fbAsyncInit = function ()
{
    FB.init({
        appId: '595213527213642',
        status: true,
        xfbml: true
    });
};

(function (d, s, id)
{
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement(s); js.id = id;
    js.src = "//connect.facebook.net/en_US/all.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));