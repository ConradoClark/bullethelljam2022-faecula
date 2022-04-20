using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggeredMusic : MonoBehaviour
{
    public ConditionalTrigger[] Triggers;
    public AudioSource Audio;

    void OnEnable()
    {
        if (Triggers != null && Triggers.Length > 0 && Triggers.Any(t => t.Trigger.Value == t.Negate))
        {
            return;
        }

        Audio.Play();
    }
}
