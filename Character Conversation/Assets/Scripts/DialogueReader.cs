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

    void Start()
    {
        dialogue = GetComponent<Dialogue>();
        choicesPanel.SetActive(false);
        textUI.text = "";
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
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
            }

            choicesPanel.SetActive(true);

            return;
        }

        textUI.text = line.dialogue;

    }

    void EndDialogue()
    {
        textUI.text = "";
        choicesPanel.SetActive(false);
        dialogue.ResetDialog();
    }
}
