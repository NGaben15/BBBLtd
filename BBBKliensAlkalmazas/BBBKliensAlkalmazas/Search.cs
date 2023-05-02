using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BBBKliensAlkalmazas
{
    public partial class Search : Form
    {
        private string _status;
        private DateTime _dateStart;
        private DateTime _dateEnd;

        public string Status { get { return _status; } }
        public DateTime DateStart { get { return _dateStart; } }
        public DateTime DateEnd { get { return _dateEnd; } }


        public Search()
        {
            InitializeComponent();

            List<string> dataSource = new List<string>();

            dataSource.Add(" ");
            foreach  (string s in HotcakesStore.OrderStatuses) { dataSource.Add(s); }


            comboBoxSStatus.DataSource = dataSource;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            _status = comboBoxSStatus.Text;
            _dateStart = dateTimePickerSStart.Value;
            _dateEnd = dateTimePickerSEnd.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult= DialogResult.Cancel;
            Close();
        }
    }
}
