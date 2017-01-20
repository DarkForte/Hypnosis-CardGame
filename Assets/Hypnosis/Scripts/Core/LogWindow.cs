using UnityEngine;
using System.Text;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class LogWindow : MonoBehaviour
{
    public Text logText;
    Queue<string> textQueue = new Queue<string>();
    public ScrollRect scrollRect;
    public Scrollbar scrollbar;

    void RefreshText()
    {
        string buffer = "";
        foreach(string text in textQueue)
        {
            buffer += text;
            buffer += '\n';
        }

        logText.text = buffer;
        scrollRect.verticalNormalizedPosition = 0.0f;
        scrollbar.value = 0.0f;
    }

    public void Log(string text, string color = "white", bool refresh = true)
    {
        text = String.Format("<color={0}>{1}</color>", color, text);
        textQueue.Enqueue(text);
        if(refresh)
            RefreshText();
    }

}
