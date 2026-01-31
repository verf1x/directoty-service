using System.Text;
using Dapper;

namespace DirectoryService.Application.Extensions;

public static class SqlExtensions
{
    extension(StringBuilder queryBuilder)
    {
        public void ApplySorting(
            string sortBy,
            string sortDirection)
        {
            if (!string.IsNullOrWhiteSpace(sortBy))
                queryBuilder.Append($"\nORDER BY {sortBy} {sortDirection.ToUpperInvariant()}");
        }

        public void ApplyPagination(
            DynamicParameters parameters,
            int pageNumber,
            int pageSize)
        {
            parameters.Add("PageSize", pageSize);
            parameters.Add("Offset", (pageNumber - 1) * pageSize);
            queryBuilder.Append("\nLIMIT @PageSize OFFSET @Offset");
        }
    }
}