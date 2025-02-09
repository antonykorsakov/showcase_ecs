using Unity.Collections;
using Unity.Entities;

namespace CharacterAnimModule.Data
{
    public struct CharacterAnimData : IComponentData
    {
        public FixedList512Bytes<Hash128> AnimClips;
    }
}