using Domain.Dao;

public interface IGoodsRepository
{
    Guid Add(GoodItemDto item);


    public class Filter
    {
        public DateTime CreatedAt{get;set;}
        public int GoodType{get;set;}
    }

    IReadOnlyList<GoodItemDto> Find(Filter filter);

    GoodItemDto Find(Guid id);
}