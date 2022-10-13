using System;

namespace NoAccident.Models
{
    public class AppConfig
    {
        public DateTime AccidentDateTime { get; set; }
        public DateTime SinceDate { get; set; }
        public int MaxUsers { get; set; }
        public int MaxDay { get; set; }
        public string API_URL { get; set; }
    }
}