using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Devices
{
    /// <summary>
    /// Connects it's children hose segments similar
    /// to a linear list.
    /// </summary>
    public class HoseController : NetworkBehaviour
    {
        [SerializeField]
        private float segmentSize = 0.5f;
        public float SegmentSize => segmentSize;

        private List<HoseSegment> segments = new List<HoseSegment>();

        private void Start()
        {
            segments = GetSegments();
            ConnectSegments();
        }

        public List<HoseSegment> GetSegments()
        {
            List<HoseSegment> list = new List<HoseSegment>();

            foreach (Transform t in transform)
            {
                HoseSegment segment = t.GetComponent<HoseSegment>();
                list.Add(segment);
            }

            return list;
        }

        private void ConnectSegments()
        {
            HoseSegment prev = null;
            foreach (HoseSegment segment in segments)
            {
                if (prev)
                {
                    prev.Next = segment;
                    segment.Previous = prev;
                }
                
                prev = segment;
            }
        }
    }
}