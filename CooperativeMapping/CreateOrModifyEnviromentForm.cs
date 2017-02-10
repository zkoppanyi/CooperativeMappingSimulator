using Accord.IO;
using Accord.Math;
using CooperativeMapping.Communication;
using CooperativeMapping.ControlPolicy;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CooperativeMapping
{
    public partial class CreateOrModifyEnviromentForm : Form
    {
        private Enviroment enviroment;
        public Enviroment Enviroment { get { return enviroment; } }
        private double selectedBinType;

        public CreateOrModifyEnviromentForm(Enviroment enviroment)
        {
            InitializeComponent();
            this.enviroment = enviroment;
            updateUI();
            selectedBinType = 1;
        }

        private void CreateOrModifyEnviroment_Load(object sender, EventArgs e)
        {
            UpdateEnviromentPictureBox();
            propertyGridEnviroment.SelectedObject = enviroment;
        }

        public void UpdateEnviromentPictureBox()
        {
            mapImageBox.BackgroundImage = enviroment.Drawer.Draw(enviroment);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            UpdateEnviromentPictureBox();
        }

        private void propertyGridEnviroment_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateEnviromentPictureBox();
        }

        private void mapImageBox_Click(object sender, EventArgs e)
        {
            
        }

        private void updateUI()
        {
            toolStripStatusLabel.Text = "Status: - ";
            if (selectedBinType == 0)
            {
                toolStripStatusLabelSelectedBinType.Text = "Selected bin type: Discovered";
            }
            else if (selectedBinType == 0.5)
            {
                toolStripStatusLabelSelectedBinType.Text = "Selected bin type: Undiscovered";
            }
            else if (selectedBinType == 1)
            {
                toolStripStatusLabelSelectedBinType.Text = "Selected bin type: Obstacle";
            }

            toolStripButtonObstacle.BackColor = enviroment.Drawer.ColorObstacle;
            toolStripButtonUndiscovered.BackColor = enviroment.Drawer.ColorUndiscovered;
            toolStripButtonDiscovered.BackColor = enviroment.Drawer.ColorDiscovered;
            toolStripButtonPlatform.BackColor = enviroment.Drawer.ColorPlatform;
            UpdateEnviromentPictureBox();

        }

        private void mapImageBox_MouseUp(object sender, MouseEventArgs e)
        {
            double x = e.X;
            double y = e.Y;

            double width = mapImageBox.BackgroundImage.Width;
            double height = mapImageBox.BackgroundImage.Height;

            double x0 = x;
            if (width < mapImageBox.Width)
            {
                x0 = x + width / 2 - mapImageBox.Width / 2;
            }

            double y0 = y;
            if (height < mapImageBox.Height)
            {
                y0 = y + height / 2 - mapImageBox.Height / 2;
            }

            double binSize = enviroment.Drawer.BinSize;

            int j = (int)(x0 / binSize);
            int i = (int)(y0 / binSize);

            if (e.Button == MouseButtons.Left)
            {
                
                if ((i < 0) || (i >= enviroment.Map.Rows) || (j < 0) || (j >= enviroment.Map.Columns))
                {
                    toolStripStatusLabel.Text = "Bin is not found!";
                    return;
                }

                if (selectedBinType == -1)
                {
                    Platform platform = enviroment.Platforms.Find(p => ((p.Pose.X == i) && (p.Pose.Y == j)));

                    if (platform == null)
                    {
                        ControlPolicy.ControlPolicyAbstract cnt = new ClosestFronterierControlPolicy();
                        NearbyCommunicationModel comm = new NearbyCommunicationModel();
                        comm.Radius = 50;
                        platform = new Platform(enviroment, cnt, comm);
                        platform.FieldOfViewRadius = 20;
                        platform.Pose = new Pose(i, j);
                    }

                    PlatformSettings platformSettings = new PlatformSettings(platform, enviroment);
                    if (platformSettings.ShowDialog() != DialogResult.OK)
                    {
                        // TODO
                    }
                }
                else
                {
                    enviroment.Map.MapMatrix[i, j] = (int)selectedBinType;
                }

                UpdateEnviromentPictureBox();
                updateUI();
            }
            else if (e.Button == MouseButtons.Right)
            {
                toolStripStatusLabel.Text = "Status: X = " + i + " Y = " + j; 
            }
        }

        private void toolStripButtonObstacle_Click(object sender, EventArgs e)
        {
            selectedBinType = 1;
            updateUI();
        }

        private void toolStripButtonUndiscovered_Click(object sender, EventArgs e)
        {
            selectedBinType = 0.5;
            updateUI();
        }

        private void toolStripButtonDiscovered_Click(object sender, EventArgs e)
        {
            selectedBinType = 0;
            updateUI();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
        }

        private void toolStripButtonPlatform_Click(object sender, EventArgs e)
        {
            selectedBinType = -1;
            updateUI();
        }

        private void limitsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream stream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "bin files (*.bin)|*.bin|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((stream = saveFileDialog.OpenFile()) != null)
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    binaryFormatter.Serialize(stream, this.enviroment);
                    stream.Close();
                }
            }
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(enviroment.Map.Columns, enviroment.Map.Rows);

            RectangleF rectf = new RectangleF(1, 1, bmp.Width, bmp.Height);

            Graphics g = Graphics.FromImage(bmp);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            Font font = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
            g.DrawString("T h a n k  y o u   f o r  \nt h e  a t t e n t i o n !", font, Brushes.Black, rectf, stringFormat);
            g.Flush();

            Enviroment env = new Enviroment(bmp.Height, bmp.Width);

            for(int i=0; i< bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color c = bmp.GetPixel(i, j);
                    if (c != bmp.GetPixel(0, 0))
                    {
                        env.Map.MapMatrix[j, i] = 1;
                    }

                }
            }

            this.enviroment = env;
            propertyGridEnviroment.SelectedObject = enviroment;
            updateUI();
        }

        private void clearMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enviroment.Map.SetAllPlace(0);
            updateUI();
        }

        private void loadFromMATFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "MAT files (*.mat)|*.mat|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            MatReader reader = new MatReader(myStream);
                            var aType = reader["img_bin"].GetType();
                            byte[,] bins = reader["img_bin"].GetValue<byte[,]>();
                            //double[,] mapMatrix = Matrix.Create<double>(bins.Rows(), bins.Columns(), 0);
                            Enviroment env = new Enviroment(bins.Rows(), bins.Columns());
                            for (int i = 0; i < bins.Rows(); i++)
                            {
                                for (int j = 0; j < bins.Columns(); j++)
                                {
                                    if (bins[i,j] == 0)
                                    {
                                        env.Map.MapMatrix[i, j] = 1;
                                    }
                                    else
                                    {
                                        env.Map.MapMatrix[i, j] = 0;
                                    }
                                }
                            }

                            this.enviroment = env;
                            propertyGridEnviroment.SelectedObject = enviroment;
                            updateUI();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }

                
            }
        }

        private void changeAllPlatformToMaxInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(Platform p in enviroment.Platforms)
            {
                p.ControlPolicy = new MaxInformationGainControlPolicy();
            }
        }

        private void changeAllPolicyToCFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Platform p in enviroment.Platforms)
            {
                p.ControlPolicy = new ClosestFronterierControlPolicy();
            }

        }

        private void removeAllPlatformsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enviroment.Platforms.Clear();
        }

        private void changeAllToBidingPolicyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Platform p in enviroment.Platforms)
            {
                p.ControlPolicy = new BidingControlPolicy();
            }
        }
    }
}
