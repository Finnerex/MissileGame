using System;
using System.Collections.Generic;
using System.Linq;
using Missiles.Components;

namespace Missiles
{
    [Serializable]
    public struct MissilePreset
    {
        public string name;
        
        public SeekerComponent seeker;
        public ComputerComponent computer;
        public WarheadComponent warhead;
        public BoosterComponent booster;
        public AvionicsComponent avionics;
        public BodyComponent body;
        
        public bool isValid => seeker != null && computer != null &&
                               warhead != null && booster != null &&
                               avionics != null && body != null;

        public ComponentCost[] GetCost()
        {
            // this might be the worst code ill write in this project
            // so many things would have been easier if the components were in an array
            // but also could have been worse
            Dictionary<ComponentItem, int> costs = new Dictionary<ComponentItem, int>();
            MissileComponent[] components = { seeker, computer, warhead, booster, avionics, body };

            foreach (MissileComponent component in components)
            {
                if (component is null) continue;
                foreach (ComponentCost cost in component.cost)
                    if (!costs.TryAdd(cost.componentItem, cost.amount))
                        costs[cost.componentItem] += cost.amount;
            }

            ComponentCost[] output = new ComponentCost[costs.Count];
            int i = 0;
            foreach (var cost in costs)
                output[i++] = new ComponentCost(cost.Key, cost.Value);

            return output;
        }
    }
}