using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatformApp.Data.Context;
using OnlineShoppingPlatformApp.Data.Entities;

namespace OnlineShoppingPlatformApp.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly OnlineShoppingPlatformAppDbContext _db;
        private readonly DbSet<TEntity> _dbSet;
        public Repository(OnlineShoppingPlatformAppDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<TEntity>(); // _db.Set<User> => _db.Users gibi 
        }
        public void Add(TEntity entity)
        {
            entity.CreatedDate = DateTime.Now;
            _dbSet.Add(entity);
        }

        public void Delete(TEntity entity, bool isSoftDelete = true)
        {
            if (isSoftDelete)
            {
                //soft delete
                entity.ModifiedDate = DateTime.Now;
                entity.IsDeleted = true;
                _dbSet.Update(entity);
            }
            else
            {
                _dbSet.Remove(entity);
            }
        }

        public void Delete(int id)
        {
            var entity = _dbSet.Find(id);
            Delete(entity);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? _dbSet : _dbSet.Where(predicate);
        }

        public TEntity GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public void Update(TEntity entity)
        {
            entity.ModifiedDate = DateTime.Now;
            _dbSet.Update(entity);

            // _db.SaveChanges();
            // bazen saveChanges olmasını istemeyebiliriz. Çünkü birden fazla işlem onaylandıktan sonra hepsi doğrulanabileceği
            // zaman saveChanges yapmak istiyebilriz.
            // örnek: bankadan para çekme işlemi gibi bu zincirleme işlemlerden sonra saveChanges yapmak isteyebiliriz.
            // NOT: bunada unit of work pattern(deseni) denir.
            //Unit of Work Pattern Nedir? Unit of Work ise bize; Veritabanı ile yapılacak olan tüm işlemleri, tek bir kanal aracılığı ile gerçekleştirme ve hafızada tutma işlemlerini sunmaktadır. Bu sayede işlemlerin toplu halde gerçekleştirilmesi ve hata durumunda geri alınabilmesi sağlamaktadır.
        }
    }

}

// not: _db.SaveChanges()'lar transaction durumları göz önünde bulundurularak yazılmalıdır. Unit of Work Pattern içinde yönetilecek.
// transaction: birden fazla işlemi bir arada yapmak istediğimizde, bu işlemlerin hepsi başarılı olursa işlemleri kaydetmek istediğimizde
// örnek: bankadan para çekme işlemi gibi bu zincirleme işlemlerden sonra saveChanges yapmak isteyebiliriz. hata durumunda geri almak istediğimizde rollback yapmak isteyebiliriz.