using System.Collections;
using UnityEngine;
using TMPro;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Rendering;

public class DialogueUI : MonoBehaviour{

    //Dialogue
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] public TMP_Text textLabel;
    private DialogueObject currentDialogue;

    //Live2d - Character
    [HideInInspector] public GameObject currentCharacter;
    private Animator animator;
    private Model_Control model_Control;

    //Dialogue - Response
    private TypewriterEffect typewriterEffect;
    private ResponseHandler responseHandler;

    //Live2d - Events
    private Vector3 originalPosition;

    //read the list of events and run them in the add sequence
    IEnumerator StepThroughEvents(){
        if (currentDialogue.Events != null)
            for (int i = 0; i < currentDialogue.Events.Length; i++){
            if (currentDialogue.Events[i].type == "before")
                yield return StartCoroutine(DisplayEvent(currentDialogue.Events[i].name, currentDialogue.Events[i].duration, currentDialogue.Events[i].animation, currentDialogue.Events[i].color, currentDialogue.Events[i].magnitude, currentDialogue.Events[i].expression, currentDialogue.Events[i].newPos));
            }
        yield return StartCoroutine(stepThroughDialogue(currentDialogue));
        if (currentDialogue.Events != null)
            for (int i = 0; i < currentDialogue.Events.Length; i++){
            if (currentDialogue.Events[i].type == "after")
                yield return StartCoroutine(DisplayEvent(currentDialogue.Events[i].name, currentDialogue.Events[i].duration, currentDialogue.Events[i].animation, currentDialogue.Events[i].color, currentDialogue.Events[i].magnitude, currentDialogue.Events[i].expression, currentDialogue.Events[i].newPos));
            }
    }
    //this is the enumerator to call each event, this way each event will run til the end before the code call the next one
    //this is the place to add new events, along with adding them to the DIALOGUEOBJECTS script, to be able to add them using inspector
    IEnumerator DisplayEvent(string name, float duration = 0f, string animation = "", Color color = new Color(), float magnitude = 0f, int expression = -1, Vector3 vec = new Vector3()) {
        switch (name){
            case "paint":
                GameObject drawables = GameObject.Find("Drawables");
                if (drawables != null)
                    foreach (Transform child in drawables.transform)
                        child.GetComponent<CubismRenderer>().Color = color;
                break;
            case "shakeHorizontal":
                yield return StartCoroutine(shakeHorizontally(currentCharacter, magnitude, duration));
                break;
            case "shakeVertical":
                yield return StartCoroutine(shakeVertically(currentCharacter, magnitude, duration));
                break;
            case "shakePendulum":
                yield return StartCoroutine(shakeInPendulum(currentCharacter, magnitude, duration));
                break;
            case "playAnimation":
                yield return StartCoroutine(playAnimation(animation));
                break;
            case "wait":
                yield return StartCoroutine(waitEvent(duration));
                break;
            case "expression":
                model_Control.SetExpression(expression);
                break;
            case "moveCharacter":
                rePosCharacter(vec);
                break;
        }
    }



    // EVENTS -
    IEnumerator waitEvent(float duration){
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator shakeVertically(GameObject gameObject, float magnitude, float duration){
        Vector3 originalPosition = gameObject.transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration){
            float y = originalPosition.y + Random.Range(-1f, 1f) * magnitude;
            gameObject.transform.position = new Vector3(originalPosition.x, y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.position = originalPosition;
    }

    IEnumerator shakeHorizontally(GameObject gameObject, float magnitude, float duration){
        Vector3 originalPosition = gameObject.transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration){
            float x = originalPosition.x + Random.Range(-1f, 1f) * magnitude;
            gameObject.transform.position = new Vector3(x, originalPosition.y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.position = originalPosition;
    }

    IEnumerator shakeInPendulum(GameObject gameObject, float magnitude, float duration){
        Vector3 originalPosition = gameObject.transform.position;
        float elapsed = 0.0f;

        while (elapsed < duration){
            float angle = Mathf.Sin(elapsed / duration * Mathf.PI) * 2 * Mathf.PI;
            float x = originalPosition.x + Mathf.Sin(angle) * magnitude;
            float y = originalPosition.y - Mathf.Cos(angle) * magnitude;
            gameObject.transform.position = new Vector3(x, y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.position = originalPosition;
    }

    IEnumerator playAnimation(string playAnimation){
        animator.Play(playAnimation);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
    }

    public void rePosCharacter(Vector3 vec){
        currentCharacter.transform.position = vec;
    }
    // EVENTS -

    //we are instantianting the character here, if you don't want to do this way, just change to the same way the SDK exemplify
    //your prefab should have the default stuff, like Model_Control, blinking and presaved expressions
    void spawnCharacter(GameObject owner){
        if (currentCharacter == null)
            currentCharacter = Instantiate(owner, GameObject.Find("CharacterList").transform);
        rePosCharacter(currentDialogue.StartPos);
        animator = currentCharacter.GetComponent<Animator>();
    }

    //Just close the Dialogue box when there's no next dialogue
    public void closeDialogueBox() {
        dialogueBox.SetActive(false);
        textLabel.text = string.Empty;
    }

    //Destroy all characters on the screen in the end of Dialogues - all characters need to be loaded as characterList childs
    public void destroyAllScreenCharacters(){
        GameObject parent = GameObject.Find("CharacterList");
        foreach (Transform child in parent.transform)
            Destroy(child.gameObject);
    }

    //Start the Dialogue
        //spawn the character asign to the dialogue
    public void StartDialogue(DialogueObject Dialogue){
        currentDialogue = Dialogue;
        spawnCharacter(currentDialogue.Owner);
        responseHandler = GetComponent<ResponseHandler>();
        typewriterEffect = GetComponent<TypewriterEffect>();
        model_Control = currentCharacter.GetComponent<Model_Control>();
        closeDialogueBox();
        if(Dialogue.AddScore != null)
            this.GetComponent<History_Controller>().IncreaseRoteScore(Dialogue.AddScore.name, Dialogue.AddScore.score);
        StartCoroutine(StepThroughEvents());
    }

    private IEnumerator stepThroughDialogue(DialogueObject dialogueObject){
        dialogueBox.SetActive(true);
        for (int i = 0; i < dialogueObject.Dialogue.Length; i++) {
            string dialogue = dialogueObject.Dialogue[i];
            yield return typewriterEffect.Run(dialogue, textLabel);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            if (i == dialogueObject.Dialogue.Length - 1 && dialogueObject.Responses != null && dialogueObject.Responses.Length > 0)
                break;
        }
        if (dialogueObject.Responses != null && dialogueObject.Responses.Length > 0){
            responseHandler.ShowResponses(dialogueObject.Responses);
        } else if (dialogueObject.NextDialogue != null && dialogueObject.NextDialogue.Dialogue.Length > 0){
            if (dialogueObject.DestroyAtEnd){
                Destroy(currentCharacter);
                currentCharacter = null;
            }
            currentDialogue = currentDialogue.NextDialogue;
            StartDialogue(currentDialogue);
        } else {
            destroyAllScreenCharacters();
            closeDialogueBox();
            this.GetComponent<History_Controller>().AdvanceHistory();
        }
    }


}