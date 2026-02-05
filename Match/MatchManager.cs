using UnityEngine;
using System;

namespace FootballSim.Match
{
    public class MatchManager : MonoBehaviour
    {
        [Header("Match")]
        public float halfLengthMinutes = 6f;
        public bool allowExtraTime = true;

        public event Action OnGoal;

        private float matchTime;
        private int currentHalf = 1;
        private bool inExtraTime;

        private void Update()
        {
            matchTime += Time.deltaTime;

            float halfSeconds = halfLengthMinutes * 60f;
            if (matchTime >= halfSeconds)
            {
                AdvanceHalf();
            }
        }

        private void AdvanceHalf()
        {
            if (currentHalf == 1)
            {
                currentHalf = 2;
                matchTime = 0f;
            }
            else if (allowExtraTime && !inExtraTime)
            {
                inExtraTime = true;
                currentHalf = 3;
                matchTime = 0f;
            }
            else
            {
                EndMatch();
            }
        }

        public void RegisterGoal()
        {
            OnGoal?.Invoke();
        }

        private void EndMatch()
        {
            enabled = false;
        }
    }
}
