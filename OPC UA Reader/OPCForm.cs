using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace OPC_UA_Reader
{
    public partial class OPCForm : Form
    {
        public OPCForm()
        {
            InitializeComponent();



        }

        private void ReadButton_Click(object sender, EventArgs e)
        {

        }

        #region UI Configuration
        private void CollectorParametersDataGridView_TableLoad()
        {
            TagDataGridView.EnableHeadersVisualStyles = true;
            TagDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray;
            TagDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11, FontStyle.Regular);
            TagDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            TagDataGridView.RowHeadersDefaultCellStyle.BackColor = Color.DarkGray;
            TagDataGridView.RowHeadersVisible = false;
            TagDataGridView.ColumnHeadersVisible = false;

            //  *********************  Main Content Data Grid ********************
            // Font style
            this.TagDataGridView.DefaultCellStyle.Font = new Font("Tahoma", 11);

            // Default color scheme for the font color and background color
            this.TagDataGridView.DefaultCellStyle.ForeColor = Color.White;
            this.TagDataGridView.DefaultCellStyle.BackColor = Color.FromArgb(64, 64, 64);
            this.TagDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(64, 64, 64);
            this.TagDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // The selected cell in the grid will have yellow text on a black background
            this.TagDataGridView.DefaultCellStyle.SelectionForeColor = Color.White;  //Color.Yellow;
            this.TagDataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(64, 64, 64); //Color.Red;

            //Populate Data
            TagDataGridView.RowHeadersVisible = false;
            this.TagDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void Close_Button_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Minimise_Button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // For use in moving application around the screen from the HeaderPanel
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        private HashSet<Control> controlsToMove = new HashSet<Control>();

        private void OPCForm_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        #endregion

    }
}
