using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
namespace EchoGrabber.GUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            archivedProgramsViewer.ShowLInks(Splash.Archived);
            actualProgramsViewer.ShowLInks(Splash.Actual);
            //_programs = Grabber.GetProgramLinks();
        }

        //public MainForm(IEnumerable<IssueInfo> programs, IEnumerable<IssueInfo> archived)
        //{
        //    InitializeComponent();
        //    //_programs = programs;
        //    //LinkFilter = (s) => true;
        //    //FindLinks(_programs);
        //}

    }
}
