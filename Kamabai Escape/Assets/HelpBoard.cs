using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelpBoard : MonoBehaviour
{
    public TutorialCard helpData;
    public GameObject helpUI;
    public GameObject promptUI;  // The UI that prompts the player to press the Help button
    public TextMeshProUGUI helpText;
    [SerializeField] private Sprite promptKeyboardSprite;
    [SerializeField] private Sprite promptControllerSprite;
    public Image helpImage;
    public bool isPlayerNear = false;
    private PlayerInputHandler player => FindAnyObjectByType<PlayerInputHandler>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Set isPlayerNear to true and show the prompt UI when the player is close to the board
            isPlayerNear = true;
            if(player.playerInput.currentControlScheme == "Keyboard")
            {
                promptUI.GetComponentInChildren<Image>().sprite = promptKeyboardSprite;
            }
            else
            {
                promptUI.GetComponentInChildren<Image>().sprite = promptKeyboardSprite;
            }
            promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Set isPlayerNear to false and hide the help and prompt UIs when the player moves away from the board
            isPlayerNear = false;
            //helpUI.SetActive(false);
            promptUI.SetActive(false);
        }
    }

    public void OnHelp()
    {

        // Toggle the help UI and hide the prompt UI when the player presses the Help button while near the board
        helpUI.SetActive(!helpUI.activeSelf);
        promptUI.SetActive(!helpUI.activeSelf);
        if (helpUI.activeSelf)
        {
            player.SwitchActionMapToHelp();
            
            helpText.text = helpData.description;
            helpImage.sprite = helpData.artwork;
        }
    }

    public void CloseHelp()
    {
        player.SwitchActionToGameplay();
        helpUI?.SetActive(false);
    }

}