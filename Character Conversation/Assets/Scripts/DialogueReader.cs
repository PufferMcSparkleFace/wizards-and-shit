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

    void Start()
    {
        dialogue = GetComponent<Dialogue>();
        choicesPanel.SetActive(false);
        textUI.text = "";
    }

    void Update()
    {   
        if (choicesPanel.activeInHierarchy)
        {
            textUI.text = "";
        }

        if (!Input.GetKeyDown(KeyCode.Space) || choicesPanel.activeInHierarchy)
        {
            return;
        }

        lineScript = dialogue.conversation.Progress();

        if (lineScript.characterID == 0)
        {
            textUI.color = Color.black;
        }
        if (lineScript.characterID == 1)
        {
            textUI.color = Color.white;
        }

        if (lineScript == null)
        {
            EndDialogue();
            return;
        }

        if (lineScript.choices.Length > 0)
        {
            textUI.text = "";

            for (int i = 0; i < lineScript.choices.Length; i++)
            {
                Choice tempChoice = lineScript.choices[i];
                GameObject go = Instantiate(buttonPrefab, choicesPanel.transform);
                go.GetComponentInChildren<TextMeshProUGUI>().text = tempChoice.dialogue;
                go.GetComponent<Button>().onClick.AddListener(()=> SelectChoice(tempChoice.targetSegment));
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

    IEnumerator TypeSentence (string sentence)
    {
        textUI.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            textUI.text += letter;
            yield return null;
        }
    }

    void EndDialogue()
    {
        textUI.text = "";
        choicesPanel.SetActive(false);
        dialogue.ResetDialog();
    }

    public void SelectChoice(string choiceSelected)
    {
        Line tempLine = dialogue.conversation.ProgressViaChoice(choiceSelected);

        foreach(Transform children in choicesPanel.transform)
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
}
