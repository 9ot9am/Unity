﻿using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Line))]
public class LineInspector : UnityEditor.Editor {

	private void OnSceneGUI () {
		Line line = target as Line;
		Transform handleTransform = line.transform;
		Quaternion handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;
		Vector3 p0 = handleTransform.TransformPoint(line.p0);
		Vector3 p1 = handleTransform.TransformPoint(line.p1);

		Handles.color = Color.white;
		Handles.DrawLine(p0, p1);
		EditorGUI.BeginChangeCheck();
		p0 = Handles.DoPositionHandle(p0, handleRotation);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(line, "Move Point");
			EditorUtility.SetDirty(line);
			line.p0 = handleTransform.InverseTransformPoint(p0);
		}
		EditorGUI.BeginChangeCheck();
		p1 = Handles.DoPositionHandle(p1, handleRotation);
		if (EditorGUI.EndChangeCheck()) {
			Undo.RecordObject(line, "Move Point");
			EditorUtility.SetDirty(line);
			line.p1 = handleTransform.InverseTransformPoint(p1);
		}
	}
}