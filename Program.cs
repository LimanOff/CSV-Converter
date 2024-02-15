using CSV_Converter.Model.Infrastructure.Database;
using System.IO;
using System.Reflection;
using System.Windows;

namespace CSV_Converter
{
    public class Program
    {
        public static DatabaseContext DatabaseContext { get; private set; }
        public static string RootFolder { get; private set; }

        static void Main(string[] args)
        {
            RootFolder = Assembly.GetExecutingAssembly().Location;

            int howManyDirectoriesClimb = 5;
            for (int i = 0; i < howManyDirectoriesClimb; i++)
            {
                RootFolder = Directory.GetParent(RootFolder).ToString();
            }

            DatabaseContext = new DatabaseContext();
            MessageBox.Show("База данных готова к использованию", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
