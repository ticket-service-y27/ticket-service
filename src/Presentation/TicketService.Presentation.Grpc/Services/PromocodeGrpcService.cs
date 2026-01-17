using Grpc.Core;
using TicketService.Application.Contracts.Promocodes;
using TicketService.Grpc.Promocodes;
using Promocode = TicketService.Application.Models.Promocodes.Promocode;

namespace TicketService.Presentation.Grpc.Services;

public class PromocodeGrpcService : PromocodesService.PromocodesServiceBase
{
    private readonly IPromocodesService _promocodesService;

    public PromocodeGrpcService(IPromocodesService promocodesService)
    {
        _promocodesService = promocodesService;
    }

    public override async Task<CreatePromocodeResponse> CreatePromocode(CreatePromocodeRequest request, ServerCallContext context)
    {
        await _promocodesService.CreatePromocodeAsync(
            request.Promo,
            request.DiscountPercentage,
            request.Count,
            context.CancellationToken);

        return new CreatePromocodeResponse();
    }

    public override async Task<GetPromocodeByCodeResponse> GetPromocodeByCode(GetPromocodeByCodeRequest request, ServerCallContext context)
    {
        Promocode? promocode = await _promocodesService.GetByCodeAsync(request.Code,  context.CancellationToken);

        if (promocode == null)
        {
            return new GetPromocodeByCodeResponse();
        }

        return new GetPromocodeByCodeResponse
        {
            Promocode = new TicketService.Grpc.Promocodes.Promocode
            {
                Id = promocode.Id,
                Code = promocode.Promo,
                DiscountPercentage = promocode.DiscountPercentage,
                Count = promocode.Count,
            },
        };
    }
}