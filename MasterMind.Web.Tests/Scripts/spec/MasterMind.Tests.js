﻿describe("MasterMind", function ()
{

    describe("Communication Failure Callback", function ()
    {
        it("should set isCommunicating to false", function ()
        {
            //Arrange
            var vm = new GameViewModel({});
            var isCommunicating = true;
            vm.isCommunicating = function (val) { isCommunicating = val; };

            //Act
            vm.binders.communicationFail();

            //Assert
            expect(isCommunicating).toBeFalsy();
        });
    });

    describe("Binding Server Results", function ()
    {
        //Arrange
        var pubsub = null;
        var vm = null;
        var isCommunicating = true;

        beforeEach(function ()
        {
            pubsub = new Pubsub();
            vm = new GameViewModel({}, pubsub);
            isCommunicating = true;
        });

        it("should add a relative time lapse percentage and color code to each result", function ()
        {
            //Arrange
            var data = null;
            vm.serverVm = function (arg) { data = arg; };

            //Act
            vm.binders.bindServerResults({
                results: [
                    { timeLapse: { totalMilliseconds: 20 } },
                    { timeLapse: { totalMilliseconds: 10 } },
                    { timeLapse: { totalMilliseconds: 30 } },
                    { timeLapse: { totalMilliseconds: 50 } },
                    { timeLapse: { totalMilliseconds: 40 } }
                ],
                maxAttempts: 5
            });

            //Assert
            var expectedPercentages = [40, 20, 60, 100, 80];
            for (var i = 0; i < expectedPercentages.length; i++)
            {
                expect(data.results[i].timeLapsePercent).toEqual(expectedPercentages[i]);
            }
        });

        it("should add color to each result based on time lapse percentage", function () {
            //Arrange
            var data = null;
            vm.serverVm = function (arg) { data = arg; };

            //Act
            vm.binders.bindServerResults({
                results: [
                    { timeLapse: { totalMilliseconds: 40 } },
                    { timeLapse: { totalMilliseconds: 20 } },
                    { timeLapse: { totalMilliseconds: 60 } },
                    { timeLapse: { totalMilliseconds: 100 } },
                    { timeLapse: { totalMilliseconds: 80 } },
                    { timeLapse: { totalMilliseconds: 95.6544248231166 } }
                ],
                maxAttempts: 6
            });

            //Assert
            var expectedColors = [
                "#669900",  //40%
                "#33cc00",  //20%
                "#996600",  //60%
                "#ff0000",  //100%
                "#cc3300",  //80%
                "#f30c00"   //95.6544248231166%
            ];
            for (var i = 0; i < expectedColors.length; i++)
            {
                expect(data.results[i].color).toEqual(expectedColors[i]);
            }
        });

        it("should not divide by zero when max time lapse is zero at beginning of game", function ()
        {
            //Arrange
            var data = null;
            vm.serverVm = function (arg) { data = arg; };

            //Act
            vm.binders.bindServerResults({
                results: [
                    { timeLapse: { totalMilliseconds: 0 } }
                ],
                maxAttempts: 5
            });

            //Assert
            var expectedPercentages = [0];
            for (var i = 0; i < expectedPercentages.length; i++)
            {
                expect(data.results[i].timeLapsePercent).toEqual(expectedPercentages[i]);
            }
        });

        it("should set isCommunicating to false", function ()
        {
            //Arrange
            vm.isCommunicating = function (val) { isCommunicating = val; };

            //Act
            vm.binders.bindServerResults({});

            //Assert
            expect(isCommunicating).toBeFalsy();
        });

        it("should set serverVm", function ()
        {
            //Arrange
            var data = null;
            vm.guessWidth(4);
            vm.serverVm = function (val) { data = val; };

            //Act
            vm.binders.bindServerResults({ maxAttempts: 1 });

            //Assert
            expect(data.results).toEqual([
                { result: [2, 2, 2, 2], guess: [0, 0, 0, 0], timeLapsePercent: 0 }
            ]);
        });

        it("should set current guess to empty array (clear the current guess)", function ()
        {
            //Arrange
            var currentGuess = null;
            vm.currentGuess = function (val) { currentGuess = val; };

            //Act
            vm.binders.bindServerResults({});

            //Assert
            expect(currentGuess).toEqual([]);
        });

        it("should run the json data through the JSONCasing lib to make sure the casing from the server is correct", function ()
        {
            //Arrange
            var receivedBindingData = null;
            vm.serverVm = function (data) { receivedBindingData = data; };

            //Act
            vm.binders.bindServerResults({ Test: 1 });

            //Assert
            expect(receivedBindingData).toEqual({ test: 1, results: [] });
        });
    });

    describe("Setup", function ()
    {
        var vm = null;
        var pubsub = null;
        var setupSuccessCallback = null;
        var setupFailCallback = null;
        var postData = null;

        beforeEach(function ()
        {
            pubsub = new Pubsub();
            vm = new GameViewModel({}, pubsub);
            setupSuccessCallback = null;
            setupFailCallback = null;

            $.post = function (url, data)
            {
                postData = data;
                return {
                    success: function (successCallback)
                    {
                        setupSuccessCallback = successCallback;
                        return {
                            fail: function (failCallback)
                            {
                                setupFailCallback = failCallback;
                            }
                        };
                    }
                };
            };
        });

        it("should post the correct data to the server", function ()
        {
            //Arrange
            vm.resultLogic("perpeg");

            //Act
            vm.setupGame(4);

            //Assert
            expect(postData).toEqual({ width: 4, resultLogic: "perpeg" });
        });

        it("should wire up the success callback", function ()
        {
            //Arrange
            //Act
            vm.setupGame(4);

            //Assert
            expect(setupSuccessCallback).toBe(vm.binders.setupSuccess);
        });

        describe("Success Callback", function ()
        {
            var defaultData = { Message: "something" };

            it("should set isCommunicating to false and alert any failure message", function ()
            {
                //Arrange
                var communicationSetToFalse = false;
                var theMessage = "";
                vm.isCommunicating = function (val)
                {
                    communicationSetToFalse = !val;
                };
                alert = function (msg) { theMessage = msg; };

                //Act
                vm.binders.setupSuccess(defaultData);

                //Assert
                expect(communicationSetToFalse).toBeTruthy();
                expect(theMessage).toEqual("something");
            });

            it("should bind server results and set isSetup to true", function ()
            {
                //Arrange
                var bindData = null;
                var isSetup = false;
                vm.binders.bindServerResults = function (data) { bindData = data; };
                vm.isSetup = function (val) { isSetup = val; };

                //Act
                vm.binders.setupSuccess({});

                //Assert
                expect(isSetup).toBeTruthy();
                expect(bindData).toEqual({});
            });
        });

        it("should wire up the fail callback", function ()
        {
            //Arrange
            //Act
            vm.setupGame(4);

            //Assert
            expect(setupFailCallback).toBe(vm.binders.communicationFail);
        });

        it("should calculate the level number from the width", function ()
        {
            //Arrange
            //Act
            vm.setupGame(4)

            //Assert
            expect(vm.level()).toBe(1);
        });

        it("should publish a setup event to the pubsub with width", function ()
        {
            //Arrange
            var recieved = null;
            pubsub.subscribe("thinkquick:setup", function (data) { recieved = data; });

            //Act
            vm.setupGame(4)

            //Assert
            expect(recieved).toEqual({ width: 4 });
        });
    });

    describe("Send Guess", function ()
    {
        var vm = null;
        var postUrl = "";
        var postData = {};
        var pubsub = null;
        var guessSuccessCallback = null;
        var guessFailCallback = null;

        beforeEach(function ()
        {
            pubsub = new Pubsub();
            postUrl = "";
            postData = {};
            window.$ = {};
            guessSuccessCallback = null;
            guessFailCallback = null;
            window.$.post = function (url, data)
            {
                postUrl = url;
                postData = data;
                return {
                    success: function (successCallback)
                    {
                        guessSuccessCallback = successCallback;
                        return {
                            fail: function (failCallback)
                            {
                                guessFailCallback = failCallback;
                            }
                        }
                    }
                };
            };
            vm = new GameViewModel({}, pubsub);
        });

        it("should wire-up the success callback", function ()
        {
            //Arrange
            //Act
            vm.sendGuess();

            //Assert
            expect(guessSuccessCallback).toBe(vm.binders.bindServerResults);
        });

        it("should wire-up the fail callback", function ()
        {
            //Arrange
            //Act
            vm.sendGuess();

            //Assert
            expect(guessFailCallback).toBe(vm.binders.communicationFail);
        });

        it("should send the guess", function ()
        {
            //Arrange
            vm.currentGuess(["red", "green", "blue", "red"]);

            //Act
            vm.sendGuess();

            //Assert
            expect(postUrl).toBe("Home/Guess");
            expect(postData).toEqual({ guess: "rgbr" });
        });

        it("should publish send guess event with width", function ()
        {
            //Arrange
            var received = null;
            vm.guessWidth = function () { return 4; };
            pubsub.subscribe("thinkquick:sendguess", function (data) { received = data; });

            //Act
            vm.sendGuess();

            //Assert
            expect(received).toEqual({ width: 4 });
        });

        describe("Solved", function ()
        {
            it("should publish a win event with details of the win", function ()
            {
                //Arrange
                var pubsub = new Pubsub();
                var received = null;
                pubsub.subscribe("thinkquick:win", function (data) { received = data; });
                vm = new GameViewModel({}, pubsub);
                vm.guessWidth = function () { return 4; };
                
                //Act
                vm.binders.bindServerResults({ isAWin: true, isOver: true });

                //Arrange
                expect(received).toEqual({ width: 4 });
            });
        });

        describe("Lost", function ()
        {
            it("should not have published a win event", function ()
            {
                //Arrange
                var pubsub = new Pubsub();
                var callbackCalled = false;
                pubsub.subscribe("thinkquick:win", function (data) { callbackCalled = true; });
                vm = new GameViewModel({}, pubsub);

                //Act
                vm.binders.bindServerResults({ isOver: true });

                //Arrange
                expect(callbackCalled).toBeFalsy();
            });

            it("should have published a lost event", function ()
            {
                //Arrange
                var pubsub = new Pubsub();
                var received = null;
                pubsub.subscribe("thinkquick:lost", function (data) { received = data; });
                vm = new GameViewModel({}, pubsub);
                vm.guessWidth = function () { return 4; };

                //Act
                vm.binders.bindServerResults({ isOver: true });

                //Arrange
                expect(received).toEqual({ width: 4 });
            });

            it("should not have published a lost event when the game isn't over yet", function ()
            {
                //Arrange
                var pubsub = new Pubsub();
                var received = false;
                pubsub.subscribe("thinkquick:lost", function (data) { received = data; });
                vm = new GameViewModel({}, pubsub);
                vm.guessWidth = function () { return 4; };

                //Act
                vm.binders.bindServerResults({ isOver: false });

                //Arrange
                expect(received).toBeFalsy();
            });
        });

        describe("Share Score", function ()
        {
            it("should publish a share score event with details of the win", function ()
            {
                //Arrange
                var pubsub = new Pubsub();
                var receivedObject = null;
                pubsub.subscribe("thinkquick:sharescore", function (data) { receivedObject = data; });
                vm = new GameViewModel({}, pubsub);
                var expectedWinContextObject = {
                    isOver: true,
                    isAWin: true,
                    score: 10,
                    colorCount: 4,
                    totalTimeLapse: 23.4,
                    level: 4,
                    width: 5
                };
                vm.level(4);
                vm.guessWidth(5);
                vm.serverVm = function ()
                {
                    return {
                        isOver: true,
                        isAWin: true,
                        score: 10,
                        colorCount: 4,
                        totalTimeLapse: 23.4
                    };
                };

                //Act
                vm.shareScore();

                //Arrange
                expect(receivedObject).toEqual(expectedWinContextObject);
            });
        });
    });

    describe("Peg Click", function ()
    {
        var vm = null;

        beforeEach(function ()
        {
            vm = new GameViewModel({}, { publish: function () { } });
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

        it("should not add the peg that was clicked when game is communicating with server", function ()
        {
            //Arrange
            vm.guessWidth(4);
            vm.currentGuess(["red"]);
            vm.isCommunicating(true);

            //Act
            vm.pegAction("blue");

            //Assert
            expect(vm.currentGuess()).toEqual(["red"]);
        });

        it("should send the guess when the current guess is filled", function ()
        {
            //Arrange
            vm.guessWidth(4);
            vm.currentGuess(["red", "green", "blue"]);
            var sendGuessCalled = false;
            vm.sendGuess = function () { sendGuessCalled = true; }

            //Act
            vm.pegAction("yellow");

            //Assert
            expect(sendGuessCalled).toBeTruthy();
        });

        it("should not send the guess when the current guess is not filled", function ()
        {
            //Arrange
            vm.guessWidth(4);
            vm.currentGuess(["red", "green"]);
            var sendGuessCalled = false;
            vm.sendGuess = function () { sendGuessCalled = true; }

            //Act
            vm.pegAction("yellow");

            //Assert
            expect(sendGuessCalled).toBeFalsy();
        });
    });
});
