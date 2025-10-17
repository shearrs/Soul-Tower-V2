using Shears;
using Shears.Logging;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SoulTower.GameManagement
{
    public class GameConsoleUI : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private TextMeshProUGUI consoleText;
        [SerializeField] private TextMeshProUGUI inputText;
        [SerializeField] private ResizableScrollRect scrollRect;

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

            StopAllCoroutines();
            StartCoroutine(IEScrollToBottom());
        }

        private IEnumerator IEScrollToBottom()
        {
            yield return CoroutineUtil.WaitForEndOfFrame;

            scrollRect.verticalNormalizedPosition = 0;
        }
    }
}
