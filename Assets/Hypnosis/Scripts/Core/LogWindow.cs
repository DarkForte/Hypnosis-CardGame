using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class LogWindow : MonoBehaviour
{
    public Text logText;
    Queue<string> textQueue = new Queue<string>();

    void RefreshText()
    {
        string buffer = "";
        foreach(string text in textQueue)
        {
            buffer += text;
            buffer += '\n';
        }

        logText.text = buffer;
    }

    public void Log(string text)
    {
        textQueue.Enqueue(text);
        RefreshText();
    }

}
