using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NodeReferencesVisualiser))]
public class NodeReferencesVisualiserEditor : Editor
{

	void OnSceneGUI()
	{
		NodeReferencesVisualiser R = (NodeReferencesVisualiser)target;

		for (int i = 0; i < R.transform.childCount; ++i)
		{
			NodeReferences ThisNode = R.transform.GetChild(i).GetComponent<NodeReferences>();
			Vector3 ThisNodePosition = ThisNode.transform.position;
			Vector3 NextNodePosition = ThisNode.nextNode.transform.position;

			Handles.color = R.Editor_Colour;
			Handles.DrawLine(ThisNodePosition, NextNodePosition);
		}
	}

}

[CustomEditor(typeof(NodeReferences))]
public class NodeRenferencesEditor : Editor
{

	void OnSceneGUI()
	{
		NodeReferences R = (NodeReferences)target;

		Handles.color = Color.white;
		Handles.DrawLine(R.transform.position,R.nextNode.transform.position);
	}

}
