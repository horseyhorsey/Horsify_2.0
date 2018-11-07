using Prism.Logging;
using Prism.Mvvm;
using System.Runtime.CompilerServices;

namespace Horsesoft.Music.Horsify.Base.ViewModels
{
    /// <summary>
    /// This base has a the logger attached for easier use
    /// </summary>
    /// <seealso cref="Prism.Mvvm.BindableBase" />
    public abstract class HorsifyBindableBase : BindableBase
    {
        protected readonly ILoggerFacade _loggerFacade;

        public HorsifyBindableBase(ILoggerFacade loggerFacade)
        {
            _loggerFacade = loggerFacade;
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cat">The cat. (log level)</param>
        /// <param name="priority">The priority.</param>
        public void Log(string message, Category cat = Category.Debug, Priority priority = Priority.None, [CallerMemberName] string caller = "")
        {            
            _loggerFacade.Log($"{this.GetType().Name} | {caller} | {message}" , cat, priority);
        }
    }
}
