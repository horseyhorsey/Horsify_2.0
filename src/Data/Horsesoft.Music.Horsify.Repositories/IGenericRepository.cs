using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Horsesoft.Music.Horsify.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        void Delete(object id);
        void Delete(TEntity entity);

        /// <summary>
        /// Gets the specified Entity from filters and orders.
        /// </summary>
        /// <param name="filter">The filter expression</param>
        /// <param name="orderBy">The order by expression</param>
        /// <param name="includeProperties">The include properties - separated with commas</param>
        /// <returns></returns>
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        TEntity GetById(object id);
        void Insert(TEntity entity);
        void Update(TEntity entity);
    }

    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        #region Fields
        internal DbContext _context;
        internal DbSet<TEntity> _dbSet;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class. <para/>
        /// The constructor accepts a database context instance and initializes the entity set variable (_dbSet)
        /// </summary>
        /// <param name="context">The context.</param>
        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();            
            _context.ChangeTracker.AutoDetectChangesEnabled = false;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;            
        } 

        #endregion

        public void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public void Delete(TEntity entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            //Create the queryable
            IQueryable<TEntity> queryable = _dbSet;

            // Apply where filter clause
            if (filter != null)
                queryable = queryable.Where(filter);

            //Include navigation properties
            foreach (var includeProp in includeProperties.Split(new char[] { ',' },
                StringSplitOptions.RemoveEmptyEntries))
            {
                queryable = queryable.Include(includeProp);
            }

            //Order the entries
            if (orderBy != null)
                return orderBy(queryable).ToList();
            else
                return queryable.ToList();
        }

        public TEntity GetById(object id)
        {            
            return _dbSet.Find(id);
        }

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(TEntity entity)
        {

            this.DetachAllEntities();

            try
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
            catch { }
            finally
            {
                
            }
        }

        public void DetachAllEntities()
        {
            var entries = this._context.ChangeTracker.Entries().ToList();

            var changedEntriesCopy = entries
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted || e.State == EntityState.Unchanged)
                .ToList();
            foreach (var entity in changedEntriesCopy)
            {
                this._context.Entry(entity.Entity).State = EntityState.Detached;
            }
        }
    }
}
