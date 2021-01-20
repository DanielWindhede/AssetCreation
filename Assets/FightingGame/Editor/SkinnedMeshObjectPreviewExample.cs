using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

// based on: https://www.blog.radiator.debacle.us/2016/06/working-with-custom-objectpreviews-and.html

//[CustomPreview(typeof(Character))]
public class SkinnedMeshObjectPreviewExample : ObjectPreview
{
	private PreviewRenderUtility _previewUtility;
	private string _previewInstanceName = "HitboxViewerPreviewInstance";

	GameObject previewPrefab, previewInstance;
	SkinnedMeshRenderer skinMeshRender;
	Mesh previewMesh;
	Material previewMaterial;

	static Vector2 previewDir = new Vector2(-180f, 0f);
	static float lerpSpeed = 0.5f;

	// very important to override this, it tells Unity to render an ObjectPreview at the bottom of the inspector
	public override bool HasPreviewGUI() { return true; }

	private void Initialize()
	{
		if (this._previewUtility == null)
		{
			this._previewUtility = new PreviewRenderUtility();
			this._previewUtility.cameraFieldOfView = 27f;
			RefreshPreviewInstance();
		}
	}

	void RefreshPreviewInstance()
	{
		// if we already instantiated a PreviewInstance previously but just lost the reference, then use that same instance instead of making a new one
		var oldInstance = GameObject.Find(_previewInstanceName);

		if (oldInstance != null)
		{
			previewInstance = oldInstance;
		}
		else
		{
			// no previous instance detected, so now let's make a fresh one
			// very important: this loads the PreviewInstance prefab and temporarily instantiates it into PreviewInstance

			//previewPrefab = (GameObject)AssetDatabase.LoadAssetAtPath<GameObject>("Assets/FightingGame/TempGraphics/prefab.prefab");
			previewPrefab = ((Character)target).gameObject;
			previewInstance = (GameObject)GameObject.Instantiate(previewPrefab, previewPrefab.transform.position, previewPrefab.transform.rotation);
			previewInstance.name = _previewInstanceName;

			// HideFlags are special editor-only settings that let you have *secret* GameObjects in a scene, or to tell Unity not to save that temporary GameObject as part of the scene
			previewInstance.hideFlags = HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild; // you could also hide it from the hierarchy or inspector, but personally I like knowing everything that's there
		}

		var sRenders = previewInstance.GetComponentsInChildren<SkinnedMeshRenderer>();

		// when instantiating stuff in the scene with HideFlags, that stuff often gets broken when loading a new scene...
		if (sRenders == null || sRenders.Length == 0)
		{
			// so I setup a check to detect that case and clean-up
			Debug.Log("cleaning up leftover PreviewInstance!");
			OnDestroy();
			RefreshPreviewInstance();
			return;
		}
		else
		{
			skinMeshRender = sRenders[0];
		}

		previewMaterial = skinMeshRender.sharedMaterial; // use sharedMaterial or else you will instantiate a new material
	}

