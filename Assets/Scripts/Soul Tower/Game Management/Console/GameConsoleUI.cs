using Shears.Input;
using TMPro;
using UnityEngine;

namespace SoulTower.GameManagement
{
    public class GameConsoleUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private TextMeshProUGUI consoleText;
        [SerializeField] private TextMeshProUGUI inputText;

        private void OnEnable()
        {
            GameConsole.Enabled += Enable;
            GameConsole.Disabled += Disable;
            GameConsole.InputTextChanged += UpdateInputText;
            GameConsole.ConsoleTextChanged += UpdateConsoleText;
        }

        private void OnDisable()
        {
            GameConsole.Enabled -= Enable;
            GameConsole.Disabled -= Disable;
            GameConsole.InputTextChanged -= UpdateInputText;
            GameConsole.ConsoleTextChanged -= UpdateConsoleText;
        }

        private void Enable()
        {
            canvas.enabled = true;
        }

        private void Disable()
        {
            canvas.enabled = false;
        }

        private void UpdateInputText(string text)
        {
            inputText.text = text;
        }

        private void UpdateConsoleText(string text)
        {
            consoleText.text += text + "\n";
        }
    }
}
