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
