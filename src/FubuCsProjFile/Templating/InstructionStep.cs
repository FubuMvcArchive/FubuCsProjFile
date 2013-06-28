namespace FubuCsProjFile.Templating
{
    public class InstructionStep : ITemplateStep
    {
        private readonly string _file;

        public InstructionStep(string file)
        {
            _file = file;
        }

        public void Alter(TemplatePlan plan)
        {
            plan.AddInstructions(plan.FileSystem.ReadStringFromFile(_file));
        }
    }
}