using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    public void Where(Position position, Action<Virus> then) {
      var founds = VirusFromPosition(pos => pos == position);
      if (founds.Count() <= 0) return;
      var found = founds.First();
      then(found);
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
        if (v.VirusPosition.IsHand) continue;
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

      // Pull up
      if (1 < to.Y && to.Y < from.Y) {
        Where(to.WithY(to.Y + 1), downer => Change.Invoke(downer.VirusId, downer.VirusPosition, to));
      }

      // Place
      if (1 < to.Y && from == Position.Hand()) {
        Where(to.WithY(to.Y - 1), upper => {
          if (!(
              (upper.VirusGrade == Virus.Grade.Big && changed.VirusGrade == Virus.Grade.Mid) ||
              (upper.VirusGrade == Virus.Grade.Mid && changed.VirusGrade == Virus.Grade.Tiny)
            )) return;
          Absorb.Invoke(upper.VirusId, changed.VirusId);
        });
      }
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
      var brokenPos = broken.VirusPosition;

      if (1 < brokenPos.Y) {
        Where(brokenPos.WithY(brokenPos.Y - 1), upper => {
          if (upper.VirusGrade == broken.VirusGrade) {
            var chain = Chain(upper.VirusId);
          }
        });
      }
      if (1 < brokenPos.X) {
        Where(brokenPos.WithX(brokenPos.X - 1), lefter => {
          if (lefter.VirusGrade == broken.VirusGrade) {
            var chain = Chain(lefter.VirusId);
          }
        });
      }
      if (brokenPos.X < Width) {
        Where(brokenPos.WithX(brokenPos.X + 1), righter => {
          if (righter.VirusGrade == broken.VirusGrade) {
            var chain = Chain(righter.VirusId);
          }
        });
      }
      Where(brokenPos.WithY(brokenPos.Y + 1), downer =>
        Change.Invoke(downer.VirusId, downer.VirusPosition, brokenPos)
      );

      crowd.Remove(broken);
    }

    async Task Chain(Virus.Id toBreak) {
      await Task.Delay(130);
      Break.Invoke(toBreak);
    }

    public event EventFromModel.Spawn Spawn;
    public event EventFromModel.Change Change;
    public event EventFromModel.Absorb Absorb;
    public event EventFromModel.Break Break;
  }
}