using SA3D.Modeling.Structs;
using System;
using System.Numerics;
using static SA3D.Common.MathHelper;

namespace SA3D.Rendering
{
	/// <summary>
	/// Camera handler.
	/// </summary>
	public sealed class Camera
	{
		#region private fields

		/// <summary>
		/// For <see cref="Position"/>
		/// </summary>
		private Vector3 _position;

		/// <summary>
		/// for <see cref="Rotation"/>
		/// </summary>
		private Vector3 _rotation;

		/// <summary>
		/// for <see cref="Orthographic"/>
		/// </summary>
		private bool _orthographic;

		/// <summary>
		/// for <see cref="Orbiting"/>
		/// </summary>
		private bool _orbiting;

		/// <summary>
		/// for <see cref="Distance"/>
		/// </summary>
		private float _distance;

		/// <summary>
		/// for <see cref="FieldOfView"/>
		/// </summary>
		private float _fov;

		/// <summary>
		/// for <see cref="Aspect"/>
		/// </summary>
		private float _aspect;

		/// <summary>
		/// for <see cref="FarPlane"/>
		/// </summary>
		private float _farPlane;

		private float _nearPlane;

		#endregion

		#region properties

		/// <summary>
		/// Position of the camera in world space <br/>
		/// Position of focus in orbit mode
		/// </summary>
		public Vector3 Position
		{
			get => _position;
			set
			{
				_position = value;
				UpdateViewMatrix();
			}
		}

		/// <summary>
		/// Position of camera in world space (regardless of orbit mode
		/// </summary>
		public Vector3 Realposition
			=> _position - (Forward * _distance);

		/// <summary>
		/// The rotation of the camera in world space
		/// </summary>
		public Vector3 Rotation
		{
			get => _rotation;
			set
			{
				_rotation = value;
				UpdateDirections();
			}
		}

		/// <summary>
		/// The Cameras global forward Direction
		/// </summary>
		public Vector3 Forward { get; private set; }

		/// <summary>
		/// The Cameras global right Direction
		/// </summary>
		public Vector3 Right { get; private set; }

		/// <summary>
		/// The Cameras global up Direction
		/// </summary>
		public Vector3 Up { get; private set; }

		/// <summary>
		/// Whether the control scheme is set to orbiting
		/// </summary>
		public bool Orbiting
		{
			get => _orbiting;
			set
			{
				if(_orbiting == value)
				{
					return;
				}

				_orbiting = value;

				Vector3 t = Forward * _distance;
				Position += _orbiting ? t : -t;

				UpdateViewMatrix();
				UpdateProjectionMatrix();
			}
		}

		/// <summary>
		/// The orbiting distance
		/// </summary>
		public float Distance
		{
			get => _distance;
			set
			{
				_distance = Math.Min(_farPlane, Math.Max(_nearPlane, value));
				UpdateViewMatrix();
				UpdateProjectionMatrix();
			}
		}

		/// <summary>
		/// The field of view
		/// </summary>
		public float FieldOfView
		{
			get => _fov;
			set
			{
				_fov = value;
				UpdateProjectionMatrix();
			}
		}

		/// <summary>
		/// The screen aspect
		/// </summary>
		public float Aspect
		{
			get => _aspect;
			set
			{
				_aspect = value;
				UpdateProjectionMatrix();
			}
		}

		/// <summary>
		/// Whether the camera is set to orthographic
		/// </summary>
		public bool Orthographic
		{
			get => _orthographic;
			set
			{
				if(_orthographic == value)
				{
					return;
				}

				_orthographic = value;
				UpdateViewMatrix();
				UpdateProjectionMatrix();
			}
		}

		/// <summary>
		/// The view/render distance
		/// </summary>
		public float FarPlane
		{
			get => _farPlane;
			set
			{
				_farPlane = value;
				UpdateViewMatrix();
				UpdateProjectionMatrix();
			}
		}

		/// <summary>
		/// The near clipping distance.
		/// </summary>
		public float NearPlane
		{
			get => _nearPlane;
			set
			{
				_nearPlane = value;
				UpdateProjectionMatrix();
			}
		}

		/// <summary>
		/// Custom world matrix for specific effects, e.g. reflection rendering
		/// </summary>
		public Matrix4x4 CustomWorldMatrix { get; set; }

		/// <summary>
		/// Camera View Matrix
		/// </summary>
		public Matrix4x4 ViewMatrix { get; private set; }

