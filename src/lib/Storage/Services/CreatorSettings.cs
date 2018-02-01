/*
 *  Created by bolorundurowb on 2/1/2018
 */

using System;
using System.Collections.Generic;
using System.IO;
using LiteDB;
using Storage.Interfaces;

namespace Storage.Services
{
    public class CreatorSettings : ISettings
    {
        private readonly string CollectionName = "CreatorSettings";

        private readonly string Database =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}OpenExamSuite";
        
        public void Add(ISettings settings)
        {
            using (var db = new LiteDatabase(Database))
            {
                var collection = db.GetCollection<CreatorSettings>(CollectionName);
                collection.Insert((CreatorSettings) settings);
            }
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public List<ISettings> GetAll()
        {
            throw new System.NotImplementedException();
        }
    }
}