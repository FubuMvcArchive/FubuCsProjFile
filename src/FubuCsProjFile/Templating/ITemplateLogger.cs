namespace FubuCsProjFile.Templating
{
    public interface ITemplateLogger
    {
        void Starting(int numberOfSteps);
        void TraceStep(ITemplateStep step);
        void Trace(string contents, params object[] parameters);
        void StartProject(int numberOfAlterations);
        void EndProject();
        void TraceAlteration(IProjectAlteration alteration);
        void Finish();
        void WriteSuccess(string message);
        void WriteWarning(string message);
    }
}