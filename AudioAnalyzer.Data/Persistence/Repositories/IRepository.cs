namespace AudioAnalyzer.Data.Persistence.Repositories;


public interface IRepository<T> : IDisposable 
    where T : class
{
    IEnumerable<T> GetEntityList(); // получение всех объектов
    T? GetEntity(int id, bool includeRelatedEntities); // получение одного объекта по id
    void Create(T item); // создание объекта
    void Update(T item); // обновление объекта
    void Delete(int id); // удаление объекта по id
    Task SaveAsync();  // сохранение изменений
}
