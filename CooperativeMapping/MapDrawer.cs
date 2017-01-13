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

<<<<<<< HEAD
        public static Bitmap Drawer(MapObject map, Enviroment enviroment = null, Platform platform = null)
=======
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
    }

        public Bitmap Draw(MapObject map)
>>>>>>> 16cd8984a20e6f1e6a2f97efa4fb6dec56704218
        {
            Bitmap bitmap = new Bitmap(map.Columns * BinSize, map.Rows * BinSize);
            Graphics g = Graphics.FromImage(bitmap);

            Pen blackPen = new Pen(Color.Black, 1);
            //Color customColor = Color.FromArgb(50, Color.Gray);
            //SolidBrush shadowBrush = new SolidBrush(customColor);
            
            for (int i = 0; i < map.Rows; i++)
            {
                for (int j = 0; j < map.Columns; j++)
                {
                    Rectangle rect = new Rectangle(StartX + j * BinSize, StartY + i * BinSize, BinSize, BinSize);

                    SolidBrush brush;
                    switch (map.MapMatrix[i, j])
                    {
                        case (int)MapPlaceIndicator.Undiscovered:
                            brush = new SolidBrush(ColorUndiscovered);
                            break;

                        case (int)MapPlaceIndicator.Discovered:
                            brush = new SolidBrush(ColorDiscovered);
                            break;

                        case (int)MapPlaceIndicator.Obstacle:
                            brush = new SolidBrush(ColorObstacle);
                            break;

                        case (int)MapPlaceIndicator.Platform:
<<<<<<< HEAD
                            brush = new SolidBrush(colorRobot);
                            if (platform != null)
                            {
                                brush = new SolidBrush(platform.Color);
                            }
                            if (enviroment != null)
                            {
                                foreach(Platform plt in enviroment.Platforms)
                                {
                                    if ((plt.Pose.X == i) && (plt.Pose.Y == j))
                                    {
                                        brush = new SolidBrush(plt.Color);
                                        break;
                                    }
                                }
                            }

=======
                            brush = new SolidBrush(ColorPlatform);
>>>>>>> 16cd8984a20e6f1e6a2f97efa4fb6dec56704218
                            break;

                        case (int)MapPlaceIndicator.NoBackVisist:
                            brush = new SolidBrush(ColorUnknown);
                            break;

                        default:
                            brush = new SolidBrush(ColorUnknown);
                            break;
                    }

                    g.FillRectangles(brush, new Rectangle[] { rect });
                    g.DrawRectangle(blackPen, rect);
                }
            }
            blackPen = new Pen(Color.Black, 2);
            g.DrawRectangle(blackPen, StartX, StartY, map.Columns * BinSize, map.Rows * BinSize);

            return bitmap;
        }

        public Bitmap Draw(Platform platform)
        {
            MapObject mapCopy = (MapObject)platform.Map.Clone();
            mapCopy.MapMatrix[platform.Pose.X, platform.Pose.Y] = (int)MapPlaceIndicator.Platform;
<<<<<<< HEAD
            return Drawer(mapCopy, null, platform);
        }

        public static Bitmap Drawer(Platform platform, Enviroment enviroment)
        {
            MapObject mapCopy = (MapObject)platform.Map.Clone();
            mapCopy.RemovePlatforms();
=======
            foreach (Platform p in platform.ObservedPlatforms)
            {
                mapCopy.MapMatrix[p.Pose.X, p.Pose.Y] = (int)MapPlaceIndicator.Platform;
            }

            return Draw(mapCopy);
        }      

        public Bitmap Draw(Enviroment enviroment)
        {
            MapObject mapCopy = (MapObject)enviroment.Map.Clone();

>>>>>>> 16cd8984a20e6f1e6a2f97efa4fb6dec56704218
            foreach (Platform p in enviroment.Platforms)
            {
                mapCopy.MapMatrix[p.Pose.X, p.Pose.Y] = (int)MapPlaceIndicator.Platform;
            }
<<<<<<< HEAD
            return Drawer(mapCopy, enviroment);
=======

            return Draw(mapCopy);
>>>>>>> 16cd8984a20e6f1e6a2f97efa4fb6dec56704218
        }
    }
}
