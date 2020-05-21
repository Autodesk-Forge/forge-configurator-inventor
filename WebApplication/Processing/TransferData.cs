using Autodesk.Forge.DesignAutomation.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication.Definitions;

namespace WebApplication.Processing
{
    /// <summary>
    /// Transfer input to output
    /// </summary>
    public class TransferData : ForgeAppBase
    {
        public TransferData(Publisher publisher) :
            base(publisher)
        {
            Engine = "Autodesk.AutoCAD+22";
        }

        public override string Id => nameof(TransferData);
        public override string Description => "Transfer input to output";

        public override List<string> ActivityCommandLine =>
            new List<string>
            {
                $"$(appbundles[{ActivityId}].path)\\EmptyExePlugin.bundle\\Contents\\EmptyExePlugin.exe"
            };

        public override Dictionary<string, Parameter> ActivityParams =>
            new Dictionary<string, Parameter>
            {
                {
                    "source",
                    new Parameter { Verb = Verb.Get, Description = "source location", LocalName = "fileForTransfer" }
                },
                {
                    "target",
                    new Parameter { Verb = Verb.Put, Description = "target location", LocalName = "fileForTransfer" }
                }
            };

        public Task<bool> ProcessAsync(string source, string target)
        {
            var workItemArgs = new Dictionary<string, IArgument>();
            workItemArgs.Add("source", new XrefTreeArgument { Url = source });
            workItemArgs.Add("target", new XrefTreeArgument { Verb = Verb.Put, Url = target });

            return RunAsync(workItemArgs);
        }
    }
}
