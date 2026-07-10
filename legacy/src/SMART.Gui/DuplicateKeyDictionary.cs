using System.Collections.Generic;

namespace SMART.Gui
{
    public class DuplicateKeyDictionary<K, T>
    {
        private readonly List<InternalHolder<K, T>> _internalHolders = new List<InternalHolder<K, T>>();

        public DuplicateKeyDictionary()
        {
        }

        public List<T> FindAll(K key)
        {
            var returnItems = new List<T>();
            
            foreach (var ih in _internalHolders)
            {
                if (ih.Key.Equals(key))
                {
                    returnItems.Add(ih.Item);
                }
            }
            return returnItems;
        }

        public bool Exists(K key, T item)
        {
            foreach (var ih in _internalHolders)
            {
                if (ih.Key.Equals(key))
                {
                    if (ih.Item.Equals(item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Add(K key, T item)
        {
            if (this.Exists(key, item))
            {
                throw new System.ArgumentException(
                        string.Format("This key/item pair already   exists.({0},{1})", key.ToString(), item.ToString()));
            }
            _internalHolders.Add(new InternalHolder<K, T>(key, item));
        }
    }

    internal class InternalHolder<K, T>
    {
        private readonly K _key;
        private readonly T _item;
        
        public K Key
        {
            get { return _key; }
        }

        public T Item
        {
            get { return _item; }
        }

        public InternalHolder(K key, T item)
        {
            _key = key;
            _item = item;
        }
    }
}