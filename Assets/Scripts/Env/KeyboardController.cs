using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour {
	public bool Listening { get; set; }
	public delegate void KeyAction();

	//Do not allow observation of the internal state of the KeyBoardController.
	private Dictionary<KeyCode, KeyAction> Bindings;

	public void Start(){
		Bindings = new Dictionary<KeyCode, KeyAction>();
	}

	/* Keyboard Controllers are expensive. Try not to run to many at once, and maintain
	 * as few as possible because of this update function.
	 */
	public void Update(){
		
		if (Input.anyKeyDown){
			foreach (KeyCode key in Bindings.Keys){
				if (Input.GetKeyDown(key)){
					KeyAction toDo = Bindings[key];

					//Run the binded action.
					toDo();
				}
			}
		}
	}

	/// <summary>
	/// Binds a key to an action.
	/// </summary>
	public void BindKey(KeyCode key, KeyAction action = null)
	{
		if (Bindings.ContainsKey(key))
		{
			//For now, kill the game if you try to bind without unbinding. We can change later.
			throw new UnityException("Attempted to bind already bound key!");
		}

		if (action != null)
		{
			Bindings.Add(key, action);
		} 
		else 
		{
			Bindings.Add(key, DefaultAction);
		}
	}

	/// <summary>
	/// Unbinds a key from an action.
	/// </summary>
	public void UnbindKey(KeyCode key) 
	{
		if (!Bindings.ContainsKey(key))
		{
			//For now, kill the game if you try to unbind without binding. We can change later.
			throw new UnityException("Attempted to unbind key that was never bound!");
		} 
		else 
		{
			Bindings.Remove(key);
		}
	}

	/// <summary>
	/// Default KeyBinding action.
	/// </summary>
	public void DefaultAction(){
		Debug.Log("Key Bound to default action.");
		return;
	}
}
