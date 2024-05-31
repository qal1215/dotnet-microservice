using Discount.Grpc;

namespace Basket.API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;

public record StoreBasketResult(ShoppingCart Cart);

public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(x => x.Cart)
            .NotNull().WithMessage("Cart can not be null!");

        RuleFor(x => x.Cart.UserName)
            .NotEmpty().WithMessage("Username is required!");
    }
}

public class StoreBasketCommandHandler
    (IBasketRepository repository, DiscountProtoService.DiscountProtoServiceClient discountProto)
    : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        await DeductDiscount(command.Cart, cancellationToken);

        await repository.StoreBasket(command.Cart, cancellationToken);

        return new StoreBasketResult(command.Cart);
    }

    private async Task DeductDiscount(ShoppingCart cart, CancellationToken cancellationToken)
    {
        foreach (var item in cart.Items)
        {
            CouponModel coupon = await discountProto.GetDiscountAsync(new GetDiscountRequest { ProductName = item.ProductName }, cancellationToken: cancellationToken);

            if (coupon is not null)
            {
                item.Price -= coupon.Amount;
            }
        }
    }
}
