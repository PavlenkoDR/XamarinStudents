using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Novella.Json
{
    public class Selector
    {

        public string text { get; set; }
        public string dialogTrigger { get; set; }
        public string message { get; set; }
    }
   public class Step {
        public string npc { get; set; }
        public string text { get; set; }
        public string message { get; set; }
    }
    public class Option
    {
        public string dialogTrigger { get; set; }
        public string text { get; set; }
        public string message { get; set; }
        public List<Selector> selectors { get; set; } = new List<Selector>();
    }
    public class Dialog
    {
        public string background { get; set; }
        public List<Step> steps { get; set; } = new List<Step>();
        public Option options { get; set; }
        public static Dialog Active { get; set; } = null;
    }
}