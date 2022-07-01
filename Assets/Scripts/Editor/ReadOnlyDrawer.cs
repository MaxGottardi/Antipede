using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		bool bGUIIsEnabled = GUI.enabled;

		GUI.enabled = false;
		EditorGUI.PropertyField(position, property, label);

		GUI.enabled = bGUIIsEnabled;
	}
}