		/// <summary>
		/// Camera projection matrix
		/// </summary>
		public Matrix4x4 ProjectionMatrix { get; private set; }

		#endregion

		/// <summary>
		/// Creates a new camera from the resolution ratio
		/// </summary>
		/// <param name="aspect"></param>
		public Camera(float aspect)
		{
			_orbiting = true;
			_distance = 50;
			_fov = DegToRad(50);
			_aspect = aspect;
			_orthographic = false;
			_farPlane = 3000;
			_nearPlane = 1;
			CustomWorldMatrix = Matrix4x4.Identity;

			UpdateDirections();
			UpdateProjectionMatrix();
		}

		/// <summary>
		/// Recalculates the directions
		/// </summary>
		private void UpdateDirections()
		{
			Matrix4x4 matX = Matrix4x4.CreateRotationX(-_rotation.X);
			Matrix4x4 matY = Matrix4x4.CreateRotationY(-_rotation.Y);
			Matrix4x4 matZ = Matrix4x4.CreateRotationZ(-_rotation.Z);

			Matrix4x4 rot = Matrix4x4.Transpose(matY * matX * matZ);

			Forward = Vector3.Normalize(Vector3.TransformNormal(-Vector3.UnitZ, rot));
			Up = Vector3.Normalize(Vector3.TransformNormal(Vector3.UnitY, rot));
			Right = Vector3.Normalize(Vector3.TransformNormal(-Vector3.UnitX, rot));

			UpdateViewMatrix();
		}

		/// <summary>
		/// Recalculates the view matrix
		/// </summary>
		private void UpdateViewMatrix()
		{
			Matrix4x4 posMtx = Matrix4x4.CreateTranslation(-_position);

			Matrix4x4 matX = Matrix4x4.CreateRotationX(-_rotation.X);
			Matrix4x4 matY = Matrix4x4.CreateRotationY(-_rotation.Y);
			Matrix4x4 matZ = Matrix4x4.CreateRotationZ(-_rotation.Z);

			Matrix4x4 rotMtx = matY * matX * matZ;

			ViewMatrix = posMtx * rotMtx;

			if(_orbiting)
			{
				Vector3 orbitOffset = Forward * (_orthographic ? _farPlane * 0.5f : _distance);
				Matrix4x4 orbitMatrix = Matrix4x4.CreateTranslation(orbitOffset);
				ViewMatrix = orbitMatrix * ViewMatrix;
			}
		}

		/// <summary>
		/// Recalculates projection matrix
		/// </summary>
		private void UpdateProjectionMatrix()
		{
			ProjectionMatrix = _orthographic && _orbiting
				? Matrix4x4.CreateOrthographic(_distance * _aspect, _distance, _nearPlane, _farPlane)
				: Matrix4x4.CreatePerspectiveFieldOfView(_fov, _aspect, _nearPlane, _farPlane);
		}

		/// <summary>
		/// Calculates the model-view-projection matrix.
		/// </summary>
		/// <param name="world">Model matrix to use.</param>
		/// <returns>The calculated model-view-projection matrix.</returns>
		public Matrix4x4 GetMVPMatrix(Matrix4x4 world)
		{
			return world * CustomWorldMatrix * ViewMatrix * ProjectionMatrix;
		}

		/// <summary>
		/// Calculates the model-projection matrix.
		/// </summary>
		/// <param name="world">Model matrix to use.</param>
		/// <returns>The calculated model-view-projection matrix.</returns>
		public Matrix4x4 GetMPMatrix(Matrix4x4 world)
		{
			return world * CustomWorldMatrix * ProjectionMatrix;
		}

		/// <summary>
		/// Calculates the model-view matrix.
		/// </summary>
		/// <param name="world">Model matrix to use.</param>
		/// <returns>The calculated model-view-projection matrix.</returns>
		public Matrix4x4 GetMVMatrix(Matrix4x4 world)
		{
			return world * CustomWorldMatrix * ViewMatrix;
		}

		/// <summary>
		/// Checks wether bounds are rendable
		/// </summary>
		/// <param name="bounds"></param>
		/// <returns></returns>
		public bool CanRender(Bounds bounds)
		{
			Vector3 viewLocation = Vector3.Transform(bounds.Position, ViewMatrix);
			return viewLocation.Length() - bounds.Radius <= _farPlane
				&& viewLocation.Z <= bounds.Radius;
		}
	}
}
