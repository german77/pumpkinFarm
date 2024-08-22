
namespace PumpkinFarm {
    public class PumpkinController {
        public readonly int size;

        private Random rand = new Random();
        private Queue<Vector2Int> fillingSequence;
        private Dictionary<Vector2Int, Pumpkin> pumpkins = [];
        private Dictionary<Pumpkin, RectInt> groups = [];

        public PumpkinController(int size) {
            this.size = size;
            this.fillingSequence = NewFillingSequence();
        }

        public void AddPumpkin(Pumpkin p) {
            Vector2Int val = p.pos;
            if (pumpkins.ContainsKey(val)) {
                throw new Exception("can't add pumpkin to an occupied spot");
            }
            //System.Diagnostics.Debug.WriteLine("Pumking added at "+p);
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
        }

        private Queue<Vector2Int> NewFillingSequence() {
            return new Queue<Vector2Int>(
                Enumerable.Range(0,size)
                    .SelectMany((l) => Enumerable.Range(0,size), (l, r) => new Vector2Int(l, r))
                    .OrderBy((item) => rand.Next())
            );
        }

        private void MergeWithOthers(Vector2Int pos) {
            int[] array = new int[size * size];
            for (int i = 0; i < array.Length; i++) {
                array[i] = (pumpkins.ContainsKey(new Vector2Int(i % size, i / size)) ? 1 : 0);
            }
            bool flag = true;
            int num2 = 1;
            while (flag) {
                num2++;
                flag = false;
                for (int j = 0; j < array.Length; j++) {
                    int num3 = j % size;
                    int num4 = j / size;
                    if (num3 + 1 < size && num4 + 1 < size &&
                        array[j] == num2 - 1 &&
                        array[num3 + 1 + size * num4] == num2 - 1 &&
                        array[num3 + size * (num4 + 1)] == num2 - 1 &&
                        array[num3 + 1 + size * (num4 + 1)] == num2 - 1) {
                        array[j] = num2;
                        flag = true;
                    }
                }
            }
            num2--;
            RectInt val = LargestSquare(array, size, num2, pos);
            foreach (Vector2Int item in IterPositions(val)) {
                groups[pumpkins[item]] = val;
            }
        }

        private RectInt LargestSquare(int[] dp, int n, int squareSize, Vector2Int pos) {
            RectInt val = new(new Vector2Int(), new Vector2Int());
            while (squareSize > 1) {
                for (int i = 0; i < dp.Length; i++) {
                    val = new(new Vector2Int(i % n, i / n), new Vector2Int(squareSize - 1, squareSize - 1));
                    if (dp[i] >= squareSize && IsInRect(val, pos) && !HasOverlaps(val)) {
                        return val;
                    }
                }
                squareSize--;
            }
            return new RectInt(pos, new Vector2Int());
        }

        private bool HasOverlaps(RectInt r) {
            foreach (Vector2Int item in IterPositions(r)) {
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
