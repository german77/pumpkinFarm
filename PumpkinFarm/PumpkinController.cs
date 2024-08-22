
namespace PumpkinFarm {
    public class PumpkinController {
        public readonly int size;

        private Random rand = new Random();
        private Queue<Vector2Int> fillingSequence;
        private Dictionary<Vector2Int, Pumpkin> pumpkins = [];
        private Dictionary<Pumpkin, RectInt> groups = [];
        private int[] dp;

        public PumpkinController(int size) {
            this.size = size;
            this.fillingSequence = NewFillingSequence();
            dp = new int[size * size];
            Clear();
        }

        public void AddPumpkin(Pumpkin p) {
            Vector2Int val = p.pos;
            if (pumpkins.ContainsKey(val)) {
                throw new Exception("can't add pumpkin to an occupied spot");
            }
            pumpkins[val] = p;
            MergeWithOthers(val);
        }

        public void FillAll() {
            while (FillNext()) ;
        }

        public bool FillNext() {
            if (pumpkins.Count >= size * size) {
                return false;
            }

            Pumpkin p = new Pumpkin(fillingSequence.Dequeue());
            AddPumpkin(p);

            return true;
        }

        public void Clear() {
            fillingSequence = NewFillingSequence();
            pumpkins.Clear();
            groups.Clear();
            Array.Clear(dp, 0, dp.Length);
        }

        private Queue<Vector2Int> NewFillingSequence() {
            return new Queue<Vector2Int>(
                Enumerable.Range(0, size)
                    .SelectMany((l) => Enumerable.Range(0, size), (l, r) => new Vector2Int(l, r))
                    .OrderBy((item) => rand.Next())
            );
        }

        private void MergeWithOthers(Vector2Int pos) {
            dp[pos.x + (pos.y * size)] = 1;

            bool flag = true;
            int squareSize = 1;
            while (flag) {
                squareSize++;
                flag = false;
                for (int x = pos.x - squareSize; x < size - 1 && x < pos.x + squareSize; x++) {
                    if (x < 0) {
                        x = 0;
                    }
                    for (int y = pos.y - squareSize; y < size - 1 && y < pos.y + squareSize; y++) {
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
            RectInt val = LargestSquare(size, squareSize, pos);
            foreach (Vector2Int item in IterPositions(val)) {
                if (!pumpkins.ContainsKey(item)) {
                    continue;
                }
                groups[pumpkins[item]] = val;
            }
        }

        private RectInt LargestSquare(int n, int squareSize, Vector2Int pos) {
            while (squareSize > 1) {
                for (int x = pos.x - squareSize; x < size && x < pos.x + squareSize; x++) {
                    if (x < 0) {
                        x = 0;
                    }
                    for (int y = pos.y - squareSize; y < size && y < pos.y + squareSize; y++) {
                        if (y < 0) {
                            y = 0;
                        }
                        RectInt val = new(new Vector2Int(x, y), new Vector2Int(squareSize - 1, squareSize - 1));
                        if (dp[x + (y * size)] >= squareSize && IsInRect(val,pos) && !HasOverlaps(val)) {
                            return val;
                        }
                    }
                }
                squareSize--;
            }
            return new RectInt(pos, new Vector2Int());
        }

        private bool HasOverlaps(RectInt r) {
            foreach (Vector2Int item in IterPositions(r)) {
                if (!pumpkins.ContainsKey(item)) {
                    continue;
                }
                if (!groups.ContainsKey(pumpkins[item])) {
                    continue;
                }
                RectInt val = groups[pumpkins[item]];
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

        public Dictionary<Vector2Int, Pumpkin> GetPumkings() {
            return pumpkins;
        }

        public Dictionary<Pumpkin, RectInt> GetGroups() {
            return groups;
        }
    }
}
