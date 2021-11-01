using System;
using UnityEngine;

/*
 * Credits for provision of interpolate functions go to Andrey Sitnik and Ivan Solovev:
 *      https://easings.net/
 */

namespace MyMath
{
    public enum InterpolateType
    {
		Lerp,
		EaseInSine,
		EaseOutSine,
		EaseInOutSine,
		EaseInQuad,
		EaseOutQuad,
		EaseInOutQuad,
		EaseInCubic,
		EaseOutCubic,
		EaseInOutCubic,
		EaseInQuart,
		EaseOutQuart,
		EaseInOutQuart,
		EaseInQuint,
		EaseOutQuint,
		EaseInOutQuint,
		EaseInExpo,
		EaseOutExpo,
		EaseInOutExpo,
		EaseInCirc,
		EaseOutCirc,
		EaseInOutCirc,
		EaseInBack,
		EaseOutBack,
		EaseInOutBack,
		EaseInElastic,
		EaseOutElastic,
		EaseInOutElastic,
		EaseInBounce,
		EaseOutBounce,
		EaseInOutBounce
	}

	public class InterpolateFunctions
	{
		public static float Interpolate(float from, float to, float time, InterpolateType type = InterpolateType.Lerp)
		{
			return Mathf.LerpUnclamped(from, to, Ease(time, type));
		}

        public static Vector2 Interpolate(Vector2 from, Vector2 to, float time, InterpolateType type = InterpolateType.Lerp)
		{
			return Vector2.LerpUnclamped(from, to, Ease(time, type));
		}

		public static Vector3 Interpolate(Vector3 from, Vector3 to, float time, InterpolateType type = InterpolateType.Lerp)
		{
			return Vector3.LerpUnclamped(from, to, Ease(time, type));
		}

		public static Quaternion Interpolate(Quaternion from, Quaternion to, float time, InterpolateType type = InterpolateType.Lerp)
		{
			return Quaternion.LerpUnclamped(from, to, Ease(time, type));
		}

		private static float Ease(float time, InterpolateType type) {
			switch (type)
			{
				case InterpolateType.EaseInSine:
					return easeInSine(time);
				case InterpolateType.EaseOutSine:
					return easeOutSine(time);
				case InterpolateType.EaseInOutSine:
					return easeInOutSine(time);
				case InterpolateType.EaseInQuad:
					return easeInQuad(time);
				case InterpolateType.EaseOutQuad:
					return easeOutQuad(time);
				case InterpolateType.EaseInOutQuad:
					return easeInOutQuad(time);
				case InterpolateType.EaseInCubic:
					return easeInCubic(time);
				case InterpolateType.EaseOutCubic:
					return easeOutCubic(time);
				case InterpolateType.EaseInOutCubic:
					return easeInOutCubic(time);
				case InterpolateType.EaseInQuart:
					return easeInQuart(time);
				case InterpolateType.EaseOutQuart:
					return easeOutQuart(time);
				case InterpolateType.EaseInOutQuart:
					return easeInOutQuart(time);
				case InterpolateType.EaseInQuint:
					return easeInQuint(time);
				case InterpolateType.EaseOutQuint:
					return easeOutQuint(time);
				case InterpolateType.EaseInOutQuint:
					return easeInOutQuint(time);
				case InterpolateType.EaseInExpo:
					return easeInExpo(time);
				case InterpolateType.EaseOutExpo:
					return easeOutExpo(time);
				case InterpolateType.EaseInOutExpo:
					return easeInOutExpo(time);
				case InterpolateType.EaseInCirc:
					return easeInCirc(time);
				case InterpolateType.EaseOutCirc:
					return easeOutCirc(time);
				case InterpolateType.EaseInOutCirc:
					return easeInOutCirc(time);
				case InterpolateType.EaseInBack:
					return easeInBack(time);
				case InterpolateType.EaseOutBack:
					return easeOutBack(time);
				case InterpolateType.EaseInOutBack:
					return easeInOutBack(time);
				case InterpolateType.EaseInElastic:
					return easeInElastic(time);
				case InterpolateType.EaseOutElastic:
					return easeOutElastic(time);
				case InterpolateType.EaseInOutElastic:
					return easeInOutElastic(time);
				case InterpolateType.EaseInBounce:
					return easeInBounce(time);
				case InterpolateType.EaseOutBounce:
					return easeOutBounce(time);
				case InterpolateType.EaseInOutBounce:
					return easeInOutBounce(time);
				default:
					return time;
			}
		}

		private static float easeInSine(float time)
		{
			return 1f - Mathf.Cos((time * Mathf.PI) / 2f);
		}

		private static float easeOutSine(float time)
		{
			return Mathf.Sin((time * Mathf.PI) / 2f);
		}

        internal static Quaternion Interpolate(Quaternion from, Quaternion to, float delta, object interpolateType)
        {
            throw new NotImplementedException();
        }

        private static float easeInOutSine(float time)
		{
			return -(Mathf.Cos(Mathf.PI * time) - 1f) / 2f;
		}

		private static float easeInQuad(float time)
		{
			return time * time;
		}

		private static float easeOutQuad(float time)
		{
			return 1f - (1f - time) * (1f - time);
		}

		private static float easeInOutQuad(float time)
		{
			return time < 0.5f ? 2f * time * time : 1f - Mathf.Pow(-2f * time + 2f, 2f) / 2f;
		}

