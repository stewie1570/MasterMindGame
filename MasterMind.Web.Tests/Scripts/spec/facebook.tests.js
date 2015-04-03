describe("Facebook integration", function ()
{
    describe("asking for current user", function ()
    {
        it("should return the current user when the system asks for it", function (done)
        {
            //Arrange
            window.FB = {
                api: function (route, callback)
                {
                    if (route == "/me")
                        callback({ name: "stewie" });
                }
            };
            var pubsub = new Pubsub();
            var facebook = new Facebook(pubsub);

            //Act
            //Assert
            pubsub
                .askFor("thinkquick:userfullname")
                .then(function (name) { expect(name).toEqual("stewie"); })
                .then(done);
        });
    });
});