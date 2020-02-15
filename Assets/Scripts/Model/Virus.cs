namespace Ruling {
  public class Virus {
    public class Id {
      static int incremental = 0;
      int id;

      public static readonly Id Null = new Id();

      public Id() {
        id = incremental;
        ++incremental;
      }

      public override bool Equals(object obj) {
        return obj is Id id &&
          this.id == id.id;
      }
      public override int GetHashCode() {
        return 1877310944 + id.GetHashCode();
      }
      public static bool operator ==(Id l, Id r) {
        return l.id == r.id;
      }
      public static bool operator !=(Id l, Id r) {
        return l.id != r.id;
      }
    }

    public readonly Id VirusId = new Id();

    public enum Grade {
      Tiny,
      Mid,
      Big
    }

    public Grade VirusGrade;
    public Position VirusPosition;
    public bool isCracked = false;
  }
}