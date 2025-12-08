using System.Collections.Generic;

using UnityEngine;

namespace ACE2EU {

    public class Inventory: MonoBehaviour {

        private void Awake() {
            Instance = this;
        }

        public static Inventory Instance { get; private set; }

        [Header("Prefabs")]
        [SerializeField]
        private InventoryItem _item;


        private static List<InventoryItem> _items = new List<InventoryItem>();

        public void Add(Storable storable) {

            int position = _items.FindIndex(item => item.Object.Type == storable.Type);

            if (position < 0) {
                var item = Instantiate(_item, transform);

                item.Initialize(storable);

                _items.Add(item);
            } else {
                ++_items[position].Count;

                Destroy(storable.gameObject);
            }
        }

        public Storable Remove(StorableType type) {

            if (!_items.Exists(item => item.Object.Type == type)) {
                return null;
            }

            var item = _items.Find(item => item.Object.Type == type);

            if (--item.Count == 0) {
                _items.Remove(item);

                var storabale = item.Object;

                return storabale;
            } else {
                return Instantiate(item.Object);
            }
        }

        public int Contains(StorableType type) {

            if (_items.Count == 0) {
                return 0;
            }

            if(!_items.Exists(item => item.Object.Type == type)) {
                return 0;
            }

            return _items.Find(item => item.Object.Type == type).Count;
        }

        public void Clear() {

            while (_items.Count > 0) {
                var item = _items[0];
                Destroy(item.gameObject);
                _items.RemoveAt(0);
            }
        }
    }
}
