using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public abstract class IEaser<T> {
  protected float duration;
  protected T start;
  public T end { get; protected set; }
  
  protected Ease easingFunction;
  protected bool shouldPause = false;

  public bool isComplete {get; protected set;} = false;
  
  protected float getTime() {
    return shouldPause ? Time.time : Time.unscaledTime;
  }

  public abstract T Update();
}

#region Float Easers
public class Easer : IEaser<float> {
  private float time;

  public Easer(float start, float end, float duration, Ease easingFunction = Ease.Linear) {
    this.start = start;
    this.end = end;
    this.duration = duration;
    this.easingFunction = easingFunction;
    time = 0;
  }

  public override float Update() {
    if (isComplete) return end;

    if (time >= duration) {
      time = duration;
      isComplete = true;
    }
    time += shouldPause ? Time.deltaTime : Time.unscaledDeltaTime;
    return EasingFunctions.GetEasingFunction(easingFunction)(start, end, time/duration); 
  }
}

public class CompositeEaser : IEaser<float> {
  Queue<Easer> easers;
  Easer currentEaser;

  public CompositeEaser(Easer firstEaser, params Easer[] additionalEasers) {
    easers = new Queue<Easer>();
    easers.Enqueue(firstEaser);
    foreach(Easer e in additionalEasers) {
      easers.Enqueue(e);
    }
    next();
  }

  public override float Update() {

    if (isComplete) return currentEaser.end;

    if (currentEaser.isComplete) {
      next();
    }
    if (isComplete) return currentEaser.end;

    return currentEaser.Update(); 
  }

  private void next() {
    if (easers.Count > 0) {
      currentEaser = easers.Dequeue();
    } else {
      isComplete = true;
    }
  }
}

// public class AsyncEaser : IEaser<float> {
//   float value;

//   public AsyncEaser(ref float value, float start, float end, float duration, Ease easingFunction = Ease.Linear) {
//     this.value = value;
//     this.start = start;
//     this.end = end;
//     this.duration = duration;
//     this.easingFunction = easingFunction;
//   }

//   public async Task performEase() {
//     float startTime = getTime();
//     float elapsedTime = 0;
//     while(elapsedTime < duration) {
//       elapsedTime = (getTime() - startTime);
//       value = EasingFunctions.GetEasingFunction(easingFunction)(start, end, elapsedTime/duration);
//       Debug.Log(value);
//       await Task.Delay(16); // one frame
//     } 
//     isComplete = true;
//   }
// }
#endregion

#region Vector Easers
public class VectorEaser : IEaser<Vector2> {
  private float time;

  public VectorEaser(Vector2 start, Vector2 end, float duration, Ease easingFunction = Ease.Linear) {
    this.start = start;
    this.end = end;
    this.duration = duration;
    this.easingFunction = easingFunction;
    time = 0;
  }

  public override Vector2 Update() {
    if (isComplete) return end;

    if (time >= duration) {
      time = duration;
      isComplete = true;
    }
    time += shouldPause ? Time.deltaTime : Time.unscaledDeltaTime;
    return VectorEasingFunctions.GetEasingFunction(easingFunction)(start, end, time/duration); 
  }
}

// public class VectorAsyncEaser : IEaser<Vector2> {
//   Vector2 value;

//   public VectorAsyncEaser(ref Vector2 value, Vector2 start, Vector2 end, float duration, Ease easingFunction = Ease.Linear) {
//     this.value = value;
//     this.start = start;
//     this.end = end;
//     this.duration = duration;
//     this.easingFunction = easingFunction;
//   }

//   public async Task performEase() {
//     float startTime = getTime();
//     float elapsedTime = startTime;
//     while(elapsedTime < duration) {
//       elapsedTime = (getTime() - startTime);
//       value = VectorEasingFunctions.GetEasingFunction(easingFunction)(start, end, elapsedTime/duration);
//       await Task.Delay(16); // one frame
//     }
//     isComplete = true;
//   }
// }
#endregion
