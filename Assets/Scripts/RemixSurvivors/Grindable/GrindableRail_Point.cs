using System;
using System.Data.SqlTypes;
using Darklight.UnityExt.Core3D;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace RemixSurvivors
{
    public partial class GrindableRail
    {
        [System.Serializable]
        public struct RailPoint : INullable, IEquatable<RailPoint>
        {
            [SerializeField, ShowOnly]
            bool _isNull;

            [SerializeField, ShowOnly]
            float _progress;

            [SerializeField, ReadOnly, AllowNesting]
            float3 _localPosition;

            [SerializeField, ReadOnly, AllowNesting]
            float3 _worldPosition;

            [SerializeField, ReadOnly, AllowNesting]
            float3 _upVector;

            [SerializeField, ReadOnly, AllowNesting]
            float3 _tangentVector;

            public float Progress => _progress;
            public Vector3 LocalPosition => _localPosition;
            public Vector3 WorldPosition => _worldPosition;
            public Vector3 Tangent => _tangentVector;
            public Vector3 Up => _upVector;
            public Vector3 Forward => CalculateForward(_tangentVector);
            public Vector3 BiNormal => CalculateBiNormal(_tangentVector, _upVector);
            public Vector3 Normal => CalculateNormal(_tangentVector, BiNormal);
            public bool IsNull => _isNull;
            public bool IsValid => !IsNull;
            public bool IsStart => Progress == 0f;
            public bool IsEnd => Progress == 1f;

            // ---------------- [ Constructor ] -----------------------------
            public RailPoint(GrindableRail rail, float progress)
            {
                _isNull = true;
                _progress = progress;
                _localPosition = default;
                _worldPosition = default;
                _tangentVector = default;
                _upVector = default;
                _tangentVector = default;

                Initialize(rail, progress);
            }

            void Initialize(GrindableRail rail, float progress)
            {
                if (rail == null)
                    return;

                _isNull = false;
                _progress = progress;

                rail.SplineContainer.Evaluate(
                    _progress,
                    out float3 position,
                    out float3 tangent,
                    out float3 upVector
                );
                _worldPosition = position;
                _localPosition = Spatial3D.WorldToLocalConversion(rail.transform, _worldPosition);
                _tangentVector = tangent;
                _upVector = upVector;
            }

            #region < PUBLIC_STATIC_OPERATORS > ============================================================================
            public static bool operator ==(RailPoint a, RailPoint b)
            {
                if (a.IsNull && b.IsNull)
                    return true;
                if (a.IsNull || b.IsNull)
                    return false;
                return a.WorldPosition == b.WorldPosition && math.abs(a._progress - b._progress) < float.Epsilon;
            }

            public static bool operator !=(RailPoint a, RailPoint b)
            {
                return !(a == b);
            }

            public static bool operator ==(RailPoint a, object b)
            {
                if (b is null)
                    return a.IsNull;
                return a.Equals(b);
            }

            public static bool operator !=(RailPoint a, object b)
            {
                return !(a == b);
            }
            #endregion

            #region < PUBLIC_METHODS > [[ Public Handlers ]] ================================================================

            public void Distance(Vector3 position, out float distance)
            {
                distance = Vector3.Distance(position, WorldPosition);
            }

            public void Distance(RailPoint other, out float distance)
            {
                distance = Vector3.Distance(WorldPosition, other.WorldPosition);
            }

            public override bool Equals(object obj)
            {
                if (obj is RailPoint other)
                    return this == other;
                return false;
            }

            public bool Equals(RailPoint other)
            {
                if (IsNull && other.IsNull)
                    return true;
                if (IsNull || other.IsNull)
                    return false;
                return WorldPosition == other.WorldPosition
                    && math.abs(_progress - other._progress) < float.Epsilon;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(WorldPosition, _progress);
            }

            #endregion

            #region < PUBLIC_METHODS > [[ Draw Gizmos ]] ================================================================
            public void DrawGizmos()
            {
                // Draw world position
                CustomGizmos.DrawWireSphere(WorldPosition, 0.1f, Color.grey);

                // Draw tangent (grey)
                CustomGizmos.DrawLine(
                    WorldPosition,
                    WorldPosition + Tangent.normalized * 0.5f,
                    Color.grey
                );

                // Draw normal (green)
                CustomGizmos.DrawLine(WorldPosition, WorldPosition + Normal * 0.5f, Color.green);
            }
            #endregion

            #region < PRIVATE_METHODS > [[ Calculations ]] ================================================================

            float3 CalculateForward(float3 tangent)
            {
                return new float3(tangent.x, 0f, tangent.z);
            }

            /// <summary>
            /// Calculates the binormal vector perpendicular to both the tangent and up vector.
            /// </summary>
            /// <returns>Normalized binormal vector</returns>
            float3 CalculateBiNormal(float3 tangent, float3 up)
            {
                float3 normalizedTangent = math.normalize(tangent);
                float3 upVector = math.normalize(up);
                if (math.abs(math.dot(normalizedTangent, upVector)) > 0.99f)
                {
                    upVector = Vector3.forward;
                }
                return math.cross(normalizedTangent, upVector);
            }

            /// <summary>
            /// Calculates the normal vector perpendicular to the rail's direction.
            /// The normal points outward from the rail, perpendicular to both the tangent and binormal.
            /// </summary>
            /// <returns>Normalized vector perpendicular to the rail's direction</returns>
            float3 CalculateNormal(float3 tangent, float3 binormal)
            {
                float3 normalizedTangent = math.normalize(tangent);
                float3 normalizedBinormal = math.normalize(binormal);
                return math.cross(normalizedBinormal, normalizedTangent);
            }
            #endregion
        }
    }
}
