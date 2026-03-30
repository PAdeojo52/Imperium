using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imperium.Data.EntityModels.Character
{
    public class Race
    {
        public int RaceID { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;

    }
}
