using UnityEngine;

namespace FootballSim.Data
{
    public enum PreferredFoot
    {
        Right,
        Left
    }

    [CreateAssetMenu(menuName = "FootballSim/Player Profile", fileName = "PlayerProfile")]
    public class PlayerProfileSO : ScriptableObject
    {
        [Header("Kimlik")]
        public string playerName = "Oyuncu";
        public PreferredFoot preferredFoot = PreferredFoot.Right;

        [Header("Temel Yetiler")]
        [Range(0f, 100f)] public float technique = 70f;
        [Range(0f, 100f)] public float strength = 65f;
        [Range(0f, 100f)] public float acceleration = 70f;
        [Range(0f, 100f)] public float agility = 68f;
        [Range(0f, 100f)] public float stamina = 80f;
        [Range(0f, 100f)] public float finishing = 70f;
        [Range(0f, 100f)] public float passing = 72f;

        [Header("Zayıf Ayak")]
        [Range(0f, 1f)] public float weakFootMultiplier = 0.75f;

        public float GetFootMultiplier(PreferredFoot usedFoot)
        {
            return usedFoot == preferredFoot ? 1f : weakFootMultiplier;
        }
    }
}
