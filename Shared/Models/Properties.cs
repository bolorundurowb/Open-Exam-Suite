using System;

namespace Shared.Models
{
    [Serializable]
    public class Properties
    {
        public string Title { get; set; }

        public string Code { get; set; }

        public int Version { get; set; }

        public double Passmark { get; set; }

        public int TimeLimit { get; set; }

        public string Instructions { get; set; }
    }
}