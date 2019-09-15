using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
	public Timer timer; //Must be set.
	public int gameTime;

    // Start is called before the first frame update
    void Start()
    {
		timer.TimeRemaining = 2.0f;
		timer.Enabled = true;
		timer.OnAlarmScript = OnAlarm;
		Debug.Log("Timer Initialized.");
	}

    // Update is called once per frame
    void Update()
    {
    }

	void OnAlarm(){
		//Do the Stuff.
		gameTime++;
		transform.GetChild(2).GetComponent<Text>().text = $"GameTime: {gameTime}"; //Component 2 is currently a Text item.

		//Reset the timer. Move to Timer class.
		timer.TimeRemaining = 2;
		timer.Enabled = true;
	}
}
