// ******************************************************************
//@file         RigidbodyTimeUser.cs
//@brief        timeUser封装的rigidbody
//@author       yufulao, yufulao@qq.com
//@createTime   2024.06.23 19:55:59
// ******************************************************************

using UnityEngine;

namespace Yu
{
	public abstract class RigidbodyTimeUser<TComponent>: ComponentTimeUser<TComponent>, IRigidbodyTimeUser where TComponent : Component
    {
	    protected float LastPositiveTimeScale = 1;
	    protected float ZeroTime;
	    protected Vector3 ZeroDestination;
	    protected abstract bool IsReallyManual { get; set; }
	    protected abstract float RealMass { get; set; }
	    protected abstract Vector3 RealVelocity { get; set; }
	    protected abstract Vector3 RealAngularVelocity { get; set; }
	    protected abstract float RealDrag { get; set; }
	    protected abstract float RealAngularDrag { get; set; }
	    protected abstract bool IsSleeping();
	    protected abstract void WakeUp();
		
	    protected abstract bool IsManual { get; }
	    
	    public float Mass
	    {
		    get => RealMass;
		    set => RealMass = value;
	    }
	    
	    public float Drag
	    {
		    get => RealDrag / TimeUser.timeScale;
		    set => RealDrag = value * TimeUser.timeScale;
	    }
	    
	    public float AngularDrag
	    {
		    get => RealAngularDrag / TimeUser.timeScale;
		    set => RealAngularDrag = value * TimeUser.timeScale;
	    }
	    
	    
        public RigidbodyTimeUser(TimeUser timeUser, TComponent component) : base(timeUser, component) { }

		public override void Update()
		{
			var timeScale = TimeUser.timeScale;

			switch (timeScale)
			{
				case <= 0:
					IsReallyManual = true;
					break;
				case > 0:
					IsReallyManual = IsManual;
					WakeUp();
					break;
			}

			if (timeScale > 0&& !IsReallyManual) //加速减速
			{
				var modifier = timeScale / LastPositiveTimeScale;
				RealVelocity *= modifier;
				RealAngularVelocity *= modifier;
				RealDrag *= modifier;
				RealAngularDrag *= modifier;
				WakeUp();
			}

			switch (timeScale)
			{
				case > 0:
					IsReallyManual = IsManual;
					LastPositiveTimeScale = timeScale;
					break;
				case < 0:
					break;
			}
		}
		
		
		
		public override void Reset()
		{
			LastPositiveTimeScale = 1;
			base.Reset();
		}

		/// <summary>
		/// 修正时间下的force
		/// </summary>
		protected virtual float AdjustForce(float force)
		{
			return force * TimeUser.timeScale;
		}

		protected virtual Vector2 AdjustForce(Vector2 force)
		{
			return force * TimeUser.timeScale;
		}

		protected virtual Vector3 AdjustForce(Vector3 force)
		{
			return force * TimeUser.timeScale;
		}
    }
}