using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping
{
    public static class MapDrawer
    {
        private static int squareSize = 10;
        private static int startX = 0;
        private static int startY = 0;

        private static Color colorUndiscovered = Color.FromArgb(100, Color.Gray);
        private static Color colorDiscovered = Color.FromArgb(10, Color.Gray);
        private static Color colorObstacle = Color.Black;
        private static Color colorRobot = Color.Blue;
        private static Color colorUnknown = Color.Red;

        public static Bitmap Drawer(MapObject map)
        {
            Bitmap bitmap = new Bitmap(map.Columns * squareSize, map.Rows * squareSize);
            Graphics g = Graphics.FromImage(bitmap);

            Pen blackPen = new Pen(Color.Black, 1);
            //Color customColor = Color.FromArgb(50, Color.Gray);
            //SolidBrush shadowBrush = new SolidBrush(customColor);
            
            for (int i = 0; i < map.Rows; i++)
            {
                for (int j = 0; j < map.Columns; j++)
                {
                    Rectangle rect = new Rectangle(startX + j * squareSize, startY + i * squareSize, squareSize, squareSize);

                    SolidBrush brush;
                    switch (map.MapMatrix[i, j])
                    {
                        case (int)MapPlaceIndicator.Undiscovered:
                            brush = new SolidBrush(colorUndiscovered);
                            break;

                        case (int)MapPlaceIndicator.Discovered:
                            brush = new SolidBrush(colorDiscovered);
                            break;

                        case (int)MapPlaceIndicator.Obstacle:
                            brush = new SolidBrush(colorObstacle);
                            break;

                        case (int)MapPlaceIndicator.Platform:
                            brush = new SolidBrush(colorRobot);
                            break;

                        case (int)MapPlaceIndicator.NoBackVisist:
                            brush = new SolidBrush(colorUnknown);
                            break;

                        default:
                            brush = new SolidBrush(colorUnknown);
                            break;
                    }

                    g.FillRectangles(brush, new Rectangle[] { rect });
                    g.DrawRectangle(blackPen, rect);
                }
            }
            blackPen = new Pen(Color.Black, 2);
            g.DrawRectangle(blackPen, startX, startY, map.Columns * squareSize, map.Rows * squareSize);

            return bitmap;
        }

        public static Bitmap Drawer(Platform platform)
        {
            MapObject mapCopy = (MapObject)platform.Map.Clone();
            mapCopy.MapMatrix[platform.Pose.X, platform.Pose.Y] = (int)MapPlaceIndicator.Platform;
            return Drawer(mapCopy);
        }

        public static Bitmap Drawer(Platform platform, Enviroment env)
        {
            MapObject mapCopy = (MapObject)platform.Map.Clone();
            mapCopy.RemovePlatforms();
            foreach (Platform p in env.Platforms)
            {
                mapCopy.MapMatrix[p.Pose.X, p.Pose.Y] = (int)MapPlaceIndicator.Platform;
            }
            return Drawer(mapCopy);
        }
    }
}
