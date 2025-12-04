using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ACE2EU {

    public class ShowDialogue: MonoBehaviour {

        [Serializable]
        private struct Dialog {

            [SerializeField]
            [TextArea(3, 10)]
            private List<string> _phrases;
            private int _position;

            public void Show() {

                if(_phrases.Count == 0) {
                    return;
                }

                string text = _phrases[_position++];

                if(_position >= _phrases.Count) {
                    _position = 0;
                    PlayerController.Instance.ShowInformation(text);
                } else {
                    PlayerController.Instance.ShowInformation(text, Show);
                }
            }
        }




        [SerializeField]
        //[TextArea(3, 10)]
        private List<Dialog> _dialogues = new List<Dialog>();

        public void Show(int position) {

            if (_dialogues.Count == 0) return;

            if (_dialogues.Count < position) return;

            _dialogues[position].Show();
        }
    }
}
