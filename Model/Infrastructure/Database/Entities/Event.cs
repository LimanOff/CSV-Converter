using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Converter.Model.Infrastructure.Database.Data
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Start_Date { get; set; }
        public int Duration_In_Days { get; set; }
        public int Id_City { get; set; }
        public string Logo_Link { get; set; }
        public int Id_Direction { get; set; }

        [ForeignKey("Id_City")]
        public City City { get; set; }

        [ForeignKey("Id_Direction")]
        public Direction Direction { get; set; }
    }
}
