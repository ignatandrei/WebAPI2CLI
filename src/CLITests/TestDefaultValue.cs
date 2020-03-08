using CLIExecute;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xbehave;

namespace CLITests
{
    public class TestDefaultValue
    {
        [Scenario]
        [Example(typeof(int), 0)]
        [Example(typeof(string), default(string))]
        public void TestGetDefaultValue(Type type, object value)
        {
            object result = null;
            $"when testing {nameof(Executor.GetDefaultValue)} with {type.Name}".x(() =>
            {
                result = Executor.GetDefaultValue(type);
            });
            $"then the resulted value {result} should be {value}".x(() =>
            {
                result.Should().BeEquivalentTo(value);
            });

        }
    }
}
