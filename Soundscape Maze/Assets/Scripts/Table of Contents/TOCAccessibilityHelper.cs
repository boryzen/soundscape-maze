using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace TableOfContents
{
    [Serializable]
    public class ContentItem
    {
        public List<KeyCode> Keys;
        public AudioClip AccessibilityAudio;
        public SceneReference SceneToOpen;
    }

    public class TOCAccessibilityHelper : MonoBehaviour
    {
        [SerializeField]
        private List<ContentItem> Contents = new List<ContentItem>();

        [SerializeField]
        private List<KeyCode> selectSceneKey = new List<KeyCode>() { KeyCode.Return, KeyCode.KeypadEnter };

        [SerializeField]
        private AudioSource audioPlayer;

        [SerializeField]
        private SceneSwitcher sceneSwitcher;

        private ContentItem lastSelection;

        // Update is called once per frame
        void Update()
        {
            // Check if the user previously selected an item in the table...
            if (lastSelection != null && selectSceneKey.Exists(code => Input.GetKeyUp(code)))
            {
                // They hit the selection key, so navigate to that scene
                sceneSwitcher.ChangeScene(lastSelection.SceneToOpen);
                return;
            }

            // Otherwise, check if they are tapping any buttons
            var item = Contents.Find(item => item.Keys.Exists(code => Input.GetKeyUp(code)));

            if (item == null)
                return;

            if (audioPlayer.isPlaying)
                audioPlayer.Stop();

            // They tapped one of the keys connected to an item in the table of contents... Play the corresponding audio
            audioPlayer.clip = item.AccessibilityAudio;
            audioPlayer.Play();
            lastSelection = item;
        }
    }
}
