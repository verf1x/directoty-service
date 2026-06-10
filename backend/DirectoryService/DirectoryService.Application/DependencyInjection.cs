using DirectoryService.Application.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DirectoryService.Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
            => services
                .AddCommands()
                .AddQueries()
                .AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        private IServiceCollection AddCommands()
            => services.Scan(scan => scan.FromAssemblies(typeof(DependencyInjection).Assembly)
                .AddClasses(classes => classes.AssignableToAny(
                    typeof(ICommandHandler<,>),
                    typeof(ICommandHandler<>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime());

        private IServiceCollection AddQueries()
            => services.Scan(scan => scan.FromAssemblies(typeof(DependencyInjection).Assembly)
                .AddClasses(classes => classes.AssignableToAny(
                    typeof(IQueryHandler<,>),
                    typeof(IQueryHandler<>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime());
    }
}