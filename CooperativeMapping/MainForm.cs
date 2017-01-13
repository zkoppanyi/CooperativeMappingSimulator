using Accord.Math;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CooperativeMapping
{
    public partial class MainForm : Form
    {
        private Timer robotTimer = new Timer();
        private Enviroment enviroment;
        private List<Controller> controllers;
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            robotTimer.Interval = 100;
            robotTimer.Tick += RobotTimer_Tick;
            StartSimulation();
        }

        private void StartSimulation()
        {
            //// Initializing new enviroment
            //enviroment = new Enviroment(15, 15);

            //for (int i = 0; i < 6; i++)
            //{
            //    enviroment.Map.MapMatrix[i, 6] = (int)MapPlaceIndicator.Obstacle;
            //}

            //for (int i = 14; i > 5; i--)
            //{
            //    enviroment.Map.MapMatrix[10, i] = (int)MapPlaceIndicator.Obstacle;
            //}

            // Initializing new enviroment
            controllers = new List<Controller>();
            enviroment = new Enviroment(50, 60);

            for (int l = 0; l < 3 * 12; l = l + 11)
            {
                for (int k = 0; k < 3 * 12; k = k + 11)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        enviroment.Map.MapMatrix[k + i, l + 6] = (int)MapPlaceIndicator.Obstacle;
                    }

                    for (int i = 14; i > 5; i--)
                    {
                        enviroment.Map.MapMatrix[k + 10, l + i] = (int)MapPlaceIndicator.Obstacle;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        enviroment.Map.MapMatrix[k + i, l + 14] = (int)MapPlaceIndicator.Obstacle;
                    }
                }
            }


            // Show map
            //textBoxConsole.Text = Matrix.ToString<int>(enviroment.Map.MapMatrix);
            mapImageBox.BackgroundImage = MapDrawer.Drawer(enviroment.Map);

            double val = 0.3;
            double[,] priorityMap1 = Matrix.Create<double>(enviroment.Map.Rows, enviroment.Map.Columns, 1);
            for (int i = 0; i<enviroment.Map.Rows/2; i++)
            {
                for (int j = 0; j < enviroment.Map.Columns/2; j++)
                {
                    priorityMap1[i, j] = val;
                }

            }

            double[,] priorityMap2 = Matrix.Create<double>(enviroment.Map.Rows, enviroment.Map.Columns, 1);
            for (int i = enviroment.Map.Rows / 2; i < enviroment.Map.Rows; i++)
            {
                for (int j = 0; j < enviroment.Map.Columns/2; j++)
                {
                    priorityMap2[i, j] = val;
                }

            }

            double[,] priorityMap3 = Matrix.Create<double>(enviroment.Map.Rows, enviroment.Map.Columns, 1);
            for (int i = enviroment.Map.Rows / 2; i < enviroment.Map.Rows; i++)
            {
                for (int j = enviroment.Map.Columns / 2; j < enviroment.Map.Columns; j++)
                {
                    priorityMap3[i, j] = val;
                }

            }

            double[,] priorityMap4 = Matrix.Create<double>(enviroment.Map.Rows, enviroment.Map.Columns, 1);
            for (int i = 0; i < enviroment.Map.Rows / 2; i++)
            {
                for (int j = enviroment.Map.Columns / 2; j < enviroment.Map.Columns; j++)
                {
                    priorityMap4[i, j] = val;
                }

            }


            // Initialize robot and timer
            Platform robot1 = new Platform(enviroment);
            robot1.Pose = new Pose(0, 0);
            RasterPathPlanningStrategy controller1 = new RasterPathPlanningStrategy(robot1);
            //controller1.PriorityMap = priorityMap1;
            controllers.Add(controller1);
            robot1.Measure();
            robot1.PlatformLogEvent += PlatformLogEvent;

            Platform robot2 = new Platform(enviroment);
            robot2.Pose = new Pose(0, 1);
            robot2.Measure();
            RasterPathPlanningStrategy controller2 = new RasterPathPlanningStrategy(robot2);
            //controller2.PriorityMap = priorityMap2;
            controllers.Add(controller2);
            robot2.Map = robot1.Map;
            robot2.PlatformLogEvent += PlatformLogEvent;

            Platform robot3 = new Platform(enviroment);
            robot3.Pose = new Pose(0, 2);
            robot3.Measure();
            RasterPathPlanningStrategy controller3 = new RasterPathPlanningStrategy(robot3);
            //controller3.PriorityMap = priorityMap3;
            controllers.Add(controller3);
            robot3.Map = robot1.Map;
            robot3.PlatformLogEvent += PlatformLogEvent;

            Platform robot4 = new Platform(enviroment);
            robot4.Pose = new Pose(1, 0);
            robot4.Measure();
            RasterPathPlanningStrategy controller4 = new RasterPathPlanningStrategy(robot4);
            //controller4.PriorityMap = priorityMap4;
            controllers.Add(controller4);
            robot4.Map = robot1.Map;
            robot4.PlatformLogEvent += PlatformLogEvent;

            /*Platform robot5 = new Platform(enviroment);
            robot5.Pose = new Pose(1, 1);
            robot5.Measure();
            Controller controller5 = new RasterPathPlanningStrategy(robot5);
            controllers.Add(controller5);
            robot5.Map = robot1.Map;
            robot5.Color = Color.Red;
            robot5.PlatformLogEvent += PlatformLogEvent;*/

            //Platform robot6 = new Platform(enviroment);
            //robot6.Pose = new Pose(1, 2);
            //robot6.Measure();
            //Controller controller6 = new RasterPathPlanningStrategy(robot6);
            //controllers.Add(controller6);
            //robot6.Map = robot1.Map;
            //robot6.PlatformLogEvent += PlatformLogEvent;

            robotTimer.Start();
        }

        private void PlatformLogEvent(object sender, PlatformLogEventArgs e)
        {
            Platform pt = (Platform)sender;
            textBoxConsole.Text += System.Environment.NewLine + "ID #" + pt.ID + ": " + e.Message;
        }

        private void RobotTimer_Tick(object sender, EventArgs e)
        {
            bool isMapDiscovered = true;
            foreach (Controller controller in controllers)
            {
                //robot.Move(1, 1);
                controller.Next();

                // Check whether the platform is at the right position
                PlatformState platformState = enviroment.CheckPlatformState(controller.Platform);
                if (platformState != PlatformState.Healthy)
                {
                    textBoxConsole.Text += System.Environment.NewLine + "Robot destroyed! Cause: ";

                    if (platformState == PlatformState.OutOfBounderies)
                    {
                        textBoxConsole.Text += "Out of bounds!";
                    }

                    if (platformState == PlatformState.Destroy)
                    {
                        textBoxConsole.Text += System.Environment.NewLine + "Collision with obstacle or other platform";
                        mapImageBox.BackgroundImage = MapDrawer.Drawer(controller.Platform, enviroment);
                    }

                    robotTimer.Stop();
                    return;
                }

                mapImageBox.BackgroundImage = MapDrawer.Drawer(controller.Platform, enviroment);
                isMapDiscovered &= controller.Platform.Map.IsDiscovered();
            }

            if (isMapDiscovered)
            {
                robotTimer.Stop();
            }

            int sumStep = 0;
            if (isMapDiscovered) textBoxConsole.Text += System.Environment.NewLine + System.Environment.NewLine;
            foreach (Controller c in controllers)
            {
                if (isMapDiscovered) textBoxConsole.Text += "Step #" + c.Platform.ID + ": " + c.Platform.Step + System.Environment.NewLine;
                sumStep += c.Platform.Step;
            }
            if (isMapDiscovered) textBoxConsole.Text += "Sum step: " + sumStep + System.Environment.NewLine;
            if (isMapDiscovered) textBoxConsole.Text += "Average step: " + (double)sumStep / (double)controllers.Count() + System.Environment.NewLine;

            toolStripStatusLabel.Text = "SUM: " + sumStep + " AVG: " + (double)sumStep / (double)controllers.Count();

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
        }

        private void mapImageBox_Click(object sender, EventArgs e)
        {

        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            robotTimer.Start();

        }

        private void textBoxConsole_TextChanged(object sender, EventArgs e)
        {
            textBoxConsole.SelectionStart = textBoxConsole.TextLength;
            textBoxConsole.ScrollToCaret();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            robotTimer.Start();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            robotTimer.Stop();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            robotTimer.Stop();
            StartSimulation();
        }
    }
}
