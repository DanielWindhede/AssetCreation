using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MoveEditorWindow : EditorWindow
{
	private GameObject gameObject;
	private HitboxDrawer _drawer;
	private HitboxDrawer Drawer
    {
		get
        {
			if (_drawer == null)
				_drawer = gameObject.GetComponent<HitboxDrawer>();
			return _drawer;
        }
    }

	static MoveEditorWindow moveEditorWindow;
	static Move move;
	private int ScrubFrames
	{
		get { return CurrentFrame; }
		set
		{ // clip null check already present in if-statement
			SmoothScrubFrames = value;
		}
	}

	private float _smoothScrubFrames;
	private float SmoothScrubFrames
    {
		get { return _smoothScrubFrames; }
		set
        {
			if (value < 0)
				_smoothScrubFrames = 0;
			else if (value > TotalFrames)
				_smoothScrubFrames = TotalFrames;
			else
				_smoothScrubFrames = value;
		}
    }

	private float ScrubTime { get { return (float)SmoothScrubFrames / (float)FPS; } }

	private int CurrentFrame { get { return (int)_smoothScrubFrames; } }
	private int TotalFrames { get { return (int)(move.clip.length * FPS); } }


	private const int FPS = 60;
	private bool generalOptions = true;

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/Move Editor")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(MoveEditorWindow));
		Initialize();
	}

	private static void Initialize()
	{
		UnityEngine.Object[] selection = Selection.GetFiltered(typeof(Move), SelectionMode.Assets);
		if (selection.Length > 0)
		{
			if (selection[0] == null)
				return;
			move = (Move)selection[0];
			//fpsTemp = moveInfo.fps;
			//animationSpeedTemp = moveInfo.animationSpeed;
			//totalFramesTemp = moveInfo.totalFrames;
		}
	}

	private void UpdateAnimation()
	{
		if (move != null)
		{
			if (!AnimationMode.InAnimationMode())
			{
				AnimationMode.StartAnimationMode();
			}
			AnimationMode.BeginSampling();
			AnimationMode.SampleAnimationClip(gameObject, move.clip, ScrubTime);
			AnimationMode.EndSampling();
		}
	}

	string title = "Select a GameObject";
	void runAnimation()
	{
		isPlaying = true;
    }

	bool isPlaying;
	float animationSpeed = 1;
	bool loopAnimation;
	bool drawHitboxes = true;
	bool useSmoothScrubbing = false;
	bool showHitboxDropdown = false;
	void OnGUI()
	{
		/*
		GUILayout.Label("Base Settings", EditorStyles.boldLabel);
		e = EditorGUILayout.Slider("Slider", e, -3, 3);
		EditorGUILayout.EndToggleGroup();
		*/

		//e = EditorGUILayout.Slider("Slider", e, -3, 3);
		//EditorGUILayout.EndToggleGroup();


		if (move == null)
			Initialize();

		if (move == null)
		{
			GUILayout.BeginHorizontal("GroupBox");
			GUILayout.Label("Select a move file first\nor create a new move.");
			GUILayout.EndHorizontal();
			EditorGUILayout.Space();
			if (GUILayout.Button("Create new move"))
				CreateAsset<Move>();
			return;
		}

		GUIStyle titleStyle = new GUIStyle();
		titleStyle.padding = new RectOffset(0, 0, 10, 10);
		titleStyle.alignment = TextAnchor.MiddleCenter;
		titleStyle.fontSize = 16;
		titleStyle.fontStyle = FontStyle.Bold;


		EditorGUILayout.BeginVertical(titleStyle);
		{
			EditorGUILayout.BeginHorizontal();
			{
				//EditorGUILayout.LabelField(move.moveName == "" ? "New Move" : move.moveName, GUILayout.Height(32));
				EditorGUILayout.LabelField(move.moveName == "" ? "New Move" : move.moveName, titleStyle);
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();



		move.moveName = EditorGUILayout.TextField("Move Name (Unique Id):", move.moveName);
		move.clip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip:", move.clip, typeof(AnimationClip), true);
		if (move.clip == null)
		{
			return;
		}
		else
		{
			gameObject = (GameObject)EditorGUILayout.ObjectField("Animation Target:", gameObject, typeof(GameObject), true);
			if (gameObject != null)
            {
				Drawer.move = move;
				EditorGUILayout.BeginHorizontal();
				{
					animationSpeed = EditorGUILayout.Slider("Animation Speed", animationSpeed, 0f, 1f);
					loopAnimation = EditorGUILayout.Toggle("Loop", loopAnimation);
					if (GUILayout.Button("Run animation"))
						runAnimation();
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginVertical("animation");
				{
					EditorGUILayout.BeginHorizontal();
					{
						Drawer.drawHitboxes = EditorGUILayout.Toggle("Draw hitboxes", Drawer.drawHitboxes);
						useSmoothScrubbing = EditorGUILayout.Toggle("Smooth scrubframes", useSmoothScrubbing);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.LabelField("Frame Advance", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });

					if (useSmoothScrubbing)
						SmoothScrubFrames = EditorGUILayout.Slider(SmoothScrubFrames, 0, (int)(move.clip.length * FPS));
					else
						ScrubFrames = EditorGUILayout.IntSlider((int)ScrubFrames, 0, (int)(move.clip.length * FPS));

					EditorGUILayout.BeginHorizontal();
					{
						if (GUILayout.Button("-"))
							ScrubFrames--;
						else if (GUILayout.Button("+"))
							ScrubFrames++;
					}
					EditorGUILayout.EndHorizontal();

					Drawer.frame = ScrubFrames;

					/*
					GUIStyle hitboxStyle = new GUIStyle();
					titleStyle.padding = new RectOffset(10, 10, 100, 10);
					titleStyle.margin = new RectOffset(10, 10, 100, 10);
					titleStyle.alignment = TextAnchor.MiddleCenter;
					titleStyle.fontSize = 22;
					*/
					EditorGUILayout.BeginVertical("ObjectFieldThumb");
					{
						showHitboxDropdown = EditorGUILayout.Foldout(showHitboxDropdown, "Hitboxes", true, EditorStyles.foldout);
						EditorGUI.indentLevel = 1;
						if (showHitboxDropdown)
						{
							if (move.hitCollection != null)
							{
								StyledMarker();
								for (int i = 0; i < move.hitCollection.Length; i++)
								{
									/*
									move.hitCollection[i].hitboxEditorToggle = EditorGUILayout.Foldout(move.hitCollection[i].hitboxEditorToggle,
										"Hitbox (" + i + ")" + ", P(" + move.hitCollection[i].priority.ToString() + ")", true, EditorStyles.foldout);

									if (move.hitCollection[i].hitboxEditorToggle)
									{
										*/
									EditorGUILayout.BeginVertical("ObjectFieldThumb");
									{
										move.hitCollection[i].isMultiHit = EditorGUILayout.Toggle("Is multi-hit:", move.hitCollection[i].isMultiHit);
										if (move.hitCollection[i].isMultiHit)
										{
											//move.hitCollection[i].frameStart = EditorGUILayout.IntField("First Active Frame:", move.hitCollection[i].frameStart);
											//StyledSlider(ref move.hitCollection[i].frameStart, 0, TotalFrames, EditorGUI.indentLevel);

											Rect tempRect = GUILayoutUtility.GetRect(1, 10);
											Rect rect = new Rect(EditorGUI.indentLevel, tempRect.y, position.width - EditorGUI.indentLevel - 100, 20);

											Hit hit = move.hitCollection[i];
											int availableTotalFrames = TotalFrames - hit.multiHitTimes * hit.multiHitActiveFrames - (hit.multiHitTimes - 1) * hit.multiHitDelay + 1;
											move.hitCollection[i].frameStart = EditorGUILayout.IntSlider("Active: ", move.hitCollection[i].frameStart, 0, availableTotalFrames);
											EditorGUILayout.BeginHorizontal();
											{
												move.hitCollection[i].multiHitTimes = EditorGUILayout.IntField("Hit times:", hit.multiHitTimes);
												move.hitCollection[i].multiHitTimes = Mathf.Max(1, hit.multiHitTimes);

												move.hitCollection[i].multiHitDelay = EditorGUILayout.IntField("Hit delay:", hit.multiHitDelay);
												move.hitCollection[i].multiHitDelay = Mathf.Max(0, hit.multiHitDelay);

												move.hitCollection[i].multiHitActiveFrames = EditorGUILayout.IntField("Active frames:", hit.multiHitActiveFrames);
												move.hitCollection[i].multiHitActiveFrames = Mathf.Max(1, hit.multiHitActiveFrames);
											}
											EditorGUILayout.EndHorizontal();

											int totalFrameEnd = (hit.frameStart + (hit.multiHitTimes - 1) * hit.multiHitDelay + hit.multiHitTimes * hit.multiHitActiveFrames - 1);
											GUILayout.Label("Active frames: " + hit.frameStart + " - " + totalFrameEnd.ToString());
										}
										else
										{
											StyledMinMaxSlider(ref move.hitCollection[i].frameStart, ref move.hitCollection[i].frameEnd, 0,
															  (int)(move.clip.length * FPS), EditorGUI.indentLevel);
										}

										GUILayout.Label("Bounds");
										EditorGUILayout.BeginHorizontal();
										{
											move.hitCollection[i].bounds.x = EditorGUILayout.FloatField("X:", move.hitCollection[i].bounds.x);
											move.hitCollection[i].bounds.y = EditorGUILayout.FloatField("Y:", move.hitCollection[i].bounds.y);
										}
										EditorGUILayout.EndHorizontal();

										EditorGUILayout.BeginHorizontal();
										{
											move.hitCollection[i].bounds.width = EditorGUILayout.FloatField("Width:", move.hitCollection[i].bounds.width);
											move.hitCollection[i].bounds.height = EditorGUILayout.FloatField("Height:", move.hitCollection[i].bounds.height);
										}
										EditorGUILayout.EndHorizontal();

										EditorGUILayout.Space(5);
										move.hitCollection[i].priority = EditorGUILayout.IntField("Priority", move.hitCollection[i].priority);
										move.hitCollection[i].damage = EditorGUILayout.FloatField("Damage", move.hitCollection[i].damage);
									}
									EditorGUILayout.Space(10);
									if (GUILayout.Button("Remove"))
										move.hitCollection = RemoveElement<Hit>(move.hitCollection, move.hitCollection[i]);

									EditorGUILayout.EndVertical();
									//}
								}
							}

							EditorGUILayout.Space(25);

							if (GUILayout.Button("Add hitbox"))
							{
								move.hitCollection = AddElement<Hit>(move.hitCollection, new Hit());
							}
						}

						EditorGUI.indentLevel = 0;
					}
					EditorGUILayout.EndVertical();


					/*
                    for (int i = 0; i < move.hitCollection.Length; i++)
					{
						bool hitboxFoldout = true;
						hitboxFoldout = EditorGUILayout.Foldout(hitboxFoldout, move.hitCollection[i].priority.ToString(), true, EditorStyles.foldout);
						if (hitboxFoldout)
						{
							EditorGUILayout.BeginVertical();
							{
								EditorGUILayout.BeginHorizontal();
								{
									move.hitCollection[i].bounds.x = EditorGUILayout.FloatField("x:", move.hitCollection[i].bounds.x);
									move.hitCollection[i].bounds.width = EditorGUILayout.FloatField("width:", move.hitCollection[i].bounds.width);
								}
								EditorGUILayout.EndHorizontal();

								EditorGUILayout.BeginHorizontal();
								{
									move.hitCollection[i].bounds.y = EditorGUILayout.FloatField("y:", move.hitCollection[i].bounds.y);
									move.hitCollection[i].bounds.height = EditorGUILayout.FloatField("height:", move.hitCollection[i].bounds.height);
								}
								EditorGUILayout.EndHorizontal();
							}
							EditorGUILayout.EndVertical();
						}
					}
					*/
					if (!isPlaying)
						UpdateAnimation();

					/*
					if (drawHitboxes && move.hitCollection.Length > 0)
					{
						foreach (Hit hit in move.hitCollection.Where(x => x.frameStart <= ScrubFrames && x.frameEnd >= ScrubFrames))
						{
							Move.DrawHitbox(hit, gameObject.transform);
						}
						SceneView.RepaintAll();
					}
					*/
					
				}
				EditorGUILayout.EndVertical();


				if (GUI.changed)
				{
                    foreach (var hit in move.hitCollection)
                    {
						hit.CalculateMultiHitFrames();
                    }

					Undo.RecordObject(move, "Move Editor Modify");

					EditorUtility.SetDirty(move);
				}
			}

			//UpdateAnimation();

			//generalOptions = EditorGUILayout.Foldout(generalOptions, "General");

			//totalFramesOriginal = (int)Mathf.Abs(Mathf.Ceil((move.fps * move.animMap.clip.length) / (float)move._animationSpeed));
			//totalFramesOriginal = (int)Mathf.Abs(Mathf.Ceil((move.fps * move.animMap.clip.length) / (float)move._animationSpeed));
			//if (move.totalFrames == 0)
			//{
			//	move.totalFrames = totalFramesOriginal;
			//}
			//if (moveInfo.totalFrames > totalFramesGlobal)
			//	totalFramesGlobal = moveInfo.totalFrames;
		}

		HashSet<Vector2Int> fillBounds;
		int hitCollectionLength;

		//void StyledMarker(string label, Vector3[] locations, int maxValue, int indentLevel, bool fillBounds)
		void StyledMarker()
		{
			Rect tempRect = GUILayoutUtility.GetRect(1, 20);
			Rect rect = new Rect(30, tempRect.y, position.width - 30 * 2, 20);
			GUI.Box(rect, "", "ProgressBarBack");

			List<Vector2Int> activeFrames = new List<Vector2Int>();
            for (int i = 0; i < move.hitCollection.Length; i++)
            {
				if (move.hitCollection[i].isMultiHit)
				{
					for (int j = 0; j < move.hitCollection[i].MultiFrameStart.Length; j++)
					{
						Vector2Int v = new Vector2Int();
						v.x = move.hitCollection[i].MultiFrameStart[j];
						v.y = move.hitCollection[i].MultiFrameEnd[j];
						activeFrames.Add(v);
					}
				}
				else
				{
					activeFrames.Add(new Vector2Int(move.hitCollection[i].FrameStart, move.hitCollection[i].FrameEnd));
				}
            }

			if (activeFrames.Count > 0)
			{
				string redColor = "flow node 6 on";
				string orangeColor = "flow node 5 on";
				string cyanColor = "flow node 2 on";
				string greenColor = "flow node 3 on";

				int lastFrame = 0;
				float unit = rect.width / TotalFrames;
				foreach (Vector2 i in activeFrames)
				{
					//lastLocation = i.y;
					//fillLeftPos = ((rect.width / TotalFrames) * i.x) + rect.x;
					//fillRightPos = ((rect.width / TotalFrames) * i.y) + rect.x;

					float left = rect.x + unit * i.x;
					float right = unit * (i.y - i.x);

					if (lastFrame < i.y)
						lastFrame = (int)i.y;

					// Active
					GUI.Box(new Rect(left, rect.y, right, rect.height), new GUIContent(), greenColor);


					//fillWidth += (rect.width/maxValue);
					//fillLeftPos -= (rect.width/maxValue);
					//GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), "flow node 5 on");

					/*
					if (i.z > 0)
					{
						GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), "flow node 5 on");
					}
					else
					{
						GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), "flow node 2 on");
					}
					*/
				}

				// Lag
				//GUI.Box(new Rect(unit * (TotalFrames - lastFrame), rect.y, rect.width - unit * (TotalFrames - lastFrame), rect.height), new GUIContent(), redColor);

				/*
				if (activeFrames.Count > 0 && lastLocation < TotalFrames)
				{
					float fillWidth = rect.width - fillRightPos + rect.x;
					GUI.Box(new Rect(fillRightPos, rect.y, fillWidth, rect.height), new GUIContent(), "flow node 6 on");
				}
				*/

				EditorGUILayout.Space();
				GUILayout.BeginHorizontal("HelpBox");
				{
					/*
					labelStyle.normal.textColor = Color.yellow;
					moveInfo.startUpFrames = locations[0].x <= 0 ? 0 : moveInfo.hits[0].activeFramesBegin - 1;
					GUILayout.Label("Start Up: " + moveInfo.startUpFrames, labelStyle);
					labelStyle.normal.textColor = Color.cyan;
					move.activeFrames = (moveInfo.hits[moveInfo.hits.Length - 1].activeFramesEnds - moveInfo.hits[0].activeFramesBegin);
					GUILayout.Label("Active: " + moveInfo.activeFrames, labelStyle);
					labelStyle.normal.textColor = Color.red;
					moveInfo.recoveryFrames = lastLocation >= maxValue ? 0 : (moveInfo.totalFrames - moveInfo.hits[moveInfo.hits.Length - 1].activeFramesEnds + 1);
					GUILayout.Label("Recovery: " + moveInfo.recoveryFrames, labelStyle);
					*/
				}
				GUILayout.EndHorizontal();
			}

			/*
			if (hitCollectionLength != move.hitCollection.Length)
			{
				*/
			//fillBounds.Clear();
			/*
			for (int i = 0; i < move.hitCollection.Length; i++)
				{
					Hit hit = move.hitCollection[i];
					if (hit.isMultiHit)
					{
						for (int time = 1; time <= hit.multiHitTimes; time++)
						{
							Vector2Int vec = new Vector2Int();

							vec.x = hit.frameStart + (time - 1) * hit.multiHitDelay + (time - 1) * hit.multiHitActiveFrames;
							vec.y = hit.frameStart + time * hit.multiHitActiveFrames - 1 + (time - 1) * hit.multiHitDelay;

							fillBounds.Add(vec);
						}
					}
					else
					{
						fillBounds.Add(new Vector2Int(hit.frameStart, hit.frameEnd));
					}

				}
			*/
			/*
			hitCollectionLength = move.hitCollection.Length;
		}
		*/

			/*
			if (indentLevel == 1)
				indentLevel++;
			int indentSpacing = 25 * indentLevel;

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			Rect tempRect = GUILayoutUtility.GetRect(1, 20);
			Rect rect = new Rect(indentSpacing, tempRect.y, position.width - indentSpacing - 60, 20);

			// Border
			GUI.Box(rect, "", borderBarStyle);

			if (fillBounds && locations.Length > 0 && locations[0].x > 0)
			{
				float firstLeftPos = ((rect.width / maxValue) * locations[0].x);
				//firstLeftPos -= (rect.width/maxValue);
				GUI.Box(new Rect(rect.x, rect.y, firstLeftPos, rect.height), new GUIContent(), "flow node 4 on");
			}

			// Overlay
			float fillLeftPos = 0;
			float fillRightPos = 0;
			float lastLocation = 0;
			foreach (Vector3 i in locations)
			{
				lastLocation = i.y;
				fillLeftPos = ((rect.width / maxValue) * i.x) + rect.x;
				fillRightPos = ((rect.width / maxValue) * i.y) + rect.x;

				float fillWidth = fillRightPos - fillLeftPos;
				//fillWidth += (rect.width/maxValue);
				//fillLeftPos -= (rect.width/maxValue);

				if (i.z > 0)
				{
					GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle5);
				}
				else
				{
					GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle2);
				}
			}

			if (fillBounds && locations.Length > 0 && lastLocation < maxValue)
			{
				float fillWidth = rect.width - fillRightPos + rect.x;
				GUI.Box(new Rect(fillRightPos, rect.y, fillWidth, rect.height), new GUIContent(), fillBarStyle4);
			}

			// Text
			GUI.Label(rect, new GUIContent(label), labelStyle);

			if (fillBounds && locations.Length > 0)
			{
				EditorGUILayout.Space();
				GUILayout.BeginHorizontal(subArrayElementStyle);
				{
					labelStyle.normal.textColor = Color.yellow;
					moveInfo.startUpFrames = locations[0].x <= 0 ? 0 : moveInfo.hits[0].activeFramesBegin - 1;
					GUILayout.Label("Start Up: " + moveInfo.startUpFrames, labelStyle);
					labelStyle.normal.textColor = Color.cyan;
					move.activeFrames = (moveInfo.hits[moveInfo.hits.Length - 1].activeFramesEnds - moveInfo.hits[0].activeFramesBegin);
					GUILayout.Label("Active: " + moveInfo.activeFrames, labelStyle);
					labelStyle.normal.textColor = Color.red;
					moveInfo.recoveryFrames = lastLocation >= maxValue ? 0 : (moveInfo.totalFrames - moveInfo.hits[moveInfo.hits.Length - 1].activeFramesEnds + 1);
					GUILayout.Label("Recovery: " + moveInfo.recoveryFrames, labelStyle);
				}
				GUILayout.EndHorizontal();
			}
			labelStyle.normal.textColor = Color.white;

			//GUI.skin.label.normal.textColor = new Color(.706f, .706f, .706f, 1);
			tempRect = GUILayoutUtility.GetRect(1, 20);
			*/
		}


		/*
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
		{
			EditorGUILayout.BeginVertical("rootGroupStyle");
			{
				EditorGUILayout.BeginHorizontal();
				{
					//generalOptions = EditorGUILayout.Foldout(generalOptions, "General", foldStyle);
					generalOptions = EditorGUILayout.Foldout(generalOptions, "General");
					//helpButton("move:general");
				}
				EditorGUILayout.EndHorizontal();

				if (generalOptions)
				{
					EditorGUILayout.BeginVertical("subGroupStyle");
					{
						EditorGUILayout.Space();
						EditorGUI.indentLevel += 1;

						EditorGUIUtility.labelWidth = 180;

						move.moveName = EditorGUILayout.TextField("Move Name (Unique Id):", move.moveName);
						move.description = EditorGUILayout.TextField("Move Description:", move.description);

						EditorGUILayout.BeginHorizontal();
						{

							string unsaved = fpsTemp != move.fps ? "*" : "";
							fpsTemp = EditorGUILayout.IntSlider("FPS Architecture:" + unsaved, fpsTemp, 10, 120);
							EditorGUI.BeginDisabledGroup(fpsTemp == move.fps);
							{
								if (GUILayout.Button("Apply"))
								{
									move.fps = fpsTemp;
								}
								//if (StyledButton("Apply"))
								//moveInfo.fps = fpsTemp;
							}
							EditorGUI.EndDisabledGroup();

						}
						EditorGUILayout.EndHorizontal();


						move.animMap.clip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip:", move.animMap.clip, typeof(UnityEngine.AnimationClip), true);
						if (move.animMap.clip != null)
						{
							//scrubTime = EditorGUILayout.Slider(scrubTime, 0f, clip.length);
							EditorGUILayout.LabelField("Frame Advance", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
							ScrubFrames = EditorGUILayout.IntSlider((int)ScrubFrames, 0, (int)(move.animMap.clip.length * move.fps));
							print("a");
							EditorGUILayout.BeginHorizontal();

							if (GUILayout.Button("-"))
								ScrubFrames--;
							else if (GUILayout.Button("+"))
								ScrubFrames++;

							EditorGUILayout.EndHorizontal();

							UpdateAnimation();

						}
					}
					EditorGUILayout.EndVertical();
				}

			}
			EditorGUILayout.EndVertical();
			// End General Options
		}
		EditorGUILayout.EndScrollView();
		*/

	}

	public T[] AddElement<T>(T[] elements, T element)
	{
		List<T> l;

		if (elements == null)
			l = new List<T>();
		else
			l = new List<T>(elements);

		l.Add(element);
		return l.ToArray();
	}


	public T[] RemoveElement<T>(T[] elements, T element)
	{
		List<T> l = new List<T>(elements);
		l.Remove(element);
		return l.ToArray();
	}

	public void StyledMinMaxSlider(ref int minValue, ref int maxValue, int minLimit, int maxLimit, int indentLevel)
	{
		int indentSpacing = 25 * indentLevel;
		//indentSpacing += 30;
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		minValue = Mathf.Max(minValue, minLimit);
		maxValue = Mathf.Max(maxValue, minLimit);

		//minValue = Mathf.Min(minValue, maxValue);
		//maxValue = Mathf.Min(maxValue, maxLimit);

		/*
		minValue = Mathf.Max(minValue, minLimit);
		minValue = Mathf.Min(minValue, maxValue);
		maxValue = Mathf.Max(maxValue, minLimit);
		maxValue = Mathf.Min(maxValue, maxLimit);
		*/

		/*if (minValue < minLimit) minValue = minLimit;
		if (maxValue < 1) maxValue = 1;
		if (maxValue > maxLimit) maxValue = maxLimit;
		if (minValue == maxValue) minValue --;*/

		float minValueFloat = (float)minValue;
		float maxValueFloat = (float)maxValue;
		float minLimitFloat = (float)minLimit;
		float maxLimitFloat = (float)maxLimit;

		Rect tempRect = GUILayoutUtility.GetRect(1, 10);

		Rect rect = new Rect(indentSpacing, tempRect.y, position.width - indentSpacing - 100, 20);
		//Rect rect = new Rect(indentSpacing + 15,tempRect.y, position.width - indentSpacing - 70, 20);
		float fillLeftPos = ((rect.width / maxLimitFloat) * minValueFloat) + rect.x;
		float fillRightPos = ((rect.width / maxLimitFloat) * maxValueFloat) + rect.x;
		float fillWidth = fillRightPos - fillLeftPos;

		//fillWidth += (rect.width/maxLimitFloat);
		//fillLeftPos -= (rect.width/maxLimitFloat);

		// Border
		GUI.Box(rect, "", "ProgressBarBack");

		// Overlay
		GUI.Box(new Rect(fillLeftPos, rect.y, fillWidth, rect.height), new GUIContent(), "ProgressBarBar");

		// Text
		//GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
		//centeredStyle.alignment = TextAnchor.UpperCenter;

		GUIStyle labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.UpperCenter;
		GUI.Label(rect, "Active: " + Mathf.Floor(minValueFloat) + " - " + Mathf.Floor(maxValueFloat), labelStyle);
		labelStyle.alignment = TextAnchor.MiddleCenter;

		// Slider
		rect.y += 10;
		rect.x = indentLevel * 10;
		rect.width = (position.width - (indentLevel * 10) - 100);

		EditorGUI.MinMaxSlider(rect, ref minValueFloat, ref maxValueFloat, minLimitFloat, maxLimitFloat);
		minValue = (int)Mathf.Floor(minValueFloat);
		maxValue = (int)Mathf.Floor(maxValueFloat);

		tempRect = GUILayoutUtility.GetRect(1, 20);
	}

	public void StyledSlider(ref int value, int minLimit, int maxLimit, int indentLevel)
	{
		int indentSpacing = 25 * indentLevel;
		//indentSpacing += 30;
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		value = Mathf.Max((int)value, (int)minLimit);

		//minValue = Mathf.Min(minValue, maxValue);
		//maxValue = Mathf.Min(maxValue, maxLimit);

		/*
		minValue = Mathf.Max(minValue, minLimit);
		minValue = Mathf.Min(minValue, maxValue);
		maxValue = Mathf.Max(maxValue, minLimit);
		maxValue = Mathf.Min(maxValue, maxLimit);
		*/

		/*if (minValue < minLimit) minValue = minLimit;
		if (maxValue < 1) maxValue = 1;
		if (maxValue > maxLimit) maxValue = maxLimit;
		if (minValue == maxValue) minValue --;*/

		Rect tempRect = GUILayoutUtility.GetRect(1, 10);

		Rect rect = new Rect(indentSpacing, tempRect.y, position.width - indentSpacing - 100, 20);
		//Rect rect = new Rect(indentSpacing + 15,tempRect.y, position.width - indentSpacing - 70, 20);
		float unit = rect.width / maxLimit;


		//fillWidth += (rect.width/maxLimitFloat);
		//fillLeftPos -= (rect.width/maxLimitFloat);

		// Border
		GUI.Box(rect, "", "ProgressBarBack");

		// Overlay
		GUI.Box(new Rect(rect.x, rect.y, unit * value + rect.x, rect.height), new GUIContent(), "ProgressBarBar");

		// Text
		//GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
		//centeredStyle.alignment = TextAnchor.UpperCenter;

		GUIStyle labelStyle = new GUIStyle();
		labelStyle.alignment = TextAnchor.UpperCenter;
		GUI.Label(rect, "Active: " + Mathf.Floor(value), labelStyle);
		labelStyle.alignment = TextAnchor.MiddleCenter;

		// Slider
		rect.y += 10;
		rect.x = indentLevel * 10;
		rect.width = (position.width - (indentLevel * 10) - 100);

		value = EditorGUI.IntSlider(rect, value, minLimit, maxLimit);

		tempRect = GUILayoutUtility.GetRect(1, 20);
	}

	float timer;
	void Update()
	{
		if (!isPlaying)
			return;
		if (!AnimationMode.InAnimationMode())
		{
			AnimationMode.StartAnimationMode();
			timer = 0;
		}
		AnimationMode.BeginSampling();
		AnimationMode.SampleAnimationClip(gameObject, move.clip, timer);
		AnimationMode.EndSampling();

		Drawer.frame = (int)(timer * FPS);

		//Move.DrawHitbox(move.hitCollection[0], gameObject.transform);

		/*
		if (drawHitboxes && move.hitCollection.Length > 0)
		{
			foreach (Hit hit in move.hitCollection.Where(x => x.frameStart <= timer * FPS && x.frameEnd >= timer * FPS))
			{
				Move.DrawHitbox(hit, gameObject.transform);
			}
			SceneView.RepaintAll();
		}
		*/
		//print((timer * FPS).ToString());
		if (timer * FPS > TotalFrames)
        {
			timer = 0;
			if (!loopAnimation)
				isPlaying = false;
		}
		timer += Time.deltaTime;
	}

	public void OnInspectorUpdate()
	{
		this.Repaint();
	}

	static void print(string value)
	{
		Debug.Log(value);
	}

	public static void CreateAsset<T>(T data = null, T oldFile = null) where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T>();
		Object referencePath = Selection.activeObject;
		if (data != null)
		{
			asset = data;
			if (oldFile != null)
				referencePath = oldFile;
		}

		string path = AssetDatabase.GetAssetPath(referencePath);
		if (path == "")
		{
			path = "Assets";
		}
		else if (Path.GetExtension(path) != "")
		{
			path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(referencePath)), "");
		}

		string fileName;
		if (oldFile != null)
		{
			fileName = oldFile.name;
		}
		else if (asset is Move)
		{
			fileName = "New Move";
		}
		else
		{
			fileName = typeof(T).ToString();
		}
		string assetPathAndName = oldFile != null ? path + fileName + ".asset" : AssetDatabase.GenerateUniqueAssetPath(path + "/" + "FightingGame/" + fileName + ".asset");

		if (!AssetDatabase.Contains(asset))
			AssetDatabase.CreateAsset(asset, assetPathAndName);

		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
		Selection.activeObject = asset;

		if (asset is Move)
		{
			///MoveEditorWindow.Initialize();
		}
	}
}

