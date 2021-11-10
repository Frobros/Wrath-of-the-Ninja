using UnityEngine;

public class DisplayTextBox : MonoBehaviour
{
    [SerializeField] private string conversationId;
    private InputManager input;
    private TextBoxManager textBoxManager;
    private bool canStartConversation = false;
    private bool isInConversation = false;

    private void Start()
    {
        input = GameManager._Input;
        textBoxManager = GameManager._TextBoxManager;
    }

    private void Update()
    {
        if (input.jump && canStartConversation && !isInConversation)
        {
            GameManager.Pause();
            isInConversation = true;
            textBoxManager.EnableTextBox(conversationId);
        }
        else if (isInConversation && textBoxManager.hasConversationEnded())
        {
            GameManager.Pause(false);
            isInConversation = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canStartConversation = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            canStartConversation = false;
        }
    }
}
