using System;
using System.Collections.Generic;
using System.Text;

namespace Novella.Json
{
    public class Profile
    {
        public List<string> closed { get; set; } = new List<string>();
        public string active { get; set; }
        public static Profile Instance { get; set; }
    }
}
