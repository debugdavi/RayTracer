using System;
using System.Collections.Generic;

namespace RayTracer.Core
{
    public class Animator
    {
        public Dictionary<string, AnimationTrack<Vector3>> PositionTracks = new Dictionary<string, AnimationTrack<Vector3>>();
        public Dictionary<string, AnimationTrack<Vector3>> RotationTracks = new Dictionary<string, AnimationTrack<Vector3>>();
        public Dictionary<string, AnimationTrack<Vector3>> ScaleTracks = new Dictionary<string, AnimationTrack<Vector3>>();

        public Matrix4x4 GetTransformAt(string objectId, double time)
        {
            Vector3 pos = PositionTracks.ContainsKey(objectId) ? PositionTracks[objectId].Evaluate(time) : new Vector3(0, 0, 0);
            Vector3 rot = RotationTracks.ContainsKey(objectId) ? RotationTracks[objectId].Evaluate(time) : new Vector3(0, 0, 0);
            Vector3 scale = ScaleTracks.ContainsKey(objectId) ? ScaleTracks[objectId].Evaluate(time) : new Vector3(1, 1, 1);

            return Transform.Translate(pos.X, pos.Y, pos.Z) * 
                   Transform.RotateX(rot.X) * Transform.RotateY(rot.Y) * Transform.RotateZ(rot.Z) * 
                   Transform.Scale(scale.X, scale.Y, scale.Z);
        }
    }
}
