using OpenAI;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Samples.Whisper
{
    public class Whisper : MonoBehaviour
    {
        [SerializeField] private Button recordButton;
        [SerializeField] private Image progressBar;
        [SerializeField] private Text message;
        [SerializeField] private Dropdown dropdown;

        private readonly string fileName = "output.wav";
        private readonly int duration = 5;

        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai = new OpenAIApi();
        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "Act as 3 year old child. Dont break character. Don't ever mentioned you are an AI model. We are going to play a game where i mentioned an object and you have to make 1 imaginative question about the question as 3 year old kid would do";


        private void Start()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
#else
            foreach (var device in Microphone.devices)
            {
                dropdown.options.Add(new Dropdown.OptionData(device));
            }
            recordButton.onClick.AddListener(StartRecording);
            dropdown.onValueChanged.AddListener(ChangeMicrophone);

            var index = PlayerPrefs.GetInt("user-mic-device-index");
            dropdown.SetValueWithoutNotify(index);
#endif
        }

        private void ChangeMicrophone(int index)
        {
            PlayerPrefs.SetInt("user-mic-device-index", index);
        }

        private void StartRecording()
        {
            isRecording = true;
            recordButton.enabled = false;

            var index = PlayerPrefs.GetInt("user-mic-device-index");

#if !UNITY_WEBGL
            clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
#endif
        }

        private async void EndRecording()
        {
            message.text = "Transcripting...";

#if !UNITY_WEBGL
            Microphone.End(null);
#endif

            byte[] data = SaveWav.Save(fileName, clip);

            // Obtener la transcripción del audio
            string transcribedText = await GetAudioTranscription(data);

            // Enviar la transcripción a ChatGPT para obtener la pregunta imaginativa
            await GenerateImaginativeQuestion(transcribedText);

            // Restablecer la UI
            progressBar.fillAmount = 0;
            recordButton.enabled = true;
        }

        private async Task<string> GetAudioTranscription(byte[] audioData)
        {
            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() { Data = audioData, Name = "audio.wav" },
                Model = "whisper-1",
                Language = "es"
            };

            var res = await openai.CreateAudioTranscription(req);
            return res.Text;
        }

        private async Task GenerateImaginativeQuestion(string transcribedText)
        {
            // Limpiar la lista de mensajes
            messages.Clear();

            // Agregar la transcripción de voz como mensaje de usuario
            ChatMessage userMessage = new ChatMessage();
            userMessage.Content = transcribedText;
            userMessage.Role = "user";
            messages.Add(userMessage);

            // Agregar el mensaje de inicio como prompt
            if (messages.Count == 0)
            {
                ChatMessage promptMessage = new ChatMessage();
                promptMessage.Content = prompt;
                promptMessage.Role = "user";
                messages.Add(promptMessage);
            }

            // Crear la solicitud para ChatGPT
            CreateChatCompletionRequest request = new CreateChatCompletionRequest();
            request.Messages = messages;
            request.Model = "gpt-3.5-turbo";

            // Enviar la solicitud a ChatGPT
            var response = await openai.CreateChatCompletion(request);

            // Procesar la respuesta de ChatGPT
            if (response.Choices != null && response.Choices.Count > 0)
            {
                var chatResponse = response.Choices[0].Message;
                messages.Add(chatResponse);

                // Mostrar la respuesta en la consola (o en la UI según necesidades)
                Debug.Log(chatResponse.Content);
                message.text = chatResponse.Content;
            }
        }

        private void Update()
        {
            if (isRecording)
            {
                time += Time.deltaTime;
                progressBar.fillAmount = time / duration;

                if (time >= duration)
                {
                    time = 0;
                    isRecording = false;
                    EndRecording();
                }
            }
        }
    }
}