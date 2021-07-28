using System.Collections.Generic;

namespace Data.Repositories.Abstract
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        TEntity AddOrUpdate(TEntity entity);
        TEntity Get(int id);
        TEntity Delete(int id);
        List<TEntity> GetAll();
    }
}