using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class TrackableEventHadler : MonoBehaviour,ITrackableEventHandler {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus) {
		throw new System.NotImplementedException();
	}
	
}
