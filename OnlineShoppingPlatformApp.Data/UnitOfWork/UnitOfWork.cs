using Microsoft.EntityFrameworkCore.Storage;
using OnlineShoppingPlatformApp.Data.Context;

namespace OnlineShoppingPlatformApp.Data.UnitOfWork
{
    /// <summary>
    /// Unit of Work Pattern implementasyonu.
    /// Veritabanı işlemlerini tek bir transaction içinde yönetir.
    /// Tüm repository'lerin kullandığı ortak context'i tutar.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OnlineShoppingPlatformAppDbContext _db;
        private IDbContextTransaction _transaction;

        public UnitOfWork(OnlineShoppingPlatformAppDbContext db)
        {
            _db = db;
        }
        // Unit of Work Pattern Nedir? Unit of Work ise bize; Veritabanı ile yapılacak olan tüm işlemleri, tek bir kanal aracılığı ile gerçekleştirme ve hafızada tutma işlemlerini sunmaktadır. Bu sayede işlemlerin toplu halde gerçekleştirilmesi ve hata durumunda geri alınabilmesi sağlamaktadır.

        /// <summary>
        /// Yeni bir veritabanı transaction'ı başlatır.
        /// Bu method, birbiriyle ilişkili birden fazla veritabanı işlemini 
        /// tek bir atomic işlem olarak gruplamak için kullanılır.
        /// </summary>
        public async Task BeginTransactionAsync()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
        }

        /// <summary>
        /// Transaction içinde yapılan tüm değişiklikleri veritabanına kalıcı olarak kaydeder.
        /// Eğer transaction içindeki tüm işlemler başarılı ise bu method çağrılır.
        /// </summary>
        public async Task CommitTransactionAsync()
        {
            await _transaction.CommitAsync();
        }

        /// <summary>
        /// Transaction içinde yapılan tüm değişiklikleri geri alır.
        /// Herhangi bir hata durumunda veritabanı tutarlılığını korumak için çağrılır.
        /// </summary>
        public async Task RollbackTransactionAsync()
        {
            await _transaction.RollbackAsync();
        }

        /// <summary>
        /// Context üzerinde yapılan tüm değişiklikleri veritabanına kaydeder.
        /// Transaction kullanılmayan durumlarda direkt olarak değişiklikleri kaydetmek için kullanılır.
        /// </summary>
        /// <returns>Etkilenen satır sayısı</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }

        /// <summary>
        /// DbContext ve Transaction gibi kaynakları temizler.
        /// Using bloğu içinde kullanıldığında otomatik olarak çağrılır.
        /// </summary>
        public void Dispose()
        {
            _db.Dispose();

            // Garbage Collector'a işi biten nesneyi temizleme talimatı verir.
            // o an silinmez, silinebilir yapıyoruz.

            // GC.Collect() -> gc direk çalıştırmak istersek 
            // GC.WaitForPendingFinalizers() -> işi biten nesneleri bekler
            // bu 2'si garbage collector'ı direk çalıştırmak için kullanılır.
        }
    }
}