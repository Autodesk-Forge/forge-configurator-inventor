using System;
using System.IO;
using Xunit.Abstractions;

namespace WebApplication.Tests.Integration
{
    internal static class XUnitUtils
    {
        public static void RedirectConsoleToXUnitOutput(ITestOutputHelper output)
        {
            Console.SetOut(new XUnitLogWriter(output));
        }
        private class XUnitLogWriter : StringWriter
        {
            private readonly ITestOutputHelper _output;

            public XUnitLogWriter(ITestOutputHelper output)
            {
                _output = output;
            }

            public override void Write(string? value)
            {
                _output.WriteLine(value?.TrimEnd());
            }
        }
    }
}
