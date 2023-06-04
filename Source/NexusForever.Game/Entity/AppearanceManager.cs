using NexusForever.Database.Character;
using NexusForever.Database.Character.Model;
using NexusForever.Game.Abstract.Customisation;
using NexusForever.Game.Abstract.Entity;
using NexusForever.Game.Customisation;
using NexusForever.Game.Static.Entity;
using NexusForever.Game.Static.Reputation;
using NexusForever.Network.World.Message.Model;

namespace NexusForever.Game.Entity
{
    public class AppearanceManager : IAppearanceManager
    {
        private readonly Dictionary</*label*/uint, ICustomisation> characterCustomisations = new();
        private readonly Dictionary<ItemSlot, IAppearance> characterAppearances = new();
        private readonly Dictionary<byte, IBone> characterBones = new();

        private readonly List<ICustomisation> deletedCharacterCustomisations = new();
        private readonly List<IAppearance> deletedCharacterAppearances = new();
        private readonly List<IBone> deletedCharacterBones = new();
       
        private readonly IPlayer owner;

        /// <summary>
        /// Create a new <see cref="IAppearanceManager"/> for <see cref="IPlayer"/>.
        /// </summary>
        public AppearanceManager(IPlayer player, CharacterModel model)
        {
            owner = player;

            foreach (CharacterAppearanceModel characterAppearance in model.Appearance)
            {
                IAppearance appearance = new Appearance(characterAppearance);
                characterAppearances.Add((ItemSlot)characterAppearance.Slot, appearance);

                owner.AddVisual(appearance.ItemSlot, appearance.DisplayId);
            }

            foreach (CharacterCustomisationModel characterCustomisation in model.Customisation)
                characterCustomisations.Add(characterCustomisation.Label, new Customisation(characterCustomisation));

            foreach (CharacterBoneModel bone in model.Bone.OrderBy(bone => bone.BoneIndex))
                characterBones.Add(bone.BoneIndex, new Bone(bone));
        }

        public void Save(CharacterContext context)
        {
            foreach (IAppearance characterAppearance in deletedCharacterAppearances)
                characterAppearance.Save(context);
            foreach (IBone characterBone in deletedCharacterBones)
                characterBone.Save(context);
            foreach (ICustomisation characterCustomisation in deletedCharacterCustomisations)
                characterCustomisation.Save(context);

            deletedCharacterAppearances.Clear();
            deletedCharacterBones.Clear();
            deletedCharacterCustomisations.Clear();

            foreach (IAppearance characterAppearance in characterAppearances.Values)
                characterAppearance.Save(context);
            foreach (IBone characterBone in characterBones.Values)
                characterBone.Save(context);
            foreach (ICustomisation characterCustomisation in characterCustomisations.Values)
                characterCustomisation.Save(context);
        }

        /// <summary>
        /// Return a collection of <see cref="ICustomisation"/> for <see cref="IPlayer"/>.
        /// </summary>
        public IEnumerable<ICustomisation> GetCustomisations()
        {
            return characterCustomisations.Values;
        }

        /// <summary>
        /// Return a collection of <see cref="IAppearance"/> for <see cref="IPlayer"/>.
        /// </summary>
        public IEnumerable<IAppearance> GetAppearances()
        {
            return characterAppearances.Values;
        }

        /// <summary>
        /// Return a collection of <see cref="IBone"/> for <see cref="IPlayer"/>.
        /// </summary>
        public IEnumerable<IBone> GetBones()
        {
            return characterBones.Values.OrderBy(b => b.BoneIndex);
        }

        /// <summary>
        /// Update <see cref="IPlayer"/> appearance.
        /// This will update, <see cref="Race"/>, <see cref="Sex"/>, customisations and bones.
        /// </summary>
        /// <remarks>
        /// This will do no validation, for customisation validation see <see cref="ICustomisationManager.Validate(Race, Sex, Faction, IList{ValueTuple{uint, uint}})"/>.
        /// </remarks>
        public void Update(Race race, Sex sex, IList<(uint Label, uint Value)> customisations, IList<float> bones)
        {
            owner.Race = race;
            owner.Sex  = sex;

            UpdateCustomisations(customisations);
            UpdateAppearances(race, sex, customisations);
            UpdateBones(bones);
        }

        private void UpdateCustomisations(IList<(uint Label, uint Value)> customisations)
        {
            foreach ((uint label, uint value) in customisations)
            {
                if (characterCustomisations.TryGetValue(label, out ICustomisation customisation))
                    customisation.Value = value;
                else
                    characterCustomisations.TryAdd(label, new Customisation(owner.CharacterId, label, value));
            }

            foreach (uint label in characterCustomisations.Keys.Except(customisations.Select(t => t.Label).ToList()))
            {
                if (!characterCustomisations.Remove(label, out ICustomisation customisation))
                    continue;

                customisation.Delete();
                deletedCharacterCustomisations.Add(customisation);
            }
        }

        private void UpdateAppearances(Race race, Sex sex, IList<(uint Label, uint Value)> customisations)
        {
            List<IItemVisual> itemVisuals = CustomisationManager.Instance.GetItemVisuals(race, sex, customisations).ToList();
            foreach (IItemVisual visual in itemVisuals)
            {
                if (characterAppearances.TryGetValue(visual.Slot, out IAppearance appearance))
                    appearance.DisplayId = visual.DisplayId.Value;
                else
                    characterAppearances.TryAdd(visual.Slot, new Appearance(owner.CharacterId, visual.Slot, visual.DisplayId.Value));

                owner.AddVisual(visual);
            }

            foreach (ItemSlot slot in characterAppearances.Keys.Except(itemVisuals.Select(a => a.Slot)).ToList())
            {
                if (!characterAppearances.Remove(slot, out IAppearance appearance))
                    continue;

                appearance.Delete();
                deletedCharacterAppearances.Add(appearance);
            }
        }

        private void UpdateBones(IList<float> bones)
        {
            for (byte i = 0; i < bones.Count; i++)
            {
                if (characterBones.TryGetValue(i, out IBone bone))
                    bone.BoneValue = bones[i];
                else
                    characterBones.Add(i, new Bone(owner.CharacterId, i, bones[i]));
            }

            owner.EnqueueToVisible(new ServerEntityBoneUpdate
            {
                UnitId = owner.Guid,
                Bones  = GetBones()
                    .Select(b => b.BoneValue)
                    .ToList()
            }, true);

            for (byte i = (byte)characterBones.Count; i >= bones.Count; i--)
            {
                if (!characterBones.Remove(i, out IBone bone))
                    continue;

                bone.Delete();
                deletedCharacterBones.Add(bone);
            }
        }
    }
}
