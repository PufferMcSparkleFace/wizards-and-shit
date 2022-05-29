using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

public class ShowDialogue : MonoBehaviour
{
    public TextAsset textAsset;

    public ShowConversation conversation;

    void Start()
    {
        string assetText = textAsset.text;
        conversation = new ShowConversation(assetText);
    }
}

[System.Serializable]
public struct ShowConversation
{
    public ShowSegment[] segments;
    public string currentSegment;

    public ShowConversation(string conversationInformation)
    {
        string[] assetSegments = conversationInformation.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        List<ShowSegment> tempSegments = new List<ShowSegment>();

        foreach(string s in assetSegments)
        {
            tempSegments.Add(new ShowSegment(s)); // ADD s here
        }

        segments = tempSegments.ToArray();
        currentSegment = segments[0].sectionID;
    }
}

[System.Serializable]
public struct ShowSegment
{
    public string sectionID;
    public string nextSectionID;
    public ShowLine[] lines;

    public ShowSegment(string segmentInformation)
    {
        string[] assetLines = segmentInformation.Split('\n');

        //seperating segment name and next section
        if (assetLines[0].Contains(':'))
        {
            string[] sections = assetLines[0].Split(':');
            sectionID = sections[0];
            nextSectionID = sections[1];

        }
        else
        {
            sectionID = assetLines[0];
            nextSectionID = "EMPTY";
        }

        // generate dialogue lines in segment
        List<ShowLine> tempLine = new List<ShowLine>();

        for (int i = 1; i < assetLines.Length; i++)
        {
            tempLine.Add(new ShowLine(assetLines[i]));
        }

        lines = tempLine.ToArray();
    }
}

[System.Serializable]
public struct ShowLine
{
    public int characterID;
    public string dialogue;
    public ShowChoice[] choices;

    public ShowLine(string lineInformation)
    {
        string[] assetLines = lineInformation.Split(']');
        characterID = int.Parse(assetLines[0]);

        dialogue = assetLines[1];
        dialogue.Remove(0, 1);

        //save our choices.
        if (assetLines[1].Contains('(') && assetLines[1].Contains(')'))
        {
            List<ShowChoice> tempChoices = new List<ShowChoice>();
            string[] assetChoices = assetLines[1].Split(new char[] { '(', ')' });

            for (int i = 1; i < assetChoices.Length; i++)
            {
                tempChoices.Add(new ShowChoice(assetChoices[i], assetChoices[i + 1]));
                i++;
            }

            choices = tempChoices.ToArray();
        }
        else
        {
            choices = new ShowChoice[0];
        }
    }
}

[System.Serializable]
public struct ShowChoice
{
    public string dialogue;
    public string targetSegment;
    public string callBack;

    public ShowChoice (string _target, string _dialogue)
    {
        if (_target.Contains(":"))
        {
            string[] tempTarget = _target.Split(':');
            targetSegment = tempTarget[0].Replace("\r", "");
            callBack = tempTarget[1].Replace("\r", "");
        }
        else
        {
            targetSegment = _target;
            callBack = "";
        }
        dialogue = _dialogue;
    }
}