using TMPro;
using UnityEngine;

namespace Hangar
{
    public class PresetButton : MonoBehaviour
    {

        [SerializeField] private TMP_Text text;
        
        public void Load()
        {
            MissileWorkbench.Instance.Load(text.text);
        }

        public void SetText(string newText)
        {
            text.text = newText;
        }

        public void Delete()
        {
            MissileWorkbench.Instance.SavedPresets.Remove(text.text);
            Destroy(gameObject);
        }
        
    }
}