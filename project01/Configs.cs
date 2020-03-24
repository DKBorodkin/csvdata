using System;
using System.IO;
using System.Collections.Generic;

namespace project01
{
    // Conf класс для работы с конфигурациями
    class Conf
    {
        // Статус загрузки 
        private bool IsDone;
        // Справочник для хранения конфигов  
        private Dictionary<string, string> confs;
        // Добавление конфига в справочник
        private void add(string s)
        {
            char ch = '=';
            int indexOfChar = s.IndexOf(ch);
            if (indexOfChar == -1)
            {
                return;
            }
            string name = s.Substring(0, indexOfChar);
            string value = s.Substring(indexOfChar + 1);

            // Если элемента с таким ключом нет, то добавляем его.
            if (!confs.ContainsKey(name))
            {
                confs.Add(name, value);
            }
        }
        // Вывод конфигураций
        public void Print()
        {
            foreach (KeyValuePair<string, string> i in confs)
            {
                Console.WriteLine("Key: {0}     Value: {1}", i.Key, i.Value);
            }
        }
        // Получить значение конфигурации по ключу
        public string Get(string key)
        {
            string s = "";
            if (confs.ContainsKey(key))
            {
                s = confs[key];
            }
            return s;
        }
        public bool GetStatus()
        {
            return IsDone;
        }
        // IsExist проверка: существует ли в справочнике такой элемент (конфигурация)
        public bool IsExist(string key)
        {
            return confs.ContainsKey(key);
        }
        // Загрузка конфигураций
        public int Load(string fileName)
        {
            IsDone = true;
            int r = 0;
            confs = new Dictionary<string, string>();
            Console.WriteLine("Conf load from " + fileName);
            try
            {
                using (StreamReader sr = new StreamReader(fileName, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                IsDone = false;
                r = 1;
                return r;
            }
            return r;
        }
    }
}
