using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomMentorText : MonoBehaviour
{
    [TextArea(5, 100)]
    public List<string> mentorTextList;
    public Text mentorText;
    const string mentorKey = "mentor";
    bool seenAllText = false;
    public GameObject speechBubble;

    int indexCount = 0;

    private void Start()
    {
        seenAllText = IntToBool(PlayerPrefs.GetInt(mentorKey, 0));
        if (seenAllText)
        {
            speechBubble.SetActive(false);
        }
    }

    public void ChooseRandomText()
    {
        mentorText.text = mentorTextList[indexCount];

        indexCount++;
        if (indexCount >= mentorTextList.Count)
        {
            seenAllText = true;
            PlayerPrefs.SetInt(mentorKey, BoolToInt(seenAllText));
            PlayerPrefs.Save();
            speechBubble.SetActive(false);
            indexCount = 0;
        }
    }

    int BoolToInt(bool key)
    {
        if (key)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    bool IntToBool(int key)
    {
        if (key != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
