using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace EchoGrabber.GUI
{
    public partial class ProgramsViewer : UserControl
    {
        public ProgramsViewer()
        {
            InitializeComponent();
            LinkFilter = (s) => true;
        }
        private IEnumerable<IssueInfo> _programs;
        private Func<string, bool> LinkFilter;

        public void ShowLInks(IEnumerable<IssueInfo> infos)
        {
            _programs = infos;
            flowLayoutPanel1.Controls.Clear();
            flowLayoutPanel1.SuspendLayout();
            foreach (var link in infos)
            {
                var ll = new LinkLabel()
                {
                    Text = link.Title,
                    AutoSize = true,
                    Padding = new Padding(3),
                    ContextMenuStrip = contextMenuStrip1
                };
                ll.Links.Add(new LinkLabel.Link(0, ll.Text.Length, link.Url));
                var path = "eg.exe";
#if DEBUG
                path = System.IO.Path.GetFullPath("../../../EchoGrabber/bin/Debug/eg.exe");
#endif
                ll.LinkClicked += (s, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        Process.Start($"{path}", $"{e.Link.LinkData}");
                        e.Link.Visited = true;
                    }
                };
                flowLayoutPanel1.Controls.Add(ll);
            }
            flowLayoutPanel1.ResumeLayout();
        }
        #region Фильтр отображаемых значений
        private const string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        private void Filter()
        {
            var filtered = _programs.Where(p => LinkFilter(p.Title));
            ShowLInks(filtered);
        }

        //1-9
        private void filter1_9toolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkFilter = (s) => char.IsNumber(s[0]);
            Filter();
        }

        //А-Г
        private void filterA_GToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkFilter = (s) => Array.IndexOf(alphabet.Substring(0, 4).ToCharArray(), s.ToLower()[0]) != -1;
            Filter();
        }

        //Д-З
        private void filterD_ZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkFilter = (s) => Array.IndexOf(alphabet.Substring(4, 5).ToCharArray(), s.ToLower()[0]) != -1;
            Filter();
        }

        //И-М
        private void filterI_MToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkFilter = (s) => Array.IndexOf(alphabet.Substring(9, 5).ToCharArray(), s.ToLower()[0]) != -1;
            Filter();
        }

        //Н-Р
        private void filterN_RToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkFilter = (s) => Array.IndexOf(alphabet.Substring(14, 4).ToCharArray(), s.ToLower()[0]) != -1;
            Filter();
        }

        //Все
        private void nofiltertoolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkFilter = (s) => true;
            Filter();
        }

        //С-Ф
        private void filterS_FToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkFilter = (s) => Array.IndexOf(alphabet.Substring(18, 4).ToCharArray(), s.ToLower()[0]) != -1;
            Filter();
        }

        //Х-Ш
        private void filterKH_SHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkFilter = (s) => Array.IndexOf(alphabet.Substring(22, 4).ToCharArray(), s.ToLower()[0]) != -1;
            Filter();
        }

        //Щ-Я
        private void filterSCH_YAToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LinkFilter = (s) => Array.IndexOf(alphabet.Substring(26, 7).ToCharArray(), s.ToLower()[0]) != -1;
            Filter();
        }
        #endregion

        //Скачать всё
        private void downloadAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var control = contextMenuStrip1.SourceControl as LinkLabel;
            Grabber.DownloadAll(control.Text, control.Links[0].LinkData.ToString());
        }

    }
}
