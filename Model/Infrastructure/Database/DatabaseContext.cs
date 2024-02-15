using CSV_Converter.Model.Infrastructure.Database.Data;
using CSV_Converter.Model.Infrastructure.ParserLogic;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace CSV_Converter.Model.Infrastructure.Database
{
    public class DatabaseContext : DbContext
    {
        private readonly string _connectionHomeString = "Data Source=localhost;Initial Catalog=_RenameMe;TrustServerCertificate=True;Integrated Security=True";

        private Parser _parser;
        private List<PropertyInfo> _dbContextProperties = new List<PropertyInfo>();

        public DbSet<Gender> Genders { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Activity> Activities { get; set; }

        public DbSet<Jury> Juries { get; set; }
        public DbSet<Organizer> Organizers { get; set; }
        public DbSet<Moderator> Moderators { get; set; }
        public DbSet<Participant> Participants { get; set; }

        public DatabaseContext()
        {
            Database.EnsureCreated();

            InitDatabase();
        }

        private void InitDatabase()
        {
            if (MessageBox.Show("Пересоздать базу данных?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Database.EnsureDeleted();
                Database.EnsureCreated();

                if (MessageBox.Show("База данных пуста.\nЗаполнить базу первичными данными?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    _dbContextProperties = GetType()
                                            .GetProperties()
                                            .ToList();
                    _parser = new Parser();
                    PopulateDatabase($"{Program.RootFolder}/Model/Infrastructure/Database/Data/");
                }
            }

            MessageBox.Show("База данных готова к использованию", "Результат", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionHomeString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gender>().HasData(
                new Gender() { Id = 1, Name = "Мужской" },
                new Gender() { Id = 2, Name = "Женский" });

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.GetForeignKeys()
                    .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade)
                    .ToList()
                    .ForEach(fk => fk.DeleteBehavior = DeleteBehavior.NoAction);
            }
        }

        /// <summary>
        /// Заполнить базу данных данными из файлов формата csv
        /// </summary>
        /// <param name="pathToFolderWithFiles">Путь до файлов формата csv</param>
        private void PopulateDatabase(string pathToFolderWithFiles)
        {
            List<string> files = Directory.GetFiles(pathToFolderWithFiles).ToList();

            List<string> sequenceOfParsing = new List<string>()
            {
                "Direction",
                "Country",
                "City",
                "Event",
                "Moderator",
                "Organizer",
                "Participant",
                "Jury",
                "Activity"
            };

            foreach (string element in sequenceOfParsing)
            {
                foreach (string file in files)
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
                    string fileNameWithExtension = Path.GetFileName(file);
                    string nameOfClass = fileNameWithoutExtension.Replace("_import", "");

                    if (nameOfClass != element)
                        continue;

                    Type inspectClassType = FindClassInGenericProperties(nameOfClass);
                    List<PropertyInfo> inspectClassProperties = inspectClassType.GetProperties()
                                                                                .Where(prop => prop.CustomAttributes.Count() == 0)
                                                                                .ToList();

                    List<string> lines = _parser.ReadFileLines($"{Program.RootFolder}/Model/Infrastructure//Database/Data/{fileNameWithExtension}", fileNameWithExtension);

                    foreach (string line in lines)
                    {
                        object inspectClassObject = Activator.CreateInstance(inspectClassType);
                        inspectClassObject = _parser.ProcessLine(line, inspectClassProperties, inspectClassObject);

                        MethodInfo method = GetType().
                                        GetMethod("AddPropertyToDbSet", BindingFlags.NonPublic | BindingFlags.Instance);

                        method.MakeGenericMethod(inspectClassType)
                                .Invoke(this, new object[] { inspectClassObject });
                        SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// Добавляет объект в нужное DbSet свойство DatabaseContext
        /// </summary>
        /// <typeparam name="T">Т</typeparam>
        /// <param name="inspectClassObject"></param>
        private void AddPropertyToDbSet<T>(T inspectClassObject) where T : class
        {
            Type type = inspectClassObject.GetType();
            Type openType = typeof(DbSet<>);
            Type genericType = openType.MakeGenericType(type);

            foreach (PropertyInfo property in _dbContextProperties)
            {
                if (property.PropertyType == genericType)
                {
                    DbSet<T> instance = (DbSet<T>)property.GetValue(this);
                    instance.GetType().GetMethod("Add").Invoke(instance, new object[] { inspectClassObject });
                }
            }
        }

        /// <summary>
        /// Находит необходимый класс в свойствах экземпляра контекста базы данных
        /// </summary>
        /// <param name="nameOfClass">Имя искомого класса</param>
        /// <returns>Type, если не находит, то null</returns>
        private Type FindClassInGenericProperties(string nameOfClass)
        {
            foreach (var dbContextProperty in _dbContextProperties)
            {
                Type dbContextGenericArgumentClass = dbContextProperty.PropertyType.GetGenericArguments()[0];

                if (dbContextGenericArgumentClass.Name == nameOfClass)
                {
                    return dbContextGenericArgumentClass;
                }
            }
            return null;
        }

        /// <summary>
        /// Находит пользователя по Id и Password, а затем возвращает его роль
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="password">Пароль</param>
        /// <returns>Роль пользователя в системе</returns>
        public string FindUserRoleByPasswordAndId(int id, string password)
        {
            if (Juries.FirstOrDefault(x => x.Id == id && x.Password == password) != null)
            {
                return "Jury";
            }
            if (Organizers.FirstOrDefault(x => x.Id == id && x.Password == password) != null)
            {
                return "Organizer";
            }
            if (Moderators.FirstOrDefault(x => x.Id == id && x.Password == password) != null)
            {
                return "Moderator";
            }
            if (Participants.FirstOrDefault(x => x.Id == id && x.Password == password) != null)
            {
                return "Participant";
            }
            return "none";
        }
    }
}
