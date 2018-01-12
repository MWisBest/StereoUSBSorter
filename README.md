# Stereo USB Sorter
This program sorts USB drives, SD cards, etc. for embedded devices, such as car stereo head units.

How it works is pretty simple. In layman's terms, a flash drive has a table listing all the folders on it, and when you add a new folder, it just gets tacked on to the end of the table. Simple embedded devices like a head unit read this table as-is, and don't sort it afterwards like your PC does. So to fix it, I go through all the folders in alphabetical order, I move them to a temporary folder, and then move them back. Effectively what this does to the table is move that folder to the end of the table. Do this in order to every folder, and it's sorted. This doesn't rewrite/stress the drive or anything, it only takes a few seconds to complete.

### License
This program is free software: you can redistribute it and/or modify it **under the terms of the GNU General Public License** as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
