using UnityEngine;
using FootballSim.PhysicsSim;

namespace FootballSim.CameraSystem
{
    public class BroadcastCamera : MonoBehaviour
    {
        [Header("References")]
        public BallPhysics ball;
        public Transform followTarget;
        public Transform penaltyAreaCenter;

        [Header("Tuning")]
        public Vector3 offset = new Vector3(0f, 18f, -22f);
        public float followSpeed = 3.5f;
        public float zoomMin = 40f;
        public float zoomMax = 58f;
        public float dramaticZoneRadius = 18f;
        public float goalCinematicDuration = 2.2f;

        private Camera cam;
        private float cinematicTimer;
        private Vector3 defaultOffset;

        private void Awake()
        {
            cam = GetComponent<Camera>();
            defaultOffset = offset;
        }

        private void LateUpdate()
        {
            if (ball == null || cam == null) return;

            if (cinematicTimer > 0f)
            {
                cinematicTimer -= Time.deltaTime;
                return;
            }

            Vector3 targetPos = followTarget != null ? followTarget.position : ball.transform.position;
            Vector3 desired = targetPos + offset;
            transform.position = Vector3.Lerp(transform.position, desired, followSpeed * Time.deltaTime);
            transform.LookAt(targetPos);

            float ballSpeed = ball.GetComponent<Rigidbody>().velocity.magnitude;
            float zoom = Mathf.Lerp(zoomMax, zoomMin, Mathf.InverseLerp(0f, 25f, ballSpeed));

            // Ceza sahasında dramatik yakınlaşma.
            if (penaltyAreaCenter != null && Vector3.Distance(targetPos, penaltyAreaCenter.position) < dramaticZoneRadius)
            {
                zoom = Mathf.Lerp(zoom, zoomMin, 0.6f);
            }

            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom, Time.deltaTime * 2f);
        }

        public void PlayGoalCinematic(Transform goalCameraPoint)
        {
            if (goalCameraPoint == null) return;
            transform.position = goalCameraPoint.position;
            transform.rotation = goalCameraPoint.rotation;
            offset = defaultOffset;
            cinematicTimer = goalCinematicDuration;
        }
    }
}
