namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(string Name, List<string> Categories, string Description, string ImageFile, decimal Price)
    : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);


internal class CreateProductCommandHandler(IDocumentSession session)
    : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        // Business logic to create a product
        //create Product entity from command properties
        //save to database
        //return CreateProductResult result

        var product = new Product
        {
            Name = command.Name,
            Categories = command.Categories,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price
        };

        //TODO
        //save to db
        session.Store(product);
        await session.SaveChangesAsync();

        //return the result
        return new CreateProductResult(product.Id);
    }
}
