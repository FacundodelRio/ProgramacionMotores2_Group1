﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class DialogueEditor : EditorWindow {
    
    [MenuItem("DialogueEditor/Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogueEditor));
    }

	//TODO Create a button to create a new dialogue
	Dialogue dialogue = null;
	List<DialogueItemWindow> controls = new List<DialogueItemWindow>();

	//TODO Create an id management that actually works.
	int dialogueCount = 0;

    void OnGUI()
    {
        if (GUILayout.Button("Create Window"))
            CreateControl();

        BeginWindows();

        for (int i = 0; i < controls.Count; i++)
        {
			controls[i].rect = GUI.Window(i, controls[i].rect, controls[i].Draw, "Window_" + i);
        }

		Zoom ();
        EndWindows();
    }
		

    public void CreateControl()
    {
		DialogueItem dialogueItem = ScriptableObject.CreateInstance<DialogueItem> ();
		dialogueItem.id = dialogueCount++;
			
		DialogueItemWindow newWindow = new DialogueItemWindow (dialogueItem, this);
		controls.Add(newWindow);

		//Default Dialogue Window Rect Size Reference for Zooming
		_defaultRectSize = new Vector2 (newWindow.rect.width, newWindow.rect.height);
		//
    }

	public void DeleteControl(DialogueItemWindow window)
    {
		controls.Remove(window);
    }


	//TODO Load a dialogue asset
	void LoadDialogue()
	{

	}

	//TODO Save a dialogue asset
	void SaveDialogue()
	{
		
	}

	//START ZOOM CONTROL

	float _zoomSensitivity = 3f;
	float _xScale = 0f;
	float _yScale = 0f;
	float _xDisplacement = 0f;
	float _yDisplacement = 0f;
	float _minXScale = 100f;
	float _minYScale = 75f;
	float _maxXScale = 400f;
	float _maxYScale = 300f;
	Vector2 _defaultRectSize = Vector2.zero;
	Vector2 _winPos = Vector2.zero;

	void Zoom(){
		Event e = Event.current;
		if (e.type == EventType.scrollWheel) {

			if (-e.delta.y > 0f) {
				_xScale = _zoomSensitivity;
				_yScale = _zoomSensitivity * (_defaultRectSize.y / _defaultRectSize.x);
			} else if (-e.delta.y < 0f) {
				_xScale = -_zoomSensitivity;
				_yScale = -_zoomSensitivity * (_defaultRectSize.y / _defaultRectSize.x);
			}

			foreach (var window in controls) {
				if (window.rect.width < _maxXScale && window.rect.height < _maxYScale &&
				    window.rect.width > _minXScale && window.rect.height > _minYScale) {

					_winPos = new Vector2 (window.rect.x + (window.rect.width / 2f), window.rect.y + (window.rect.height / 2f));

					if (e.mousePosition.x < _winPos.x) {
						_xDisplacement = _xScale;
					} else if (e.mousePosition.x > _winPos.x) {
						_xDisplacement = -_xScale;
					}

					if (e.mousePosition.y < _winPos.y) {
						_yDisplacement = _yScale;
					} else if (e.mousePosition.y > _winPos.y) {
						_yDisplacement = -_yScale;
					}

				} else {
					_xDisplacement = 0f;
					_yDisplacement = 0f;
				}
				window.rect = new Rect (
					window.rect.x + _xDisplacement * Vector2.Distance(e.mousePosition, window.rect.position) / window.rect.width,
					window.rect.y + _yDisplacement * Vector2.Distance(e.mousePosition, window.rect.position) / window.rect.height,
					Mathf.Clamp (window.rect.width + _xScale, _minXScale, _maxXScale),
					Mathf.Clamp (window.rect.height + _yScale, _minYScale, _maxYScale)
				);
			}
		}
		Repaint ();
	}
	//END ZOOM CONTROL
}