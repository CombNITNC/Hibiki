namespace Ruling {

  public class EventToModel {
    public delegate void Move(int X);
    public delegate void Manipulate();
    public delegate void Drop();

  }

  public class EventFromModel {
    public delegate void Spawn(Virus virus);
    public delegate void Change(Virus.Id id, Position from, Position to);
    public delegate void Absorb(Virus.Id eater, Virus.Id eaten);
    public delegate void Break(Virus.Id broken);
  }
}