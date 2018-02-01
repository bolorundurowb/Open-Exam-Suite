/*
 *  Created by bolorundurowb on 2/1/2018
 */

using System;
using System.Collections.Generic;
using System.IO;
using LiteDB;
using Storage.Enums;
using Storage.Interfaces;

namespace Storage.Models
{
    public class Settings : ISettings
    {
        #region Variables

        private readonly string _collectionName;

        private readonly string _database =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}{Path.DirectorySeparatorChar}OpenExamSuite";

        #endregion

        #region Constructor

        public Settings(SettingsType settingsType)
        {
            if (settingsType == SettingsType.Creator)
            {
                _collectionName = "CreatorSettings";
            }

            if (settingsType == SettingsType.Simulator)
            {
                _collectionName = "SimulatorSettings";
            }
        }

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
                var collection = db.GetCollection<Settings>(_collectionName);
                collection.Insert((Settings) settings);
            }
        }

        public void Clear()
        {
            using (var db = new LiteDatabase(_database))
            {
                db.DropCollection(_collectionName);
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