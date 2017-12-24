using System;
using HoloToolkit.Unity.InputModule;
using UniRx;
using UnityEngine;

public class HandGesture : MonoBehaviour, IHoldHandler, IFocusable, INavigationHandler {
    private SwipeEnum swipe = SwipeEnum.None;
    private DateTime starttime;

    private Subject<Unit> onAirTap = new Subject<Unit>();

    public Subject<Unit> OnAirTap {
        get { return onAirTap; }
    }

    private Subject<Unit> onAirDoubleTap = new Subject<Unit>();

    public Subject<Unit> OnAirDoubleTap {
        get { return onAirDoubleTap; }
    }

    private Subject<Unit> onSwipeRight = new Subject<Unit>();

    public Subject<Unit> OnSwipeRight {
        get { return onSwipeRight; }
    }

    private Subject<Unit> onSwipeLeft = new Subject<Unit>();

    public Subject<Unit> OnSwipeLeft {
        get { return onSwipeLeft; }
    }

    private Subject<Unit> onSwipeUp = new Subject<Unit>();

    public Subject<Unit> OnSwipeUp {
        get { return onSwipeUp; }
    }

    private Subject<Unit> onSwipeDown = new Subject<Unit>();

    public Subject<Unit> OnSwipeDown {
        get { return onSwipeDown; }
    }


    void OnEnable() {
        var eventTrigger = gameObject.AddComponent<ObservableHandGestureTrigger>();

        eventTrigger.OnInputDownObservable()
            .TakeUntilDisable(this)
            //          .Where(eventData => eventData.selectedObject == gameObject)
            .Subscribe(_ => starttime = DateTime.Now);

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
        //   gameObject.GetComponent<Renderer>().material.color = Color.red;
    }

    public void OnFocusExit() {
        //    gameObject.GetComponent<Renderer>().material.color = Color.white;
    }

    public void OnNavigationStarted(NavigationEventData eventData) {
        InputManager.Instance.PushModalInputHandler(eventData.selectedObject);
    }

    public void OnNavigationUpdated(NavigationEventData eventData) {
        var x = eventData.NormalizedOffset.x;
        var y = eventData.NormalizedOffset.y;


        var type = Math.Abs(x) > Math.Abs(y)
            ? RightOrLeft(x)
            : UpOrDown(y);

        switch (type) {
            case SwipeEnum.Right:
                swipe = SwipeEnum.Right;
                break;
            case SwipeEnum.Left:
                swipe = SwipeEnum.Left;
                break;
            case SwipeEnum.Up:
                swipe = SwipeEnum.Up;
                break;
            case SwipeEnum.Down:
                swipe = SwipeEnum.Down;
                break;
            default:
                swipe = SwipeEnum.None;
                break;
        }
//        Debug.LogFormat("OnNavigationUpdated\r\nSource: {0}  SourceId: {1}\r\nCumulativeDelta: {2} {3} {4}",
//            eventData.InputSource,
//            eventData.SourceId,
//            eventData.NormalizedOffset.x,
//            eventData.NormalizedOffset.y,
//            eventData.NormalizedOffset.z);
    }

    public void OnNavigationCompleted(NavigationEventData eventData) {
        Debug.Log(eventData.selectedObject);
        if ((DateTime.Now - starttime).TotalMilliseconds < 800) {
            switch (swipe) {
                case SwipeEnum.Right:
                    onSwipeRight.OnNext(Unit.Default);
                    break;
                case SwipeEnum.Left:
                    onSwipeLeft.OnNext(Unit.Default);
                    break;
                case SwipeEnum.Up:
                    onSwipeUp.OnNext(Unit.Default);
                    break;
                case SwipeEnum.Down:
                    onSwipeDown.OnNext(Unit.Default);
                    break;
                default:
                    swipe = SwipeEnum.None;
                    break;
            }
        }
        InputManager.Instance.PopModalInputHandler();
    }

    public void OnNavigationCanceled(NavigationEventData eventData) {
        Debug.Log(eventData.selectedObject);
    }

    public SwipeEnum RightOrLeft(double x) {
        return x > 0 ? SwipeEnum.Right : SwipeEnum.Left;
    }


    public SwipeEnum UpOrDown(double y) {
        return y > 0 ? SwipeEnum.Up : SwipeEnum.Down;
    }

    public enum SwipeEnum {
        Right,
        Left,
        Up,
        Down,
        None
    }
}