using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Unity.InputModule;
using UniRx;
using UnityEngine;
using DateTime = System.DateTime;

public class HandGesture : MonoBehaviour,IHoldHandler,IFocusable,INavigationHandler {

    private Subject<Unit> onAirTap = new Subject<Unit>();

    public Subject<Unit> OnAirTap {
        get { return onAirTap; }
    }
    
    private Subject<Unit> onAirDoubleTap = new Subject<Unit>();

    public Subject<Unit> OnAirDoubleTap {
        get { return onAirDoubleTap; }
    }
    
    
    private DateTime starttime;
    void OnEnable() {
        var eventTrigger = gameObject.AddComponent<ObservableHandGestureTrigger>();

        eventTrigger.OnInputDownObservable()
            .TakeUntilDisable(this)
  //          .Where(eventData => eventData.selectedObject == gameObject)
            .Subscribe(_=> starttime = DateTime.Now);

        var tap = eventTrigger.OnInputUpObservable()
            .TakeUntilDisable(this)
  //          .Where(eventData => eventData.selectedObject == gameObject)
            .Where(_ => (DateTime.Now - starttime).TotalSeconds < 1)
            .Buffer(TimeSpan.FromMilliseconds(800)).Share();
        
        tap.Where(l => l.Count == 1).Subscribe(_ => onAirTap.OnNext(Unit.Default));
        tap.Where(l => l.Count == 2).Subscribe(_ => onAirDoubleTap.OnNext(Unit.Default));
    }

    public void OnHoldStarted(HoldEventData eventData) {
        Debug.Log(eventData.selectedObject);
    }

    public void OnHoldCompleted(HoldEventData eventData) {
        Debug.Log(eventData.selectedObject);
    }

    public void OnHoldCanceled(HoldEventData eventData) {
        Debug.Log(eventData.selectedObject);
    }

    public void OnFocusEnter() {
        gameObject.GetComponent<Renderer>().material.color = Color.red;
    }

    public void OnFocusExit() {
        gameObject.GetComponent<Renderer>().material.color = Color.white;
    }

    public void OnNavigationStarted(NavigationEventData eventData) {
        InputManager.Instance.PushModalInputHandler(eventData.selectedObject);
    }

    public void OnNavigationUpdated(NavigationEventData eventData) {    
        Debug.LogFormat("OnNavigationUpdated\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
        eventData.InputSource,
        eventData.SourceId,
        eventData.NormalizedOffset.x,
        eventData.NormalizedOffset.y,
        eventData.NormalizedOffset.z);
    }

    public void OnNavigationCompleted(NavigationEventData eventData) {
        Debug.Log(eventData.selectedObject);
    }

    public void OnNavigationCanceled(NavigationEventData eventData) {
        Debug.Log(eventData.selectedObject);
    }
}
