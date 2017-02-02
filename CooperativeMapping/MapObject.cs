using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using System.ComponentModel;

namespace CooperativeMapping
{
    /*[Serializable]
    public enum MapPlaceIndicator
    {
        Undiscovered = 0,
        Discovered = 1,
        Obstacle = 2,
        Platform = 3,
        OutOfBound = 4,
        NoBackVisist = 5
    }*/

    [Serializable]
    public class RegionLimits
    {
        public int MinLimitX { get; set; }
        public int MaxLimitX { get; set; }
        public int MinLimitY { get; set; }
        public int MaxLimitY { get; set; }

        public List<Pose> GetPosesWithinLimits()
        {
            List<Pose> poses = new List<Pose>();

            for (int i = this.MinLimitX; i <= this.MaxLimitX; ++i)
            {
                for (int j = this.MinLimitY; j <= this.MaxLimitY; ++j)
                {
                    poses.Add(new Pose(i, j));
                }
            }

            return poses;
        }

    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class MapObject : ICloneable
    {
        [Browsable(false)]
        public double[,] MapMatrix { get; set; }

        public int Rows { get { return MapMatrix.Rows();  } set { Resize(value, this.Columns); } }
        public int Columns { get { return MapMatrix.Columns(); } set { Resize(this.Rows, value); } }

        public static int IDs = 0;
        public int ID { get; }

        public MapObject(int rows, int cols)
        {
            MapMatrix = Matrix.Create<double>(rows, cols, 0.5);
            this.ID = ++IDs;
        }

        public void Resize(int nrow, int ncol)
        {
            MapObject nobj = new MapObject(nrow, ncol);
            nobj.SetAllPlace(0.5);
            int rowl = nrow > this.Rows ? this.Rows : nrow;
            int coll = ncol > this.Columns ? this.Columns : ncol;
            for (int i = 0; i < rowl; i++)
            {
                for (int j = 0; j < coll; j++)
                {
                    nobj.MapMatrix[i, j] = this.MapMatrix[i, j];
                }
            }

            this.MapMatrix = nobj.MapMatrix;
        }

        public void UpdateFromAnotherMap(MapObject map)
        {
            int nrow = map.Rows;
            int ncol = map.Columns;

            int rowl = nrow > this.Rows ? this.Rows : nrow;
            int coll = ncol > this.Columns ? this.Columns : ncol;

            for (int i = 0; i < rowl; i++)
            {
                for (int j = 0; j < coll; j++)
                {
                    if (Math.Abs(0.5 - map.MapMatrix[i, j]) > Math.Abs(0.5 - this.MapMatrix[i, j]))
                    {
                        this.MapMatrix[i, j] = map.MapMatrix[i, j];
                    }
                }
            }
        }
                
        public object Clone()
        {
            MapObject obj = new MapObject(this.Rows, this.Columns);
            for (int i = 0; i < this.Rows; i++)
            {
                for (int j = 0; j < this.Columns; j++)
                {
                    obj.MapMatrix[i, j] = this.MapMatrix[i, j];
                }
            }

            return obj;
        }

        public double GetPlace(int i, int j)
        {
            if (isOutOfBound(i,j))
            {
                return -1;
            }

            return this.MapMatrix[i, j];
        }

        public double GetPlace(Pose pose)
        {
            return GetPlace(pose.X, pose.Y);
        }

        public bool isOutOfBound(int i, int j)
        {
            if ((i < 0) || (i >= this.Rows) || (i < 0) || (i >= this.Columns))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public RegionLimits CalculateLimits(int i, int j, int fieldOfViewRadius)
        {
            RegionLimits limits = new RegionLimits();
            limits.MinLimitX = i - fieldOfViewRadius > 0 ? i - fieldOfViewRadius : 0;
            limits.MaxLimitX = i + fieldOfViewRadius < this.Rows ? i + fieldOfViewRadius : this.Rows-1;

            limits.MinLimitY = j - fieldOfViewRadius > 0 ? j - fieldOfViewRadius : 0;
            limits.MaxLimitY = j + fieldOfViewRadius < this.Columns ? j + fieldOfViewRadius : this.Columns-1;
            return limits;

        }

        public RegionLimits CalculateLimits(Pose pose, int fieldOfViewRadius)
        {
            return CalculateLimits(pose.X, pose.Y, fieldOfViewRadius);
        }

        public void SetAllPlace(double val)
        {
            for (int i = 0; i < this.Rows; ++i)
            {
                for (int j = 0; j < this.Columns; ++j)
                {
                    this.MapMatrix[i, j] = val;
                }
            }
        }

        public bool IsDiscovered(Platform p)
        {
            foreach (double d in MapMatrix)
            {
                if (!((d >= p.OccupiedThreshold) || (d <= p.FreeThreshold)))
                {
                    return false;
                }
            }

            return true;
        }

        public int NumDiscoveredBins()
        {
            int sum = 0;
            for (int i = 0; i < this.Rows; ++i)
            {
                for (int j = 0; j < this.Columns; ++j)
                {
                    if (this.MapMatrix[i, j] != 0.5)
                    {
                        sum++;
                    }
                }
            }

            return sum;
        }

        public int NumBins()
        {
            return this.Rows * this.Columns;
        }

    }
}
