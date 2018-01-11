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
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Windows.Forms;

namespace StereoUSBSorter
{
	public partial class frmMain : Form
	{
		private DirectoryInfo selectedDirectory;
		private bool logEnabled = true;
		private bool isBusySorting = false;

		public frmMain()
		{
			InitializeComponent();
			// Add version number to the window title.
			string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			this.Text = this.Text + " v" + version.Substring( 0, version.LastIndexOf( '.' ) );
			// Hide individual file sorting and timestamp change buttons until implemented.
			this.miOptionsAdvancedSortFiles.Enabled = false;
			this.miOptionsAdvancedSortFiles.Visible = false;
			this.miOptionsAdvancedChangeDates.Enabled = false;
			this.miOptionsAdvancedChangeDates.Visible = false;
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault( false );
			Application.Run( new frmMain() );
		}

		public void writeToLog( string text )
		{
			if( this.logEnabled )
			{
				if( this.txtLog.Text != "" )
				{
					this.txtLog.AppendText( Environment.NewLine );
				}
				this.txtLog.AppendText( text );
				this.txtLog.SelectionStart = this.txtLog.Text.Length;
				this.txtLog.Refresh();
			}
		}

		/// <summary>
		/// "Sorts" a directory for head units/other embedded devices to read in alphabetical order that otherwise wouldn't.
		/// 
		/// This is done by simply moving a folder/file to a temporary directory, and then back to its original location, in alphabetical order.
		/// </summary>
		/// <param name="dir">directory to sort</param>
		/// <param name="sortFolders">whether or not to sort (move) folders</param>
		/// <param name="sortFiles">whether or not to sort (move) individual files</param>
		/// <param name="isRootDir">whether or not this is the first directory (controls progress bar)</param>
		private void sortDirectory( DirectoryInfo dir, bool sortFolders = true, bool sortFiles = false, bool isRootDir = false )
		{
			DirectoryInfo[] dis;
			string dirFullName;

			try
			{
				dirFullName = dir.FullName;
			}
			catch
			{
				this.writeToLog( "WARNING: Unable to read full name of input directory. Wat? Skipping..." );
				return;
			}

			try
			{
				dis = dir.GetDirectories();
			}
			catch( DirectoryNotFoundException )
			{
				this.writeToLog( "WARNING: Directory \"" + dirFullName + "\" has disappeared? Skipping..." );
				return;
			}
			catch( SystemException se ) when( se is UnauthorizedAccessException || se is SecurityException )
			{
				this.writeToLog( "WARNING: Not authorized to access directory \"" + dirFullName + "\". Skipping..." );
				return;
			}

			if( sortFolders )
			{
				dis = dis.OrderBy( x => x.Name ).ToArray();
			}

			DirectoryInfo temp;

			try
			{
				temp = Util.getTempDir( dir );
				temp.Create();
			}
			catch
			{
				this.writeToLog( "WARNING: Unable to create temp directory in \"" + dirFullName + "\". Skipping..." );
				return;
			}

			if( isRootDir )
			{
				this.progressBar.Value = 0;
				Application.DoEvents();
			}
			float progressBarInc = 100 / dis.Length;

			for( int i = 0; i < dis.Length; ++i )
			{
				if( sortFolders )
				{
					try
					{
						if( isRootDir )
						{
							this.writeToLog( Environment.NewLine + "Sorting Folder: " + dis[i].Name );
						}
						else
						{
							this.writeToLog( "Sorting Folder: " + Path.Combine( dis[i].Parent.Name, dis[i].Name ) );
						}
					}
					catch
					{
						this.writeToLog( "WARNING: Failed to log folder being sorted? Attempting to continue..." );
					}

					try
					{
						dis[i].MoveTo( Path.Combine( temp.FullName, dis[i].Name ) );
					}
					catch
					{
						this.writeToLog( "WARNING: Failed to move \"" + dis[i].Name + "\" to temporary directory! Skipping..." );
						goto loopSkipAhead;
					}

					try
					{
						dis[i].MoveTo( Path.Combine( temp.Parent.FullName, dis[i].Name ) );
					}
					catch
					{
						this.writeToLog( "MAJOR ERROR! Failed to move \"" + dis[i].Name + "\" back from temporary directory! You may need to find/fix this or restore a backup!" );
						goto loopSkipAhead;
					}
				}
			loopSkipAhead:
				// The following if checks are separated out so a failure in GetDirectories doesn't interfere with GetFiles and vice-versa
				bool sortNextDirFolders = false, sortNextDirFiles = false;

				if( sortFolders )
				{
					try
					{
						if( dis[i].GetDirectories().Length > 0 )
						{
							sortNextDirFolders = true;
						}
					}
					catch
					{
						this.writeToLog( "WARNING: Sub-folders in \"" + dis[i].FullName + "\" will not be able to be sorted due to an exception. Skipping..." );
					}
				}

				if( sortFiles )
				{
					try
					{
						if( dis[i].GetFiles().Length > 0 )
						{
							sortNextDirFiles = true;
						}
					}
					catch
					{
						this.writeToLog( "WARNING: Files in \"" + dis[i].FullName + "\" will not be able to be sorted due to an exception. Skipping..." );
					}
				}

				if( sortNextDirFolders || sortNextDirFiles )
				{
					try
					{
						sortDirectory( dis[i], sortNextDirFolders, sortNextDirFiles );
					}
					catch( Exception e )
					{
						this.writeToLog( "UNEXPECTED EXCEPTION OCCURRED! " + e.GetType().ToString() + ": " + e.Message );
						this.writeToLog( "ERROR: Major unhandled exception. Please copy this log and file a bug report." );
						this.writeToLog( "We will attempt to continue in hopes that this is benign." );
					}
				}

				// TODO: Fix progress bar to increment smoothly as sub-folders are progressed through as well, not just main folders.
				if( isRootDir )
				{
					this.progressBar.Value = (int)(progressBarInc * i);
					Application.DoEvents();
				}
			}

			try
			{
				temp.Delete();
			}
			catch( DirectoryNotFoundException )
			{
				this.writeToLog( "WARNING: Unable to remove temp directory \"" + temp.FullName + "\" because it no longer exists. Skipping..." );
			}
			catch
			{
				this.writeToLog( "WARNING: Unable to remove temp directory \"" + temp.FullName + "\". Skipping..." );
			}

			if( isRootDir )
			{
				this.progressBar.Value = 100;
				Application.DoEvents();
			}
		}

