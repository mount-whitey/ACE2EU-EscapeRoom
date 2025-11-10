using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ACE2EU {

    public class InventoryItem: MonoBehaviour {

        [Header("Prefabs")]
        [SerializeField]
        private RenderTexture _renderTexture;

        [Header("Prefabs")]
        [SerializeField]
        private Transform _container;

        private RenderTexture _texture;
        private Camera _camera;
        private RawImage _image;
        private TMP_Text _counter;

        public Storable Object { get; private set; } = null;

        public int Count {
            get {
                return _count;
            }
            set {
                _count = value;

                if (_counter != null) {
                    _counter.text = _count.ToString();
                }
            }
        }
        private int _count = 1;

        private void Awake() {

            _camera = GetComponentInChildren<Camera>(true);
            _image = GetComponentInChildren<RawImage>(true);
            _counter = GetComponentInChildren<TMP_Text>(true);

            _texture = Instantiate(_renderTexture);

            _camera.targetTexture = _texture;
            _image.texture = _texture;

            _camera.enabled = true;
        }

        private void Update() {
            if(_count == 0){
                Destroy(gameObject);
            }
        }

        public void Initialize(Storable storable) {

            storable.transform.parent = _container;
            storable.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            storable.transform.localScale = Vector3.one;

            Object = storable;
        }
    }
}
