﻿using CLIExecute;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xbehave;

namespace CLITests
{
    public class TestExecutorWithFakes
    {
        [Scenario]
        [Example("Test_Get_Add_Http", 1)]
        [Example("test_get_add_http", 1)]
        [Example("GetMathId_Http", 1)]
        [Example("MathPOST", 1)]
        [Example("MathPut", 1)]
        [Example("MathDelete", 1)]
        [Example("WeatherGet", 1)]
        [Example("WeatherGet,MathDelete", 2)]
        [Example("WeatherGet,NotExistThisCommand", 1)]
        [Example("NotExistThisCommand", 0)]
        public void TestRetrieveCommand(string command, int numberCommandsFound)
        {
            Executor e = null;
            $"creating executor with fake ".x(() =>
            {
                var m = new Mock<IConfiguration>();
                m.Setup(it => it["CLI_Commands"])
                 .Returns(command);

                e = new Executor(m.Object, null, null, null);
            });
            if(numberCommandsFound>0)
            $"then asking for commands should retrieve only {command}".x(() =>
            {
                var execs = e.CommandsToExecute();
                execs.Should().NotBeNull();
                execs.Should().HaveCount(numberCommandsFound);
                if( command.IndexOf(",")<0)
                    execs[0].NameCommand.Should().BeEquivalentTo(command);
            });
            if (numberCommandsFound == 0)
            {
                $"then asking for a command that not exists with name: {command}".x(() =>
                {
                    e.Invoking(e => e.CommandsToExecute())
                        .Should().Throw<ArgumentException>()
                        ;
                });
            }
            $"then asking for ShouldExecuteCommands should return {command?.Length > 0}".x(() =>
            {
                var shouldExecute = e.ShouldExecuteCommands();
                shouldExecute.Should().Be((command?.Length > 0));
            });
        }
        [Scenario]
        [Example("1", true)]
        [Example("0", false)]
        [Example("", false)]
        [Example(null, false)]
        public void TestShowHelp(string commandValue, bool showHelp)
        {
            Executor e = null;
            $"creating executor with fake ".x(() =>
            {
                var m = new Mock<IConfiguration>();
                m.Setup(it => it["CLI_HELP"])
                 .Returns(commandValue);

                e = new Executor(m.Object, null, null, null);
            });
           $"then asking for ShowHelp should return {showHelp}".x(() =>
                {
                    var execs = e.ShouldShowHelp();
                    execs.Should().Be(showHelp);
                });
        }
    }
}
