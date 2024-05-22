using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestionCountdown : MonoBehaviour
{
    public TMP_Text countdownTvText;
    private float countdownTime = 15f;
    private bool questionReady { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        countdownTvText.text = $"Question: (In about {countdownTime} secs)";
        questionReady = false ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator UpdateTime ()
    {
        print("HERE");
        while (countdownTime > 0 || questionReady)
        {
            yield return new WaitForSeconds(1);
            countdownTime -= 1;

            countdownTvText.text = $"Question: (In {countdownTime} secs)";
        }
       
        countdownTime = 15f;
    }
    
}
