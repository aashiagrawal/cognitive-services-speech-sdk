//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CognitiveServices.Speech.Tests.EndToEnd
{
    sealed class SpeechWithStreamHelper
    {
        private readonly SpeechFactory factory;
        private readonly TimeSpan timeout;

        public SpeechWithStreamHelper(SpeechFactory factory)
        {
            this.factory = factory;
            timeout = TimeSpan.FromSeconds(90);
        }

        SpeechRecognizer CreateSpeechRecognizerWithStream(String audioFile, String language = null, SpeechOutputFormat format = SpeechOutputFormat.Simple)
        {
            var stream = Config.OpenWaveFile(audioFile);
            if (string.IsNullOrEmpty(language))
            {
                return this.factory.CreateSpeechRecognizerWithStream(stream);
            }
            else
            {
                return this.factory.CreateSpeechRecognizerWithStream(stream, language, format);
            }
        }

        public async Task<SpeechRecognitionResult> GetSpeechFinalRecognitionResult(String audioFile, String language = null, SpeechOutputFormat format = SpeechOutputFormat.Simple)
        {
            using (var recognizer = CreateSpeechRecognizerWithStream(audioFile, language, format))
            {
                SpeechRecognitionResult result = null;
                await Task.WhenAny(recognizer.RecognizeAsync().ContinueWith(t => result = t.Result), Task.Delay(timeout));
                return result;
            }
        }

        public async Task<List<SpeechRecognitionResultEventArgs>> GetSpeechFinalRecognitionContinuous(String audioFile, String language = null, SpeechOutputFormat format = SpeechOutputFormat.Simple)
        {
            using (var recognizer = CreateSpeechRecognizerWithStream(audioFile, language, format))
            {
                var tcs = new TaskCompletionSource<bool>();
                var textResultEvents = new List<SpeechRecognitionResultEventArgs>();

                recognizer.FinalResultReceived += (s, e) => textResultEvents.Add(e);

                recognizer.OnSessionEvent += (s, e) =>
                {
                    if (e.EventType == SessionEventType.SessionStoppedEvent)
                    {
                        tcs.TrySetResult(true);
                    }
                };

                await recognizer.StartContinuousRecognitionAsync();
                await Task.WhenAny(tcs.Task, Task.Delay(timeout));
                await recognizer.StopContinuousRecognitionAsync();
                
                return textResultEvents;
            }
        }

        public async Task<List<List<SpeechRecognitionResultEventArgs>>> GetSpeechIntermediateRecognitionContinuous(String audioFile, String language = null, SpeechOutputFormat format = SpeechOutputFormat.Simple)
        {
            using (var recognizer = CreateSpeechRecognizerWithStream(audioFile, language, format))
            {
                var tcs = new TaskCompletionSource<bool>();
                var listOfIntermediateResults = new List<List<SpeechRecognitionResultEventArgs>>();
                List<SpeechRecognitionResultEventArgs> receivedIntermediateResultEvents = null;

                recognizer.OnSessionEvent += (s, e) =>
                {
                    if (e.EventType == SessionEventType.SessionStartedEvent)
                    {
                        receivedIntermediateResultEvents = new List<SpeechRecognitionResultEventArgs>();
                    }
                    if (e.EventType == SessionEventType.SessionStoppedEvent)
                    {
                        tcs.TrySetResult(true);
                    }
                };
                recognizer.IntermediateResultReceived += (s, e) => receivedIntermediateResultEvents.Add(e);
                recognizer.FinalResultReceived += (s, e) =>
                {
                    listOfIntermediateResults.Add(receivedIntermediateResultEvents);
                    receivedIntermediateResultEvents = new List<SpeechRecognitionResultEventArgs>();
                };

                await recognizer.StartContinuousRecognitionAsync();
                await Task.WhenAny(tcs.Task, Task.Delay(timeout));
                await recognizer.StopContinuousRecognitionAsync();

                return listOfIntermediateResults;
            }
        }
    }
}
