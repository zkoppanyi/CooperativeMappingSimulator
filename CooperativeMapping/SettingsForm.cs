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
    public partial class SettingsForm : Form
    {
        public object ObjectInst {
            get { return propertyGrid.SelectedObject;  }
            set { propertyGrid.SelectedObject = value; }
        }

        public SettingsForm(object objectInst)
        {
            InitializeComponent();
            ObjectInst = objectInst;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
