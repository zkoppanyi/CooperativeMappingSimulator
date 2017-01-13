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
            //textBoxConsole.Text = Matrix.ToString<int>(enviroment.Map.MapMatrix);
            mapImageBox.BackgroundImage = enviroment.Drawer.Draw(enviroment.Map);

            // Initialize robot and timer
            Controller rasterPlanningController = new RasterPathPlanningStrategy();
            Controller naiveController = new NaiveStrategyController();

            Platform robot1 = new Platform(enviroment, rasterPlanningController);
            robot1.Pose = new Pose(0, 0);            
            robot1.Measure();
            robot1.PlatformLogEvent += PlatformLogEvent;


            Platform robot2 = new Platform(enviroment, rasterPlanningController);
            robot2.Pose = new Pose(0, 1);
            robot2.Measure();
            robot2.Map = (MapObject)robot1.Map.Clone();
            robot2.PlatformLogEvent += PlatformLogEvent;

            Platform robot3 = new Platform(enviroment, naiveController);
            robot3.Pose = new Pose(0, 2);
            robot3.Measure();
            robot3.Map = robot1.Map;
            robot3.PlatformLogEvent += PlatformLogEvent;

            Platform robot4 = new Platform(enviroment, rasterPlanningController);
            robot4.Pose = new Pose(1, 0);
            robot4.Measure();
            robot4.Map = robot1.Map;
            robot4.PlatformLogEvent += PlatformLogEvent;

            Platform robot5 = new Platform(enviroment, rasterPlanningController);
            robot5.Pose = new Pose(1, 1);
            robot5.Measure();
            robot5.Map = robot1.Map;
            robot5.PlatformLogEvent += PlatformLogEvent;

            Platform robot6 = new Platform(enviroment, rasterPlanningController);
            robot6.Pose = new Pose(1, 2);
            robot6.Measure();
            robot6.Map = robot1.Map;
            robot6.PlatformLogEvent += PlatformLogEvent;

            selectedPlatform = robot1;
            updateUI();

            robotTimer.Start();
        }

        private void PlatformLogEvent(object sender, PlatformLogEventArgs e)
        {
            Platform pt = (Platform)sender;
            textBoxConsole.Text += System.Environment.NewLine + "ID #" + pt.ID + ": " + e.Message;
        }

        private void RobotTimer_Tick(object sender, EventArgs e)
        {
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
                        textBoxConsole.Text += System.Environment.NewLine +  "Collision with obstacle or other platform";
                        mapImageBox.BackgroundImage = enviroment.Drawer.Draw(enviroment);
                    }

                    robotTimer.Stop();
                    return;
                }
            }

            if (selectedPlatform != null)
            {
                mapImageBox.BackgroundImage = enviroment.Drawer.Draw(selectedPlatform);
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
            if (selectedPlatform != null)
            {
                mapImageBox.BackgroundImage = enviroment.Drawer.Draw(selectedPlatform);
            }
        }

        private void loadEnviromentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                FileStream stream = new FileStream(openFileDialog.FileName, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                this.enviroment = (Enviroment)formatter.Deserialize(stream);
                stream.Close();
                updateUI();
            }
        }
    }
}
