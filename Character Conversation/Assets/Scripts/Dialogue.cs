using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public TextAsset textAsset; // the .txt file to parse in
    [HideInInspector] public Conversation conversation; // our fully parsed conversation to use

    void Start()
    {
        ReadDialogue(); // can do this before play even to save memory.
    }

    void ReadDialogue() // here we parse our text asset into a single string
    {
        string assetText = textAsset.text; // using text asset this is easy, but any other document type has a lot more work to do here.
        conversation = new Conversation(assetText); // create our new conversation from our string.
    }

    public void ResetDialog()
    {
        // can have a different one to start from a certain section?
        // for now we will simply parse the dialogue script again to auto set everything. 
        // this isn't good for performance memory sake. 
        ReadDialogue();
    }
}

// in this script every part takes care to process itself. Each section is only interested in how it needs to function.
public class Conversation
{
    // we store all our segments in a dictionary so we can find them via a string name, which we call segmentID
    private Dictionary<string, Segment> segments; // all the segments in the currect conversation
    public string currentSegment; // whats the current section we're in

    public Conversation(string conversationInformation)
    {
        // here we're spliting up all the segments, we take one string, our full convo, and split it when there's a double line break and that gets returned as an array.
        string[] assetSegments = conversationInformation.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.RemoveEmptyEntries);

        segments = new Dictionary<string, Segment>(); // empty dictionary to fill
        
        foreach (string s in assetSegments) // for each segment
        {
            Segment tempSegment = new Segment(s); // create a new segment
            segments.Add(tempSegment.sectionID, tempSegment); // and add it to our dictionary/
        }      

        currentSegment = segments.Keys.First(); // set our first segment as the start point.
    }

    public Line Progress() // this get's called to move our conversation forward.
    {
        if (currentSegment == "END") // check if we're at the end or not! 
        {
            Debug.Log("end of everything?");
            return null; // exit out of here so we don't go looking at nothing.
        }

        // if we're at the end of a segment, go to the connecting segment
        if (segments[currentSegment].lineID + 1 >= segments[currentSegment].lines.Length)
        {
            currentSegment = segments[currentSegment].nextSectionID;
            segments[currentSegment].lineID = -1; // always enter at the start of the segment
        }

        // return the next line in the current segment. 
        return segments[currentSegment].NextLine();
    }

    public Line ProgressViaChoice(string choiceTarget) // this get's called when we need to make a choice
    {
        currentSegment = choiceTarget; // set the next section manually. 
        segments[currentSegment].lineID = -1; // always enter at the start of the segment
        return Progress();// progress normally.
    }
}

public class Segment
{
    public string sectionID; // what is the name of this currect section
    public string nextSectionID; // Next section to progress to if we don't progress via a choice
    public Line[] lines; // all the dialogue lines in this section, in order
    public int lineID; // which line are we currently at, set to -1 on start so we progress into 0 first.
    public Segment(string segmentInformation) 
    {
        string[] assetLines = segmentInformation.Split('\n'); // break our segment string up by line spaces.

        if(assetLines[0].Contains(':')) // if we have a : we have a next section ID to grab.
        {
            string[] sections = assetLines[0].Split(':'); // split the string at :
            sectionID = sections[0]; // first element is this section
            nextSectionID = sections[1]; // second element is the connecting one.
        }
        else
        {
            sectionID = assetLines[0]; // otherwise the string itself is our section ID
            nextSectionID = "EMPTY"; // set our next section to EMPTY so we know.
        }

        // these two remove any line spaces that happen to be in the string. There are other more advanced ways of doing this, but it's comlicated.
        sectionID = sectionID.Replace("\r", "");
        nextSectionID = nextSectionID.Replace("\r", "");

        List<Line> tempLines = new List<Line>(); // empty list to fill with our dialogue lines

        for (int i = 1; i < assetLines.Length; i++)
        {
            tempLines.Add(new Line(assetLines[i])); // for each remaining line after our section information, create a new line for it
        }

        lines = tempLines.ToArray(); // set our list as an array
        lineID = -1; // set our start Line ID
    }

    // this is called to move to the next line
    public Line NextLine ()
    {
        lineID++; // progress our counter
        return lines[lineID]; // return the current line
    }
}

public class Line
{
    // who's speaking and the dialogue if there is no choices.
    public int characterID;
    public string dialogue;
    // if we have choices they go here.
    public Choice[] choices;
    public string callBack;

    public Line(string lineInformation) // take the line information and split it into dialogue or choices.
    {
        string[] assetLine = lineInformation.Split(']'); // split the line at the speaker marker }.
        characterID = int.Parse(assetLine[0]); // first element is the ID

        dialogue = assetLine[1]; // second element is the actual dialogue
        dialogue.Remove(0, 1); // remove the space at the begining of the line

        if (assetLine[1].Contains('(') && assetLine[1].Contains(')')) // if we have ( and ) split into choices.
        {
            List<Choice> tempChoices = new List<Choice>(); // empty list to fill
            string[] assetChoices = assetLine[1].Split(new char[] { '(', ')' }); // split up out choices string at ( and ) points

            for (int i = 1; i < assetChoices.Length; i++)
            {
                tempChoices.Add(new Choice(assetChoices[i], assetChoices[i + 1])); // we need both the dialogue and choice segment target here
                i++; // progress the for loop once more as we want to move up by odd numbers,  1, 3, 5, etc
            }

            choices = tempChoices.ToArray(); // set our choices
        }
        else
        {
            choices = new Choice[0]; // set emtpy choices if nothing there.
        }

        if (assetLine[1].Contains(":"))
        {
            string[] tempTarget = assetLine[1].Split(':');
            dialogue = tempTarget[0].Replace("\r", "");
            dialogue.Remove(0, 1);
            callBack = tempTarget[1].Replace("\r", "");
        }
        else
        {
            callBack = "";
        }
    }
}

public class Choice
{
    // the dialogue and target segment for the specific choice.
    public string dialogue;
    public string targetSegment;
    public string callBack; // added a callback method to be invoked on choice.

    public Choice (string _target, string _dialogue)
    {
        if (_target.Contains(":"))
        {
            string[] tempTarget = _target.Split(':');
            targetSegment =tempTarget[0].Replace("\r", "");
            callBack =tempTarget[1].Replace("\r", "");
        }
        else
        {
            targetSegment = _target;
        }
        dialogue = _dialogue;
    }
}