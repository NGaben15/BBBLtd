using Hotcakes.Commerce.Accounts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using Hotcakes.CommerceDTO.v1.Client;
using System.Diagnostics;

namespace BBBKliensAlkalmazas
{
    public partial class Form1 : Form
    {

        private HotcakesStore store = new HotcakesStore();

        DataGridViewCellStyle headerCellStyle = new DataGridViewCellStyle();
        DataGridViewCellStyle selectedCellStyle = new DataGridViewCellStyle();

        public Form1()
        {

            InitializeComponent();
            store.Init();
            headerCellStyle.BackColor = Color.AntiqueWhite;
            dataGridViewOrders.MultiSelect = false;
            dataGridViewOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            comboBoxMStatus.DataSource = store.Statuses;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            labelConnectivityStatus.Text = "Rendelések letöltése folyamatban...";
            store.UpdateDataFromHotcakes();
            labelConnectivityStatus.Text = "Kész";
            dataGridViewOrders.DataSource = store.Orders;
            Style();
        }

        private void Style()
        {
            dataGridViewOrders.RowHeadersVisible = false;
            dataGridViewOrders.ColumnHeadersDefaultCellStyle = headerCellStyle;
        }

        private void dataGridViewOrders_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                string selectedStatus = store.Orders[dataGridViewOrders.SelectedRows[0].Index].Status;
                comboBoxMStatus.Text = selectedStatus;
                string selectedOrderId = store.Orders[dataGridViewOrders.SelectedRows[0].Index].Id;

                UpdateOrderDetails(selectedOrderId);
            }
            catch 
            {               
            }

        }

        private void UpdateOrderDetails(string orderId)
        {
            labelConnectivityStatus.Text = "Rendelés termékei letöltése folyamatban...";
            dataGridViewOrderItems.DataSource = store.GetOrderItems(orderId);

            if (dataGridViewOrderItems.DataSource == null)
            {
                labelConnectivityStatus.Text = "Hiba lépett fel a termékek letöltése közben";
            }
            else
            {
                labelConnectivityStatus.Text = "Kész";
            }
        }

        private void buttonMSave_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedOrderId = store.Orders[dataGridViewOrders.SelectedRows[0].Index].Id;
                labelConnectivityStatus.Text = "Változások mentése...";
                bool done = store.SetOrderState(selectedOrderId, comboBoxMStatus.Text);
                if (done)
                {
                    labelConnectivityStatus.Text = "Kész";
                }
                else
                {
                    labelConnectivityStatus.Text = "Hiba lépett fel a mentés közben";
                }
            }
            catch
            {
            }
        }
    }
}
