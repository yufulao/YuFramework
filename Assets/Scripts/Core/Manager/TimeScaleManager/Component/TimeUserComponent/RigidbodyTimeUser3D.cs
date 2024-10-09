// ******************************************************************
//@file         RigidbodyTimeUser3D.cs
//@brief        timeUser封装的rigidbody3d
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.23 20:05:36
// ******************************************************************

using UnityEngine;

namespace Yu
{
    public class RigidbodyTimeUser3D : RigidbodyTimeUser<Rigidbody>
    {
        protected bool ReallyUsesGravity
        {
            get => Component.useGravity;
            set => Component.useGravity = value;
        }

        private bool IsReallyKinematic
        {
            get => Component.isKinematic;
            set => Component.isKinematic = value;
        }

        protected override bool IsReallyManual
        {
            get => IsReallyKinematic;
            set => IsReallyKinematic = value;
        }

        protected override float RealMass
        {
            get => Component.mass;
            set => Component.mass = value;
        }

        protected override Vector3 RealVelocity
        {
            get => Component.velocity;
            set => Component.velocity = value;
        }

        protected override Vector3 RealAngularVelocity
        {
            get => Component.angularVelocity;
            set => Component.angularVelocity = value;
        }

        protected override float RealDrag
        {
            get => Component.drag;
            set => Component.drag = value;
        }

        protected override float RealAngularDrag
        {
            get => Component.angularDrag;
            set => Component.angularDrag = value;
        }
        
        private bool _isKinematic;

        public bool IsKinematic
        {
            get => _isKinematic;
            set
            {
                _isKinematic = value;
                if (TimeUser.timeScale > 0)
                {
                    IsReallyKinematic = value;
                }
            }
        }

        protected override bool IsManual => IsKinematic;

        public bool UseGravity { get; set; }

        public Vector3 Velocity
        {
            get => RealVelocity / TimeUser.timeScale;
            set => RealVelocity = value * TimeUser.timeScale;
        }

        public Vector3 AngularVelocity
        {
            get => RealAngularVelocity / TimeUser.timeScale;
            set => RealAngularVelocity = value * TimeUser.timeScale;
        }
        
        
        public RigidbodyTimeUser3D(TimeUser timeUser, Rigidbody component) : base(timeUser, component)
        {
        }
        
        public override void FixedUpdate()
        {
            if (UseGravity && !Component.isKinematic && TimeUser.timeScale > 0)
            {
                Velocity += (Physics.gravity * TimeUser.fixedDeltaTime);
            }
        }

        protected override void CopyProperties(Rigidbody source)
        {
            IsKinematic = source.isKinematic;
            UseGravity = source.useGravity;
            source.useGravity = false;
        }

        protected override void WakeUp()
        {
            Component.WakeUp();
        }

        protected override bool IsSleeping()
        {
            return Component.IsSleeping();
        }

        public void AddForce(Vector3 force, ForceMode mode = ForceMode.Force)
        {
            Component.AddForce(AdjustForce(force), mode);
        }

        public void AddRelativeForce(Vector3 force, ForceMode mode = ForceMode.Force)
        {
            Component.AddRelativeForce(AdjustForce(force), mode);
        }

        public void AddForceAtPosition(Vector3 force, Vector3 position, ForceMode mode = ForceMode.Force)
        {
            Component.AddForceAtPosition(AdjustForce(force), position, mode);
        }

        public void AddExplosionForce(float explosionForce, Vector3 explosionPosition, float explosionRadius, float upwardsModifier = 0, ForceMode mode = ForceMode.Force)
        {
            Component.AddExplosionForce(AdjustForce(explosionForce), explosionPosition, explosionRadius, upwardsModifier, mode);
        }

        public void AddTorque(Vector3 torque, ForceMode mode = ForceMode.Force)
        {
            Component.AddTorque(AdjustForce(torque), mode);
        }

        public void AddRelativeTorque(Vector3 torque, ForceMode mode = ForceMode.Force)
        {
            Component.AddRelativeTorque(AdjustForce(torque), mode);
        }
    }
}