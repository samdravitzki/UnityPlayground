using System;
using UnityEngine;

namespace FPController
{
    [CreateAssetMenu(fileName = "FirstPersonPreset", menuName = "FP Config", order = 1500)]
    public class FirstPersonConfiguration: ScriptableObject
    {
        public String Name = "Player";
        public String Tag = "Tag";
        
        public float Radius = 0.35f;
        public float Height = 1.75f;
        public float WalkSpeed = 5f;
        
        public float MaxSlopeAngle = 45f;
        public float MaxFriction = 20f;
        
        public float JumpForce = 5f;
    }
}