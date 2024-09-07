namespace Domain.Dao;

public class GoodItemDto
{
    public Guid Id {get;set;}

    public decimal Price {get;set;}

    public string? Summary{get;set;}

    public DateTime CreatedAt{get;set;}
}
