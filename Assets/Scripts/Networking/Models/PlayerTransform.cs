
using Assets.FlatBuffers;
using FlatBuffers;
using hololensMulti;
using hololensMultiplayer.Models;
using hololensMultiplayer.Packets;
using LiteNetLib;
using UnityEngine;

namespace hololensMultiModels
{
    public class PlayerTransform : BaseMessageType
    {
        public Vector3 Pos { get; set; } = Vector3.zero;
        public Quaternion Rot { get; set; } = Quaternion.identity;
        public Vector3 QrRotationOffset { get; set; } = Vector3.zero;

        public bool RHActive { get; set; }
        public FingersState RHFingers { get; set; }
        public Vector3 RHPos { get; set; } = Vector3.zero;
        public Quaternion RHRot { get; set; } = Quaternion.identity;

        public bool LHActive { get; set; }
        public FingersState LHFingers { get; set; }
        public Vector3 LHPos { get; set; } = Vector3.zero;
        public Quaternion LHRot { get; set; } = Quaternion.identity;

        public new MessageTypes MsgType = MessageTypes.PlayerTransform;

        public PlayerTransform()
        {

        }

        public PlayerTransform(byte[] data)
        {
            Deserialize(data);
        }

        public override WrapperPacket Serialize()
        {
            var builder = new FlatBufferBuilder(1);
            TransformFB.StartTransformFB(builder);

            TransformFB.AddPlayerID(builder, SenderID);

            TransformFB.AddPos(builder, Pos.ToVec3(builder));
            TransformFB.AddRot(builder, Rot.ToQuat(builder));
            TransformFB.AddQrOffset(builder, QrRotationOffset.ToVec3(builder));

            TransformFB.AddRHActive(builder, RHActive);
            TransformFB.AddRHState(builder, HandState.CreateHandState(builder, RHFingers.Pinky, RHFingers.Ring, RHFingers.Middle, RHFingers.Index, RHFingers.Thumb));
            TransformFB.AddRHPos(builder, RHPos.ToVec3(builder));
            TransformFB.AddRHRot(builder, RHRot.ToQuat(builder));

            TransformFB.AddLHActive(builder, LHActive);
            TransformFB.AddLHState(builder, HandState.CreateHandState(builder, LHFingers.Pinky, LHFingers.Ring, LHFingers.Middle, LHFingers.Index, LHFingers.Thumb));
            TransformFB.AddLHPos(builder, LHPos.ToVec3(builder));
            TransformFB.AddLHRot(builder, LHRot.ToQuat(builder));

            var offset = TransformFB.EndTransformFB(builder);
            TransformFB.FinishTransformFBBuffer(builder, offset);

            return new WrapperPacket(MsgType, builder.SizedByteArray(), DeliveryMethod.Unreliable);
        }

        public override void Deserialize(byte[] data)
        {
            ByteBuffer bb = new ByteBuffer(data);
            TransformFB transformFB = TransformFB.GetRootAsTransformFB(bb);

            SenderID = transformFB.PlayerID;
            RHActive = transformFB.RHActive;
            LHActive = transformFB.LHActive;

            if (transformFB.Pos.HasValue)
                Pos = transformFB.Pos.ToVector3();

            if (transformFB.Rot.HasValue)
                Rot = transformFB.Rot.ToQuaternion();

            if (transformFB.QrOffset.HasValue)
                QrRotationOffset = transformFB.QrOffset.ToVector3();

            if (transformFB.RHState.HasValue)
                RHFingers = new FingersState(transformFB.RHState.Value.Pinky, transformFB.RHState.Value.Ring, transformFB.RHState.Value.Middle, transformFB.RHState.Value.Index, transformFB.RHState.Value.Thumb);

            if (transformFB.RHPos.HasValue)
                RHPos = transformFB.RHPos.ToVector3();

            if (transformFB.RHRot.HasValue)
                RHRot = transformFB.RHRot.ToQuaternion();

            if (transformFB.LHState.HasValue)
                LHFingers = new FingersState(transformFB.LHState.Value.Pinky, transformFB.LHState.Value.Ring, transformFB.LHState.Value.Middle, transformFB.LHState.Value.Index, transformFB.LHState.Value.Thumb);

            if (transformFB.LHPos.HasValue)
                LHPos = transformFB.LHPos.ToVector3();

            if (transformFB.LHRot.HasValue)
                LHRot = transformFB.LHRot.ToQuaternion();
        }
    }

}
