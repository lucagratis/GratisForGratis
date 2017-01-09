using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GratisForGratis.Models
{
    class Autocomplete
    {
        public int Value { get; set; }
        public string Label { get; set; }
    }

    class AutocompleteGuid
    {
        public Guid Value { get; set; }
        public string Label { get; set; }
    }
}