/*
string myString = "Hello World";
bool groupEnabled;
bool myBool = true;
float myFloat = 1.23f;

// Add menu item named "My Window" to the Window menu
[MenuItem("Window/Move Editor")]
public static void ShowWindow()
{
	//Show existing window instance. If one doesn't exist, make one.
	EditorWindow.GetWindow(typeof(MoveEditorWindow));
}

void OnGUI()
{
	GUILayout.Label("Base Settings", EditorStyles.boldLabel);
	myString = EditorGUILayout.TextField("Text Field", myString);

	groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
	myBool = EditorGUILayout.Toggle("Toggle", myBool);
	myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
	EditorGUILayout.EndToggleGroup();
}
*/
/*
private GameObject gameObject;
static MoveEditorWindow moveEditorWindow;
Move move;
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
private float ScrubTime { get { return (float)ScrubFrames / (float)move.fps; } }

private int CurrentFrame { get { return _scrubFrames; } }
private int TotalFrames { get { return (int)(move.animMap.clip.length * move.fps); } }

private float e;

[MenuItem("Window/Move Editor")]
[CanEditMultipleObjects]
public static void Initialize()
{
	//moveEditorWindow = EditorWindow.GetWindow<MoveEditorWindow>(false, "Move Editor", true);

	EditorWindow.GetWindow<MoveEditorWindow>(false, "Move Editor", true);


	//moveEditorWindow.Show();
	//EditorWindow.FocusWindowIfItsOpen<SceneView>();

	Camera sceneCam = GameObject.FindObjectOfType<Camera>();
	if (sceneCam != null)
	{
		moveEditorWindow.initialFieldOfView = sceneCam.fieldOfView;
		moveEditorWindow.initialCamPosition = sceneCam.transform.position;
		moveEditorWindow.initialCamRotation = sceneCam.transform.rotation;
	}
	moveEditorWindow.Populate();

	//moveEditorWindow.Populate();
}
void OnSelectionChange()
{
	Populate();
}

void OnEnable()
{
	Populate();
}

void OnFocus()
{
	Populate();
}

Vector2 scrollPos = Vector2.zero;
bool generalOptions;
int fpsTemp;
public void OnGUI()
{
	GUILayout.Label("Base Settings", EditorStyles.boldLabel);
	/*
	myString = EditorGUILayout.TextField("Text Field", myString);

	groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
	myBool = EditorGUILayout.Toggle("Toggle", myBool);
	*/
