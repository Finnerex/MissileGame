using System;
using Utility;

namespace Missiles
{
    public enum ComponentItem
    {
        // computer, seeker
        BasicCircuitry,
        AdvancedCircuitry,
        
        // booster, body, avionics
        MetalPlating,
        AerodynamicCasing,
        
        // avionics, seeker
        BasicActuators,
        AdvancedActuators,
        
        // warhead, booster
        Combustible,
        
        // computer, seeker, warhead
        BasicSensors,
        AdvancedSensors,
        
    }

    [Serializable]
    public struct ComponentCost
    {
        public ComponentCost(ComponentItem item, int amount)
        {
            componentItem = item;
            this.amount = amount;
        }
        
        public ComponentItem componentItem;
        public int amount;

        public override string ToString()
        {
            return $"{Util.GetNiceName(componentItem.ToString())} - {amount}\n";
        }
    }
    
}