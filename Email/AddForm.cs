using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Email
{
    public partial class AddForm : Form
    {
        MainForm form;
        public AddForm()
        {
            InitializeComponent();
        }

        public AddForm(MainForm f)
        {
            this.TopMost = true;
            form = f;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using(emailContext db = new emailContext())
            {
                try
                {
                    string? fio = textBox1.Text.Count() == 0 ? null : textBox1.Text;
                    string? email = textBox2.Text.Count() == 0 ? null : textBox2.Text;
                    int p;

                    switch (Regex.Replace(email, @"(\b\w+@)", ""))
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


                    Emailtb tmp = new Emailtb { Fio = fio, Email = email, Pr = p };

                    db.Emailtbs.Add(tmp);
                }

                
                catch (System.ArgumentNullException ex)
                {
                    Emailtb.MessegeOk("Заполните поля!!!");
                }

                catch (Exception ex)
                {

                    Emailtb.MessegeOk(ex.Message);

                }

                Emailtb.TruCathcSave(db, form, this, "Заполните все поля!");

            }
        }

        private void AddForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            form.Enabled = true;
            form.TopMost = true;
        }
    }
}
