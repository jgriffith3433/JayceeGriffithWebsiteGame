using UnityEngine;
using System.Collections;

namespace RootMotion.Demos {
	
	/// <summary>
	/// User input for an AI controlled character controller.
	/// </summary>
	public class UserControlAI : UserControlThirdPerson {

        public KeyCode hitKey;

        public Transform moveTarget;
		public float stopAtDistance = 0.5f;
		public float stoppingThreshold = 1.5f;
        public float walkAtDistance = 7.5f;
        public float walkingThreshold = 1.5f;
        public Navigator navigator;
        public bool canControl = false;

        public System.Action ChangedInputState;

        protected override void Start()
        {
            base.Start();

            navigator.Initiate(transform);
        }

        protected override void Update () {
            if (moveTarget == null)
            {
                return;
            }
            var changedState = false;
            var wasCrouching = state.crouch;
            var wasWalking = state.walk;
            var previousActionIndex = state.actionIndex;
            var wasJumping = state.jump;
            // read inputs
            if (canControl)
            {
                state.crouch = canCrouch && Input.GetKey(KeyCode.C);
                state.walk = canWalk && Input.GetKey(KeyCode.LeftShift);
                state.jump = canJump && Input.GetButton("Jump");

                // calculate the head look target position
                state.lookPos = transform.position + transform.forward * 100f;
                state.actionIndex = Input.GetKey(hitKey) ? 1 : 0;
            }
            float moveSpeed = walkByDefault ? 0.5f : 1f;
            var moveTargetPosition = moveTarget.position;
            moveTargetPosition = new Vector3(moveTargetPosition.x, transform.position.y, moveTargetPosition.z);
            Vector3 direction = moveTargetPosition - transform.position;
            float distance = direction.magnitude;

            Vector3 normal = transform.up;
            Vector3.OrthoNormalize(ref normal, ref direction);
            float sD = state.move != Vector3.zero ? stopAtDistance : stopAtDistance * stoppingThreshold;
            float wD = state.move != Vector3.zero ? walkAtDistance : walkAtDistance * walkingThreshold;

            // If using Unity Navigation
            if (navigator.activeTargetSeeking)
            {
                navigator.Update(moveTarget.position);
                state.move = navigator.normalizedDeltaPosition * moveSpeed;
            }
            // No navigation, just move straight to the target
            else
            {
                state.move = distance > sD ? direction * moveSpeed : Vector3.zero;

                state.lookPos = moveTarget.position;
            }
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                state.walk = distance < wD;
            }
            float walkMultiplier = state.walk ? 0.5f : 1;
            state.move *= walkMultiplier;
            if (wasCrouching != state.crouch || wasWalking != state.walk || wasJumping != state.jump || previousActionIndex != state.actionIndex)
            {
                ChangedInputState?.Invoke();
            }
        }

        // Visualize the navigator
        void OnDrawGizmos()
        {
            if (navigator.activeTargetSeeking) navigator.Visualize();
        }
	}
}

