using UnityEngine;
using FootballSim.Core;
using FootballSim.PhysicsSim;

namespace FootballSim.AI
{
    public enum Role
    {
        Defender,
        Midfielder,
        Attacker
    }

    [RequireComponent(typeof(Rigidbody))]
    public class AIStateMachine : MonoBehaviour
    {
        [Header("References")]
        public BallPhysics ball;
        public Transform homePosition;
        public Transform opponentGoal;
        public Transform supportTarget;

        [Header("AI Settings")]
        public Role role = Role.Midfielder;
        public float maxSpeed = 6.5f;
        public float acceleration = 10f;
        public float pressDistance = 12f;
        public float supportDistance = 10f;
        public float mistakeChance = 0.08f;
        public float decisionCooldown = 0.4f;

        private readonly StateMachine stateMachine = new StateMachine();
        private Rigidbody rb;
        private Vector3 desiredVelocity;
        private float decisionTimer;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            stateMachine.ChangeState(new IdleState(this));
        }

        private void Update()
        {
            decisionTimer -= Time.deltaTime;
            stateMachine.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            stateMachine.FixedTick(Time.fixedDeltaTime);
            Move(Time.fixedDeltaTime);
        }

        private void Move(float deltaTime)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, desiredVelocity, acceleration * deltaTime);
        }

        private void SetDesiredVelocity(Vector3 targetDir)
        {
            desiredVelocity = targetDir.normalized * maxSpeed;
            if (Random.value < mistakeChance)
            {
                desiredVelocity += transform.right * Random.Range(-1f, 1f);
            }
        }

        private bool IsBallNear() => ball != null && Vector3.Distance(transform.position, ball.transform.position) < pressDistance;
        private bool IsSupportNeeded() => supportTarget != null && Vector3.Distance(transform.position, supportTarget.position) > supportDistance;
        private bool CanDecide() => decisionTimer <= 0f;
        private void ResetDecisionTimer() => decisionTimer = decisionCooldown;

        private abstract class BaseState : IState
        {
            protected readonly AIStateMachine owner;

            protected BaseState(AIStateMachine owner)
            {
                this.owner = owner;
            }

            public virtual void Enter() { }
            public virtual void Exit() { }
            public virtual void Tick(float deltaTime) { }
            public virtual void FixedTick(float fixedDeltaTime) { }
        }

        private class IdleState : BaseState
        {
            public IdleState(AIStateMachine owner) : base(owner) { }

            public override void Tick(float deltaTime)
            {
                if (!owner.CanDecide()) return;

                if (owner.IsBallNear())
                {
                    owner.stateMachine.ChangeState(new PressState(owner));
                }
                else if (owner.IsSupportNeeded())
                {
                    owner.stateMachine.ChangeState(new SupportState(owner));
                }
                else
                {
                    owner.stateMachine.ChangeState(new PositionState(owner));
                }

                owner.ResetDecisionTimer();
            }
        }

        private class PositionState : BaseState
        {
            public PositionState(AIStateMachine owner) : base(owner) { }

            public override void Tick(float deltaTime)
            {
                if (!owner.CanDecide()) return;

                if (owner.IsBallNear())
                {
                    owner.stateMachine.ChangeState(new PressState(owner));
                    owner.ResetDecisionTimer();
                    return;
                }

                if (owner.role == Role.Attacker && owner.opponentGoal != null)
                {
                    owner.stateMachine.ChangeState(new AttackRunState(owner));
                    owner.ResetDecisionTimer();
                    return;
                }

                if (owner.homePosition != null)
                {
                    Vector3 dir = owner.homePosition.position - owner.transform.position;
                    owner.SetDesiredVelocity(dir);
                }
            }
        }

        private class SupportState : BaseState
        {
            public SupportState(AIStateMachine owner) : base(owner) { }

            public override void Tick(float deltaTime)
            {
                if (owner.supportTarget == null)
                {
                    owner.stateMachine.ChangeState(new PositionState(owner));
                    return;
                }

                Vector3 dir = owner.supportTarget.position - owner.transform.position;
                owner.SetDesiredVelocity(dir);

                if (!owner.IsSupportNeeded())
                {
                    owner.stateMachine.ChangeState(new PositionState(owner));
                    owner.ResetDecisionTimer();
                }
            }
        }

        private class AttackRunState : BaseState
        {
            public AttackRunState(AIStateMachine owner) : base(owner) { }

            public override void Tick(float deltaTime)
            {
                if (owner.opponentGoal == null)
                {
                    owner.stateMachine.ChangeState(new PositionState(owner));
                    return;
                }

                Vector3 target = owner.opponentGoal.position + owner.transform.right * Random.Range(-4f, 4f);
                Vector3 dir = target - owner.transform.position;
                owner.SetDesiredVelocity(dir);

                if (owner.IsBallNear())
                {
                    owner.stateMachine.ChangeState(new PressState(owner));
                    owner.ResetDecisionTimer();
                }
            }
        }

        private class PressState : BaseState
        {
            public PressState(AIStateMachine owner) : base(owner) { }

            public override void Tick(float deltaTime)
            {
                if (!owner.IsBallNear())
                {
                    owner.stateMachine.ChangeState(new PositionState(owner));
                    owner.ResetDecisionTimer();
                    return;
                }

                if (owner.ball == null) return;

                Vector3 dir = owner.ball.transform.position - owner.transform.position;
                owner.SetDesiredVelocity(dir);

                if (dir.magnitude < 2.2f)
                {
                    owner.stateMachine.ChangeState(new TackleState(owner));
                }
            }
        }

        private class TackleState : BaseState
        {
            private float timer;

            public TackleState(AIStateMachine owner) : base(owner) { }

            public override void Enter()
            {
                timer = 0.25f;
            }

            public override void Tick(float deltaTime)
            {
                timer -= deltaTime;
                if (timer <= 0f)
                {
                    owner.stateMachine.ChangeState(new PositionState(owner));
                }
            }

            public override void FixedTick(float fixedDeltaTime)
            {
                if (owner.ball == null) return;
                Vector3 dir = owner.ball.transform.position - owner.transform.position;
                owner.SetDesiredVelocity(dir * 0.5f);
            }
        }
    }
}
