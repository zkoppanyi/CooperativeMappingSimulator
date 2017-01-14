using CooperativeMapping.Communication;
using CooperativeMapping.Controllers;
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
        private MapPlaceIndicator selectedBinType;

        public CreateOrModifyEnviromentForm(Enviroment enviroment)
        {
            InitializeComponent();
            this.enviroment = enviroment;
            updateUI();
            selectedBinType = MapPlaceIndicator.Obstacle;
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
            if (selectedBinType == MapPlaceIndicator.Discovered)
            {
                toolStripStatusLabelSelectedBinType.Text = "Selected bin type: Discovered";
            }
            else if (selectedBinType == MapPlaceIndicator.Undiscovered)
            {
                toolStripStatusLabelSelectedBinType.Text = "Selected bin type: Undiscovered";
            }
            else if (selectedBinType == MapPlaceIndicator.Obstacle)
            {
                toolStripStatusLabelSelectedBinType.Text = "Selected bin type: Obstacle";
            }
            else if (selectedBinType == MapPlaceIndicator.OutOfBound)
            {
                toolStripStatusLabelSelectedBinType.Text = "Selected bin type: OutOfBound";
            }
            else if (selectedBinType == MapPlaceIndicator.Platform)
            {
                toolStripStatusLabelSelectedBinType.Text = "Selected bin type: Platform";
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

                if (selectedBinType == MapPlaceIndicator.Platform)
                {
                    Platform platform = enviroment.Platforms.Find(p => ((p.Pose.X == i) && (p.Pose.Y == j)));

                    if (platform == null)
                    {
                        Controller cnt = new RasterPathPlanningStrategy2();
                        CommunicationModel comm = new GlobalCommunicationModel();
                        platform = new Platform(enviroment, cnt, comm);
                        platform.FieldOfViewRadius = 5;
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
            selectedBinType = MapPlaceIndicator.Obstacle;
            updateUI();
        }

        private void toolStripButtonUndiscovered_Click(object sender, EventArgs e)
        {
            selectedBinType = MapPlaceIndicator.Undiscovered;
            updateUI();
        }

        private void toolStripButtonDiscovered_Click(object sender, EventArgs e)
        {
            selectedBinType = MapPlaceIndicator.Discovered;
            updateUI();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
        }

        private void toolStripButtonPlatform_Click(object sender, EventArgs e)
        {
            selectedBinType = MapPlaceIndicator.Platform;
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
                        env.Map.MapMatrix[j, i] = (int)MapPlaceIndicator.Obstacle;
                    }

                }
            }

            this.enviroment = env;
            propertyGridEnviroment.SelectedObject = enviroment;
            updateUI();
        }

        private void clearMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            enviroment.Map.SetAllPlace(MapPlaceIndicator.Undiscovered);
            updateUI();
        }
    }
}
