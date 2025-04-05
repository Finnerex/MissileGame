using System.Linq;
using UnityEngine;
using Utility;

namespace Missiles.Components
{
    public abstract class MissileComponent : ScriptableObject
    {
        public string displayName;

        // cost will be deducted from inventory at launch time and added back for remaining missiles
        // Or, will be deducted when the missile is added to aircraft (i think this is better)
        public ComponentCost[] cost;


        public int size; // small ass class !!!!! :(

        public string CostString()
        {
            return cost.Aggregate("Cost: \n", (current, c) => current + "  " + c);
        }
        
        public override string ToString()
        {
            return $"Size: {size}u";
        }
    }
}