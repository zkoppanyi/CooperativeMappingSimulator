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
    public partial class CreateOrModifyEnviromentForm : Form
    {
        private Enviroment enviroment;
        public Enviroment Enviroment { get { return enviroment; } }

        public CreateOrModifyEnviromentForm(Enviroment enviroment)
        {
            InitializeComponent();
            this.enviroment = enviroment;
        }

        private void CreateOrModifyEnviroment_Load(object sender, EventArgs e)
        {
            UpdateEnviromentPictureBox();
            propertyGridEnviroment.SelectedObject = enviroment;
        }

        public void UpdateEnviromentPictureBox()
        {
            mapImageBox.BackgroundImage = enviroment.Drawer.Draw(Enviroment.Map);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            UpdateEnviromentPictureBox();
        }

        private void propertyGridEnviroment_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            UpdateEnviromentPictureBox();
        }
    }
}
