using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace CSV_Converter.Model.Infrastructure.ParserLogic
{
    public class Parser
    {
        /// <summary>
        /// Преобразует значение из колонки в необходимый тип
        /// </summary>
        /// <param name="columnValue">Значение из колонки</param>
        /// <param name="typeToConvert">Тип, в который нужно преобразовать данные</param>
        /// <param name="resultOfConvertion">Результат преобразования данных</param>
        public void ConvertColumnValue(string columnValue, Type typeToConvert, out object resultOfConvertion)
        {
            resultOfConvertion = Convert.ChangeType(columnValue, typeToConvert);
        }

        /// <summary>
        /// Считывает данные из файла и возвращает строку, если есть данные.
        /// <para>Вызывет исключение и сообщает об этом пользователю, если файл, с которого берутся данные открыт</para>
        /// </summary>
        /// <param name="filePath">Путь до файла</param>
        /// <param name="fileNameWithExtension">Имя файла с расширением</param>
        /// <returns><![CDATA[List<string>]]></returns>
        public List<string> ReadFileLines(string filePath, string fileNameWithExtension)
        {
            List<string> lines = new List<string>();
            Exception exception = null;
            do
            {
                try
                {
                    lines = File.ReadAllLines(filePath, Encoding.UTF8).Select(x => x.Trim()).ToList();
                    exception = null;
                }
                catch (Exception exc)
                {
                    exception = exc;
                    MessageBox.Show($"Невозможно прочесть файл {fileNameWithExtension}, пока он открыт. Для дальнейшей работы закройте файл {fileNameWithExtension}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            } while (exception != null);
            string firstLine = lines[0];
            lines.RemoveAt(0);
            return lines;
        }

        /// <summary>
        /// Обрабатывает строку и возвращает заполненный объект
        /// </summary>
        /// <param name="line">Строка с данными</param>
        /// <param name="properties">Свойства</param>
        /// <param name="objectForFilling">Объект для заполнения</param>
        /// <returns></returns>
        public object ProcessLine(string line, List<PropertyInfo> properties, object objectForFilling)
        {
            List<string> columns = line.Split(";").ToList();

            int index = 0;
            foreach (PropertyInfo property in properties)
            {
                if (index == columns.Count)
                    break;

                if (property.Name == "Id")
                    continue;

                object value;

                if (property.PropertyType.GetGenericArguments().Length == 0)
                {
                    ConvertColumnValue(columns[index], property.PropertyType, out value);
                }
                else
                {
                    ConvertColumnValue(columns[index], UtilityHelper.GetTypeInGenericClass(property.PropertyType), out value);
                }
                property.SetValue(objectForFilling, value);
                index++;
            }

            return objectForFilling;
        }
    }
}
