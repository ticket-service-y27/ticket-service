using Grpc.Core;
using TicketService.Application.Contracts.Tickets;
using TicketService.Grpc.Tickets;
using TicketService.Presentation.Grpc.Mappers;

namespace TicketService.Presentation.Grpc.Services;

public class TicketGrpcService : TicketsService.TicketsServiceBase
{
    private readonly ITicketService _ticketService;
    private readonly ModelMapper _mapper;

    public TicketGrpcService(ITicketService ticketService, ModelMapper mapper)
    {
        _ticketService = ticketService;
        _mapper = mapper;
    }

    public override async Task<CreateTicketResponse> CreateTicket(CreateTicketRequest request, ServerCallContext context)
    {
        long id = await _ticketService.CreateAsync(
            request.UserId,
            request.EventId,
            request.Price,
            request.Row,
            request.Number,
            context.CancellationToken,
            string.IsNullOrEmpty(request.AppliedPromocode) ? null : request.AppliedPromocode);

        return new CreateTicketResponse { TicketId = id };
    }

    public override async Task<PayTicketResponse> PayTicket(
        PayTicketRequest request,
        ServerCallContext context)
    {
        try
        {
            await _ticketService.PayTicketAsync(
                request.TicketId,
                context.CancellationToken);

            return new PayTicketResponse { Success = true };
        }
        catch (Exception e)
        {
            return new PayTicketResponse
            {
                Success = false,
                FailReason = e.Message,
            };
        }
    }

    public override async Task<GetUserTicketsResponse> GetUserTickets(
        GetUserTicketsRequest request,
        ServerCallContext context)
    {
        var result = new GetUserTicketsResponse();

        await foreach (Application.Models.Tickets.Ticket ticket in await _ticketService.GetAllTicketsByUserIdAsync(context.CancellationToken))
        {
            result.Tickets.Add(_mapper.Map(ticket));
        }

        return result;
    }

    public override async Task<RefundTicketsResponse> RefundTickets(RefundTicketsRequest request, ServerCallContext context)
    {
        bool res = await _ticketService.RefundSeats(request.TicketId, request.SchemeId, context.CancellationToken);

        return new RefundTicketsResponse
        {
            Success = res,
        };
    }
}