		#region Form Controls
		private void btnApply_Click( object sender, EventArgs e )
		{
			if( this.selectedDirectory == null )
			{
				goto dirNotSelected;
			}
			if( !this.selectedDirectory.Exists )
			{
				goto dirDoesntExist;
			}
			try
			{
				if( Util.isSystemDrive( this.selectedDirectory ) )
				{
					goto dirIsSystemDrive;
				}
			}
			catch
			{
				Util.showErrorMessageBox( "Error determining if drive is system drive. Aborting." );
				return;
			}
			if( !miOptionsAdvancedSortFolders.Checked && !miOptionsAdvancedSortFiles.Checked )
			{
				goto optionsRationalityError;
			}
			if( isBusySorting )
			{
				goto alreadySorting;
			}
			string dialogText = "Are you sure you want to continue?";
			dialogText += Environment.NewLine + "Reordering drive/folder: " + this.selectedDirectory;
			dialogText += Environment.NewLine + "This is potentially dangerous if the wrong drive is selected!!";
			if( MessageBox.Show( dialogText, "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.Yes )
			{
				Application.UseWaitCursor = true;
				Application.DoEvents();
				this.isBusySorting = true;
				string dirFullName;
				try
				{
					dirFullName = this.selectedDirectory.FullName;
				}
				catch
				{
					this.writeToLog( "ERROR: Couldn't read directory name. Something is clearly wrong, aborting..." );
					goto skipFinishedSorting;
				}
				this.writeToLog( "Begin Sorting: " + dirFullName );
				try
				{
					sortDirectory( this.selectedDirectory, this.miOptionsAdvancedSortFolders.Checked, this.miOptionsAdvancedSortFiles.Checked, true );
				}
				catch( Exception exc )
				{
					this.writeToLog( "UNEXPECTED EXCEPTION OCCURRED! " + exc.GetType().ToString() + ": " + exc.Message );
					this.writeToLog( "ERROR: Major unhandled exception. Please copy this log and file a bug report." );
				}
				this.writeToLog( "Finished Sorting: " + dirFullName );
			skipFinishedSorting:
				this.isBusySorting = false;
				Application.UseWaitCursor = false;
			}

			return;

		dirNotSelected:
			Util.showErrorMessageBox( "Drive hasn't been selected!" );
			return;

		dirDoesntExist:
			Util.showErrorMessageBox( "Drive/folder no longer exists!" + Environment.NewLine + "Did you unplug the drive?" );
			return;

		dirIsSystemDrive:
			Util.showErrorMessageBox( "You appear to have selected your system drive. Aborting..." );
			return;

		optionsRationalityError:
			Util.showErrorMessageBox( "Options rationality error. Must sort something..." );
			return;

		alreadySorting:
			Util.showErrorMessageBox( "Already busy sorting. Please wait." );
			return;
		}

		private void miFileOpen_Click( object sender, EventArgs e )
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.ShowNewFolderButton = false;
			fbd.RootFolder = Environment.SpecialFolder.MyComputer;
			fbd.Description = "Select drive/root folder to 'alphabetize'";
			if( fbd.ShowDialog() == DialogResult.OK )
			{
				try
				{
					this.selectedDirectory = new DirectoryInfo( fbd.SelectedPath );
					this.lblSelectedDrive.Text = "Selected Drive/Folder: " + fbd.SelectedPath;
				}
				catch
				{
					Util.showErrorMessageBox( "Error! Unable to open selected drive/folder." );
					return;
				}
			}
		}

		private void miOptionsEnableLog_CheckStateChanged( object sender, EventArgs e )
		{
			ToolStripMenuItem mi = (ToolStripMenuItem)sender;
			if( mi.CheckState == CheckState.Checked )
			{
				this.logEnabled = true;
				this.txtLog.BackColor = SystemColors.Info;
			}
			else
			{
				this.logEnabled = false;
				this.txtLog.BackColor = SystemColors.Control;
				this.txtLog.Text = "";
			}
		}
		#endregion
	}
}
