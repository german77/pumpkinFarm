namespace PumpkinFarm {
    public partial class Form1 : Form {
        private PumpkinController controller = new PumpkinController(10);
        private bool needsClear = false;
        private bool isTimed = false;
        private long totalTime = 0;
        private float runs = 0;

        public Form1() {
            InitializeComponent();
            timer1.Start();
        }

        private void MainTimerEvent(object sender, EventArgs e) {
            SolidBrush orangeBrush = new SolidBrush(Color.Orange);
            SolidBrush redBrush = new SolidBrush(Color.Red);

            if (isTimed) {
                // Timed Run
                var watch = System.Diagnostics.Stopwatch.StartNew();
                controller.FillAll();
                watch.Stop();
                needsClear = true;
                runs++;
                totalTime += watch.ElapsedMilliseconds;
                Console.WriteLine(runs + " Average:" + (totalTime / runs) + " Time:" + watch.ElapsedMilliseconds);
            } else {
                // Visualization 
                needsClear = !controller.FillNext();
            }

            // Draw pumpkings
            Bitmap bmp = new Bitmap(picture.Width, picture.Height);
            int spacing = picture.Height / controller.size;
            int rad = spacing * 3 / 4;

            using (Graphics graphics = Graphics.FromImage(bmp)) {
                graphics.Clear(Color.White);
                foreach (var (key, item) in controller.GetGroups()) {
                    Rectangle rectangle = new Rectangle(
                        (item.min.x * spacing) + 10 + (rad / 2), (item.min.y * spacing) + 10 + (rad / 2),
                        (item.max.x - item.min.x) * spacing, (item.max.y - item.min.y) * spacing);
                    graphics.FillRectangle(orangeBrush, rectangle);
                }
                foreach (var (key, item) in controller.GetPumkings()) {
                    Rectangle rectangle = new Rectangle(
                        (key.x * spacing) + 10,
                        (key.y * spacing) + 10, rad, rad);
                    graphics.FillEllipse(redBrush, rectangle);

                }
            }
            picture.Image = bmp;

            if (needsClear) {
                controller.Clear();
                needsClear = false;
            }
        }
    }
}
