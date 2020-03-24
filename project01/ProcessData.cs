using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace project01
{
    // CSVData класс для работы с данными в csv-файла
    class CSVData
    {
        // Список для хранения строк csv-файла
        private ArrayList RawData;
        private ArrayList Data;
        // Имя файла
        private string csvFileName;
        // Коннект с БД
        SqlConnection conn;
        // Конструктор с параметром
        public CSVData(string name)
        {
            csvFileName = name;
            RawData = new ArrayList();
            Data = new ArrayList();
        }

        // Загрузка данных из csv-файла.
        public int Load()
        {
            int r = 0;
            try
            {
                using (StreamReader sr = new StreamReader(csvFileName, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        // Добавление строки в коллекцию
                        RawData.Add(line);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                r = 1;
                return r;
            }
            return r;
        }

        // ProcessRow обработка строки. Возвращается структура с данными
        private Record ProcessRow(string row)
        {
            Record record = new Record();
            record.status = 0;

            // Разбиение строки по разделителю (запятой)
            string[] valuesStr = row.Split(',');

            // Если неправильный формат csv-файла, то выход.
            if (valuesStr.Length != 3)
            {
                Console.Write("non-correct format format ({0})", valuesStr);
                record.status = 1;
                return record;
            }

            // Бизнес-логика обработки записи

            string valueStr = valuesStr[2];
            Int32 valueStart;
            try
            {
                valueStart = Int32.Parse(valueStr);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                record.status = 2;
                return record;
            }
            Int32 price = processValue(valueStart);

            // Преобразуем ID
            Int32 ID = 0;
            try
            {
                ID = Int32.Parse(valuesStr[0]);
            }
            catch (FormatException e)
            {
                Console.WriteLine(e.Message);
                record.status = 2;
                return record;
            }

            record.ID = ID;
            record.Name = valuesStr[1];
            record.Price = price;

            return record;
        }
        // Обработка целого значения
        private Int32 processValue(Int32 valueStart)
        {
            Int32 valueEnd = valueStart;
            // Для примера: если цена больше 1000, то увеличиваем на 10%.
            if (valueStart < 1000)
            {
                double d;
                d = (double)valueStart * 1.1;
                valueEnd = (Int32)d;
            }
            return valueEnd;
        }

        // Обработать все значения (строки)
        public int ProcessData()
        {
            int r = 0;

            var query = from n in RawData.AsParallel().Cast<string>()
                        select ProcessRow(n);
            foreach (var k in query)
            {
                // Console.WriteLine(k.Price);
                Data.Add(k);

                /*int rr;
                rr = saveToDB(k);
                if (rr != 0)
                {
                    Console.WriteLine("saveToDB error, code = {0}", rr);
                }*/
            }

            return r;
        }

        public int InitDB(string datasource, string database, string username, string password)
        {
            int r = 0;

            string connString = @"Data Source=" + datasource + ";Initial Catalog="
                       + database + ";Persist Security Info=True;User ID=" + username + ";Password=" + password;

            conn = new SqlConnection(connString);


            return r;
        }
        // Сохранение значения в БД
        public void SaveToDB()
        {

            conn.Open();
            foreach (Record record in Data)
            {
                // Если данные некорректные, то запись не делаем
                if (record.status != 0)
                {
                    Console.WriteLine("Non correct data");
                    continue;
                }

                // Console.WriteLine("Объект: {0} Стоимость = {1}", record.Name, record.Price);
                try
                {
                    // Команда Insert
                    string sql = "Insert into TestTable (ID, Name, Price) "
                                                     + " values (@ID, @Name, @Price) ";

                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandText = sql;

                    // Добавить параметр @ID
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = record.ID;

                    // Добавить параметр @Name
                    cmd.Parameters.Add("@Name", SqlDbType.String).Value = record.Name;

                    // Добавить параметр @Price
                    cmd.Parameters.Add("@Price", SqlDbType.Int).Value = record.Price;

                    // Выполнить Command 
                    int rowCount = cmd.ExecuteNonQuery();
                    Console.WriteLine("Row Count affected = " + rowCount);
                }
                catch (Exception e) { Console.WriteLine("Error: " + e); }
                finally
                {
                    // Закрываем коннект. Очищаем ресурсы.
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
        }
    }
    // Record структура для хранения данных для записи в БД
    struct Record
    {
        // Статус
        public int status;
        // Идентификатор объекта
        public Int32 ID;
        // Название объекта
        public string Name;
        // Стоимость объекта
        public Int32 Price;
    }
}