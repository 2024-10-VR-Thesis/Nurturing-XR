using UnityEngine;
using UnityEngine.Events;
using Samples.Whisper;
using Scripts.Conversation;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class EyeInteractable : MonoBehaviour
{
    public bool IsHovered { get; set; }

    public UnityEvent<GameObject> OnObjectHover;

    [SerializeField] Whisper whisper;
    [SerializeField] Conversation conversation;

    private async Task HandleHoverAsync()
    {
        if (!(conversation.talking || conversation.listening) && whisper.scores.Last() > 7)
        {
            lock (whisper.scores)
            {
                if (whisper.scores.Any())
                {
                    whisper.scores.RemoveAt(0);
                }
            }
            Debug.Log("JOIN THE OBJECT TO WHISPER");
            OnObjectHover?.Invoke(gameObject);
            string name = gameObject.name;
            Debug.Log(name + "----------------------------------");
            conversation.talking = true;
            await whisper.GenerateImaginativeQuestion(name, Whisper.QuestionMode.OBJECT);
        }
    }

    private void Update()
    {
        if (IsHovered)
        {
            _ = HandleHoverAsync(); // Fire and forget async method
        }
    }
}