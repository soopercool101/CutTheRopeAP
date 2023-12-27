using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CutTheRope.windows
{
	internal static class NativeMethods
	{
		public static Cursor LoadCustomCursor(string path)
		{
			IntPtr intPtr = LoadCursorFromFile(path);
			if (intPtr == IntPtr.Zero)
			{
				throw new Win32Exception();
			}
			Cursor cursor = new Cursor(intPtr);
			/*
			FieldInfo field = typeof(Cursor).GetField("ownHandle", BindingFlags.Instance | BindingFlags.NonPublic);
			field.SetValue(cursor, true);
			*/
			return cursor;
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr LoadCursorFromFile(string path);
	}
}
