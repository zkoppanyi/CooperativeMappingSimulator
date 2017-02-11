using Accord.Math;
using System;
using System.Linq;
using System.Windows.Forms;
using CooperativeMapping.ControlPolicy;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CooperativeMapping.Communication;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CooperativeMapping
{
    public partial class MainForm : Form
    {
        private Timer robotTimer = new Timer();
        private Enviroment enviroment;
        private Platform selectedPlatform;
        public String currentEnviromentLocation = null;
        private Stopwatch measureRun = new Stopwatch();
        
        public MainForm()
        {
            InitializeComponent();
            updateUI();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            robotTimer.Interval = 10;
            robotTimer.Tick += RobotTimer_Tick;
            StartUpSimulation();

            this.textBoxConsole.Text += "Number Of Cores: " + Environment.ProcessorCount + System.Environment.NewLine;
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

            if (currentEnviromentLocation != null)
            {
                this.Text = "Swarm Mapping Simulator - " + currentEnviromentLocation;
            }
        }

        int displayIter = 0;
        private void StartUpSimulation()
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
            enviroment.Map.SetAllPlace(0);

            for (int l = 0; l < 3 * 12; l = l + 11)
            {
                for (int k = 0; k < 3 * 12; k = k + 11)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        enviroment.Map.MapMatrix[k + i, l + 6] = 1;
                    }

                    for (int i = 14; i > 5; i--)
                    {
                        enviroment.Map.MapMatrix[k + 10, l + i] = 1;
                    }

                    for (int i = 0; i < 10; i++)
                    {
                        enviroment.Map.MapMatrix[k + i, l + 14] = 1;
                    }
                }
            }


            // Show map
            mapImageBox.BackgroundImage = enviroment.Drawer.Draw(enviroment.Map, enviroment.Platforms);

            // Initialize robot and timer
            ControlPolicy.ControlPolicyAbstract policy = new MaxInformationGainControlPolicy();
            ControlPolicy.ControlPolicyAbstract priorityMapStrategy1 = new BidingControlPolicy();
            ControlPolicy.ControlPolicyAbstract priorityMapStrategy2 = new BidingControlPolicy();
            ControlPolicy.ControlPolicyAbstract priorityMapStrategy3 = new BidingControlPolicy();
            ControlPolicy.ControlPolicyAbstract priorityMapStrategy4 = new BidingControlPolicy();

            CommunicationModel commModel = new GlobalCommunicationModel();
            int FieldOfViewRadius = 5;

            // Priority map strategy
            Platform robot1 = new Platform(enviroment, new BidingControlPolicy(), commModel);
            robot1.Pose = new Pose(1, 2);
            robot1.FieldOfViewRadius = FieldOfViewRadius;
            robot1.Measure();
            robot1.PlatformLogEvent += PlatformLogEvent;

            Platform robot2 = new Platform(enviroment, new BidingControlPolicy(), commModel);
            robot2.Pose = new Pose(1, 4);
            robot2.Measure();
            robot2.FieldOfViewRadius = FieldOfViewRadius;
            robot2.PlatformLogEvent += PlatformLogEvent;

            /*Platform robot3 = new Platform(enviroment, new ClosestFronterierControlPolicy(), commModel);
            robot3.Pose = new Pose(3, 2);
            robot3.Measure();
            robot3.FieldOfViewRadius = FieldOfViewRadius;
            robot3.PlatformLogEvent += PlatformLogEvent;

            Platform robot4 = new Platform(enviroment, new ClosestFronterierControlPolicy(), commModel);
            robot4.Pose = new Pose(3, 4);
            robot4.Measure();
            robot4.FieldOfViewRadius = FieldOfViewRadius;
            robot4.PlatformLogEvent += PlatformLogEvent;*/

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
        }

        private void PlatformLogEvent(object sender, PlatformLogEventArgs e)
        {
            Platform pt = (Platform)sender;
            textBoxConsole.Text += System.Environment.NewLine + "ID #" + pt.ID + ": " + e.Message;
        }

        private void RobotTimer_Tick(object sender, EventArgs e)
        {
            measureRun.Restart();

            try
            {
                Parallel.ForEach(enviroment.Platforms, new ParallelOptions { MaxDegreeOfParallelism = 7 }, (plt) =>
                {
                    System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Highest;
                    plt.Next();
                });
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error occured: " + ex.ToString() + " " + ex.InnerException.ToString());
                robotTimer.Stop();
            }

            bool isMapDiscovered = true;
            foreach (Platform plt in enviroment.Platforms)
            {
                isMapDiscovered &= plt.Map.IsDiscovered(plt);

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

                bool val = plt.Map.IsDiscovered(plt);
                isMapDiscovered &= plt.Map.IsDiscovered(plt);
            }

            if (selectedPlatform != null)
            {
                displayIter++;

                if ((displayIter % 1) == 0)
                {
                    mapImageBox.BackgroundImage = enviroment.Drawer.Draw(selectedPlatform);
                }
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
            measureRun.Stop();
            toolStripStatusLabel.Text = "SUM: " + sumStep + " AVG: " + (double)sumStep / (double)enviroment.Platforms.Count() + " RunTime: " + measureRun.ElapsedMilliseconds;

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

            if (currentEnviromentLocation == null)
            {
                StartUpSimulation();
            }
            else
            {
                LoadEnviroment(currentEnviromentLocation);
            }
        }

        private void createEnviromentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Enviroment env = new Enviroment(10, 10);
            env.Map.SetAllPlace(0.5);
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
            textBoxConsole.Text += "Controller: " + System.Environment.NewLine + selectedPlatform.ControlPolicy.ToString() + System.Environment.NewLine;
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
                currentEnviromentLocation = openFileDialog.FileName;
                LoadEnviroment(currentEnviromentLocation);


            }
        }

        public void LoadEnviroment(String fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            this.enviroment = (Enviroment)formatter.Deserialize(stream);
            if (enviroment.Platforms.Count > 0)
            {
                selectedPlatform = enviroment.Platforms[0];
                foreach(Platform plt in enviroment.Platforms)
                {
                    plt.PlatformLogEvent += PlatformLogEvent;
                }
            }
            else
            {
                selectedPlatform = null;
            }

            textBoxConsole.Text += System.Environment.NewLine + "Enviroment Loaded: " + currentEnviromentLocation + System.Environment.NewLine;
            stream.Close();
            updateUI();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void startToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            robotTimer.Start();
        }

        private void savePlayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream stream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "pla files (*.pla)|*.pla|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((stream = saveFileDialog.OpenFile()) != null)
                {
                    ReplayObject obj = new ReplayObject();
                    obj.EnviromentPath = currentEnviromentLocation;
                    obj.FinalPlatforms = enviroment.Platforms;

                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    binaryFormatter.Serialize(stream, obj);
                    stream.Close();
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "pla files (*.pla)|*.pla|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                String location = openFileDialog.FileName;
                FileStream stream = new FileStream(location, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                ReplayObject replayObject = (ReplayObject)formatter.Deserialize(stream);
                LoadEnviroment(replayObject.EnviromentPath);

                foreach (Platform plt in replayObject.FinalPlatforms)
                {
                    Platform envPlatform = enviroment.Platforms.Find(p => p.Equals(plt));
                    //envPlatform = plt;

                    while (plt.ControlPolicy.Trajectory.Count > 0)
                    {
                        Pose p = plt.ControlPolicy.Trajectory.Pop();
                        envPlatform.ControlPolicy.CommandSequence.Push(p);
                    }
                }

            }
        }
    }
}
