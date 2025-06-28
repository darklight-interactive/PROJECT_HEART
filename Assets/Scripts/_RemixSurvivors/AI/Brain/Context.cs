using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;
using UnityUtils;

namespace RemixSurvivors.AI
{
    /// <summary>
    /// A context object is used to pass information to a consideration,
    /// and is like a mini Blackboard that contains information about the agent and its surroundings.
    /// </summary>
    [Serializable]
    public class Context
    {
        [SerializeField, ReadOnly, AllowNesting]
        NavMeshAgent _agent;

        [SerializeField, ReadOnly, AllowNesting]
        Brain _brain;

        [SerializeField, ReadOnly, AllowNesting]
        Sensor _sensor;

        [SerializeField, ReadOnly, AllowNesting]
        Transform _target;

        [SerializeField, ReadOnly]
        SerializedDictionary<string, object> data = new();

        public Context(Brain brain)
        {
            // Check if the brain is null
            Preconditions.CheckNotNull(brain, nameof(brain));

            // Assign the brain to the context
            this._brain = brain;
            this._target = brain.transform;

            // Get the NavMeshAgent component from the brain's game object
            this._agent = brain.gameObject.GetComponent<NavMeshAgent>();
            this._sensor = brain.gameObject.GetComponent<Sensor>();
        }

        public NavMeshAgent Agent => _agent;
        public Brain Brain => _brain;
        public Sensor Sensor => _sensor;

        public Transform Target => _target;

        /// <summary>
        /// Get the data associated with the given key.
        /// </summary>
        /// <param name="key">The key to get the data for.</param>
        /// <typeparam name="T">The type of the data to get.</typeparam>
        /// <returns>The data associated with the given key, or the default value if the key is not found.</returns>
        public T GetData<T>(string key) =>
            data.TryGetValue(key, out var value) ? (T)value : default;

        /// <summary>
        /// Set the data associated with the given key.
        /// </summary>
        /// <param name="key">The key to set the data for.</param>
        /// <param name="value">The data to set.</param>
        public void SetData(string key, object value) => data[key] = value;
    }
}
