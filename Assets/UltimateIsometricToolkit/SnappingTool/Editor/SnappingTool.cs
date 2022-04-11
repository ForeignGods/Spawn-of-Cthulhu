using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnappingTool.Editor {
	/// <summary>
	///     A better version of Unity's built-in snapping tool
	/// </summary>
	public class SnappingTool : EditorWindow {
		/// <summary>
		///	 Snaps Transforms to closest multiple of this vector  
		/// </summary>
		public static Vector3 SnappingVector = Vector3.one / 2;

		/// <summary>
		/// Enabled/Disabled flag
		/// </summary>
		public static bool Snap;


		[MenuItem("Tools/SnappingTool %_l")]
		private static void Init() {
			var window = (SnappingTool) GetWindow(typeof(SnappingTool));
			window.titleContent = new GUIContent("Snapping Tool");
			window.maxSize = new Vector2(400, 200);
		}

		private void OnEnable() {
			var values = EditorPrefs.GetString("snappingVector").Split(';');
			SnappingVector.x = float.Parse(values[0]);
			SnappingVector.y = float.Parse(values[1]);
			SnappingVector.z = float.Parse(values[2]);

			Snap = EditorPrefs.GetBool("doSnap");
		}

		private void OnDisable() {
			EditorPrefs.SetString("snappingVector", SnappingVector.x + ";" + SnappingVector.y + ";" + SnappingVector.z + ";");
			EditorPrefs.SetBool("doSnap", Snap);
		}

		public void OnGUI() {
			SnappingVector.x =
				EditorGUILayout.FloatField(
					new GUIContent("X-Axis:", "Snaps selection to closest multiple of that vector on global X axis"),
					SnappingVector.x);
			SnappingVector.y =
				EditorGUILayout.FloatField(
					new GUIContent("Y-Axis:", "Snaps selection to closest multiple of that vector on global Y(up) axis"),
					SnappingVector.y);
			SnappingVector.z =
				EditorGUILayout.FloatField(
					new GUIContent("Z-Axis:", "Snaps selection to closest multiple of that vector on global Z axis"),
					SnappingVector.z);

			Snap = EditorGUILayoutExtensions.ToggleButton(new GUIContent("Auto Snap"), Snap);

			if (GUILayout.Button(new GUIContent("Snap selection",
				"Snap the current selection in Scene view to the \n closest multiple of the snapping Vector")))
				SnapSelection();
		}
		
		private void SnapSelection() {
			if (EditorApplication.isPlaying)
				return;
			Undo.RecordObjects(Selection.gameObjects, "ChangePosition");

			foreach (var transform in Selection.gameObjects.Where(c => c.GetComponent<Transform>() != null).ToList()
				.Select(obj => obj.GetComponent<Transform>())) {
				transform.transform.position = Round(transform.transform.position);
				EditorUtility.SetDirty(transform);
			}

			if (!EditorApplication.isPlaying)
				EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
		}

		private void Update() {
			if (!Snap)
				return;
			if (EditorApplication.isPlaying) {
				if (EditorPrefs.GetBool("autoSnappingWarning")) {
					Debug.LogWarning(
						"AutoSnap in SnappingTool still enabled. Physics components may act weird. Open Tools/SnappingTool and disable AutoSnap.");
					EditorPrefs.SetBool("autoSnappingWarning", false);
					return;
				}
			}
			else {
				EditorPrefs.SetBool("autoSnappingWarning", true);
			}

			foreach (var t in Selection.gameObjects.Select(g => g.transform)) t.position = Round(t.position);
		}

		/// <summary>
		///     Rounds input vector to the next multiple SnappingVector
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static Vector3 Round(Vector3 input) {
			if (Math.Abs(SnappingVector.x) > Mathf.Epsilon)
				input.x = SnappingVector.x * Mathf.Round(input.x / SnappingVector.x);
			if (Math.Abs(SnappingVector.y) > Mathf.Epsilon)
				input.y = SnappingVector.y * Mathf.Round(input.y / SnappingVector.y);
			if (Math.Abs(SnappingVector.z) > Mathf.Epsilon)
				input.z = SnappingVector.z * Mathf.Round(input.z / SnappingVector.z);

			return input;
		}
	}
}