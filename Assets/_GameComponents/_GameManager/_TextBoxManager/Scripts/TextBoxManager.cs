using UnityEngine;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour
{
    private GameObject textBox;
    private GameObject textBoxBackground;
    private GameObject textBoxAuthor;
    private GameObject textBoxContent;
    private AudioSource audioSource;
    private TextImporter textImporter;
    private bool isActive = false;
    private bool isPaused = false;

    private Conversation currentConversation;

    internal bool hasConversationEnded()
    {
        return currentConversation.hasEnded();
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        textImporter = GetComponent<TextImporter>();
        textBox = GameObject.FindGameObjectWithTag("TextBox");
        if (textBox != null)
        {

            Transform[] children = textBox.GetComponentsInChildren<Transform>(true);

            foreach (Transform tr in children) 
            {
                switch (tr.gameObject.tag)
                {
                    case "TextBoxBackground":
                        textBoxBackground = tr.gameObject;
                        break;
                    case "TextBoxAuthor":
                        textBoxAuthor = tr.gameObject;
                        break;
                    case "TextBoxContent":
                        textBoxContent = tr.gameObject;
                        break;
                    default:
                        break;
                }
            }
        } else
        {
            Debug.LogWarning("Configure a Text Box!");
        }
    }

    private void Update()
    {
        if (isActive && currentConversation != null && GameManager._Input.confirm)
        {
            currentConversation.currentSnippet++;
            if (currentConversation.hasEnded())
            {
                DisableTextBox();
            }
            else
            {
                DisplayConversation();
            }
        }
    }

    private void DisplayConversation()
    {
        if (audioSource) audioSource.Stop();

        textBoxContent.GetComponent<Text>().text = currentConversation.getCurrentSnippet().text;
        Debug.Log(textBoxContent.GetComponent<Text>().text);

        if (currentConversation.getAuthor().Length == 0)
        {
            textBoxAuthor.gameObject.SetActive(false);
        }
        else
        {
            textBoxAuthor.GetComponent<Text>().text = currentConversation.getAuthor();
        }

        if (currentConversation.getCurrentSnippet() != null)
        {
            AudioClip vocals = currentConversation.getCurrentSnippet().vocals;
            if (audioSource && vocals != null)
            {
                audioSource.PlayOneShot(vocals, 1F);
            }
        }

    }

    public void EnableTextBox(string conversationId)
    {
        Conversation conversation = textImporter.textFileToConversation(conversationId);
        isActive = true;
        // textBox.GetComponent<Image>().CrossFadeAlpha(100F, 0.1f, false);
        foreach (Transform go in textBox.GetComponentsInChildren<Transform>(true))
        {
            go.gameObject.SetActive(true);
        }
        currentConversation = conversation;
        DisplayConversation();
        textBoxBackground.SetActive(true);
        textBoxContent.SetActive(true);
        textBoxAuthor.SetActive(true);
    }

    private void DisableTextBox()
    {
        textBox.GetComponent<Image>().CrossFadeAlpha(0F, 0.1f, false);
        isActive = false;
        textBoxBackground.SetActive(false);
        textBoxContent.SetActive(false);
        textBoxAuthor.SetActive(false);
    }

    public bool isPlayingSound()
    {
        return audioSource != null && audioSource.isPlaying;
    }
}
