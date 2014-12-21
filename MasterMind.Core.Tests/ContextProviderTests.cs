using FizzWare.NBuilder;
using FluentAssertions;
using MasterMind.Core.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace MasterMind.Core.Tests
{
    [TestClass]
    public class ContextProviderTests
    {
        [TestMethod]
        public void Add_ShouldAddContextsToCurrentUser()
        {
            //Arrange
            var currentUser = new CurrentUser { Contexts = new List<GameContext>() };
            var contextProvider = new ContextProvider(currentUser: currentUser);
            var context = Builder<GameContext>.CreateNew().Build();

            //Act
            contextProvider.Add(context);

            //Assert
            currentUser.Contexts.ShouldBeEquivalentTo(new List<GameContext> { context });
        }

        [TestMethod]
        public void Add_ShouldAddContextsToTheProviderEvenWhenContextsIsNull()
        {
            //Arrange
            var currentUser = new CurrentUser { Contexts = null };
            var contextProvider = new ContextProvider(currentUser: currentUser);
            var context = Builder<GameContext>.CreateNew().Build();

            //Act
            contextProvider.Add(context);

            //Assert
            currentUser.Contexts.ShouldBeEquivalentTo(new List<GameContext> { context });
        }

        [TestMethod]
        public void Add_ShouldNotAddMoreThan5Contexts()
        {
            //Arrange
            var currentUser = new CurrentUser();
            var contextProvider = new ContextProvider(currentUser: currentUser);
            var contexts = Builder<GameContext>.CreateListOfSize(6).Build().ToList();

            //Act
            contexts.ForEach(c => contextProvider.Add(c));

            //Assert
            currentUser.Contexts.ShouldBeEquivalentTo(contexts.Take(5));
        }

        //TODO: Finish this test when the score is moved into the domain.
        //[TestMethod]
        //public void Add_ShouldKeepTheBestScoresInDescendingOrder()
        //{
        //    //Arrange
        //    var currentUser = new CurrentUser();
        //    var contextProvider = new ContextProvider(currentUser: currentUser);
        //    var contexts = Builder<Context>.CreateListOfSize(10).Build().ToList();
        //    for (var i = 0; i < contexts.Count; i++)
        //    {
        //        //contexts[i].Score = i;
        //    }

        //    //Act
        //    contexts.ForEach(c => contextProvider.Add(c));

        //    //Assert
        //    currentUser.Contexts.ShouldBeEquivalentTo(contexts.Take(5));
        //}
    }
}
