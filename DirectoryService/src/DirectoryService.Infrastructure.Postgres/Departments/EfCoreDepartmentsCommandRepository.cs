using CSharpFunctionalExtensions;
using DirectoryService.Application.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Shared;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Departments;

public class EfCoreDepartmentsCommandRepository : IDepartmentsCommandRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<EfCoreDepartmentsCommandRepository> _logger;

    public EfCoreDepartmentsCommandRepository(
        DirectoryServiceDbContext dbContext,
        ILogger<EfCoreDepartmentsCommandRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Guid, Error>> AddAsync(Department department, CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return department.Id.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error adding department with ID {DepartmentId} to the database",
                department.Id.Value);

            return Error.Failure(
                "department.insert",
                "An error occurred while adding the department to the database.");
        }
    }
}