using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace hw1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        private static string IPaddr = new IPAddress(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].Address).ToString();
        string myConnectionString = "SERVER =" + IPaddr + "; DATABASE=csharp_partice ; UID=root ; PASSWORD=Yisinglabuse ; charset=utf8;";
        DataTable data = new DataTable();

        public Window1()
        {
            InitializeComponent();
            data = getDataTable();
            list_table.ItemsSource = data.DefaultView ;
        }

        private DataTable getDataTable()
        {

            DataTable dt = new DataTable();
            string getData = "SELECT * FROM calculator";

            MySqlConnection conn = new MySqlConnection(myConnectionString);
            
            try { conn.Open(); }
            catch(Exception e)
            {
                MessageBox.Show("Cannot connect to database");
                return dt;
            }

            MySqlCommand getdata = new MySqlCommand(getData,conn);
            MySqlDataAdapter adapter = new MySqlDataAdapter(getdata);
            adapter.Fill(dt);
            conn.Close();

            return dt;
        }

        private void Goto_back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow next = new MainWindow();
            this.Close();
            next.Show();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

            DataRowView rows = list_table.SelectedItem as DataRowView;
            DataTable dataTable = data;

            //delete data base
            string inf = rows.Row["infix"].ToString();
            string query = "DELETE FROM calculator WHERE infix = '"+inf+"' ";
            MySqlConnection conn = new MySqlConnection(myConnectionString);
            conn.Open();
            MySqlCommand deleteData = new MySqlCommand(query, conn);
            int count = deleteData.ExecuteNonQuery();
            conn.Close();

            if (count == 0) MessageBox.Show("ERROR : cannot delete data");
            
            //delete data table
            int row = list_table.SelectedIndex;
            if (row == -1) return;
            dataTable.Rows[row].Delete();
            data = dataTable;

        }

    }
}
