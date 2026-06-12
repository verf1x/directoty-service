using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using DirectoryService.Application.Abstractions;
using DirectoryService.Application.Database;
using DirectoryService.Application.Validation;
using DirectoryService.Contracts;
using DirectoryService.Contracts.Departments;
using DirectoryService.Domain.Shared;
using FluentValidation;

namespace DirectoryService.Application.Departments.Get;

public class GetDepartmentsHandler(
    IValidator<GetDepartmentsQuery> validator,
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetDepartmentsQuery, PagedResult<GetDepartmentsResponseItemDto>>
{
    public async Task<Result<PagedResult<GetDepartmentsResponseItemDto>, ErrorList>> HandleAsync(
        GetDepartmentsQuery query,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(query, cancellationToken);

        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var parameters = new DynamicParameters();
        parameters.Add(
            "search",
            string.IsNullOrWhiteSpace(query.Search) ? null : $"%{query.Search}%",
        DbType.String);
        parameters.Add("limit", query.Pagination.PageSize, DbType.Int32);
        parameters.Add("offset", (query.Pagination.Page - 1) * query.Pagination.PageSize, DbType.Int32);

        var sortBy = query.SortBy switch
        {
            "name" => "d.name",
            "created_at" => "d.created_at",
            _ => "d.name"
        };

        var sortDirection = string.Equals(query.SortDirection, "DESC", StringComparison.OrdinalIgnoreCase)
            ? "DESC"
            : "ASC";

        var orderByClause = $"ORDER BY {sortBy} {sortDirection}";

        using var dbConnection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var departments = await dbConnection.QueryAsync<GetDepartmentsRow>(
            $"""
            SELECT
                d.id,
                d.name,
                d.path,
                d.created_at,
                COUNT(*) OVER() AS total_count
            FROM departments d
            WHERE (@search IS NULL OR d.name ILIKE @search)
            {orderByClause}
            LIMIT @limit OFFSET @offset;
            """,
            param: parameters);

        var totalCount = departments.FirstOrDefault()?.TotalCount ?? 0;

        var responseItems = departments.Select(d => new GetDepartmentsResponseItemDto
        {
            Id = d.Id,
            Name = d.Name,
            Path = d.Path,
            CreatedAt = d.CreatedAt,
        }).ToList();

        return new PagedResult<GetDepartmentsResponseItemDto>(
            responseItems,
            query.Pagination.Page,
            query.Pagination.PageSize,
            totalCount);
    }
}
