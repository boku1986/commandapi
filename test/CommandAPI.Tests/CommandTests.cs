using System;
using CommandAPI.Models;
using Xunit;

namespace CommandAPI.Tests
{
    public class CommandTests : IDisposable
    {
        private Command testCommand;

        public CommandTests()
        {
            testCommand = new Command
            {
                HowTo = "Do something",
                Platform = "Some Platform",
                CommandLine = "Some commandline"
            };
        }

        public void Dispose()
        {
            testCommand = null;
        }

        [Fact]
        public void CanChangeHowTo()
        {
            //Given
            //When
            testCommand.HowTo = "Execute Unit Tests";
            //Then
            Assert.Equal("Execute Unit Tests", testCommand.HowTo);
        }
        [Fact]
        public void CanChangePlatform()
        {
            //Given

            //When
            testCommand.Platform = "Execute Unit Tests";
            //Then
            Assert.Equal("Execute Unit Tests", testCommand.Platform);
        }

        [Fact]
        public void CanChangeCommandLine()
        {
            //Given
            //When
            testCommand.CommandLine = "dotnet test right";
            //Then
            Assert.Equal("dotnet test right", testCommand.CommandLine);
        }
    }
}