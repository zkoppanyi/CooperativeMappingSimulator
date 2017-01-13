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
using CooperativeMapping.Controllers;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CooperativeMapping.Communication;

namespace CooperativeMapping
{
    public partial class MainForm : Form
    {
        private Timer robotTimer = new Timer();
        private Enviroment enviroment;
        private Platform selectedPlatform;
        
        public MainForm()
        {
            InitializeComponent();
            updateUI();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            robotTimer.Interval = 100;
            robotTimer.Tick += RobotTimer_Tick;
            StartSimulation();
        }

        private void updateUI()
        {
            toolStripComboBoxMaps.Items.Clear();
            toolStripComboBoxMaps.SelectedItem = null;
            if (enviroment != null)
            {
                foreach (Platform ptf in enviroment.Platforms)
                {
                    toolStripComboBoxMaps.Items.Add(ptf);
                }
                toolStripComboBoxMaps.SelectedItem = selectedPlatform;
            }

            if (selectedPlatform != null)
            {
                mapImageBox.BackgroundImage = enviroment.Drawer.Draw(selectedPlatform);
            }
            else if (enviroment != null)
            {
                mapImageBox.BackgroundImage = enviroment.Drawer.Draw(enviroment);
            }
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
            mapImageBox.BackgroundImage = enviroment.Drawer.Draw(enviroment.Map);

            // Priority maps
            double val = 0.1;
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
            Controller rasterPlanningController = new RasterPathPlanningStrategy2();
            Controller naiveController = new NaiveStrategyController();
            Controller priorityMapStrategy1 = new RasterPathPlanningWithPriorityStrategy(priorityMap1);
            Controller priorityMapStrategy2 = new RasterPathPlanningWithPriorityStrategy(priorityMap2);
            Controller priorityMapStrategy3 = new RasterPathPlanningWithPriorityStrategy(priorityMap3);
            Controller priorityMapStrategy4 = new RasterPathPlanningWithPriorityStrategy(priorityMap4);

            CommunicationModel globComm = new GlobalCommunicationModel();



            // Priority map strategy
            Platform robot1 = new Platform(enviroment, priorityMapStrategy1, globComm);
            robot1.Pose = new Pose(0, 0);            
            robot1.Measure();
            robot1.PlatformLogEvent += PlatformLogEvent;

            Platform robot2 = new Platform(enviroment, priorityMapStrategy2, globComm);
            robot2.Pose = new Pose(0, 1);
            robot2.Measure();
            robot2.PlatformLogEvent += PlatformLogEvent;

            Platform robot3 = new Platform(enviroment, priorityMapStrategy3, globComm);
            robot3.Pose = new Pose(0, 2);
            robot3.Measure();
            robot3.PlatformLogEvent += PlatformLogEvent;

            Platform robot4 = new Platform(enviroment, priorityMapStrategy4, globComm);
            robot4.Pose = new Pose(1, 0);
            robot4.Measure();
            robot4.PlatformLogEvent += PlatformLogEvent;

            // Raster planning strategy
            /*Platform robot1 = new Platform(enviroment, rasterPlanningController, globComm);
            robot1.Pose = new Pose(0, 0);
            robot1.Measure();
            robot1.PlatformLogEvent += PlatformLogEvent;

            Platform robot2 = new Platform(enviroment, rasterPlanningController, globComm);
            robot2.Pose = new Pose(0, 1);
            robot2.Measure();
            robot2.PlatformLogEvent += PlatformLogEvent;

            Platform robot3 = new Platform(enviroment, rasterPlanningController, globComm);
            robot3.Pose = new Pose(0, 2);
            robot3.Measure();
            robot3.PlatformLogEvent += PlatformLogEvent;

            Platform robot4 = new Platform(enviroment, rasterPlanningController, globComm);
            robot4.Pose = new Pose(1, 0);
            robot4.Measure();
            robot4.PlatformLogEvent += PlatformLogEvent;*/

            /*Platform robot5 = new Platform(enviroment, rasterPlanningController, globComm);
            robot5.Pose = new Pose(1, 1);
            robot5.Measure();
            robot5.Color = Color.Red;
            robot5.PlatformLogEvent += PlatformLogEvent;

            Platform robot6 = new Platform(enviroment, naiveController, globComm);
            robot6.Pose = new Pose(1, 2);
            robot6.Measure();
            robot6.PlatformLogEvent += PlatformLogEvent;*/

            selectedPlatform = robot1;
            updateUI();

            //robotTimer.Start();
        }

        private void PlatformLogEvent(object sender, PlatformLogEventArgs e)
        {
            Platform pt = (Platform)sender;
            textBoxConsole.Text += System.Environment.NewLine + "ID #" + pt.ID + ": " + e.Message;
        }

