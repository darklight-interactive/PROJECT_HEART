using UnityEngine;

namespace ProjectHeart
{
    [CreateAssetMenu(fileName = "BossStats", menuName = "Scriptable Objects/BossStats")]
    public class BossStats : ScriptableObject
    {
        public float Health;

        public float Speed;

        public float Strength; 
    
    }
}
