using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Duende.IdentityServer.Models;

namespace MicroServiceApp.IdentityServer;

public interface IServiceBusSender<T> where T : class
{
    Task SendAsync<T>(T message);
}
public class AzureServiceBusMessageSender<T> : IServiceBusSender<T> where T : class
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly string _queueName;

    public AzureServiceBusMessageSender(string connectionString, string queueName)
    {
        _queueName = queueName;
        _serviceBusClient = new ServiceBusClient(connectionString);
    }
    
    public async Task SendAsync<T>(T message)
    {
        var sender = _serviceBusClient.CreateSender(_queueName);
        var json = JsonSerializer.Serialize(message);
        var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(json));
        await sender.SendMessageAsync(serviceBusMessage);
    }
}