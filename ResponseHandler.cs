using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResponseHandler : MonoBehaviour {

    [SerializeField] RectTransform responseBox;
    [SerializeField] RectTransform responseButtonTemplate;
    [SerializeField] RectTransform responseContainer;

    DialogueUI dialogueUI;

    List<GameObject> tempResponseButtons = new List<GameObject>();

    private void Start(){
        dialogueUI = GetComponent<DialogueUI>();
    }

    public void ShowResponses(Response[] responses){
        float responseBoxHeight = 0;

        foreach (Response response in responses) {
            GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseButton.gameObject.SetActive(true);
            responseButton.GetComponent<TMP_Text>().text = response.ResponseText;
            responseButton.GetComponent<Button>().onClick.AddListener(() => OnPicketResponse(response));

            responseBoxHeight += responseButtonTemplate.sizeDelta.y;

            tempResponseButtons.Add(responseButton);

        }
        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }

    void OnPicketResponse(Response response) {
        responseBox.gameObject.SetActive(false);
        foreach (GameObject button in tempResponseButtons){
            Destroy(button);
        }
        dialogueUI.StartDialogue(response.DialogueObject);
    }

}
