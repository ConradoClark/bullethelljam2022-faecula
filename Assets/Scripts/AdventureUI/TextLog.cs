using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Events;
using Licht.Impl.Memory;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class TextLog : MonoBehaviour
{
    public TMP_Text TextComponent;
    public int TailSize;
    public int MaxLines;
    private Caterpillar<string> _logs;
    public Color CurrentTimeColor;
    public AudioSource TextSound;

    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;

    private bool _animatingText;

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

    private void OnDisable()
    {
        this.StopObservingEvent<TextLogEvents, string>(TextLogEvents.OnLogEntry, OnEvent);
    }

    private void OnEvent(string text)
    {
        var tail = TailSize;
        var now = DateTime.Now;
        _logs.Current = $"<color=#{ColorUtility.ToHtmlStringRGBA(CurrentTimeColor)}>.{now:HH:mm}.</color> {text}";

        var messages = _logs.GetTail(tail).Reverse().ToArray();
        //while (TextComponent.GetTextInfo(string.Join("\n", messages)).lineCount > MaxLines)
        //{
        //    tail--;
        //    messages = _logs.GetTail(tail).Reverse().ToArray();
        //    if (tail == 0) break;
        //}

        MachineryRef.Machinery.AddBasicMachine(AnimateText(messages));
        MachineryRef.Machinery.AddBasicMachine(PlayTextLogSound(_logs.Current.Length));
    }

    private IEnumerable<IEnumerable<Action>> AnimateText(IReadOnlyCollection<string> messages)
    {
        _animatingText = true;
        var original = messages.Count > 1 ? messages.Take(messages.Count - 1).ToArray() : Array.Empty<string>();
        var lastMessage = new Queue<char>(messages.Last());

        var msg = "";

        while (lastMessage.Count > 0)
        {
            var ch = lastMessage.Dequeue();
            msg += ch;
            if (ch == '<')
            {
                while (ch != '>' && lastMessage.Count > 0)
                {
                    ch = lastMessage.Dequeue();
                    msg += ch;
                }
            }

            var msgs = original.Concat(new[] { msg })
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            var rem = 0;
            while (TextComponent.GetTextInfo(string.Join("\n", msgs.Skip(rem).Take(msgs.Length))).lineCount > MaxLines)
            {
                rem++;
            }

            TextComponent.text = string.Join("\n", msgs.Skip(rem).Take(msgs.Length));
            Debug.Log(TextComponent.text);

            yield return TimeYields.WaitMilliseconds(TimerRef.Timer, 10);
        }
    }

    private IEnumerable<IEnumerable<Action>> PlayTextLogSound(int textLength)
    {
        for (var i = 0; i < textLength / 7f; i++)
        {
            TextSound.Play();
            TextSound.pitch = 0.85f + Random.value * 0.35f;
            yield return TimeYields.WaitMilliseconds(TimerRef.Timer, 50);
        }
    }
}
