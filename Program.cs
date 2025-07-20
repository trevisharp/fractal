using Fractal = System.Collections.Generic.List<(double x, double y)>;
using Face = (double sx, double sy, double ex, double ey);

ApplicationConfiguration.Initialize();

int deep = 0;
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

    switch (e.KeyCode)
    {
        case Keys.W:
            deep = int.Min(8, deep + 1);
            break;

        case Keys.S:
            deep = int.Max(0, deep - 1);
            break;
        
        default:
            return;
    }

    Update();
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

form.Load += (o, e) => Update();

Application.Run(form);

void Show(List<PointF> pts)
{
    points = pts
        .Select(p => new PointF(p.X, form.Height - p.Y))
        .ToList();
    form.Invalidate();
}

void Update()
{
    double step = form.Width / 5;
    double x1 = 2 * step;
    double y1 = step;
    double x2 = 3 * step;
    double y2 = step;
    double x3 = 2.5 * step;
    double y3 = (Math.Sqrt(1.25) + 1) * step;

    var p1 = MakeFractal(KochFractal(), (x2, y2, x1, y1), deep, 10_000);
    var p2 = MakeFractal(KochFractal(), (x1, y1, x3, y3), deep, 10_000);
    var p3 = MakeFractal(KochFractal(), (x3, y3, x2, y2), deep, 10_000);

    Show([..p1, ..p2, ..p3]);
}

Fractal KochFractal()
    => [ (0, 0), (0.25, 0), (0.5, 0.25), (0.75, 0), (1, 0) ];

List<PointF> MakeFractal(Fractal fractal, Face initial, int deep, int points)
{
    var point = 0;
    var x = 0.0;
    var dx = 1.0 / points;
    List<PointF> fractalPoints = [];

    while (point < points)
    {
        var pt = ComputePoint(x, fractal, deep, initial);
        fractalPoints.Add(new PointF((float)pt.x, (float)pt.y));

        point++;
        x += dx;
    }

    return fractalPoints;
}

(double x, double y) ComputePoint(double x, Fractal fractal, int deep, Face initial)
{
    Face crrFace = initial;

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
            face.sx + mod * (x0 * bx + y0 * hx),
            face.sy + mod * (x0 * by + y0 * hy),
            face.sx + mod * (xf * bx + yf * hx),
            face.sy + mod * (xf * by + yf * hy)
        );
        var prop = (x - x0) / (xf - x0);

        return (newFace, prop);
    }

    throw new Exception("Out of fractal");
}

(double x, double y) GetPoint(Face face, double p)
    => ((face.ex - face.sx) * p + face.sx, (face.ey - face.sy) * p + face.sy);
