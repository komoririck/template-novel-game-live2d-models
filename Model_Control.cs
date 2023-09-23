using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.Expression;
using System.Linq;
using System.IO;
using Live2D.Cubism.Framework.Json;
using TMPro;


[System.Serializable]
public class Model_Control : MonoBehaviour {

    CubismModel live2dModel;
    [SerializeField] public Expression[] Expressions;
    [SerializeField] private Vector2[] speakingVowels = new Vector2[5];
    private CubismParameter mouthOpen;
    private CubismParameter mouthForm;
    private char lastChar = 'z';
    private int currentTextLenght = 0;
    [SerializeField] private float speakingSpeed = 12f;
    float t;
    private float transitionTime = 0.001f;
    private float elapsedTime;

    [HideInInspector] public AnimationCurve mouthOpenCurve;
    [HideInInspector] public AnimationCurve mouthFormCurve;
    Vector2 startValues;
    Vector2 targetValues;
    TMP_Text textLabel;

    void Awake(){
        textLabel = GameObject.Find("GameControler").GetComponent<DialogueUI>().textLabel;
        addVtubeStudioExpression();
        live2dModel = this.FindCubismModel();
        if (live2dModel != null) {
            //those are the default ids for live2d, if you change there, change here
            mouthOpen = live2dModel.Parameters.FindById("ParamMouthOpenY");
            mouthForm = live2dModel.Parameters.FindById("ParamMouthForm");
        }
        startValues = new Vector2(0, mouthForm.Value);
    }

    void LateUpdate() {
        if (live2dModel != null){
            Speaking();
        }
    }

    void Speaking() {
        if (textLabel.isActiveAndEnabled && !string.IsNullOrEmpty(textLabel.text)) {
            char currentChar = textLabel.text[textLabel.text.Length - 1];
            if (currentChar != lastChar) {
                lastChar = currentChar;
                switch (currentChar) {
                    case 'a': targetValues = speakingVowels[0]; break;
                    case 'i': targetValues = speakingVowels[1]; break;
                    case 'u': targetValues = speakingVowels[2]; break;
                    case 'e': targetValues = speakingVowels[3]; break;
                    case 'o': targetValues = speakingVowels[4]; break;
                    default: targetValues = startValues; break;
                }
                elapsedTime = 0f;

            }

            if (elapsedTime < transitionTime) {
                float t = elapsedTime / transitionTime;
                t = Mathf.SmoothStep(0f, 1.4f, t);
                mouthOpen.Value = Mathf.Lerp(0, targetValues.x, t);
                mouthForm.Value = Mathf.Lerp(0, targetValues.y, t);
                elapsedTime += Time.deltaTime * speakingSpeed;
            } else {
                mouthOpen.Value = targetValues.y;
                mouthForm.Value = targetValues.x;
            }
        }
        mouthOpen.Value = 0;
    }

    public void SetExpression(int n) {
        live2dModel.GetComponent<CubismExpressionController>().CurrentExpressionIndex = n;
    }

    //ALTERADO CORE AQUI - Change in the SDK CORE
    //Live2d SDK already assign the expression list to the models when u import, but i made this to be sure to add the expressions list, also because i wish to add a way to add expressions directly from the unity panel
    //O SDK já add as expressoes quando importa o modelo, mas adicionei isso para ter certeza de add a lista no modelo e também pq eu queria adicionar expressoes direito do painel
    //Live2D.Cubism.Framework.Expression.CubismExpressionData
    public void addVtubeStudioExpression(){
        string path = "Assets/Models/" + gameObject.name;
        string[] files = Directory.GetFiles(path.Substring(0, path.Length - 7), "*.exp3.json");
        List<CubismExpressionData> listExp = new List<CubismExpressionData>();

        listExp = new List<CubismExpressionData>();

        foreach (string file in files){
            CubismExpressionData expressionData = CubismExpressionData.CreateInstance(CubismExp3Json.LoadFrom(File.ReadAllText(file)));
            expressionData.FadeOutTime = 1;
            expressionData.FadeInTime = 1;
            listExp.Add(expressionData);
        }

        for (int i = 0; i < Expressions.Length; i++){
            CubismExpressionData item = CubismExpressionData.CreateInstance("Live2D Expression", 1, 1, Expressions[i].Parameters.ToArray());
            listExp.Add(item);
        }


        CubismExpressionList expressionList = ScriptableObject.CreateInstance<CubismExpressionList>();
        if (GetComponent<CubismExpressionController>().ExpressionsList == null) { 
            expressionList.CubismExpressionObjects = listExp.ToArray();
            GetComponent<CubismExpressionController>().ExpressionsList = expressionList;
        } else {
            GetComponent<CubismExpressionController>().ExpressionsList.CubismExpressionObjects.Concat(listExp.ToArray());
        }
    }
}

[System.Serializable]
public class Parameter {
    [SerializeField] public string parameterID;
    [SerializeField] public float parameterValue;
    [SerializeField] public string Blend;
}

[System.Serializable]
public class Expression {
    [SerializeField] public string expressionName;
    [SerializeField] private List<Parameter> parameters = new List<Parameter>();

    public List<Parameter> Parameters => parameters;
}
