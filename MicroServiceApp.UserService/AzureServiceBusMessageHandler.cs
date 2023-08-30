using System.Text;
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using MicroServiceApp.UserService.Services;

namespace MicroServiceApp.UserService;

public interface IServiceBusProcessor<T>
{
    Task StartAsync(Action<T>? action);
}
public class AzureServiceBusMessageHandler<T> : IServiceBusProcessor<T>
{
    private readonly ServiceBusClient _serviceBusClient;
    private readonly string _queueName;
    private ServiceBusProcessor? _processor;
    private Action<T>? _action;

    public AzureServiceBusMessageHandler(string connectionString, string queueName)
    {
        _queueName = queueName;
        _serviceBusClient = new ServiceBusClient(connectionString);
    }

    private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        Console.WriteLine(arg.Exception.ToString());
        return Task.CompletedTask;
    }

    private Task ProcessMessageAsync(ProcessMessageEventArgs arg)
    {
         var message = arg.Message;
         var json = Encoding.UTF8.GetString(message.Body);
         var obj = JsonConvert.DeserializeObject<T>(json);
         _action?.Invoke(obj);
         return Task.CompletedTask;
    }

    public Task StartAsync(Action<T>? action)
    {
        _action = action;
        _processor = _serviceBusClient.CreateProcessor(_queueName);
        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;
        return _processor.StartProcessingAsync();
    }
}