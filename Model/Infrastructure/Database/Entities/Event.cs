using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations.Schema;

namespace CSV_Converter.Model.Infrastructure.Database.Data
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Start_Date { get; set; }
        public int Duration_In_Days { get; set; }
        public int Id_City { get; set; }

        [ForeignKey("Id_City")]
        public City City { get; set; }
    }
}
