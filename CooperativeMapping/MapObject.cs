using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;
using System.ComponentModel;

namespace CooperativeMapping
{
    public enum MapPlaceIndicator
    {
        Undiscovered = 0,
        Discovered = 1,
        Obstacle = 2,
        Platform = 3,
        OutOfBound = 4,
        NoBackVisist = 5
    }

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
    public class MapObject : ICloneable
    {
        [Browsable(false)]
        public int[,] MapMatrix { get; set; }

        public int Rows { get { return MapMatrix.Rows();  } set { Resize(value, this.Rows, MapPlaceIndicator.Discovered); } }
        public int Columns { get { return MapMatrix.Columns(); } set { Resize(this.Rows, value, MapPlaceIndicator.Discovered); } }

        public MapObject(int rows, int cols)
        {
            MapMatrix = Matrix.Create<int>(rows, cols, (int)MapPlaceIndicator.Undiscovered);
        }

        public void Resize(int nrow, int ncol, MapPlaceIndicator indicator)
        {
            MapObject nobj = new MapObject(nrow, ncol);
            nobj.SetAllPlace(indicator);
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

        public MapPlaceIndicator GetPlace(int i, int j)
        {
            if (isOutOfBound(i,j))
            {
                return MapPlaceIndicator.OutOfBound;
            }

            return (MapPlaceIndicator)this.MapMatrix[i, j];
        }

        public MapPlaceIndicator GetPlace(Pose pose)
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

        public void RemovePlatforms()
        {
            // Delete the previous platform positions
            for (int i = 0; i < this.Rows; ++i)
            {
                for (int j = 0; j < this.Columns; ++j)
                {
                    if (this.GetPlace(i, j) == MapPlaceIndicator.Platform)
                    {
                        this.MapMatrix[i, j] = (int)MapPlaceIndicator.Discovered;
                    }
                }
            }
        }

        public void SetAllPlace(MapPlaceIndicator indicator)
        {
            for (int i = 0; i < this.Rows; ++i)
            {
                for (int j = 0; j < this.Columns; ++j)
                {
                    this.MapMatrix[i, j] = (int)indicator;
                }
            }
        }
    }
}
