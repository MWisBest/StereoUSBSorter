/*
 * Stereo USB Alphabetizer
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
using System.IO;
using System.Security;
using System.Windows.Forms;

namespace StereoUSBAlphabetizer
{
	public static class Util
	{
		private static readonly Random random = new Random();
		private static readonly object randomLock = new object();

		/// <summary>
		/// Gets a candidate for a new, empty, temporary directory inside a specified directory.
		/// 
		/// THIS METHOD DOES NOT CREATE THE DIRECTORY.
		/// </summary>
		/// <param name="root">temp directory root</param>
		/// <returns>possible temporary directory</returns>
		/// <exception cref="ArgumentException">fail ¯\_(ツ)_/¯</exception>
		/// <exception cref="ArgumentNullException">null argument supplied?</exception>
		/// <exception cref="SecurityException">one of many possible DirectoryInfo exceptions</exception>
		/// <exception cref="PathTooLongException">one of many possible DirectoryInfo exceptions</exception>
		public static DirectoryInfo getTempDir( DirectoryInfo root )
		{
			DirectoryInfo temp = new DirectoryInfo( Path.Combine( root.FullName, "temp" ) );

			// Do an if check instead of a simple while loop to avoid taking the lock unnecessarily
			if( temp.Exists )
			{
				// Random.Next is not thread-safe.
				// While we shouldn't be using this in multiple threads, better safe than sorry.
				lock( randomLock )
				{
					// We already determined temp exists, so just do-while
					do
					{
						string curFN = temp.FullName;
						temp = new DirectoryInfo( curFN + random.Next( 10 ) );
					}
					while( temp.Exists );
				}
			}

			return temp;
		}

		/// <summary>
		/// Attempts to determine if the selected drive is the system's root drive (e.x. "C:\")
		/// 
		/// TODO: Needs implementing on non-Windows operating systems.
		/// </summary>
		/// <param name="dir">directory to check</param>
		/// <returns>true if directory appears to be the system's root drive</returns>
		/// <exception cref="ArgumentException">fail ¯\_(ツ)_/¯</exception>
		/// <exception cref="PlatformNotSupportedException">Environment.GetFolderPath failure</exception>
		/// <exception cref="DirectoryNotFoundException">dir.GetDirectories failure</exception>
		/// <exception cref="SecurityException">dir.GetDirectories or dir.FullName failure</exception>
		/// <exception cref="UnauthorizedAccessException">dir.GetDirectories failure</exception>
		/// <exception cref="PathTooLongException">dir.FullName failure</exception>
		public static bool isSystemDrive( DirectoryInfo dir )
		{
			switch( Environment.OSVersion.Platform )
			{
				case PlatformID.Win32S:
				case PlatformID.Win32Windows:
				case PlatformID.Win32NT:
				case PlatformID.WinCE:
					DirectoryInfo[] dis = dir.GetDirectories();
					string[] badFolders =
					{
						Environment.GetFolderPath( Environment.SpecialFolder.Windows ),
						Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles ),
						Environment.GetFolderPath( Environment.SpecialFolder.ProgramFilesX86 )
					};
					foreach( DirectoryInfo di in dis )
					{
						if( Array.IndexOf( badFolders, di.FullName ) != -1 )
						{
							return true;
						}
					}
					return false;
				case PlatformID.Unix:
					if( dir.FullName == "/" || dir.FullName == "//" )
					{
						return true;
					}
					return false;
				case PlatformID.MacOSX:
					return false;
				default:
					return false;
			}
		}

		/// <summary>
		/// Shortcut for <see cref="MessageBox.Show(string, string, MessageBoxButtons, MessageBoxIcon)"/>
		/// Uses "Error", <see cref="MessageBoxButtons.OK"/>, and <see cref="MessageBoxIcon.Error"/>
		/// </summary>
		/// <param name="text">first parameter</param>
		/// <returns>MessageBox.Show result</returns>
		public static DialogResult showErrorMessageBox( string text )
		{
			return MessageBox.Show( text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
		}
	}
}
