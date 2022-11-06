using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace CPP_Lab_1._3
{
    public class Prisoner
    {
        public long ID { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Middle_Name { get; set; }
        public DateTime Birthday { get; set; }
        public string[] Dates_of_Convictions { get; set; }
        public DateTime Date_of_Last_Imprisonment { get; set; }
        public DateTime? Date_of_Last_Dismissal { get; set; }
        public void PrintPrisoner()
        {

            if (Dates_of_Convictions.Length == 2)
            {
                string[] str = {ID.ToString(),Surname+" "+Name+" "+ Middle_Name,String.Format("{0:dd/MM/yyyy}", Birthday),
                    Dates_of_Convictions[0]+"-"+ Dates_of_Convictions[1],String.Format("{0:dd/MM/yyyy}",
                    Date_of_Last_Imprisonment),String.Format("{0:dd/MM/yyyy}", Date_of_Last_Dismissal)};

                Program.PrintRow(str);
            }
            else
            {
                string[] str = {ID.ToString(),Surname+" "+Name+" "+ Middle_Name,String.Format("{0:dd/MM/yyyy}", Birthday),
                    Dates_of_Convictions[0]+"-"+ Dates_of_Convictions[1],String.Format("{0:dd/MM/yyyy}",
                    Date_of_Last_Imprisonment),String.Format("{0:dd/MM/yyyy}", Date_of_Last_Dismissal)};
                Program.PrintRow(str);

                for (int i = 2; i < Dates_of_Convictions.Length - 1; i++)
                {
                    Program.PrintRow("","", "", Dates_of_Convictions[i] + "-" + Dates_of_Convictions[i + 1], "", "");
                }

            }
            Program.PrintLine();
        }
    }


    public class Program
    {
        static int tableWidth = 188;

        public static void PrintTable(Prisoner[] arrp)
        {

            PrintLine();
            PrintRow("ID", "П.І.П", "Дата народження ", "Дати ув'язнень", "Дата ост ув'язнення", "Дата ост звільнення");
            PrintLine();
            foreach (Prisoner p in arrp)
            {
                p.PrintPrisoner();
            }

        }


        public static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth - 2));
        }

        public static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - 10) / 5;
            string row = "|";

            for (int i = 0; i < columns.Length; i++)
            {
                if (i != 0)
                {
                    row += AlignCentre(columns[i], width) + "|";
                }
                else
                {
                    row += AlignCentre(columns[i], 5) + "|";
                }

            }

            Console.WriteLine(row);
        }

        public static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }


        public static void Read_File(string filePfth, ref Police_file<Prisoner> pf1)
        {
            try
            {
                
                List<string> lines = File.ReadAllLines(filePfth).ToList();

                Prisoner[] Prisoner = new Prisoner[lines.Count];

                for (int k = 0; k < lines.Count; k++)
                {
                    Prisoner[k] = new Prisoner();
                    string[] entries = lines[k].Split(',');
                    
                    Prisoner[k].ID = Convert.ToInt64(entries[0]);
                    Prisoner[k].Surname = entries[1];

                    Prisoner[k].Name = entries[2];
                    Prisoner[k].Middle_Name = entries[3];
                    Prisoner[k].Birthday = Convert.ToDateTime(entries[4]);

                    string[] date = new string[entries.Length - 5];
                    int i = 5, j = 0;

                    while (i < entries.Length)
                    {
                        date[j] = entries[i];
                        i++;
                        j++;

                    }
                    Prisoner[k].Dates_of_Convictions = date;
                    Prisoner[k].Date_of_Last_Imprisonment = Convert.ToDateTime(date[j - 2]);
                    if(date[j - 1] != "...") Prisoner[k].Date_of_Last_Dismissal = Convert.ToDateTime(date[j - 1]);

                }
                pf1 = new Police_file<Prisoner>(Prisoner);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        static void Serialize(XmlSerializer formatter, Police_file<Prisoner> pf1)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Зберегти";
            saveFile.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";
            saveFile.FilterIndex = 2;
            saveFile.RestoreDirectory = true;


            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(Path.GetFullPath(saveFile.FileName), FileMode.OpenOrCreate))
                {

                    formatter.Serialize(fs, pf1);
                    Console.WriteLine("Серіалізували об'єкт");
                }
            }

        }

        static Police_file<Prisoner> Deserialize(XmlSerializer formatter)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Відкрити";
            openFile.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                using (FileStream fs = new FileStream(Path.GetFullPath(openFile.FileName), FileMode.OpenOrCreate))
                {
                    Police_file<Prisoner> pf2 = (Police_file<Prisoner>)formatter.Deserialize(fs);
                    Console.WriteLine("Десеріалізували об'єкт");
                    return pf2;
                }

            }
            return null;

        }
        static void Add(ref Police_file<Prisoner> pf1)
        {
            bool errors = false;
            Regex regex = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$", RegexOptions.IgnorePatternWhitespace);
            List<string> data = new List<string>();
           

            Prisoner newPrisoner = new Prisoner();
            newPrisoner.ID = pf1.GetItem(pf1.Count()-1).ID+1;
            Console.WriteLine("  ID :"+ newPrisoner.ID.ToString());
          

           Console.WriteLine("  Введіть:");
           Console.Write("  Прізвище: "); data.Add(Console.ReadLine());
           Console.Write("  Ім'я:"); data.Add(Console.ReadLine());
           Console.Write("  По батькові:"); data.Add(Console.ReadLine());

                do
                {
                    Console.Write("  День народження(дд.мм.рррр): "); data.Add(Console.ReadLine()); 
                    Match x = regex.Match(data[3]);
                    if (x.Success == true)
                    {
                        if (Convert.ToDateTime(data[3]) < DateTime.Now)
                        {
                            errors = true;
                        }
                    }
                    else
                    {
                        data.RemoveAt(3);
                        Console.WriteLine("Дата введена на правильно");
                    }

                } while (errors == false);

                errors = false;
                do
                {
                    Console.Write("  Дата ув'язнення (дд.мм.рррр):"); data.Add(Console.ReadLine());
                    Match x = regex.Match(data[4]);
                     if (x.Success == true)
                     {
                         if (Convert.ToDateTime(data[4]) < DateTime.Now)
                         {
                            errors = true;
                         }

                      }
                     else
                     {
                        data.RemoveAt(4);
                        Console.WriteLine("Дата введена на правильно");
                      }

                } while (errors == false);

            newPrisoner.Surname = data[0];
            newPrisoner.Name = data[1];
            newPrisoner.Middle_Name = data[2];
            newPrisoner.Birthday = Convert.ToDateTime(data[3]);
            newPrisoner.Date_of_Last_Imprisonment = Convert.ToDateTime(data[4]);
            string[] date = { data[4], "..." };
            newPrisoner.Dates_of_Convictions = date;
            pf1.Append(newPrisoner);
            
        }
        static void Edit(ref Police_file<Prisoner> pf1)
        {

            long id;
            Prisoner prisoner = new Prisoner();
            do
            {
                Console.Write("  Введіть ID: "); id = Convert.ToInt64(Console.ReadLine());
                prisoner = pf1.GetItem(id);
                if (prisoner == null) Console.WriteLine(" Неправильне ID!");

            } while (prisoner == null);

            string date;
            bool errors = false;
            Regex regex = new Regex(@"^([0]?[0-9]|[12][0-9]|[3][01])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$", RegexOptions.IgnorePatternWhitespace);
           

            Console.WriteLine("  ID :" + prisoner.ID.ToString());
            Console.WriteLine("  Прізвище: "+ prisoner.Surname);
            Console.WriteLine("  Ім'я: " +prisoner.Name);
            Console.WriteLine("  По батькові: "+prisoner.Middle_Name);
            Console.WriteLine("  День народження: "+ String.Format("{0:dd/MM/yyyy}", prisoner.Birthday));

            
            if (prisoner.Date_of_Last_Dismissal == null)
            {
                Console.WriteLine("  Дата ув'язнення: " + String.Format("{0:dd/MM/yyyy}", prisoner.Date_of_Last_Imprisonment));
                do
                {
                    Console.Write("  Дата звільнення(дд.мм.рррр):"); date = Console.ReadLine();
                    Match x = regex.Match(date);
                    if (x.Success == true)
                    {
                        if (Convert.ToDateTime(date) <= DateTime.Now && Convert.ToDateTime(date) > prisoner.Date_of_Last_Imprisonment)
                        {
                            errors = true;
                        }
                    }
                    else
                    {
                        date = null;
                        Console.WriteLine("Дата введена на правильно");
                    }

                } while (errors == false);

                prisoner.Date_of_Last_Dismissal = Convert.ToDateTime(date);
                string[] date_arr = new string[prisoner.Dates_of_Convictions.Length];
                int i = 0;
                while (i < date_arr.Length)
                {
                    date_arr[i] = prisoner.Dates_of_Convictions[i];
                    i++;
                }
                date_arr[i-1] = date;
                prisoner.Dates_of_Convictions = date_arr;
               
            }
            else
            {
                Console.WriteLine("  Дата ост ув'язнення: " + String.Format("{0:dd/MM/yyyy}", prisoner.Date_of_Last_Imprisonment));
                Console.WriteLine("  Дата ост звільнення: " + String.Format("{0:dd/MM/yyyy}", prisoner.Date_of_Last_Dismissal));
                do
                {
                    Console.Write("  Дата ув'язнення (дд.мм.рррр):"); date = Console.ReadLine();
                    Match x = regex.Match(date);
                    if (x.Success == true)
                    {
                        if (Convert.ToDateTime(date) <= DateTime.Now&& Convert.ToDateTime(date)>prisoner.Date_of_Last_Dismissal)
                        {
                            errors = true;
                        }
                    }
                    else
                    {
                        date = null;
                        Console.WriteLine("Дата введена на правильно");
                    }

                } while (errors == false);

                prisoner.Date_of_Last_Imprisonment = Convert.ToDateTime(date);
                string[] date_arr = new string[prisoner.Dates_of_Convictions.Length+2];
                int i = 0;
                while(  i < date_arr.Length-2)
                {
                    date_arr[i] = prisoner.Dates_of_Convictions[i];
                    i++;
                }
                date_arr[i] = date;
                date_arr[i+1] = "...";
                prisoner.Dates_of_Convictions = date_arr;
                prisoner.Date_of_Last_Dismissal = null;

            }
            

           
   

        }
        static Police_file<Prisoner> Sort_by( Police_file<Prisoner> pf1,int sort_by)
        {
            Prisoner[] p = pf1.Get_arr();
            switch (sort_by)
            {
                case 1:
                    {
                        Array.Sort(p, (x, y) => x.Surname.CompareTo(y.Surname));
                        break;
                    }
                case 2:
                    {
                        Array.Sort(p, (x, y) => x.Birthday.CompareTo(y.Birthday));
                        break;
                    }
                case 3:
                    {
                        Array.Sort(p, (x, y) => x.Date_of_Last_Imprisonment.CompareTo(y.Date_of_Last_Imprisonment));
                        break;
                    }
                case 4:
                    {
                        while (!IsSorted(p))
                        {
                            p = RandomPermutation(p);
                        }
                        break;
                    }
                default:
                    {
                        Console.WriteLine("\nНевідома дія!");
                        break;
                    }

            }
            pf1 = new Police_file<Prisoner>(p);
            return pf1;

        }
        static bool IsSorted(Prisoner[] a)
        {
            for (int i = 0; i < a.Length - 1; i++)
            {
                DateTime d1 = a[i].Date_of_Last_Imprisonment == null ? DateTime.MinValue : a[i].Date_of_Last_Imprisonment;
                DateTime d2 = a[i+1].Date_of_Last_Imprisonment == null ? DateTime.MinValue : a[i+1].Date_of_Last_Imprisonment;
                if (d1 > d2)
                    return false;
            }

            return true;
        }

        //перемішування елементів масиву
        static Prisoner[] RandomPermutation(Prisoner[] a)
        {
            Random random = new Random();
            var n = a.Length;
            while (n > 1)
            {
                n--;
                var i = random.Next(n + 1);
                var temp = a[i];
                a[i] = a[n];
                a[n] = temp;
            }

            return a;
        }


        static void Delete(ref Police_file<Prisoner> pf1)
        {
            bool delete = false;
            do
            {
                Console.Write("  Введіть ID: "); int id = Convert.ToInt32(Console.ReadLine());
                Prisoner[] prisoners = pf1.Get_arr();

                for (int i = 0; i < prisoners.Length; i++)
                {
                    if (prisoners[i].ID == id)
                    {
                        pf1.Remove(i);
                        delete = true;
                    }

                }
                if (delete == false) Console.WriteLine(" Неправильне ID!");

            } while (delete == false);

          
           
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.SetWindowSize(193, 50);
            Police_file<Prisoner> pf1 = new Police_file<Prisoner>();
            Police_file<Prisoner> pf1_sort = new Police_file<Prisoner>();




            //string filePfth = "..//..//police_file.txt";
            //Read_File(filePfth, ref pf1);
            //PrintTable(pf1.Get_arr());

            //// 3. Створити ітератор для екземпляру ag1
            //Police_file_Iterator<Prisoner> it1 = new Police_file_Iterator<Prisoner>(pf1);

            ////// 4. Демонстрація роботи ітератора
            //it1.First();
            //while (!it1.IsDone())
            //{
            //    Prisoner p = it1.CurrentItem();
            //    p.PrintPrisoner();

            //    it1.Next();
            //}


            bool exit = false;
            do
            {
                int action = -1;
                Console.WriteLine(" Дії:");
                Console.WriteLine("  [1] - Десеріалізувати");
                Console.WriteLine("  [2] - Серіалізувати");
                Console.WriteLine("  [3] - Додати");
                Console.WriteLine("  [4] - Редагувати");
                Console.WriteLine("  [5] - Видалити");
                Console.WriteLine("  [6] - Сортувати");
                Console.WriteLine("  [0] - Завершити роботу");
                Console.Write("  Виберіть дію: "); action = Convert.ToInt32(Console.ReadLine());
                switch (action)
                {
                    case 1:
                        {
                            Console.Clear();
                            XmlSerializer formatter = new XmlSerializer(typeof(Police_file<Prisoner>));
                            pf1 = Deserialize(formatter);
                            if(pf1!=null)PrintTable(pf1.Get_arr());
                            break;
                        }
                    case 2:
                        {
                            Console.Clear();
                            XmlSerializer formatter = new XmlSerializer(typeof(Police_file<Prisoner>));
                            Serialize(formatter, pf1);
                            break;
                        }
                    case 3:
                        {
                            Console.Clear();
                            Add(ref pf1);
                            PrintTable(pf1.Get_arr());
                            break;
                        }
                    case 4:
                        {
                            Console.Clear();
                            Edit(ref pf1);
                            PrintTable(pf1.Get_arr());
                            break;
                        }
                    case 5:
                        {
                           
                            Delete(ref pf1);
                            Console.Clear();
                            PrintTable(pf1.Get_arr());

                            break;
                        }
                    case 6:
                        {
                            Console.WriteLine(" Сортувати:");
                            Console.WriteLine("  [1] - за Прізвищем");
                            Console.WriteLine("  [2] - за Датаою народження");
                            Console.WriteLine("  [3] - за Дата ост. ув'язнення");
                            Console.WriteLine("  [4] - за Дата ост. звільнення");
                            Console.Write("  Виберіть дію: "); int sort_by = Convert.ToInt32(Console.ReadLine());
                            switch (sort_by)
                            {
                                case 1:
                                    {
                                        Console.Clear();
                                        Console.WriteLine("\n Відсортовано за Прізвищем");
                                        pf1_sort = Sort_by(pf1, 1);
                                        PrintTable(pf1_sort.Get_arr());
                                        break;
                                    }
                                case 2:
                                    {
                                        Console.Clear();
                                        Console.WriteLine("\n Відсортовано за Датаою народження");
                                        pf1_sort = Sort_by(pf1, 2);
                                        PrintTable(pf1_sort.Get_arr());
                                        break;
                                    }
                                case 3:
                                    {
                                        Console.Clear();
                                        Console.WriteLine("\n Відсортовано за Дата ост ув'язнення");
                                        pf1_sort = Sort_by(pf1, 3);
                                        PrintTable(pf1_sort.Get_arr());
                                        break;
                                    }
                                case 4:
                                    {
                                        Console.Clear();
                                        Console.WriteLine("\n Відсортовано за Дата ост звільнення");
                                        pf1_sort = Sort_by(pf1, 4);
                                        PrintTable(pf1_sort.Get_arr());
                                        break;
                                    }
                                default:
                                    {
                                        Console.Clear();
                                        PrintTable(pf1.Get_arr());
                                        Console.WriteLine("\nНевідома дія!");
                                        break;
                                    }

                            }

                            break;
                        }

                         case 0:
                        {
                            exit = true;
                            break;
                        }
                         default:
                        {
                            Console.Clear();
                            Console.WriteLine("\nНевідома дія!");
                            break;
                        }
                }

            } while (exit == false);
           
        }
    }
}
