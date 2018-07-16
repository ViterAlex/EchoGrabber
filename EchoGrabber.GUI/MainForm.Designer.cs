namespace EchoGrabber.GUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.onAirTabPage = new System.Windows.Forms.TabPage();
            this.archivedTabPage = new System.Windows.Forms.TabPage();
            this.actualProgramsViewer = new EchoGrabber.GUI.ProgramsViewer();
            this.archivedProgramsViewer = new EchoGrabber.GUI.ProgramsViewer();
            this.tabControl1.SuspendLayout();
            this.onAirTabPage.SuspendLayout();
            this.archivedTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.onAirTabPage);
            this.tabControl1.Controls.Add(this.archivedTabPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(550, 281);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 2;
            // 
            // onAirTabPage
            // 
            this.onAirTabPage.Controls.Add(this.actualProgramsViewer);
            this.onAirTabPage.Location = new System.Drawing.Point(4, 22);
            this.onAirTabPage.Name = "onAirTabPage";
            this.onAirTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.onAirTabPage.Size = new System.Drawing.Size(542, 255);
            this.onAirTabPage.TabIndex = 0;
            this.onAirTabPage.Text = "В эфире";
            this.onAirTabPage.UseVisualStyleBackColor = true;
            // 
            // archivedTabPage
            // 
            this.archivedTabPage.Controls.Add(this.archivedProgramsViewer);
            this.archivedTabPage.Location = new System.Drawing.Point(4, 22);
            this.archivedTabPage.Name = "archivedTabPage";
            this.archivedTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.archivedTabPage.Size = new System.Drawing.Size(542, 255);
            this.archivedTabPage.TabIndex = 1;
            this.archivedTabPage.Text = "Архивные";
            this.archivedTabPage.UseVisualStyleBackColor = true;
            // 
            // actualProgramsViewer
            // 
            this.actualProgramsViewer.AutoSize = true;
            this.actualProgramsViewer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.actualProgramsViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actualProgramsViewer.Location = new System.Drawing.Point(3, 3);
            this.actualProgramsViewer.MinimumSize = new System.Drawing.Size(200, 100);
            this.actualProgramsViewer.Name = "actualProgramsViewer";
            this.actualProgramsViewer.Size = new System.Drawing.Size(536, 249);
            this.actualProgramsViewer.TabIndex = 0;
            // 
            // archivedProgramsViewer
            // 
            this.archivedProgramsViewer.AutoSize = true;
            this.archivedProgramsViewer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.archivedProgramsViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.archivedProgramsViewer.Location = new System.Drawing.Point(3, 3);
            this.archivedProgramsViewer.MinimumSize = new System.Drawing.Size(200, 100);
            this.archivedProgramsViewer.Name = "archivedProgramsViewer";
            this.archivedProgramsViewer.Size = new System.Drawing.Size(536, 249);
            this.archivedProgramsViewer.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 281);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Подкасты «Эха Москвы»";
            this.tabControl1.ResumeLayout(false);
            this.onAirTabPage.ResumeLayout(false);
            this.onAirTabPage.PerformLayout();
            this.archivedTabPage.ResumeLayout(false);
            this.archivedTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage archivedTabPage;
        private System.Windows.Forms.TabPage onAirTabPage;
        private ProgramsViewer actualProgramsViewer;
        private ProgramsViewer archivedProgramsViewer;
    }
}

