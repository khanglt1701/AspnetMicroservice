using Discount.Grpc.Protos;

namespace Basket.API.GrpcServices
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _discoutProtoService;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discoutProtoService)
        {
            _discoutProtoService = discoutProtoService;
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            var discountRequest = new GetDiscountRequest { ProductName = productName };
            return await _discoutProtoService.GetDiscountAsync(discountRequest);
        }
    }
}
