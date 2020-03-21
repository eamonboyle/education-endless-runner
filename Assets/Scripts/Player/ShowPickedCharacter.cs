using UnityEngine;
using UnityEngine.UI;

public class ShowPickedCharacter : MonoBehaviour
{
    public GameObject boy;
    public GameObject girl;
    public GameObject modeButton;

    // Start is called before the first frame update
    private void Start()
    {
        if (GameState.IsFirstLoad())
        {
            GameManager.instance.LoadCharacterSelection();
        }

        string player = GameState.GetCharacter();

        if (player == "girl")
        {
            girl.SetActive(true);
            boy.SetActive(false);
        }
        else
        {
            girl.SetActive(false);
            boy.SetActive(true);
        }

        // change the mode button
        string questionType = GameState.GetQuestionType();
        string modeText = "Mode [";

        switch (questionType)
        {
            case "addition":
                modeText += "+";
                break;

            case "subtraction":
                modeText += "-";
                break;

            case "multiply":
                modeText += "x";
                break;

            case "division":
                modeText += "÷";
                break;

            default:
                modeText += "+";
                break;
        }

        modeText += "]";

        modeButton.GetComponent<Text>().text = modeText;
    }
}