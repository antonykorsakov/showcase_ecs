using Unity.Entities;
using Unity.Physics;

namespace PhysicsExpansionModule.Core
{
    // Extends ISimulationEvent with extra StatefulEventState.
    public interface IStatefulSimulationEvent<T> : IBufferElementData, ISimulationEvent<T>
    {
        public StatefulEventState State { get; set; }
    }
}