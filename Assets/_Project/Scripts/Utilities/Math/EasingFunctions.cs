using UnityEngine;

public enum Ease {
  InQuad,
  OutQuad,
  InOutQuad,
  InCubic,
  OutCubic,
  InOutCubic,
  InQuart,
  OutQuart,
  InOutQuart,
  InQuint,
  OutQuint,
  InOutQuint,
  InSine,
  OutSine,
  InOutSine,
  InExpo,
  OutExpo,
  InOutExpo,
  InCirc,
  OutCirc,
  InOutCirc,
  Linear,
  Spring,
  InBounce,
  OutBounce,
  InOutBounce,
  InBack,
  OutBack,
  InOutBack,
  InElastic,
  OutElastic,
  InOutElastic,
}

public class VectorEasingFunctions {

  private const float NATURAL_LOG_OF_2 = 0.693147181f;

  #region (Vector2) Easing Functions
  public static Vector2 Linear(Vector2 start, Vector2 end, float value)
  {
    return Vector2.Lerp(start, end, value);
  }

  public static Vector2 Spring(Vector2 start, Vector2 end, float value)
  {
    value = Mathf.Clamp01(value);
    value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
    return start + (end - start) * value;
  }

  public static Vector2 EaseInQuad(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    return end * value * value + start;
  }

  public static Vector2 EaseOutQuad(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    return -end * value * (value - 2) + start;
  }

  public static Vector2 EaseInOutQuad(Vector2 start, Vector2 end, float value)
  {
    value /= .5f;
    end -= start;
    if (value < 1) return end * 0.5f * value * value + start;
    value--;
    return -end * 0.5f * (value * (value - 2) - 1) + start;
  }

  public static Vector2 EaseInCubic(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    return end * value * value * value + start;
  }

  public static Vector2 EaseOutCubic(Vector2 start, Vector2 end, float value)
  {
    value--;
    end -= start;
    return end * (value * value * value + 1) + start;
  }

  public static Vector2 EaseInOutCubic(Vector2 start, Vector2 end, float value)
  {
    value /= .5f;
    end -= start;
    if (value < 1) return end * 0.5f * value * value * value + start;
    value -= 2;
    return end * 0.5f * (value * value * value + 2) + start;
  }

  public static Vector2 EaseInQuart(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    return end * value * value * value * value + start;
  }

  public static Vector2 EaseOutQuart(Vector2 start, Vector2 end, float value)
  {
    value--;
    end -= start;
    return -end * (value * value * value * value - 1) + start;
  }

  public static Vector2 EaseInOutQuart(Vector2 start, Vector2 end, float value)
  {
    value /= .5f;
    end -= start;
    if (value < 1) return end * 0.5f * value * value * value * value + start;
    value -= 2;
    return -end * 0.5f * (value * value * value * value - 2) + start;
  }

  public static Vector2 EaseInQuint(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    return end * value * value * value * value * value + start;
  }

  public static Vector2 EaseOutQuint(Vector2 start, Vector2 end, float value)
  {
    value--;
    end -= start;
    return end * (value * value * value * value * value + 1) + start;
  }

  public static Vector2 EaseInOutQuint(Vector2 start, Vector2 end, float value)
  {
    value /= .5f;
    end -= start;
    if (value < 1) return end * 0.5f * value * value * value * value * value + start;
    value -= 2;
    return end * 0.5f * (value * value * value * value * value + 2) + start;
  }

  public static Vector2 EaseInSine(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
  }

  public static Vector2 EaseOutSine(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
  }

  public static Vector2 EaseInOutSine(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
  }

  public static Vector2 EaseInExpo(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    return end * Mathf.Pow(2, 10 * (value - 1)) + start;
  }

  public static Vector2 EaseOutExpo(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
  }

  public static Vector2 EaseInOutExpo(Vector2 start, Vector2 end, float value)
  {
    value /= .5f;
    end -= start;
    if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
    value--;
    return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
  }

  public static Vector2 EaseInCirc(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
  }

  public static Vector2 EaseOutCirc(Vector2 start, Vector2 end, float value)
  {
    value--;
    end -= start;
    return end * Mathf.Sqrt(1 - value * value) + start;
  }

  public static Vector2 EaseInOutCirc(Vector2 start, Vector2 end, float value)
  {
    value /= .5f;
    end -= start;
    if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
    value -= 2;
    return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
  }

