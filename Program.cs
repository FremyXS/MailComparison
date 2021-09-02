using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CheckMail
{
    class Program
    {
        static void Main(string[] args)
        {
            Mail.GetMails(File.ReadAllLines(@"../../../mails.txt"));
            Mail.GetResult(@"../../../finally.txt");
        }
    }
    public class Mail
    {
        public string Login { get; private set; }
        public string Domain { get; }
        public List<Mail> Similars { get; private set; } = new List<Mail>();
        public Mail(string[] path)
        {
            Login = path[0];
            Domain = path[1];
        }
        public static List<Mail> Mails { get; private set; } = new List<Mail>();
        private static string Gmail { get; } = "gmail.com";
        public static void GetMails(string[] data)
        {
            if (int.Parse(data[0]) < 1 || int.Parse(data[0]) > 2 * Math.Pow(10, 4)) 
                throw new Exception("Некорректное количество адресов");
            
            for(var i = 1; i < data.Length; i++)
                CheckMail(data[i], i);

            Comparison();
        }
        private static void CheckMail(string mail, int id)
        {
            if (mail.Length < 3 || mail.Length > 100) 
                throw new Exception($"почта {id} имеет некорректную длину");

            Mails.Add(new Mail(mail.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)));
        }
        private static void Comparison()
        {
            for(var id = 0; id < Mails.Count; id++)
            {
                IsSimilar(Mails[id]);
            }
        }
        private static void IsSimilar(Mail mail)
        {
            if(mail.Domain.ToLower() == Gmail && Mails.Any(e => e.Domain.ToLower() == Gmail))
            {
                var listGmail = Mails.Where(e => e.Domain.ToLower() == Gmail && e!=mail).ToList();
                SearchSimilar(mail, listGmail);
            }
            else
            {
                mail.Similars.AddRange(Mails.Where(e => e.Login.ToLower() == mail.Login.ToLower() && e != mail));
                Delete(mail);
            }
        }
        private static void SearchSimilar(Mail firstMail, List<Mail> mails)
        {
            foreach(var secondMail in mails)
            {
                if (firstMail.Login.ToLower() == secondMail.Login.Replace(".", "").ToLower())
                {
                    firstMail.Similars.Add(secondMail);
                    Mails.Remove(secondMail);
                }
            }
        }
        private static void Delete(Mail mail)
        {
            foreach(var i in mail.Similars)
            {
                if (Mails.Contains(i)) Mails.Remove(i);
            }
        }
        public static void GetResult(string link)
        {
            var listRes = new List<string>();
            listRes.Add(Mails.Count().ToString());

            foreach(var el in Mails)
            {
                listRes.Add(AddString(el));
            }

            File.WriteAllLines(link, listRes.ToArray());
        }
        private static string AddString(Mail mail)
        {
            var str = $"{mail.Login}@{mail.Domain} ";

            foreach(var el in mail.Similars)
            {
                str += $"{el.Login}@{el.Domain} ";
            }

            return str;
        }
            
    }
    

}
