//------------------------------------------------------------------------------
// <copyright file="FileEnumerator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

/*
 * FileEnumerator class
 * 
 * Copyright (c) 2003 Microsoft Corporation
 *
 * Class to efficiently enumerate the files in a directory.  The only thing the framework provides
 * to do this is Directory.GetFiles(), which is unusable on large directories because it returns an
 * array containing all the file names at once (huge memory allocation).
 *
 * An efficient alternative is to use FindFirstFile/FindNextFile, which works but requires a lot
 * more code.  Also, it makes our code base harder to port to non-windows platforms.
 *
 * This FileEnumerator class solves both problem, by providing a simple and efficient wrapper.
 * By working with a single object, it is almost as efficient as calling FindFirstFile/FindNextFile,
 * but is much easier to use.  e.g. instead of:
 *
 *      UnsafeNativeMethods.WIN32_FIND_DATA wfd;
 *      IntPtr hFindFile = UnsafeNativeMethods.FindFirstFile(physicalDir + @"\*.*", out wfd);
 *
 *      if (hFindFile == INVALID_HANDLE_VALUE)
 *          return;
 *
 *      try {
 *          for (bool more=true; more; more=UnsafeNativeMethods.FindNextFile(hFindFile, out wfd)) {
 *
 *              // Skip false directories
 *              if (wfd.cFileName == "." || wfd.cFileName == "..")
 *                  continue;
 *              
 *              string fullPath = Path.Combine(physicalDir, wfd.cFileName);
 *
 *              ProcessFile(fullPath);
 *          }
 *      }
 *      finally {
 *          UnsafeNativeMethods.FindClose(hFindFile);
 *      }
 *
 * we can simply write
 *
 *      foreach (FileData fileData in FileEnumerator.Create(physicalDir)) {
 *          ProcessFile(fileData.FullName);
 *      }
 */


namespace System.Web.Util
{

	using System;
	using System.IO;
	using System.Collections;
	using System.Configuration;
	using System.Collections.Generic;
	using System.Runtime.InteropServices;
	using System.Linq;

	/*
	 * This is a somewhat artificial base class for FileEnumerator.  The main reason
	 * for it is to allow user code to be more readable, by looking like:
	 *      foreach (FileData fileData in FileEnumerator.Create(path)) { ... }
	 * instead of
	 *      foreach (FileEnumerator fileData in FileEnumerator.Create(path)) { ... }
	 */
	internal abstract class FileData
	{

		protected string _path;
		protected UnsafeNativeMethods.WIN32_FIND_DATA _wfd;
		protected FileSystemInfo[] infos = null;
		protected int index = 0;

		internal string Name
		{
			get { return infos != null ? infos[index].Name : _wfd.cFileName; }
		}

		internal string FullName
		{
			get { return infos != null ? infos[index].FullName : _path + Path.DirectorySeparatorChar + _wfd.cFileName; }
		}

		internal bool IsDirectory
		{
			get { return infos != null ? infos[index] is DirectoryInfo : (_wfd.dwFileAttributes & UnsafeNativeMethods.FILE_ATTRIBUTE_DIRECTORY) != 0; }
		}

		internal bool IsHidden
		{
			get { return infos != null ? ((infos[index].Attributes & FileAttributes.Hidden) != 0) : (_wfd.dwFileAttributes & UnsafeNativeMethods.FILE_ATTRIBUTE_HIDDEN) != 0; }
		}

		internal FindFileData GetFindFileData()
		{
			if (infos != null)
			{
				var info = infos[index];
				var data = new FindFileData();
				data.FileNameLong = info.Name;
				data.FileNameShort = info.Name;
				if (info is FileInfo) data.FileAttributesData = new FileAttributesData((FileInfo)info);
				return data;
			}
			return new FindFileData(ref _wfd);
		}
	}

	internal class FileEnumerator : FileData, IEnumerable, IEnumerator, IDisposable
	{
		private IntPtr _hFindFile = UnsafeNativeMethods.INVALID_HANDLE_VALUE;

		internal static FileEnumerator Create(string path)
		{
			return new FileEnumerator(path);
		}

		private FileEnumerator(string path)
		{
			_path = Path.GetFullPath(path);
		}

		~FileEnumerator()
		{
			infos = null;
			((IDisposable)this).Dispose();
		}

		// Should the current file be excluded from the enumeration
		private bool SkipCurrent()
		{

			// Skip false directories
			if (_wfd.cFileName == "." || _wfd.cFileName == "..")
				return true;

			return false;
		}

		// We just return ourselves for the enumerator, to avoid creating a new object
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this;
		}

		bool IEnumerator.MoveNext()
		{
			if (OSInfo.IsWindows)
			{
				for (; ; )
				{
					if (_hFindFile == UnsafeNativeMethods.INVALID_HANDLE_VALUE)
					{
						_hFindFile = UnsafeNativeMethods.FindFirstFile(_path + @"\*.*", out _wfd);

						// Empty enumeration case
						if (_hFindFile == UnsafeNativeMethods.INVALID_HANDLE_VALUE)
							return false;
					}
					else
					{
						bool hasMoreFiles = UnsafeNativeMethods.FindNextFile(_hFindFile, out _wfd);
						if (!hasMoreFiles)
							return false;
					}

					if (!SkipCurrent())
						return true;
				}
			} else
			{
				if (infos == null)
				{
					var dir = new DirectoryInfo(_path);
					if (dir.Exists) infos = dir.EnumerateFileSystemInfos("*.*", SearchOption.TopDirectoryOnly).ToArray();
					else infos = Enumerable.Empty<FileSystemInfo>().ToArray();
					index = 0;
				}
				else index++;
				return index < infos.Length;
			}
		}

		// The current object of the enumeration is always ourselves.  No new object created.
		object IEnumerator.Current
		{
			get { return this; }
		}

		void IEnumerator.Reset()
		{
			// We don't support reset, though it would be easy to add if needed
			throw new InvalidOperationException();
		}

		void IDisposable.Dispose()
		{
			infos = null;

			if (_hFindFile != UnsafeNativeMethods.INVALID_HANDLE_VALUE)
			{
				UnsafeNativeMethods.FindClose(_hFindFile);
				_hFindFile = UnsafeNativeMethods.INVALID_HANDLE_VALUE;
			}
			System.GC.SuppressFinalize(this);
		}
	}

}
