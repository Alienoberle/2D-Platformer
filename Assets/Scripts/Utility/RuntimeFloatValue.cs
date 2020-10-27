using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FloatValue", menuName = "ScriptableObject/RuntimeValues")]
public class RuntimeFloatValue : ScriptableObject, ISerializationCallbackReceiver
{
	public float InitialValue;
	[System.NonSerialized]
	public float RuntimeValue;

	public void OnAfterDeserialize()
	{
		RuntimeValue = InitialValue;
	}

	public void OnBeforeSerialize() {}

}