  public static Vector2 EaseInBounce(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    float d = 1f;
    return end - EaseOutBounce(Vector2.zero, end, d - value) + start;
  }

  public static Vector2 EaseOutBounce(Vector2 start, Vector2 end, float value)
  {
    value /= 1f;
    end -= start;
    if (value < (1 / 2.75f))
    {
      return end * (7.5625f * value * value) + start;
    }
    else if (value < (2 / 2.75f))
    {
        value -= (1.5f / 2.75f);
      return end * (7.5625f * (value) * value + .75f) + start;
    }
    else if (value < (2.5 / 2.75))
    {
        value -= (2.25f / 2.75f);
      return end * (7.5625f * (value) * value + .9375f) + start;
    }
    else
    {
        value -= (2.625f / 2.75f);
      return end * (7.5625f * (value) * value + .984375f) + start;
    }
  }

  public static Vector2 EaseInOutBounce(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    float d = 1f;
    if (value < d * 0.5f) return EaseInBounce(Vector2.zero, end, value * 2) * 0.5f + start;
    else return EaseOutBounce(Vector2.zero, end, value * 2 - d) * 0.5f + end * 0.5f + start;
  }

  public static Vector2 EaseInBack(Vector2 start, Vector2 end, float value)
  {
    end -= start;
    value /= 1;
    float s = 1.70158f;
    return end * (value) * value * ((s + 1) * value - s) + start;
  }

  public static Vector2 EaseOutBack(Vector2 start, Vector2 end, float value)
  {
    float s = 1.70158f;
    end -= start;
    value = (value) - 1;
    return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
  }

  public static Vector2 EaseInOutBack(Vector2 start, Vector2 end, float value)
  {
    float s = 1.70158f;
    end -= start;
    value /= .5f;
    if ((value) < 1)
    {
      s *= (1.525f);
      return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
    }
    value -= 2;
    s *= (1.525f);
    return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
  }

  public static Vector2 EaseInElastic(Vector2 start, Vector2 end, float value)
  {
    Vector2 n = (end - start).normalized;
    return n * EaseInElasticFloat(start.magnitude, end.magnitude, value);
  }

  public static float EaseInElasticFloat(float start, float end, float value)
  {
    end -= start;

    float d = 1f;
    float p = d * .3f;
    float s;
    float a = 0;

    if (value == 0) return start;

    if ((value /= d) == 1) return start + end;

    if (a == 0f || a < Mathf.Abs(end))
    {
      a = end;
      s = p / 4;
    }
    else
    {
      s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
    }

    return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
  }

  public static Vector2 EaseOutElastic(Vector2 start, Vector2 end, float value)
  {
    Vector2 n = (end - start).normalized;
    return n * EaseOutElasticFloat(start.magnitude, end.magnitude, value);
  }
  
  public static float EaseOutElasticFloat(float start, float end, float value)
  {
    end -= start;

    float d = 1f;
    float p = d * .3f;
    float s;
    float a = 0;

    if (value == 0) return start;

    if ((value /= d) == 1) return start + end;

    if (a == 0f || a < Mathf.Abs(end))
    {
      a = end;
      s = p * 0.25f;
    }
    else
    {
      s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
    }

    return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
  }

  public static Vector2 EaseInOutElastic(Vector2 start, Vector2 end, float value)
  {
    Vector2 n = (end - start).normalized;
    return n * EaseInOutElasticFloat(start.magnitude, end.magnitude, value);
  }

  public static float EaseInOutElasticFloat(float start, float end, float value) {
    end -= start;

    float d = 1f;
    float p = d * .3f;
    float s;
    float a = 0;

    if (value == 0) return start;

    if ((value /= d * 0.5f) == 2) return start + end;

    if (a == 0f || a < Mathf.Abs(end))
    {
      a = end;
      s = p / 4;
    }
    else
    {
      s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
    }

    if (value < 1) 
      return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
    return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
  }
  #endregion

  public delegate Vector2 Function(Vector2 s, Vector2 e, float v);

