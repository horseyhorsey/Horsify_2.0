using Prism.Interactivity.InteractionRequest;
using System;

namespace Horsesoft.Music.Horsify.Base.Interface
{
    public interface IHorsifyDialogService
    {
        void Show(string title, object content, InteractionRequest<IConfirmation> request, Action<IConfirmation> callback);
    }
}
