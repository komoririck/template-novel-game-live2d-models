using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameRote {
    [SerializeField] public string name;
    [SerializeField] public int score;
}

public class History_Controller : MonoBehaviour {

    [SerializeField] private DialogueObject[] DialogueList;
    [SerializeField] private int HistoryHigh = 0;
    [SerializeField] public List<GameRote> gameRotes;

    void Start() {
        GetComponent<DialogueUI>().StartDialogue(DialogueList[0]);
    }

    public void AdvanceHistory() {
        HistoryHigh++;
    }

    public void IncreaseRoteScore(string name, int value) { 
        foreach (GameRote g in gameRotes){
            if(g.name == name){
                g.score += value;
                return;
            }
        }
        GameRote newRote = new GameRote();
        newRote.name = name;
        newRote.score = value;
        gameRotes.Add(newRote);
    }
}