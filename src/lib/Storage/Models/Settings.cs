/*
 *  Created by bolorundurowb on 2/1/2018
 */

using System;
using System.Collections.Generic;
using System.IO;
using LiteDB;
using Storage.Interfaces;

namespace Storage.Models
{
    public class Settings : ISettings
    {
        #region Variables

        private readonly string CollectionName = "CreatorSettings";

        private readonly string _database =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}OpenExamSuite";

        #endregion

        #region Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public string FilePath { get; set; }

        #endregion

        #region Methods

        public void Add(ISettings settings)
        {
            using (var db = new LiteDatabase(_database))
            {
                var collection = db.GetCollection<Settings>(CollectionName);
                collection.Insert((Settings) settings);
            }
        }

        public void Clear()
        {
            using (var db = new LiteDatabase(_database))
            {
                db.DropCollection(CollectionName);
            }
        }

        public IEnumerable<ISettings> GetAll()
        {
            using (var db = new LiteDatabase(_database))
            {
                var collection = db.GetCollection<Settings>();
                return collection.FindAll();
            }
        }

        #endregion
    }
}