using UnityEngine;

namespace RemixSurvivors
{
    [System.Serializable]
    public class MotionVector
    {
        Vector3 _vector = Vector3.zero;

        public MotionVector()
        {
            Horizontal = Vector2.zero;
            Vertical = 0f;
        }

        public MotionVector(Vector2 horizontal, float vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }

        public static MotionVector Zero => new MotionVector(Vector2.zero, 0f);

        public Vector2 Horizontal
        {
            get => new Vector2(_vector.x, _vector.z);
            set
            {
                _vector.x = value.x;
                _vector.z = value.y;
            }
        }

        public Vector3 HorizontalVec3
        {
            get => new Vector3(Horizontal.x, 0f, Horizontal.y);
            set
            {
                Horizontal = new Vector2(value.x, value.z);
            }
        }

        public float Vertical
        {
            get => _vector.y;
            set => _vector.y = value;
        }

        public Vector3 VerticalVec3
        {
            get => new Vector3(0f, Vertical, 0f);
            set => Vertical = value.y;
        }

        public Vector3 Combined
        {
            get => new Vector3(Horizontal.x, Vertical, Horizontal.y);
            set
            {
                Horizontal = new Vector2(value.x, value.z);
                Vertical = value.y;
            }
        }

        public Vector3 Normalized => Combined.normalized;
        public float Magnitude => Combined.magnitude;



        public void Set(Vector3 vector)
        {
            Horizontal = new Vector2(vector.x, vector.z);
            Vertical = vector.y;
        }

        public void Set(Vector2 horizontal, float vertical)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }

        public void Set(MotionVector motionVector)
        {
            Horizontal = motionVector.Horizontal;
            Vertical = motionVector.Vertical;
        }

        public void SetZero()
        {
            Horizontal = Vector2.zero;
            Vertical = 0f;
        }

        public void SetOne()
        {
            Horizontal = Vector2.one;
            Vertical = 1f;
        }

        public void Clamp(float horzClamp, float vertClamp)
        {
            Horizontal = new Vector2(
                Mathf.Clamp(Horizontal.x, -horzClamp, horzClamp),
                Mathf.Clamp(Horizontal.y, -horzClamp, horzClamp)
            );
            Vertical = Mathf.Clamp(Vertical, -vertClamp, vertClamp);
        }

        public void Reset()
        {
            Horizontal = Vector2.zero;
            Vertical = 0f;
        }

        public string Print()
        {
            return $"Horizontal: {Horizontal}, Vertical: {Vertical}";
        }

        
        public static MotionVector Lerp(MotionVector a, MotionVector b, float t)
        {
            return new MotionVector(
                Vector2.Lerp(a.Horizontal, b.Horizontal, t),
                Mathf.Lerp(a.Vertical, b.Vertical, t)
            );
        }
    }
}
