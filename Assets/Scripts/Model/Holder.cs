using System;
namespace Ruling {
  public class Holder {
    int x = 3;

    public int X {
      get { return x; }
      set {
        x = Math.Min(Math.Max(value, 1), Board.Width);
      }
    }

    public Virus.Id HeldId = Virus.Id.Null;
  }
}