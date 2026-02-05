using UnityEngine;
using FootballSim.Data;
using FootballSim.PhysicsSim;

namespace FootballSim.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Data")]
        public PlayerProfileSO profile;
        public ControlSettingsSO controlSettings;

        [Header("References")]
        public Animator animator;
        public BallPhysics ball;

        [Header("Movement")]
        public float maxSpeed = 7.2f;
        public float acceleration = 14f;
        public float deceleration = 16f;
        public float turnSpeed = 8f;
        public float balanceLossThreshold = 5f;

        [Header("Ball Interaction")]
        public float passPower = 12f;
        public float shotPower = 18f;
        public float firstTouchRadius = 1.2f;
        public float firstTouchSpeedThreshold = 3.5f;
        public float firstTouchDamping = 0.55f;
        public float controlErrorMaxAngle = 10f;

        [Header("Stamina")]
        public float staminaDrainSprint = 12f;
        public float staminaRegen = 8f;

        private Rigidbody rb;
        private Rigidbody ballRb;
        private Vector3 inputDir;
        private float currentSpeed;
        private float stamina01 = 1f;
        private bool isSprinting;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            if (ball != null)
            {
                ballRb = ball.GetComponent<Rigidbody>();
            }
        }

        private void Update()
        {
            ReadInput();
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            UpdateStamina(Time.fixedDeltaTime);
            Move(Time.fixedDeltaTime);
            ApplyBalance();
            ApplyFirstTouch();
        }

        private void ReadInput()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            isSprinting = Input.GetButton("Sprint");

            inputDir = new Vector3(horizontal, 0f, vertical);
            inputDir = Vector3.ClampMagnitude(inputDir, 1f);
            if (controlSettings != null)
            {
                inputDir *= controlSettings.moveSensitivity;
            }

            if (Input.GetButtonDown("Pass"))
            {
                TryPass();
            }
            if (Input.GetButtonDown("Shoot"))
            {
                TryShoot();
            }
        }

        private void Move(float deltaTime)
        {
            float staminaFactor = Mathf.Lerp(0.7f, 1f, stamina01);
            float desiredSpeed = maxSpeed * staminaFactor;
            if (isSprinting)
            {
                desiredSpeed *= controlSettings != null ? controlSettings.sprintMultiplier : 1.3f;
            }

            float targetSpeed = inputDir.sqrMagnitude > 0.01f ? desiredSpeed : 0f;
            float accel = targetSpeed > currentSpeed ? acceleration : deceleration;

            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * deltaTime);

            if (inputDir.sqrMagnitude > 0.01f)
            {
                Vector3 desiredVelocity = inputDir.normalized * currentSpeed;
                Vector3 velocity = new Vector3(desiredVelocity.x, rb.velocity.y, desiredVelocity.z);
                rb.velocity = velocity;

                float turn = turnSpeed * Mathf.Lerp(0.6f, 1f, staminaFactor) * deltaTime;
                Quaternion targetRot = Quaternion.LookRotation(inputDir, Vector3.up);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, turn));
            }
            else
            {
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            }
        }

        private void ApplyBalance()
        {
            // Sprint sırasında kontrol kaybı: topa yakınken hareket yönü sapar.
            if (!isSprinting || ball == null) return;

            float distance = Vector3.Distance(transform.position, ball.transform.position);
            if (distance < firstTouchRadius)
            {
                float loss = controlSettings != null ? controlSettings.sprintControlLoss : 0.15f;
                Vector3 lateral = transform.right * Random.Range(-loss, loss);
                rb.velocity += lateral;
            }
        }

        private void ApplyFirstTouch()
        {
            if (ball == null || ballRb == null || profile == null) return;

            float distance = Vector3.Distance(transform.position, ball.transform.position);
            if (distance > firstTouchRadius) return;

            float ballSpeed = ballRb.velocity.magnitude;
            if (ballSpeed < firstTouchSpeedThreshold) return;

            float controlQuality = Mathf.Lerp(0.4f, 1f, profile.technique / 100f);
            float fatiguePenalty = Mathf.Lerp(0.6f, 1f, stamina01);
            float sprintPenalty = isSprinting ? 0.85f : 1f;
            float damping = firstTouchDamping * controlQuality * fatiguePenalty * sprintPenalty;

            Vector3 desiredDir = inputDir.sqrMagnitude > 0.01f ? inputDir : transform.forward;
            float errorAngle = ComputeControlErrorAngle(controlQuality * fatiguePenalty);
            desiredDir = Quaternion.Euler(0f, Random.Range(-errorAngle, errorAngle), 0f) * desiredDir;

            Vector3 targetVelocity = desiredDir.normalized * (ballSpeed * damping);
            ballRb.velocity = Vector3.Lerp(ballRb.velocity, targetVelocity, 0.65f);
        }

        private float ComputeControlErrorAngle(float controlQuality)
        {
            float error = Mathf.Lerp(controlErrorMaxAngle, 2f, controlQuality);
            if (isSprinting)
            {
                error += 3f;
            }

            return error;
        }

        private void UpdateStamina(float deltaTime)
        {
            if (profile == null) return;

            if (isSprinting && inputDir.sqrMagnitude > 0.01f)
            {
                stamina01 -= (staminaDrainSprint / Mathf.Max(profile.stamina, 1f)) * deltaTime;
            }
            else
            {
                stamina01 += (staminaRegen / Mathf.Max(profile.stamina, 1f)) * deltaTime;
            }

            stamina01 = Mathf.Clamp01(stamina01);
        }

        private void TryPass()
        {
            if (ball == null || profile == null) return;
            if (Vector3.Distance(transform.position, ball.transform.position) > firstTouchRadius) return;

            PreferredFoot usedFoot = GetUsedFoot();
            float footMultiplier = profile.GetFootMultiplier(usedFoot);
            float technique = Mathf.Lerp(0.5f, 1f, profile.passing / 100f);
            float fatiguePenalty = Mathf.Lerp(0.7f, 1f, stamina01);

            Vector3 direction = GetManualDirection();
            float power = passPower * footMultiplier * technique * fatiguePenalty;

            ball.Kick(ApplyControlError(direction, technique), power, RandomSpin(usedFoot, 0.3f));
        }

        private void TryShoot()
        {
            if (ball == null || profile == null) return;
            if (Vector3.Distance(transform.position, ball.transform.position) > firstTouchRadius) return;

            PreferredFoot usedFoot = GetUsedFoot();
            float footMultiplier = profile.GetFootMultiplier(usedFoot);
            float technique = Mathf.Lerp(0.4f, 1f, profile.finishing / 100f);
            float fatiguePenalty = Mathf.Lerp(0.6f, 1f, stamina01);

            Vector3 direction = GetManualDirection();
            float power = shotPower * footMultiplier * technique * fatiguePenalty;

            ball.Kick(ApplyControlError(direction, technique), power, RandomSpin(usedFoot, 0.6f));
        }

        private Vector3 GetManualDirection()
        {
            bool manual = controlSettings != null && controlSettings.manualPass;
            if (manual && inputDir.sqrMagnitude > 0.01f)
            {
                return inputDir;
            }

            return transform.forward;
        }

        private Vector3 ApplyControlError(Vector3 direction, float technique)
        {
            float controlQuality = Mathf.Lerp(0.5f, 1f, technique);
            float errorAngle = ComputeControlErrorAngle(controlQuality * Mathf.Lerp(0.7f, 1f, stamina01));
            return Quaternion.Euler(0f, Random.Range(-errorAngle, errorAngle), 0f) * direction.normalized;
        }

        private PreferredFoot GetUsedFoot()
        {
            // Dönüşlerde ayak değiştirme hissi: dönüş yönüne göre ayak seçimi.
            float side = Vector3.Dot(transform.right, inputDir);
            return side >= 0f ? PreferredFoot.Right : PreferredFoot.Left;
        }

        private Vector3 RandomSpin(PreferredFoot usedFoot, float intensity)
        {
            float direction = usedFoot == PreferredFoot.Right ? 1f : -1f;
            return new Vector3(0f, Random.Range(0.4f, 0.8f), direction * intensity);
        }

        private void UpdateAnimator()
        {
            if (animator == null) return;

            float speed01 = currentSpeed / Mathf.Max(maxSpeed, 0.01f);
            animator.SetFloat("Speed", speed01);
            animator.SetBool("Sprint", isSprinting);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude > balanceLossThreshold)
            {
                // Çarpışmalarda denge kaybı: kısa süreli yavaşlama.
                currentSpeed *= 0.5f;
            }
        }
    }
}