//e = EditorGUILayout.Slider("Slider", e, -3, 3);
//EditorGUILayout.EndToggleGroup();

/*
if (move == null)
{
	GUILayout.BeginHorizontal("GroupBox");
	GUILayout.Label("Select a move file first\nor create a new move.");
	GUILayout.EndHorizontal();
	EditorGUILayout.Space();
	if (GUILayout.Button("Create new move"))
		CreateAsset<Move>();
	return;
}
*/
/*
EditorGUILayout.BeginVertical("titleStyle");
{
	EditorGUILayout.BeginHorizontal();
	{
		EditorGUILayout.LabelField("", (move.moveName == "" ? "New Move" : move.moveName), GUILayout.Height(32));
	}
	EditorGUILayout.EndHorizontal();
}
EditorGUILayout.EndVertical();

if (move.animMap.clip != null)
{
	generalOptions = EditorGUILayout.Foldout(generalOptions, "General");
	//totalFramesOriginal = (int)Mathf.Abs(Mathf.Ceil((move.fps * move.animMap.clip.length) / (float)move._animationSpeed));
	//totalFramesOriginal = (int)Mathf.Abs(Mathf.Ceil((move.fps * move.animMap.clip.length) / (float)move._animationSpeed));
	//if (move.totalFrames == 0)
	//{
	//	move.totalFrames = totalFramesOriginal;
	//}
	//if (moveInfo.totalFrames > totalFramesGlobal)
	//	totalFramesGlobal = moveInfo.totalFrames;
}

EditorGUILayout.LabelField("Frame Advance", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
*/
/*
scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
{
	EditorGUILayout.BeginVertical("rootGroupStyle");
	{
		EditorGUILayout.BeginHorizontal();
		{
			//generalOptions = EditorGUILayout.Foldout(generalOptions, "General", foldStyle);
			generalOptions = EditorGUILayout.Foldout(generalOptions, "General");
			//helpButton("move:general");
		}
		EditorGUILayout.EndHorizontal();

		if (generalOptions)
		{
			EditorGUILayout.BeginVertical("subGroupStyle");
			{
				EditorGUILayout.Space();
				EditorGUI.indentLevel += 1;

				EditorGUIUtility.labelWidth = 180;

				move.moveName = EditorGUILayout.TextField("Move Name (Unique Id):", move.moveName);
				move.description = EditorGUILayout.TextField("Move Description:", move.description);

				EditorGUILayout.BeginHorizontal();
				{

					string unsaved = fpsTemp != move.fps ? "*" : "";
					fpsTemp = EditorGUILayout.IntSlider("FPS Architecture:" + unsaved, fpsTemp, 10, 120);
					EditorGUI.BeginDisabledGroup(fpsTemp == move.fps);
					{
						if (GUILayout.Button("Apply"))
						{
							move.fps = fpsTemp;
						}
						//if (StyledButton("Apply"))
						//moveInfo.fps = fpsTemp;
					}
					EditorGUI.EndDisabledGroup();

				}
				EditorGUILayout.EndHorizontal();


				move.animMap.clip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip:", move.animMap.clip, typeof(UnityEngine.AnimationClip), true);
				if (move.animMap.clip != null)
				{
					//scrubTime = EditorGUILayout.Slider(scrubTime, 0f, clip.length);
					EditorGUILayout.LabelField("Frame Advance", new GUIStyle() { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
					ScrubFrames = EditorGUILayout.IntSlider((int)ScrubFrames, 0, (int)(move.animMap.clip.length * move.fps));
					print("a");
					EditorGUILayout.BeginHorizontal();

					if (GUILayout.Button("-"))
						ScrubFrames--;
					else if (GUILayout.Button("+"))
						ScrubFrames++;

					EditorGUILayout.EndHorizontal();

					UpdateAnimation();

				}
			}
			EditorGUILayout.EndVertical();
		}

	}
	EditorGUILayout.EndVertical();
	// End General Options
}
EditorGUILayout.EndScrollView();
*/

