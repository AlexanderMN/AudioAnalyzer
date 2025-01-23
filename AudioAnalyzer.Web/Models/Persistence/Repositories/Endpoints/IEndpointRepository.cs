using System.Net;

namespace AudioAnalyzer.Web.Models.Persistence.Repositories.Endpoints;

public interface IEndpointRepository<TKey> 
                                              where TKey : 
                                              IComparable<TKey>, IEquatable<TKey>
{
    public IDictionary<TKey, IPEndPoint> GetEndpoints(); // получение всех объектов
    public IPEndPoint GetEndpoint(TKey id); // получение одного объекта по id
    
}