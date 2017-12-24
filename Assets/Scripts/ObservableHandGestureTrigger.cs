using HoloToolkit.Unity.InputModule;
using UniRx;
using UniRx.Triggers;

public class ObservableHandGestureTrigger : ObservableTriggerBase, IInputHandler {

    Subject<InputEventData> onInputDown;

    void IInputHandler.OnInputDown(InputEventData eventData) {
        onInputDown?.OnNext(eventData);
    }

    public Subject<InputEventData> OnInputDownObservable() {
        return onInputDown ?? (onInputDown = new Subject<InputEventData>());
    }

    Subject<InputEventData> onInputUp;
    
    void IInputHandler.OnInputUp(InputEventData eventData) {
         onInputUp?.OnNext(eventData);
    }
    
    public Subject<InputEventData> OnInputUpObservable() {
        return onInputUp?? (onInputUp = new Subject<InputEventData>());
    }
    
    protected override void RaiseOnCompletedOnDestroy() {
        if (onInputDown != null) {
            onInputDown.OnCompleted();
        }
        if (onInputUp!=null) {
            onInputUp.OnCompleted();
        }
    }
}