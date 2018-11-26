using System;

namespace Horsesoft.Music.Data.Model.Horsify
{
    public enum VoiceCommand
    {
        None,
        Play,
        Queue,
        Search
    }

    public interface IVoiceControl
    {
        /// <summary>
        /// Is a Horsify Command active
        /// </summary>
        bool Activated { get; set; }

        /// <summary>
        /// The type of Voice Command
        /// </summary>
        VoiceCommand Command { get; set; }

        /// <summary>
        /// Is Voice recognition disabled
        /// </summary>
        bool Disabled { get; set; }

        /// <summary>
        /// Starts the recognition engine
        /// </summary>
        /// <returns></returns>
        bool Start();

        /// <summary>
        /// Stops the recognition engine
        /// </summary>
        void Stop();

        /// <summary>
        /// A voice command has been processed and sent
        /// </summary>
        event VoiceCommandDelegate VoiceCommandSent;        
    }

    public delegate void VoiceCommandDelegate(string voicetext);
}
