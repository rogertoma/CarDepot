using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CarDepot
{
    public abstract class AdminItemCache: IAdminItemCache
    {
        public enum UpdateType
        {
            AddItem,
            ModifyItem,
            RemoveItem
        }

        public class RefreshEventArgs : EventArgs
        {
            public RefreshEventArgs(Guid refreshId)
                : base()
            {
                RefreshId = refreshId;
            }

            public Guid RefreshId { get; set; }
        }

        public class UpdateEventArgs : EventArgs
        {
            string _itemId;
            UpdateType _type;

            public UpdateEventArgs(string itemId, UpdateType type)
                : base()
            {
                _itemId = itemId;
                _type = type;
            }

            public string ItemId
            {
                get { return _itemId; }
            }

            public UpdateType Type
            {
                get { return _type; }
            }
        }

        public event EventHandler<RefreshEventArgs> RefreshStarted;
        public event EventHandler<RefreshEventArgs> RefreshEnded;
        public event EventHandler<UpdateEventArgs> ItemUpdate;

        bool _initialRefreshPerformed;

        Dictionary<string, IAdminObject> _cache = new Dictionary<string, IAdminObject>();
        ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public AdminItemCache()
        {
        }

        public virtual void AddItem(IAdminObject item)
        {
            System.Diagnostics.Debug.Assert(item != null, "Parameter 'item' may not be null");

            _lock.EnterWriteLock();
            try
            {
                System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(item.ObjectId), "item has not been saved to the store");
                _cache.Add(item.ObjectId, item);

            }
            finally
            {
                _lock.ExitWriteLock();
            }

            if (ItemUpdate != null)
            {
                ItemUpdate.Invoke(this, new UpdateEventArgs(item.ObjectId, UpdateType.AddItem));
            }
        }

        protected Dictionary<string, IAdminObject> Cache
        {
            get { return _cache; }
        }

        public bool ContainsKey(string objectId)
        {
            return _cache.ContainsKey(objectId);
        }

        public int Count
        {
            get { return _cache.Keys.Count; }
        }

        /// <summary>
        /// Override in a derived class to refresh the actual data.
        /// </summary>
        protected abstract void DoRefresh(Dictionary<string, IAdminObject> cache, Guid refreshId);

        /// <summary>
        /// Override in a derived class to refresh a particular item.
        /// Returns null if the item is no longer present in the data store.
        /// </summary>
        protected abstract IAdminObject DoRefreshItem(IAdminObject item);

        public virtual void ExitReadLock()
        {
            _lock.ExitReadLock();
        }

        public bool InitialRefreshPerformed
        {
            get { return _initialRefreshPerformed; }
            set { _initialRefreshPerformed = value; }
        }

        public virtual void ModifyItem(IAdminObject item)
        {
            _lock.EnterWriteLock();
            try
            {
                _cache[item.ObjectId] = item;
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            if (ItemUpdate != null)
            {
                ItemUpdate.Invoke(this, new UpdateEventArgs(item.ObjectId, UpdateType.ModifyItem));
            }
        }

        public void Refresh()
        {
            Guid refreshId = Guid.NewGuid();

            if (RefreshStarted != null)
            {
                RefreshStarted.Invoke(this, new RefreshEventArgs(refreshId));
            }

            _lock.EnterWriteLock();
            try
            {
                DoRefresh(_cache, refreshId);
            }
            finally
            {
                _lock.ExitWriteLock();

                if (RefreshEnded != null)
                {
                    RefreshEnded.Invoke(this, new RefreshEventArgs(refreshId));
                }
            }

            InitialRefreshPerformed = true;
        }

        public virtual IAdminObject RefreshItem(IAdminObject item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }

            _lock.EnterWriteLock();
            IAdminObject newItem = null;
            try
            {
                newItem = DoRefreshItem(item);
                if (newItem == null)
                {
                    _cache.Remove(item.ObjectId);
                }
                else
                {
                    _cache[item.ObjectId] = newItem;
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            if (ItemUpdate != null)
            {
                ItemUpdate.Invoke(this, new UpdateEventArgs(item.ObjectId, newItem == null ? UpdateType.RemoveItem : UpdateType.ModifyItem));
            }

            return newItem;
        }

        public virtual void RemoveItem(string objectId)
        {
            _lock.EnterWriteLock();
            try
            {
                _cache.Remove(objectId);
            }
            finally
            {
                _lock.ExitWriteLock();
            }

            if (ItemUpdate != null)
            {
                ItemUpdate.Invoke(this, new UpdateEventArgs(objectId, UpdateType.RemoveItem));
            }
        }

        public IAdminObject this[string objectId]
        {
            get
            {
                IAdminObject result = null;
                try
                {
                    _lock.EnterReadLock();
                    if (_cache.ContainsKey(objectId))
                    {
                        result = _cache[objectId];
                    }
                }
                finally
                {
                    _lock.ExitReadLock();

                }
                return result;
            }
            set
            {
                _lock.EnterWriteLock();
                try
                {
                    if (value == null)
                    {
                        throw new InvalidOperationException();
                    }

                    this[objectId] = value;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }

            }
        }

        public List<T> ToList<T>() where T : IAdminObject
        {
            List<T> list = new List<T>();
            _lock.EnterReadLock();
            try
            {
                foreach (string key in _cache.Keys)
                {
                    list.Add((T)_cache[key]);
                }
            }
            finally
            {
                _lock.ExitReadLock();

            }

            return list;
        }

        public bool TryEnterReadLock()
        {
            bool success = _lock.TryEnterReadLock(0);
            return success;
        }
    }
}
