using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingPlatformApp.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        //kaç kayda etki ettiğini döndürür
        Task<int> SaveChangesAsync();

        //Task asenkron metotların voidi gibi düşünülebilir.
        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();

    }
}
