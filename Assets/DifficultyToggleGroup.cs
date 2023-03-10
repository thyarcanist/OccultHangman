using UnityEngine;
using UnityEngine.UI;

public class DifficultyToggleGroup : MonoBehaviour
{
    public Toggle easyToggle;
    public Toggle normalToggle;
    public Toggle hardToggle;
    public GameManager gameManager;
    public SessionDifficulty difficulty = SessionDifficulty.Normal;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        easyToggle.onValueChanged.AddListener(OnEasyToggle);
        normalToggle.onValueChanged.AddListener(OnNormalToggle);
        hardToggle.onValueChanged.AddListener(OnHardToggle);
    }

    public void OnEasyToggle(bool isOn)
    {
        if (isOn)
        {
            gameManager.difficulty =  SessionDifficulty.Easy;
            normalToggle.isOn = false;
            hardToggle.isOn = false;
        }
    }

    public void OnNormalToggle(bool isOn)
    {
        if (isOn)
        {
            gameManager.difficulty = SessionDifficulty.Normal;
            easyToggle.isOn = false;
            hardToggle.isOn = false;
        }
    }

    public void OnHardToggle(bool isOn)
    {
        if (isOn)
        {
            gameManager.difficulty = SessionDifficulty.Hard;
            easyToggle.isOn = false;
            normalToggle.isOn = false;
        }
    }
}