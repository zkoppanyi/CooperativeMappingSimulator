using CooperativeMapping.Communication;
using CooperativeMapping.ControlPolicy;
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
    public partial class PlatformSettings : Form
    {
        public Platform Platform { get; }
        public Enviroment Enviroment { get; }

        public PlatformSettings(Platform platform, Enviroment enviroment)
        {
            InitializeComponent();
            this.Platform = platform;
            this.Enviroment = enviroment;

            comboBoxController.Items.Clear();
            comboBoxController.Items.Add(new NaiveStrategyControlPolicy());
            comboBoxController.Items.Add(new ClosestFronterierControlPolicy());
            comboBoxController.Items.Add(new MaxInformationGainControlPolicy());
            comboBoxController.Items.Add(new RasterPathPlanningWithPriorityMapStrategyController());
            comboBoxController.Items.Add(new RasterPathPlanningWithPriorityDirectionStrategyController());
            comboBoxController.Items.Add(Platform.Controller);
            comboBoxController.SelectedItem = Platform.Controller;

            comboBoxCommunicationModel.Items.Clear();
            comboBoxCommunicationModel.Items.Add(new NearbyCommunicationModel());
            comboBoxCommunicationModel.Items.Add(new GlobalCommunicationModel());
            comboBoxCommunicationModel.Items.Add(new NoCommunication());
            comboBoxCommunicationModel.Items.Add(Platform.CommunicationModel);
            comboBoxCommunicationModel.SelectedItem = Platform.CommunicationModel;

            propertyGrid.SelectedObject = this.Platform;
        }

        private void PlatformSettings_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void comboBoxController_SelectedIndexChanged(object sender, EventArgs e)
        {
            object sel = comboBoxController.SelectedItem;
            Platform.Controller = sel as ControlPolicy.ControlPolicyAbstract;
            
        }

        private void comboBoxMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            object sel = comboBoxCommunicationModel.SelectedItem;
            Platform.CommunicationModel = sel as CommunicationModel;            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Enviroment.Platforms.Remove(Platform);
            this.DialogResult = DialogResult.OK;
        }

        private void tabPageStrategy_Click(object sender, EventArgs e)
        {
            
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            propertyGridStrategy.SelectedObject = Platform.Controller;
            propertyGridCommunication.SelectedObject = Platform.CommunicationModel;
        }
    }
}
