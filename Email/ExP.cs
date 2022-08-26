using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelDataReader;
using System.Text.RegularExpressions;

namespace Email
{
    public partial class ExP : Form
    {
        MainForm form;
        public ExP()
        {
            InitializeComponent();
        }

        public ExP(MainForm f)
        {
            this.TopMost = true;
            form = f;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Excel Files|*.xlsx";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Выбор файла пользователем
                    var filePath = openFileDialog.FileName;
                    textBox1.Text = filePath;
                    textBox1.Enabled = false;

                    //Открываем файл для чтения считываем в дата сет
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    try
                    {
                        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                        {

                            var reader = ExcelReaderFactory.CreateReader(stream);
                            var dataSet = reader.AsDataSet();
                            var dataTable = dataSet.Tables[0];
                            int count = dataTable.Columns.Count;
                            if (count == 2)
                            {
                                dataGridView1.DataSource = dataTable;
                                dataGridView1.Columns[0].HeaderText = "Электронный адрес";
                                dataGridView1.Columns[0].Width = 160;
                                dataGridView1.Columns[1].HeaderText = "Имя";
                                dataGridView1.Columns[1].Width = 153;
                                button2.Enabled = true;
                            }
                            else
                            {
                                Emailtb.MessegeOk("Количество колонок в файле не равно 2");

                            }

                            this.TopMost = true;
                        }
                    }
                    catch (System.IO.IOException)
                    {
                        Emailtb.MessegeOk("Закройте выбранный файл");
                        this.TopMost = true;
                    }

                }
            }
        }

        private void ExP_FormClosed(object sender, FormClosedEventArgs e)
        {
            form.Enabled = true;
            form.TopMost = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Проверяем на совпадение данных из файла и данных в справочнике, если ФИО или email совпадают то исключаем запись,
            //остальные запись добавляем в справочник

            using (emailContext db = new emailContext())

            {
                for (int j = 0; j < dataGridView1.Rows.Count - 1; j++)
                {
                    string? emailtmp = dataGridView1[0, j].Value.ToString();
                    string? nametmp = dataGridView1[1, j].Value.ToString().ToLower();

                    if (nametmp != null && emailtmp != null)
                    {
                        var emp = db.Emailtbs
                                                .Where(e => e.Fio.Equals(nametmp) || e.Email.Equals(emailtmp))
                                                .ToList();

                        if (emp.Any())
                        {
                            dataGridView1.Rows.RemoveAt(j);
                            j--;

                        }

                        else
                        {
                            try
                            {
                                int p;

                                switch (Regex.Replace(emailtmp, @"(\b\w+@)", ""))
                                {
                                    case "mail.ru":
                                        p = 1;
                                        break;
                                    case "yandex.ru":
                                        p = 2;
                                        break;
                                    default:
                                        p = 3;
                                        break;

                                }

                                Emailtb empl = new Emailtb { Fio = nametmp, Email = emailtmp, Pr = p };
                                db.Emailtbs.Add(empl);
                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                form.TopMost = true;
                                Emailtb.MessegeOk(ex.Message);
                                break;
                            }
                        }

                    }
                }

                dataGridView1.Refresh();
                button2.Enabled = false;

                Emailtb.UpdateDG(form.dataGridView1);

            }
        }
    }
}
