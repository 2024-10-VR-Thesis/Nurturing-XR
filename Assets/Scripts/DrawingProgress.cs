using UnityEngine;

namespace Scripts.DrawingProgress
{
    public class DrawingProgress : MonoBehaviour
    {

        public GameObject[] drawings;
        public int index;
        public Conversation.Conversation conversation;

        void Start()
        {
            DisableObjects();
            index = -1;
        }

        void Update()
        {
            if (index != -1 && index < drawings.Length)
            {
                drawings[index].SetActive(true);
            }
            if (index == 3) { 
                conversation.playing = false; 
            }
        }

        void DisableObjects()
        {
            foreach (GameObject obj in drawings)
            {
                obj.SetActive(false);
            }
        }

        public void increaseIndex()
        {
            index += 1;
        }

        public int GetDrawnObjects()
        {
            return index+1;
        }
    }
}
