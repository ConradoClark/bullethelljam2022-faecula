using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Builders;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class FaeSlowdown : MonoBehaviour
{
    private PlayerInput _input;

    public BasicMachineryScriptable MachineryRef;
    public FaeStats Stats;
    public TimerScriptable TimerRef;
    public TimerScriptable SlowdownTimerRef;

    public AudioSource SlowDownAudio;
    public AudioSource SpeedUpAudio;

    public float Delay;
    public int Cost;

    public AudioMixerGroup AudioMixerGroup;

    void OnEnable()
    {
        _input = _input != null ? _input : PlayerInput.GetPlayerByIndex(0);
        MachineryRef.Machinery.AddBasicMachine(HandleSlowdown());
    }

    IEnumerable<IEnumerable<Action>> HandleSlowdown()
    {
        while (isActiveAndEnabled)
        {
            if (!_input.actions[Constants.Actions.Click].triggered || Stats.Magic <= Cost)
            {
                yield return TimeYields.WaitOneFrameX;
                continue;
            }

            AudioMixerGroup.audioMixer.SetFloat("Lowpass", 4232);
            AudioMixerGroup.audioMixer.SetFloat("Highpass", 5838);
            SlowDownAudio.Play();
            yield return new LerpBuilder(value => SlowdownTimerRef.Timer.Multiplier = value, () => (float)SlowdownTimerRef.Timer.Multiplier)
                .Decrease(0.75f)
                .Over(0.15f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                .UsingTimer(TimerRef.Timer)
                .Build();

            do
            {
                Stats.ConsumeMagic(Cost);
                yield return TimeYields.WaitSeconds(TimerRef.Timer, Delay);
            } while (_input.actions[Constants.Actions.Click].IsPressed() && Stats.Magic > Cost);

            SpeedUpAudio.Play();

            AudioMixerGroup.audioMixer.SetFloat("Lowpass", 22000);
            AudioMixerGroup.audioMixer.SetFloat("Highpass", 0);

            yield return new LerpBuilder(value => SlowdownTimerRef.Timer.Multiplier = value, () => (float)SlowdownTimerRef.Timer.Multiplier)
                .Increase(0.75f)
                .Over(0.5f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseIn)
                .UsingTimer(TimerRef.Timer)
                .Build();
        }
    }
}
