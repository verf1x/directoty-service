using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.Positions;
using DirectoryService.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LocationTimeZone = DirectoryService.Domain.Locations.TimeZone;
using Path = DirectoryService.Domain.Departments.Path;

namespace DirectoryService.Infrastructure.Postgres.Seeding;

public class DirectoryServiceSeeder
{
    private const int RootDepartmentCount = 8;
    private const int ChildDepartmentsPerRoot = 12;
    private const int TeamsPerChildDepartment = 8;
    private const int LocationCount = 60;
    private const int PositionCount = 120;
    private const int LocationsPerDepartment = 2;
    private const int PositionsPerDepartment = 5;

    private static readonly string[] Regions =
    [
        "Samara Region",
        "Moscow Region",
        "Saint Petersburg",
        "Tatarstan",
        "Sverdlovsk Region",
        "Novosibirsk Region",
        "Krasnodar Region",
        "Nizhny Novgorod Region",
    ];

    private static readonly string[] Cities =
    [
        "Samara",
        "Moscow",
        "Saint Petersburg",
        "Kazan",
        "Yekaterinburg",
        "Novosibirsk",
        "Krasnodar",
        "Nizhny Novgorod",
    ];

    private static readonly string[] Streets =
    [
        "Lenina",
        "Gagarina",
        "Molodezhnaya",
        "Sovetskaya",
        "Mira",
        "Sadovaya",
        "Centralnaya",
        "Industrialnaya",
    ];

    private static readonly string[] TimeZones =
    [
        "UTC",
        "Europe/Moscow",
        "Europe/Samara",
        "Asia/Yekaterinburg",
        "Asia/Novosibirsk",
    ];

    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<DirectoryServiceSeeder> _logger;

    public DirectoryServiceSeeder(
        DirectoryServiceDbContext dbContext,
        ILogger<DirectoryServiceSeeder> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        ValidateSettings();

        await _dbContext.Database.MigrateAsync();

        if (await DatabaseHasData())
        {
            _logger.LogInformation("Directory service database already contains data. Seeding skipped.");
            return;
        }

        _logger.LogInformation("Directory service seeding started.");

        var locations = CreateLocations();
        var positions = CreatePositions();
        var departments = CreateDepartments();
        var departmentLocations = CreateDepartmentLocations(departments, locations);
        var departmentPositions = CreateDepartmentPositions(departments, positions);

        _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

        try
        {
            await _dbContext.Locations.AddRangeAsync(locations);
            await _dbContext.Positions.AddRangeAsync(positions);
            await _dbContext.Departments.AddRangeAsync(departments);
            await _dbContext.Set<DepartmentLocation>().AddRangeAsync(departmentLocations);
            await _dbContext.Set<DepartmentPosition>().AddRangeAsync(departmentPositions);

            await _dbContext.SaveChangesAsync();
        }
        finally
        {
            _dbContext.ChangeTracker.AutoDetectChangesEnabled = true;
        }

        _logger.LogInformation(
            "Directory service seeding completed. Departments: {DepartmentsCount}, locations: {LocationsCount}, positions: {PositionsCount}, department-location links: {DepartmentLocationLinksCount}, department-position links: {DepartmentPositionLinksCount}.",
            departments.Count,
            locations.Count,
            positions.Count,
            departmentLocations.Count,
            departmentPositions.Count);
    }

    private async Task<bool> DatabaseHasData()
    {
        return await _dbContext.Departments.AnyAsync()
               || await _dbContext.Locations.AnyAsync()
               || await _dbContext.Positions.AnyAsync();
    }

    private List<Location> CreateLocations()
    {
        var locations = new List<Location>(LocationCount);

        for (int index = 0; index < LocationCount; index++)
        {
            var address = CreateValue(
                Address.Create(
                    postalCode: (100000 + index).ToString(),
                    region: Pick(Regions, index),
                    city: Pick(Cities, index),
                    district: $"District {(index % 12) + 1}",
                    street: $"{Pick(Streets, index)} street",
                    house: $"{(index % 120) + 1}",
                    building: index % 3 == 0 ? $"{(index % 8) + 1}" : null,
                    apartment: index % 4 == 0 ? $"{(index % 200) + 1}" : null),
                nameof(Address));

            locations.Add(
                new Location(
                    CreateValue(LocationName.Create($"Office {index + 1}"), nameof(LocationName)),
                    address,
                    CreateValue(LocationTimeZone.Create(Pick(TimeZones, index)), nameof(LocationTimeZone))));
        }

        return locations;
    }

    private List<Position> CreatePositions()
    {
        var positions = new List<Position>(PositionCount);

        for (int index = 0; index < PositionCount; index++)
        {
            positions.Add(
                new Position(
                    PositionId.CreateNew(),
                    CreateValue(PositionName.Create($"Position {index + 1}"), nameof(PositionName)),
                    CreateValue(
                        Description.Create(
                            $"Seeded position #{index + 1} for directory service load, filtering, and relationship scenarios."),
                        nameof(Description)),
                    []));
        }

        return positions;
    }

