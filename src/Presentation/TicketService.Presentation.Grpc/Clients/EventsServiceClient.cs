using EventService.Presentation.Grpc;
using TicketService.Application.Contracts.Clients;

namespace TicketService.Presentation.Grpc.Clients;

public class EventsServiceClient : IEventServiceClient
{
    private readonly SeatValidationGrpcService.SeatValidationGrpcServiceClient _client;

    public EventsServiceClient(SeatValidationGrpcService.SeatValidationGrpcServiceClient client)
    {
        _client = client;
    }

    public async Task<RefundResult> RefundSeats(long schemeId, long row, long number, CancellationToken cancellationToken)
    {
        var req = new ReturnSeatsRequest
        {
            HallSchemeId = schemeId,
        };

        req.Seats.Add(new SeatInfo
        {
            Row = (int)row,
            SeatNumber = (int)number,
        });

        ReturnSeatsResponse resp = await _client.ReturnSeatsAsync(req, cancellationToken: cancellationToken);

        return new RefundResult(Success: resp.Success);
    }
}