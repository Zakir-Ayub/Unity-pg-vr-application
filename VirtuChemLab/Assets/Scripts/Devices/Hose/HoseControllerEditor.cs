using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Devices
{
    /// <summary>
    /// Provides utility functions for the unity editor, to
    /// align, connect and resize the hose segments.
    /// </summary>
    [CustomEditor(typeof(HoseController))]
    public class HoseControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Connect joints"))
            {
                ConnectJoints();
            }
            else if (GUILayout.Button("Update segment position"))
            {
                SetSegmentPositions();
            }
            else if (GUILayout.Button("Update segment size"))
            {
                SetSegmentSizes();
            }
        }
        
        private void ConnectJoints()
        {
            HoseSegment prev = null;
            foreach (HoseSegment segment in GetHoseController().GetSegments())
            {
                if (prev)
                {
                    segment.GetComponent<ConfigurableJoint>().connectedBody = prev.GetComponent<Rigidbody>();
                }
                
                prev = segment;
            }
        }

        private void SetSegmentPositions()
        {
            HoseController controller = GetHoseController();
            List<HoseSegment> segments = controller.GetSegments();

            for (int i = 0; i < segments.Count; i++)
            {
                HoseSegment segment = segments[i];
                float y = i * controller.SegmentSize;
                segment.transform.localPosition = new Vector3(0f, y, 0f);
            }
        }
        
        private void SetSegmentSizes()
        {
            HoseController controller = GetHoseController();
            
            foreach (HoseSegment segment in controller.GetSegments())
            {
                segment.GetComponent<CapsuleCollider>().height = controller.SegmentSize;
            }
        }

        private HoseController GetHoseController()
        {
            return (HoseController) target;
        }
    }
}