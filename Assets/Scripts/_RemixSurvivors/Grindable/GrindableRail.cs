using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Darklight.UnityExt.Core3D;
using Darklight.UnityExt.Editor;
using NaughtyAttributes;
using RemixSurvivors.Survivor;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RemixSurvivors
{
    [RequireComponent(typeof(SplineContainer))]
    [RequireComponent(typeof(SplineExtrude))]
    public partial class GrindableRail : MonoBehaviour
    {
        const float MAX_RAILPOINT_SPACING = 0.25f;

        SplineContainer _splineContainer;
        SplineExtrude _splineExtrude;

        [SerializeField, ShowOnly]
        float _totalSplineLength;

        /// <summary>
        /// List of spline points, as calculated from the number of segments.
        /// </summary>
        /// <returns></returns>
        [SerializeField, ReadOnly, AllowNesting]
        List<RailPoint> _railPoints = new();

        public SplineContainer SplineContainer => _splineContainer;
        public Spline Spline => _splineContainer.Spline;
        public float TotalSplineLength => _totalSplineLength;
        public List<RailPoint> RailPoints => _railPoints;

        void Awake() => Initialize();

        public void Initialize()
        {
            // ( Get or Add Spline Container ) ------------------------------------
            _splineContainer = GetComponent<SplineContainer>();
            if (_splineContainer == null)
                _splineContainer = gameObject.AddComponent<SplineContainer>();

            // ( Get or Add Spline Extrude ) ------------------------------------
            _splineExtrude = GetComponent<SplineExtrude>();
            if (_splineExtrude == null)
                _splineExtrude = gameObject.AddComponent<SplineExtrude>();

            // ( Calculate Spline Length ) ------------------------------------
            _totalSplineLength = _splineContainer.CalculateLength();

            // ( Create Spline Points ) ------------------------------------
            CreateRailPoints(out _railPoints);
        }

        /// <summary>
        /// Create the spline points for the rail.
        /// </summary>
        /// <param name="splinePoints">The list of spline points to create.</param>
        private void CreateRailPoints(out List<RailPoint> splinePoints)
        {
            splinePoints = new();
            float totalLength = _splineContainer.CalculateLength();
            int estimatedPointCount = Mathf.CeilToInt(totalLength / MAX_RAILPOINT_SPACING);

            float targetSpacing = totalLength / estimatedPointCount;
            int truePointCount = Mathf.CeilToInt(totalLength / targetSpacing);

            for (int i = 0; i < truePointCount; i++)
            {
                float progress = (float)i / (truePointCount - 1);
                splinePoints.Add(new RailPoint(this, progress));
            }
        }

        /// <summary>
        /// Calculates the normalised time value for the rail's spline by evlating the player's position.
        /// </summary>
        /// <param name="position">Vector3 position to find the closest point on the spline to.</param>
        /// <param name="splineWorldPosition">Vector3 position for point on the spline the player is closest to.</param>
        /// <returns>float time - Normalised time value between 0 & 1.</returns>
        public void CalculateClosestSplinePosition(
            Vector3 position,
            out Vector3 splineWorldPosition,
            out float splineProgress
        )
        {
            float3 nearestPoint;
            float time;

            Vector3 localPos = Spatial3D.WorldToLocalConversion(transform, position);
            SplineUtility.GetNearestPoint(Spline, localPos, out nearestPoint, out time);
            splineWorldPosition = Spatial3D.LocalToWorldConversion(transform, nearestPoint);
            splineProgress = time;
        }

        #region < METHODS > [[ RailPoint Handlers ]] ==========================================================
        /// <summary>
        /// Get a point by its index in the spline.
        /// </summary>
        /// <param name="index">The index of the point to get.</param>
        /// <param name="point">The point at the given index.</param>
        public void GetRailPointByIndex(int index, out RailPoint point)
        {
            point = _railPoints[index];
        }

        /// <summary>
        /// Get the point on the spline that is closest to the given world position.
        /// </summary>
        /// <param name="worldPosition">The world position to find the closest point to.</param>
        /// <returns>The point on the spline that is closest to the given world position.</returns>
        public RailPoint GetClosestRailPointToPosition(Vector3 worldPosition)
        {
            return _railPoints
                .OrderBy(s => Vector3.Distance(s.WorldPosition, worldPosition))
                .First();
        }

        /// <summary>
        /// Get the point on the spline that is closest to the given world position.
        /// </summary>
        /// <param name="worldPosition">The world position to find the closest point to.</param>
        /// <param name="point">The point on the spline that is closest to the given world position.</param>
        public void GetClosestRailPointToPosition(Vector3 worldPosition, out RailPoint point)
        {
            point = GetClosestRailPointToPosition(worldPosition);
        }

        /// <summary>
        /// Get the point on the spline that is closest to the given progress along the spline.
        /// </summary>
        /// <param name="progress">The progress along the spline to find the closest point to.</param>
        /// <returns>The point on the spline that is closest to the given progress along the spline.</returns>
        public RailPoint GetClosestRailPointToProgress(float progress)
        {
            if (progress < 0 || progress > 1)
                throw new ArgumentOutOfRangeException(
                    nameof(progress),
                    "Normalised time must be between 0 and 1"
                );

            int index = (int)(progress * (_railPoints.Count - 1));
            if (index >= _railPoints.Count)
            {
                Debug.LogWarning(
                    $"Normalised time {progress} is out of range for spline with {_railPoints.Count} segments"
                );
                return _railPoints[_railPoints.Count - 1];
            }

            return _railPoints[index];
        }

        /// <summary>
        /// Get the next point on the spline by adding 1 to the index of the given point.
        /// </summary>
        /// <param name="point">The point to get the next point from.</param>
        /// <param name="nextPoint">The next point on the spline.</param>
        /// <returns>True if the next point is valid, false otherwise.</returns>
        public bool GetNextRailPoint(RailPoint point, out RailPoint nextPoint, int pointsToSkip = 1)
        {
            nextPoint = new RailPoint();

            int index = _railPoints.IndexOf(point);
            int nextIndex = index + pointsToSkip;

            // Return false if the index is out of range
            if (!IsRailPointIndexInRange(nextIndex))
                return false;

            // Set the next point and return IsNull value
            nextPoint = _railPoints[nextIndex];
            return nextPoint.IsNull;
        }

        /// <summary>
        /// Get the previous point on the spline by subtracting 1 from the index of the given point.
        /// </summary>
        /// <param name="point">The point to get the previous point from.</param>
        /// <param name="previousPoint">The previous point on the spline.</param>
        /// <returns>True if the previous point is valid, false otherwise.</returns>
        public bool GetPreviousRailPoint(
            RailPoint point,
            out RailPoint previousPoint,
            int pointsToSkip = 1
        )
        {
            previousPoint = new RailPoint();

            int index = _railPoints.IndexOf(point);
            int prevIndex = index - pointsToSkip;

            // Return false if the index is out of range
            if (!IsRailPointIndexInRange(prevIndex))
                return false;

            // Set the previous point and return IsNull value
            previousPoint = _railPoints[prevIndex];
            return previousPoint.IsNull;
        }

        /// <summary>
        /// Calculates the direction the player is going on the rail based on their direction during the initial collision.
        /// </summary>
        /// <param name="railForward">The forward of the point on the spline.</param>
        /// <param name="playerForward">The player's forward.</param>
        public void CalculateEnterDirection(
            RailPoint point,
            Vector3 playerDirection,
            out int enterDirection
        )
        {
            //This calculates the severity of the angle between the player's forward and the forward of the point on the spline.
            //90 degrees is the cutoff point as it's the perpendicular to the rail. Anything more than that and the player is clearly
            //facing the other direction to the rail point.
            float angle = Vector3.Angle(point.Forward, playerDirection.normalized);
            if (angle > 90f)
                enterDirection = -1;
            else
                enterDirection = 1;
        }

        bool IsRailPointIndexInRange(int index)
        {
            return index >= 0 && index <= _railPoints.Count - 1;
        }
        #endregion

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(GrindableRail))]
        public class GrindableRailCustomEditor : UnityEditor.Editor
        {
            SerializedObject _serializedObject;
            GrindableRail _script;
            Vector3 _lastPosition;

            private void OnEnable()
            {
                _serializedObject = new SerializedObject(target);
                _script = (GrindableRail)target;
                _script.Initialize();
            }

            public override void OnInspectorGUI()
            {
                _serializedObject.Update();

                EditorGUI.BeginChangeCheck();

                if (GUILayout.Button("Initialize"))
                    _script.Initialize();

                base.OnInspectorGUI();

                if (EditorGUI.EndChangeCheck())
                {
                    _serializedObject.ApplyModifiedProperties();
                    _script.Initialize();
                }
            }

            public void OnSceneGUI()
            {
                GrindableRail rail = (GrindableRail)target;

                // ( Draw the spline points ) ------------------------------------
                foreach (var point in rail._railPoints)
                {
                    point.DrawGizmos();
                }

                // ( Reinitialize if the rail has moved ) ------------------------------------
                if (_lastPosition != rail.transform.position)
                {
                    rail.Initialize();
                    _lastPosition = rail.transform.position;
                }
            }
        }
#endif
    }
}
