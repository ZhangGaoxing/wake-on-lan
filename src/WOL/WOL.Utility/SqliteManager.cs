using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WOL.Utility
{
    public class SqliteManager<T> where T : new()
    {
        /// <summary>
        /// 数据库文件路径
        /// </summary>
        private string dbPath = string.Empty;

        public string DbPath
        {
            get
            {
                if (string.IsNullOrEmpty(dbPath))
                {
                    dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "info.db3");
                }

                return dbPath;
            }
        }

        /// <summary>
        /// 数据库连接
        /// </summary>
        public SQLiteConnection DbConnection
        {
            get
            {
                return new SQLiteConnection(DbPath);
            }
        }

        public SqliteManager()
        {

        }

        public SqliteManager(string dbPath)
        {
            this.dbPath = dbPath;
        }

        /// <summary>
        /// 创建表
        /// </summary>
        public void CreateTable()
        {
            using (var db = DbConnection)
            {
                var c = db.CreateTable<T>();
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="item">插入值</param>
        public bool Insert(T item)
        {
            using (var db = DbConnection)
            {
                return (db.Insert(item) > 0) ? true : false;
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="item">插入值</param>
        public void InsertAll(IEnumerable<T> list)
        {
            using (var db = DbConnection)
            {
                db.InsertAll(list);
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="item">删除值</param>
        public void Delete(T item)
        {
            using (var db = DbConnection)
            {
                db.Delete(item);
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="item">更新值</param>
        public void Update(T item)
        {
            using (var db = DbConnection)
            {
                db.Update(item);
            }
        }

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns>表中所有数据</returns>
        public List<T> QueryAll()
        {
            using (var db = DbConnection)
            {
                return db.Table<T>().ToList();
            }
        }
    }
}