/*

// Begin General Options
scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
{
	EditorGUILayout.BeginVertical("rootGroupStyle");
	{
		EditorGUILayout.BeginHorizontal();
		{
			//generalOptions = EditorGUILayout.Foldout(generalOptions, "General", foldStyle);
			generalOptions = EditorGUILayout.Foldout(generalOptions, "General");
			//helpButton("move:general");
		}
		EditorGUILayout.EndHorizontal();

		if (generalOptions)
		{
			EditorGUILayout.BeginVertical("subGroupStyle");
			{
				EditorGUILayout.Space();
				EditorGUI.indentLevel += 1;

				EditorGUIUtility.labelWidth = 180;

				move.moveName = EditorGUILayout.TextField("Move Name (Unique Id):", move.moveName);
				move.description = EditorGUILayout.TextField("Move Description:", move.description);

				EditorGUILayout.BeginHorizontal();
				{

					string unsaved = fpsTemp != move.fps ? "*" : "";
					fpsTemp = EditorGUILayout.IntSlider("FPS Architecture:" + unsaved, fpsTemp, 10, 120);
					EditorGUI.BeginDisabledGroup(fpsTemp == move.fps);
					{
						if (GUILayout.Button("Apply"))
						{
							move.fps = fpsTemp;
						}
						//if (StyledButton("Apply"))
							//moveInfo.fps = fpsTemp;
					}
					EditorGUI.EndDisabledGroup();

				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndVertical();
		}

	}
	EditorGUILayout.EndVertical();
	// End General Options
}
EditorGUILayout.EndScrollView();
}
*/
//}



/*
private void UpdateAnimation()
{
	if (move != null)
	{
		if (!AnimationMode.InAnimationMode())
		{
			AnimationMode.StartAnimationMode();
		}
		AnimationMode.BeginSampling();
		AnimationMode.SampleAnimationClip(gameObject, move.animMap.clip, ScrubTime);
		AnimationMode.EndSampling();
	}
}



void Populate()
{

	UnityEngine.Object[] selection = Selection.GetFiltered(typeof(Move), SelectionMode.Assets);
	if (selection.Length > 0)
	{
		if (selection[0] == null)
			return;
		move = (Move)selection[0];
		print(move.name);
	}
}
*/

