﻿using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace SA.BehaviorEditor
{
    [CreateAssetMenu(menuName = "Editor/Nodes/State Node")]
    public class StateNode : DrawNode
    {
        private string newStateName = "";
        private string newFolderName = "";
        private float _addStateModeHeight = 200;
        private string newActionName;
        private string newActionFolderName;
        private string newActionMenuPath;
        private string newSOName;

        public override void DrawWindow(BaseNode b)
        {
            if (b.stateRef.currentState == null)
            {
                EditorGUILayout.LabelField("State Name");
                newStateName = EditorGUILayout.TextField("", newStateName);
                EditorGUILayout.LabelField("New folder name :");
                newFolderName = EditorGUILayout.TextField("", newFolderName);

                if (GUILayout.Button("Create State S.O."))
                {
                    b.stateRef.currentState = BehaviorIO.CreateStateSO(newStateName, newFolderName);
                }

                EditorGUILayout.LabelField("Or, add state to modify:");
                b.windowRect.height = _addStateModeHeight;
            }
            else
            {
                if (!b.collapse)
                {
                }
                else
                {
                    b.windowRect.height = 100;
                }

                b.collapse = EditorGUILayout.Toggle(" ", b.collapse);
            }

            b.stateRef.currentState = (State)EditorGUILayout.ObjectField(b.stateRef.currentState, typeof(State), false);

            if (b.previousCollapse != b.collapse)
            {
                b.previousCollapse = b.collapse;
            }

            if (b.stateRef.previousState != b.stateRef.currentState)
            {
                //b.serializedState = null;
                b.isDuplicate = BehaviorEditor.settings.currentGraph.IsStateDuplicate(b);
                b.stateRef.previousState = b.stateRef.currentState;

                if (!b.isDuplicate)
                {
                    Vector3 pos = new Vector3(b.windowRect.x, b.windowRect.y, 0);
                    pos.x += b.windowRect.width * 2;

                    SetupReordableLists(b);

                    //Load transtions
                    for (int i = 0; i < b.stateRef.currentState.transitions.Count; i++)
                    {
                        pos.y += i * 100;
                        BehaviorEditor.AddTransitionNodeFromTransition(b.stateRef.currentState.transitions[i], b, pos);
                    }

                    BehaviorEditor.forceSetDirty = true;
                }
            }

            if (b.isDuplicate)
            {
                EditorGUILayout.LabelField("State is a duplicate!");
                b.windowRect.height = _addStateModeHeight;
                return;
            }

            if (b.stateRef.currentState != null)
            {
                b.isAssigned = true;

                if (!b.collapse)
                {
                    if (b.stateRef.serializedState == null)
                    {
                        SetupReordableLists(b);

                        //	SerializedObject serializedState = new SerializedObject(b.stateRef.currentState);
                    }

                    float standard = 150;
                    b.stateRef.serializedState.Update();


                    b.showActions = EditorGUILayout.Toggle("Show Actions ", b.showActions);
                    if (b.showActions)
                    {
                        b.createActions = EditorGUILayout.Toggle("Create Actions ", b.createActions);
                        if (b.createActions)
                        {
                            EditorGUILayout.LabelField("Select Actions :");
                            EditorGUILayout.LabelField("New Action Name :");
                            newActionName = EditorGUILayout.TextField("", newActionName);
                            EditorGUILayout.LabelField("New folder path (USE '\\') :");
                            newActionFolderName = EditorGUILayout.TextField("", newActionFolderName);
                            EditorGUILayout.LabelField("SO Name : ");
                            newSOName = EditorGUILayout.TextField("", newSOName);
                            EditorGUILayout.LabelField("New Menu Path (USE '/'):");
                            newActionMenuPath = EditorGUILayout.TextField("", newActionMenuPath);

                            if (GUILayout.Button("Create Script and State S.O."))
                            {
                                EditorGUILayout.LabelField($"{newActionName} @ {newActionFolderName} already exists.");
                                BehaviorIO.CreateScriptForActionWithMenuPath(newActionName, newActionFolderName,
                                    newActionMenuPath);
                            }

                            if (GUILayout.Button("Create Action SO at Menu Path"))
                            {
                                BehaviorIO.CreateSOForAction(newActionName, newSOName, newActionMenuPath);
                            }
                        }

                        b.showUpdateAndFixed = EditorGUILayout.Toggle("Show Update / FixedUpdate ", b.showUpdateAndFixed);
                        if (b.showUpdateAndFixed)
                        {
                            EditorGUILayout.LabelField("");
                            b.stateRef.onFixedList.DoLayoutList();
                            EditorGUILayout.LabelField("");
                            b.stateRef.onUpdateList.DoLayoutList();
                        }

                        standard += 100 + 40 + (b.stateRef.onUpdateList.count + b.stateRef.onFixedList.count) * 20;
                    }

                    b.showEnterExit = EditorGUILayout.Toggle("Show Enter/Exit ", b.showEnterExit);
                    if (b.showEnterExit)
                    {
                        EditorGUILayout.LabelField("");
                        b.stateRef.onEnterList.DoLayoutList();
                        EditorGUILayout.LabelField("");
                        b.stateRef.onExitList.DoLayoutList();
                        standard += 100 + 40 + (b.stateRef.onEnterList.count + b.stateRef.onExitList.count) * 20;
                    }

                    b.stateRef.serializedState.ApplyModifiedProperties();
                    b.windowRect.height = standard + 500;
                }
            }
            else
            {
                b.isAssigned = false;
            }
        }


        void SetupReordableLists(BaseNode b)
        {
            b.stateRef.serializedState = new SerializedObject(b.stateRef.currentState);
            b.stateRef.onFixedList = new ReorderableList(b.stateRef.serializedState,
                b.stateRef.serializedState.FindProperty("onFixed"), true, true, true, true);
            b.stateRef.onUpdateList = new ReorderableList(b.stateRef.serializedState,
                b.stateRef.serializedState.FindProperty("onUpdate"), true, true, true, true);
            b.stateRef.onEnterList = new ReorderableList(b.stateRef.serializedState,
                b.stateRef.serializedState.FindProperty("onEnter"), true, true, true, true);
            b.stateRef.onExitList = new ReorderableList(b.stateRef.serializedState,
                b.stateRef.serializedState.FindProperty("onExit"), true, true, true, true);

            HandleReordableList(b.stateRef.onFixedList, "On Fixed");
            HandleReordableList(b.stateRef.onUpdateList, "On Update");
            HandleReordableList(b.stateRef.onEnterList, "On Enter");
            HandleReordableList(b.stateRef.onExitList, "On Exit");
        }

        void HandleReordableList(ReorderableList list, string targetName)
        {
            list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, targetName); };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                EditorGUI.ObjectField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element,
                    GUIContent.none);
            };
        }

        public override void DrawCurve(BaseNode b)
        {
        }

        public Transition AddTransition(BaseNode b)
        {
            return b.stateRef.currentState.AddTransition();
        }

        public void ClearReferences()
        {
            //      BehaviorEditor.ClearWindowsFromList(dependencies);
            //    dependencies.Clear();
        }
    }
}