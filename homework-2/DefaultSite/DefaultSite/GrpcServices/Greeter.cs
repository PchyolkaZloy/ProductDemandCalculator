using Grpc.Core;
using GrpcService;

public class GreeterService:GrpcService.Greeter.GreeterBase
{
    private readonly IGoodsRepository _goodsRepository;

    public GreeterService(IGoodsRepository goodsRepository)
    {
        _goodsRepository = goodsRepository;
    }


    public override Task<Reply> SayHello(HelloRequest request, ServerCallContext context)
    {
       if(Guid.TryParse(request.Name, out var id))
       {
        var item = _goodsRepository.Find(id);
        return Task.FromResult(new Reply{
            Message = item.Summary
        });
       }

       return Task.FromResult(new Reply{
        Message = "Not FOUND"
       });
    }
}