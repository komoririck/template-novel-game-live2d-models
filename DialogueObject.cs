using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

[System.Serializable]
public class Response{
    [SerializeField] string responseText;
    [SerializeField] DialogueObject dialogueObject;

    public string ResponseText => responseText;
    public DialogueObject DialogueObject => dialogueObject;
}


[System.Serializable]
public class Event
{
    [SerializeField] public string name;
    [SerializeField] public string type;
    [SerializeField] public float duration;
    [SerializeField] public string animation;
    [SerializeField] public Color color;
    [SerializeField] public float magnitude;
    [SerializeField] public int expression;
    [SerializeField] public Vector3 newPos;
    public Event() { }
}

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{

    [SerializeField] GameObject owner;
    [SerializeField] Vector3 startPos;
    [SerializeField][TextArea] private string[] dialogue;
    [SerializeField] Response[] responses;
    [SerializeField] DialogueObject nextDialogue;
    [SerializeField] Event[] events;
    [SerializeField] bool destroyAtEnd;
    [SerializeField] GameRote addScore;




    public GameObject Owner => owner;
    public Vector3 StartPos => startPos;
    public bool DestroyAtEnd => destroyAtEnd;
    public string[] Dialogue => dialogue;
    public Response[] Responses => responses;
    public DialogueObject NextDialogue => nextDialogue;
    public Event[] Events => events;
    public GameRote AddScore => addScore;
}



//INSPECTOR TAB - ADD EVENTS HERE
[CustomEditor(typeof(DialogueObject))]
public class DialogueObjectEditor : Editor
{
    private SerializedProperty ownerProp;
    private SerializedProperty startPosProp;
    private SerializedProperty dialogueProp;
    private SerializedProperty responsesProp;
    private SerializedProperty nextDialogueProp;
    private SerializedProperty eventsProp;
    private SerializedProperty destroyAtEndProp;
    private SerializedProperty addScoreProp;

    private string[] eventOptions = new string[] { "None", "Paint", "Shake Horizontal", "Shake Vertical", "Shake Pendulum", "Play Animation", "Wait", "Expression", "Move Character" };

    private void OnEnable()
    {
        ownerProp = serializedObject.FindProperty("owner");
        startPosProp = serializedObject.FindProperty("startPos");
        dialogueProp = serializedObject.FindProperty("dialogue");
        responsesProp = serializedObject.FindProperty("responses");
        nextDialogueProp = serializedObject.FindProperty("nextDialogue");
        eventsProp = serializedObject.FindProperty("events");
        destroyAtEndProp = serializedObject.FindProperty("destroyAtEnd");
        addScoreProp = serializedObject.FindProperty("addScore");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(ownerProp);
        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(startPosProp);
        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(dialogueProp);
        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(responsesProp);
        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(nextDialogueProp);
        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(destroyAtEndProp);
        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(addScoreProp);
        EditorGUILayout.Separator();

        for (int i = 0; i < eventsProp.arraySize; i++)
        {
            SerializedProperty eventProp = eventsProp.GetArrayElementAtIndex(i);
            SerializedProperty nameProp = eventProp.FindPropertyRelative("name");
            SerializedProperty typeProp = eventProp.FindPropertyRelative("type");
            SerializedProperty durationProp = eventProp.FindPropertyRelative("duration");
            SerializedProperty animationProp = eventProp.FindPropertyRelative("animation");
            SerializedProperty colorProp = eventProp.FindPropertyRelative("color");
            SerializedProperty magnitudeProp = eventProp.FindPropertyRelative("magnitude");
            SerializedProperty expressionProp = eventProp.FindPropertyRelative("expression");
            SerializedProperty positionListProp = eventProp.FindPropertyRelative("newPos");

            int selectedEventIndex = GetSelectedEventIndex(i);
            selectedEventIndex = EditorGUILayout.Popup("Event " + i + ":", selectedEventIndex, eventOptions);
            if (selectedEventIndex == 0){
                nameProp.stringValue = "";
            } else {
                nameProp.stringValue = eventOptions[selectedEventIndex];
            }

            string[] typeOptions = { "after", "before" };
            int selectedTypeIndex = Array.IndexOf(typeOptions, typeProp.stringValue);
            if (selectedTypeIndex == -1)
            {
                selectedTypeIndex = 0;
            }
            selectedTypeIndex = GUILayout.Toolbar(selectedTypeIndex, typeOptions);
            if (selectedTypeIndex == 0)
            {
                typeProp.stringValue = "after";
            }
            else if (selectedTypeIndex == 1)
            {
                typeProp.stringValue = "before";
            }




            switch (nameProp.stringValue)
            {
                case "Paint":
                    colorProp.colorValue = EditorGUILayout.ColorField("color", colorProp.colorValue);
                    break;
                case "Shake Horizontal":
                case "Shake Vertical":
                case "Shake Pendulum":
                    durationProp.floatValue = EditorGUILayout.FloatField("duration", durationProp.floatValue);
                    magnitudeProp.floatValue = EditorGUILayout.FloatField("magnitude", magnitudeProp.floatValue);
                    break;
                case "Play Animation":
                    animationProp.stringValue = EditorGUILayout.TextField("animation", animationProp.stringValue);
                    break;
                case "Wait":
                    durationProp.floatValue = EditorGUILayout.FloatField("duration", durationProp.floatValue);
                    break;
                case "Expression":
                    expressionProp.intValue = EditorGUILayout.IntField("expression", expressionProp.intValue);
                    break;
                case "Move Character":
                    positionListProp.vector3Value = EditorGUILayout.Vector3Field("Position", positionListProp.vector3Value);
                    break;
            }
        EditorGUILayout.Separator();
        }

        EditorGUILayout.Separator();
        EditorGUILayout.PropertyField(eventsProp, true);

        serializedObject.ApplyModifiedProperties();
    }

    private int GetSelectedEventIndex(int index)
    {
        string currentValue = eventsProp.GetArrayElementAtIndex(index).FindPropertyRelative("name").stringValue;
        for (int i = 0; i < eventOptions.Length; i++)
        {
            if (currentValue == eventOptions[i])
            {
                return i;
            }
        }
        return 0;
    }


    private int GetSelectedEventIndex()
    {
        for (int i = 0; i < eventOptions.Length; i++)
        {
            if (eventsProp.arraySize > i && eventsProp.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == eventOptions[i + 1])
            {
                return i + 1;
            }
        }

        return 0;
    }
}
