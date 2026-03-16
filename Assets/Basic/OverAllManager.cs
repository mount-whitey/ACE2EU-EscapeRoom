using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ACE2EU {

    public enum Scene {
        BASE = 0,
        MUG = 1,
        THI = 2,
        CUAS = 3,
        UFV = 4,
        UCB = 5
    }

    public class OverAllManager: MonoBehaviour {

        public static OverAllManager Instance = null;

        public Action<Scene> OnSceneGetsLoaded = (_) => { };
        public Action<Scene> OnSceneAvailable = (_) => { };

        [SerializeField]
        private Dictionary<Scene, GameObject> _sceneInfo = new Dictionary<Scene, GameObject>();
        [SerializeField]
        private GameObject _sceneObject = null;


        public Scene Scene {
            get {
                return _current;
            }
        }
        private Scene _current;

        private bool _firstEncounter { get; set; } = true;

        public Pose InitialPose { get; set; } = new Pose(new Vector3(0, 0, 0), Quaternion.identity);

        private void Awake() {

            if (Instance != null) {
                Destroy(gameObject);
                return;
            }

            // References
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start() {

            if (Instance != this) {
                Destroy(gameObject);
                return;
            }

            _current = Scene.BASE;

            foreach (Scene scene in Enum.GetValues(typeof(Scene))) {
                InitializeScene(scene);
            }

            ShowPanel();
        }

        private void InitializeScene(Scene scene) {

            OnSceneGetsLoaded?.Invoke(scene);

            Addressables.LoadAssetAsync<GameObject>(scene.ToString()).Completed += (handle) => {

                if (_sceneInfo.ContainsKey(scene)) {
                    _sceneInfo[scene] = handle.Result;
                    Initialize();
                } else {
                    _sceneInfo.Add(scene, handle.Result);
                    Debug.Log("SCENE - " + scene);
                    OnSceneAvailable?.Invoke(scene);
                }
            };
        }

        public void LoadScene(Scene scene) {

            Deactivate();

            _current = scene;

            Activate(_current);
        }

        public bool IsSceneAvailable(Scene scene) {
            return _sceneInfo.ContainsKey(scene);
        }

        private void Activate(Scene scene) {
            Initialize();
        }

        private void Deactivate() {

            Destroy(_sceneObject);
            _sceneObject = null;
        }

        private void Initialize() {

            // Content
            var goINST = Instantiate(_sceneInfo[_current], Vector3.zero, Quaternion.identity);
            goINST.SetActive(true);
            _sceneObject = goINST;

            // Settings
            PlayerController.Instance.JustForward(false);
            PlayerController.Instance.CanMove = false;

            // Inventory
            Inventory.Instance.Clear();

            // Panel
            ShowPanel();
        }

        private void ShowPanel() {

            // Initial
            switch (_current) {
                case Scene.BASE:
                    if (_firstEncounter) {
                        _firstEncounter = false;
                        PlayerController.Instance.ShowHeader(prePart: "Welcome to the ACE²-EU:", mainLeft: "<font-weight=\"900\">Escape", mainRight: "<font-weight=\"900\">Room", postPart: "Experience our Alliance universities in an\ninteractive, playful way - <font-weight=\"700\">let’s begin!");
                    } else {
                        PlayerController.Instance.ShowHeader(mainLeft: "<font-weight=\"900\">Welcome back!", mainRight: "We hope you had a great time\nexploring our escape room.");
                    }

                    transform.SetPositionAndRotation(InitialPose.position, InitialPose.rotation);

                    break;
                case Scene.THI:
                    PlayerController.Instance.ShowHeader(mainLeft: "<font-weight=\"900\">Germany", mainRight: "Technische Hochschule\nIngolstadt - THI");
                    break;
                case Scene.MUG:
                    PlayerController.Instance.ShowHeader(mainLeft: "<font-weight=\"900\">Poland", mainRight: "Medical University\nof Gdańsk - MUG");
                    break;
                case Scene.CUAS:
                    PlayerController.Instance.ShowHeader(mainLeft: "<font-weight=\"900\">Austria", mainRight: "Carinthia University of\nApplied Sciences - CUAS");
                    break;
                case Scene.UFV:
                    PlayerController.Instance.ShowHeader(mainLeft: "<font-weight=\"900\">Spain", mainRight: "Universidad Francisco\nde Vitoria - UFV");
                    break;
                case Scene.UCB:
                    PlayerController.Instance.ShowHeader(mainLeft: "<font-weight=\"900\">Romania", mainRight: "University Constantin\nBrâncuşi - UCB");
                    break;
            }

            PlayerController.Instance.ShowHome(_current != Scene.BASE);
            PlayerController.Instance.transform.SetPositionAndRotation(InitialPose.position, InitialPose.rotation);
            PlayerController.Instance.FadeIn();
        }
    }
}
