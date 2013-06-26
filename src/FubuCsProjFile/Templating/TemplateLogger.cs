using System;
using System.Diagnostics;
using FubuCore;
using FubuCore.CommandLine;

namespace FubuCsProjFile.Templating
{
    public class TemplateLogger : ITemplateLogger
    {
        private int _indention = 0;
        private int _stepCount;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private int _numberOfSteps;
        private int _numberOfAlterations;
        private int _alterationNumber;

        public void Starting(int numberOfSteps)
        {
            _numberOfSteps = numberOfSteps;
            _stopwatch.Start();
        }

        public void TraceStep(ITemplateStep step)
        {
            _stepCount++;
            var text = _stepCount.ToString().PadLeft(3) + "/" + _numberOfSteps + ": " + step.ToString();

            ConsoleWriter.WriteWithIndent(ConsoleColor.White, 0, text);

            _indention = 8;
        }

        public void Trace(string contents, params object[] parameters)
        {
            ConsoleWriter.WriteWithIndent(ConsoleColor.White, _indention, contents.ToFormat(parameters));
        }

        public void StartProject(int numberOfAlterations)
        {
            _alterationNumber = 0;
            _numberOfAlterations = numberOfAlterations;
            _indention = 12;
        }

        public void EndProject()
        {
            _indention = 8;
        }

        public void TraceAlteration(IProjectAlteration alteration)
        {
            _alterationNumber++;
            var text = _alterationNumber.ToString().PadLeft(3) + "/" + _numberOfAlterations + ": " + alteration.ToString();
        
            ConsoleWriter.WriteWithIndent(ConsoleColor.Gray, _indention, text);
        }

        public void Finish()
        {
            _stopwatch.Stop();

            ConsoleWriter.Write(ConsoleColor.Green, "Templating successful in {0} ms".ToFormat(_stopwatch.ElapsedMilliseconds));
        }

        public void WriteSuccess(string message)
        {
            ConsoleWriter.WriteWithIndent(ConsoleColor.Green, _indention, message);
        }

        public void WriteWarning(string message)
        {
            ConsoleWriter.WriteWithIndent(ConsoleColor.Yellow, _indention, message);
        }
    }
}