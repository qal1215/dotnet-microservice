

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
    (IDocumentSession session)
    : IQueryHandler<GetProductByCategoryQuery, GetProductByCategoryResult>
{
    public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
    {
        var products = await session.Query<Product>()
            .Where(p => p.Categories.Contains(query.Catelogy))
            .ToListAsync(cancellationToken);

        return new GetProductByCategoryResult(products);
    }
}
