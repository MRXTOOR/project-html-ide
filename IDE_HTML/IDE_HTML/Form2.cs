using OfficeOpenXml;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace IDE_HTML
{
    public partial class Form2 : Form
    {
        private string connectionString = "Data Source=DESKTOP-6BFJ2BM;Initial Catalog=History_log;Integrated Security=SSPI;";
        private SqlConnection connection;
        private SqlCommand command;
        private DateTime startTime;
        private string filePath;

        public Form2()
        {
            InitializeComponent();
            connection = new SqlConnection(connectionString);
            command = new SqlCommand();
            command.Connection = connection;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;


        }

        private void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                string computerName = System.Environment.MachineName;
                startTime = DateTime.Now;

                // Сохраняем данные в базу данных
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("INSERT INTO user_activities (computer_name, start_time) VALUES (@computerName, @startTime)", connection);
                    command.Parameters.AddWithValue("@computerName", computerName);
                    command.Parameters.AddWithValue("@startTime", startTime);
                    command.ExecuteNonQuery();
                }

                // Загружаем данные из базы данных и выводим на экран
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } // Получаем системные параметры

        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM user_activities", connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void SaveData(string computerName, DateTime startTime, DateTime endTime, string filePath)
        {
            try
            {
                // открываем соединение с БД и создаем команду для выполнения запросов
                connection.Open();
                command.Connection = connection;

                // формируем SQL-запрос для сохранения данных
                string query = $"INSERT INTO user_activities (computer_name, start_time, end_time, file_path) VALUES ('{computerName}'," +
                    $" '{startTime.ToString("yyyy-MM-dd HH:mm:ss")}', '{endTime.ToString("yyyy-MM-dd HH:mm:ss")}', '{filePath}')";
                command.CommandText = query;
                command.ExecuteNonQuery();

                // закрываем соединение с БД
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                DateTime endTime = DateTime.Now;
                filePath = ((Form1)Application.OpenForms["Form1"]).GetFilePath();
                SaveData(Environment.MachineName, startTime, endTime, filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        //----------------------------------------



        private void ExportToExcel()
        {

            try
            {


                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                // создаем новый файл Excel
                var package = new ExcelPackage();

                // добавляем лист в файл
                var worksheet = package.Workbook.Worksheets.Add("User Activities");

                // заголовки столбцов
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Computer Name";
                worksheet.Cells[1, 3].Value = "Start Time";


                // получаем данные из базы данных
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("SELECT * FROM user_activities", connection);
                    SqlDataReader reader = command.ExecuteReader();

                    // заполняем ячейки в Excel файле
                    int row = 2;
                    while (reader.Read())
                    {
                        worksheet.Cells[row, 1].Value = reader["id"].ToString();
                        worksheet.Cells[row, 2].Value = reader["computer_name"].ToString();
                        worksheet.Cells[row, 3].Value = reader["start_time"].ToString();
                        row++;
                    }
                }

                // сохраняем файл Excel на диск и открываем его
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                saveFileDialog.FileName = "User Activities.xlsx";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var file = new FileInfo(saveFileDialog.FileName);
                    package.SaveAs(file);

                    MessageBox.Show("Data exported successfully!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }




        }

        private void btnExport_Click_1(object sender, EventArgs e)
        {
            ExportToExcel();
        }
    }
}