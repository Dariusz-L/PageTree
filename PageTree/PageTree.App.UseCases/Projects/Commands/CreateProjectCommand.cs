using Common.Basic.Blocks;
using Common.Basic.Repository;
using Mediator;
using PageTree.App.UseCases.Common;
using PageTree.Domain;
using PageTree.Domain.Practice;
using PageTree.Domain.Projects;
using Practicer.Domain.Pages.Common;

namespace PageTree.App.Projects.Commands;

public class CreateProjectCommandHandler : BaseCommandHandler, ICommandHandler<CreateProjectCommand, Result>
{
    private readonly IRepository<Project> _projectRepository;
    private readonly IRepository<ProjectUserList> _projectUserListRepository;
    private readonly IRepository<Page> _pageRepository;
    private readonly IRepository<PracticeCategory> _practiceCategoryRepository;
    private readonly IRepository<PracticeTactic> _practiceTacticRepository;

    public CreateProjectCommandHandler(
         IRepository<Project> projectRepository,
         IRepository<ProjectUserList> projectUserListRepository,
         IRepository<Page> pageRepository,
         IRepository<PracticeCategory> practiceCategoryRepository,
         IRepository<PracticeTactic> practiceTacticRepository)
    {
        _projectRepository = projectRepository;
        _projectUserListRepository = projectUserListRepository;
        _pageRepository = pageRepository;
        _practiceCategoryRepository = practiceCategoryRepository;
        _practiceTacticRepository = practiceTacticRepository;
    }

    public async ValueTask<Result> Handle(CreateProjectCommand command, CancellationToken ct)
    {
        var result = Result.Success();

        var projectList = await _projectUserListRepository.Get(command.ProjectUserListID, result);
        if (!result.IsSuccess || projectList == null)
            return result.Fail();

        var rootPage = new Page(NewID, "Root Page", projectList.OwnerID);
        var rootPracticeCategory = new PracticeCategory(NewID);
        var rootPracticeTactic = new PracticeTactic(NewID);

        var project = new Project(NewID, rootPage.ID, projectList.OwnerID, rootPracticeCategory.ID, rootPracticeTactic.ID);
        if (!projectList.ProjectsCreatedIDs.Create(project.ID))
            return result.Fail();

        rootPage.ProjectID = project.ID;

        await _pageRepository.Save(rootPage, result);
        await _projectRepository.Save(project, result);
        await _projectUserListRepository.Save(projectList, result);
        await _practiceCategoryRepository.Save(rootPracticeCategory, result);
        await _practiceTacticRepository.Save(rootPracticeTactic, result);

        return result;
    }
}

public sealed record CreateProjectCommand(string ProjectUserListID) : ICommand<Result>;
