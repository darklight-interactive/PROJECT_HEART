using UnityEngine;

namespace ProjectHeart
{
    [CreateAssetMenu(fileName = "BossStats", menuName = "Scriptable Objects/BossStats")]
    public class BossStats : ScriptableObject
    {
        public int Health;

        public int Speed;

        public int Strength; 
    
    }
}
