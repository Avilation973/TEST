using UnityEngine;
using FootballSim.Data;
using System.Collections.Generic;

namespace FootballSim.Career
{
    public class CareerManager : MonoBehaviour
    {
        [Header("Roster")]
        public int squadSize = 22;
        public List<PlayerProfileSO> roster = new List<PlayerProfileSO>();

        public void GenerateRandomSquad()
        {
            roster.Clear();
            for (int i = 0; i < squadSize; i++)
            {
                PlayerProfileSO player = ScriptableObject.CreateInstance<PlayerProfileSO>();
                player.playerName = $"Oyuncu {i + 1}";
                player.preferredFoot = Random.value > 0.7f ? PreferredFoot.Left : PreferredFoot.Right;
                player.technique = Random.Range(55f, 90f);
                player.stamina = Random.Range(60f, 95f);
                player.finishing = Random.Range(50f, 85f);
                player.passing = Random.Range(55f, 88f);
                roster.Add(player);
            }
        }

        public void ApplyMatchForm(PlayerProfileSO player, float performanceScore)
        {
            player.technique = Mathf.Clamp(player.technique + performanceScore, 40f, 99f);
            player.stamina = Mathf.Clamp(player.stamina + performanceScore * 0.5f, 40f, 99f);
        }
    }
}
