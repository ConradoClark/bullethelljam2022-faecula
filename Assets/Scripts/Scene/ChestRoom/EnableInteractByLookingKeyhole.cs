using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using UnityEngine;

public class EnableInteractByLookingKeyhole : MonoBehaviour
{
    public Interactive KeyHole;
    public InteractiveAction InteractAction;

    void OnEnable()
    {
        this.ObserveEvent<Interactive.InteractiveEvents, Interactive>(Interactive.InteractiveEvents.OnInteractiveClicked, OnEvent);
    }

    private void OnEvent(Interactive obj)
    {
        if (obj != KeyHole) return;
        InteractAction.gameObject.SetActive(true);
        this.StopObservingEvent(Interactive.InteractiveEvents.OnInteractiveClicked, (Action<Interactive>)OnEvent);
    }

    void OnDisable()
    {
        this.StopObservingEvent(Interactive.InteractiveEvents.OnInteractiveClicked, (Action<Interactive>) OnEvent);
    }
}
