using FlatBuffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using hololensMultiplayer;
using hololensMultiplayer.Models;
using hololensMulti;

public class Serializer
{
    public static byte[] SerializePlayerTransform(PlayerTransform player)
    {
        var builder = new FlatBufferBuilder(1);
        TransformFB.StartTransformFB(builder);

        TransformFB.AddPlayerID(builder, player.SenderID);
        TransformFB.AddPos(builder, Vec3.CreateVec3(builder, player.Pos.x, player.Pos.y, player.Pos.z));
        TransformFB.AddRot(builder, Quat.CreateQuat(builder, player.Rot.x, player.Rot.y, player.Rot.z, player.Rot.w));
        TransformFB.AddQrOffset(builder, Vec3.CreateVec3(builder, player.QrOffset.x, player.QrOffset.y, player.QrOffset.z));

        TransformFB.AddRHPos(builder, Vec3.CreateVec3(builder, player.RHPos.x, player.RHPos.y, player.RHPos.z));
        TransformFB.AddRHRot(builder, Quat.CreateQuat(builder, player.RHRot.x, player.RHRot.y, player.RHRot.z, player.RHRot.w));

        TransformFB.AddLHPos(builder, Vec3.CreateVec3(builder, player.LHPos.x, player.LHPos.y, player.LHPos.z));
        TransformFB.AddLHRot(builder, Quat.CreateQuat(builder, player.LHRot.x, player.LHRot.y, player.LHRot.z, player.LHRot.w));

        var offset = TransformFB.EndTransformFB(builder);
        TransformFB.FinishTransformFBBuffer(builder, offset);

        return builder.SizedByteArray();
    }

    public static byte[] SerializeObjectTransform(ObjectTransformMsg objectTransform)
    {
        var builder = new FlatBufferBuilder(1);
        var objectTypeOffset = builder.CreateString(objectTransform.ObjectType);

        ObjectFB.StartObjectFB(builder);

        ObjectFB.AddObjectID(builder, objectTransform.ObjectID);
        ObjectFB.AddObjectType(builder, objectTypeOffset);
        ObjectFB.AddPos(builder, Vec3.CreateVec3(builder, objectTransform.Pos.x, objectTransform.Pos.y, objectTransform.Pos.z));
        ObjectFB.AddRot(builder, Quat.CreateQuat(builder, objectTransform.Rot.x, objectTransform.Rot.y, objectTransform.Rot.z, objectTransform.Rot.w));
        ObjectFB.AddScale(builder, Vec3.CreateVec3(builder, objectTransform.Scale.x, objectTransform.Scale.y, objectTransform.Scale.z));
        ObjectFB.AddOwnerID(builder, objectTransform.OwnerID);

        var offset = ObjectFB.EndObjectFB(builder);
        ObjectFB.FinishObjectFBBuffer(builder, offset);

        return builder.SizedByteArray();
    }

    public static byte[] SerializeWelcome(Welcome welcome)
    {
        var builder = new FlatBufferBuilder(1);
        var playerName = builder.CreateString(welcome.Name);

        WelcomeFB.StartWelcomeFB(builder);

        WelcomeFB.AddPlayerID(builder, welcome.SenderID);
        WelcomeFB.AddPlayerName(builder, playerName);

        var offset = WelcomeFB.EndWelcomeFB(builder);
        WelcomeFB.FinishWelcomeFBBuffer(builder, offset);

        return builder.SizedByteArray();
    }

    public static byte[] SerializeDisconnect(DisconnectMessage disconnect)
    {
        var builder = new FlatBufferBuilder(1);
        
        DisconnectFB.StartDisconnectFB(builder);

        DisconnectFB.AddPlayerID(builder, disconnect.SenderID);
        
        var offset = DisconnectFB.EndDisconnectFB(builder);
        DisconnectFB.FinishDisconnectFBBuffer(builder, offset);

        return builder.SizedByteArray();
    }
}
