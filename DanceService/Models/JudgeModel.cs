using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DanceService.Models
{
    public class JudgeModel
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public bool Locked { get; set; }
    }
}