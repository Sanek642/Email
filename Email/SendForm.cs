using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Email
{
    public partial class SendForm : Form
    {
        List<string> dname = new List<string>{ "gmail.com", "mail.ru", "yandex.ru", "masterhost.ru", "timeweb.ru", "beget.com", "nic.ru", "jino.ru", "rambler.ru "};
        MainForm form;
        List<Emailtb> mail;
        DateTime date;

        string login;
        string pass;
        string smtp;
        string title;
        string body;
        int pr;

        public SendForm()
        {
            InitializeComponent();
        }

        public SendForm(MainForm f)
        {
            this.TopMost = true;
            form = f;
            InitializeComponent();
        }

        private void SendForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            form.Enabled = true;
            form.TopMost = true;
        }

        private void SendForm_Load(object sender, EventArgs e)
        {
            
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {

                //Собираем выборку писем в зависимости отвыбранного сервера

                login = textBox1.Text;
                pass = textBox2.Text;
                smtp = textBox4.Text;
                title = textBox3.Text;
                body = richTextBox2.Text;

                if (maskedTextBox1.Text.Length==0 || maskedTextBox2.Text.Length == 0 || maskedTextBox3.Text.Length == 0 )
                {
                    throw new Exception("Заполните параметры отправки!");
                }

                int port = Convert.ToInt32(maskedTextBox1.Text);
                int count = Convert.ToInt32(maskedTextBox2.Text);
                int ping = Convert.ToInt32(maskedTextBox3.Text) * 1000;

                switch (Regex.Replace(login, @"(\b\w+@)", ""))
                {
                    case "mail.ru":
                        pr = 1;
                        break;
                    case "yandex.ru":
                        pr = 2;
                        break;
                    default:
                        pr = 3;
                        break;

                }

                if (login.Length==0 || pass.Length==0 || smtp.Length==0 || title.Length==0 || body.Length == 0)
                {
                    throw new Exception("Заполните все текстовые поля!");
                }

                if (!Regex.IsMatch(login, @"(\b\w+@[a-zA-Z_]+\b)"))
                {
                    throw new Exception("Введите корректный логин!");
                }

                if (Regex.Replace(login, @"(\b\w+@)", "")!= Regex.Replace(smtp, @"(\b\w+@)", ""))
                {
                    throw new Exception("Домен логина и smtp сервера должны совпадать!");
                }

                if (!dname.Contains(Regex.Replace(login, @"(\b\w+@)", "")))
                {
                    string s = string.Join("\n", dname.ToArray());
                    throw new Exception($"Список поддерживаемых доменов: \n{s}");
                }

                richTextBox1.AppendText("Отправка начата!" + "\n");

                using (emailContext db = new emailContext())
                {
                    mail = db.Emailtbs.Where(x => x.Pr == pr && x.DataMes == null).Take(count).ToList();

                    // отправитель - устанавливаем адрес и отображаемое в письме имя

                    MailAddress from = new MailAddress(login, "Биочистка Биофокс");


                    foreach (var t in mail)
                    {
                        MailAddress to = new MailAddress(t.Email);
                        MailMessage m = new MailMessage(from, to);
                        // тема письма
                        m.Subject = title;
                        // текст письма
                        m.Body = body;
                        // письмо представляет код html
                        m.IsBodyHtml = true;

                        try
                        {
                            // адрес smtp-сервера и порт, с которого будем отправлять письмо
                            SmtpClient smtpm = new SmtpClient(smtp, port)
                            {
                                Credentials = new NetworkCredential(login, pass),
                                EnableSsl = true
                            };

                            // логин и пароль

                            await smtpm.SendMailAsync(m);

                            date = DateTime.Now;

                            richTextBox1.AppendText(t.Fio + " " + t.Email + " " + date + "\n");

                            t.DataMes = BitConverter.GetBytes(date.Ticks);

                            db.Update(t);
                            db.SaveChanges();

                            await Task.Delay(ping);
                        }

                        catch (Exception ex)
                        {
                            Emailtb.MessegeOk(ex.Message);
                            continue;
                        }
                    }

                    richTextBox1.AppendText("Отправка завершена!" + "\n");
                }

                Emailtb.UpdateDG(form.dataGridView1);
            }

            catch (Exception ex)
            {
                Emailtb.MessegeOk(ex.Message);
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void VisitLink()
        {
            // Change the color of the link text by setting LinkVisited
            // to true.
            linkLabel1.LinkVisited = true;
            //Call the Process.Start method to open the default browser
            //with a URL:
            Process.Start("explorer.exe", "https://htmled.it");
        }
    }
}
