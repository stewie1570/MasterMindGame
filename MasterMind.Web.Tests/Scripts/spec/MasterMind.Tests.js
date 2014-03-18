///<reference path="../../../MasterMind.Web/Scripts/pubsub.js" />
///<reference path="../../../MasterMind.Web/Scripts/MasterMind.js" />
///<reference path="/lib/jasmine-2.0.0/jasmine.js" />

var customJasmineMatchers = {
    toBeJSONStringEqual: function (util, customEqualityTesters)
    {
        return {
            compare: function (actual, expected)
            {
                var actualString = JSON.stringify(actual);
                var expectedString = JSON.stringify(expected);

                return {
                    pass: actualString == expectedString,
                    message: actual + " does not equal expected JSON: " + expectedString
                }
            }
        };
    }
};

describe("MasterMind", function ()
{
    describe("Core", function ()
    {
        var pubsub = null;
        var masterMindGame = null;

        beforeEach(function ()
        {
            jasmine.addMatchers(customJasmineMatchers);
            pubsub = new Pubsub();
            masterMindGame = new MasterMindCore(pubsub);
        });

        describe("Setup New Game", function ()
        {
            it("should tell the server to setup the game with a width and max attempts.", function ()
            {
                //Arrange
                var result = null;
                pubsub.subscribe("mastermind:comm:setup", function (setupData) { result = setupData; });

                //Act
                pubsub.publish("mastermind:core:setup", { width: 4, attempts: 10 });

                //Assert
                expect(result).toBeJSONStringEqual({ width: 4, attempts: 10 });
            });
        });

        describe("Start and Initialize Game", function ()
        {
            it("should tell the UI to show the game view with a clean view model.", function ()
            {
                //Arrange
                var vm = null;
                pubsub.subscribe("mastermind:ui:bind", function (resultVM) { vm = resultVM; });

                //Act
                pubsub.publish("mastermind:core:start");

                //Assert
                expect(vm).toBeJSONStringEqual({ Results: [], IsOver: false, IsAWin: false, IsSetup: true });
            });
        });

        describe("Send Guess", function ()
        {
            it("should send the guess in single char per peg string format to the server.", function ()
            {
                //Arrange
                var guessRequestString = null;
                pubsub.subscribe("mastermind:comm:guess", function (guess) { guessRequestString = guess; });

                //Act
                pubsub.publish("mastermind:core:guess", [1, 2, 3, 4, 5]);

                //Assert
                expect(guessRequestString).toBe("rbgyp");
            });
        });

        describe("Receive Guess", function ()
        {
            it("should receive the guess from the server and send it to the UI.", function ()
            {
                //Arrange
                var vm = null;
                pubsub.subscribe("mastermind:ui:bind", function (results) { vm = results; });

                //Act
                pubsub.publish("mastermind:core:results", { test: true });

                //Assert
                expect(vm).toBeJSONStringEqual({ test: true });
            });
        });

        describe("Start New Game", function ()
        {
            it("should show a view to setup a new game and clear the VM.", function ()
            {
                //Arrange
                var vm = null;
                pubsub.subscribe("mastermind:ui:bind", function (results) { vm = results; });

                //Act
                pubsub.publish("mastermind:core:newgame");

                //Assert
                expect(vm).toBeJSONStringEqual({ Results: [], IsOver: false, IsAWin: false, IsSetup: false });
            });
        });
    });
});
