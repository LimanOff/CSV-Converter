using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Converter.Model
{
    public static class UtilityHelper
    {
        /// <summary>
        /// Возвращает тип, который хранится в обобщенном классе
        /// </summary>
        /// <param name="genericClass">Обобщенный класс</param>
        /// <returns>Type, который хранится в обобщенном классе</returns>
        public static Type GetTypeInGenericClass(Type genericClass)
        {
            return genericClass.GetGenericArguments()[0];
        }
    }
}
