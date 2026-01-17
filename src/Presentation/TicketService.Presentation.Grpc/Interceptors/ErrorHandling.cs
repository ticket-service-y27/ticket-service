using Grpc.Core;
using Grpc.Core.Interceptors;
using TicketService.Application.Models.Promocodes;
using TicketService.Application.Models.Tickets;

namespace TicketService.Presentation.Grpc.Interceptors;

public class ErrorHandling : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (TicketException e)
        {
            var error = new RpcException(
                new Status(StatusCode.InvalidArgument, e.Message));

            throw error;
        }
        catch (PromocodeException e)
        {
            var error = new RpcException(
                new Status(StatusCode.InvalidArgument, e.Message));

            throw error;
        }
        catch (Exception e)
        {
            var error = new RpcException(
                new Status(StatusCode.Internal, e.Message));

            throw error;
        }
    }
}