	// the main ObjectPreview function... it's called constantly, like other IMGUI On*GUI() functions
	public override void OnPreviewGUI(Rect r, GUIStyle background)
	{
		// if this is happening, you have bigger problems
		if (!ShaderUtil.hardwareSupportsRectRenderTexture)
		{
			if (Event.current.type == EventType.Repaint)
			{
				EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Mesh preview requires\nrender texture support");
			}
			return;
		}

		Initialize();

		previewDir = Drag2D(previewDir, r);
		if (Event.current.type != EventType.Repaint)
		{
			// if we don't need to update yet, then don't
			return;
		}

		_previewUtility.BeginPreview(r, background); // set up the PreviewRenderUtility's mini internal scene

		DoRenderPreview();

		Texture image = _previewUtility.EndPreview(); // grab the RenderTexture resulting from DoRenderPreview() > RenderMeshPreview() > PreviewRenderUtility.m_Camera.Render()
		GUI.DrawTexture(r, image, ScaleMode.StretchToFill, false); // draw the RenderTexture in the ObjectPreview pane
		EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), target.name);
	}
	private void DoRenderPreview()
	{
		if (skinMeshRender == null)
		{
			RefreshPreviewInstance();
		}

		// very important: we have to call BakeMesh, to bake that animated SkinnedMesh into a plain static Mesh suitable for Graphics.DrawMesh()
		previewMesh = new Mesh();
		skinMeshRender.BakeMesh(previewMesh);

		// now, actually render out the RenderTexture
		RenderMeshPreview(previewMesh, _previewUtility, previewMaterial, previewDir, -1);
	}

	void RenderMeshPreview(Mesh mesh, PreviewRenderUtility previewUtility, Material material, Vector2 direction, int meshSubset)
	{
		if (mesh == null || previewUtility == null)
		{
			return;
		}
		// Measure the mesh's bounds so you know where to put the camera and stuff
		Bounds bounds = mesh.bounds;
		float magnitude = bounds.extents.magnitude;
		float distance = 8f * magnitude;

		// setup the ObjectPreview's camera
		previewUtility.camera.backgroundColor = Color.gray;
		previewUtility.camera.clearFlags = CameraClearFlags.Color;
		previewUtility.camera.transform.position = new Vector3(0f, 1f, -0.5f); // this used to be "-Vector3.forward * num" but I hardcoded my camera position instead
		previewUtility.camera.transform.rotation = Quaternion.identity;
		previewUtility.camera.nearClipPlane = 0.3f;
		previewUtility.camera.farClipPlane = distance + magnitude * 1.1f;

		// figure out where to put the model
		Quaternion quaternion = Quaternion.Euler(0f, direction.x, 0f); // this used to have "Quaternion.Euler(direction.y, 0f, 0f) * " to apply pitch rotation as well, but I didn't need it
		Vector3 pos = quaternion * -bounds.center;

		// we are technically rendering everything in the scene, so scene fog might affect it...
		bool fog = RenderSettings.fog; // ... let's remember the current fog setting...
		Unsupported.SetRenderSettingsUseFogNoDirty(false); // ... and then temporarily turn it off

		// submesh support, in case the mesh is made of multiple parts
		int subMeshCount = mesh.subMeshCount;
		if (meshSubset < 0 || meshSubset >= subMeshCount)
		{
			for (int i = 0; i < subMeshCount; i++)
			{
				// PreviewRenderUtility.DrawMesh() actually draws the mesh
				previewUtility.DrawMesh(mesh, pos, quaternion, material, i);
			}
		}
		else
		{
			// no submeshes here so let's just draw it normally
			previewUtility.DrawMesh(mesh, pos, quaternion, material, meshSubset);
		}
		// VERY IMPORTANT: this manually tells the camera to render and produce the render texture
		previewUtility.camera.Render();

		// reset the scene's fog from before
		Unsupported.SetRenderSettingsUseFogNoDirty(fog);
	}

	// this is where you draw any settings or controls above the ObjectPreview
	public override void OnPreviewSettings()
	{
		GUILayout.Label("LerpSpeed: ");
		lerpSpeed = EditorGUILayout.Slider(lerpSpeed, 0.1f, 0.5f, GUILayout.Width(100f));
	}

	// cleanup stuff so we don't leak everywhere
	public void OnDestroy()
	{
		Debug.Log(_previewInstanceName + " OnDestroy()");
		if (this._previewUtility != null)
		{
			this._previewUtility.Cleanup();
			this._previewUtility = null;
		}
		if (previewInstance != null)
		{
			GameObject.DestroyImmediate(previewInstance);
		}
	}

    // from http://timaksu.com/post/126337219047/spruce-up-your-custom-unity-inspectors-with-a
    // this is our inlined version of EditorGUIUtility.Draw2D(), which is internal or something
    public static Vector2 Drag2D(Vector2 scrollPosition, Rect position)
    {
        int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
        Event current = Event.current;
        switch (current.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                if (position.Contains(current.mousePosition) && position.width > 50f)
                {
                    GUIUtility.hotControl = controlID;
                    current.Use();
                    EditorGUIUtility.SetWantsMouseJumping(1);
                }
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == controlID)
                {
                    GUIUtility.hotControl = 0;
                }
                EditorGUIUtility.SetWantsMouseJumping(0);
                break;
            case EventType.MouseDrag:
                if (GUIUtility.hotControl == controlID)
                {
                    scrollPosition -= current.delta * (float)((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height) * 140f;
                    scrollPosition.y = Mathf.Clamp(scrollPosition.y, -90f, 90f);
                    current.Use();
                    GUI.changed = true;
                }
                break;
        }
        return scrollPosition;
    }


	/*
	

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

public override void OnInspectorGUI()
{
	base.OnInspectorGUI();

	// Draw some things, omitted for shortness...
	EditorGUILayout.BeginVertical("box");
	DrawPreviewWindow();
	EditorGUILayout.EndVertical();

}
*/
}