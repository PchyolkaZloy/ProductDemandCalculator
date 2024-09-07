
using Swashbuckle.AspNetCore.Annotations;

[SwaggerSchema]
public class GoodItem
{
    [SwaggerSchema("цена продукта")]
    public decimal Price {get;set;}

    [SwaggerSchema("описание продукта")]
    public string Summary{get;set;}
}
