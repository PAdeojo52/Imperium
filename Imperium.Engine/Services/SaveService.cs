using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imperium.Data.DataInterfaces;
using Imperium.Data.Models;

namespace Imperium.Engine.Services
{
    public class SaveService
    {
        private readonly ISaveMetadataRepository _repository;

        public SaveService(ISaveMetadataRepository repository)
        {
            _repository = repository;
        }

        public void SaveMetadata(string saveName)
        {
            var metadata = new SaveMetadata
            {
                SaveName = saveName,
                TimeStamp = DateTime.Now
            };

            _repository.Insert(metadata);
        }
    }
}
