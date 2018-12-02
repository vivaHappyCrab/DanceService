using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DanceService.Models
{
    public class GroupModel
    {
        public string Name { get; set; }

        public int Pairs { get; set; }

        public int Advanced { get; set; }

        public int Tour { get; set; }

        public int DancesCount { get; set; }

        public IEnumerable<string> Dances { get; set; }
    }
}