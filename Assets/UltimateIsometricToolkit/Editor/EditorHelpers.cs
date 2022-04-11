using System;
using UltimateIsometricToolkit.Scripts.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace UltimateIsometricToolkit.Editor
{
	/// <summary>
	///    Editor Helper methods
	/// </summary>
	[InitializeOnLoad]
	public static class EditorHelpers
	{
		private static readonly string projectionEnabledEditorPrefsKey = "Tools/UIT/Toggle SceneView %g";
		private static readonly string selectedProjectionEditorPrefsKey = "selectedProjection";
		private static bool _enabled;

		private static Isometric.Projection _selectedProjection;
		static EditorHelpers()
		{
			_enabled = EditorPrefs.GetBool(projectionEnabledEditorPrefsKey, false);
			
			EditorApplication.delayCall += () => {
				SetProjection((Isometric.Projection)EditorPrefs.GetInt(selectedProjectionEditorPrefsKey, 0));
				ApplyProjection(_enabled);
			};
		}


		[MenuItem("Tools/UIT/Toggle SceneView %g")]
		private static void ToggleAction()
		{
			/// Toggling action
			ApplyProjection(!_enabled);
		}

		[MenuItem("Tools/UIT/Projection/Isometric")]
		private static void SetIsoProjection() {
			SetProjection(Isometric.Projection.Isometric);
			ApplyProjection(true);
		}

		[MenuItem("Tools/UIT/Projection/Dimetric1x2")]
		private static void SetDimetricProjection()
		{
			SetProjection(Isometric.Projection.Dimetric1x2);
			ApplyProjection(true);
		}

		[MenuItem("Tools/UIT/Projection/Military")]
		private static void SetMilitaryProjection()
		{
			SetProjection(Isometric.Projection.Military);
			ApplyProjection(true);
		}

		[MenuItem("Tools/UIT/Projection/Dimetric42x7")]
		private static void SetMilitary()
		{
			SetProjection(Isometric.Projection.Dimetric42x7);
			ApplyProjection(true);
		}

		/// <summary>
		/// applies selected projection and sets editor prefs accordingly
		/// </summary>
		/// <param name="projection"></param>
		private static void SetProjection(Isometric.Projection projection) {
			_selectedProjection = projection;
			for (int i = 0; i < 4; i++)
				Menu.SetChecked("Tools/UIT/Projection/" + (Isometric.Projection) i, false);
			Menu.SetChecked("Tools/UIT/Projection/" + projection, true);

			EditorPrefs.SetInt(selectedProjectionEditorPrefsKey, (int)projection);
		}
		

		/// <summary>
		/// creates parent child object and adds <see cref="T"/> as a component to the appropriate object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		private static T AddType<T>() where T : Component
		{
			var active = Selection.activeTransform;
			GameObject parent;
			if (active.parent != null && string.Equals(active.parent.gameObject.name, active.name, StringComparison.InvariantCulture))
				parent = active.parent.gameObject;
			else
				parent = new GameObject(active.gameObject.name);
			parent.transform.parent = active.parent;
			parent.transform.position = active.position;
			active.parent = parent.transform;
			return parent.AddComponent<T>();
		}



		public static void ApplyProjection(bool enabled)
		{
			var wasEnabled = _enabled;
			_enabled = enabled;
			var sceneView = SceneView.lastActiveSceneView;
			if (sceneView == null)
				return;

			if (wasEnabled && _enabled && sceneView.in2DMode)
				_enabled = false;
			if (!wasEnabled && _enabled && sceneView.in2DMode)
				_enabled = true;

			Menu.SetChecked(projectionEnabledEditorPrefsKey, _enabled);
			EditorPrefs.SetBool(projectionEnabledEditorPrefsKey, _enabled);
			if (_enabled)
			{
				sceneView.in2DMode = false;
			sceneView.rotation = Isometric.GetProjectionQuaternion(_selectedProjection);
			sceneView.isRotationLocked = true;
			sceneView.orthographic = true;
			Camera.main.transform.rotation = sceneView.rotation;
			Camera.main.orthographic = true;
			}
			else if (!sceneView.in2DMode)
			{
				sceneView.isRotationLocked = false;
				sceneView.orthographic = false;
			}

			if (Camera.main != null)
				SetSorting(Camera.main);

			//scene view camera
			if (Camera.current != null)
				SetSorting(sceneView.camera);
		}

		private static void SetSorting(Camera camera)
		{
			if (camera == null)
				throw new ArgumentNullException("camera");
			GraphicsSettings.transparencySortMode = camera.transparencySortMode = TransparencySortMode.CustomAxis;
			GraphicsSettings.transparencySortAxis = camera.transparencySortAxis = new Vector3(0.49f, -1, 0.49f);
		}
	}
}