  public static Function GetEasingFunction(Ease easingFunction) {
    if (easingFunction == Ease.InQuad) {
      return EaseInQuad;
    }

    if (easingFunction == Ease.OutQuad) {
      return EaseOutQuad;
    }

    if (easingFunction == Ease.InOutQuad) {
      return EaseInOutQuad;
    }

    if (easingFunction == Ease.InCubic) {
      return EaseInCubic;
    }

    if (easingFunction == Ease.OutCubic) {
      return EaseOutCubic;
    }

    if (easingFunction == Ease.InOutCubic) {
      return EaseInOutCubic;
    }

    if (easingFunction == Ease.InQuart) {
      return EaseInQuart;
    }

    if (easingFunction == Ease.OutQuart) {
      return EaseOutQuart;
    }

    if (easingFunction == Ease.InOutQuart) {
      return EaseInOutQuart;
    }

    if (easingFunction == Ease.InQuint) {
      return EaseInQuint;
    }

    if (easingFunction == Ease.OutQuint) {
      return EaseOutQuint;
    }

    if (easingFunction == Ease.InOutQuint) {
      return EaseInOutQuint;
    }

    if (easingFunction == Ease.InSine) {
      return EaseInSine;
    }

    if (easingFunction == Ease.OutSine) {
      return EaseOutSine;
    }

    if (easingFunction == Ease.InOutSine) {
      return EaseInOutSine;
    }

    if (easingFunction == Ease.InExpo) {
      return EaseInExpo;
    }

    if (easingFunction == Ease.OutExpo) {
      return EaseOutExpo;
    }

    if (easingFunction == Ease.InOutExpo) {
      return EaseInOutExpo;
    }

    if (easingFunction == Ease.InCirc) {
      return EaseInCirc;
    }

    if (easingFunction == Ease.OutCirc) {
      return EaseOutCirc;
    }

    if (easingFunction == Ease.InOutCirc) {
      return EaseInOutCirc;
    }

    if (easingFunction == Ease.Linear) {
      return Linear;
    }

    if (easingFunction == Ease.Spring) {
      return Spring;
    }

    if (easingFunction == Ease.InBounce) {
      return EaseInBounce;
    }

    if (easingFunction == Ease.OutBounce) {
      return EaseOutBounce;
    }

    if (easingFunction == Ease.InOutBounce) {
      return EaseInOutBounce;
    }

    if (easingFunction == Ease.InBack) {
      return EaseInBack;
    }

    if (easingFunction == Ease.OutBack) {
      return EaseOutBack;
    }

    if (easingFunction == Ease.InOutBack) {
      return EaseInOutBack;
    }

    if (easingFunction == Ease.InElastic) {
      return EaseInElastic;
    }

    if (easingFunction == Ease.OutElastic) {
      return EaseOutElastic;
    }

    if (easingFunction == Ease.InOutElastic) {
      return EaseInOutElastic;
    }

    return null;
  }
}


public class EasingFunctions {

  private const float NATURAL_LOG_OF_2 = 0.693147181f;

  #region (Float) Easing Functions
  public static float Linear(float start, float end, float value)
  {
    return Mathf.Lerp(start, end, value);
  }

  public static float Spring(float start, float end, float value)
  {
    value = Mathf.Clamp01(value);
    value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
    return start + (end - start) * value;
  }

  public static float EaseInQuad(float start, float end, float value)
  {
    end -= start;
    return end * value * value + start;
  }

  public static float EaseOutQuad(float start, float end, float value)
  {
    end -= start;
    return -end * value * (value - 2) + start;
  }

  public static float EaseInOutQuad(float start, float end, float value)
  {
    value /= .5f;
    end -= start;
    if (value < 1) return end * 0.5f * value * value + start;
    value--;
    return -end * 0.5f * (value * (value - 2) - 1) + start;
  }

  public static float EaseInCubic(float start, float end, float value)
  {
    end -= start;
    return end * value * value * value + start;
  }

  public static float EaseOutCubic(float start, float end, float value)
  {
    value--;
    end -= start;
    return end * (value * value * value + 1) + start;
  }

  public static float EaseInOutCubic(float start, float end, float value)
  {
    value /= .5f;
    end -= start;
    if (value < 1) return end * 0.5f * value * value * value + start;
    value -= 2;
    return end * 0.5f * (value * value * value + 2) + start;
  }

  public static float EaseInQuart(float start, float end, float value)
  {
    end -= start;
    return end * value * value * value * value + start;
  }

  public static float EaseOutQuart(float start, float end, float value)
  {
    value--;
    end -= start;
    return -end * (value * value * value * value - 1) + start;
  }

