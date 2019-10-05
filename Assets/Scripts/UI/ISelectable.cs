using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstraction for objects that can be selected by the selection controller.
/// </summary>
public interface ISelectable
{
	void OnSelect();
	void OnDeselect();
}
