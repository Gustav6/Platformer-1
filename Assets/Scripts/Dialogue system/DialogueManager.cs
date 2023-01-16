using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private const float V = 0f;
    public Image actorImage;
    public Text actorName;
    public Text messageText;
    public RectTransform backroundBox;
    public Rigidbody2D rb;

    Message[] currentMessages;
    Actor[] currentActors;
    int activeMessage = 0;
    public static bool isActive = false;

    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        currentActors = actors;
        currentMessages = messages;
        activeMessage = 0;
        isActive = true;

        Debug.Log("Started Conversation! Loaded Messages: " + messages.Length);
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        DispayMessage();
        backroundBox.LeanScale(Vector3.one, 0.5f);
    }


    void DispayMessage()
    {
        Message messageToDisplay= currentMessages[activeMessage];
        messageText.text = messageToDisplay.message;

        Actor actorToDisplay= currentActors[messageToDisplay.actorId];
        actorName.text = actorToDisplay.name;
        actorImage.sprite = actorToDisplay.sprite;
    }

    public void NextMessage()
    {
        activeMessage++;
        if (activeMessage < currentMessages.Length)
        {
            DispayMessage();
        }
        else
        {
            Debug.Log("Conversation ended");
            rb.constraints = RigidbodyConstraints2D.None;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            backroundBox.LeanScale(Vector3.zero, 0.5f).setEaseInOutExpo();
            isActive = false;
        }
    }


    void Start()
    {
        backroundBox.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && isActive == true) 
        {
            NextMessage();
        }
    }
}
