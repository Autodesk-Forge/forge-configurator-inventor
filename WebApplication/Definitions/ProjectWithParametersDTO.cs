namespace WebApplication.Definitions
{
    public class ProjectWithParametersDTO
    {
        public ProjectDTO Project { get; }
        public ProjectStateDTO Parameters { get; }

        public ProjectWithParametersDTO(ProjectDTO project, ProjectStateDTO parameters)
        {
            Project = project;
            Parameters = parameters;
        }
    }
}
