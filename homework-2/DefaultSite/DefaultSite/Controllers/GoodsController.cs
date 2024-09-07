using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace DefaultSite.Controllers;

[ApiController]
[Route("/api/v1/[controller]")]
public class GoodsController : ControllerBase
{

    private readonly ILogger<GoodsController> _logger;
    private readonly IGoodsRepository _goodsRepository;
    private readonly IOptions<MyOptions> _options;
    private readonly IValidator<GoodItem> _validator;
    private readonly IValidator<UpdateGoodItem> _updateGoodItemValidator;

    public GoodsController(ILogger<GoodsController> logger,
        IGoodsRepository goodsRepository,
        IOptions<MyOptions> options,
        IValidator<GoodItem> validator,
        IValidator<UpdateGoodItem> updateGoodItemValidator)
    {
        _logger = logger;
        _goodsRepository = goodsRepository;
        _options = options;
        _validator = validator;
        _updateGoodItemValidator= updateGoodItemValidator;
    }

    [HttpPost]
    [SwaggerOperation("метод добавления товара")]
    public IActionResult AddGoods(GoodItem goodItem)
    {
        var result = _validator.Validate(goodItem);
        if(!result.IsValid)
        {
            return BadRequest();
        }

        return Ok(_goodsRepository.Add(new Domain.Dao.GoodItemDto(){
            Price = goodItem.Price,
            Summary = string.IsNullOrEmpty(goodItem.Summary) ? 
            _options.Value.DefaultName :  goodItem.Summary
        }));
    }

    [HttpGet]
    [SwaggerOperation("метод поиска товара по id")]
    public IActionResult FindById(Guid id)
    {
       var value =  _goodsRepository.Find(id);
        return Ok(new GoodItem(){
            Price = value.Price,
            Summary = value.Summary
        });
    }

    [HttpPatch]
    public IActionResult UpdateItem(UpdateGoodItem item)
    {
        var validator = _updateGoodItemValidator.Validate(item);
         if(!validator.IsValid)
        {
            return BadRequest();
        }

        return Ok();
    }
}
