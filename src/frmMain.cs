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
using System.Data;
using System.Drawing;
using System.IO;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StereoUSBSorter
{
	public partial class frmMain : Form
	{
		private DirectoryInfo selectedDirectory;
		private bool logEnabled = true;
		private bool hasUnsavedChanges = false;
		private bool isBusySorting = false;
		private bool disableRowChangedHandler = false;
		private object changingFileSystemEventStateLock = new object();
		private DataSet db;

		public frmMain()
		{
			InitializeComponent();
			// Add version number to the window title.
			string version = Application.ProductVersion;
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
		/// Disables the file system event watcher.
		/// 
		/// Locks if a delayed re-enable is pending.
		/// </summary>
		private void disableFileSystemEvents()
		{
			lock( this.changingFileSystemEventStateLock )
			{
				this.fileSystemWatcher.EnableRaisingEvents = false;
			}
		}

		/// <summary>
		/// Delays re-enabling the file system event watcher to ensure all pending changes are flushed to the disk
		/// </summary>
		/// <param name="msDelay">ms to delay</param>
		private void enableFileSystemEvents( int msDelay = 1000 )
		{
			lock( this.changingFileSystemEventStateLock )
			{
				Thread.Sleep( msDelay );
				this.fileSystemWatcher.EnableRaisingEvents = true;
			}
		}

		/// <summary>
		/// "Sorts" a directory for head units/other embedded devices to read in order that otherwise wouldn't.
		/// 
		/// This is done by simply moving a folder/file to a temporary directory, and then back to its original location,
		/// in the order specified by TreeNode.
		/// </summary>
		/// <param name="node">node to sort</param>
		private void sortTreeNodeDirectories( TreeNode node )
		{
			DirectoryInfo dir = (DirectoryInfo)node.Tag;
			bool isRootDir = node.Parent == null;
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
			float progressBarInc = 100 / node.Nodes.Count;

			for( int i = 0; i < node.Nodes.Count; ++i )
			{
				if( node.Nodes[i].Tag is DirectoryInfo subdir )
				{
					try
					{
						if( isRootDir )
						{
							this.writeToLog( Environment.NewLine + "Sorting Folder: " + subdir.Name );
						}
						else
						{
							this.writeToLog( "Sorting Folder: " + Path.Combine( subdir.Parent.Name, subdir.Name ) );
						}
					}
					catch
					{
						this.writeToLog( "WARNING: Failed to log folder being sorted? Attempting to continue..." );
					}

					try
					{
						subdir.MoveTo( Path.Combine( temp.FullName, subdir.Name ) );
					}
					catch
					{
						this.writeToLog( "WARNING: Failed to move \"" + subdir.Name + "\" to temporary directory! Skipping..." );
						goto loopSkipAhead;
					}

					try
					{
						subdir.MoveTo( Path.Combine( temp.Parent.FullName, subdir.Name ) );
					}
					catch
					{
						this.writeToLog( "MAJOR ERROR! Failed to move \"" + subdir.Name + "\" back from temporary directory! You may need to find/fix this or restore a backup!" );
						goto loopSkipAhead;
					}

				loopSkipAhead:
					try
					{
						if( node.Nodes[i].Nodes.Count > 0 )
						{
							sortTreeNodeDirectories( node.Nodes[i] );
						}
					}
					catch( Exception e )
					{
						this.writeToLog( "UNEXPECTED EXCEPTION OCCURRED! " + e.GetType().ToString() + ": " + e.Message );
						this.writeToLog( "ERROR: Major unhandled exception. Please copy this log and file a bug report." );
						this.writeToLog( "We will attempt to continue in hopes that this is benign." );
					}
				}
				else
				{
					this.writeToLog( "Odd error has occurred, Node Tag is not a DirectoryInfo?" );
				}

				// TODO: Fix progress bar to increment smoothly as sub-folders are progressed through as well, not just main folders.
				if( isRootDir )
				{
					this.progressBar.Value = (int)( progressBarInc * i );
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

		private TreeNode addNodesForDirectory( DirectoryInfo dir, TreeNode root )
		{
			TreeNode tn = new TreeNode()
			{
				Tag = dir,
				Text = dir.Name,
				Name = dir.Name
			};
			DirectoryInfo[] subdirs = dir.GetDirectories();
			for( int i = 0; i < subdirs.Length; ++i )
			{
				addNodesForDirectory( subdirs[i], tn );
			}
			if( root != null )
			{
				root.Nodes.Add( tn );
			}
			return tn;
		}

		private void fillInDataSet( DataSet set, TreeNodeCollection nodes )
		{
			for( int i = 0; i < nodes.Count; ++i )
			{
				set.Tables.Add( this.createDataTableFromTreeNode( nodes[i] ) );
				if( nodes[i].Nodes.Count > 0 )
				{
					fillInDataSet( set, nodes[i].Nodes );
				}
			}
		}

		private DataTable createDataTableFromTreeNode( TreeNode node )
		{
			DataTable ret = new DataTable( node.FullPath );
			DataColumn dirCol = new DataColumn( "Directory" )
			{
				DataType = typeof( string )
			};

			ret.Columns.Add( dirCol );

			DataColumn nodeCol = new DataColumn( "TreeNode" )
			{
				DataType = typeof( TreeNode ),
				ColumnMapping = MappingType.Hidden
			};

			ret.Columns.Add( nodeCol );

			for( int i = 0; i < node.Nodes.Count; ++i )
			{
				DataRow row = ret.NewRow();
				row["Directory"] = node.Nodes[i].Text;
				row["TreeNode"] = node.Nodes[i];
				ret.Rows.Add( row );
			}

			ret.RowChanged += this.tableRowChanged;
			return ret;
		}

		private void tableRowChanged( object sender, DataRowChangeEventArgs e )
		{
			if( !this.disableRowChangedHandler && e.Action == DataRowAction.Add && sender is DataTable table )
			{
				this.hasUnsavedChanges = true;
				this.tvHierarchy.BeginUpdate();
				TreeNode nodeMoved = (TreeNode)e.Row["TreeNode"];
				TreeNode parent = nodeMoved.Parent;
				int newIndex = table.Rows.IndexOf( e.Row );
				parent.Nodes.Remove( nodeMoved );
				parent.Nodes.Insert( newIndex, nodeMoved );
				this.tvHierarchy.EndUpdate();
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
			if( !this.miOptionsAdvancedSortFolders.Checked && !this.miOptionsAdvancedSortFiles.Checked )
			{
				goto optionsRationalityError;
			}
			if( this.isBusySorting )
			{
				goto alreadySorting;
			}

			if( !this.hasUnsavedChanges )
			{
				string alreadySavedText = "You don't appear to have any unsaved changes.";
				alreadySavedText += Environment.NewLine + "Would you like to continue anyway?";
				if( MessageBox.Show( alreadySavedText, "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Information ) != DialogResult.Yes )
				{
					return;
				}
			}

			string continueText = "Are you sure you want to continue?";
			continueText += Environment.NewLine + "Reordering drive/folder: " + this.selectedDirectory;
			continueText += Environment.NewLine + "This is potentially dangerous if the wrong drive is selected!!";
			if( MessageBox.Show( continueText, "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == DialogResult.Yes )
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

				// Note: disableFileSystemEvents() may lock waiting on a re-enable event.
				this.disableFileSystemEvents();

				try
				{
					this.writeToLog( "Begin Sorting: " + dirFullName );
					this.sortTreeNodeDirectories( this.tvHierarchy.Nodes[0] );
					this.writeToLog( "Finished Sorting: " + dirFullName );
					this.hasUnsavedChanges = false;
				}
				catch( Exception exc )
				{
					this.writeToLog( "UNEXPECTED EXCEPTION OCCURRED! " + exc.GetType().ToString() + ": " + exc.Message );
					this.writeToLog( "ERROR: Major unhandled exception. Please copy this log and file a bug report." );
				}
				finally
				{
					// Not all sorting changes may be flushed to the disk yet, so a delay is
					// required before re-enabling events (see this.enableFileSystemEvents())
					Task t = Task.Run( () => { this.enableFileSystemEvents( 1500 ); } );
				}

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
			FolderBrowserDialog fbd = new FolderBrowserDialog
			{
				ShowNewFolderButton = false,
				RootFolder = Environment.SpecialFolder.MyComputer,
				Description = "Select drive/root folder to 'alphabetize'"
			};
			if( fbd.ShowDialog() == DialogResult.OK )
			{
				this.hasUnsavedChanges = false;
				try
				{
					this.selectedDirectory = new DirectoryInfo( fbd.SelectedPath );
					this.fileSystemWatcher.Path = this.selectedDirectory.FullName;
					this.fileSystemWatcher.EnableRaisingEvents = true;
					this.db = new DataSet( this.selectedDirectory.Name );
					this.tvHierarchy.BeginUpdate();
					this.tvHierarchy.Nodes.Clear();
					this.tvHierarchy.Nodes.Add( this.addNodesForDirectory( this.selectedDirectory, null ) );
					this.fillInDataSet( this.db, this.tvHierarchy.Nodes );
					this.tvHierarchy.EndUpdate();
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

		private void tvHierarchy_AfterSelect( object sender, TreeViewEventArgs e )
		{
			this.dgvEditable.DataSource = this.db.Tables[e.Node.FullPath];
		}

		private void dgvEditable_Sorted( object sender, EventArgs e )
		{
			this.hasUnsavedChanges = true;
			this.tvHierarchy.BeginUpdate();
			DataGridViewWithDraggableRows control = (DataGridViewWithDraggableRows)sender;

			// Sorting the view for the table doesn't actually sort the underlying data like we want.
			// We could export the view to a temporary table and use that to sort only the tree, but that
			// breaks future drag'n'drop operations, so we must sort the actual data ourself with this ugly.
			DataTable table = (DataTable)control.DataSource;
			DataTable sortedTable = table.DefaultView.ToTable();
			// can't fire off row changed events here...
			this.disableRowChangedHandler = true;

			table.Rows.Clear();

			for( int i = 0; i < sortedTable.Rows.Count; ++i )
			{
				// NOTE: ImportRow does not work, the TreeNode loses its Parent etc; .NET BUG??
				table.LoadDataRow( sortedTable.Rows[i].ItemArray, true );
				TreeNode rowNode = (TreeNode)sortedTable.Rows[i]["TreeNode"];
				TreeNode parent = rowNode.Parent;
				parent.Nodes.Remove( rowNode );
				parent.Nodes.Insert( i, rowNode );
			}

			// re-enable row changed events
			this.disableRowChangedHandler = false;
			this.tvHierarchy.EndUpdate();
		}

		private void fileSystemEvent( object sender, FileSystemEventArgs e )
		{
			switch( e.ChangeType )
			{
				case WatcherChangeTypes.Deleted:
					this.writeToLog( "Received Deleted file system event. Path: " + e.FullPath );
					break;
				case WatcherChangeTypes.Created:
					this.writeToLog( "Received Created file system event. Path: " + e.FullPath );
					break;
				case WatcherChangeTypes.Changed:
					this.writeToLog( "Received Changed file system event. Path: " + e.FullPath );
					break;
				default:
					this.writeToLog( "Received unknown file system event " + (int)e.ChangeType );
					break;
			}
			this.writeToLog( "You may need to reload the program." );
		}

		private void fileSystemEventRenamed( object sender, RenamedEventArgs e )
		{
			this.writeToLog( "Received Changed file system event. Before: " + e.OldFullPath + "; After: " + e.FullPath );
			this.writeToLog( "You may need to reload the program." );
		}

		private void frmMain_FormClosing( object sender, FormClosingEventArgs e )
		{
			// Don't intercept force closes e.g. the OS is shutting down
			if( e.CloseReason == CloseReason.UserClosing )
			{
				if( this.hasUnsavedChanges )
				{
					string text = "You have not saved your changes!";
					text += Environment.NewLine + "Are you sure you want to exit?";
					if( MessageBox.Show( text, "Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation ) == DialogResult.No )
					{
						e.Cancel = true;
					}
				}
			}
		}
		#endregion
	}
}
