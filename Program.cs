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

(double x, double y) ComputePoint(double p, Fractal fractal, int deep)
{
    Face crrFace = (0.0, 0.0, 1.0, 0.0);

    while (deep > 0)
    {
        (crrFace, p) = FindFace(fractal, crrFace, p);
        deep--;
    }

    return GetPoint(crrFace, p);
}

(Face face, double p) FindFace(Fractal fractal, Face face, double p)
{
    var bx = face.ex - face.sx;
    var by = face.ey - face.sy;

    var hx = -by;
    var hy = bx;

    for (int i = 0; i < fractal.Count - 1; i++)
    {
        if (fractal[i].x <= p && p <= fractal[i + 1].x)
        {
            var newFace = (
                fractal[i].x * bx,
                fractal[i].y * hx,
                fractal[i + 1].x * bx,
                fractal[i + 1].y * hy
            );
            var prop = p - fractal[i].x; // FIX

            return (newFace, prop);
        }
    }

    throw new Exception("Out of fractal");
}

(double x, double y) GetPoint(Face face, double p)
    => ((face.ex - face.sx) * p + face.sx, (face.ey - face.sy) * p + face.sy);