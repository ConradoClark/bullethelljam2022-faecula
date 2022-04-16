using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using Licht.Impl.Orchestration;
using Licht.Unity.Accessors;
using Licht.Unity.Objects;
using TMPro;
using UnityEngine;

public class HelpText : MonoBehaviour
{
    public BasicMachineryScriptable MachineryRef;
    public TimerScriptable TimerRef;
    public TMP_Text TextComponent;
    public enum HelpTextEvents
    {
        TextChanged,
        TextClear,
        TextLock,
        TextUnlock
    }

    public class TextChangedEvent : TextEvent
    {
        public string Text;
    }

    public class TextEvent
    {
        public object Source;
    }

    private object _currentSource;
    private bool _animating;
    private bool _breakAnimation;
    private string _currentText;

    private bool _lockedText;

    void OnEnable()
    {
        this.ObserveEvent<HelpTextEvents, TextChangedEvent>(HelpTextEvents.TextChanged, OnTextChanged);
        this.ObserveEvent<HelpTextEvents, TextEvent>(HelpTextEvents.TextClear, OnTextClear);
        this.ObserveEvent<HelpTextEvents, TextChangedEvent>(HelpTextEvents.TextLock, OnTextLock);
        this.ObserveEvent<HelpTextEvents, TextEvent>(HelpTextEvents.TextUnlock, OnTextUnlock);
        _currentText = TextComponent.text = "";

        MachineryRef.Machinery.AddBasicMachine(Fade());
    }

    private void OnTextChanged(TextChangedEvent @event)
    {
        if (_lockedText) return;
        if (_currentSource != null && _currentSource != @event.Source) return;
        if (_currentText == @event.Text) return;

        _currentText = @event.Text;
        _currentSource = @event.Source;
        if (_animating) _breakAnimation = true;
    }

    private void OnTextClear(TextEvent @event)
    {
        if (_lockedText) return;
        if (_currentSource != null && _currentSource != @event.Source) return;
        if (_currentText == string.Empty) return;

        _currentText = string.Empty;
        _currentSource = null;
    }

    private void OnTextLock(TextChangedEvent @event)
    {
        if (_lockedText) return;
        _lockedText = true;
        _currentText = @event.Text;
        _currentSource = @event.Source;
        if (_animating) _breakAnimation = true;
    }

    private void OnTextUnlock(TextEvent @event)
    {
        if (_currentSource != null && _currentSource != @event.Source) return;
        _lockedText = false;
        _currentText = string.Empty;
        _currentSource = null;
    }

    IEnumerable<IEnumerable<Action>> Fade()
    {
        while (isActiveAndEnabled)
        {
            if (!string.IsNullOrWhiteSpace(_currentText) &&
                _currentText != TextComponent.text && Math.Abs(TextComponent.color.a - 1f) < 0.01f)
            {
                TextComponent.text = _currentText;
            }

            if (string.IsNullOrWhiteSpace(_currentText) && TextComponent.color.a > 0f)
            {
                _animating = true;
                yield return new ColorAccessor(c => TextComponent.color = c, () => TextComponent.color)
                    .A
                    .SetTarget(0)
                    .Over(0.3f)
                    .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                    .UsingTimer(TimerRef.Timer)
                    .BreakIf(() => _breakAnimation)
                    .Build();
                TextComponent.text = _currentText;
                _breakAnimation = false;

            }
            else if (!string.IsNullOrWhiteSpace(_currentText) && TextComponent.color.a <= 0f)
            {
                _animating = true;
                TextComponent.text = _currentText;
                yield return new ColorAccessor(c => TextComponent.color = c, () => TextComponent.color)
                    .A
                    .SetTarget(1)
                    .Over(0.3f)
                    .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                    .UsingTimer(TimerRef.Timer)
                    .BreakIf(() => _breakAnimation)
                    .Build();
                _breakAnimation = false;
            }
            else
            {
                _animating = false;
                yield return TimeYields.WaitOneFrameX;
            }
            _animating = false;
        }
    }
}
