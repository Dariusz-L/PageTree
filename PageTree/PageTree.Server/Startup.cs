using Common.Basic.DDD;
using Common.Basic.Repository;
using Corelibs.AspNetApi.Authorization;
using Corelibs.Basic.Repository;
using Corelibs.BlazorShared;
using Corelibs.MongoDB;
using Corelibs.MongoDB.Logging;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PageTree.App.UseCases;
using PageTree.Server.Data;
using PageTree.Server.DataUpdates;

namespace PageTree.Server.Api
{
    public static class Startup
    {
        public static void AddPageTreeSetup(this IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddMediatorExt();
            services.AddMongoDbLogger(env);

            services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DbContextTransactionBehaviour<,>));

            services.AddRepositories();
            services.AddResourceAuthorizationHandlers();
        }

        private static void AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IRepository<DataUpdate>, DbContextRepository<DataUpdate>>();

            services.AddJsonDbRepository<Domain.Users.User, PageTree.Server.Data.User>(nameof(AppDbContext.Users));
            services.AddJsonDbRepository<Domain.Projects.ProjectUserList, PageTree.Server.Data.ProjectUserList>(nameof(AppDbContext.ProjectUserLists));
            services.AddJsonDbRepository<Domain.Projects.Project, PageTree.Server.Data.Project>(nameof(AppDbContext.Projects));
            services.AddJsonDbRepository<Domain.Page, PageTree.Server.Data.Page>(nameof(AppDbContext.Pages));
            services.AddJsonDbRepository<App.Entities.Signatures.Signature, PageTree.Server.Data.Signature>(nameof(AppDbContext.Signatures));
            services.AddJsonDbRepository<Domain.Practice.PracticeCategory, PageTree.Server.Data.PracticeCategory>(nameof(AppDbContext.PracticeCategories));
            services.AddJsonDbRepository<Domain.Practice.PracticeTactic, PageTree.Server.Data.PracticeTactic>(nameof(AppDbContext.PracticeTactics));
        }

        private static void AddResourceAuthorizationHandlers(this IServiceCollection services)
        {
            // clean up! common method!
            services.AddAuthorization(AddEditPolicies);
            services.AddSingleton<IAuthorizationHandler, ResourceAuthorizationHandler<Domain.Projects.Project>>();
            services.AddSingleton<IAuthorizationHandler, ResourceAuthorizationHandler<Domain.Projects.ProjectUserList>>();
            services.AddSingleton<IAuthorizationHandler, ResourceAuthorizationHandler<Domain.Page>>();

            void AddEditPolicies(AuthorizationOptions options)
            {
                services.AddPolicyAndHandler<Domain.Projects.Project>(options);
                services.AddPolicyAndHandler<Domain.Projects.ProjectUserList>(options);
                services.AddPolicyAndHandler<Domain.Page>(options);
            }
        }

        private static void AddJsonDbRepository<TEntity, TDataEntity>(this IServiceCollection services, string tableName)
            where TEntity : class, IEntity
            where TDataEntity : JsonEntity, new()
        {
            services.AddScoped<IRepository<TEntity>>(sp =>
            {
                var dbContext = sp.Get<AppDbContext>();
                var dbContextRP = new DbContextRepository<TDataEntity>(dbContext);
                return new JsonEntityRepositoryDecorator<TEntity, TDataEntity>(dbContextRP);
            });
        }

        private static void AddMongoDbLogger(this IServiceCollection services, IWebHostEnvironment env)
        {
            var conn = Environment.GetEnvironmentVariable("LogDatabaseConn");
            var databaseName = env.IsDevelopment() ? "pageTree_log_dev" : "pageTree_log_prod";

            var logger = new MongoDbLogger(new MongoDbRepositoryT<Log>(conn, databaseName, "logs"));
            services.AddSingleton<Corelibs.Basic.Logging.ILogger>(logger);
            logger.Log("Start server");
        }
    }
}
