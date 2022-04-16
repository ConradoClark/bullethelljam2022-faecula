using System;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Memory;
using TMPro;
using UnityEngine;

public class TextLog : MonoBehaviour
{
    public TMP_Text TextComponent;
    public int TailSize;
    public int MaxLines;
    private Caterpillar<string> _logs;
    public Color CurrentTimeColor;

    public enum TextLogEvents
    {
        OnLogEntry
    }

    private void OnEnable()
    {
        TextComponent.text = "";
        _logs = new Caterpillar<string>
        {
            TailSize = TailSize
        };
        this.ObserveEvent<TextLogEvents, string>(TextLogEvents.OnLogEntry, OnEvent);
    }

    private void OnEvent(string text)
    {
        var tail = TailSize;
        var now = DateTime.Now;
        _logs.Current = $"<color=#{ColorUtility.ToHtmlStringRGBA(CurrentTimeColor)}>.{now:HH:mm}.</color> {text}";

        var messages = _logs.GetTail(tail).Reverse().ToArray();
        while (TextComponent.GetTextInfo(string.Join("\n", messages)).lineCount > MaxLines)
        {
            tail--;
            messages = _logs.GetTail(tail).Reverse().ToArray();
            if (tail == 0) break;
        }

        TextComponent.text = string.Join("\n", messages);
    }
}
