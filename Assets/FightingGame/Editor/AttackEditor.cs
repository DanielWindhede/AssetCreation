using UnityEditor;
using UnityEngine;

namespace Fami.FightingGame
{
    [CustomEditor(typeof(Attack))]
    public class AttackEditor : Editor
    {
        [SerializeField] private AnimationClip clip;
        private GameObject gameObject;
        private Editor gameObjectEditor;

        private const int FPS = 60;
        private int _scrubFrames;
        private int ScrubFrames
        {
            get { return CurrentFrame; }
            set
            { // clip null check already present in if-statement
                if (value < 0)
                    _scrubFrames = 0;
                else if (value > TotalFrames)
                    _scrubFrames = TotalFrames;
                else
                    _scrubFrames = value;
            }
        }

        private float ScrubTime { get { return (float)ScrubFrames / (float)FPS; } }

        private int CurrentFrame { get { return _scrubFrames; } }
        private int TotalFrames { get { return (int)(clip.length * FPS); } }

        /*
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Attack attack = (Attack)target;

            if (GUILayout.Button("Aa"))
            {
                print("aa");
            }
            //EditorGUILayout
        }
        */

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // Draw some things, omitted for shortness...
            EditorGUILayout.BeginVertical("box");
            DrawPreviewWindow();
            EditorGUILayout.EndVertical();

        }

        public override void OnPreviewSettings()
        {
            base.OnPreviewSettings();
        }

        private bool toggle = true;
        private void DrawPreviewWindow()
        {
            toggle = EditorGUILayout.Toggle("Use Selected GameObject", toggle);

            if (toggle)
                gameObject = ((Attack)target).gameObject;
            else
                gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);

            clip = (AnimationClip)EditorGUILayout.ObjectField(clip, typeof(AnimationClip), true);


            if (gameObject != null)
            {

                if (gameObjectEditor == null)
                    gameObjectEditor = Editor.CreateEditor(gameObject);

                if (clip != null)
                {
                    //scrubTime = EditorGUILayout.Slider(scrubTime, 0f, clip.length);
                    EditorGUILayout.LabelField("Frame Advance", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                    ScrubFrames = EditorGUILayout.IntSlider((int)ScrubFrames, 0, (int)(clip.length * FPS));
                    print("a");
                    EditorGUILayout.BeginHorizontal();

                    if (GUILayout.Button("-"))
                        ScrubFrames--;
                    else if (GUILayout.Button("+"))
                        ScrubFrames++;

                    EditorGUILayout.EndHorizontal();

                    UpdateAnimation();

                }


                GUIStyle bgColor = new GUIStyle();
                gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(256, 256), bgColor);

                Handles.BeginGUI();
                Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1f, EventType.Layout);
                Handles.EndGUI();

                gameObjectEditor.ReloadPreviewInstances();
            }
        }

        private void UpdateAnimation()
        {
            if (!AnimationMode.InAnimationMode())
            {
                AnimationMode.StartAnimationMode();
            }
            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(gameObjectEditor.target as GameObject, clip, ScrubTime);
            AnimationMode.EndSampling();
        }


        private void print(string value)
        {
            Debug.Log(value);
        }
    }
}