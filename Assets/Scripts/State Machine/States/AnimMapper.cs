using UnityEngine;
using System;
using TypeReferences;
using System.Collections.Generic;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "AnimMapper")]
public class AnimMapper : ScriptableObject {
  private static AnimMapper instance;
  public static AnimMapper Instance { get { return instance; } }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        instance = Resources.LoadAll<AnimMapper>("Managers")[0];
    }

  public List<StateAnim> stateAnims;

  [Serializable]
  public struct StateAnim {
    [ClassExtends(typeof(State), Grouping = ClassGrouping.ByNamespace)]
    public ClassTypeReference state;
    public AnimationClip startAnim;
    public AnimationClip updateAnim;
    public AnimationClip exitAnim;
  }

  public static AnimationClip getClip<T>(ClipType clipType = ClipType.StartClip) where T : State {
    StateAnim stateAnim = Instance.stateAnims.Find(s => s.state.Type.Equals(typeof(T)));
    if (stateAnim.state == null) throw new Exception("No stateAnim data found for state: " + typeof(T));
    
    if (clipType.Equals(ClipType.ExitClip)) return stateAnim.exitAnim;
    return clipType.Equals(ClipType.StartClip) ? stateAnim.startAnim : stateAnim.updateAnim;
  }

  public static Task awaitClip<T>(ClipType clipType = ClipType.StartClip) where T : State {
    AnimationClip clip = getClip<T>(clipType);
    return AtlasHelpers.WaitSeconds(clip.length);
  }

  public enum ClipType {
    StartClip,
    UpdateClip,
    ExitClip
  }
}