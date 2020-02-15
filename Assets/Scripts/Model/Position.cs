using System;
namespace Ruling {
  public struct Position {
    public readonly int X, Y;

    public Position(int x, int y) {
      if (!(1 <= x && x <= Board.Width && 1 <= y && y <= Board.Height)) {
        throw new ArgumentOutOfRangeException();
      }
      X = x;
      Y = y;
    }

    public Position WithX(int newX) {
      return new Position(newX, Y);
    }
    public Position WithY(int newY) {
      return new Position(X, newY);
    }

    public override bool Equals(object obj) {
      return obj is Position position &&
        X == position.X &&
        Y == position.Y;
    }
    public override int GetHashCode() {
      int hashCode = 1861411795;
      hashCode = hashCode * -1521134295 + X.GetHashCode();
      hashCode = hashCode * -1521134295 + Y.GetHashCode();
      return hashCode;
    }
    public static bool operator ==(Position l, Position r) {
      return l.X == r.X && l.Y == r.Y;
    }
    public static bool operator !=(Position l, Position r) {
      return l.X != r.X || l.Y != r.Y;
    }
  }
}