using System.Reflection;
using System.Text;
using System.Text.Json;
using AudioAnalyzer.Infrastructure.Broker;
using AudioAnalyzer.Web.Hubs;
using AudioAnalyzer.Web.Models.AudioAnalyzerResponse;

namespace AudioAnalyzer.Web;

public class RabbitMqQueueCallbacks : BrokerQueueCallbacks
{
    private FileUploadHub _fileUploadHub;
    public RabbitMqQueueCallbacks(FileUploadHub fileUploadHub)
    {
        _fileUploadHub = fileUploadHub;
        RegisterDelegates();
    }
    
    public void RegisterDelegates()
    {
        var eventMethods = typeof(RabbitMqQueueCallbacks).GetMethods(BindingFlags.NonPublic | 
                                                             BindingFlags.Instance | 
                                                             BindingFlags.DeclaredOnly);

        foreach (var eventMethod in eventMethods)
        {
            var eventDelegate = Delegate.CreateDelegate(
                type: typeof(Func<object, BrokerEventArgs, Task>), this, eventMethod, true);
            Callbacks.Add(eventDelegate.Method.Name, eventDelegate);
        }
    }
    
    
    private async Task Search(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);
        var jsonResponse = JsonSerializer.Deserialize<AnalyzerResponseJson>(text);
        
        if (jsonResponse == null)
            return;
        
        await _fileUploadHub.SendFileProcessedMessage(jsonResponse.AudioResponses[0].Filename, text);
    }

    private async Task Transcribe(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);
        Console.WriteLine(text);
        var jsonResponse = JsonSerializer.Deserialize<AnalyzerResponseJson>(text);
        
        if (jsonResponse == null)
            return;
        
        await _fileUploadHub.SendFileProcessedMessage(jsonResponse.AudioResponses[0].Filename, text);
    }
}
