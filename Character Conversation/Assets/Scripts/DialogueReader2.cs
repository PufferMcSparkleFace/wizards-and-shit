using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Dialogue))]

public class DialogueReader2 : MonoBehaviour
{
    public Dialogue dialogue;
    public Line lineScript;
    public TextMeshProUGUI textUI;
    public GameObject buttonPrefab;
    public GameObject choicesPanel;
    public LevelChanger levelChanger;
    public bool isTyping = false;
    public string currentSentence;

    void Start()
    {
        dialogue = GetComponent<Dialogue>();
        choicesPanel.SetActive(false);
        textUI.color = Color.white;
        textUI.text = "The game is set towards the end of Greece's dominance, as the Romans are beginning to invade.";
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

        if (lineScript == null)
        {
            EndDialogue();
            return;
        }

        if (isTyping)
        {
            StopAllCoroutines();
            textUI.text = currentSentence;
            isTyping = false;
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
        currentSentence = lineScript.dialogue;
        StartCoroutine(TypeSentence(currentSentence));

    }

    IEnumerator TypeSentence (string sentence)
    {
        isTyping = true;
        textUI.text = "";
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
        levelChanger.FadeToLevel(1);
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
}
