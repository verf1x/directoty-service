using DirectoryService.Application.Abstractions;

namespace DirectoryService.Application.Positions.GetById;

public record GetPositionByIdQuery(Guid Id) : IQuery;