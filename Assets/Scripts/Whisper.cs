using Amazon.Polly.Model;
using Amazon.Polly;
using Amazon.Runtime;
using OpenAI;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Scripts.TexToSpeech;
using Scripts.DrawingProgress;
using Amazon;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using Unity.VisualScripting;
using System.Security.Cryptography;
using Scripts.Conversation;
using TMPro;
using Valve.VR;

namespace Samples.Whisper
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] private Text message;

        public TMP_Text questionTvText;
        public TMP_Text answerTvText;
        public TMP_Text scoreTvText;

        private readonly string fileName = "output.wav";
        private readonly int duration = 5;

        private AudioClip clip;
        private OpenAIApi openai = new OpenAIApi();
        private OpenAIApi openAI = new OpenAIApi();
        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "Act as 5 y/o child. Dont break character. Dont ever mention you are an AI model. Create an imaginative question based on an object or a previous answer, just the question.";
        private string scorePrompt = "Score the answer given to a question from 1-10, dont write words just the score based on a 5 y/o child pov and if the answer and question are in the same context. ";
        public List<int> scores = new List<int>();
        public TextToSpeech tts;
        private CreateChatCompletionRequest requestAI;
        public AnimationsHandler animationsHandler;
        public DrawingProgress drawingProgress;
        public QuestionCountdown questionCountdown;
        private string question;
        public Conversation conversation;
        public AudioManager audioManager;
        public int contadorMusica;
        public EndGame endGame;
        public bool askedAlready = false;
        public SteamVR_Action_Boolean triggerAction;

        private void Start()
        {
            //drawingProgress = GetComponent<DrawingProgress>();
            //GenerateImaginativeQuestion("Pillow", QuestionMode.OBJECT);
            //Debug.Log("Inicio");
            scores.Add(8);
            contadorMusica = 0;
        }

        public void StartRecording()
        {

#if !UNITY_WEBGL
            clip = Microphone.Start(Microphone.devices[0], false, duration, 44100);
#endif
        }

        public async void EndRecording()
        {

#if !UNITY_WEBGL
            Microphone.End(null);
            conversation.listening = false;
#endif
            if (conversation.playing)
            {
                byte[] data = SaveWav.Save(fileName, clip);

                // Obtener la transcripción del audio
                string transcribedText = await GetAudioTranscription(data);
                answerTvText.text = "Your answer: " + transcribedText;

                await scoreAnswer(transcribedText); // Enviar la transcripción a ChatGPT para obtener la pregunta imaginativa

                if (scores.Count > 0 && scores.Last() <= 7)
                {
                    scoreTvText.text += ", Try again!";
                    conversation.talking = true;
                    contadorMusica++;
                    audioManager.changeTrack(contadorMusica);
                    StartCoroutine(questionCountdown.UpdateTime());
                    await Task.Delay(20000);
                    await GenerateImaginativeQuestion(transcribedText, QuestionMode.ASK_AGAIN);
                    Debug.Log("BAD, TRY AGAIN");

                }
                else if (scores.Count > 0 && scores.Last() > 7)
                {
                    scoreTvText.text += ", Good answer!";

                    drawingProgress.increaseIndex();

                    messages.Clear();
                    contadorMusica = 0;
                    audioManager.changeTrack(contadorMusica); // TODO: handle win case

                    if (drawingProgress.GetDrawnObjects() < 4)
                    {
                        askedAlready = false;
                        questionTvText.text = "Question: (Please look arond to find an object)";
                    }
                }
            }
        }

        public async Task<string> GetAudioTranscription(byte[] audioData)
        {
            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() { Data = audioData, Name = "audio.wav" },
                Model = "whisper-1",
                Language = "en"
            };

            var res = await openai.CreateAudioTranscription(req);
            Debug.Log(res.Text);

            return res.Text;
        }


        private int ExtractRatingFromResponse(string responseText)
        {
            Match match = Regex.Match(responseText, @"\d+");

            if (match.Success)
            {
                return int.Parse(match.Value);
            }
            else
            {
                return -1; //Si no se encuentra un número en el comentario se devuelve -1 por lo que no se imprime en consola
            }
        }

        public async Task GenerateImaginativeQuestion(string transcribedText, QuestionMode mode) //no es necesariamente transcripcion, tambien es objeto
        {
            Debug.Log("--------------------LLEGO PREGUNTA------------------------");
            Debug.Log(transcribedText);
            ChatMessage newMessage = new ChatMessage();
            //newMessage.Content = transcribedText;
            newMessage.Role = "user";
            var questionPrompt = prompt;
            if (mode == QuestionMode.ASK_AGAIN)
            {
                var previousAnswer = transcribedText;
                questionPrompt += "Previous answer: " + previousAnswer;
            }
            else
            {
                var objeto = transcribedText;
                questionPrompt += "Object: " + objeto;
            }

            newMessage.Content = questionPrompt;
            messages.Add(newMessage);

            requestAI = new CreateChatCompletionRequest();
            requestAI.Messages = messages;
            requestAI.Model = "gpt-3.5-turbo";

            var aiResponse = await openAI.CreateChatCompletion(requestAI);

            if (aiResponse.Choices != null && aiResponse.Choices.Count > 0)
            {
                var chatResponse = aiResponse.Choices[0].Message;
                messages.Add(chatResponse);
                string text = chatResponse.Content;
                question = text;
                questionTvText.text = "Question: " + text + (mode == QuestionMode.OBJECT ? " [" + transcribedText + "]" : ""); // trasncribedText es objeto
                scoreTvText.text = "Score: ";
                tts.texttospeech(text);
                conversation.listening = true;
            }

            answerTvText.text = "Your answer: (Hold A to record)";

        }

        async void Update()
        {
            if (Input.GetKeyDown(KeyCode.T) && conversation.listening)
            {
                Debug.Log("Tecla T presionada");
                StartRecording();
            }
            else if (Input.GetKeyUp(KeyCode.T) && conversation.listening)
            {
                Debug.Log("Tecla T no presionada");
                await Task.Delay(1000);
                EndRecording();
            }

            if (triggerAction.GetStateDown(SteamVR_Input_Sources.Any))
            {
                Debug.Log("Tecla Trigger presionada");
                StartRecording();
            }
            else if (triggerAction.GetStateUp(SteamVR_Input_Sources.Any))
            {
                Debug.Log("Tecla Trigger no presionada");
                EndRecording();
            }


        }

        private async Task scoreAnswer(string transcribedAnswer)
        {
            int rating = AverageScore();
            try
            {
                Debug.Log("--------------------LLEGO SCORE------------------------");
                ChatMessage newMessage = new ChatMessage();
                var fullScorePrompt = scorePrompt;
                var answer = transcribedAnswer;
                fullScorePrompt += "Question: " + question + ". Answer: " + answer;
                newMessage.Content = fullScorePrompt;
                newMessage.Role = "user";
                messages.Add(newMessage);

                requestAI = new CreateChatCompletionRequest();
                requestAI.Messages = messages;
                requestAI.Model = "gpt-3.5-turbo";

                var aiResponse = await openAI.CreateChatCompletion(requestAI);

                if (aiResponse.Choices != null && aiResponse.Choices.Count > 0)
                {
                    var chatResponse = aiResponse.Choices[0].Message;
                    string text = chatResponse.Content;
                    rating = ExtractRatingFromResponse(text);

                    if (rating == -1)
                    {
                        rating = AverageScore();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            scoreTvText.text = $"Score: {rating}/10";
            controllAnswersValues(rating);
            scores.Add(rating);

            animationsHandler.setRating(rating);

            Debug.Log("Calificación obtenida: " + scores.Last());
        }

        private int AverageScore()
        {
            if (scores.Count >= 1)
            {
                int promedio = (int)Math.Floor((Math.Round(scores.Average(), 1)));
                return promedio;
            }
            else
            {
                return 1;
            }
        }

        public void controllAnswersValues(double score)
        {
            if (score < 4)
            {
                conversation.soBad_v++;
                if (conversation.soBad_v == 3)
                {
                    endGame.razon = 2;
                    conversation.playing = false;
                }
            }
            else if (score >= 4 && score <= 7)
            {
                conversation.bad_v++;
                if (conversation.bad_v == 2)
                {
                    conversation.soBad_v++;
                    conversation.bad_v = 0;
                }
            }
            else if (score > 7)
            {
                if (conversation.soBad_v > 0) { conversation.soBad_v--; }
                else if (conversation.bad_v > 0) { conversation.bad_v--; }
            }


        }

        public enum QuestionMode
        {
            ASK_AGAIN,
            OBJECT
        }

        private void resetTvtTexts()
        {
            questionTvText.text = "Question: ";
            answerTvText.text = "Your answer: ";
            scoreTvText.text = "Score: ";
        }
    }
}