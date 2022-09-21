using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Devices
{
    /// <summary>
    /// Controls the air/gas flow and allows the joints
    /// to stretch. Also prevents visual bugs by un-stretching
    /// and untangling the segments.
    /// </summary>
    public class HoseSegment : NetworkBehaviour
    {
        protected new Rigidbody rigidbody;
        private ConfigurableJoint joint;
        
        private HoseSegment next;
        public HoseSegment Next
        {
            set => next = value;
            get => next;
        }

        private HoseSegment prev;
        public HoseSegment Previous
        {
            set => prev = value;
            get => prev;
        }

        public float radius = 1f;
        public float height = 10f;

        public float Volume => (float) Math.PI * radius * 2 * (radius * height);
        
        public float MaxFluidAmount => Volume;
        
        [SerializeField]
        private float fluidAmount = 0f;
        public float FluidAmount
        {
            private set => fluidAmount = value;
            get => fluidAmount;
        }

        [SerializeField, Tooltip("How long should it takes to unstretch after being disconnected from a socket")]
        private float unstretchTime = 1f;
        private float unstretchTimer = 0f;
        private bool isUnstretching;
        
        private float linearLimitWhenUnstretched = 0f;
        private float linearLimitWhenStretched = 0.5f;

        [SerializeField, Tooltip("Controls how long the joint linear limit is set to 0 in order to untangle the segments")]
        private float untangleTime = 1f;
        private float untangleTimer = 0f;
        private bool isUntangling = false;

        private int prevNumberOfConnectedEnds = 0;

        protected virtual void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            joint = GetComponent<ConfigurableJoint>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkDespawn();

            if (!IsServer)
            {
                // the joints can cause major desync issues on clients
                Destroy(joint);
            }
        }

        protected virtual void Update()
        {
            if (IsServer)
            {
                if (joint) 
                {
                    int numConnectedEnds = GetNumConnectedEnds();

                    bool prevAllowStretching = prevNumberOfConnectedEnds == 2;
                    bool allowStretching = numConnectedEnds == 2;

                    if (allowStretching)
                    {
                        StopUnstretching(untangle: !prevAllowStretching);
                    
                        if (!isUntangling)
                        {
                            SetJointLinearLimit(linearLimitWhenStretched);
                        }
                        else
                        {
                            Untangle();
                        }
                    }
                    else
                    {
                        if (numConnectedEnds > prevNumberOfConnectedEnds)
                        {
                            // hose connected to socket or hand
                            StopUnstretching(untangle: true);
                        }
                        else if (numConnectedEnds < prevNumberOfConnectedEnds && numConnectedEnds > 0)
                        {
                            // hose disconnected from socket or hand but is still connected to at least one
                            StopUntangling();
                            BeginUnstretching();
                        }

                        if (isUntangling)
                        {
                            Untangle();
                        }
                        if (isUnstretching)
                        {
                            Unstretch();
                        }

                        if (!(isUntangling || isUnstretching))
                        {
                            SetJointLinearLimit(linearLimitWhenUnstretched);
                        }
                    }

                    prevNumberOfConnectedEnds = numConnectedEnds;
                }
                
                // balance fluids across all segments
                if (next)
                {
                    FluidFlow(next);
                }
                if (prev)
                {
                    FluidFlow(prev);
                }
            }

            // prevents too fast movement
            if (rigidbody.velocity.magnitude > 2f)
            {
                rigidbody.velocity = rigidbody.velocity.normalized * 2f;
            }
            if (rigidbody.angularVelocity.magnitude > 5f)
            {
                rigidbody.angularVelocity = rigidbody.angularVelocity.normalized * 5f;
            }
        }

        /// <summary>
        /// Adds or removes fluid to the target segment depending
        /// on how much fluid there is in this segment and in the
        /// other segment.
        /// </summary>
        /// <param name="to">The segment to which to add fluids</param>
        private void FluidFlow(HoseSegment to)
        {
            if (to.FluidAmount < FluidAmount)
            {
                // TODO: calculate pressure based on radius of pipe and volume difference
                float amount = Math.Min(FluidAmount, ((FluidAmount - to.FluidAmount) / 2f));
                
                RemoveFluids(amount);
                to.AddFluids(amount);
            }
        }
        
        /// <summary>
        /// Add fluid to this hose segment. 
        /// </summary>
        /// <param name="amount">How much fluid to add</param>
        public void AddFluids(float amount)
        {
            FluidAmount = Math.Min(MaxFluidAmount, FluidAmount + amount);
        }

        /// <summary>
        /// Remove fluid from this hose segment. 
        /// </summary>
        /// <param name="amount">How much fluid to remove</param>
        public void RemoveFluids(float amount)
        {
            FluidAmount = Math.Max(0f, FluidAmount - amount);
        }

        private void BeginUntangle()
        {
            SetJointLinearLimit(linearLimitWhenUnstretched);
            
            isUntangling = true;
            untangleTimer = 0f;
        }

        private void Untangle()
        {
            untangleTimer += Time.deltaTime;
            if (untangleTimer >= untangleTime)
            {
                StopUntangling();
            }
        }

        private void StopUntangling()
        {
            SetJointLinearLimit(linearLimitWhenStretched);
            
            isUntangling = false;
            untangleTimer = 0f;
        }
        
        private void BeginUnstretching()
        {
            SetJointSpring(20000f, 5000f);
            
            isUnstretching = true;
            unstretchTimer = 0f;
        }

        private void Unstretch()
        {
            unstretchTimer += Time.deltaTime;
            if (unstretchTimer >= unstretchTime)
            {
                StopUnstretching();
            }
        }

        private void StopUnstretching(bool untangle = true)
        {
            SetJointSpring(2000f, 500f);
            if (untangle)
            {
                BeginUntangle();
            }

            isUnstretching = false;
            unstretchTimer = 0f;
        }

        private void SetJointLinearLimit(float linearLimit)
        {
            SoftJointLimit limit = joint.linearLimit;
            limit.limit = linearLimit;
            joint.linearLimit = limit;
        }
        
        private void SetJointSpring(float spring, float damper)
        {
            SoftJointLimitSpring limit = joint.linearLimitSpring;
            limit.spring = spring;
            limit.damper = damper;
            joint.linearLimitSpring = limit;
        }

        /// <summary>
        /// Returns the number of segments in this hose,
        /// which are connected to a socket. 
        /// </summary>
        /// <returns>either 0, 1 or 2</returns>
        private int GetNumConnectedEnds()
        {
            int connected = 0;
            if (IsTopKinematic())
            {
                connected++;
            }
            if (IsBottomKinematic())
            {
                connected++;
            }
            return connected;
        }

        /// <summary>
        /// Returns whether the "top" i.e. first segment
        /// in the hose is kinematic.
        /// </summary>
        protected virtual bool IsTopKinematic()
        {
            if (Previous)
            {
                return Previous.IsTopKinematic();
            }
            return false;
        }
        
        /// <summary>
        /// Returns whether the "bottom" i.e. last segment
        /// in the hose is kinematic.
        /// </summary>
        protected virtual bool IsBottomKinematic()
        {
            if (Next)
            {
                return Next.IsTopKinematic();
            }
            return false;
        }
    }
}