namespace AudioAnalyzer.Data.Persistence.Repositories;


public interface IRepository<T> : IDisposable 
    where T : class
{
    List<T> GetEntityList(Func<T, bool>? predicate = null); // получение всех объектов
    Task<T?> GetEntity(int id, bool includeRelatedEntities); // получение одного объекта по id
    void Create(T item); // создание объекта
    void Update(T item); // обновление объекта
    Task Delete(int id); // удаление объекта по id
    Task SaveAsync();  // сохранение изменений
}
