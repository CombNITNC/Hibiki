using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ruling {
  public interface BoardOperator {
    event EventToModel.Move Move;
    event EventToModel.Manipulate Manipulate;
    event EventToModel.Drop Drop;
  }

  public class Board {
    public static readonly int Width = 5, Height = 12;
    List<Virus> crowd = new List<Virus>(Width * Height);
    Holder holder = new Holder();
    System.Random rand = new System.Random();

    public Board(BoardOperator op) {
      op.Move += OnMove;
      op.Manipulate += OnManipulate;
      op.Drop += OnDrop;

      Spawn += OnSpawn;
      Change += OnChange;
      Absorb += OnAbsorb;
      Break += OnBreak;
    }

    public delegate bool PositionFilter(Position pos);

    public IEnumerable<Virus> VirusFromPosition(PositionFilter filter) {
      foreach (var v in crowd) {
        if (filter(v.VirusPosition)) {
          yield return v;
        }
      }
    }
    public IEnumerable<Virus> VirusFromId(Virus.Id id) {
      foreach (var v in crowd) {
        if (v.VirusId == id) {
          yield return v;
        }
      }
    }

    void OnMove(int X) {
      holder.X = X;
      if (holder.HeldId != Virus.Id.Null) {
        var toPlace = VirusFromId(holder.HeldId).First();
        Change.Invoke(toPlace.VirusId, Position.Hand(), Position.Hand());
      }
    }

    void OnManipulate() {
      if (holder.HeldId == Virus.Id.Null) {
        var column = VirusFromPosition(pos => pos.X == holder.X).OrderByDescending(v => v.VirusPosition.Y);
        if (column.Count() < 1) return;

        var toTake = column.First();
        holder.HeldId = toTake.VirusId;
        Change.Invoke(toTake.VirusId, toTake.VirusPosition, Position.Hand());
      } else {
        var toPlace = VirusFromId(holder.HeldId).First();
        var end = crowd.Select(v => v.VirusPosition)
          .Where(pos => pos.X == holder.X)
          .OrderByDescending(pos => pos.Y)
          .Select(pos => pos.Y + 1)
          .DefaultIfEmpty(1)
          .First();
        Change.Invoke(toPlace.VirusId, Position.Hand(), Position.OnBoard(holder.X, end));

        holder.HeldId = Virus.Id.Null;
      }
    }

    void OnDrop() {
      // Shift all virus down
      foreach (var v in crowd) {
        Change.Invoke(v.VirusId,
          v.VirusPosition,
          v.VirusPosition.WithY(v.VirusPosition.Y + 1)
        );
      }

      var grades = Enum.GetValues(typeof(Virus.Grade));
      var newRow = Enumerable.Range(1, Width).Select(i => {
        var v = new Virus();
        v.VirusPosition = Position.OnBoard(i, 1);
        v.VirusGrade = (Virus.Grade) grades.GetValue(rand.Next(grades.Length));
        return v;
      });
      foreach (var v in newRow) {
        Spawn.Invoke(v);
      }
    }

    void OnSpawn(Virus virus) {
      crowd.Add(virus);
    }

    void OnChange(Virus.Id id, Position from, Position to) {
      var changed = VirusFromId(id).First();
      changed.VirusPosition = to;

      if (to.Y <= 1 || from != Position.Hand()) return;
      var up = to.WithY(to.Y - 1);
      var upper = VirusFromPosition(pos => pos == up).First();
      if (!(
          (upper.VirusGrade == Virus.Grade.Big && changed.VirusGrade == Virus.Grade.Mid) ||
          (upper.VirusGrade == Virus.Grade.Mid && changed.VirusGrade == Virus.Grade.Tiny)
        )) return;
      Absorb.Invoke(upper.VirusId, changed.VirusId);
    }

    void OnAbsorb(Virus.Id eaterId, Virus.Id eatenId) {
      var eaten = VirusFromId(eatenId).First();
      crowd.Remove(eaten);

      var eater = VirusFromId(eaterId).First();
      if (eater.isCracked || eaten.isCracked) {
        Break.Invoke(eaterId);
      }
      eater.isCracked = true;
    }

    void OnBreak(Virus.Id brokenId) {
      var broken = VirusFromId(brokenId).First();
      crowd.Remove(broken);
    }

    public event EventFromModel.Spawn Spawn;
    public event EventFromModel.Change Change;
    public event EventFromModel.Absorb Absorb;
    public event EventFromModel.Break Break;
  }
}