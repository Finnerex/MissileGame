using System;
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
    }
}