/*
float totalFramesOriginal;
Vector2 scrollPos;
bool generalOptions;
bool animationOptions;
public void OnGUI()
{
	if (move == null)
	{
		GUILayout.BeginHorizontal("GroupBox");
		GUILayout.Label("Select a move file first\nor create a new move.");
		GUILayout.EndHorizontal();
		EditorGUILayout.Space();
		if (GUILayout.Button("Create new move"))
			CreateAsset<Move>();
		return;
	}

	//EditorGUIUtility.labelWidth = 150;
	GUIStyle fontStyle = new GUIStyle();
	//fontStyle.font = (Font)EditorGUIUtility.Load("EditorFont.TTF");
	fontStyle.font = (Font)Resources.Load("EditorFont");
	fontStyle.fontSize = 30;
	fontStyle.alignment = TextAnchor.UpperCenter;
	fontStyle.normal.textColor = Color.white;
	fontStyle.hover.textColor = Color.white;
	EditorGUILayout.BeginVertical("titleStyle");
	{
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.LabelField("", (move.moveName == "" ? "New Move" : move.moveName), fontStyle, GUILayout.Height(32));
			//helpButton("move:start");
		}
		EditorGUILayout.EndHorizontal();
	}
	EditorGUILayout.EndVertical();

	if (move.animMap.clip != null)
	{
		//totalFramesOriginal = (int)Mathf.Abs(Mathf.Ceil((move.fps * move.animMap.clip.length) / (float)move._animationSpeed));
		totalFramesOriginal = (int)Mathf.Abs(Mathf.Ceil((move.fps * move.animMap.clip.length) / (float)move._animationSpeed));
		/*
		if (move.totalFrames == 0)
		{
			move.totalFrames = totalFramesOriginal;
		}
		if (moveInfo.totalFrames > totalFramesGlobal)
			totalFramesGlobal = moveInfo.totalFrames;
	}


	// Begin General Options
	scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
	{
		EditorGUILayout.BeginVertical("rootGroupStyle");
		{
			EditorGUILayout.BeginHorizontal();
			{
				//generalOptions = EditorGUILayout.Foldout(generalOptions, "General", foldStyle);
				generalOptions = EditorGUILayout.Foldout(generalOptions, "General");
				//helpButton("move:general");
			}
			EditorGUILayout.EndHorizontal();

			if (generalOptions)
			{
				EditorGUILayout.BeginVertical("subGroupStyle");
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel += 1;

					EditorGUIUtility.labelWidth = 180;

					move.moveName = EditorGUILayout.TextField("Move Name (Unique Id):", move.moveName);
					move.description = EditorGUILayout.TextField("Move Description:", move.description);

					EditorGUILayout.BeginHorizontal();
					{
						/*
						string unsaved = fpsTemp != move.fps ? "*" : "";
						fpsTemp = EditorGUILayout.IntSlider("FPS Architecture:" + unsaved, fpsTemp, 10, 120);
						EditorGUI.BeginDisabledGroup(fpsTemp == moveInfo.fps);
						{
							if (StyledButton("Apply"))
								moveInfo.fps = fpsTemp;
						}
						EditorGUI.EndDisabledGroup();

					}
					EditorGUILayout.EndHorizontal();


					//SubGroupTitle("Behaviour");
					EditorGUIUtility.labelWidth = 230;
					/*
					moveInfo.ignoreGravity = EditorGUILayout.Toggle("Ignore Gravity", moveInfo.ignoreGravity, toggleStyle);
					moveInfo.ignoreFriction = EditorGUILayout.Toggle("Ignore Friction", moveInfo.ignoreFriction, toggleStyle);
					moveInfo.cancelMoveWheLanding = EditorGUILayout.Toggle("Cancel Move On Landing", moveInfo.cancelMoveWheLanding, toggleStyle);
					moveInfo.autoCorrectRotation = EditorGUILayout.Toggle("Auto Correct Rotation", moveInfo.autoCorrectRotation, toggleStyle);
					if (moveInfo.autoCorrectRotation)
					{
						moveInfo.frameWindowRotation = EditorGUILayout.IntField("Frame Window:", Mathf.Clamp(moveInfo.frameWindowRotation, 0, moveInfo.totalFrames));
					}

					EditorGUIUtility.labelWidth = 150;
					EditorGUILayout.Space();

					EditorGUI.indentLevel -= 1;

				}
				EditorGUILayout.EndVertical();
			}

		}
		EditorGUILayout.EndVertical();
		// End General Options


		// Begin Animation Options
		EditorGUILayout.BeginVertical("rootGroupStyle");
		{
			EditorGUILayout.BeginHorizontal();
			{
				animationOptions = EditorGUILayout.Foldout(animationOptions, "Animation");
				//helpButton("move:animation");
			}
			EditorGUILayout.EndHorizontal();

			if (animationOptions)
			{
				EditorGUILayout.BeginVertical("subGroupStyle");
				{
					EditorGUILayout.Space();
					EditorGUI.indentLevel += 1;
					EditorGUIUtility.labelWidth = 200;

					//SubGroupTitle("File");
					move.animMap.clip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip:", move.animMap.clip, typeof(UnityEngine.AnimationClip), true);
					if (move.animMap.clip != null)
					{

						move.animMap.length = move.animMap.clip.length;
						//move.wrapMode = (WrapMode)EditorGUILayout.EnumPopup("Wrap Mode:", move.wrapMode, enumStyle);
						move.wrapMode = (WrapMode)EditorGUILayout.EnumPopup("Wrap Mode:", move.wrapMode);

						/*if (moveInfo.applyRootMotion){
							moveInfo.rootMotionNode = (BodyPart)EditorGUILayout.EnumPopup("Root Motion Node:", moveInfo.rootMotionNode, enumStyle);
							moveInfo.forceGrounded = EditorGUILayout.Toggle("Force Grounded", moveInfo.forceGrounded, toggleStyle);
						}

						//SubGroupTitle("Speed");
						//move.fixedSpeed = EditorGUILayout.Toggle("Fixed Speed", moveInfo.fixedSpeed, toggleStyle);
						move._animationSpeed = EditorGUILayout.FloatField("Base Speed:", (float)move._animationSpeed);

						float animTime = 0;

						if (move.fixedSpeed)
						{

							//totalFramesTemp = totalFramesOriginal;
							animTime = move.animMap.clip.length * (float)move._animationSpeed;

						}
						else
						{
							move.speedKeyFrameToggle = EditorGUILayout.Foldout(move.speedKeyFrameToggle, "Speed Key Frames", EditorStyles.foldout);
							if (moveInfo.speedKeyFrameToggle)
							{
								EditorGUILayout.BeginVertical(subGroupStyle);
								{
									EditorGUI.indentLevel += 1;

									List<int> castingValues = new List<int>();
									foreach (AnimSpeedKeyFrame animationKeyFrame in move.animSpeedKeyFrame)
										castingValues.Add(animationKeyFrame.castingFrame);
									StyledMarker("Casting Timeline", castingValues.ToArray(), totalFramesOriginal, EditorGUI.indentLevel);

									totalFramesTemp = 0;
									//int nextCastingFrame = 0;
									int previousCastingFrame = 0;
									float displacement = 0;

									if (moveInfo.animSpeedKeyFrame.Length > 0)
									{
										//totalFramesTemp = (int)Mathf.Floor(moveInfo.animSpeedKeyFrame[0].castingFrame / moveInfo.animationSpeed);
									}

									for (int i = 0; i < moveInfo.animSpeedKeyFrame.Length; i++)
									{
										EditorGUILayout.Space();
										EditorGUILayout.BeginVertical(arrayElementStyle);
										{
											EditorGUILayout.Space();
											EditorGUILayout.BeginHorizontal();
											{
												moveInfo.animSpeedKeyFrame[i].castingFrame = EditorGUILayout.IntSlider("Casting Frame:", moveInfo.animSpeedKeyFrame[i].castingFrame, 0, totalFramesOriginal);
												if (GUILayout.Button("", "PaneOptions"))
												{
													PaneOptions<AnimSpeedKeyFrame>(moveInfo.animSpeedKeyFrame, moveInfo.animSpeedKeyFrame[i], delegate (AnimSpeedKeyFrame[] newElement) { moveInfo.animSpeedKeyFrame = newElement; });
												}
											}
											EditorGUILayout.EndHorizontal();
											moveInfo.animSpeedKeyFrame[i]._speed = EditorGUILayout.FloatField("New Speed:", (float)FPMath.Max(moveInfo.animSpeedKeyFrame[i]._speed, 0.2));
											moveInfo.animSpeedKeyFrame[i].castingFrame = Mathf.Max(moveInfo.animSpeedKeyFrame[i].castingFrame, previousCastingFrame);
											moveInfo.animSpeedKeyFrame[i].castingFrame = Mathf.Min(moveInfo.animSpeedKeyFrame[i].castingFrame, totalFramesOriginal);

											if (i == moveInfo.animSpeedKeyFrame.Length - 1)
											{
												displacement = totalFramesOriginal;
											}
											else
											{
												displacement = moveInfo.animSpeedKeyFrame[i + 1].castingFrame;
											}
											// Update Display Value
											displacement -= moveInfo.animSpeedKeyFrame[i].castingFrame;
											displacement /= (float)moveInfo.animSpeedKeyFrame[i]._speed;
											EditorGUILayout.LabelField("Frame Window (Aprox.):", displacement.ToString());


											previousCastingFrame = moveInfo.animSpeedKeyFrame[i].castingFrame;

											//totalFramesTemp += Mathf.RoundToInt(displacement / moveInfo.animSpeedKeyFrame[i].speed);
											//nextCastingFrame = (i == moveInfo.animSpeedKeyFrame.Length - 1) ? totalFramesUnsaved : moveInfo.animSpeedKeyFrame[i + 1].castingFrame;
											//displacement = Mathf.Max(0, nextCastingFrame - moveInfo.animSpeedKeyFrame[i].castingFrame);

											EditorGUILayout.Space();

										}
										EditorGUILayout.EndVertical();
										EditorGUILayout.Space();
									}

									animTime = 0;
									int frameCounter = 0;
									do
									{
										frameCounter++;
										float frameSpeed = (float)moveInfo._animationSpeed;
										foreach (AnimSpeedKeyFrame speedKeyFrame in moveInfo.animSpeedKeyFrame)
										{
											if (frameCounter > speedKeyFrame.castingFrame)
											{
												frameSpeed = (float)moveInfo._animationSpeed * (float)speedKeyFrame._speed;
												break;
											}
										}
										animTime += ((float)1 / moveInfo.fps) * frameSpeed;

									} while (animTime < moveInfo.animMap.clip.length);
									totalFramesTemp = frameCounter;

									if (totalFramesTemp == 0)
										totalFramesTemp = moveInfo.totalFrames;
									//totalFramesUnsaved = totalFramesTemp;

									if (StyledButton("New Keyframe"))
										moveInfo.animSpeedKeyFrame = AddElement<AnimSpeedKeyFrame>(moveInfo.animSpeedKeyFrame, new AnimSpeedKeyFrame());

									EditorGUILayout.Space();
									EditorGUI.indentLevel -= 1;

								}
								EditorGUILayout.EndVertical();
							}
						}


						//animationSpeedTemp = StyledSlider("Animation Speed" + unsaved, animationSpeedTemp, EditorGUI.indentLevel, -5, 5);
						string unsaved = totalFramesTemp != moveInfo.totalFrames ? "*" : "";
						EditorGUILayout.LabelField("Original Frames:", totalFramesOriginal.ToString());
						EditorGUILayout.LabelField("Time (seconds):", animTime.ToString() + unsaved);
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField("Total frames:", totalFramesTemp.ToString() + unsaved);
							if (StyledButton("Apply"))
							{
								moveInfo.totalFrames = totalFramesTemp;
							}
						}
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.Space();


						SubGroupTitle("Blending");
						moveInfo.overrideBlendingIn = EditorGUILayout.Toggle("Override Blending (In)", moveInfo.overrideBlendingIn, toggleStyle);
						if (moveInfo.overrideBlendingIn)
						{
							moveInfo._blendingIn = EditorGUILayout.FloatField("Blend In Duration:", (float)moveInfo._blendingIn);
						}

						moveInfo.overrideBlendingOut = EditorGUILayout.Toggle("Override Blending (Out)", moveInfo.overrideBlendingOut, toggleStyle);
						if (moveInfo.overrideBlendingOut)
						{
							moveInfo._blendingOut = EditorGUILayout.FloatField("Blend Out Duration:", (float)moveInfo._blendingOut);
						}
						EditorGUILayout.Space();


						SubGroupTitle("Orientation");
						EditorGUIUtility.labelWidth = 230;
						moveInfo.forceMirrorLeft = EditorGUILayout.Toggle("Mirror Animation (Left)", moveInfo.forceMirrorLeft, toggleStyle);
						moveInfo.invertRotationLeft = EditorGUILayout.Toggle("Rotate Character (Left)", moveInfo.invertRotationLeft, toggleStyle);

						EditorGUILayout.Space();
						moveInfo.forceMirrorRight = EditorGUILayout.Toggle("Mirror Animation (Right)", moveInfo.forceMirrorRight, toggleStyle);
						moveInfo.invertRotationRight = EditorGUILayout.Toggle("Rotate Character (Right)", moveInfo.invertRotationRight, toggleStyle);
						EditorGUILayout.Space();


						SubGroupTitle("Preview");
						EditorGUIUtility.labelWidth = 180;
						GameObject newCharacterPrefab = (GameObject)EditorGUILayout.ObjectField("Character Prefab:", moveInfo.characterPrefab, typeof(UnityEngine.GameObject), true);
						if (newCharacterPrefab != null && moveInfo.characterPrefab != newCharacterPrefab && !EditorApplication.isPlayingOrWillChangePlaymode)
						{
							if (PrefabUtility.GetPrefabType(newCharacterPrefab) != PrefabType.Prefab)
							{
								characterWarning = true;
								errorMsg = "This character is not a prefab.";
							}
							else if (newCharacterPrefab.GetComponent<HitBoxesScript>() == null)
							{
								characterWarning = true;
								errorMsg = "This character doesn't have hitboxes!\n Please add the HitboxScript and try again.";
							}
							else
							{
								characterWarning = false;
								moveInfo.characterPrefab = newCharacterPrefab;
							}
						}
						else if (moveInfo.characterPrefab != newCharacterPrefab && EditorApplication.isPlayingOrWillChangePlaymode)
						{
							characterWarning = true;
							errorMsg = "You can't change this field while in play mode.";
						}
						else if (newCharacterPrefab == null)
							moveInfo.characterPrefab = null;

						if (characterPrefab == null)
						{
							if (StyledButton("Animation Preview"))
							{
								if (moveInfo.characterPrefab == null)
								{
									characterWarning = true;
									errorMsg = "Drag a character into 'Character Prefab' first.";
								}
								else if (EditorApplication.isPlayingOrWillChangePlaymode)
								{
									characterWarning = true;
									errorMsg = "You can't preview animations while in play mode.";
								}
								else
								{
									characterWarning = false;
									EditorCamera.SetPosition(Vector3.up * 4);
									EditorCamera.SetRotation(Quaternion.identity);
									EditorCamera.SetOrthographic(true);
									EditorCamera.SetSize(10);

									characterPrefab = (GameObject)PrefabUtility.InstantiatePrefab(moveInfo.characterPrefab);
									characterPrefab.transform.position = new Vector3(0, 0, 0);
								}
							}

							if (characterWarning)
							{
								GUILayout.BeginHorizontal("GroupBox");
								GUILayout.FlexibleSpace();
								GUILayout.Label(errorMsg, "CN EntryWarn");
								GUILayout.FlexibleSpace();
								GUILayout.EndHorizontal();
							}
						}
						else
						{
							EditorGUI.indentLevel += 1;
							if (smoothPreview)
							{
								animFrame = StyledSlider("Animation Frames", animFrame, EditorGUI.indentLevel, 0, totalFramesGlobal);
							}
							else
							{
								animFrame = StyledSlider("Animation Frames", (int)animFrame, EditorGUI.indentLevel, 0, totalFramesGlobal);
							}
							EditorGUI.indentLevel -= 1;

							if (cameraOptions)
							{
								GUILayout.BeginHorizontal("GroupBox");
								GUILayout.Label("You must close 'Cinematic Options' first.", "CN EntryError");
								GUILayout.EndHorizontal();
							}

							smoothPreview = EditorGUILayout.Toggle("Smooth Preview", smoothPreview, toggleStyle);
							AnimationSampler(characterPrefab, moveInfo.animMap.clip, 0, true, true, moveInfo.forceMirrorLeft, moveInfo.invertRotationLeft);

							EditorGUILayout.LabelField("Current Speed:", currentSpeed.ToString());

							EditorGUILayout.Space();

							EditorGUILayout.BeginHorizontal();
							{
								if (StyledButton("Reset Scene View"))
								{
									EditorCamera.SetPosition(Vector3.up * 4);
									EditorCamera.SetRotation(Quaternion.identity);
									EditorCamera.SetOrthographic(true);
									EditorCamera.SetSize(10);
								}
								if (StyledButton("Close Preview"))
									Clear(true, true);
							}
							EditorGUILayout.EndHorizontal();

							EditorGUILayout.Space();
						}
					}
					EditorGUI.indentLevel -= 1;
					EditorGUIUtility.labelWidth = 150;

				}
				EditorGUILayout.EndVertical();
			}
			else if (characterPrefab != null && !cameraOptions)
			{
				Clear(true, true);
			}

		}
		EditorGUILayout.EndVertical();
		// End Animation Options

		bool activeFramesOptions;
		bool hitsToggle;

		// Begin Active Frame Options
		EditorGUILayout.BeginVertical("rootGroupStyle");
		{
			EditorGUILayout.BeginHorizontal();
			{
				activeFramesOptions = EditorGUILayout.Foldout(activeFramesOptions, "Active Frames", EditorStyles.foldout);
				//helpButton("move:activeframes");
			}
			EditorGUILayout.EndHorizontal();

			if (activeFramesOptions)
			{
				EditorGUILayout.BeginVertical("subGroupStyle");
				{
					EditorGUI.indentLevel += 1;

					// Hits Toggle
					hitsToggle = EditorGUILayout.Foldout(hitsToggle, "Hits (" + move.hits.Length + ")", EditorStyles.foldout);
					if (hitsToggle)
					{
						EditorGUILayout.BeginVertical(subGroupStyle);
						{
							EditorGUI.indentLevel += 1;
							List<Vector3> castingValues = new List<Vector3>();
							foreach (Hit hit in moveInfo.hits)
							{
								castingValues.Add(new Vector3(hit.activeFramesBegin, hit.activeFramesEnds, (hit.hitConfirmType == HitConfirmType.Throw ? 1 : 0)));
							}
							StyledMarker("Frame Data Timeline", castingValues.ToArray(), moveInfo.totalFrames,
										 EditorGUI.indentLevel, true);

							for (int i = 0; i < moveInfo.hits.Length; i++)
							{
								EditorGUILayout.Space();
								EditorGUILayout.BeginVertical(arrayElementStyle);
								{
									EditorGUILayout.Space();
									EditorGUILayout.BeginHorizontal();
									{
										StyledMinMaxSlider("Active Frames", ref moveInfo.hits[i].activeFramesBegin, ref moveInfo.hits[i].activeFramesEnds, 0, moveInfo.totalFrames, EditorGUI.indentLevel);
										if (GUILayout.Button("", "PaneOptions"))
										{
											PaneOptions<Hit>(moveInfo.hits, moveInfo.hits[i], delegate (Hit[] newElement) { moveInfo.hits = newElement; });
										}
									}
									EditorGUILayout.EndHorizontal();

									EditorGUIUtility.labelWidth = 180;

									EditorGUILayout.Space();

									moveInfo.hits[i].hitConfirmType = (HitConfirmType)EditorGUILayout.EnumPopup("Hit Confirm Type:", moveInfo.hits[i].hitConfirmType, enumStyle);

									// Hurt Boxes Toggle
									int amount = moveInfo.hits[i].hurtBoxes != null ? moveInfo.hits[i].hurtBoxes.Length : 0;
									moveInfo.hits[i].hurtBoxesToggle = EditorGUILayout.Foldout(moveInfo.hits[i].hurtBoxesToggle, "Hurt Boxes (" + amount + ")", EditorStyles.foldout);
									if (moveInfo.hits[i].hurtBoxesToggle)
									{
										EditorGUILayout.BeginVertical(subGroupStyle);
										{
											EditorGUI.indentLevel += 1;
											if (amount > 0)
											{
												for (int y = 0; y < moveInfo.hits[i].hurtBoxes.Length; y++)
												{
													EditorGUILayout.BeginVertical(subArrayElementStyle);
													{
														EditorGUILayout.BeginHorizontal();
														{
															moveInfo.hits[i].hurtBoxes[y].bodyPart = (BodyPart)EditorGUILayout.EnumPopup("Body Part:", moveInfo.hits[i].hurtBoxes[y].bodyPart, enumStyle);
															if (GUILayout.Button("", "PaneOptions"))
															{
																PaneOptions<HurtBox>(moveInfo.hits[i].hurtBoxes, moveInfo.hits[i].hurtBoxes[y], delegate (HurtBox[] newElement) { moveInfo.hits[i].hurtBoxes = newElement; });
															}
														}
														EditorGUILayout.EndHorizontal();

														moveInfo.hits[i].hurtBoxes[y].shape = (HitBoxShape)EditorGUILayout.EnumPopup("Shape:", moveInfo.hits[i].hurtBoxes[y].shape, enumStyle);
														if (moveInfo.hits[i].hurtBoxes[y].shape == HitBoxShape.circle)
														{
															moveInfo.hits[i].hurtBoxes[y]._radius = EditorGUILayout.FloatField("Radius:", (float)moveInfo.hits[i].hurtBoxes[y]._radius);
															moveInfo.hits[i].hurtBoxes[y]._offSet = FPVector.ToFPVector(EditorGUILayout.Vector2Field("Off Set:", moveInfo.hits[i].hurtBoxes[y]._offSet.ToVector2()));
														}
														else
														{
															moveInfo.hits[i].hurtBoxes[y].rect = EditorGUILayout.RectField("Rectangle:", moveInfo.hits[i].hurtBoxes[y].rect);
															moveInfo.hits[i].hurtBoxes[y]._rect = new FPRect(moveInfo.hits[i].hurtBoxes[y].rect);


															EditorGUIUtility.labelWidth = 200;
															bool tmpFollowXBounds = moveInfo.hits[i].hurtBoxes[y].followXBounds;
															bool tmpFollowYBounds = moveInfo.hits[i].hurtBoxes[y].followYBounds;

															moveInfo.hits[i].hurtBoxes[y].followXBounds = EditorGUILayout.Toggle("Follow Bounds (X)", moveInfo.hits[i].hurtBoxes[y].followXBounds);
															moveInfo.hits[i].hurtBoxes[y].followYBounds = EditorGUILayout.Toggle("Follow Bounds (Y)", moveInfo.hits[i].hurtBoxes[y].followYBounds);

															if (tmpFollowXBounds != moveInfo.hits[i].hurtBoxes[y].followXBounds)
																moveInfo.hits[i].hurtBoxes[y].rect.width = moveInfo.hits[i].hurtBoxes[y].followXBounds ? 0 : 4;
															if (tmpFollowYBounds != moveInfo.hits[i].hurtBoxes[y].followYBounds)
																moveInfo.hits[i].hurtBoxes[y].rect.height = moveInfo.hits[i].hurtBoxes[y].followYBounds ? 0 : 4;

															EditorGUIUtility.labelWidth = 150;
														}

														EditorGUILayout.Space();
													}
													EditorGUILayout.EndVertical();
												}
											}
											if (StyledButton("New Hurt Box"))
												moveInfo.hits[i].hurtBoxes = AddElement<HurtBox>(moveInfo.hits[i].hurtBoxes, new HurtBox());

											EditorGUI.indentLevel -= 1;
										}
										EditorGUILayout.EndVertical();
									}

									// Hit Conditions Toggle
									moveInfo.hits[i].hitConditionsToggle = EditorGUILayout.Foldout(moveInfo.hits[i].hitConditionsToggle, "Hit Conditions", EditorStyles.foldout);
									if (moveInfo.hits[i].hitConditionsToggle)
									{
										EditorGUILayout.BeginVertical(subGroupStyle);
										{
											EditorGUIUtility.labelWidth = 240;
											EditorGUI.indentLevel += 1;

											SubGroupTitle("Basic Filters");
											moveInfo.hits[i].groundHit = EditorGUILayout.Toggle("Standing", moveInfo.hits[i].groundHit, toggleStyle);
											moveInfo.hits[i].crouchingHit = EditorGUILayout.Toggle("Crouching", moveInfo.hits[i].crouchingHit, toggleStyle);
											moveInfo.hits[i].airHit = EditorGUILayout.Toggle("In the Air", moveInfo.hits[i].airHit, toggleStyle);
											moveInfo.hits[i].stunHit = EditorGUILayout.Toggle("Stunned", moveInfo.hits[i].stunHit, toggleStyle);
											moveInfo.hits[i].downHit = EditorGUILayout.Toggle("Down", moveInfo.hits[i].downHit, toggleStyle);

											PlayerConditionsGroup("Advanced Filters", moveInfo.hits[i].opponentConditions, false);

											EditorGUI.indentLevel -= 1;
											EditorGUIUtility.labelWidth = 150;

											EditorGUILayout.Space();
										}
										EditorGUILayout.EndVertical();
									}

									EditorGUILayout.Space();

									if (moveInfo.hits[i].hitConfirmType == HitConfirmType.Hit)
									{
										EditorGUIUtility.labelWidth = 180;
										moveInfo.hits[i].continuousHit = EditorGUILayout.Toggle("Continuous Hit", moveInfo.hits[i].continuousHit, toggleStyle);
										if (moveInfo.hits[i].continuousHit)
										{
											moveInfo.hits[i].spaceBetweenHits = (Sizes)EditorGUILayout.EnumPopup("Space Between Hits:", moveInfo.hits[i].spaceBetweenHits, enumStyle);
										}
										moveInfo.hits[i].armorBreaker = EditorGUILayout.Toggle("Armor Breaker", moveInfo.hits[i].armorBreaker, toggleStyle);
										moveInfo.hits[i].unblockable = EditorGUILayout.Toggle("Unblockable", moveInfo.hits[i].unblockable, toggleStyle);
										moveInfo.hits[i].hitType = (HitType)EditorGUILayout.EnumPopup("Hit Type:", moveInfo.hits[i].hitType, enumStyle);
										moveInfo.hits[i].hitStrength = (HitStrengh)EditorGUILayout.EnumPopup("Hit Strength:", moveInfo.hits[i].hitStrength, enumStyle);
										moveInfo.hits[i].resetHitAnimations = EditorGUILayout.Toggle("Reset Hit Animation", moveInfo.hits[i].resetHitAnimations, toggleStyle);
										moveInfo.hits[i].forceStand = EditorGUILayout.Toggle("Force Stand", moveInfo.hits[i].forceStand, toggleStyle);
										EditorGUIUtility.labelWidth = 150;

										EditorGUILayout.Space();

										// Damage Toggle
										moveInfo.hits[i].damageOptionsToggle = EditorGUILayout.Foldout(moveInfo.hits[i].damageOptionsToggle, "Damage Options", EditorStyles.foldout);
										if (moveInfo.hits[i].damageOptionsToggle)
										{
											EditorGUILayout.BeginVertical(subGroupStyle);
											{
												EditorGUIUtility.labelWidth = 180;
												EditorGUI.indentLevel += 1;
												moveInfo.hits[i].damageType = (DamageType)EditorGUILayout.EnumPopup("Damage Type:", moveInfo.hits[i].damageType, enumStyle);
												moveInfo.hits[i]._damageOnHit = EditorGUILayout.FloatField("Damage on Hit:", (float)moveInfo.hits[i]._damageOnHit);
												moveInfo.hits[i]._damageOnBlock = EditorGUILayout.FloatField("Damage on Block:", (float)moveInfo.hits[i]._damageOnBlock);
												moveInfo.hits[i].damageScaling = EditorGUILayout.Toggle("Damage Scaling", moveInfo.hits[i].damageScaling, toggleStyle);
												moveInfo.hits[i].doesntKill = EditorGUILayout.Toggle("Hit Doesn't Kill", moveInfo.hits[i].doesntKill, toggleStyle);
												EditorGUI.indentLevel -= 1;
												EditorGUIUtility.labelWidth = 150;

												EditorGUILayout.Space();
											}
											EditorGUILayout.EndVertical();
										}

										// Hit Stun Toggle
										moveInfo.hits[i].hitStunOptionsToggle = EditorGUILayout.Foldout(moveInfo.hits[i].hitStunOptionsToggle, "Hit Stun Options", EditorStyles.foldout);
										if (moveInfo.hits[i].hitStunOptionsToggle)
										{
											EditorGUILayout.BeginVertical(subGroupStyle);
											{
												EditorGUIUtility.labelWidth = 180;
												EditorGUI.indentLevel += 1;

												moveInfo.hits[i].hitStunType = (HitStunType)EditorGUILayout.EnumPopup("Hit Stun Type:", moveInfo.hits[i].hitStunType, enumStyle);
												moveInfo.hits[i].resetPreviousHitStun = EditorGUILayout.Toggle("Reset Hit Stun", moveInfo.hits[i].resetPreviousHitStun, toggleStyle);
												moveInfo.hits[i].resetCrumples = EditorGUILayout.Toggle("Reset Crumples", moveInfo.hits[i].resetCrumples, toggleStyle);

												bool preSetValues = moveInfo.hits[i].hitType == HitType.MidKnockdown || moveInfo.hits[i].hitType == HitType.HighKnockdown || moveInfo.hits[i].hitType == HitType.Sweep;
												if (preSetValues)
													moveInfo.hits[i].customStunValues = EditorGUILayout.Toggle("Custom Stun Values", moveInfo.hits[i].customStunValues, toggleStyle);
												EditorGUI.BeginDisabledGroup(preSetValues && !moveInfo.hits[i].customStunValues);
												{
													if (moveInfo.hits[i].hitStunType == HitStunType.FrameAdvantage)
													{
														EditorGUILayout.LabelField("Frame Advantage on Hit:");
														moveInfo.hits[i].frameAdvantageOnHit = EditorGUILayout.IntSlider("", moveInfo.hits[i].frameAdvantageOnHit, -40, 120);
													}
													else
													{
														moveInfo.hits[i]._hitStunOnHit = EditorGUILayout.FloatField("Hit Stun on Hit:", (float)moveInfo.hits[i]._hitStunOnHit);
														if (moveInfo.hits[i].hitStunType == HitStunType.Frames)
															moveInfo.hits[i]._hitStunOnHit = FPMath.Ceiling(moveInfo.hits[i]._hitStunOnHit);
													}
												}
												EditorGUI.EndDisabledGroup();

												if (moveInfo.hits[i].hitStunType == HitStunType.FrameAdvantage)
												{
													EditorGUILayout.LabelField("Frame Advantage on Block:");
													moveInfo.hits[i].frameAdvantageOnBlock = EditorGUILayout.IntSlider("", moveInfo.hits[i].frameAdvantageOnBlock, -40, 120);
												}
												else
												{
													moveInfo.hits[i]._hitStunOnBlock = EditorGUILayout.FloatField("Hit Stun on Block:", (float)moveInfo.hits[i]._hitStunOnBlock);
													if (moveInfo.hits[i].hitStunType == HitStunType.Frames)
														moveInfo.hits[i]._hitStunOnBlock = FPMath.Ceiling(moveInfo.hits[i]._hitStunOnBlock);
												}

												EditorGUI.indentLevel -= 1;
												EditorGUIUtility.labelWidth = 150;

												EditorGUILayout.Space();
											}
											EditorGUILayout.EndVertical();
										}

										// Force Toggle
										moveInfo.hits[i].forceOptionsToggle = EditorGUILayout.Foldout(moveInfo.hits[i].forceOptionsToggle, "Force Options", EditorStyles.foldout);
										if (moveInfo.hits[i].forceOptionsToggle)
										{
											EditorGUILayout.BeginVertical(subGroupStyle);
											{
												EditorGUI.indentLevel += 1;
												EditorGUIUtility.labelWidth = 240;
												moveInfo.hits[i].opponentForceToggle = EditorGUILayout.Foldout(moveInfo.hits[i].opponentForceToggle, "Opponent", EditorStyles.foldout);
												if (moveInfo.hits[i].opponentForceToggle)
												{
													EditorGUI.indentLevel += 1;
													EditorGUI.BeginDisabledGroup(moveInfo.hits[i].hitType == HitType.MidKnockdown || moveInfo.hits[i].hitType == HitType.HighKnockdown || moveInfo.hits[i].hitType == HitType.Sweep);
													{
														moveInfo.hits[i].resetPreviousHorizontalPush = EditorGUILayout.Toggle("Reset X Forces", moveInfo.hits[i].resetPreviousHorizontalPush, toggleStyle);
														moveInfo.hits[i].resetPreviousVerticalPush = EditorGUILayout.Toggle("Reset Y Forces", moveInfo.hits[i].resetPreviousVerticalPush, toggleStyle);
														moveInfo.hits[i]._pushForce = FPVector.ToFPVector(EditorGUILayout.Vector2Field("Applied Force", moveInfo.hits[i]._pushForce.ToVector2()));
														moveInfo.hits[i].applyDifferentAirForce = EditorGUILayout.Toggle("Apply Different Air Force", moveInfo.hits[i].applyDifferentAirForce, toggleStyle);
														if (moveInfo.hits[i].applyDifferentAirForce)
															moveInfo.hits[i]._pushForceAir = FPVector.ToFPVector(EditorGUILayout.Vector2Field("Applied Force (Air)", moveInfo.hits[i]._pushForceAir.ToVector2()));
														moveInfo.hits[i].applyDifferentBlockForce = EditorGUILayout.Toggle("Apply Different Block Force", moveInfo.hits[i].applyDifferentBlockForce, toggleStyle);
														if (moveInfo.hits[i].applyDifferentBlockForce)
															moveInfo.hits[i]._pushForceBlock = FPVector.ToFPVector(EditorGUILayout.Vector2Field("Applied Force (Block)", moveInfo.hits[i]._pushForceBlock.ToVector2()));

													}
													EditorGUI.EndDisabledGroup();
													EditorGUI.indentLevel -= 1;
												}
												moveInfo.hits[i].selfForceToggle = EditorGUILayout.Foldout(moveInfo.hits[i].selfForceToggle, "Self", EditorStyles.foldout);
												if (moveInfo.hits[i].selfForceToggle)
												{
													EditorGUI.indentLevel += 1;
													moveInfo.hits[i].resetPreviousHorizontal = EditorGUILayout.Toggle("Reset X Forces", moveInfo.hits[i].resetPreviousHorizontal, toggleStyle);
													moveInfo.hits[i].resetPreviousVertical = EditorGUILayout.Toggle("Reset Y Forces", moveInfo.hits[i].resetPreviousVertical, toggleStyle);
													moveInfo.hits[i]._appliedForce = FPVector.ToFPVector(EditorGUILayout.Vector2Field("Applied Force", moveInfo.hits[i]._appliedForce.ToVector2()));
													EditorGUI.indentLevel -= 1;
												}

												EditorGUIUtility.labelWidth = 150;
												EditorGUI.indentLevel -= 1;

												EditorGUILayout.Space();
											}
											EditorGUILayout.EndVertical();
										}

										// Stage Reactions
										moveInfo.hits[i].stageReactionsToggle = EditorGUILayout.Foldout(moveInfo.hits[i].stageReactionsToggle, "Stage Reactions", EditorStyles.foldout);
										if (moveInfo.hits[i].stageReactionsToggle)
										{
											EditorGUILayout.BeginVertical(subGroupStyle);
											{
												EditorGUI.indentLevel += 1;
												EditorGUIUtility.labelWidth = 220;
												moveInfo.hits[i].cornerPush = EditorGUILayout.Toggle("Corner Push", moveInfo.hits[i].cornerPush, toggleStyle);
												moveInfo.hits[i].groundBounce = EditorGUILayout.Toggle("Ground Bounce", moveInfo.hits[i].groundBounce, toggleStyle);
												if (moveInfo.hits[i].groundBounce)
												{
													moveInfo.hits[i].groundBounceToggle = EditorGUILayout.Foldout(moveInfo.hits[i].groundBounceToggle, "Ground Bounce Options", EditorStyles.foldout);
													if (moveInfo.hits[i].groundBounceToggle)
													{
														EditorGUI.indentLevel += 1;
														EditorGUIUtility.labelWidth = 240;
														moveInfo.hits[i].overrideForcesOnGroundBounce = EditorGUILayout.Toggle("Override Bounce Forces", moveInfo.hits[i].overrideForcesOnGroundBounce, toggleStyle);
														EditorGUI.BeginDisabledGroup(!moveInfo.hits[i].overrideForcesOnGroundBounce);
														{
															moveInfo.hits[i].resetGroundBounceHorizontalPush = EditorGUILayout.Toggle("Reset X Force", moveInfo.hits[i].resetGroundBounceHorizontalPush, toggleStyle);
															moveInfo.hits[i].resetGroundBounceVerticalPush = EditorGUILayout.Toggle("Reset Y Force", moveInfo.hits[i].resetGroundBounceVerticalPush, toggleStyle);
															moveInfo.hits[i]._groundBouncePushForce = FPVector.ToFPVector(EditorGUILayout.Vector2Field("Applied Force", moveInfo.hits[i]._groundBouncePushForce.ToVector2()));
														}
														EditorGUI.EndDisabledGroup();
														EditorGUI.indentLevel -= 1;
													}
												}

												moveInfo.hits[i].wallBounce = EditorGUILayout.Toggle("Wall Bounce", moveInfo.hits[i].wallBounce, toggleStyle);
												if (moveInfo.hits[i].wallBounce)
												{
													moveInfo.hits[i].wallBounceToggle = EditorGUILayout.Foldout(moveInfo.hits[i].wallBounceToggle, "Wall Bounce Options", EditorStyles.foldout);
													if (moveInfo.hits[i].wallBounceToggle)
													{
														EditorGUI.indentLevel += 1;
														EditorGUIUtility.labelWidth = 240;
														moveInfo.hits[i].knockOutOnWallBounce = EditorGUILayout.Toggle("Knockdown", moveInfo.hits[i].knockOutOnWallBounce, toggleStyle);
														moveInfo.hits[i].bounceOnCameraEdge = EditorGUILayout.Toggle("Bounce on Camera Edge", moveInfo.hits[i].bounceOnCameraEdge, toggleStyle);
														moveInfo.hits[i].overrideForcesOnWallBounce = EditorGUILayout.Toggle("Override Bounce Forces", moveInfo.hits[i].overrideForcesOnWallBounce, toggleStyle);
														EditorGUI.BeginDisabledGroup(!moveInfo.hits[i].overrideForcesOnWallBounce);
														{
															moveInfo.hits[i].resetWallBounceHorizontalPush = EditorGUILayout.Toggle("Reset X Force", moveInfo.hits[i].resetWallBounceHorizontalPush, toggleStyle);
															moveInfo.hits[i].resetWallBounceVerticalPush = EditorGUILayout.Toggle("Reset Y Force", moveInfo.hits[i].resetWallBounceVerticalPush, toggleStyle);
															moveInfo.hits[i]._wallBouncePushForce = FPVector.ToFPVector(EditorGUILayout.Vector2Field("Applied Force", moveInfo.hits[i]._wallBouncePushForce.ToVector2()));
														}
														EditorGUI.EndDisabledGroup();
														EditorGUI.indentLevel -= 1;
													}
												};

												EditorGUIUtility.labelWidth = 150;
												EditorGUI.indentLevel -= 1;

												EditorGUILayout.Space();
											}
											EditorGUILayout.EndVertical();
										}

										// Pull In Toggle
										moveInfo.hits[i].pullInToggle = EditorGUILayout.Foldout(moveInfo.hits[i].pullInToggle, "Pull In Options", EditorStyles.foldout);
										if (moveInfo.hits[i].pullInToggle)
										{
											EditorGUIUtility.labelWidth = 180;
											EditorGUILayout.BeginVertical(subGroupStyle);
											{
												EditorGUI.indentLevel += 1;
												pullEnemyInToggle = EditorGUILayout.Foldout(pullEnemyInToggle, "Opponent Towards Self", EditorStyles.foldout);
												if (pullEnemyInToggle)
												{
													EditorGUI.indentLevel += 1;
													moveInfo.hits[i].pullEnemyIn.speed = EditorGUILayout.IntSlider("Speed:", moveInfo.hits[i].pullEnemyIn.speed, 1, 100);
													moveInfo.hits[i].pullEnemyIn.characterBodyPart = (BodyPart)EditorGUILayout.EnumPopup("Body Part (self):", moveInfo.hits[i].pullEnemyIn.characterBodyPart, enumStyle);
													moveInfo.hits[i].pullEnemyIn.enemyBodyPart = (BodyPart)EditorGUILayout.EnumPopup("Body Part (enemy):", moveInfo.hits[i].pullEnemyIn.enemyBodyPart, enumStyle);
													moveInfo.hits[i].pullEnemyIn._targetDistance = EditorGUILayout.FloatField("Distance:", (float)moveInfo.hits[i].pullEnemyIn._targetDistance);
													moveInfo.hits[i].pullEnemyIn.forceStand = EditorGUILayout.Toggle("Force Grounded", moveInfo.hits[i].pullEnemyIn.forceStand, toggleStyle);
													EditorGUI.indentLevel -= 1;
												}
												pullSelfInToggle = EditorGUILayout.Foldout(pullSelfInToggle, "Self Towards Opponent", EditorStyles.foldout);
												if (pullSelfInToggle)
												{
													EditorGUI.indentLevel += 1;
													moveInfo.hits[i].pullSelfIn.speed = EditorGUILayout.IntSlider("Speed:", moveInfo.hits[i].pullSelfIn.speed, 1, 100);
													moveInfo.hits[i].pullSelfIn.characterBodyPart = (BodyPart)EditorGUILayout.EnumPopup("Body Part (self):", moveInfo.hits[i].pullSelfIn.characterBodyPart, enumStyle);
													moveInfo.hits[i].pullSelfIn.enemyBodyPart = (BodyPart)EditorGUILayout.EnumPopup("Body Part (enemy):", moveInfo.hits[i].pullSelfIn.enemyBodyPart, enumStyle);
													moveInfo.hits[i].pullSelfIn._targetDistance = EditorGUILayout.FloatField("Distance:", (float)moveInfo.hits[i].pullSelfIn._targetDistance);
													moveInfo.hits[i].pullSelfIn.forceStand = EditorGUILayout.Toggle("Force Grounded", moveInfo.hits[i].pullSelfIn.forceStand, toggleStyle);
													EditorGUI.indentLevel -= 1;
												}

												EditorGUIUtility.labelWidth = 150;
												EditorGUI.indentLevel -= 1;

												EditorGUILayout.Space();
											}
											EditorGUILayout.EndVertical();

										}

										// Override Events Toggle
										moveInfo.hits[i].overrideEventsToggle = EditorGUILayout.Foldout(moveInfo.hits[i].overrideEventsToggle, "Override Events (On Hit)", EditorStyles.foldout);
										if (moveInfo.hits[i].overrideEventsToggle)
										{
											EditorGUILayout.BeginVertical(subGroupStyle);
											{
												EditorGUI.indentLevel += 1;
												EditorGUIUtility.labelWidth = 240;
												moveInfo.hits[i].overrideHitEffects = EditorGUILayout.Toggle("Override Hit Effects (Hit)", moveInfo.hits[i].overrideHitEffects, toggleStyle);
												EditorGUIUtility.labelWidth = 210;
												if (moveInfo.hits[i].overrideHitEffects)
													HitOptionBlock("Hit Effects", moveInfo.hits[i].hitEffects);
												EditorGUIUtility.labelWidth = 240;
												moveInfo.hits[i].overrideHitEffectsBlock = EditorGUILayout.Toggle("Override Hit Effects (Block)", moveInfo.hits[i].overrideHitEffectsBlock, toggleStyle);
												EditorGUIUtility.labelWidth = 210;
												if (moveInfo.hits[i].overrideHitEffectsBlock)
													HitOptionBlock("Hit Effects", moveInfo.hits[i].hitEffectsBlock);
												EditorGUIUtility.labelWidth = 240;

												moveInfo.hits[i].overrideHitAnimation = EditorGUILayout.Toggle("Override Hit Animation", moveInfo.hits[i].overrideHitAnimation, toggleStyle);
												EditorGUIUtility.labelWidth = 210;
												if (moveInfo.hits[i].overrideHitAnimation)
													moveInfo.hits[i].newHitAnimation = (BasicMoveReference)EditorGUILayout.EnumPopup("- Hit Animation:", moveInfo.hits[i].newHitAnimation, enumStyle);
												EditorGUIUtility.labelWidth = 240;

												//moveInfo.hits[i].overrideHitAcceleration = EditorGUILayout.Toggle("Override Hit Acceleration", moveInfo.hits[i].overrideHitAcceleration, toggleStyle);

												moveInfo.hits[i].overrideEffectSpawnPoint = EditorGUILayout.Toggle("Override Effect Spawn Point", moveInfo.hits[i].overrideEffectSpawnPoint, toggleStyle);
												EditorGUIUtility.labelWidth = 210;
												if (moveInfo.hits[i].overrideEffectSpawnPoint)
													moveInfo.hits[i].spawnPoint = (HitEffectSpawnPoint)EditorGUILayout.EnumPopup("- Spawn Point:", moveInfo.hits[i].spawnPoint, enumStyle);
												EditorGUIUtility.labelWidth = 240;

												moveInfo.hits[i].overrideHitAnimationBlend = EditorGUILayout.Toggle("Override Hit Animation Blend-in", moveInfo.hits[i].overrideHitAnimationBlend, toggleStyle);
												EditorGUIUtility.labelWidth = 210;
												if (moveInfo.hits[i].overrideHitAnimationBlend)
													moveInfo.hits[i]._newHitBlendingIn = EditorGUILayout.FloatField("- Blending In:", (float)moveInfo.hits[i]._newHitBlendingIn);
												EditorGUIUtility.labelWidth = 240;

												moveInfo.hits[i].overrideJuggleWeight = EditorGUILayout.Toggle("Override Juggle Weight", moveInfo.hits[i].overrideJuggleWeight, toggleStyle);
												EditorGUIUtility.labelWidth = 210;
												if (moveInfo.hits[i].overrideJuggleWeight)
													moveInfo.hits[i]._newJuggleWeight = EditorGUILayout.FloatField("- New Weight:", (float)moveInfo.hits[i]._newJuggleWeight);
												EditorGUIUtility.labelWidth = 240;

												moveInfo.hits[i].overrideAirRecoveryType = EditorGUILayout.Toggle("Override Air Recovery Type", moveInfo.hits[i].overrideAirRecoveryType, toggleStyle);
												EditorGUIUtility.labelWidth = 210;
												if (moveInfo.hits[i].overrideAirRecoveryType)
												{
													moveInfo.hits[i].newAirRecoveryType = (AirRecoveryType)EditorGUILayout.EnumPopup("- New Air Recovery Type:", moveInfo.hits[i].newAirRecoveryType, enumStyle);
													if (moveInfo.hits[i].newAirRecoveryType == AirRecoveryType.CantMove)
													{
														moveInfo.hits[i].instantAirRecovery = EditorGUILayout.Toggle("- Instant Air Recovery", moveInfo.hits[i].instantAirRecovery, toggleStyle);
													}
												}
												EditorGUIUtility.labelWidth = 240;

												moveInfo.hits[i].overrideCameraSpeed = EditorGUILayout.Toggle("Override Camera Speed", moveInfo.hits[i].overrideCameraSpeed, toggleStyle);
												EditorGUIUtility.labelWidth = 210;
												if (moveInfo.hits[i].overrideCameraSpeed)
												{
													moveInfo.hits[i]._newMovementSpeed = EditorGUILayout.FloatField("- New Movement Speed:", (float)moveInfo.hits[i]._newMovementSpeed);
													moveInfo.hits[i]._newRotationSpeed = EditorGUILayout.FloatField("- New Rotation Speed:", (float)moveInfo.hits[i]._newRotationSpeed);
													moveInfo.hits[i]._cameraSpeedDuration = EditorGUILayout.FloatField("- Override Duration:", (float)moveInfo.hits[i]._cameraSpeedDuration);
												};

												EditorGUIUtility.labelWidth = 150;
												EditorGUI.indentLevel -= 1;

												EditorGUILayout.Space();
											}
											EditorGUILayout.EndVertical();
										}

									}
									else if (moveInfo.hits[i].hitConfirmType == HitConfirmType.Throw)
									{

										EditorGUIUtility.labelWidth = 180;

										moveInfo.hits[i].throwMove = (MoveInfo)EditorGUILayout.ObjectField("Throw Move Confirm:", moveInfo.hits[i].throwMove, typeof(MoveInfo), false);
										EditorGUILayout.BeginHorizontal();
										{
											EditorGUILayout.Space();
											if (GUILayout.Button("Open Move", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Width(250) }))
											{
												MoveEditorWindow.sentMoveInfo = moveInfo.hits[i].throwMove;
												MoveEditorWindow.Init();
											}
											EditorGUILayout.Space();
										}
										EditorGUILayout.EndHorizontal();

										EditorGUILayout.Space();

										moveInfo.hits[i].techable = EditorGUILayout.Toggle("Techable", moveInfo.hits[i].techable, toggleStyle);
										if (moveInfo.hits[i].techable)
										{
											moveInfo.hits[i].techMove = (MoveInfo)EditorGUILayout.ObjectField("Tech Move:", moveInfo.hits[i].techMove, typeof(MoveInfo), false);
											EditorGUILayout.BeginHorizontal();
											{
												EditorGUILayout.Space();
												if (GUILayout.Button("Open Move", new GUILayoutOption[] { GUILayout.ExpandWidth(true), GUILayout.Width(250) }))
												{
													MoveEditorWindow.sentMoveInfo = moveInfo.hits[i].techMove;
													MoveEditorWindow.Init();
												}
												EditorGUILayout.Space();
											}
											EditorGUILayout.EndHorizontal();
										}

										EditorGUIUtility.labelWidth = 150;
									}

									EditorGUILayout.Space();
								}
								EditorGUILayout.EndVertical();
							}

							if (StyledButton("New Hit"))
								moveInfo.hits = AddElement<Hit>(moveInfo.hits, new Hit());

							EditorGUI.indentLevel -= 1;
						}
						EditorGUILayout.EndVertical();
					}

					// Blockable Area Toggle
					blockableAreaToggle = EditorGUILayout.Foldout(blockableAreaToggle, "Blockable Area", EditorStyles.foldout);
					if (blockableAreaToggle)
					{
						EditorGUILayout.BeginVertical(subGroupStyle);
						{
							EditorGUI.indentLevel += 1;
							StyledMinMaxSlider("Active Frames", ref moveInfo.blockableArea.activeFramesBegin, ref moveInfo.blockableArea.activeFramesEnds, 0, moveInfo.totalFrames, EditorGUI.indentLevel);

							moveInfo.blockableArea.bodyPart = (BodyPart)EditorGUILayout.EnumPopup("Body Part:", moveInfo.blockableArea.bodyPart, enumStyle);
							moveInfo.blockableArea.shape = (HitBoxShape)EditorGUILayout.EnumPopup("Shape:", moveInfo.blockableArea.shape, enumStyle);
							if (moveInfo.blockableArea.shape == HitBoxShape.circle)
							{
								moveInfo.blockableArea._radius = EditorGUILayout.FloatField("Radius:", (float)moveInfo.blockableArea._radius);
								moveInfo.blockableArea._offSet = FPVector.ToFPVector(EditorGUILayout.Vector2Field("Off Set:", moveInfo.blockableArea._offSet.ToVector2()));
							}
							else
							{
								moveInfo.blockableArea.rect = EditorGUILayout.RectField("Rectangle:", moveInfo.blockableArea.rect);
								moveInfo.blockableArea._rect = new FPRect(moveInfo.blockableArea.rect);
							}

							EditorGUI.indentLevel -= 1;
						}
						EditorGUILayout.EndVertical();
					}
					EditorGUI.indentLevel -= 1;

				}
				EditorGUILayout.EndVertical();
			}

		}
		EditorGUILayout.EndVertical();
		// End Active Frame Options


		// Begin Armor Options
		EditorGUILayout.BeginVertical("rootGroupStyle");
		{
			EditorGUILayout.BeginHorizontal();
			{
				armorOptions = EditorGUILayout.Foldout(armorOptions, "Armor Options", EditorStyles.foldout);
				helpButton("move:armor");
			}
			EditorGUILayout.EndHorizontal();

			if (armorOptions)
			{
				EditorGUILayout.BeginVertical(subGroupStyle);
				{
					EditorGUI.indentLevel += 2;
					StyledMinMaxSlider("Armor Frames", ref moveInfo.armorOptions.activeFramesBegin, ref moveInfo.armorOptions.activeFramesEnds, 0, moveInfo.totalFrames, EditorGUI.indentLevel);

					EditorGUI.indentLevel -= 1;
					EditorGUIUtility.labelWidth = 200;
					moveInfo.armorOptions.hitAbsorption = Mathf.Max(0, EditorGUILayout.IntField("Hit Absorption:", moveInfo.armorOptions.hitAbsorption));

					EditorGUI.BeginDisabledGroup(moveInfo.armorOptions.hitAbsorption == 0);
					{
						moveInfo.armorOptions.damageAbsorption = EditorGUILayout.IntSlider("Damage Absorption (%):", moveInfo.armorOptions.damageAbsorption, 0, 100);

						moveInfo.armorOptions.overrideHitEffects = EditorGUILayout.Toggle("Override Hit Effects", moveInfo.armorOptions.overrideHitEffects, toggleStyle);
						if (moveInfo.armorOptions.overrideHitEffects)
							HitOptionBlock("Hit Effects", moveInfo.armorOptions.hitEffects);

						bodyPartsToggle = EditorGUILayout.Foldout(bodyPartsToggle, "Non Affected Body Parts (" + moveInfo.armorOptions.nonAffectedBodyParts.Length + ")", EditorStyles.foldout);
						if (bodyPartsToggle)
						{
							EditorGUILayout.BeginVertical(subGroupStyle);
							{
								EditorGUILayout.Space();
								EditorGUI.indentLevel += 1;
								if (moveInfo.armorOptions.nonAffectedBodyParts != null)
								{
									for (int y = 0; y < moveInfo.armorOptions.nonAffectedBodyParts.Length; y++)
									{
										EditorGUILayout.Space();
										EditorGUILayout.BeginVertical(subArrayElementStyle);
										{
											EditorGUILayout.BeginHorizontal();
											{
												moveInfo.armorOptions.nonAffectedBodyParts[y] = (BodyPart)EditorGUILayout.EnumPopup("Body Part:", moveInfo.armorOptions.nonAffectedBodyParts[y], enumStyle);
												if (GUILayout.Button("", "PaneOptions"))
												{
													PaneOptions<BodyPart>(moveInfo.armorOptions.nonAffectedBodyParts, moveInfo.armorOptions.nonAffectedBodyParts[y], delegate (BodyPart[] newElement) { moveInfo.armorOptions.nonAffectedBodyParts = newElement; });
												}
											}
											EditorGUILayout.EndHorizontal();
											EditorGUILayout.Space();
										}
										EditorGUILayout.EndVertical();
									}
								}
								if (StyledButton("New Body Part"))
									moveInfo.armorOptions.nonAffectedBodyParts = AddElement<BodyPart>(moveInfo.armorOptions.nonAffectedBodyParts, BodyPart.none);

								EditorGUI.indentLevel -= 1;
							}
							EditorGUILayout.EndVertical();
						}
						EditorGUIUtility.labelWidth = 150;
						EditorGUILayout.Space();
					}
					EditorGUI.EndDisabledGroup();

					EditorGUI.indentLevel -= 1;
				}
				EditorGUILayout.EndVertical();
			}

		}
		EditorGUILayout.EndVertical();
		// End Armor Options


		// Begin Invincible Frames Options
		EditorGUILayout.BeginVertical("rootGroupStyle");
		{
			EditorGUILayout.BeginHorizontal();
			{
				invincibleFramesOptions = EditorGUILayout.Foldout(invincibleFramesOptions, "Invincible Frames (" + moveInfo.invincibleBodyParts.Length + ")", EditorStyles.foldout);
				helpButton("move:invincibleframes");
			}
			EditorGUILayout.EndHorizontal();

			if (invincibleFramesOptions)
			{
				EditorGUILayout.BeginVertical(subGroupStyle);
				{
					EditorGUI.indentLevel += 1;
					List<Vector3> castingValues = new List<Vector3>();
					foreach (InvincibleBodyParts invBodyPart in moveInfo.invincibleBodyParts)
						castingValues.Add(new Vector3(invBodyPart.activeFramesBegin, invBodyPart.activeFramesEnds));
					StyledMarker("Invincible Frames Timeline", castingValues.ToArray(), moveInfo.totalFrames, EditorGUI.indentLevel, false);

					EditorGUI.indentLevel += 1;
					for (int i = 0; i < moveInfo.invincibleBodyParts.Length; i++)
					{
						EditorGUILayout.Space();
						EditorGUILayout.BeginVertical(arrayElementStyle);
						{
							EditorGUILayout.Space();
							EditorGUILayout.BeginHorizontal();
							{
								StyledMinMaxSlider("Invincible Frames", ref moveInfo.invincibleBodyParts[i].activeFramesBegin, ref moveInfo.invincibleBodyParts[i].activeFramesEnds, 0, moveInfo.totalFrames, EditorGUI.indentLevel);
								if (GUILayout.Button("", "PaneOptions"))
								{
									PaneOptions<InvincibleBodyParts>(moveInfo.invincibleBodyParts, moveInfo.invincibleBodyParts[i], delegate (InvincibleBodyParts[] newElement) { moveInfo.invincibleBodyParts = newElement; });
								}
							}
							EditorGUILayout.EndHorizontal();

							EditorGUILayout.Space();

							EditorGUIUtility.labelWidth = 240;
							moveInfo.invincibleBodyParts[i].completelyInvincible = EditorGUILayout.Toggle("Completely Invincible", moveInfo.invincibleBodyParts[i].completelyInvincible, toggleStyle);
							moveInfo.invincibleBodyParts[i].ignoreBodyColliders = EditorGUILayout.Toggle("Ignore Body Colliders", moveInfo.invincibleBodyParts[i].ignoreBodyColliders, toggleStyle);
							EditorGUIUtility.labelWidth = 150;

							if (!moveInfo.invincibleBodyParts[i].completelyInvincible)
							{
								bodyPartsToggle = EditorGUILayout.Foldout(bodyPartsToggle, "Body Parts (" + moveInfo.invincibleBodyParts[i].bodyParts.Length + ")", EditorStyles.foldout);
								if (bodyPartsToggle)
								{
									EditorGUILayout.Space();
									EditorGUILayout.BeginVertical(subGroupStyle);
									{
										EditorGUI.indentLevel += 1;
										if (moveInfo.invincibleBodyParts[i].bodyParts != null)
										{
											for (int y = 0; y < moveInfo.invincibleBodyParts[i].bodyParts.Length; y++)
											{
												EditorGUILayout.Space();
												EditorGUILayout.BeginVertical(subArrayElementStyle);
												{
													EditorGUILayout.BeginHorizontal();
													{
														moveInfo.invincibleBodyParts[i].bodyParts[y] = (BodyPart)EditorGUILayout.EnumPopup("Body Part:", moveInfo.invincibleBodyParts[i].bodyParts[y], enumStyle);
														if (GUILayout.Button("", "PaneOptions"))
														{
															PaneOptions<BodyPart>(moveInfo.invincibleBodyParts[i].bodyParts, moveInfo.invincibleBodyParts[i].bodyParts[y], delegate (BodyPart[] newElement) { moveInfo.invincibleBodyParts[i].bodyParts = newElement; });
														}
													}
													EditorGUILayout.EndHorizontal();
													EditorGUILayout.Space();
												}
												EditorGUILayout.EndVertical();
											}
										}

										if (StyledButton("New Body Part"))
											moveInfo.invincibleBodyParts[i].bodyParts = AddElement<BodyPart>(moveInfo.invincibleBodyParts[i].bodyParts, BodyPart.none);

										EditorGUI.indentLevel -= 1;
									}
									EditorGUILayout.EndHorizontal();
								}
							}
							EditorGUILayout.Space();
						}
						EditorGUILayout.EndVertical();
					}

					if (StyledButton("New Invincible Frame Group"))
						moveInfo.invincibleBodyParts = AddElement<InvincibleBodyParts>(moveInfo.invincibleBodyParts, new InvincibleBodyParts());

					EditorGUI.indentLevel -= 2;

				}
				EditorGUILayout.EndVertical();
			}

		}
		EditorGUILayout.EndVertical();
		// End Invincible Frames Options
	}
	EditorGUILayout.EndScrollView();

		if (GUI.changed)
		{
			Undo.RecordObject(move, "Move Editor Modify");
			EditorUtility.SetDirty(move);
		}
	}
	*/

/*
*/