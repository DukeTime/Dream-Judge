namespace DefaultNamespace.Dialogue_system
{
    #if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;
    using System;
    using System.Linq;

    public class DialogueGraphEditor : EditorWindow
    {
        private DialogueDatabase database;
        private Vector2 scrollPosition;
        private DialoguePhrase selectedPhrase;
        private GUIStyle nodeStyle;
        private GUIStyle selectedNodeStyle;
        private GUIStyle entryPointStyle;
        private Vector2 dragOffset;
        private bool isDragging;
        private string newLinkFromId;
        private string searchString = "";

        [MenuItem("Window/Dialogue Graph Editor")]
        public static void ShowWindow()
        {
            GetWindow<DialogueGraphEditor>("Dialogue Graph");
        }

        private void OnEnable()
        {
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.alignment = TextAnchor.MiddleCenter;

            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
            selectedNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            selectedNodeStyle.alignment = TextAnchor.MiddleCenter;

            entryPointStyle = new GUIStyle();
            entryPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node6.png") as Texture2D;
            entryPointStyle.border = new RectOffset(12, 12, 12, 12);
            entryPointStyle.padding = new RectOffset(20, 20, 20, 20);
            entryPointStyle.alignment = TextAnchor.MiddleCenter;
        }

        private void OnGUI()
        {
            DrawToolbar();

            if (database == null)
            {
                EditorGUILayout.HelpBox("No Dialogue Database selected!", MessageType.Info);
                return;
            }

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            // Рисуем линии сначала (под узлами)
            if (selectedPhrase != null)
            {
                foreach (var choice in selectedPhrase.choices)
                {
                    if (!string.IsNullOrEmpty(choice.nextPhraseId))
                    {
                        var nextPhrase = database.GetPhraseById(choice.nextPhraseId);
                        if (nextPhrase != null)
                        {
                            DrawNodeCurve(selectedPhrase, nextPhrase, choice.text);
                        }
                    }
                }
            }

            // Рисуем точки входа
            foreach (var entryPoint in database.entryPoints)
            {
                if (!string.IsNullOrEmpty(entryPoint.startPhraseId))
                {
                    var startPhrase = database.GetPhraseById(entryPoint.startPhraseId);
                    if (startPhrase != null)
                    {
                        DrawEntryPointCurve(entryPoint, startPhrase);
                    }
                }
            }

            // Рисуем узлы
            foreach (var phrase in database.phrases)
            {
                DrawNode(phrase);
            }

            // Рисуем точки входа
            foreach (var entryPoint in database.entryPoints)
            {
                DrawEntryPoint(entryPoint);
            }

            EditorGUILayout.EndScrollView();

            if (isDragging && selectedPhrase != null)
            {
                selectedPhrase.graphPosition += dragOffset;
                GUI.changed = true;
            }

            if (selectedPhrase != null)
            {
                DrawSelectedNodeInspector();
            }
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            database = (DialogueDatabase)EditorGUILayout.ObjectField(database, typeof(DialogueDatabase), false);
            
            if (GUILayout.Button("New Phrase", EditorStyles.toolbarButton))
            {
                var newPhrase = new DialoguePhrase
                {
                    id = Guid.NewGuid().ToString(),
                    characterName = "New Character",
                    text = "New dialogue text",
                    graphPosition = scrollPosition + new Vector2(200, 200)
                };
                database.phrases.Add(newPhrase);
                selectedPhrase = newPhrase;
            }

            if (GUILayout.Button("New Entry Point", EditorStyles.toolbarButton))
            {
                var newEntryPoint = new DialogueEntryPoint
                {
                    id = Guid.NewGuid().ToString(),
                    condition = "always"
                };
                database.entryPoints.Add(newEntryPoint);
            }

            searchString = EditorGUILayout.TextField(searchString, EditorStyles.toolbarSearchField);

            EditorGUILayout.EndHorizontal();
        }

        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            for (int x = 0; x < widthDivs; x++)
            {
                Handles.DrawLine(new Vector3(x * gridSpacing, 0, 0), new Vector3(x * gridSpacing, position.height, 0f));
            }

            for (int y = 0; y < heightDivs; y++)
            {
                Handles.DrawLine(new Vector3(0, y * gridSpacing, 0f), new Vector3(position.width, y * gridSpacing, 0f));
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void DrawNode(DialoguePhrase phrase)
        {
            var style = (selectedPhrase == phrase) ? selectedNodeStyle : nodeStyle;
            GUILayout.BeginArea(new Rect(phrase.graphPosition.x, phrase.graphPosition.y, 200, 150), style);
            
            EditorGUILayout.LabelField(phrase.characterName, EditorStyles.boldLabel);
            EditorGUILayout.LabelField(phrase.text, EditorStyles.wordWrappedLabel);
            
            GUILayout.EndArea();

            // Обработка событий
            var rect = new Rect(phrase.graphPosition.x, phrase.graphPosition.y, 200, 150);
            var e = Event.current;

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                selectedPhrase = phrase;
                isDragging = true;
                dragOffset = Vector2.zero;
                e.Use();
            }

            if (e.type == EventType.MouseDrag && isDragging && selectedPhrase == phrase)
            {
                dragOffset = e.delta;
                e.Use();
            }

            if (e.type == EventType.MouseUp && isDragging)
            {
                isDragging = false;
                e.Use();
            }

            if (e.type == EventType.ContextClick && rect.Contains(e.mousePosition))
            {
                ShowNodeContextMenu(phrase);
                e.Use();
            }
        }

        private void DrawEntryPoint(DialogueEntryPoint entryPoint)
        {
            var pos = new Vector2(50, database.entryPoints.IndexOf(entryPoint) * 100 + 50);
            GUILayout.BeginArea(new Rect(pos.x, pos.y, 150, 60), entryPointStyle);
            
            EditorGUILayout.LabelField("Entry Point", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(entryPoint.condition, EditorStyles.wordWrappedLabel);
            
            GUILayout.EndArea();

            var rect = new Rect(pos.x, pos.y, 150, 60);
            var e = Event.current;

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                selectedPhrase = null;
                ShowEntryPointContextMenu(entryPoint);
                e.Use();
            }
        }

        private void DrawNodeCurve(DialoguePhrase from, DialoguePhrase to, string choiceText)
        {
            var startPos = new Vector3(from.graphPosition.x + 200, from.graphPosition.y + 75, 0);
            var endPos = new Vector3(to.graphPosition.x, to.graphPosition.y + 75, 0);
            var startTan = startPos + Vector3.right * 50;
            var endTan = endPos + Vector3.left * 50;
            
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.white, null, 3f);
            
            var labelPos = (startPos + endPos) * 0.5f;
            Handles.Label(labelPos, choiceText);
        }

        private void DrawEntryPointCurve(DialogueEntryPoint from, DialoguePhrase to)
        {
            var startPos = new Vector3(200, database.entryPoints.IndexOf(from) * 100 + 80, 0);
            var endPos = new Vector3(to.graphPosition.x, to.graphPosition.y + 75, 0);
            var startTan = startPos + Vector3.right * 50;
            var endTan = endPos + Vector3.left * 50;
            
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.green, null, 3f);
        }

        private void DrawSelectedNodeInspector()
        {
            GUILayout.BeginArea(new Rect(position.width - 300, 0, 300, position.height), EditorStyles.helpBox);
            
            EditorGUILayout.LabelField("Selected Node", EditorStyles.boldLabel);
            
            selectedPhrase.id = EditorGUILayout.TextField("ID", selectedPhrase.id);
            selectedPhrase.characterName = EditorGUILayout.TextField("Character Name", selectedPhrase.characterName);
            selectedPhrase.characterIcon = (Sprite)EditorGUILayout.ObjectField("Character Icon", selectedPhrase.characterIcon, typeof(Sprite), false);
            selectedPhrase.text = EditorGUILayout.TextArea(selectedPhrase.text, GUILayout.Height(100));
            
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Choices", EditorStyles.boldLabel);
            
            for (int i = 0; i < selectedPhrase.choices.Count; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
    
                selectedPhrase.choices[i].text = EditorGUILayout.TextField("Choice Text", selectedPhrase.choices[i].text);
                selectedPhrase.choices[i].condition = EditorGUILayout.TextField("Condition", selectedPhrase.choices[i].condition);
    
                var nextPhrase = database.GetPhraseById(selectedPhrase.choices[i].nextPhraseId);
                var newNextPhrase = (DialoguePhrase)EditorGUILayout.ObjectField(
                    "Next Phrase", 
                    nextPhrase, 
                    typeof(DialoguePhrase), 
                    false);
    
                if (newNextPhrase != nextPhrase)
                {
                    selectedPhrase.choices[i].nextPhraseId = newNextPhrase?.id;
                }
    
                if (GUILayout.Button("Remove Choice"))
                {
                    selectedPhrase.choices.RemoveAt(i);
                    break;
                }
    
                EditorGUILayout.EndVertical();
            }
            
            if (GUILayout.Button("Add Choice"))
            {
                selectedPhrase.choices.Add(new DialogueChoice());
            }
            
            if (GUILayout.Button("Delete Node"))
            {
                if (EditorUtility.DisplayDialog("Delete Node", "Are you sure you want to delete this node?", "Yes", "No"))
                {
                    database.phrases.Remove(selectedPhrase);
                    selectedPhrase = null;
                }
            }
            
            GUILayout.EndArea();
        }

        private void ShowNodeContextMenu(DialoguePhrase phrase)
        {
            var menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Make Entry Point"), false, () => {
                var entryPoint = new DialogueEntryPoint
                {
                    id = Guid.NewGuid().ToString(),
                    startPhraseId = phrase.id,
                    condition = "always"
                };
                database.entryPoints.Add(entryPoint);
            });
            
            menu.AddItem(new GUIContent("Link to New Phrase"), false, () => {
                var newPhrase = new DialoguePhrase
                {
                    id = Guid.NewGuid().ToString(),
                    characterName = phrase.characterName,
                    text = "New dialogue text",
                    graphPosition = phrase.graphPosition + new Vector2(300, 0)
                };
                database.phrases.Add(newPhrase);
                
                phrase.choices.Add(new DialogueChoice
                {
                    text = "Choice",
                    nextPhraseId = newPhrase.id
                });
                
                selectedPhrase = phrase;
            });
            
            menu.AddItem(new GUIContent("Delete Node"), false, () => {
                if (EditorUtility.DisplayDialog("Delete Node", "Are you sure you want to delete this node?", "Yes", "No"))
                {
                    database.phrases.Remove(phrase);
                    if (selectedPhrase == phrase) selectedPhrase = null;
                }
            });
            
            menu.ShowAsContext();
        }

        private void ShowEntryPointContextMenu(DialogueEntryPoint entryPoint)
        {
            var menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Set Start Phrase"), false, () => {
                newLinkFromId = entryPoint.id;
            });
            
            menu.AddItem(new GUIContent("Delete Entry Point"), false, () => {
                if (EditorUtility.DisplayDialog("Delete Entry Point", "Are you sure you want to delete this entry point?", "Yes", "No"))
                {
                    database.entryPoints.Remove(entryPoint);
                }
            });
            
            menu.ShowAsContext();
        }
    }
    #endif
}