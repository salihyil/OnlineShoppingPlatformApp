using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OnlineShoppingPlatformApp.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        //Repository niye kullanıyoruz?
        //Repository, veritabanı işlemlerini gerçekleştirmek için kullanılan bir tasarım desenidir.

        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity, bool isSoftDelete = true);
        void Delete(int id);
        TEntity GetById(int id);

        //Get (x => x.Name == "Ahmet") gibi
        TEntity Get(Expression<Func<TEntity, bool>> predicate);

        //birden fazla nesne geldiği için bunları sıralamak istiyebiliriz IQueryable kullanırız
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null);


    }
}