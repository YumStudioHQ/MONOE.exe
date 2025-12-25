using Godot;

namespace monoe.exe.Core.Bridge.Types;

public class Animation2D : Exposable
{
  protected AnimatedSprite2D animatedSprite;

  /* Features
   *    * NewAnimation(string name)
   *    * Play(string name)
   *    * Remove(string name)
   *    * Loop(string name)
   *    * From Images(Image[]) (Marshal: object[])
   *    * From Files(string[]) (Marshal: object[])
   *    * NRef()
   *    * SetImageAt(string animation, UID(:Image) image, long pos)
   */
}
