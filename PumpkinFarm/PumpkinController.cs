
namespace PumpkinFarm {
    public class PumpkinController {
        private Random rand = new Random();
        private Dictionary<Vector2Int, Pumpkin> pumpkins = [];
        private Dictionary<Pumpkin, RectInt> groups = [];
        public readonly int size = 10;

        public bool AddPumpkin(Pumpkin p) {
            Vector2Int val = p.pos;
            if (pumpkins.ContainsKey(val)) {
                return false;
            }
            //System.Diagnostics.Debug.WriteLine("Pumking added at "+p);
            pumpkins[val] = p;
            MergeWithOthers(val);
            return true;
        }

        public void FillAll() {
            while (FillNext()) ;
        }

        public bool FillNext() {
            if (pumpkins.Count >= size * size) {
                return false;
            }

            Pumpkin p = new Pumpkin();
            p.pos.x = rand.Next(size);
            p.pos.y = rand.Next(size);
            while (!AddPumpkin(p)) {
                p.pos.x++;
                if (p.pos.x >= size) {
                    p.pos.y++;
                    p.pos.x = 0;
                    if (p.pos.y >= size) {
                        p.pos.y = 0;
                    }
                }
            }


            return true;

        }

        public void Clear() {
            pumpkins.Clear();
            groups.Clear();
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