  public static float EaseInOutQuart(float start, float end, float value)
  {
    value /= .5f;
    end -= start;
    if (value < 1) return end * 0.5f * value * value * value * value + start;
    value -= 2;
    return -end * 0.5f * (value * value * value * value - 2) + start;
  }

  public static float EaseInQuint(float start, float end, float value)
  {
    end -= start;
    return end * value * value * value * value * value + start;
  }

  public static float EaseOutQuint(float start, float end, float value)
  {
    value--;
    end -= start;
    return end * (value * value * value * value * value + 1) + start;
  }

  public static float EaseInOutQuint(float start, float end, float value)
  {
    value /= .5f;
    end -= start;
    if (value < 1) return end * 0.5f * value * value * value * value * value + start;
    value -= 2;
    return end * 0.5f * (value * value * value * value * value + 2) + start;
  }

  public static float EaseInSine(float start, float end, float value)
  {
    end -= start;
    return -end * Mathf.Cos(value * (Mathf.PI * 0.5f)) + end + start;
  }

  public static float EaseOutSine(float start, float end, float value)
  {
    end -= start;
    return end * Mathf.Sin(value * (Mathf.PI * 0.5f)) + start;
  }

  public static float EaseInOutSine(float start, float end, float value)
  {
    end -= start;
    return -end * 0.5f * (Mathf.Cos(Mathf.PI * value) - 1) + start;
  }

  public static float EaseInExpo(float start, float end, float value)
  {
    end -= start;
    return end * Mathf.Pow(2, 10 * (value - 1)) + start;
  }

  public static float EaseOutExpo(float start, float end, float value)
  {
    end -= start;
    return end * (-Mathf.Pow(2, -10 * value) + 1) + start;
  }

  public static float EaseInOutExpo(float start, float end, float value)
  {
    value /= .5f;
    end -= start;
    if (value < 1) return end * 0.5f * Mathf.Pow(2, 10 * (value - 1)) + start;
    value--;
    return end * 0.5f * (-Mathf.Pow(2, -10 * value) + 2) + start;
  }

  public static float EaseInCirc(float start, float end, float value)
  {
    end -= start;
    return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
  }

  public static float EaseOutCirc(float start, float end, float value)
  {
    value--;
    end -= start;
    return end * Mathf.Sqrt(1 - value * value) + start;
  }

  public static float EaseInOutCirc(float start, float end, float value)
  {
    value /= .5f;
    end -= start;
    if (value < 1) return -end * 0.5f * (Mathf.Sqrt(1 - value * value) - 1) + start;
    value -= 2;
    return end * 0.5f * (Mathf.Sqrt(1 - value * value) + 1) + start;
  }

  public static float EaseInBounce(float start, float end, float value)
  {
    end -= start;
    float d = 1f;
    return end - EaseOutBounce(0, end, d - value) + start;
  }

  public static float EaseOutBounce(float start, float end, float value)
  {
    value /= 1f;
    end -= start;
    if (value < (1 / 2.75f))
    {
      return end * (7.5625f * value * value) + start;
    }
    else if (value < (2 / 2.75f))
    {
        value -= (1.5f / 2.75f);
      return end * (7.5625f * (value) * value + .75f) + start;
    }
    else if (value < (2.5 / 2.75))
    {
        value -= (2.25f / 2.75f);
      return end * (7.5625f * (value) * value + .9375f) + start;
    }
    else
    {
        value -= (2.625f / 2.75f);
      return end * (7.5625f * (value) * value + .984375f) + start;
    }
  }

  public static float EaseInOutBounce(float start, float end, float value)
  {
    end -= start;
    float d = 1f;
    if (value < d * 0.5f) return EaseInBounce(0, end, value * 2) * 0.5f + start;
    else return EaseOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
  }

  public static float EaseInBack(float start, float end, float value)
  {
    end -= start;
    value /= 1;
    float s = 1.70158f;
    return end * (value) * value * ((s + 1) * value - s) + start;
  }

  public static float EaseOutBack(float start, float end, float value)
  {
    float s = 1.70158f;
    end -= start;
    value = (value) - 1;
    return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
  }

  public static float EaseInOutBack(float start, float end, float value)
  {
    float s = 1.70158f;
    end -= start;
    value /= .5f;
    if ((value) < 1)
    {
      s *= (1.525f);
      return end * 0.5f * (value * value * (((s) + 1) * value - s)) + start;
    }
    value -= 2;
    s *= (1.525f);
    return end * 0.5f * ((value) * value * (((s) + 1) * value + s) + 2) + start;
  }

