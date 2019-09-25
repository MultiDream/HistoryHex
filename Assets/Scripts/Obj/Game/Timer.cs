using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that can use real time to scheduale an event.
/// </summary>
public class Timer : MonoBehaviour {

	public float TimeRemaining { get; set; } = 0;
	public bool Enabled { get; set; } = false;

	/*  Need to revisit this area and check for memory leaks. */
	public delegate void Action();
	public Action OnAlarmScript = DefaultAction; 

	// Start is called before the first frame update
	void Start()
    {
	}

    // Update is called once per frame
    void Update()
    {
		//Do not continue if not enabled.
		if (!Enabled) {
			return;
		}

		TimeRemaining -= Time.deltaTime;
		if (TimeRemaining < 0){
			OnAlarm();
		}
	}

	private void OnAlarm(){
		Enabled = false;
		OnAlarmScript();
	}

	public static void DefaultAction(){
		Debug.Log("Alarm Activated!");
		return;
	}
}
