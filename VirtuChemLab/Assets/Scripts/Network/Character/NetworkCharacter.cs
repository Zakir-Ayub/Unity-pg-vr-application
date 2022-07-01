using System;
using Character;
using Unity.Netcode;
using Unity.XR.CoreUtils;
using UnityEngine;
using VR;

namespace Network
{
    /// <summary>
    /// Controls and moves the character model of our player model, so
    /// other people can see our VR movement.
    /// </summary>
    [RequireComponent(typeof(Character.Character))]
    public class NetworkCharacter : NetworkBehaviour
    {
        [Tooltip("Tag of the XROrigin root GameObject")]
        public const string VRPlayerTag = "VRPlayer";
        
        [Tooltip(
            "On some models the foot bone is at ankle height. To align the shoe soles with the floor, we need an approximate shoe height (offset from ankle to floor)")]
        public float shoeHeight = 0f;

        // variables used to position the character rig
        // TODO move into VRRig
        public UnityEngine.Transform topOfHead, bottomOfHead;
        private float cameraFloorOffset, headHeight, headToSpineDistance;
        private Character.Character character;
        private Character.Character vrCharacter;
        
        public VRRig vrRig { private set; get; }

        private void Start()
        {
            vrRig = GetComponentInChildren<VRRig>();
            
            if (IsLocalPlayer)
            {
                GameObject vrPlayer = GameObject.FindWithTag(VRPlayerTag);
                if (!vrPlayer)
                {
                    throw new Exception($"Could not find GameObject with Tag {VRPlayerTag}");
                }


                XROrigin xrOrigin = vrPlayer.GetComponent<XROrigin>();
                if (!xrOrigin)
                {
                    throw new Exception($"Could not find XROrigin on GameObject with tag {VRPlayerTag}");
                }

                cameraFloorOffset = xrOrigin.CameraYOffset;
                headHeight = topOfHead.position.y - bottomOfHead.position.y;
                headToSpineDistance = bottomOfHead.position.y - transform.position.y;

                character = GetComponent<Character.Character>();
                vrCharacter = vrPlayer.GetComponent<Character.Character>();
            }
        }

        private void Update()
        {
            if (IsLocalPlayer)
            {
                AlignCharacter(character, vrCharacter);
            }
        }

        private void AlignCharacter(Character.Character c, Character.Character vr)
        {
            AlignHead(c.head, vr.head);
            AlignBody(c.hips, vr.head);

            AlignHand(c.leftHand, vr.leftHand);
            AlignHand(c.rightHand, vr.rightHand);

            AlignFoot(c.leftFoot, vr.head);
            AlignFoot(c.rightFoot, vr.head);
        }

        private void AlignHead(UnityEngine.Transform head, UnityEngine.Transform vrHead)
        {
            head.position = vrHead.position - Vector3.up * headHeight / 2;
            head.rotation = vrHead.rotation;
        }

        private void AlignBody(UnityEngine.Transform body, UnityEngine.Transform vrHead)
        {
            body.position = vrHead.position - Vector3.up * headToSpineDistance - Vector3.up * headHeight / 2;
            body.eulerAngles = new Vector3(0, vrHead.eulerAngles.y, 0);
        }

        private void AlignHand(CharacterHand hand, CharacterHand vrHand)
        {
            hand.root.position = vrHand.root.position;
            hand.root.rotation = vrHand.root.rotation;

            AlignFinger(hand.thumb, vrHand.thumb);
            AlignFinger(hand.index, vrHand.index);
            AlignFinger(hand.middle, vrHand.middle);
            AlignFinger(hand.ring, vrHand.ring);
            AlignFinger(hand.pinky, vrHand.pinky);
        }

        private void AlignFinger(CharacterHand.Finger finger, CharacterHand.Finger vrFinger)
        {
            finger.tip.position = vrFinger.tip.position;
            //finger.middle.position = vrFinger.middle.position;
            //finger.root.position = vrFinger.root.position;
        }

        /// <summary>
        /// Sets the players "feet y = head y - camera offset y", to
        /// align them with the floor. If there is something in the way
        /// (e.g. floor or box) then set the feet y to the correct position.
        /// </summary>
        /// <param name="foot">Left or right foot</param>
        /// <param name="vrHead">VR Camera position</param>
        private void AlignFoot(UnityEngine.Transform foot, UnityEngine.Transform vrHead)
        {
            Vector3 footPosition = foot.position;
            Vector3 raycastOrigin = new Vector3(footPosition.x, vrHead.position.y - cameraFloorOffset / 2, footPosition.z);
            Vector3 shoeOffset = Vector3.up * shoeHeight;
            
            Debug.DrawRay(raycastOrigin, Vector3.down * cameraFloorOffset / 2);
            
            RaycastHit hit;
            if (Physics.Raycast(raycastOrigin, Vector3.down, out hit, cameraFloorOffset / 2))
            {
                foot.position = hit.point + shoeOffset;
            }
            else
            {
                foot.position = raycastOrigin + Vector3.down * cameraFloorOffset / 2 + shoeOffset;
            }
        }
    }
}