using System;
using DiscordRPC;
using DiscordRPC.Logging;
using Horsesoft.Music.Horsify.Base.Interface;

namespace Horsesoft.Horsify.ServicesModule
{
    public class DiscordRpcService : IDiscordRpcService, IDisposable
    {
        private DiscordRpcClient _discClient;

        /// <summary>
        /// Info to push to discord
        /// </summary>
        private RichPresence _presence;
        private bool IsEnabled;

        public DiscordRpcService(string appId)
        {
            _discClient = new DiscordRPC.DiscordRpcClient(appId);
            //Set the logger
            _discClient.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
        }

        public void Enable(bool enable)
        {
            if (!IsEnabled && enable)
            {
                //Subscribe to events
                _discClient.OnReady += (sender, e) =>
                {
                    Console.WriteLine("Received Ready from user {0}", e.User.Username);
                };
                _discClient.OnPresenceUpdate += (sender, e) =>
                {
                    Console.WriteLine("Received Update! {0}", e.Presence);
                };

                //Connect to the RPC
                _discClient.Initialize();

                _presence = new RichPresence()
                {
                    Details = "Horsify 2.0",
                    State = "Playing",
                    Assets = new Assets()
                    {
                        LargeImageKey = "small",
                        LargeImageText = "Horsify 2.0",
                    }
                };

                //Set the rich presence
                //Call this as many times as you want and anywhere in your code.
                _discClient.SetPresence(_presence);

                IsEnabled = true;
            }
            else
            {
                _discClient.OnReady -= null;
                _discClient.OnPresenceUpdate -= null;
                IsEnabled = false;
            }
        }

        public void SetPrecense(string state, string details, int songLength = 0, int position = 0)
        {
            _presence.State = state;
            _presence.Details = details;
            
             _presence.Timestamps = null;
            var timeLeft = songLength - position;            
            _discClient.UpdateEndTime(DateTime.Now.AddSeconds(timeLeft));
        }

        /// <summary>
        /// Invokes all the events, such as OnPresenceUpdate
        /// </summary>
        public void Update()
        {
            if (IsEnabled)
                _discClient.Invoke();
        }

        #region Disposable
        private bool _disposed = false;        
        public void Dispose()
        {
            if (!_disposed)
            {
                _discClient.Dispose();
                _disposed = true;
            }
        }

        #endregion
    }
}
