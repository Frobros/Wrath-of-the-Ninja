using UnityEngine;

public class DisplayTextBox : MonoBehaviour
{
    [SerializeField] private string conversationId;
    private float timeOffset = 0f;

    private void Start()
    {
        timeOffset = Time.time;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        bool canTrigger = (timeOffset + 1f) < Time.time;
        if (InputManager.jump)
        {
            Debug.Log("Offset: " + timeOffset);
            Debug.Log("Time: " + Time.time);
        }

        if (collision.tag == "Player" && InputManager.jump && canTrigger)
        {
            Time.timeScale = 0F;
            timeOffset = Time.time;
            Conversation conversation = TextImporter.textFileToConversation(conversationId);
            TextBoxManager.EnableTextBox(conversation);
        }
    }
}
