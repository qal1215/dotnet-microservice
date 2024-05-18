

namespace Catalog.API.Products.GetProductByCategory;

public record GetProductByCategoryQuery(string Catelogy) : IQuery<GetProductByCategoryResult>;

public record GetProductByCategoryResult(IEnumerable<Product> Products);

public class GetProductByCategoryQueryValidator : AbstractValidator<GetProductByCategoryQuery>
{
    public GetProductByCategoryQueryValidator()
    {
        RuleFor(x => x.Catelogy).NotEmpty().WithMessage("Category is required");
    }
}

internal class GetProductByCategoryQueryHandler
    (IDocumentSession session, ILogger<GetProductByCategoryQueryHandler> logger)
    : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
{
    public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("GetProductByCatelogyQueryHandler.Handle called with {@Query}", query);

        var products = await session.Query<Product>()
            .Where(p => p.Categories.Contains(query.Catelogy))
            .ToListAsync(cancellationToken);

        return new GetProductByCategoryResult(products);
    }
}
