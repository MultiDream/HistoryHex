using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : MonoBehaviour {
	public bool Listening { get; set; }
	public delegate void KeyAction();

	//Do not allow observation of the internal state of the KeyBoardController.
	private Dictionary<KeyCode, KeyAction> Bindings = new Dictionary<KeyCode, KeyAction>();

	public void Start(){
	}

	/* Keyboard Controllers are expensive. Try not to run to many at once, and maintain
	 * as few as possible because of this update function.
	 */
	public void Update(){
		if (Listening) {
			if (Input.anyKeyDown) {
				foreach (KeyCode key in Bindings.Keys) {
					if (Input.GetKeyDown(key)) {
						//Compiler says this can be simplified. Don't trust it.
						KeyAction toDo = Bindings[key];

						//Run the binded action.
						if (toDo != null){
							toDo();
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Binds a key to an action.
	/// For now, multicasting is forbidden.
	/// </summary>
	public void BindKey(KeyCode key, KeyAction action)
	{
		if (Bindings.ContainsKey(key))
		{
			//For now, kill the game if you try to bind without unbinding. We can change later.
			throw new UnityException("Attempted to bind already bound key!");
		}
		Bindings.Add(key, action);
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
}
