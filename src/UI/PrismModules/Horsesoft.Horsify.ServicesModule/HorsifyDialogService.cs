using Horsesoft.Music.Horsify.Base.Interface;
using Prism.Interactivity.InteractionRequest;
using System;

namespace Horsesoft.Horsify.ServicesModule
{
    public class HorsifyDialogService : IHorsifyDialogService
    {
        public void Show(string title, object content, InteractionRequest<IConfirmation> request, Action<IConfirmation> callback)
        {
            request.Raise(new Confirmation { Content = content, Title = title }
                , callback);
        }
    }
}