    private List<Department> CreateDepartments()
    {
        var departments = new List<Department>(CalculateDepartmentCount());

        for (int rootIndex = 0; rootIndex < RootDepartmentCount; rootIndex++)
        {
            var rootIdentifier = CreateIdentifier("division", rootIndex);
            var root = CreateRootDepartment(rootIndex, rootIdentifier);

            departments.Add(root);

            for (int childIndex = 0; childIndex < ChildDepartmentsPerRoot; childIndex++)
            {
                var childIdentifier = CreateIdentifier("department", rootIndex, childIndex);
                var child = CreateChildDepartment(
                    $"Department {rootIndex + 1}.{childIndex + 1}",
                    childIdentifier,
                    root.Id,
                    root.Path,
                    root.Depth);

                departments.Add(child);

                for (int teamIndex = 0; teamIndex < TeamsPerChildDepartment; teamIndex++)
                {
                    var teamIdentifier = CreateIdentifier("team", rootIndex, childIndex, teamIndex);
                    departments.Add(
                        CreateChildDepartment(
                            $"Team {rootIndex + 1}.{childIndex + 1}.{teamIndex + 1}",
                            teamIdentifier,
                            child.Id,
                            child.Path,
                            child.Depth));
                }
            }
        }

        return departments;
    }

    private List<DepartmentLocation> CreateDepartmentLocations(
        IReadOnlyList<Department> departments,
        IReadOnlyList<Location> locations)
    {
        var departmentLocations = new List<DepartmentLocation>(departments.Count * LocationsPerDepartment);

        for (int departmentIndex = 0; departmentIndex < departments.Count; departmentIndex++)
        {
            foreach (var location in PickMany(locations, departmentIndex, LocationsPerDepartment))
            {
                departmentLocations.Add(new DepartmentLocation(departments[departmentIndex].Id, location.Id));
            }
        }

        return departmentLocations;
    }

    private List<DepartmentPosition> CreateDepartmentPositions(
        IReadOnlyList<Department> departments,
        IReadOnlyList<Position> positions)
    {
        var departmentPositions = new List<DepartmentPosition>(departments.Count * PositionsPerDepartment);

        for (int departmentIndex = 0; departmentIndex < departments.Count; departmentIndex++)
        {
            foreach (var position in PickMany(positions, departmentIndex * PositionsPerDepartment, PositionsPerDepartment))
            {
                departmentPositions.Add(new DepartmentPosition(departments[departmentIndex].Id, position.Id));
            }
        }

        return departmentPositions;
    }

    private Department CreateRootDepartment(int rootIndex, Identifier identifier)
    {
        return CreateValue(
            Department.CreateRoot(
                DepartmentId.CreateNew(),
                CreateValue(DepartmentName.Create($"Division {rootIndex + 1}"), nameof(DepartmentName)),
                identifier,
                []),
            nameof(Department));
    }

    private Department CreateChildDepartment(
        string name,
        Identifier identifier,
        DepartmentId parentId,
        Path parentPath,
        short parentDepth)
    {
        return CreateValue(
            Department.CreateChild(
                DepartmentId.CreateNew(),
                CreateValue(DepartmentName.Create(name), nameof(DepartmentName)),
                identifier,
                parentId,
                CreateValue(Path.Create($"{parentPath.Value}.{identifier.Value}"), nameof(Path)),
                (short)(parentDepth + 1),
                []),
            nameof(Department));
    }

    private Identifier CreateIdentifier(string prefix, params int[] parts)
    {
        string suffix = string.Join(string.Empty, parts.Select(ToLetters));

        return CreateValue(Identifier.Create($"{prefix}{suffix}"), nameof(Identifier));
    }

    private IReadOnlyList<T> PickMany<T>(IReadOnlyList<T> items, int startIndex, int count)
    {
        var selected = new List<T>(count);

        for (int offset = 0; offset < count; offset++)
        {
            selected.Add(items[(startIndex + offset) % items.Count]);
        }

        return selected;
    }

    private string Pick(IReadOnlyList<string> items, int index)
    {
        return items[index % items.Count];
    }

    private string ToLetters(int value)
    {
        const string alphabet = "abcdefghijklmnopqrstuvwxyz";

        var result = string.Empty;
        var number = value;

        do
        {
            result = alphabet[number % alphabet.Length] + result;
            number = (number / alphabet.Length) - 1;
        }
        while (number >= 0);

        return result;
    }

    private int CalculateDepartmentCount()
    {
        return RootDepartmentCount * (1 + ChildDepartmentsPerRoot + (ChildDepartmentsPerRoot * TeamsPerChildDepartment));
    }

    private T CreateValue<T>(CSharpFunctionalExtensions.Result<T, Error> result, string name)
    {
        if (result.IsFailure)
            throw new InvalidOperationException($"Failed to create seed value '{name}': {result.Error.Message}");

        return result.Value;
    }

    private void ValidateSettings()
    {
        if (RootDepartmentCount <= 0)
            throw new InvalidOperationException($"{nameof(RootDepartmentCount)} must be greater than zero.");

        if (ChildDepartmentsPerRoot <= 0)
            throw new InvalidOperationException($"{nameof(ChildDepartmentsPerRoot)} must be greater than zero.");

        if (TeamsPerChildDepartment <= 0)
            throw new InvalidOperationException($"{nameof(TeamsPerChildDepartment)} must be greater than zero.");

        if (LocationCount < LocationsPerDepartment)
            throw new InvalidOperationException($"{nameof(LocationCount)} must be greater than or equal to {nameof(LocationsPerDepartment)}.");

        if (PositionCount < PositionsPerDepartment)
            throw new InvalidOperationException($"{nameof(PositionCount)} must be greater than or equal to {nameof(PositionsPerDepartment)}.");

        if (LocationsPerDepartment <= 0)
            throw new InvalidOperationException($"{nameof(LocationsPerDepartment)} must be greater than zero.");

        if (PositionsPerDepartment <= 0)
            throw new InvalidOperationException($"{nameof(PositionsPerDepartment)} must be greater than zero.");
    }
}
