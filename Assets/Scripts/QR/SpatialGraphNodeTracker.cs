using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

using Microsoft.MixedReality.OpenXR;

namespace QRTracking
{
    public class SpatialGraphNodeTracker : MonoBehaviour
    {
        private System.Guid _id = System.Guid.Empty;
        private SpatialGraphNode node;

        public System.Guid Id
        {
            get => _id;

            set
            {
                if (_id != value)
                {
                    _id = value;
                    InitializeSpatialGraphNode(force: true);
                }
            }
        }

        void Start()
        {
            InitializeSpatialGraphNode();
        }

        void Update()
        {
            InitializeSpatialGraphNode();
            if (node != null && node.TryLocate(FrameTime.OnUpdate, out Pose pose))
            {
                transform.SetPositionAndRotation(pose.position, pose.rotation);
                transform.Rotate(new Vector3(90, 0, 0));
                //transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }

        private void InitializeSpatialGraphNode(bool force = false)
        {
            if (node == null || force)
            {
                node = (Id != System.Guid.Empty) ? SpatialGraphNode.FromStaticNodeId(Id) : null;
            }
        }
    }
}