using Cysharp.Threading.Tasks;
using TMPEffects.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Runtime.Services
{
    public class DialogController : MonoBehaviour, IService, IInitializable
    {
        [SerializeField] private TextMeshProUGUI _dialogText;
        [SerializeField] private TMPWriter _dialogWriter;

        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private Image _dialogPanel;
        [SerializeField] private Image _iconImage;

        public Image IconImage => _iconImage; 
        public Image DialogPanel => _dialogPanel; 
        public TextMeshProUGUI Name => _name; 
        public TMPWriter DialogWriter => _dialogWriter; 
        public TextMeshProUGUI DialogText => _dialogText;

        public void Initialize()
        {
            gameObject.SetActive(false);
            _iconImage.gameObject.SetActive(false);
        }

        public async UniTask PrintText(string text)
        {
            DialogText.text = text;
            DialogWriter.StartWriter();

            bool onTextPrinted = false;

            DialogWriter.OnFinishWriter.AddListener( x => onTextPrinted = true);

            await UniTask.WaitUntil(() => onTextPrinted);
        }
    }
}
