﻿using Mediator;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using PageTree.Client.Shared.Services.CQRS;

namespace PageTree.Client.Web;

public static class Startup
{
    public static void AddCQRSRepositories(this IServiceCollection services)
    {
       
    }

    public static void AddCQRS(this IServiceCollection services)
    {
        services.AddSingleton<IQueryExecutor>(sp =>
            new QueryExecutorTryCatchDecorator<AccessTokenNotAvailableException>(
                new MediatorQueryExecutor(sp.GetRequiredService<IMediator>()), onCatch: ex => ex.Redirect()));

    }
}
