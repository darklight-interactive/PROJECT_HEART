using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace RemixSurvivors
{
    /// <summary>
    /// Represents a direction vector with additional linear algebra operations
    /// </summary>
    public struct DirectionVector : IEquatable<DirectionVector>
    {
        private Vector3 _vector;

        /// <summary>
        /// Creates a new DirectionVector from a Vector3
        /// </summary>
        /// <param name="vector">The source vector</param>
        public DirectionVector(Vector3 vector)
        {
            _vector = vector;
        }

        public static DirectionVector Zero => new DirectionVector(Vector3.zero);
        public static DirectionVector Up => new DirectionVector(Vector3.up);
        public static DirectionVector Down => new DirectionVector(Vector3.down);
        public static DirectionVector Left => new DirectionVector(Vector3.left);
        public static DirectionVector Right => new DirectionVector(Vector3.right);
        public static DirectionVector Forward => new DirectionVector(Vector3.forward);
        public static DirectionVector Back => new DirectionVector(-Vector3.forward);

        /// <summary>
        /// The normalized direction of the vector
        /// </summary>
        public Vector3 Normalized => _vector.normalized;

        /// <summary>
        /// The magnitude (length) of the vector
        /// </summary>
        public float Magnitude => _vector.magnitude;

        /// <summary>
        /// The raw vector value
        /// </summary>
        public Vector3 Raw => _vector;

        #region Operators and Equality
        public static DirectionVector operator +(DirectionVector a, DirectionVector b) =>
            new DirectionVector(a._vector + b._vector);

        public static DirectionVector operator +(DirectionVector a, Vector3 b) =>
            new DirectionVector(a._vector + b);

        public static DirectionVector operator +(Vector3 a, DirectionVector b) =>
            new DirectionVector(a + b._vector);

        public static DirectionVector operator -(DirectionVector a, DirectionVector b) =>
            new DirectionVector(a._vector - b._vector);

        public static DirectionVector operator -(DirectionVector a, Vector3 b) =>
            new DirectionVector(a._vector - b);

        public static DirectionVector operator -(Vector3 a, DirectionVector b) =>
            new DirectionVector(a - b._vector);

        public static DirectionVector operator *(DirectionVector a, float scalar) =>
            new DirectionVector(a._vector * scalar);

        public static DirectionVector operator *(float scalar, DirectionVector a) =>
            new DirectionVector(scalar * a._vector);

        public static DirectionVector operator /(DirectionVector a, float scalar) =>
            new DirectionVector(a._vector / scalar);

        public static bool operator ==(DirectionVector left, DirectionVector right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DirectionVector left, DirectionVector right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Vector Operations

        /// <summary>
        /// Projects this vector onto another vector
        /// </summary>
        /// <param name="onto">Vector to project onto</param>
        /// <returns>The projected DirectionVector</returns>
        public DirectionVector ProjectOnto(DirectionVector onto)
        {
            Vector3 projection = Vector3.Project(_vector, onto._vector);
            return new DirectionVector(projection);
        }

        /// <summary>
        /// Lerps between this vector and another vector
        /// </summary>
        /// <param name="target">Target vector</param>
        /// <param name="t">Interpolation parameter (0-1)</param>
        /// <returns>The interpolated DirectionVector</returns>
        public DirectionVector LerpTo(DirectionVector target, float t)
        {
            return new DirectionVector(Vector3.Lerp(_vector, target._vector, t));
        }

        /// <summary>
        /// Spherically interpolates between this vector and another vector
        /// </summary>
        /// <param name="target">Target vector</param>
        /// <param name="t">Interpolation parameter (0-1)</param>
        /// <returns>The interpolated DirectionVector</returns>
        public DirectionVector SlerpTo(DirectionVector target, float t)
        {
            return new DirectionVector(Vector3.Slerp(_vector, target._vector, t));
        }

        #endregion

        #region Linear Algebra

        /// <summary>
        /// Calculates the dot product with another vector
        /// </summary>
        /// <param name="other">The other vector</param>
        /// <returns>Dot product value</returns>
        public float Dot(DirectionVector other)
        {
            return Vector3.Dot(_vector, other._vector);
        }

        /// <summary>
        /// Calculates the cross product with another vector
        /// </summary>
        /// <param name="other">The other vector</param>
        /// <returns>Cross product vector</returns>
        public DirectionVector Cross(DirectionVector other)
        {
            return new DirectionVector(Vector3.Cross(_vector, other._vector));
        }

        public bool Equals(DirectionVector other)
        {
            return _vector == other._vector;
        }

        public override bool Equals(object obj)
        {
            return obj is DirectionVector other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _vector.GetHashCode();
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Rotates the vector around an axis by a specified angle
        /// </summary>
        /// <param name="axis">Axis of rotation</param>
        /// <param name="angle">Angle in degrees</param>
        /// <returns>Rotated vector</returns>
        public DirectionVector RotateAround(DirectionVector axis, float angle)
        {
            return new DirectionVector(Quaternion.AngleAxis(angle, axis.Normalized) * _vector);
        }

        public override string ToString()
        {
            return $"Direction: {Normalized}, Magnitude: {Magnitude}";
        }

        public static implicit operator Vector3(DirectionVector d) => d._vector;

        public static implicit operator DirectionVector(Vector3 v) => new DirectionVector(v);

        #endregion

        #region SceneGUI Methods

#if UNITY_EDITOR
        public void DrawInEditor(Color color = default)
        {
            Handles.color = color == default ? Color.grey : color;
            Handles.DrawLine(Vector3.zero, _vector);
        }
#endif

        #endregion
    }
}
