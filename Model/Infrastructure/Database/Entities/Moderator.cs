using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSV_Converter.Model.Infrastructure.Database.Data
{
    public class Moderator
    {
        public int Id { get; set; }
        public string Last_Name { get; set; }
        public string First_Name { get; set; }
        public string Patronymic { get; set; }
        public int Id_Gender { get; set; }
        public string EMail { get; set; }
        public DateTime Birthday { get; set; }
        public int Id_Country { get; set; }
        public string Phone { get; set; }
        public int Id_Direction { get; set; }
        public int? Id_Event { get; set; }
        public string Password { get; set; }
        public string Photo_Link { get; set; }

        [ForeignKey("Id_Gender")]
        public Gender Gender { get; set; }

        [ForeignKey("Id_Country")]
        public Country Country { get; set; }

        [ForeignKey("Id_Direction")]
        public Direction Direction { get; set; }

        [ForeignKey("Id_Event")]
        public Event Event { get; set; }
    }
}