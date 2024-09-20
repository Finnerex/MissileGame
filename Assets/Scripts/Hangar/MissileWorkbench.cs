using Missiles;
using Missiles.Components;
using UnityEngine;

namespace Hangar
{
    public class MissileWorkbench : HangarInteractable
    {
        public MissilePreset preset; // blueprint?

        [SerializeField] private Transform missileParent;

        private BodyComponent _body;
        private int _remainingCapacity;

        // public TechTreeNode currentResearchNode;

        // i dont know if i should use instance or getcomponent
        // my heart says static is bad but my brain says getcomponent is slow
        public static MissileWorkbench Instance;

        private void Awake() { Instance = this; }

        public bool AddMissileComponent(MissileComponent component)
        {

            if (component is BodyComponent body)
            {
                ReplaceComponent(ref _body, body);
                
                _remainingCapacity = body.size;

                // still a little ugly
                ReplaceComponent(ref preset.seeker, null);
                ReplaceComponent(ref preset.computer, null);
                ReplaceComponent(ref preset.warhead, null);
                ReplaceComponent(ref preset.booster, null);
                ReplaceComponent(ref preset.avionics, null);

                return true;
            }

            if (component.size > _remainingCapacity)
                return false; // cannot be added

            _remainingCapacity -= component.size;

            switch (component)
            {
                case SeekerComponent seeker:
                    ReplaceComponent(ref preset.seeker, seeker);
                    break;
                case ComputerComponent guidanceComputer:
                    ReplaceComponent(ref preset.computer, guidanceComputer);
                    break;
                // _missile.Computer = guidanceComputer;
                case WarheadComponent warhead:
                    ReplaceComponent(ref preset.warhead, warhead);
                    break;
                case BoosterComponent booster:
                    ReplaceComponent(ref preset.booster, booster);
                    break;
                case AvionicsComponent avionics:
                    ReplaceComponent(ref preset.avionics, avionics);
                    break;
            }


            ResetPositions();
            return true;
        }

        // this is cool asf b/c you normally cant do ref polymorphism
        private void ReplaceComponent<T>(ref T oldComponent, T newComponent) 
        where T : MissileComponent
        {
            if (oldComponent is not null)
                Destroy(oldComponent.SpawnedObject); // despawn the old object

            if (newComponent is not null) // not simply deleted
                newComponent.SpawnedObject = Instantiate(newComponent.prefab, missileParent); // spawn the new one, will be moved
            
            oldComponent = newComponent;
        }

        private void ResetPositions()
        {
            int nextItemPosition = 0;

            nextItemPosition = SetPosition(preset.seeker, nextItemPosition);
            nextItemPosition = SetPosition(preset.warhead, nextItemPosition);
            nextItemPosition = SetPosition(preset.computer, nextItemPosition);
            nextItemPosition = SetPosition(preset.avionics, nextItemPosition);

            SetPosition(preset.booster, nextItemPosition);

        }

        private int SetPosition(MissileComponent component, int nextItemPosition)
        {
            if (component is null)
                return nextItemPosition + 1;

            component.SpawnedObject.transform.position = _body.SpawnedObject.transform.position -
                                                         _body.SpawnedObject.transform.forward *
                                                         nextItemPosition /* with a transformation */;
            return nextItemPosition + component.size;
        }

        public void Save(/*string name?*/)
        {
            if (preset.seeker != null && preset.computer != null &&
                 preset.warhead != null && preset.booster != null &&
                 preset.avionics != null && _body != null)
            {
                ; // do something probably
            }
        }

        public void OnInteract()
        {
            
        }
    }
}