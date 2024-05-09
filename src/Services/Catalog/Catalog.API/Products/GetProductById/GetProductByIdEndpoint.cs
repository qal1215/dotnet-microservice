
namespace Catalog.API.Products.GetProductById;

public record GetProductRequest(Guid Id);
public record GetProductResponse(Product Product);

public class GetProductByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProductByIdQuery(id));

            var response = result.Adapt<GetProductResponse>();

            return Results.Ok(response);
        })
        .WithName("GetProductById")
        .Produces<GetProductResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Gets a product by its ID")
        .WithDescription("Gets a product by its ID from the catalog");
    }
}
