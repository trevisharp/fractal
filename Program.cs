using Fractal = System.Collections.Generic.List<(double x, double y)>;
using Face = (double sx, double sy, double ex, double ey);

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

form.Load += (o, e) => Show(KochFractal(), 2, 100);

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
    var x = 0.0;
    var dx = 1.0 / points;
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

(double x, double y) ComputePoint(double x, Fractal fractal, int deep)
{
    Face crrFace = (0.0, 0.0, 1.0, 0.0);

    while (deep > 0)
    {
        (crrFace, x) = FindFace(fractal, crrFace, x);
        deep--;
    }

    return GetPoint(crrFace, x);
}

(Face face, double x) FindFace(Fractal fractal, Face face, double x)
{
    var bx = face.ex - face.sx;
    var by = face.ey - face.sy;
    var mod = Math.Sqrt(bx * bx + by * by);
    bx /= mod;
    by /= mod;

    var hx = -by;
    var hy = bx;

    for (int i = 0; i < fractal.Count - 1; i++)
    {
        var (x0, y0) = fractal[i];
        var (xf, yf) = fractal[i + 1];

        if (x > xf || x < x0)
            continue;

        Face newFace = (
            x0 * bx + y0 * by,
            x0 * hx + y0 * hy,
            xf * bx + yf * by,
            xf * hx + yf * hy
        );
        var prop = (x - x0) / (xf - x0);

        return (newFace, prop);
    }

    throw new Exception("Out of fractal");
}

(double x, double y) GetPoint(Face face, double p)
    => ((face.ex - face.sx) * p + face.sx, (face.ey - face.sy) * p + face.sy);
