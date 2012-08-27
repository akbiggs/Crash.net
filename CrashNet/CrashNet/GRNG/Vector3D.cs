using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;

namespace CrashNet.GRNG
{
    /**
     * An implementation-independent basic 3d vector struct for use with the 
     * GRNG random number class.
     **/
    public struct Vector3D
    {
        private double x, y, z;

        public Vector3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3D(int x, int y, int z)
        {
            this.x = (double)x;
            this.y = (double)y;
            this.z = (double)z;
        }

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        public double Z
        {
            get { return z; }
            set { z = value; }

        }
    }
}
