
using System.Drawing;

namespace PumpkinFarm {
    public class PumpkinController {
        public readonly int size;

        private Random rand = new Random();
        private Queue<Vector2Int> fillingSequence;
        private RectInt[] groups;
        private int[] groupLookup;
        private int[] dp;
        private int pCount;
        private int gCount;
        private int dpMax;

        public PumpkinController(int size) {
            this.size = size;
            this.fillingSequence = NewFillingSequence();
            dp = new int[size * size];
            groupLookup = new int[size * size];
            groups = new RectInt[size * size / 4];
            Clear();
        }

        private bool HasPumpkin(Vector2Int pos) {
            return dp[pos.x + pos.y * size] > 0;
        }

        private bool HasGroup(Vector2Int pos) {
            return groupLookup[pos.x + pos.y * size] != -1;
        }

        public void AddPumpkin(Pumpkin p) {
            Vector2Int pos = p.pos;
            if (HasPumpkin(pos)) {
                throw new Exception("can't add pumpkin to an occupied spot");
            }
            dp[pos.x + (pos.y * size)] = 1;
            pCount++;
            MergeWithOthers(pos);
        }

        public void FillAll() {
            while (FillNext()) ;
        }

        public bool FillNext() {
            if (pCount >= size * size) {
                return false;
            }

            Pumpkin p = new Pumpkin(fillingSequence.Dequeue());
            AddPumpkin(p);

            return true;
        }

        public void Clear() {
            fillingSequence = NewFillingSequence();
            Array.Clear(dp);
            Array.Clear(groups);
            Array.Fill(groupLookup, -1);
            pCount = 0;
            gCount = 0;
            dpMax = 0;
        }

        private Queue<Vector2Int> NewFillingSequence() {
            return new Queue<Vector2Int>(
                Enumerable.Range(0, size)
                    .SelectMany((l) => Enumerable.Range(0, size), (l, r) => new Vector2Int(l, r))
                    .OrderBy((item) => rand.Next())
            );
        }

        private void MergeWithOthers(Vector2Int pos) {
            bool flag = true;
            int squareSize = 1;

            while (flag) {
                squareSize++;
                flag = false;
                for (int x = pos.x - squareSize; x < size - 1 && x <= pos.x; x++) {
                    if (x < 0) {
                        x = 0;
                    }
                    for (int y = pos.y - squareSize; y < size - 1 && y <= pos.y; y++) {
                        if (y < 0) {
                            y = 0;
                        }
                        if (dp[x + (y * size)] == squareSize - 1 &&
                            dp[x + 1 + size * y] >= squareSize - 1 &&
                            dp[x + size * (y + 1)] >= squareSize - 1 &&
                            dp[x + 1 + size * (y + 1)] >= squareSize - 1) {
                            dp[x + (y * size)] = squareSize;
                            flag = true;
                        }
                    }
                }
            }

            squareSize--;

            if (dpMax < squareSize) {
                dpMax = squareSize;
            }

            RectInt val = LargestSquare(squareSize, pos);
            if (val.min.x == val.max.x && val.min.y == val.max.y) {
                return;
            }
            groups[gCount] = val;
            foreach (Vector2Int item in IterPositions(val)) {
                groupLookup[item.x + item.y * size] = gCount;
            }
            gCount++;
        }

        private RectInt LargestSquare(int squareSize, Vector2Int pos) {
            while (squareSize > 1) {
                for (int x = pos.x - squareSize; x < size && x <= pos.x; x++) {
                    if (x < 0) {
                        x = 0;
                    }
                    for (int y = pos.y - squareSize; y < size && y <= pos.y; y++) {
                        if (y < 0) {
                            y = 0;
                        }
                        RectInt val = new(new Vector2Int(x, y), new Vector2Int(squareSize - 1, squareSize - 1));
                        if (dp[x + (y * size)] >= squareSize && IsInRect(val, pos) && !HasOverlaps(val)) {
                            return val;
                        }
                    }
                }
                squareSize--;
            }
            return new RectInt(pos, new Vector2Int());
        }

        private bool HasOverlaps(RectInt r) {
            foreach (Vector2Int item in IterBorders(r)) {
                if (!HasPumpkin(item)) {
                    continue;
                }
                if (!HasGroup(item)) {
                    continue;
                }
                RectInt val = groups[groupLookup[item.x + item.y * size]];
                if (val.max.x <= r.max.x) {
                    if (val.max.y <= r.max.y) {
                        if (val.min.x >= r.min.x) {
                            if (val.min.y >= r.min.y) {
                                continue;
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private bool IsInRect(RectInt r, Vector2Int pos) {
            if (pos.x >= r.min.x && pos.y >= r.min.y && pos.x <= r.max.x) {
                return pos.y <= r.max.y;
            }
            return false;
        }

        private IEnumerable<Vector2Int> IterPositions(RectInt r) {
            for (int i = r.min.x; i <= r.max.x; i++) {
                for (int j = r.min.y; j <= r.max.y; j++) {
                    yield return new Vector2Int(i, j);
                }
            }
        }

        private IEnumerable<Vector2Int> IterBorders(RectInt r) {
            for (int i = r.min.x; i <= r.max.x; i++) {
                yield return new Vector2Int(i, r.min.y);
                yield return new Vector2Int(i, r.max.y);
            }
            for (int j = r.min.y; j <= r.max.y; j++) {
                yield return new Vector2Int(r.min.x, j);
                yield return new Vector2Int(r.max.x, j);
            }
        }

        public IEnumerable<Vector2Int> GetPumkings() {
            for (int x = 0; x < size; x++) {
                for (int y = 0; y < size; y++) {
                    if (dp[x + y * size] == 0) {
                        continue;
                    }
                    yield return new Vector2Int(x, y);
                }
            }
        }

        public RectInt[] GetGroups() {
            return groups;
        }

        public int[] GetDp() {
            return dp;
        }
    }
}
