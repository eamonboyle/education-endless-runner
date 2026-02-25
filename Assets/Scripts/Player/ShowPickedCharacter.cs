using UnityEngine;
using UnityEngine.UI;

public class ShowPickedCharacter : MonoBehaviour
{
    public GameObject boy;
    public GameObject girl;
    public GameObject modeButton;

    private void Start()
    {
        if (GameState.IsFirstLoad())
        {
            if (GameManager.instance != null)
                GameManager.instance.LoadCharacterSelection();
            return;
        }

        string player = GameState.GetCharacter();

        if (boy != null) boy.SetActive(player != "girl");
        if (girl != null) girl.SetActive(player == "girl");

        string questionType = GameState.GetQuestionType();
        string symbol = GetModeSymbol(questionType);

        if (modeButton != null)
        {
            var text = modeButton.GetComponent<Text>();
            if (text != null)
                text.text = "Mode [" + symbol + "]";
        }
    }

    private string GetModeSymbol(string questionType)
    {
        switch (questionType)
        {
            case "addition": return "+";
            case "subtraction": return "-";
            case "multiply": return "x";
            case "division": return "÷";
            case "mixed": return "?";
            default: return "+";
        }
    }
}
