using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperium.Data.Models;

namespace Imperium.Data.DataInterfaces
{
    public interface ISaveMetadataRepository
    {
       void Insert(SaveMetadata metadata);
    }
}
