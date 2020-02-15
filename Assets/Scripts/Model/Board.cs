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
    }

    void OnManipulate() {
      if (holder.HeldId == Virus.Id.Null) {
        var toTake = VirusFromPosition(pos => pos.X == holder.X).Max(v => v.VirusPosition.Y);
      } else {

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

      var rand = new System.Random();
      var grades = Enum.GetValues(typeof(Virus.Grade));
      var newRow = Enumerable.Range(1, Width).Select(i => {
        var v = new Virus();
        v.VirusPosition = new Position(i, 1);
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
      foreach (var found in VirusFromId(id)) {
        found.VirusPosition = to;
      }
    }

    void OnAbsorb(Virus.Id eater, Virus.Id eaten) {

    }

    void OnBreak(Virus.Id broken) {

    }

    public event EventFromModel.Spawn Spawn;
    public event EventFromModel.Change Change;
    public event EventFromModel.Absorb Absorb;
    public event EventFromModel.Break Break;
  }
}