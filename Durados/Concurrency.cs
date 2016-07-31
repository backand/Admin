using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Durados
{
    public class Concurrency
    {
        public int ObsoleteItemInterval { get; set; }
        public int ObsoleteReaderInterval { get; set; }
        private Dictionary<string, SortedDictionary<DateTime, Item>> ItemsByViews; 
        private SortedDictionary<DateTime, IObsolete> Items; 
        private Dictionary<string, Dictionary<string, Reader>> ReadersByViews;
        private SortedDictionary<DateTime, IObsolete> Readers;

        private Cleaner itemsCleaner;
        private Cleaner readersCleaner;

        public Concurrency()
        {
            ItemsByViews = new Dictionary<string, SortedDictionary<DateTime, Item>>();
            Items = new SortedDictionary<DateTime, IObsolete>();
            ReadersByViews = new Dictionary<string, Dictionary<string, Reader>>();
            Readers = new SortedDictionary<DateTime, IObsolete>();

            itemsCleaner = new ItemsCleaner();
            readersCleaner = new ReadersCleaner();
        }

        protected void Clean()
        {
            CleanItems();
            CleanReaders();
        }

        protected void CleanItems()
        {
            itemsCleaner.Clean(Items, ItemsByViews, ObsoleteItemInterval);
        }

        protected void CleanReaders()
        {
            readersCleaner.Clean(Readers, ReadersByViews, ObsoleteReaderInterval);
        }

        public void Write(string viewName, Writer writer, object data)
        {
            lock (this)
            {
                Clean();

                if (!ItemsByViews.ContainsKey(viewName))
                    ItemsByViews.Add(viewName, new SortedDictionary<DateTime, Item>());

                SortedDictionary<DateTime, Item> items = ItemsByViews[viewName];
                DateTime timeStamp = DateTime.Now;
                while (items.ContainsKey(timeStamp) || Items.ContainsKey(timeStamp))
                {
                    timeStamp.AddMilliseconds(1);
                }
                Item item = new Item() { Data = data, TimeStemp = timeStamp, ViewName = viewName, Writer = writer };
                items.Add(timeStamp, item);
                Items.Add(timeStamp, item);

            }
        }

        public List<object> Read(Reader reader, string viewName)
        {
            lock (this)
            {
                AddReader(viewName, reader);
                if (ItemsByViews.ContainsKey(viewName))
                {
                    List<object> items = new List<object>();
                    foreach (Item item in ItemsByViews[viewName].Values.Where(item => !item.Readers.ContainsKey(reader.Username)))
                    {
                        items.Add(item.Data);
                        if (item.Readers.ContainsKey(reader.Username))
                            item.Readers.Remove(reader.Username);
                        item.Readers.Add(reader.Username, reader);
                    }
                    return items;
                }
                else
                    return null;
            }
        }

        public List<Reader> GetCurrentUsers(string viewName)
        {
            if (ReadersByViews.ContainsKey(viewName))
            {
                return ReadersByViews[viewName].Values.ToList();
            }
            else
            {
                return null;
            }
        }

        private void AddReader(string viewName, Reader reader)
        {
            lock (this)
            {
                Clean();

                if (!ReadersByViews.ContainsKey(viewName))
                    ReadersByViews.Add(viewName, new Dictionary<string, Reader>());

                Dictionary<string, Reader> readers = ReadersByViews[viewName];
                if (readers.ContainsKey(reader.Username))
                {
                    Reader prevReader = readers[reader.Username];
                    readers.Remove(reader.Username);
                    Readers.Remove(prevReader.TimeStemp);
                }
                DateTime timeStamp = DateTime.Now;
                while (Readers.ContainsKey(timeStamp))
                {
                    timeStamp.AddMilliseconds(1);
                }
                reader.ViewName = viewName;
                reader.TimeStemp = timeStamp;
                readers.Add(reader.Username, reader);
                Readers.Add(timeStamp, reader);
            }
        }

        public interface IObsolete
        {
            string ViewName { get; set; }
            DateTime TimeStemp { get; set; }

            bool Obsolete(int interval);

        }

        public class Item : IObsolete
        {
            public string ViewName { get; set; }
            public Writer Writer { get; set; }
            public Dictionary<string, Reader> Readers { get; private set; }
            public DateTime TimeStemp { get; set; }
            public object Data { get; set; }

            public Item()
            {
                Readers = new Dictionary<string, Reader>();
            }

            public bool Obsolete(int interval)
            {
                return DateTime.Now.Subtract(TimeStemp).TotalMilliseconds > interval;
            }
        }

        public class User
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Fullname { get; set; }
        }

        public class Reader : User, IObsolete
        {
            public DateTime TimeStemp { get; set; }
            public string ViewName { get; set; }
            
            public bool Obsolete(int interval)
            {
                return DateTime.Now.Subtract(TimeStemp).TotalMilliseconds > interval;
            }
        }

        public class Writer : User
        {
        }

        public abstract class Cleaner
        {
            public void Clean(SortedDictionary<DateTime, IObsolete> items, object itemsByView, int interval)
            {
                if (items.Count == 0)
                {
                    return;
                }
                List<Item> itemsToClean = new List<Item>();
                foreach (Item item in items.Values)
                {
                    if (item.Obsolete(interval))
                        itemsToClean.Add(item);
                    else
                        break;
                }
                foreach (Item item in itemsToClean)
                {
                }
            }

            protected virtual void Remove(SortedDictionary<DateTime, IObsolete> items, object itemsByView, IObsolete item)
            {
                items.Remove(item.TimeStemp);
                RemoveItemByView(itemsByView, item);
            }

            protected abstract void RemoveItemByView(object itemsByView, IObsolete item);
            
        }

        public class ItemsCleaner : Cleaner
        {
            protected override void RemoveItemByView(object itemsByView, IObsolete item)
            {
                RemoveItemByView((Dictionary<string, SortedDictionary<DateTime, Item>>)itemsByView, (Item)item); 
            }

            private void RemoveItemByView(Dictionary<string, SortedDictionary<DateTime, Item>> itemsByView, Item item)
            {
                itemsByView[item.ViewName].Remove(item.TimeStemp);
            }
        }

        public class ReadersCleaner : Cleaner
        {
            protected override void RemoveItemByView(object itemsByView, IObsolete item)
            {
                RemoveItemByView((Dictionary<string, SortedDictionary<string, Reader>>)itemsByView, (Reader)item);
            }

            private void RemoveItemByView(Dictionary<string, SortedDictionary<string, Reader>> readersByView, Reader reader)
            {
                readersByView[reader.ViewName].Remove(reader.Username);
            }
        }
    }
}
