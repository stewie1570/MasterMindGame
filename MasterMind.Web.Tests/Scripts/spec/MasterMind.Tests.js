///<reference path="../../../MasterMind.Web/Scripts/pubsub.js" />
///<reference path="../../../MasterMind.Web/Scripts/MasterMind.js" />
///<reference path="/lib/jasmine-2.0.0/jasmine.js" />

describe("MasterMind", function ()
{
    describe("Send Guess", function ()
    {
        var vm = null;

        beforeEach(function ()
        {
            vm = new GameViewModel({});
        });

        it("should send the guess", function ()
        {
            //Arrange
            window.$ = {};
            var postUrl = "";
            var postData = {};
            $.post = function (url, data)
            {
                postUrl = url; postData = data;
                return { done: function () { } };
            };
            vm.currentGuess(["red", "green", "blue", "red"]);

            //Act
            vm.sendGuess();

            //Assert
            expect(postUrl).toBe("Home/Guess");
            expect(postData).toEqual({ guess: "rgbr" });
        });

        describe("Solved", function ()
        {
            it("should publish a win event with details of the win", function ()
            {
                //Arrange
                var pubsub = new Pubsub();
                var receivedObject = null;
                pubsub.subscribe("thinkquick:win", function (data) { receivedObject = data; });
                vm = new GameViewModel({}, pubsub);
                var expectedWinContextObject = {
                    IsOver: true,
                    IsAWin: true,
                    Score: 10,
                    ColorCount: 4,
                    TotalTimeLapse: 23.4
                };
                
                //Act
                vm.binders.bindServerResults(expectedWinContextObject);

                //Arrange
                expect(receivedObject).toBe(expectedWinContextObject);
            });
        });

        describe("Lost", function ()
        {
            it("should publish a win event with details of the win", function ()
            {
                //Arrange
                var pubsub = new Pubsub();
                var callbackCalled = false;
                pubsub.subscribe("thinkquick:win", function (data) { callbackCalled = true; });
                vm = new GameViewModel({}, pubsub);
                var expectedWinContextObject = {};

                //Act
                vm.binders.bindServerResults(expectedWinContextObject);

                //Arrange
                expect(callbackCalled).toBeFalsy();
            });
        });
    });

    describe("Helpers", function ()
    {
        var vm = null;

        beforeEach(function ()
        {
            vm = new GameViewModel({});
        });

        it("padRight should expand an array to provided length using provided filler", function ()
        {
            //Arrange
            //Act
            //Assert
            expect(vm.helpers.padRight([1, 2, 3], 5, 0)).toEqual([1, 2, 3, 0, 0]);
        });

        it("padRight should be able to receive null as an empty array", function ()
        {
            //Arrange
            //Act
            //Assert
            expect(vm.helpers.padRight(null, 5, 0)).toEqual([0, 0, 0, 0, 0]);
        });
    });

    describe("Peg Click", function ()
    {
        var vm = null;

        beforeEach(function ()
        {
            vm = new GameViewModel({});
        });

        it("should remove the last peg when backspace 'peg' is clicked", function ()
        {
            //Arrange
            var currentGuessPopMethodWasCalled = false;
            vm.currentGuess.pop = function () { currentGuessPopMethodWasCalled = true; };

            //Act
            vm.pegAction("peg that doesn't exist");

            //Assert
            expect(currentGuessPopMethodWasCalled).toBeTruthy();
        });

        it("should add the peg that was clicked", function ()
        {
            //Arrange
            vm.guessWidth(4);
            vm.currentGuess(["red", "green", "blue"]);

            //Act
            vm.pegAction("red");

            //Assert
            expect(vm.currentGuess()).toEqual(["red", "green", "blue", "red"]);
        });

        it("should not add the peg that was clicked when the guess length is met", function ()
        {
            //Arrange
            vm.guessWidth(3);
            vm.currentGuess(["red", "green", "blue"]);

            //Act
            vm.pegAction("red");

            //Assert
            expect(vm.currentGuess()).toEqual(["red", "green", "blue"]);
        });
    });
});
