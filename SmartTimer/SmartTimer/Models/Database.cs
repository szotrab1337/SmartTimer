using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartTimer.Models
{
    public class Database
    {
        readonly SQLiteAsyncConnection _database;

        public Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Template>().Wait();
        }

        public Task<List<Template>> GetAllTemplates()
        {
            return _database.Table<Template>().ToListAsync();
        }

        public Task AddNewTemplate(Template Template)
        {
            return _database.InsertAsync(Template);
        }

        public Task RemoveTemplate(Template Template)
        {
            return _database.DeleteAsync(Template);
        }

        public Task UpdateTemplate(Template Template)
        {
            return _database.UpdateAsync(Template);
        }
    }
}
