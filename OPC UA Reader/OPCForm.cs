using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Opc.Ua;
using Opc.Ua.Configuration;

namespace OPC_UA_Reader
{
    public partial class OPCForm : Form
    {
        private DataTable _tagDataTable;

        // OPC variables
        private OPCUAClient _client;
        private ApplicationInstance _application;

        public OPCForm()
        {
            InitializeComponent();
            TagDataGridView_TableLoad();

            // Initialize DataTable used to hold tag information
            _tagDataTable = new DataTable();
            _tagDataTable.Columns.AddRange(new DataColumn[] {
                new DataColumn("Tag Address"),
                new DataColumn("Data Type"),
                new DataColumn("Value"),
                new DataColumn("Timestamp"),
                new DataColumn("Quality")
            });

            //AddDemoDetails();

            // Set DataTable as DataSource
            TagDataGridView.DataSource = _tagDataTable;


            // Initialize OPC UA Application Instance and load application configuration
            // from .xml file. Strictly speaking, this step is not necessary to create a 
            // connection with an OPC server as we will later see. However, it is good
            // practice to do so, and allows for the configuration to be easily loaded
            // and referenced in different parts of a proper program.
            _application = new ApplicationInstance
            {
                ApplicationName = "OPC UA Tag Reader",
                ApplicationType = ApplicationType.Client,
            };

            _application.LoadApplicationConfiguration("OpcUa.Config.xml", true);

        }

        private void ReadButton_Click(object sender, EventArgs e)
        {
            if (_client == null)
            {
                // Get IP address and port
                string ipAddress = IPAddressTextBox.Text;
                string port = PortTextBox.Text;

                _client = new OPCUAClient(ipAddress, port, _application.ApplicationConfiguration);
            }

            if (!_client.Connected)
            {
                // If the client hasn't connected something has gone wrong.
                // Reset client and return
                _client = null;
                return;
            }

            // Get server status and update TextBoxes
            ServerStatusDataType serverStatus = _client.GetServerStatus();

            ServerStatusTextBox.Text = serverStatus.State.ToString();
            ServerStartTimeTextBox.Text = serverStatus.StartTime.ToLocalTime().ToString();
            CurrentServerTimeTextBox.Text = serverStatus.CurrentTime.ToLocalTime().ToString();


            // Loop through each tag, get value and update row
            foreach (DataRow row in _tagDataTable.Rows)
            {
                DataValue dataValue = _client.GetValue(row["Tag Address"].ToString(), 2);

                if (dataValue == null)
                {
                    continue;
                }

                row["Data Type"] = dataValue.WrappedValue.TypeInfo.ToString();
                row["Value"] = dataValue.Value.ToString();
                row["Timestamp"] = dataValue.SourceTimestamp.ToLocalTime().ToString();
                row["Quality"] = StatusCode.IsGood(dataValue.StatusCode) ? "Good" : "Bad";
            }
        }

        #region Demo Details
        private void AddDemoDetails()
        {
            // Adds OPC details so I don't have to do it by hand every time
            IPAddressTextBox.Text = "127.0.0.1";
            PortTextBox.Text = "49320";

            DataRow row1 = _tagDataTable.NewRow();
            row1["Tag Address"] = "Channel1.Device1.Tag1";

            _tagDataTable.Rows.Add(row1);

            DataRow row2 = _tagDataTable.NewRow();
            row2["Tag Address"] = "Channel1.Device1.Tag2";

            _tagDataTable.Rows.Add(row2);

            DataRow row3 = _tagDataTable.NewRow();
            row3["Tag Address"] = "Channel1.Device1.Tag3";

            _tagDataTable.Rows.Add(row3);

        }
        #endregion

        #region UI Configuration
        private void TagDataGridView_TableLoad()
        {
            TagDataGridView.EnableHeadersVisualStyles = false;
            TagDataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            TagDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(64, 64, 64);
            TagDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11, FontStyle.Bold);
            TagDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            TagDataGridView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            TagDataGridView.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(64, 64, 64);
            TagDataGridView.RowHeadersVisible = false;
            TagDataGridView.ColumnHeadersVisible = true;

            TagDataGridView.BorderStyle = BorderStyle.FixedSingle;

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
            this.TagDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
