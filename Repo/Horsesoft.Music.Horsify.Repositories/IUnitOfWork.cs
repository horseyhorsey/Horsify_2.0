using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Horsesoft.Music.Horsify.Repositories
{
    public interface IUnitOfWork
    {
        void Save();
        Task SaveAsync();
    }

    public abstract class UnitOfWork : IUnitOfWork, IDisposable
    {
        #region Fields
        private bool _disposed = false;
        internal DbContext _context;
        #endregion

        public UnitOfWork()
        {            
        }

        public UnitOfWork(DbContext _context)
        {
            
        }

        #region Public Methods

        public void Save()
        {
            _context.SaveChanges();
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources. Specifically the underlying context.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }

            _disposed = true;
        }

        #endregion
    }
}