		private static float easeInCubic(float time)
		{
			return time * time * time;
		}

		private static float easeOutCubic(float time)
		{
			return 1f - Mathf.Pow(1f - time, 3f);
		}

		private static float easeInOutCubic(float time)
		{
			return time < 0.5f 
				? 4f * time * time * time 
				: 1f - Mathf.Pow(-2f * time + 2f, 3f) / 2f;
		}

		private static float easeInQuart(float time)
		{
			return time * time * time * time;
		}

		private static float easeOutQuart(float time)
		{
			return 1f - Mathf.Pow(1f - time, 4f);
		}

		private static float easeInOutQuart(float time)
		{
			return time < 0.5f 
				? 8f * time * time * time * time 
				: 1f - Mathf.Pow(-2 * time + 2f, 4f) / 2f;
		}

		private static float easeInQuint(float time)
		{
			return time * time * time * time * time;
		}

		private static float easeOutQuint(float time)
		{
			return 1f - Mathf.Pow(1f - time, 5f);
		}

		private static float easeInOutQuint(float time)
		{
			return time < 0.5f 
				? 16f * time * time * time * time * time 
				: 1f - Mathf.Pow(-2f * time + 2f, 5f) / 2f;
		}

		private static float easeInExpo(float time)
		{
			return time == 0f
				? 0f 
				: Mathf.Pow(2f, 10f * time - 10f);
		}

		private static float easeOutExpo(float time)
		{
			return time == 1f
				? 1f 
				: 1f - Mathf.Pow(2f, -10f * time);
		}

		private static float easeInOutExpo(float time)
		{
			return time == 0f
			  ? 0f
			  : time == 1f
				? 1f
				: time < 0.5f 
					? Mathf.Pow(2f, 20f * time - 10f) / 2f
					: (2 - Mathf.Pow(2f, -20f * time + 10f)) / 2f;
		}

		private static float easeInCirc(float time)
		{
			return 1f - Mathf.Sqrt(1f - Mathf.Pow(time, 2f));
		}

		private static float easeOutCirc(float time)
		{
			return Mathf.Sqrt(1f - Mathf.Pow(time - 1f, 2f));
		}

		private static float easeInOutCirc(float time)
		{
			return time < 0.5f
			  ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * time, 2f))) / 2f
			  : (Mathf.Sqrt(1f - Mathf.Pow(-2f * time + 2f, 2f)) + 1f) / 2f;
		}

		private static float easeInBack(float time)
		{
			float c1 = 1.70158f;
			float c3 = c1 + 1f;

			return c3 * time * time * time - c1 * time * time;
		}

		private static float easeOutBack(float time)
		{
			float c1 = 1.70158f;
			float c3 = c1 + 1f;

			return 1f + c3 * Mathf.Pow(time - 1f, 3f) + c1 * Mathf.Pow(time - 1f, 2f);
		}

		private static float easeInOutBack(float time)
		{
			float c1 = 1.70158f;
			float c2 = c1 * 1.525f;

			return time < 0.5f
			  ? (Mathf.Pow(2f * time, 2f) * ((c2 + 1f) * 2f * time - c2)) / 2f
			  : (Mathf.Pow(2f * time - 2f, 2f) * ((c2 + 1f) * (time * 2f - 2f) + c2) + 2f) / 2f;

		}

		private static float easeInElastic(float time)
		{
			float c4 = (2f * Mathf.PI) / 3f;
			return time == 0f
			  ? 0f
			  : time == 1f
			  ? 1f
			  : -Mathf.Pow(2f, 10f * time - 10f) * Mathf.Sin((time * 10f - 10.75f) * c4);
		}

		private static float easeOutElastic(float time)
		{
			float c4 = (2f * Mathf.PI) / 3f;
			return time == 0f
			  ? 0f
			  : time == 1f
			  ? 1f
			  : Mathf.Pow(2f, -10f * time) * Mathf.Sin((time * 10f - 0.75f) * c4) + 1f;
		}

		private static float easeInOutElastic(float time)
		{
			float c5 = (2f * Mathf.PI) / 4.5f;

			return time == 0f
			  ? 0f
			  : time == 1f
				  ? 1f
				  : time < 0.5f
					  ? -(Mathf.Pow(2f, 20f * time - 10f) * Mathf.Sin((20f * time - 11.125f) * c5)) / 2f
					  : (Mathf.Pow(2f, -20f * time + 10f) * Mathf.Sin((20f * time - 11.125f) * c5)) / 2f + 1f;
		}

		private static float easeInBounce(float time)
		{
			return 1 - easeOutBounce(1 - time);
		}

		private static float easeOutBounce(float time)
		{
			float n1 = 7.5625f;
			float d1 = 2.75f;
			if (time < 1f / d1)
			{
				return n1 * time * time;
			}
			else if (time < 2f / d1)
			{
				return n1 * (time -= 1.5f / d1) * time + 0.75f;
			}
			else if (time < 2.5f / d1)
			{
				return n1 * (time -= 2.25f / d1) * time + 0.9375f;
			}
			else
			{
				return n1 * (time -= 2.625f / d1) * time + 0.984375f;
			}
		}

		private static float easeInOutBounce(float time)
		{
			return time < 0.5f
			  ? (1f - easeOutBounce(1f - 2f * time)) / 2f
			  : (1f + easeOutBounce(2f * time - 1f)) / 2f;
		}

	}
}
