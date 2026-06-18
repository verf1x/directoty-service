using Bogus;
using DirectoryService.Application.Positions.Create;

namespace DirectoryService.IntegrationTests.Fakers;

public class CreatePositionCommandFakers
{
    public static Faker<CreatePositionCommand> Create(Guid[] departmentIds)
        => new Faker<CreatePositionCommand>()
            .CustomInstantiator(f => new CreatePositionCommand(
                $"Position {f.Random.AlphaNumeric(10)}",
                $"Description {f.Random.AlphaNumeric(20)}",
                departmentIds));

    public static Faker<CreatePositionCommand> CreateWithoutDescription(Guid[] departmentIds)
        => new Faker<CreatePositionCommand>()
            .CustomInstantiator(_ => Create(departmentIds).Generate() with
            {
                Description = null,
            });

    public static Faker<CreatePositionCommand> CreateWithMinLengthName(Guid[] departmentIds)
        => new Faker<CreatePositionCommand>()
            .CustomInstantiator(_ => Create(departmentIds).Generate() with
            {
                Name = "Abc",
            });

    public static Faker<CreatePositionCommand> CreateWithMaxLengthName(Guid[] departmentIds)
        => new Faker<CreatePositionCommand>()
            .CustomInstantiator(_ => Create(departmentIds).Generate() with
            {
                Name = new string('A', 100),
            });

    public static Faker<CreatePositionCommand> CreateWithInvalidLongName(Guid[] departmentIds)
        => new Faker<CreatePositionCommand>()
            .CustomInstantiator(_ => Create(departmentIds).Generate() with
            {
                Name = new string('A', 101),
            });

    public static Faker<CreatePositionCommand> CreateWithInvalidLongDescription(Guid[] departmentIds)
        => new Faker<CreatePositionCommand>()
            .CustomInstantiator(_ => Create(departmentIds).Generate() with
            {
                Description = new string('A', 1001),
            });

    public static Faker<CreatePositionCommand> CreateWithInvalidEmptyDescription(Guid[] departmentIds)
        => new Faker<CreatePositionCommand>()
            .CustomInstantiator(_ => Create(departmentIds).Generate() with
            {
                Description = " ",
            });
}
