using Bogus;
using DirectoryService.Domain.Departments;

namespace DirectoryService.IntegrationTests.Fakers;

public static class DepartmentFaker
{
    public static Faker<Department> Default => new Faker<Department>("ru")
        .CustomInstantiator(f => Department.CreateRoot(
            DepartmentId.CreateNew(),
            DepartmentName.Create(f.Commerce.Department()).Value,
            Identifier.Create(f.Lorem.Sentence(10)).Value,
            []).Value);
}