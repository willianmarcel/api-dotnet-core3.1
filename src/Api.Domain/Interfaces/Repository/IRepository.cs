using System;
using System.Threading.Tasks;
using Api.Domain.Entities;
using System.Collections.Generic;

namespace Api.Domain.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
         Task<T> InsertAsync(T item);
         Task<T> UpdateAsysnc(T item);
         Task<bool> DeleteAsync(Guid id);
         Task<T> SelectAsync(Guid id);
         Task<IEnumerable<T>> SelectAsync();
         Task<bool> ExistsAsync(Guid id);
    }
}