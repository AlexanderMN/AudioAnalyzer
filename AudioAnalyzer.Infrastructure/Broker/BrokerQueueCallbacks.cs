using System.Reflection;

namespace RabbitMqInfrastructure.Broker;

public abstract class BrokerQueueCallbacks
{
  public Dictionary<string, Delegate> Callbacks { get; set;}

    protected BrokerQueueCallbacks()
    {
        Callbacks = new Dictionary<string, Delegate>();
    }
    
    protected void RegisterDelegates(Type? childType)
    {
        if (childType == null)
            return;
        
        if (!childType.IsSubclassOf(typeof(BrokerQueueCallbacks)))
            return;
        
        var eventMethods = childType.GetMethods(BindingFlags.NonPublic | 
                                                BindingFlags.Instance | 
                                                BindingFlags.DeclaredOnly);

        eventMethods = eventMethods.Where(m => m.IsSpecialName == false).ToArray();
        
        foreach (var eventMethod in eventMethods)
        {
            var eventDelegate = Delegate.CreateDelegate(
                type: typeof(Func<object, BrokerEventArgs, Task>), this, eventMethod, true);
            
            if (eventDelegate == null)
                continue;
            
            Callbacks.Add(eventDelegate.Method.Name, eventDelegate);
        }
    }
}
