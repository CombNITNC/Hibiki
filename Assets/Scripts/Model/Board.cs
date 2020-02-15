using System;
using System.Collections.Generic;
using System.Linq;

namespace Ruling {
  public interface BoardOperator {
    void OnChange(Virus.Id id, Position from, Position to);
    void OnAbsorb(Virus.Id eater, Virus.Id eaten);
    void OnBreak(Virus.Id broken);

    event EventToModel.Move Move;
    event EventToModel.Manipulate Manipulate;
    event EventToModel.Drop Drop;
  }

  public class Board {
    public static readonly int Width = 6, Height = 12;
    List<Virus> crowd;
    Holder holder;

    public Board(BoardOperator op) {

      op.Move += OnMove;
      op.Manipulate += OnManipulate;
      op.Drop += OnDrop;

      Change += OnChange;
      Change += op.OnChange;
      Absorb += op.OnAbsorb;
      Absorb += OnAbsorb;
      Break += op.OnBreak;
      Break += OnBreak;
    }

    public delegate bool PositionFilter(Position pos);

    IEnumerable<Virus> VirusFromPosition(PositionFilter filter) {
      foreach (var v in crowd) {
        if (filter(v.VirusPosition)) {
          yield return v;
        }
      }
    }
    IEnumerable<Virus> VirusFromId(Virus.Id id) {
      foreach (var v in crowd) {
        if (v.VirusId == id) {
          yield return v;
        }
      }
    }

    void OnMove(int X) {
      holder.X = X;
    }

    void OnManipulate() {
      if (holder.HeldId == Virus.Id.Null) {
        var toTake = VirusFromPosition(pos => pos.X == holder.X).Max(v => v.VirusPosition.Y);
      } else {

      }
    }

    void OnDrop() {

    }

    void OnChange(Virus.Id id, Position from, Position to) {

    }

    void OnAbsorb(Virus.Id eater, Virus.Id eaten) {

    }

    void OnBreak(Virus.Id broken) {

    }

    event EventFromModel.Change Change;
    event EventFromModel.Absorb Absorb;
    event EventFromModel.Break Break;
  }
}