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
        
    }
}
