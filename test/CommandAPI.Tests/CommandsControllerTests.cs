using System.Linq;
using System;
using Xunit;
using CommandAPI.Controllers;
using CommandAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests : IDisposable
    {
        private DbContextOptionsBuilder<CommandDBContext> optionsBuilder;
        private CommandDBContext dbContext;
        private CommandsController controller;

        public CommandsControllerTests()
        {
            optionsBuilder = new DbContextOptionsBuilder<CommandDBContext>();
            optionsBuilder.UseInMemoryDatabase("UnitTestInMemDB");
            dbContext = new CommandDBContext(optionsBuilder.Options);
            //Controller
            controller = new CommandsController(dbContext);
        }

        public void Dispose()
        {
            optionsBuilder = null;
            foreach (var cmd in dbContext.CommandItems)
            {
                dbContext.CommandItems.Remove(cmd);
            }

            dbContext.SaveChanges();
            dbContext.Dispose();
            controller = null;
        }

        //ACTION 1 Tests: GET /api/commands

        //TEST 1.1 REQUEST OBJECTS WHEN NONE EXIST – RETURN "NOTHING"
        [Fact]
        public void GetCommandItems_ReturnsZeroItems_WhenDBIsEmpty()
        {
            //Act
            var result = controller.GetCommandItems();
            //Assert
            Assert.Empty(result.Value);
        }

        // TEST 1.2: R ETURNING A COUNT OF 1 FOR A SINGLE C OMMAND O BJECT
        [Fact]
        public void GetCommandItemsReturnsOneItemWhenDBHasOneObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.Add(command);
            dbContext.SaveChanges();

            //Act
            var result = controller.GetCommandItems();
            // Assert
            Assert.Single(result.Value);
        }

        // TEST 1.3: RETURNING A COUNT OF N FOR N COMMAND OBJECTS
        [Fact]
        public void GetCommandItemsReturnNItemsWhenDBHasObjects()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            var command2 = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };


            dbContext.Add(command);
            dbContext.Add(command2);
            dbContext.SaveChanges();

            //Act
            var result = controller.GetCommandItems();
            // Assert
            Assert.Equal(2, result.Value.Count());
        }


        // TEST 1.4: RETURNS THE EXPECTED TYPE
        [Fact]
        public void GetCommandItemsReturnsTheCorrectType()
        {
            //Arrange
            //Act
            var result = controller.GetCommandItems();
            //Assert
            Assert.IsType<ActionResult<IEnumerable<Command>>>(result);
        }


        //TEST 2.1 Invalid resource ID - Null object value result
        [Fact]
        public void GetCommandItemReturnsNullResultWhenInvalidID()
        {
            //Arrange
            //DB should be empty, any ID will be invalid

            //Act
            var result = controller.GetCommandItem(0);
            //Assert
            Assert.Null(result.Value);
        }

        //TEST 2.2 Invalid resource ID - 404 Not Found return code
        [Fact]
        public void GetCommandItemReturns404NotFoundWhenInvalidID()
        {
            //Arrange
            //DB should be empty, any ID will be invalid

            //Act
            var result = controller.GetCommandItem(0);
            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        //TEST 2.3 Valid resource ID - Check correct return type
        [Fact]
        public void GetCommandItemReturnsCorrectType()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            var cmdId = command.Id;

            //Act
            var result = controller.GetCommandItem(cmdId);
            //Assert
            Assert.IsType<ActionResult<Command>>(result);
        }

        //TEST 2.4 Valid resource ID - Collect resource returned
        [Fact]
        public void GetCommandItemReturnsTheCorrectResource()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            var cmdId = command.Id;

            //Act
            var result = controller.GetCommandItem(cmdId);
            //Assert
            Assert.Equal(cmdId, result.Value.Id);
        }


        //TEST 3.1 Valid object submitted - Object count increments by 1
        [Fact]
        public void PostCommandItemObjectCountIncrementWhenValidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            var oldCount = dbContext.CommandItems.Count();
            //Act
            var result = controller.PostCommandItem(command);
            //Assert
            Assert.Equal(oldCount + 1, dbContext.CommandItems.Count());
        }

        //TEST 3.2 Valid object submitted - 201 created return code
        [Fact]
        public void PostCommandItemReturns201CreatedWhenValidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            //Act
            var result = controller.PostCommandItem(command);
            //Assert
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        //TEST 4.1 Valid object submitted - Attribute is updated
        [Fact]
        public void PutCommandItem_AttributeUpdated_WhenValidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id;

            command.HowTo = "UPDATED";
            //Act
            controller.PutCommandItem(cmdId, command);
            var result = dbContext.CommandItems.Find(cmdId);
            //Assert
            Assert.Equal(command.HowTo, result.HowTo);
        }

        //TEST 4.2 Valid object submitted - 204 return code
        [Fact]
        public void PutCommandItem_Returns204_WhenValidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id;

            command.HowTo = "UPDATED";
            //Act
            var result = controller.PutCommandItem(cmdId, command);
            //Assert
            Assert.IsType<NoContentResult>(result);
        }

        //TEST 4.3 Invalid object submitted - 400 return code

        [Fact]
        public void PutCommandItem_Returns400_WhenInvalidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var cmdId = command.Id + 1;

            command.HowTo = "UPDATED";
            //Act
            var result = controller.PutCommandItem(cmdId, command);
            //Assert
            Assert.IsType<BadRequestResult>(result);
        }

        //TEST 4.4 Invalid object submitted - Object remains unchanged
        [Fact]
        public void PutCommandItem_AttributeUnchanged_WhenInvalidObject()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            var command2 = new Command
            {
                Id = command.Id,
                HowTo = "UPDATED",
                Platform = "UPDATED",
                CommandLine = "UPDATED"
            };
            //Act
            controller.PutCommandItem(command.Id + 1, command2);
            var result = dbContext.CommandItems.Find(command.Id);
            //Assert
            Assert.Equal(command.HowTo, result.HowTo);
        }

        //TEST 5.1 Valid Object ID submitted - Object count decrements by 1
        [Fact]
        public void DeleteCommandItem_ObjectsDecrement_WhenValidObjectID()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            var objCount = dbContext.CommandItems.Count();
            //Act
            controller.DeleteCommandItem(command.Id);
            //Assert
            Assert.Equal(objCount - 1, dbContext.CommandItems.Count());
        }

        //TEST 5.2 Valid Object ID submitted - 200 OK return code
        [Fact]
        public void DeleteCommandItem_Returns200_OK_WhenValidObjectID()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();
            var objCount = dbContext.CommandItems.Count();
            //Act
            var result = controller.DeleteCommandItem(command.Id);
            //Assert
            Assert.Null(result.Result);
        }

        //TEST 5.3 Invalid Object ID submitted - 404 Not Found return code
        [Fact]
        public void DeleteCommandItem_Returns404NotFound_WhenInvalidObjectID()
        {
            //Arrange
            //Act
            var result = controller.DeleteCommandItem(-1);
            //Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        //TEST 5.4 Valid Object ID submitted - Object count remains unchanged
        [Fact]
        public void DeleteCommandItem_ObjectCountNotDecremented_WhenInvalidObjectID()
        {
            //Arrange
            var command = new Command
            {
                HowTo = "Do Something",
                Platform = "Some Platform",
                CommandLine = "Some Command"
            };
            dbContext.CommandItems.Add(command);
            dbContext.SaveChanges();

            var objCount = dbContext.CommandItems.Count();
            //Act
            var result = controller.DeleteCommandItem(command.Id + 1);
            //Assert
            Assert.Equal(objCount, dbContext.CommandItems.Count());
        }
    }
}