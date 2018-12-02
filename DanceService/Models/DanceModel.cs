using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DanceService.Models
{
    public class DanceModel
    {
        public IEnumerable<TurnModel> Turns { get; set; } 
    }

    public class TurnModel
    {
        public string Dance { get; set; }
        
        public IEnumerable<int> Pairs { get; set; }     
    }
}