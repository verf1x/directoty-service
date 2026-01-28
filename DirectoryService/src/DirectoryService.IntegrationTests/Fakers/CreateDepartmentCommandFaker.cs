using Bogus;
using DirectoryService.Application.Departments.Create;

namespace DirectoryService.IntegrationTests.Fakers;

public class CreateDepartmentCommandFaker
{
    public static Faker<CreateDepartmentCommand> CreateRoot(Guid[] locationIds)
        => new Faker<CreateDepartmentCommand>()
            .CustomInstantiator(f => new CreateDepartmentCommand(
                $"Department_{f.Random.AlphaNumeric(10)}",
                f.Internet.DomainWord().ToLower(),
                null,
                locationIds));


    public static Faker<CreateDepartmentCommand> CreateChild(Guid parentId, Guid[] locationIds)
        => new Faker<CreateDepartmentCommand>()
            .CustomInstantiator(f => new CreateDepartmentCommand(
                $"Department_{f.Random.AlphaNumeric(10)}",
                f.Internet.DomainWord().ToLower(),
                parentId,
                locationIds));
}