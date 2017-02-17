using CooperativeMapping.ControlPolicy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Serializable]
    public class MapDrawer
    {
        [ReadOnly(false)]                            
        [Description("The square size of a bin on the map")]   
        [Category("Bin")]                      
        [DisplayName("Square Size")]
        public int BinSize { get; set; }

        [Browsable(false)]
        public int StartX { get; set; }

        [Browsable(false)]
        public int StartY { get; set; }

        [ReadOnly(false)]
        [Description("Color of the undiscovered bin")]
        [Category("Colors")]
        [DisplayName("Undiscovered Color")]
        public Color ColorUndiscovered { get; set; }

        [ReadOnly(false)]
        [Description("Color of the discovered bin")]
        [Category("Colors")]
        [DisplayName("Discovered Color")]
        public Color ColorDiscovered { get; set; }

        [ReadOnly(false)]
        [Description("Color of the obstacle bin")]
        [Category("Colors")]
        [DisplayName("Obstacle Color")]
        public Color ColorObstacle { get; set; }

        [ReadOnly(false)]
        [Description("Color of the platform bin")]
        [Category("Colors")]
        [DisplayName("Platform Color")]
        public Color ColorPlatform { get; set; }

        [ReadOnly(false)]
        [Description("Color of the unknown bin")]
        [Category("Colors")]
        [DisplayName("Unknown Color")]
        public Color ColorUnknown { get; set; }

        [ReadOnly(false)]
        [Description("True, if show distance map")]
        [Category("Display")]
        [DisplayName("Show distance map")]
        public bool ShowDistanceMap { get; set; }

        [ReadOnly(false)]
        [Description("True, if show the previous steps (breadcumbers)")]
        [Category("Display")]
        [DisplayName("Show bread cumbers")]
        public bool ShowBreadCumbers { get; set; }

        public MapDrawer()
        {
            BinSize = 10;
            StartX = 0;
            StartY = 0;

            ColorUndiscovered = Color.FromArgb(100, Color.Gray);
            ColorDiscovered = Color.FromArgb(10, Color.Gray);
            ColorObstacle = Color.Black;
            ColorPlatform = Color.Blue;
            ColorUnknown = Color.Red;

            this.ShowBreadCumbers = true;
            this.ShowDistanceMap = true;
    }

        public Bitmap Draw(MapObject map, List<Platform> platforms, Platform platform = null)
        {
            Bitmap bitmap = new Bitmap(map.Columns * BinSize, map.Rows * BinSize);
            Graphics g = Graphics.FromImage(bitmap);

            Pen blackPen = new Pen(Color.Black, 1);
            Pen platformPen = new Pen(ColorPlatform, 1);
            //Color customColor = Color.FromArgb(50, Color.Gray);
            //SolidBrush shadowBrush = new SolidBrush(customColor);

            // draw map structure
            for (int i = 0; i < map.Rows; i++)
            {
                for (int j = 0; j < map.Columns; j++)
                {
                    Rectangle rect = new Rectangle(StartX + j * BinSize, StartY + i * BinSize, BinSize, BinSize);

                    // frame for boxes
                    int val = 255 - (int)(map.MapMatrix[i, j] * 255);
                    SolidBrush brush = new SolidBrush(Color.FromArgb(val, val, val));
                    g.FillRectangles(brush, new Rectangle[] { rect });
                    //g.DrawRectangle(blackPen, rect);
                   
                    // draw distance map if it is applicable
                    if ((this.ShowDistanceMap) && (platform != null) && (platform.ControlPolicy is IDistanceMap))
                    {
                        IDistanceMap policy = platform.ControlPolicy as IDistanceMap;
                        double[,] distMap = policy.DistMap;
                        if (distMap != null)
                        {
                            if ((distMap[i, j] != Double.NegativeInfinity) && (distMap[i, j] != Double.PositiveInfinity))
                            {
                                double minDistMap = policy.MinDistMap - 1;
                                double maxDistMap = policy.MaxDistMap + 1;
                                if (maxDistMap != minDistMap)
                                {
                                    int val_map = (int)((distMap[i, j] - minDistMap) / (maxDistMap - minDistMap) * 100 + 1);

                                    brush = new SolidBrush(Color.FromArgb(255 - val_map, 0, 0));
                                    g.FillRectangles(brush, new Rectangle[] { rect });
                                }
                            }
                        }
                    }

                }
            }


            // draw platform spcific info
            if ((this.ShowBreadCumbers) && (platform != null))
            {
                ControlPolicy.ControlPolicyAbstract policy = platform.ControlPolicy;
                if (policy.Trajectory != null)
                {
                    foreach (Pose p in policy.Trajectory)
                    {
                        Rectangle rect = new Rectangle(StartX + p.Y * BinSize, StartY + p.X * BinSize, BinSize, BinSize);
                        g.FillRectangles(new SolidBrush(Color.Yellow), new Rectangle[] { rect });
                    }
                }

                if (policy.CommandSequence != null)
                {
                    foreach (Pose p in policy.CommandSequence)
                    {
                        Rectangle rect = new Rectangle(StartX + p.Y * BinSize, StartY + p.X * BinSize, BinSize, BinSize);
                        g.FillRectangles(new SolidBrush(Color.Orange), new Rectangle[] { rect });
                    }
                }

                if (policy.BestFronterier != null)
                {
                    Rectangle minPoseRect = new Rectangle(StartX + policy.BestFronterier.Y * BinSize, StartY + policy.BestFronterier.X * BinSize, BinSize, BinSize);
                    g.FillRectangles(new SolidBrush(Color.Orange), new Rectangle[] { minPoseRect });
                }

            }

            //draw platforms
            if (platforms != null)
            {
                foreach (Platform p in platforms)
                {
                    drawPlatform(g, p);
                }
            }

            if (platform != null)
            {
                drawPlatform(g, platform);
            }


            if ((platform != null) && (platform.ControlPolicy is IAllocationMap))
            {
                IAllocationMap policy = platform.ControlPolicy as IAllocationMap;
                if (policy.AllocationMap != null)
                {
                    for (int i = 0; i < map.Rows; i++)
                    {
                        for (int j = 0; j < map.Columns; j++)
                        {
                            if (policy.AllocationMap[i, j] == platform.ID)
                            {
                                Rectangle rect = new Rectangle(StartX + j * BinSize, StartY + i * BinSize, BinSize, BinSize);
                                g.DrawRectangle(platformPen, rect);
                            }
                        }
                    }
                }
            }

            // frame around the map to show extents
            blackPen = new Pen(Color.Black, 2);
            g.DrawRectangle(blackPen, StartX, StartY, map.Columns * BinSize, map.Rows * BinSize);

            return bitmap;
        }

        private void drawPlatform(Graphics g, Platform p)
        {
            int pt1 = StartX + p.Pose.Y * BinSize;
            int pt2 = StartY + p.Pose.X * BinSize;

            Rectangle rect = new Rectangle(pt1, pt2, BinSize, BinSize);
            Brush brush = new SolidBrush(ColorPlatform);
            g.FillRectangles(brush, new Rectangle[] { rect });

            Pen blackPen = new Pen(Color.Black, 2);
            int dx = (int)(Math.Sin(p.Pose.Heading / 180.0 * Math.PI) * BinSize);
            int dy = (int)(Math.Cos(p.Pose.Heading / 180.0 * Math.PI) * BinSize);
            g.DrawLine(blackPen, pt1 + BinSize / 2, pt2 + BinSize / 2, pt1 + BinSize / 2 + dx, pt2 + BinSize /2 + dy);

        }

        public Bitmap Draw(Platform platform)
        {
            /*MapObject mapCopy = (MapObject)platform.Map.Clone();
            mapCopy.MapMatrix[platform.Pose.X, platform.Pose.Y] = -1;
            foreach (Platform p in platform.ObservedPlatforms)
            {
                mapCopy.MapMatrix[p.Pose.X, p.Pose.Y] = -1;
            }*/

            return Draw(platform.Map, platform.ObservedPlatforms, platform);
        }      

        public Bitmap Draw(Enviroment enviroment)
        {
            /*MapObject mapCopy = (MapObject)enviroment.Map.Clone();

            foreach (Platform p in enviroment.Platforms)
            {
                mapCopy.MapMatrix[p.Pose.X, p.Pose.Y] = -1;
            }*/

            return Draw(enviroment.Map, enviroment.Platforms);
        }
    }
}
