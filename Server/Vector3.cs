using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Vector3
    {
        public float x = 0;
        public float y = 0;
        public float z = 0;

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
       
    }
}
