using UnityEngine;
using System.Collections;

namespace Fraktalia.Core.Samples
{
    public class RotatingObject : MonoBehaviour
    {
        public float RotateSpeed = 1;

        public float TargetSpeed = 1;
        public float LerpSpeed = 0.02f;
        public bool ShouldLerp = false;


        public bool X = false;
        public bool Y = false;
        public bool Z = true;

		public bool UseNormalUpdate;
		public Space RotationSpace = Space.Self;

        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
			if (!UseNormalUpdate)
			{
				Vector3 rotator = new Vector3();
				if (X) rotator.x = RotateSpeed;
				if (Y) rotator.y = RotateSpeed;
				if (Z) rotator.z = RotateSpeed;
				transform.Rotate(rotator, RotationSpace);

				if (ShouldLerp) RotateSpeed = Mathf.Lerp(RotateSpeed, TargetSpeed, LerpSpeed);
			}
		}

		private void Update()
		{
			if(UseNormalUpdate)
			{
				Vector3 rotator = new Vector3();
				if (X) rotator.x = RotateSpeed;
				if (Y) rotator.y = RotateSpeed;
				if (Z) rotator.z = RotateSpeed;
				transform.Rotate(rotator, RotationSpace);

				if (ShouldLerp) RotateSpeed = Mathf.Lerp(RotateSpeed, TargetSpeed, LerpSpeed);
			}
		}


		public void SetRotatingSpeed(float Value)
        {
            TargetSpeed = Value;
        }
    }
}
