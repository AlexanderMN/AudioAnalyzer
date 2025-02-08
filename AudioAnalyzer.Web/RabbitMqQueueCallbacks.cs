using System.Reflection;
using System.Text;
using AudioAnalyzer.Infrastructure.Broker;

namespace AudioAnalyzer.Web;

public class RabbitMqQueueCallbacks : BrokerQueueCallbacks
{
    
    public RabbitMqQueueCallbacks()
    {
        RegisterDelegates();
    }
    
    public void RegisterDelegates()
    {
        var eventMethods = typeof(RabbitMqQueueCallbacks).GetMethods(BindingFlags.NonPublic | 
                                                             BindingFlags.Static | 
                                                             BindingFlags.DeclaredOnly);

        foreach (var eventMethod in eventMethods)
        {
            var eventDelegate = Delegate.CreateDelegate(typeof(Action<object, BrokerEventArgs>), eventMethod);
            Callbacks.Add(eventDelegate.Method.Name, eventDelegate);
        }
    }
    
    
    private static void Search(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);
        Console.WriteLine(text);
        // var jsonResponse = JsonSerializer.Deserialize<AnalyzerResponseJson>(text);
        //
        // _searchViewModel.AudioAnalyzerResponse = jsonResponse ?? new AnalyzerResponseJson();
    }

    private static void Transcribe(object state, BrokerEventArgs args)
    {
        string text = Encoding.UTF8.GetString(args.Message, 0, args.Message.Length);
        Console.WriteLine(text);
        // var jsonResponse = JsonSerializer.Deserialize<AnalyzerResponseJson>(text);
        //
        // if (jsonResponse != null)
        // {
        //     _transcribeViewModel.TranscribedText = jsonResponse.AudioResponses[0].AnalyzedTexts[0].Text;
        // }
        // else
        // {
        //     _transcribeViewModel.TranscribedText = text;
        // }
    }
}
