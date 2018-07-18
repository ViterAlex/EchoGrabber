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
            archivedProgramsViewer.ShowLinks(EchoPrograms.Archived);
            actualProgramsViewer.ShowLinks(EchoPrograms.Actual);
        }
    }
}
