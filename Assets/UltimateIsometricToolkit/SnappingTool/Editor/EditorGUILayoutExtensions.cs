using UnityEditor;
using UnityEngine;

namespace SnappingTool.Editor {

	/// <summary>
	/// Extension class for <see cref="EditorGUILayout"/>
	/// </summary>
	public static class EditorGUILayoutExtensions {

		/// <summary>
		/// Adds a toggle button with desired content and toggle value
		/// </summary>
		/// <param name="content">Content, e.g. a string</param>
		/// <param name="value">toggle value, where true marks the button as toggled</param>
		/// <returns></returns>
		public static bool ToggleButton(GUIContent content, bool value) {
			var size = EditorStyles.miniButton.CalcSize(content);

			value = EditorGUILayout.Toggle(value, EditorStyles.miniButton, GUILayout.Width(size.x),
				GUILayout.Height(size.y * 1.1f));

			var style = new GUIStyle("Label") {normal = {textColor = value ? Color.white : Color.black}};
			EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), content, style);

			return value;
		}
	}
}