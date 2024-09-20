using System;
using Missiles.Components;

namespace Missiles
{
    [Serializable]
    public struct MissilePreset
    {
        public SeekerComponent seeker;
        public ComputerComponent computer;
        public WarheadComponent warhead;
        public BoosterComponent booster;
        public AvionicsComponent avionics;
    }
}