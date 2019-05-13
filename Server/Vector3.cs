using System;

namespace Server
{
    internal class Vector3
    {
        public float x = 0;
        public float y = 0;
        public float z = 0;

        public static Vector3 zero = new Vector3(0, 0, 0);

        public Vector3()
        {
            x = y = z = 0;
        }

        public Vector3(float xVal, float yVal)
        {
            x = xVal;
            y = yVal;
            z = 0;
        }

        public Vector3(float xVal, float yVal, float zVal)
        {
            x = xVal;
            y = yVal;
            z = zVal;
        }

        public float Length()
        {
            return (float)Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        public Vector3 Normalize()
        {
            if (Length() != 0)
            {
                return new Vector3(x / Length(), y / Length(), z / Length());
            }
            else return zero;
        }

        public static Vector3 operator -(Vector3 vec)
        {
            return new Vector3(-vec.x, -vec.y, -vec.z);
        }

        public static Vector3 operator -(Vector3 vec1, Vector3 vec2)
        {
            return new Vector3(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
        }

        public static Vector3 operator *(Vector3 vec, float num)
        {
            return new Vector3(vec.x * num, vec.y * num, vec.z * num);
        }
    }
}