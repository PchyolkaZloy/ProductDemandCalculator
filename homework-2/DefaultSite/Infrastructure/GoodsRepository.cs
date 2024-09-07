using Domain.Dao;

namespace Infrastructure;

public class GoodsRepository:IGoodsRepository
{

    private readonly Dictionary<Guid,GoodItemDto>  _goodItems = new();
    public Guid Add(GoodItemDto item)
    {
        var index = Guid.NewGuid();
        item.Id = index;
        _goodItems.Add(index, item);
        
        return index;
    }

    public GoodItemDto Find(Guid id)
    {
        if(!_goodItems.ContainsKey(id))
        {
            throw new NotFoundExceptionException();
        }

        return _goodItems[id];
    }

    public IReadOnlyList<GoodItemDto> Find(IGoodsRepository.Filter filter)
    {
        throw new NotImplementedException();
    }
}
