using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Events;
using UnityEngine;

public class EnableInteractByLookingKeyhole : MonoBehaviour
{
    public Lookable KeyHole;
    public InteractAction InteractAction;

    void OnEnable()
    {
        this.ObserveEvent<Lookable.LookableEvents, Lookable>(Lookable.LookableEvents.OnLookableClicked, OnEvent);
    }

    private void OnEvent(Lookable obj)
    {
        if (obj != KeyHole) return;
        InteractAction.gameObject.SetActive(true);
        this.StopObservingEvent(Lookable.LookableEvents.OnLookableClicked, (Action<Lookable>)OnEvent);
    }

    void OnDisable()
    {
        this.StopObservingEvent(Lookable.LookableEvents.OnLookableClicked, (Action<Lookable>) OnEvent);
    }
}
