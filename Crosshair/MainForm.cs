using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace Crosshair
{
	[Obsolete]
	public partial class MainForm : Form
	{
		[DllImport("user32.dll", EntryPoint = "GetWindowLong")]
		public static extern int GetWindowLong(IntPtr hWnd, GWL nIndex);

		[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
		public static extern int SetWindowLong(IntPtr hWnd, GWL nIndex, int dwNewLong);

		[DllImport("user32.dll", EntryPoint = "SetLayeredWindowAttributes")]
		public static extern bool SetLayeredWindowAttributes(IntPtr hWnd, int crKey, byte alpha, LWA dwFlags);


		private const int WM_WINDOWPOSCHANGING = 0x0046;
		private const int WM_GETMINMAXINFO = 0x0024;

		struct WindowPos
		{
			public IntPtr hwnd;
			public IntPtr hwndInsertAfter;
			public int x;
			public int y;
			public int width;
			public int height;
			public uint flags;
		}

		struct POINT
		{
			public int x;
			public int y;
		}

		struct MinMaxInfo
		{
			public POINT ptReserved;
			public POINT ptMaxSize;
			public POINT ptMaxPosition;
			public POINT ptMinTrackSize;
			public POINT ptMaxTrackSize;
		}

		public enum GWL
		{
			ExStyle = -20
		}

		public enum WS_EX
		{
			Transparent = 0x20,
			Layered = 0x80000
		}

		public enum LWA
		{
			ColorKey = 0x1,
			Alpha = 0x2
		}

		public MainForm()
		{
			InitializeComponent();
			
		}
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == WM_WINDOWPOSCHANGING)
			{
				WindowPos windowPos = (WindowPos)m.GetLParam(typeof(WindowPos));

				// Make changes to windowPos

				// Then marshal the changes back to the message
				Marshal.StructureToPtr(windowPos, m.LParam, true);
			}

			base.WndProc(ref m);

			// Make changes to WM_GETMINMAXINFO after it has been handled by the underlying
			// WndProc, so we only need to repopulate the minimum size constraints
			if (m.Msg == WM_GETMINMAXINFO)
			{
				MinMaxInfo minMaxInfo = (MinMaxInfo)m.GetLParam(typeof(MinMaxInfo));
				minMaxInfo.ptMinTrackSize.x = this.MinimumSize.Width;
				minMaxInfo.ptMinTrackSize.y = this.MinimumSize.Height;
				Marshal.StructureToPtr(minMaxInfo, m.LParam, true);
			}
		}
		
		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			int wl = GetWindowLong(this.Handle, GWL.ExStyle);
			wl = wl | 0x80000 | 0x20;
			SetWindowLong(this.Handle, GWL.ExStyle, wl);
			SetLayeredWindowAttributes(this.Handle, 0, 100, LWA.Alpha);

		}
		private void Form1_Load(object sender, EventArgs e)
		{
			GraphicsPath path = new GraphicsPath();
			path.AddEllipse(0, 0, this.Width, this.Height);
			Region region = new Region(path);
			this.Region = region;

			this.Top += 18;
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}
	}
}
