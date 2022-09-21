using UnityEngine;

namespace Devices
{
    /// <summary>
    /// A hose segment, but additionally can
    /// be connected to a <see cref="HoseSocket"/>.
    /// </summary>
    public class HoseEnd : HoseSegment
    {
        [SerializeField]
        public bool isBottom = false;
        
        private HoseSocket currentSocket;

        public void Connect(HoseSocket socket)
        {
            currentSocket = socket;
        }

        public void Disconnect()
        {
            currentSocket = null;
        }

        private bool IsConnectedToSocket()
        {
            return currentSocket;
        }

        protected override bool IsTopKinematic()
        {
            bool isTop = !isBottom;
            if (isTop)
            {
                return rigidbody.isKinematic;
            }
            return base.IsTopKinematic();
        }
        
        protected override bool IsBottomKinematic()
        {
            if (isBottom)
            {
                return rigidbody.isKinematic;
            }
            return base.IsTopKinematic();
        }
    }
}