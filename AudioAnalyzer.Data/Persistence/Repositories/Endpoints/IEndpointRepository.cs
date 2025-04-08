using System.Net;

namespace AudioAnalyzer.Data.Persistence.Repositories.Endpoints;

public interface IEndpointRepository<TKey>
{
    public IDictionary<TKey, IPEndPoint> GetEndpoints(); // получение всех объектов
    public IPEndPoint GetEndpoint(TKey id); // получение одного объекта по id
    
}