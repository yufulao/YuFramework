// ******************************************************************
//@file         RigidbodyTimeUser2D.cs
//@brief        timeUser封装的rigidbody2d
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.23 20:03:48
// ******************************************************************
using UnityEngine;

namespace Yu
{
    public class RigidbodyTimeUser2D: RigidbodyTimeUser<Rigidbody2D>
    {
	    protected float RealGravityScale
		{
			get => Component.gravityScale;
			set => Component.gravityScale = value;
		}

		protected override bool IsReallyManual
		{
			get => RealBodyType == RigidbodyType2D.Kinematic;
			set => RealBodyType = value ? RigidbodyType2D.Kinematic : BodyType;
		}

		private RigidbodyType2D RealBodyType
		{
			get => Component.bodyType;
			set => Component.bodyType = value;
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
			get => Component.angularVelocity * Vector3.one;
			set => Component.angularVelocity = value.x;
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
		
		private RigidbodyType2D _bodyType;
		
		public RigidbodyType2D BodyType
		{
			get => _bodyType;
			set
			{
				_bodyType = value;
				if (TimeUser.timeScale > 0)
				{
					RealBodyType = value;
				}
			}
		}

		protected override bool IsManual => BodyType == RigidbodyType2D.Kinematic;
		
		public float GravityScale { get; set; }
		
		public Vector2 Velocity
		{
			get => RealVelocity / TimeUser.timeScale;
			set => RealVelocity = value * TimeUser.timeScale;
		}
		
		public float AngularVelocity
		{
			get => RealAngularVelocity.x / TimeUser.timeScale;
			set => RealAngularVelocity = value * Vector3.one * TimeUser.timeScale;
		}
		
		
		public RigidbodyTimeUser2D(TimeUser timeUser, Rigidbody2D component) : base(timeUser, component) { }
	    
		public override void FixedUpdate()
		{
			if (IsReallyManual && !IsManual)
			{
				RealVelocity = Vector3.zero;
				RealAngularVelocity = Vector3.zero;
			}

			if (!IsReallyManual && TimeUser.timeScale > 0)
			{
				Velocity += (Physics2D.gravity * (GravityScale * TimeUser.fixedDeltaTime));
			}
		}
	    
		protected override void CopyProperties(Rigidbody2D source)
		{
			BodyType = source.bodyType;
			GravityScale = source.gravityScale;
			source.gravityScale = 0;
		}

		protected override bool IsSleeping()
		{
			return Component.IsSleeping();
		}

		protected override void WakeUp()
		{
			Component.WakeUp();
		}

		/// <summary>
		/// 施加力
		/// </summary>
		public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force)
		{
			Component.AddForce(AdjustForce(force), mode);
		}

		/// <summary>
		/// 施加相对作用力
		/// </summary>
		public void AddRelativeForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force)
		{
			Component.AddRelativeForce(AdjustForce(force), mode);
		}

		/// <summary>
		/// rigidbody的AddForceAtPosition
		/// </summary>
		public void AddForceAtPosition(Vector2 force, Vector2 position, ForceMode2D mode = ForceMode2D.Force)
		{
			Component.AddForceAtPosition(AdjustForce(force), position, mode);
		}

		/// <summary>
		/// rigidbody的AddTorque扭矩
		/// </summary>
		public void AddTorque(float torque, ForceMode2D mode = ForceMode2D.Force)
		{
			Component.AddTorque(AdjustForce(torque), mode);
		}
    }
}