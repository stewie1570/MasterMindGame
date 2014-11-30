window.fbAsyncInit = function ()
{
    FB.init({
        appId: '595213527213642',
        status: true,
        xfbml: true
    });
    window.pubsub.publish("thinkquick:facebook:init");
};

(function (d, s, id)
{
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) { return; }
    js = d.createElement(s); js.id = id;
    js.src = "//connect.facebook.net/en_US/all.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));