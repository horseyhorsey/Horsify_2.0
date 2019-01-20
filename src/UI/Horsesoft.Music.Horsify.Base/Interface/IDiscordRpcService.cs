namespace Horsesoft.Music.Horsify.Base.Interface
{
    public interface IDiscordRpcService
    {
        /// <summary>
        /// Enable/Disable, doesn't dispose
        /// </summary>
        /// <param name="enable"></param>
        void Enable(bool enable);

        void SetPrecense(string state, string details, int songLength = 0, int position = 0);
        /// <summary>
        /// Invokes all the events, such as OnPresenceUpdate
        /// </summary>
        void Update();

        /// <summary>
        /// Clear the presence
        /// </summary>
        void Clear();
    }
}
