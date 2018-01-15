/*
 * Stereo USB Sorter
 * Copyright (C) 2018, Kyle Repinski
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
namespace StereoUSBSorter
{
	partial class frmMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && ( components != null ) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
			this.lblSelectedDrive = new System.Windows.Forms.Label();
			this.btnApply = new System.Windows.Forms.Button();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.miFile = new System.Windows.Forms.ToolStripMenuItem();
			this.miFileOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.miOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.miOptionsAdvanced = new System.Windows.Forms.ToolStripMenuItem();
			this.miOptionsAdvancedSortFolders = new System.Windows.Forms.ToolStripMenuItem();
			this.miOptionsAdvancedSortFiles = new System.Windows.Forms.ToolStripMenuItem();
			this.miOptionsAdvancedChangeDates = new System.Windows.Forms.ToolStripMenuItem();
			this.miOptionsEnableLog = new System.Windows.Forms.ToolStripMenuItem();
			this.txtLog = new System.Windows.Forms.TextBox();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.treeContainer = new System.Windows.Forms.SplitContainer();
			this.tvHierarchy = new System.Windows.Forms.TreeView();
			this.lbSorting = new System.Windows.Forms.ListBox();
			this.fileSystemWatcher = new System.IO.FileSystemWatcher();
			this.menuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.treeContainer)).BeginInit();
			this.treeContainer.Panel1.SuspendLayout();
			this.treeContainer.Panel2.SuspendLayout();
			this.treeContainer.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
			this.SuspendLayout();
			// 
			// lblSelectedDrive
			// 
			this.lblSelectedDrive.AutoSize = true;
			this.lblSelectedDrive.Location = new System.Drawing.Point(12, 36);
			this.lblSelectedDrive.Name = "lblSelectedDrive";
			this.lblSelectedDrive.Size = new System.Drawing.Size(143, 13);
			this.lblSelectedDrive.TabIndex = 1;
			this.lblSelectedDrive.Text = "Selected Drive/Folder: None";
			// 
			// btnApply
			// 
			this.btnApply.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.btnApply.Location = new System.Drawing.Point(0, 276);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(627, 23);
			this.btnApply.TabIndex = 2;
			this.btnApply.Text = "&Apply";
			this.btnApply.UseVisualStyleBackColor = true;
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miOptions});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(627, 24);
			this.menuStrip.TabIndex = 3;
			this.menuStrip.Text = "menuStrip";
			// 
			// miFile
			// 
			this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileOpen});
			this.miFile.Name = "miFile";
			this.miFile.Size = new System.Drawing.Size(37, 20);
			this.miFile.Text = "&File";
			// 
			// miFileOpen
			// 
			this.miFileOpen.Name = "miFileOpen";
			this.miFileOpen.Size = new System.Drawing.Size(103, 22);
			this.miFileOpen.Text = "&Open";
			this.miFileOpen.Click += new System.EventHandler(this.miFileOpen_Click);
			// 
			// miOptions
			// 
			this.miOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miOptionsAdvanced,
            this.miOptionsEnableLog});
			this.miOptions.Name = "miOptions";
			this.miOptions.Size = new System.Drawing.Size(61, 20);
			this.miOptions.Text = "Options";
			// 
			// miOptionsAdvanced
			// 
			this.miOptionsAdvanced.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miOptionsAdvancedSortFolders,
            this.miOptionsAdvancedSortFiles,
            this.miOptionsAdvancedChangeDates});
			this.miOptionsAdvanced.Name = "miOptionsAdvanced";
			this.miOptionsAdvanced.Size = new System.Drawing.Size(132, 22);
			this.miOptionsAdvanced.Text = "Advanced";
			// 
			// miOptionsAdvancedSortFolders
			// 
			this.miOptionsAdvancedSortFolders.Checked = true;
			this.miOptionsAdvancedSortFolders.CheckState = System.Windows.Forms.CheckState.Checked;
			this.miOptionsAdvancedSortFolders.Name = "miOptionsAdvancedSortFolders";
			this.miOptionsAdvancedSortFolders.Size = new System.Drawing.Size(147, 22);
			this.miOptionsAdvancedSortFolders.Text = "Sort Folders";
			// 
			// miOptionsAdvancedSortFiles
			// 
			this.miOptionsAdvancedSortFiles.CheckOnClick = true;
			this.miOptionsAdvancedSortFiles.Name = "miOptionsAdvancedSortFiles";
			this.miOptionsAdvancedSortFiles.Size = new System.Drawing.Size(147, 22);
			this.miOptionsAdvancedSortFiles.Text = "Sort Files";
			// 
			// miOptionsAdvancedChangeDates
			// 
			this.miOptionsAdvancedChangeDates.CheckOnClick = true;
			this.miOptionsAdvancedChangeDates.Name = "miOptionsAdvancedChangeDates";
			this.miOptionsAdvancedChangeDates.Size = new System.Drawing.Size(147, 22);
			this.miOptionsAdvancedChangeDates.Text = "Change Dates";
			// 
			// miOptionsEnableLog
			// 
			this.miOptionsEnableLog.Checked = true;
			this.miOptionsEnableLog.CheckOnClick = true;
			this.miOptionsEnableLog.CheckState = System.Windows.Forms.CheckState.Checked;
			this.miOptionsEnableLog.Name = "miOptionsEnableLog";
			this.miOptionsEnableLog.Size = new System.Drawing.Size(132, 22);
			this.miOptionsEnableLog.Text = "Enable Log";
			this.miOptionsEnableLog.CheckStateChanged += new System.EventHandler(this.miOptionsEnableLog_CheckStateChanged);
			// 
			// txtLog
			// 
			this.txtLog.BackColor = System.Drawing.SystemColors.Info;
			this.txtLog.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.txtLog.Location = new System.Drawing.Point(0, 299);
			this.txtLog.MaxLength = 0;
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtLog.Size = new System.Drawing.Size(627, 93);
			this.txtLog.TabIndex = 4;
			this.txtLog.TabStop = false;
			// 
			// progressBar
			// 
			this.progressBar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.progressBar.Location = new System.Drawing.Point(0, 392);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(627, 23);
			this.progressBar.TabIndex = 5;
			// 
			// treeContainer
			// 
			this.treeContainer.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.treeContainer.Location = new System.Drawing.Point(0, 52);
			this.treeContainer.Name = "treeContainer";
			// 
			// treeContainer.Panel1
			// 
			this.treeContainer.Panel1.Controls.Add(this.tvHierarchy);
			// 
			// treeContainer.Panel2
			// 
			this.treeContainer.Panel2.Controls.Add(this.lbSorting);
			this.treeContainer.Size = new System.Drawing.Size(627, 224);
			this.treeContainer.SplitterDistance = 260;
			this.treeContainer.TabIndex = 9;
			// 
			// tvHierarchy
			// 
			this.tvHierarchy.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvHierarchy.Location = new System.Drawing.Point(0, 0);
			this.tvHierarchy.Name = "tvHierarchy";
			this.tvHierarchy.Size = new System.Drawing.Size(260, 224);
			this.tvHierarchy.TabIndex = 0;
			this.tvHierarchy.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvHierarchy_AfterSelect);
			// 
			// lbSorting
			// 
			this.lbSorting.AllowDrop = true;
			this.lbSorting.DisplayMember = "Text";
			this.lbSorting.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lbSorting.FormattingEnabled = true;
			this.lbSorting.Location = new System.Drawing.Point(0, 0);
			this.lbSorting.Name = "lbSorting";
			this.lbSorting.Size = new System.Drawing.Size(363, 224);
			this.lbSorting.TabIndex = 0;
			this.lbSorting.ValueMember = "Text";
			this.lbSorting.DragDrop += new System.Windows.Forms.DragEventHandler(this.lbSorting_DragDrop);
			this.lbSorting.DragOver += new System.Windows.Forms.DragEventHandler(this.lbSorting_DragOver);
			this.lbSorting.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbSorting_MouseDown);
			// 
			// fileSystemWatcher
			// 
			this.fileSystemWatcher.IncludeSubdirectories = true;
			this.fileSystemWatcher.SynchronizingObject = this;
			this.fileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(this.fileSystemEvent);
			this.fileSystemWatcher.Created += new System.IO.FileSystemEventHandler(this.fileSystemEvent);
			this.fileSystemWatcher.Deleted += new System.IO.FileSystemEventHandler(this.fileSystemEvent);
			this.fileSystemWatcher.Renamed += new System.IO.RenamedEventHandler(this.fileSystemEventRenamed);
			// 
			// frmMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(627, 415);
			this.Controls.Add(this.treeContainer);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.txtLog);
			this.Controls.Add(this.progressBar);
			this.Controls.Add(this.lblSelectedDrive);
			this.Controls.Add(this.menuStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip;
			this.MinimumSize = new System.Drawing.Size(300, 300);
			this.Name = "frmMain";
			this.Text = "Stereo USB Sorter";
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.treeContainer.Panel1.ResumeLayout(false);
			this.treeContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.treeContainer)).EndInit();
			this.treeContainer.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}

		#endregion
		private System.Windows.Forms.Label lblSelectedDrive;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem miFile;
		private System.Windows.Forms.ToolStripMenuItem miFileOpen;
		private System.Windows.Forms.ToolStripMenuItem miOptions;
		private System.Windows.Forms.ToolStripMenuItem miOptionsAdvanced;
		private System.Windows.Forms.ToolStripMenuItem miOptionsAdvancedSortFolders;
		private System.Windows.Forms.ToolStripMenuItem miOptionsAdvancedSortFiles;
		private System.Windows.Forms.ToolStripMenuItem miOptionsAdvancedChangeDates;
		private System.Windows.Forms.TextBox txtLog;
		private System.Windows.Forms.ToolStripMenuItem miOptionsEnableLog;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.SplitContainer treeContainer;
		private System.Windows.Forms.TreeView tvHierarchy;
		private System.IO.FileSystemWatcher fileSystemWatcher;
		private System.Windows.Forms.ListBox lbSorting;
	}
}
