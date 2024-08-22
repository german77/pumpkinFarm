
namespace PumpkinFarm {
    public class Vector2Int {
        public int x;
        public int y;

        public Vector2Int(int x = 0, int y = 0) {
            this.x = x;
            this.y = y;
        }

        public override string ToString() {
            return x.ToString() + ", " + y.ToString();
        }

        public override int GetHashCode() {
            return x + (y * 1000);
        }

        public override bool Equals(object? obj) {
            if (obj == null) return false;
            if (obj is not Vector2Int) return false;

            Vector2Int other = (Vector2Int)obj;
            return x == other.x && y == other.y;
        }
    }

    public class RectInt {
        public Vector2Int min = new();
        public Vector2Int max = new();

        public RectInt(Vector2Int min, Vector2Int size) {
            this.min = min;
            max = new Vector2Int(min.x + size.x, min.y + size.y);
        }

        public override string ToString() {
            return "min: " + min.ToString() + " max: " + max.ToString();
        }

        public override int GetHashCode() {
            return max.GetHashCode() * 1000000 + min.GetHashCode();
        }

        public override bool Equals(object? obj) {
            if (obj == null) return false;
            if (obj is not RectInt) return false;

            RectInt other = (RectInt)obj;
            return min == other.min && max == other.max;
        }
    }

    public class Pumpkin {
        public Vector2Int pos;

        public Pumpkin(Vector2Int pos) {
            this.pos = pos;
        }

        public override string ToString() {
            return pos.ToString();
        }

        public override int GetHashCode() {
            return pos.GetHashCode();
        }

        public override bool Equals(object? obj) {
            if (obj == null) return false;
            if (obj is not Pumpkin) return false;

            Pumpkin other = (Pumpkin)obj;
            return pos == other.pos;
        }
    }
}
