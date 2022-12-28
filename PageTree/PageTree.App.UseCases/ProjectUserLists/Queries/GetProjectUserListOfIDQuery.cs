﻿using Common.Basic.Blocks;
using Common.Basic.Repository;
using Mediator;
using PageTree.Domain.Projects;

namespace PageTree.App.ProjectUserLists.Queries;

public class GetProjectUserListOfIDQueryHandler : IQueryHandler<GetProjectUserListQuery, Result<GetProjectUserListQueryOut>>
{
    private IRepository<Project> _projectRepository;
    private IRepository<ProjectUserList> _projectUserListRepository;

    public GetProjectUserListOfIDQueryHandler(IRepository<Project> projectRepository, IRepository<ProjectUserList> projectUserListRepository)
    {
        _projectRepository = projectRepository;
        _projectUserListRepository = projectUserListRepository;
    }

    public async ValueTask<Result<GetProjectUserListQueryOut>> Handle(GetProjectUserListQuery query, CancellationToken ct)
    {
        var result = Result<GetProjectUserListQueryOut>.Success();

        var projectUserList = await _projectUserListRepository.Get(query.ID, result);
        var projects = await _projectRepository.Get(projectUserList.ProjectsCreatedIDs, result);

        var @out = new GetProjectUserListQueryOut(
            new ProjectUserListVM(
                projectUserList.ID, projects.Select(p =>
                    new ProjectVM(p.ID, p.Name, p.Description)).ToArray()));

        return result.With(@out);
    }
}

public sealed record GetProjectUserListQuery(string ID) : IQuery<Result<GetProjectUserListQueryOut>>;
public sealed record GetProjectUserListQueryOut(ProjectUserListVM ProjectUserListVM);

public sealed record ProjectUserListVM(string ID, ProjectVM[] Projects);
public sealed record ProjectVM(string ID, string Name, string Description);