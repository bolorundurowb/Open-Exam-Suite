namespace Storage.Services
{
    public class SettingsCollection : ISettin
    {
        private SettingsCollection _settingsCollection;

        public SettingsCollection()
        {
            
        }
        
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
    }
}