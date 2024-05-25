using Scripts.Conversation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Samples.Whisper;

public class Endgamecanvas : MonoBehaviour
{
    public Conversation conversation;
    public Whisper whisper;
    public TMP_Text scoreTvText;
   
    // Start is called before the first frame update
    public Behaviour endgameCanvas;
    void Start()
    {
        endgameCanvas.enabled = false;
        scoreTvText.text = "Hola Mundo";
    }

    // Update is called once per frame
    void Update()
    {
        if (!conversation.playing)
        {
            int calificacion = 0;
            endgameCanvas.enabled = true;
            Debug.Log(whisper.scores.Count);
            for(int i = 0; i < whisper.scores.Count; i++)
            {
                calificacion += whisper.scores[i];
            }
            double promedio = calificacion/ whisper.scores.Count;
            scoreTvText.text = "The End \n  \n This was your conversation score: "+promedio;
        }
    }
}
