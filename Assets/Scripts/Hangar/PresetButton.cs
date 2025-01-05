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
            
            // likely should differentiate between the 2 uis this is used in

            HangarController.Instance.CurrentlySelectedPreset = MissileWorkbench.Instance.SavedPresets[text.text];
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