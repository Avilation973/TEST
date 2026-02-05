using UnityEngine;

namespace FootballSim.Data
{
    [CreateAssetMenu(menuName = "FootballSim/Control Settings", fileName = "ControlSettings")]
    public class ControlSettingsSO : ScriptableObject
    {
        [Header("Analog Hassasiyet")]
        [Range(0.5f, 2f)] public float moveSensitivity = 1.1f;
        [Range(0.5f, 2f)] public float lookSensitivity = 1.0f;

        [Header("Koşu")]
        public float sprintMultiplier = 1.35f;
        public float sprintControlLoss = 0.18f;

        [Header("Manuel Pas/Şut")]
        public bool manualPass = true;
        public bool manualShot = true;
    }
}
