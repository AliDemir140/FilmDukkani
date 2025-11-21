using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;


namespace Application.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        //Get
        Task<T> GetByIdAsync(int id);

        //GetAll
        //Parametresiz
        Task<IReadOnlyList<T>> GetAllAsync();

        //Parametreli
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate);

        //Add
        Task<T> AddAsync(T entity);

        //Update
        Task UpdateAsync(T entity);

        //Delete
        Task DeleteAsync(T entity);
    }
}