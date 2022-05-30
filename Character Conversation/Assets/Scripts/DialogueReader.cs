using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Dialogue))]

public class DialogueReader : MonoBehaviour
{
    public Dialogue dialogue;
    public TextMeshProUGUI textUI;
    public GameObject buttonPrefab;
    public GameObject choicesPanel;
    public GameObject athenaTitle;
    public GameObject melaniaTitle;

    void Start()
    {
        dialogue = GetComponent<Dialogue>();
        choicesPanel.SetActive(false);
        textUI.text = "";
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || choicesPanel.activeInHierarchy)
        {
            return;
        }

        Line line = dialogue.conversation.Progress();

        if (line == null)
        {
            EndDialogue();
            return;
        }

        if (line.choices.Length > 0)
        {
            textUI.text = "";

            for (int i = 0; i < line.choices.Length; i++)
            {
                Choice tempChoice = line.choices[i];
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
        StartCoroutine(TypeSentence(line.dialogue));

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
