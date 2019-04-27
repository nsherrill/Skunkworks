using RFKBackend.Shared.DataContracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Dashboard.Models
{
    public class YearConfigModel
    {
        public int Year { get; set; }
        public RoleCountModel[] RoleCounts { get; set; }

        public YearConfigModel(int year)
        {
            this.Year = year;
        }
    }

}