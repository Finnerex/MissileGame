using Hangar;
using Missiles.Components;
using UnityEngine;

namespace Missiles
{
    public class TestAddButton : MonoBehaviour
    {
        [SerializeField] private MissileComponent component;
        [SerializeField] private MissileWorkbench wb;

        public void Add()
        {
            Debug.Log("clickied");
            wb.AddMissileComponent(component);
        }


    }
}