using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;



namespace Email
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            AddForm femp = new AddForm(this);
            femp.Show(); //передаем ссылку на главную форму
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Emailtb.UpdateDG(dataGridView1);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = Emailtb.MessegeYesNo("Удалить запись?");

            if (result == DialogResult.Yes)
            {
                Emailtb.DeleteEmp(Emailtb.CurIndex(dataGridView1));
                Emailtb.UpdateDG(dataGridView1);
                this.TopMost = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            SendForm femp = new SendForm(this);
            femp.Show(); //передаем ссылку на главную форму
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Emailtb tmp = Emailtb.GetEmp(this, dataGridView1);
            using (emailContext db = new())
            {
                try
                {
                    //изменяем данные 
                    tmp.DataMes = null;

                    //применяем изменения
                    db.Update(tmp);

                }
                catch (Exception ex)
                {
                    Emailtb.MessegeOk(ex.Message);

                }

                //сохраняем с проверкой
                Emailtb.TruCathcSave(db, this, "Не удалось установить дату");

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            ExP femp = new ExP(this);
            femp.Show(); //передаем ссылку на главную форму

        }
    }
}
