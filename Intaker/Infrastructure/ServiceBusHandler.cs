using Azure;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using CSharpFunctionalExtensions;
using Polly;

namespace Intaker.Infrastructure;

public record Error(string Message);

public record NotFoundError(string Message) : Error(Message);

public record InfrastructureError(string Message) : Error(Message);
public interface IBus
{
    Task<UnitResult<Error>> SendMessage<TMessage>(string queueName, TMessage message);
}
public class ServiceBusHandler : IBus
{
    private readonly ServiceBusClient _client;

    public ServiceBusHandler(string fqdn)
    {
        var clientOptions = new ServiceBusClientOptions
        { 
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        _client = new ServiceBusClient(
            fqdn,
            new DefaultAzureCredential(),
            clientOptions);
    }

    public async Task<UnitResult<Error>> SendMessage<TMessage>(string queueName, TMessage message)
    {
        if (string.IsNullOrWhiteSpace(queueName))
            return new InfrastructureError("Queue name cannot be empty");
        var sender = _client.CreateSender(queueName);
        var outcome = await Policy.Handle<ServiceBusException>()
            .WaitAndRetryAsync([TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1)])
            .ExecuteAndCaptureAsync(() => sender.SendMessageAsync(new ServiceBusMessage(BinaryData.FromObjectAsJson(message))));
        if(outcome.Outcome == OutcomeType.Failure)
            return new InfrastructureError(outcome.FinalException.Message);
        return UnitResult.Success<Error>();
    }
}
