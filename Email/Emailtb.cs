using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Email
{
    public partial class Emailtb
    {
        private string email;
        public long Id { get; set; }
        public string Email
        {

            get { return email; }
            set
            {
                if (value == null) throw new Exception("Email не заполнен!");
                if (Regex.IsMatch(value, @"(\b\w+@[a-zA-Z_]+\b)")) email = value;
                else throw new Exception("Используйте корректный формат элетронного адреса");

            }
        }
        public string Fio { get; set; } = null!;
        public long Pr { get; set; }
        public byte[]? DataMes { get; set; }

        public static void MessegeOk(string error)
        {
            MessageBox.Show(
            error, "Ошибка!",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information,
            MessageBoxDefaultButton.Button1,
            MessageBoxOptions.DefaultDesktopOnly);

        }

        public static void TruCathcSave(emailContext db, MainForm mainForm, Form curForm, string error)
        {
            try
            {
                db.SaveChanges();
                UpdateDG(mainForm.dataGridView1);
                curForm.Close();
            }
            catch (Exception ex)
            {
                MessegeOk(ex.Message);

                MessegeOk(error);
                curForm.TopMost = true;

            }

        }

        public static void TruCathcSave(emailContext db, MainForm mainForm, string error)
        {
            try
            {
                db.SaveChanges();
                UpdateDG(mainForm.dataGridView1);
  
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException)
            {

                MessegeOk(error);

            }

        }

        public static void UpdateDG(DataGridView dg)
        {
            //очищаем datagrid перед заполнением
            dg.Rows.Clear();
            using (emailContext db = new emailContext())
            {

                // получаем объекты из бд и передаем в датагрид
                var employees = db.Emailtbs.ToList();
                foreach (var i in employees)
                {
                    if (i.DataMes is not null)
                    {
                        long longVarb = BitConverter.ToInt64(i.DataMes, 0);
                        DateTime date = DateTime.FromBinary(longVarb);

                        dg.Rows.Add(i.Id, i.Fio, i.Email, i.Pr, date);
                    }
                    else
                    {
                        dg.Rows.Add(i.Id, i.Fio, i.Email, i.Pr);
                    }
                }
            }
        }

        public static DialogResult MessegeYesNo(string question)
        {
            return MessageBox.Show(
                   question, "Изменения информации",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);

        }

        public static long CurIndex(DataGridView dg)
        {
            if (dg.Rows.Count > 0)
                return (long)dg.CurrentRow.Cells[0].Value;
            else throw new Exception("Таблица пуста!");
        }

        public static void DeleteEmp(long index)
        {
            using (emailContext db = new emailContext())
            {
                Emailtb? emp = db.Emailtbs.Where(e => e.Id == index).FirstOrDefault();
                if (emp != null)
                {
                    //удаляем объект
                    db.Emailtbs.Remove(emp);
                    db.SaveChanges();
                }

            }
        }

        public static Emailtb GetEmp(Form form, DataGridView dg)
        {
            if (form is MainForm form1)
            {

                long index = CurIndex(dg);
                using emailContext db = new();
                Emailtb? emp = db.Emailtbs.Where(e => e.Id == index).FirstOrDefault();
                if (emp != null)
                    return emp;
                else
                    return new Emailtb();
            }
            else
                return new Emailtb();

        }
    }
}
