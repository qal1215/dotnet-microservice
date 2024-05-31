namespace Basket.API.Basket.StoreBasket;

public record StoreBasketReququest(ShoppingCart Cart);

public record StoreBasketResponse(ShoppingCart Cart);


public class StoreBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket", async (StoreBasketReququest request, ISender sender) =>
        {
            var command = request.Adapt<StoreBasketCommand>();

            var result = await sender.Send(command);

            var respone = result.Adapt<StoreBasketResponse>();

            return Results.Created($"/basket/{respone.Cart.UserName}", respone);
        })
        .WithName("StoreBasket")
        .Produces<StoreBasketResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithSummary("Store a basket")
        .WithDescription("Store a basket for a user");
    }
}
