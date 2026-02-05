using UnityEngine;

namespace FootballSim.Referee
{
    public class RefereeSystem : MonoBehaviour
    {
        [Header("Foul Rules")]
        public float slideTackleSpeed = 6f;
        public float foulChance = 0.25f;

        public bool IsFoul(Collision collision, bool isSlideTackle)
        {
            float speed = collision.relativeVelocity.magnitude;
            if (isSlideTackle && speed > slideTackleSpeed)
            {
                return Random.value < foulChance;
            }

            return false;
        }
    }
}
