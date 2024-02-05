using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CSV_Converter.Model.Infrastructure.Database.Data
{
    public class Activity
    {
        public int Id { get; set; }
        public int Id_Event { get; set; }
        public string Name_Of_Activity { get; set; }
        public int Day { get; set; }
        public DateTime Start_Time { get; set; }
        public int Id_Moderator { get; set; }
        public int Id_Jury_1 { get; set; }
        public int Id_Jury_2 { get; set; }
        public int Id_Jury_3 { get; set; }
        public int Id_Jury_4 { get; set; }
        public int Id_Jury_5 { get; set; }
        public int Id_Winner_Participants { get; set; }

        [ForeignKey("Id_Moderator")]
        public Moderator Moderator { get; set; }

        [ForeignKey("Id_Event")]
        public Event Event { get; set; }

        [ForeignKey("Id_Jury_1")]
        public Jury Jury_1 { get; set; }

        [ForeignKey("Id_Jury_2")]
        public Jury Jury_2 { get; set; }

        [ForeignKey("Id_Jury_3")]
        public Jury Jury_3 { get; set; }

        [ForeignKey("Id_Jury_4")]
        public Jury Jury_4 { get; set; }

        [ForeignKey("Id_Jury_5")]
        public Jury Jury_5 { get; set; }

        [ForeignKey("Id_Winner_Participants")]
        public Participant Winner { get; set; }
    }
}