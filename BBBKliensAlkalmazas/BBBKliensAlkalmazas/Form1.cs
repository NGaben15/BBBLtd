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

using BBBKliensAlkalmazas.DataModels;

namespace BBBKliensAlkalmazas
{
    public partial class Form1 : Form
    {

        private HotcakesStore store = new HotcakesStore();
        private Search search = new Search();

        DataGridViewCellStyle headerCellStyle = new DataGridViewCellStyle();
        DataGridViewCellStyle selectedCellStyle = new DataGridViewCellStyle();

        private List<Order> filteredOrders = new List<Order>();

        private BindingSource bindingSource = new BindingSource();

        public Form1()
        {

            InitializeComponent();
            store.Init();
            headerCellStyle.BackColor = Color.AntiqueWhite;


            dataGridViewOrders.DataSource = bindingSource;

            dataGridViewOrders.MultiSelect = false;
            dataGridViewOrders.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewOrders.ReadOnly = true;

            dataGridViewOrderItems.MultiSelect = false;
            dataGridViewOrderItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewOrderItems.ReadOnly = true;

            comboBoxMStatus.DataSource = HotcakesStore.OrderStatuses;
        }


        private void dataGridViewOrders_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                var order = store.Orders[dataGridViewOrders.SelectedRows[0].Index];

                comboBoxMStatus.Text = order.Status;
                textBoxPrice.Text = order.Price.ToString();
                UpdateOrderDetails(order.Bvin);
            }
            catch (Exception ex)
            {
                // Azért szükséges, mert amíg a DataGrid nincs inicializálva, kifagyna az alkalmazás
            }

        }

        private void UpdateOrderDetails(string orderId)
        {
            labelConnectivityStatus.Text = "Rendelés termékei letöltése folyamatban...";
            
            List<Item> items = store.GetOrderItems(orderId);

            foreach (Item item in items)
            {
                Debug.WriteLine(item.Name, item.Quantity, item.Price);
            }

            dataGridViewOrderItems.DataSource = items;

            if (items == null)
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
                string selectedOrderBvin = store.Orders[dataGridViewOrders.SelectedRows[0].Index].Bvin;
                labelConnectivityStatus.Text = "Változások mentése...";
                bool done = store.SetOrderState(selectedOrderBvin, comboBoxMStatus.Text);
                if (done)
                {
                    store.Orders[dataGridViewOrders.SelectedRows[0].Index].Status = comboBoxMStatus.Text;
                    bindingSource.ResetBindings(false);
                    labelConnectivityStatus.Text = "Kész";
                }
                else
                {
                    labelConnectivityStatus.Text = "Hiba lépett fel a mentés közben";
                }
            }
            catch
            {
                // Azért szükséges, mert amíg a DataGrid nincs inicializálva, kifagyna az alkalmazás
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            labelConnectivityStatus.Text = "Rendelések letöltése folyamatban...";
            store.UpdateDataFromHotcakes();
            labelConnectivityStatus.Text = "Kész";
            bindingSource.DataSource = store.Orders;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            search.ShowDialog();
            
            filteredOrders = new List<Order>();

            foreach (Order order in store.Orders)
            {
                if (order.Date >= search.DateStart && order.Date <= search.DateEnd)
                {
                    if(search.Status != " ")
                    {
                        if (search.Status == order.Status)
                        {
                            filteredOrders.Add(order);
                        }
                    }
                    else
                    {
                        filteredOrders.Add(order);
                    }
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                bindingSource.DataSource = filteredOrders;
            }
            else
            {
                bindingSource.DataSource = store.Orders;
            }
        }
    }
}
