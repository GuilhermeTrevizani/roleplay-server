using System;

namespace Roleplay.Models
{
    public class BeautifulException
    {
        public DateTime DateTime { get; set; } = DateTime.Now;
        public Exception Exception { get; set; } = null;
    }
}