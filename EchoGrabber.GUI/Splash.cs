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

        protected async override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            var result = await new TaskFactory<DialogResult>().StartNew(() =>
            {
                try
                {
                    EchoPrograms.Actual = Grabber.GetPodcastLinks().ToList(); ;
                    EchoPrograms.Archived = Grabber.GetPodcastLinks("/programs/archived").ToList();
                }
                catch (Exception)
                {

                    return DialogResult.Cancel;
                }
                return DialogResult.OK;
            });
            DialogResult = result;
            Close();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.DrawImage(Properties.Resources.echo_logo, 0f, 0f, ClientRectangle.Width, ClientRectangle.Height);
        }


        internal static IEnumerable<IssueInfo> Actual { get; private set; }
        internal static IEnumerable<IssueInfo> Archived { get; private set; }
    }
}
