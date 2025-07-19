using Fractal = System.Collections.Generic.List<(double x, double y)>;

ApplicationConfiguration.Initialize();

List<PointF> points = [];

var form = new Form {
    FormBorderStyle = FormBorderStyle.None,
    WindowState = FormWindowState.Maximized,
    KeyPreview = true
};

form.KeyDown += (o, e) =>
{
    if (e.KeyCode == Keys.Escape)
    {
        Application.Exit();
        return;
    }
};

form.Paint += (o, e) =>
{
    var g = e.Graphics;
    g.Clear(Color.Black);

    if (points.Count < 2)
        return;
    
    var pen = new Pen(Brushes.White, 2f);
    g.DrawLines(pen, [ ..points ]);
};

form.Load += (o, e) => Show(KochFractal(), 1, 100);

Application.Run(form);

Fractal KochFractal()
    => [ (0, 0), (0.25, 0), (0.5, 1.0), (0.75, 0), (1, 0) ];

void Show(Fractal fractal, int deep, int ptCount)
{
    var points = MakeFractal(fractal, deep, ptCount);
    SetPoits(points, form.Width, form.Height);
}

void SetPoits(List<PointF> fracPoints, int widht, int height)
{
    points = fracPoints
        .Select(pt => new PointF(20 + pt.X * (widht - 40), height - 20 - pt.Y * (height - 40)))
        .ToList();
}

List<PointF> MakeFractal(Fractal fractal, int deep, int points)
{
    var point = 0;
    var x = 0f;
    var dx = 1f / points;
    List<PointF> fractalPoints = [];

    while (point < points)
    {
        var pt = ComputePoint(x, fractal, deep);
        fractalPoints.Add(new PointF((float)pt.x, (float)pt.y));

        point++;
        x += dx;
    }

    return fractalPoints;
}

(double x, double y) ComputePoint(float x, Fractal fractal, int deep)
{

}