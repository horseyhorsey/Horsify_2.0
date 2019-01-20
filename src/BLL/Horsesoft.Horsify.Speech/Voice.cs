using Horsesoft.Music.Data.Model.Horsify;
using SpeechLib;
using System;
using System.Speech.Recognition;

namespace Horsesoft.Horsify.Speech
{

    public class VoiceControl : IVoiceControl
    {
        public bool Activated { get; set; }
        public bool Disabled { get; set; } = true;
        public VoiceCommand Command { get; set; }
        public event VoiceCommandDelegate VoiceCommandSent;

        private SpeechRecognitionEngine _recognizer;
        private DictationGrammar dict = new DictationGrammar();        

        public VoiceControl()
        {
            VoiceCommandSent = new VoiceCommandDelegate(OnVoiceSent);
            _recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));

            AddWordToSpeechDictionary("Neurofunk");
            AddWordToSpeechDictionary("Techstep");
            AddWordToSpeechDictionary("Drum & Bass, Neurofunk");
        }

        private void OnVoiceSent(string voicetext)
        {
            return;
        }

        /// <summary>
        /// Adds a pronunciation to windows speech dict. TODO: Add other cultures
        /// </summary>
        /// <param name="words"></param>
        private void AddWordToSpeechDictionary(string words)
        {
            SpLexicon lex = new SpeechLib.SpLexicon();
            int langid = new System.Globalization.CultureInfo("en-US").LCID;
            lex.AddPronunciation(words, langid);
        }

        /// <summary>
        /// TODO: can fail if no input device. Should be quiet in debug?
        /// </summary>
        private void StartListening()
        {
            Disabled = false;

            var choices = new Choices("horsify search", "horsify play", "horsify queue");
            // Create and load a dictation grammar.  
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder(choices)));

            // Add a handler for the speech recognized event.  
            _recognizer.SpeechRecognized +=
              new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);

            // Configure input to the speech recognizer.  
            _recognizer.SetInputToDefaultAudioDevice();

            // Start asynchronous, continuous speech recognition.  
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (Activated)
            {
                if (Command == VoiceCommand.Search)
                {
                    VoiceCommandSent?.Invoke($"{e.Result.Text}");
                    Command = VoiceCommand.None;                    
                    _recognizer.UnloadGrammar(dict);
                    Activated = false;
                }
                else if (Command == VoiceCommand.Play)
                {
                    VoiceCommandSent?.Invoke($"{e.Result.Text}");
                    Command = VoiceCommand.None;
                    _recognizer.UnloadGrammar(dict);
                    Activated = false;
                }
                else if (Command == VoiceCommand.Queue)
                {
                    VoiceCommandSent?.Invoke($"{e.Result.Text}");
                    Command = VoiceCommand.None;
                    _recognizer.UnloadGrammar(dict);
                    Activated = false;
                }
            }

            if (!Activated)
            {
                if (e.Result.Text == "horsify search")
                {
                    Activated = true;
                    VoiceCommandSent?.Invoke($"{e.Result.Text}");
                    Console.WriteLine("horsify search activated");
                    _recognizer.LoadGrammar(dict);
                    Command = VoiceCommand.Search;
                    return;
                }
                else if (e.Result.Text == "horsify play")
                {
                    Activated = true;
                    VoiceCommandSent?.Invoke($"{e.Result.Text}");
                    Console.WriteLine("horsify play activated");
                    _recognizer.LoadGrammar(dict);
                    Command = VoiceCommand.Play;
                    return;
                }
                else if (e.Result.Text == "horsify queue")
                {
                    Activated = true;
                    VoiceCommandSent?.Invoke($"{e.Result.Text}");
                    Console.WriteLine("horsify queue activated");
                    _recognizer.LoadGrammar(dict);
                    Command = VoiceCommand.Queue;
                    return;
                }
            }
        }

        public bool Start()
        {
            if (Disabled)
            {
                StartListening();
                return true;
            }

            return false;
        }

        public void Stop()
        {            
            _recognizer.RecognizeAsyncStop();
            Activated = false;
            Command = VoiceCommand.None;
            Disabled = true;
        }
    }
}
