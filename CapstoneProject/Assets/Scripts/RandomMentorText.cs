using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomMentorText : MonoBehaviour
{
    [TextArea]
    public List<string> mentorTextList;
    public Text mentorText;

    int indexCount = 0;

    public void ChooseRandomText()
    {
        mentorText.text = mentorTextList[indexCount];

        indexCount++;
        if (indexCount >= mentorTextList.Count)
        {
            indexCount = 0;
        }
    }
}
