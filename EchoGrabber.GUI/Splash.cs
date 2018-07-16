using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EchoGrabber.GUI
{
    public partial class Splash : Form
    {
        public Splash()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Actual = Grabber.GetProgramLinks();
            Archived = Grabber.GetProgramLinks("/programs/archived");
            DialogResult = DialogResult.OK;
        }

        internal static IEnumerable<IssueInfo> Actual { get; private set; }
        internal static IEnumerable<IssueInfo> Archived { get; private set; }
    }
}
