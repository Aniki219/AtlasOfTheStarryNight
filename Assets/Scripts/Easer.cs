using UnityEngine;
using System.Threading.Tasks;

public class Easer {
  private float duration;
  private Vector2 start;
  public Vector2 end { get; private set; }
  private float time;
  private Ease easingFunction;
  private bool shouldPause = false;

  public bool isComplete {get; private set;} = false;

  public Easer(Vector2 start, Vector2 end, float duration, Ease easingFunction = Ease.Linear) {
    this.start = start;
    this.end = end;
    this.duration = duration;
    this.easingFunction = easingFunction;
    time = 0;
  }

  // async void startEase() {
  //   float startTime = getTime();
  //   float time = startTime;
  //   while(time < duration) {
  //     time = (getTime() - startTime) / duration;
  //     EasingFunctions.GetEasingFunction(easingFunction)(start, end, time);
  //     await Task.Delay(16); // one frame
  //   }
  // }

  public Vector2 Update() {
    if (isComplete) return end;

    if (time >= duration) {
      time = duration;
      isComplete = true;
    }
    time += shouldPause ? Time.deltaTime : Time.unscaledDeltaTime;

    return EasingFunctions.GetEasingFunction(easingFunction)(start, end, time/duration);
  }

  private float getTime() {
    return shouldPause ? Time.time : Time.unscaledTime;
  }
}