  public static float EaseInElastic(float start, float end, float value)
  {
    end -= start;

    float d = 1f;
    float p = d * .3f;
    float s;
    float a = 0;

    if (value == 0) return start;

    if ((value /= d) == 1) return start + end;

    if (a == 0f || a < Mathf.Abs(end))
    {
      a = end;
      s = p / 4;
    }
    else
    {
      s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
    }

    return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
  }
  
  public static float EaseOutElastic(float start, float end, float value)
  {
    end -= start;

    float d = 1f;
    float p = d * .3f;
    float s;
    float a = 0;

    if (value == 0) return start;

    if ((value /= d) == 1) return start + end;

    if (a == 0f || a < Mathf.Abs(end))
    {
      a = end;
      s = p * 0.25f;
    }
    else
    {
      s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
    }

    return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
  }

  public static float EaseInOutElastic(float start, float end, float value) {
    end -= start;

    float d = 1f;
    float p = d * .3f;
    float s;
    float a = 0;

    if (value == 0) return start;

    if ((value /= d * 0.5f) == 2) return start + end;

    if (a == 0f || a < Mathf.Abs(end))
    {
      a = end;
      s = p / 4;
    }
    else
    {
      s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
    }

    if (value < 1) 
      return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
    return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
  }
  #endregion

  public delegate float Function(float s, float e, float v);

  public static Function GetEasingFunction(Ease easingFunction) {
    if (easingFunction == Ease.InQuad) {
      return EaseInQuad;
    }

    if (easingFunction == Ease.OutQuad) {
      return EaseOutQuad;
    }

    if (easingFunction == Ease.InOutQuad) {
      return EaseInOutQuad;
    }

    if (easingFunction == Ease.InCubic) {
      return EaseInCubic;
    }

    if (easingFunction == Ease.OutCubic) {
      return EaseOutCubic;
    }

    if (easingFunction == Ease.InOutCubic) {
      return EaseInOutCubic;
    }

    if (easingFunction == Ease.InQuart) {
      return EaseInQuart;
    }

    if (easingFunction == Ease.OutQuart) {
      return EaseOutQuart;
    }

    if (easingFunction == Ease.InOutQuart) {
      return EaseInOutQuart;
    }

    if (easingFunction == Ease.InQuint) {
      return EaseInQuint;
    }

    if (easingFunction == Ease.OutQuint) {
      return EaseOutQuint;
    }

    if (easingFunction == Ease.InOutQuint) {
      return EaseInOutQuint;
    }

    if (easingFunction == Ease.InSine) {
      return EaseInSine;
    }

    if (easingFunction == Ease.OutSine) {
      return EaseOutSine;
    }

    if (easingFunction == Ease.InOutSine) {
      return EaseInOutSine;
    }

    if (easingFunction == Ease.InExpo) {
      return EaseInExpo;
    }

    if (easingFunction == Ease.OutExpo) {
      return EaseOutExpo;
    }

    if (easingFunction == Ease.InOutExpo) {
      return EaseInOutExpo;
    }

    if (easingFunction == Ease.InCirc) {
      return EaseInCirc;
    }

    if (easingFunction == Ease.OutCirc) {
      return EaseOutCirc;
    }

    if (easingFunction == Ease.InOutCirc) {
      return EaseInOutCirc;
    }

    if (easingFunction == Ease.Linear) {
      return Linear;
    }

    if (easingFunction == Ease.Spring) {
      return Spring;
    }

    if (easingFunction == Ease.InBounce) {
      return EaseInBounce;
    }

    if (easingFunction == Ease.OutBounce) {
      return EaseOutBounce;
    }

    if (easingFunction == Ease.InOutBounce) {
      return EaseInOutBounce;
    }

    if (easingFunction == Ease.InBack) {
      return EaseInBack;
    }

    if (easingFunction == Ease.OutBack) {
      return EaseOutBack;
    }

    if (easingFunction == Ease.InOutBack) {
      return EaseInOutBack;
    }

    if (easingFunction == Ease.InElastic) {
      return EaseInElastic;
    }

    if (easingFunction == Ease.OutElastic) {
      return EaseOutElastic;
    }

    if (easingFunction == Ease.InOutElastic) {
      return EaseInOutElastic;
    }

    return null;
  }
}