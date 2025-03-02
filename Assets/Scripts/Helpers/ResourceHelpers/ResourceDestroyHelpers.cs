using Components;
using Components.Health;
using Leopotam.Ecs;
using ServerCommunication;

namespace Systems
{
    public static class ResourceDestroyHelpers
    {
        public static void CreateDestroyEvent(EcsEntity resourceEntity)
        {
            resourceEntity.Set<EntityDestroyedSoonComponent>();
            ServerClient.Communication.ClientInfoReceiver.ToServerRemoveBag.Add(resourceEntity.Get<ResourceComponent>().Guid);
        }
    }
}