        private void RobotTimer_Tick(object sender, EventArgs e)
        {
           bool isMapDiscovered = true;
            foreach (Platform plt in enviroment.Platforms)
            {
                //robot.Move(1, 1);
                plt.Next();

                // Check whether the platform is at the right position
                PlatformState platformState = enviroment.CheckPlatformState(plt);
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
                        mapImageBox.BackgroundImage = enviroment.Drawer.Draw(plt);
                    }

                    robotTimer.Stop();
                    return;
                }

                isMapDiscovered &= plt.Map.IsDiscovered();
            }

            if (selectedPlatform != null)
            {
                mapImageBox.BackgroundImage = enviroment.Drawer.Draw(selectedPlatform);
            }

            // Print coordinates at each step
            /*textBoxConsole.Text += System.Environment.NewLine;
            foreach (Platform plt in enviroment.Platforms)
            {
                textBoxConsole.Text += "ID: " + plt.ID + " X = " + plt.Pose.X + " Y = " + plt.Pose.Y + System.Environment.NewLine;
            }*/

                if (isMapDiscovered)
            {
                robotTimer.Stop();
            }

            int sumStep = 0;
            if (isMapDiscovered) textBoxConsole.Text += System.Environment.NewLine + System.Environment.NewLine;
            foreach (Platform plt in enviroment.Platforms)
            {
                if (isMapDiscovered) textBoxConsole.Text += "Step #" + plt.ID + ": " + plt.Step + System.Environment.NewLine;
                sumStep += plt.Step;
            }

            if (isMapDiscovered) textBoxConsole.Text += "Sum step: " + sumStep + System.Environment.NewLine;
            if (isMapDiscovered) textBoxConsole.Text += "Average step: " + (double)sumStep / (double)enviroment.Platforms.Count() + System.Environment.NewLine;
            toolStripStatusLabel.Text = "SUM: " + sumStep + " AVG: " + (double)sumStep / (double)enviroment.Platforms.Count();

            if (selectedPlatform != null)
            {
                toolStripStatusLabel.Text += " Discovered Area: " + ((double)selectedPlatform.Map.NumDiscoveredBins() / (double)selectedPlatform.Map.NumBins() * 100).ToString("0.00") + "%";
            }
            

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

        private void createEnviromentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Enviroment env = new Enviroment(10, 10);
            env.Map.SetAllPlace(MapPlaceIndicator.Undiscovered);
            CreateOrModifyEnviromentForm createOrModifyEnviromentForm = new CreateOrModifyEnviromentForm(env);
            createOrModifyEnviromentForm.ShowDialog();
            updateUI();

        }

        private void mapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsForm settings = new SettingsForm(enviroment.Drawer);
            settings.ShowDialog();
        }

        private void modifyEnviromentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateOrModifyEnviromentForm createOrModifyEnviromentForm = new CreateOrModifyEnviromentForm(this.enviroment);
            createOrModifyEnviromentForm.ShowDialog();
            updateUI();
        }

        private void toolStripComboBoxMaps_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripComboBoxMaps_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPlatform = (Platform)toolStripComboBoxMaps.SelectedItem;
            textBoxConsole.Text += System.Environment.NewLine;
            textBoxConsole.Text += "Platform #" + selectedPlatform.ID + System.Environment.NewLine;
            textBoxConsole.Text += "--------------------------------" + System.Environment.NewLine;
            textBoxConsole.Text += "Controller: " + System.Environment.NewLine + selectedPlatform.Controller.ToString() + System.Environment.NewLine;
            textBoxConsole.Text += "Communication: " + System.Environment.NewLine + selectedPlatform.CommunicationModel.ToString() + System.Environment.NewLine;
            textBoxConsole.Text += "Steps: " + selectedPlatform.Step + System.Environment.NewLine;
            textBoxConsole.Text += "Discovered area: " + selectedPlatform.Map.NumDiscoveredBins() + System.Environment.NewLine;
            textBoxConsole.Text += "Area: " + selectedPlatform.Map.NumBins() + System.Environment.NewLine;
            textBoxConsole.Text += "Discovered area ratio: " + ((double)selectedPlatform.Map.NumDiscoveredBins() / (double)selectedPlatform.Map.NumBins() * 100).ToString("0.00") + "%" + System.Environment.NewLine;

            if (selectedPlatform != null)
            {
                mapImageBox.BackgroundImage = enviroment.Drawer.Draw(selectedPlatform);
            }
        }

        private void loadEnviromentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "bin files (*.bin)|*.bin|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream stream = new FileStream(openFileDialog.FileName, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                this.enviroment = (Enviroment)formatter.Deserialize(stream);
                if (enviroment.Platforms.Count > 0)
                {
                    selectedPlatform = enviroment.Platforms[0];
                }
                else
                {
                    selectedPlatform = null;
                }
                stream.Close();
                updateUI();
            }
        }
    }
}
