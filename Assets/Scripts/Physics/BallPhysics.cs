using UnityEngine;

namespace FootballSim.PhysicsSim
{
    [RequireComponent(typeof(Rigidbody))]
    public class BallPhysics : MonoBehaviour
    {
        [Header("Physics")]
        public float groundFriction = 0.18f;
        public float airDrag = 0.02f;
        public float magnusStrength = 0.35f;
        public float bounceDamping = 0.7f;
        public float spinDamping = 0.15f;

        private Rigidbody rb;
        private bool isGrounded;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.maxAngularVelocity = 60f;
        }

        private void FixedUpdate()
        {
            ApplyAirDrag();
            ApplyMagnusEffect();
            ApplyGroundFriction();
            ApplySpinDamping();
        }

        public void Kick(Vector3 direction, float power, Vector3 spin)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(direction.normalized * power, ForceMode.VelocityChange);
            rb.AddTorque(spin, ForceMode.VelocityChange);
        }

        private void ApplyAirDrag()
        {
            rb.velocity *= 1f - airDrag * Time.fixedDeltaTime;
        }

        private void ApplyMagnusEffect()
        {
            if (rb.velocity.sqrMagnitude < 0.1f) return;
            Vector3 magnus = Vector3.Cross(rb.angularVelocity, rb.velocity) * magnusStrength;
            rb.AddForce(magnus, ForceMode.Force);
        }

        private void ApplyGroundFriction()
        {
            if (!isGrounded) return;
            Vector3 lateral = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            lateral *= 1f - groundFriction * Time.fixedDeltaTime;
            rb.velocity = new Vector3(lateral.x, rb.velocity.y, lateral.z);
        }

        private void ApplySpinDamping()
        {
            if (!isGrounded) return;
            rb.angularVelocity = Vector3.Lerp(rb.angularVelocity, Vector3.zero, spinDamping * Time.fixedDeltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            UpdateGroundedState(collision);
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * bounceDamping, rb.velocity.z);
        }

        private void OnCollisionStay(Collision collision)
        {
            UpdateGroundedState(collision);
        }

        private void OnCollisionExit(Collision collision)
        {
            isGrounded = false;
        }

        private void UpdateGroundedState(Collision collision)
        {
            if (collision.contacts.Length == 0) return;
            Vector3 normal = collision.contacts[0].normal;
            float dot = Vector3.Dot(normal, Vector3.up);
            isGrounded = dot > 0.5f;
        }
    }
}
