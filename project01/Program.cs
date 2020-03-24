using System;
using System.IO;

namespace project01
{
    class Program
    {
        static int Main(string[] args)
        {
            // Если не указан файл с конфигурацией, то выход
            if (args.Length < 1)
            {
                Console.WriteLine("Нет конфигурационного файла");
                return 1;
            }

            // Загрузка конфигураций
            int r;
            string configFileName = args[0];
            Conf conf = new Conf();
            r = conf.Load(configFileName);
            // Проверка корректности загрузки конфигов
            if (r != 0)
            {
                Console.WriteLine("Load configuration error (code = {0})", r);
                return r;
            }

            // Файл с входными данными
            string inputFileName = conf.Get("InputFileName");

            // Параметры подключения к БД
            string datasource = conf.Get("datasource");
            string DataBaseName = conf.Get("DataBaseName");
            string username = conf.Get("username");
            string password = conf.Get("password");

            // Обработка файла csv-формата
            CSVData csvData = new CSVData(inputFileName);
            r = csvData.Load();
            if (r != 0)
            {
                Console.WriteLine("Ошибка чтения csv-файла. Код ошибки = {0}", r);
                return r;
            }

            // Обработка загруженных данных (строк)
            r = csvData.ProcessData();
            if (r != 0)
            {
                Console.WriteLine("Ошибка обработки данных. Код ошибки = {0}", r);
            }

            r = csvData.InitDB(datasource, DataBaseName, username, password);
            if (r != 0)
            {
                Console.WriteLine("Ошибка инициализации БД. Код ошибки = {0}", r);
                return r;
            }

            // Сохранение в БД
            csvData.SaveToDB();

            return 0;
        }



    }



}
