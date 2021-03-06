using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Dialogue))]

public class DialogueReader : MonoBehaviour
{
    public Dialogue dialogue;
    public Line lineScript;
    public TextMeshProUGUI textUI;
    public GameObject buttonPrefab;
    public GameObject choicesPanel;
    public bool isTyping = false;
    public GameObject dialogueBox;
    public GameObject athenaTitle;
    public GameObject melaniaTitle;
    public Animator anim;

    void Start()
    {
        dialogue = GetComponent<Dialogue>();
        choicesPanel.SetActive(false);
        textUI.color = Color.black;
        textUI.text = "Athena!";
    }

    void Update()
    {
        if (choicesPanel.activeInHierarchy)
        {
            textUI.text = "";
            StopAllCoroutines();
            return;
        }

        /*if (Input.GetKeyDown(KeyCode.Q))
        {
            isTalking();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            isAngry();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isDismissive();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            isAgreeing();
        }*/

        if (!Input.GetKeyDown(KeyCode.Space))
        {
            return;
        }

        lineScript = dialogue.conversation.Progress();

        if (lineScript == null)
        {
            EndDialogue();
            StopAllCoroutines();
            textUI.text = "";
            return;
        }

        if (lineScript.characterID == 1)
        {
            melaniaTitle.SetActive(false);
            athenaTitle.SetActive(true);
            dialogueBox.SetActive(true);
            //textUI.color = Color.white;
        }
        else
        {
            dialogueBox.SetActive(true);
            melaniaTitle.SetActive(true);
            athenaTitle.SetActive(false);
        }

        if (lineScript.choices.Length > 0)
        {
            textUI.text = "";
            melaniaTitle.SetActive(true);

            for (int i = 0; i < lineScript.choices.Length; i++)
            {
                Choice tempChoice = lineScript.choices[i];
                GameObject go = Instantiate(buttonPrefab, choicesPanel.transform);
                go.GetComponentInChildren<TextMeshProUGUI>().text = tempChoice.dialogue;
                go.GetComponent<Button>().onClick.AddListener(() => SelectChoice(tempChoice.targetSegment));
                if (!string.IsNullOrEmpty(tempChoice.callBack))
                {
                    go.GetComponent<Button>().onClick.AddListener(() => SelectCallBack(tempChoice.callBack));
                }
            }

            choicesPanel.SetActive(true);

            return;
        }

        //textUI.text = line.dialogue;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(lineScript.dialogue));

    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        textUI.text = "";
        if (lineScript.callBack != "")
        {
            SelectCallBack(lineScript.callBack);
        }
        foreach (char letter in sentence.ToCharArray())
        {
            textUI.text += letter;
            yield return new WaitForSeconds(0.03f);
        }
        isTyping = false;
    }

    void EndDialogue()
    {
        textUI.text = "";
        choicesPanel.SetActive(false);
        dialogueBox.SetActive(false);
        athenaTitle.SetActive(false);
        melaniaTitle.SetActive(false);
        print("QuitGame");
        Application.Quit();
    }

    public void SelectChoice(string choiceSelected)
    {
        Line tempLine = dialogue.conversation.ProgressViaChoice(choiceSelected);

        foreach (Transform children in choicesPanel.transform)
        {
            Destroy(children.gameObject);
        }

        choicesPanel.SetActive(false);

        textUI.text = tempLine.dialogue;
    }

    public void SelectCallBack(string callBack)
    {
        Invoke(callBack, 0);
    }

    public void TestFunction()
    {
        print("success");
    }

    public void isTalking()
    {
        anim.SetBool("isTalking", true);
        anim.SetBool("isAngry", false);
        anim.SetBool("isDismissive", false);
        anim.SetBool("isAgreeing", false);
    }

    public void isAngry()
    {
        anim.SetBool("isTalking", false);
        anim.SetBool("isAngry", true);
        anim.SetBool("isDismissive", false);
        anim.SetBool("isAgreeing", false);
    }

    public void isDismissive()
    {
        anim.SetBool("isTalking", false);
        anim.SetBool("isAngry", false);
        anim.SetBool("isDismissive", true);
        anim.SetBool("isAgreeing", false);
    }

    public void isAgreeing()
    {
        anim.SetBool("isTalking", false);
        anim.SetBool("isAngry", false);
        anim.SetBool("isDismissive", false);
        anim.SetBool("isAgreeing", true);
    }
}
