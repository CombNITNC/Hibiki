using System;
namespace Ruling {
  public struct Position {
    public readonly int X;
    public readonly int Y;
    public readonly bool IsHand;

    private Position(int x, int y, bool isHand) {
      X = x;
      Y = y;
      IsHand = isHand;
    }
    public static Position OnBoard(int x, int y) {
      if (!(1 <= x && x <= Board.Width && 1 <= y && y <= Board.Height)) {
        throw new ArgumentOutOfRangeException();
      }
      return new Position(x, y, false);
    }
    public static Position Hand() {
      return new Position(0, 0, true);
    }

    public Position WithX(int newX) {
      return Position.OnBoard(newX, Y);
    }
    public Position WithY(int newY) {
      return Position.OnBoard(X, newY);
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