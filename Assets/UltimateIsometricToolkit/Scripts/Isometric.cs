using System;
using UnityEngine;

namespace UltimateIsometricToolkit.Scripts.Utils {
	/// <summary>
	///     Isometric helper class
	/// </summary>
	public class Isometric {
		/// <summary>
		///     Defines projection type
		/// </summary>
		public enum Projection {
			/// <summary>
			///     classic isometric projection
			/// </summary>
			Isometric = 0,

			/// <summary>
			///     classic dimetric projection
			/// </summary>
			Dimetric1x2 = 1,

			/// <summary>
			///     Military (oblique) projection
			/// </summary>
			Military = 2,

			/// <summary>
			///     Dimetric projection with 42 deg on one side and 7 deg on the other
			/// </summary>
			Dimetric42x7 =3 
		}

		/// <summary>
		///     Returns the projection matrix for a given projection.
		/// </summary>
		/// <param name="projection">Projection</param>
		/// <param name="xRot">custom rotation around x axis</param>
		/// <param name="yRot">custom rotation around y (up) axis</param>
		/// <returns></returns>
		public static Matrix4x4 GetProjectionMatrix(Projection projection) {
			Matrix4x4 projectionMatrix;
			switch (projection) {
				case Projection.Dimetric1x2:
					projectionMatrix = GetOrthographicProjectionMatrix(30, 45);
					break;
				case Projection.Isometric:
					projectionMatrix = GetOrthographicProjectionMatrix(35.625f, 45);
					break;
				case Projection.Military:
					projectionMatrix = GetOrthographicProjectionMatrix(45, 45);
					break;
				case Projection.Dimetric42x7:
					projectionMatrix = GetOrthographicProjectionMatrix(20, 70);
					break;
				default:
					throw new ArgumentOutOfRangeException("projection", projection, null);
			}

			return projectionMatrix;
		}

		/// <summary>
		///     Returns an orthographic projection matrix with a rotation around the global x and y axis
		/// </summary>
		/// <param name="xRot"></param>
		/// <param name="yRot"></param>
		/// <returns></returns>
		private static Matrix4x4 GetOrthographicProjectionMatrix(float xRot, float yRot) {
			return Matrix4x4.Rotate(GetQuaternion(xRot,yRot));
		}

		private static Quaternion GetQuaternion(float xRot, float yRot) {
			return Quaternion.AngleAxis(yRot, Vector3.up) * Quaternion.AngleAxis(xRot, Vector3.right);
		}

		/// <summary>
		///     Returns the quaternion that rotates from Unity to Iso space given a projection,global xRot and yRot
		/// </summary>
		/// <param name="projection">desired projection</param>
		/// <param name="xRot">x axis rotation, only relevant for custom projections</param>
		/// <param name="yRot">y axis rotation, only relevant for custom projections</param>
		/// <returns></returns>
		public static Quaternion GetProjectionQuaternion(Projection projection) {
			Quaternion quaternion;
			switch (projection) {
				case Projection.Dimetric1x2:
					quaternion = GetQuaternion(30, 45);
					break;
				case Projection.Isometric:
					quaternion = GetQuaternion(35.625f, 45);
					break;
				case Projection.Military:
					quaternion = GetQuaternion(45, 45);
					break;
				case Projection.Dimetric42x7:
					quaternion = GetQuaternion(20, 70);
					break;
				default:
					throw new ArgumentOutOfRangeException("projection", projection, null);
			}

			return quaternion;
		}
	}
}