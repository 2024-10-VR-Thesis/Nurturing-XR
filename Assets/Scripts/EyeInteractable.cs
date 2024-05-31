using UnityEngine;
using UnityEngine.Events;
using Samples.Whisper;
using Scripts.Conversation;
using System.Linq;
using System.Threading.Tasks;
using Scripts.DrawingProgress;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    public bool IsHovered { get; set; }
    public bool AskedAlready { get; set; }
    public string objectName;

    public UnityEvent<GameObject> OnObjectHover;

    [SerializeField] Whisper whisper;
    [SerializeField] Conversation conversation;

    public QuestionCountdown questionCountdown;
    public MinuteHandMovement minuteHandMovement;

    public DrawingProgress drawingProgress;

    private bool gameStarted;

    private void Start()
    {
        objectName = gameObject.name;
        AskedAlready = false;
    }

    private async Task HandleHoverAsync()
    {
        if (!(conversation.talking || conversation.listening) && !AskedAlready && gameStarted && !whisper.askedAlready)
        {
            conversation.talking = true;

            OnObjectHover?.Invoke(gameObject);
            AskedAlready = true;

            whisper.askedAlready = true;
            StartCoroutine(questionCountdown.UpdateTime());
            await Task.Delay(20000);
            await whisper.GenerateImaginativeQuestion(objectName, Whisper.QuestionMode.OBJECT);
        }
    }

    private void Update()
    {
        gameStarted = minuteHandMovement.GetStart();
        if (IsHovered)
        {
            _ = HandleHoverAsync(); // Fire and forget async method
        }
    }
}