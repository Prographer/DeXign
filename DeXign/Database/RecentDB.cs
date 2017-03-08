using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeXign.Models;

namespace DeXign.Database
{

    public class RecentDB : IDisposable
    {
        static RecentDB db;

        LiteDatabase liteDb;
        LiteCollection<RecentItem> collection;

        private RecentDB()
        {
            liteDb = new LiteDatabase($"{Environment.UserName}.recent.db");
            collection = liteDb.GetCollection<RecentItem>("recents");
        }

        public void Dispose()
        {
            liteDb?.Dispose();
            liteDb = null;
        }

        private void Add(string fileName)
        {
            if (!Contains(fileName))
                collection.Insert(new RecentItem(fileName));
            else
                Update(fileName);
        }

        private void Remove(string fileName)
        {
            if (Contains(fileName))
                collection.Delete(item => item.FileName == fileName);
        }

        public void Update(string fileName)
        {
            if (!Contains(fileName))
                return;

            RecentItem item = collection
                .FindOne(r => r.FileName == fileName);

            item.LastedTime = DateTime.Now;

            collection.Update(item);
        }

        public bool Contains(string fileName)
        {
            return collection
                .Find(f => f.FileName == fileName)
                .Count() > 0;
        }

        public static void Open()
        {
            if (db == null)
                db = new RecentDB();
        }

        public static void Close()
        {
            db?.Dispose();
            db = null;
        }

        public static void AddFile(string fileName)
        {
            db.Add(fileName);
        }

        public static void RemoveFile(string fileName)
        {
            db.Remove(fileName);
        }

        public static IEnumerable<RecentItem> GetFiles()
        {
            return db.collection
                .FindAll()
                .OrderByDescending(item => item.LastedTime);
        }
    }
}
