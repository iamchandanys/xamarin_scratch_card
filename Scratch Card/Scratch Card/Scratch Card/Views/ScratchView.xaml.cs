using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TouchTracking;
using System;

namespace Scratch_Card.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScratchView : ContentPage
    {
        Dictionary<long, SKPath> inProgressPaths = new Dictionary<long, SKPath>();
        List<SKPath> completedPaths = new List<SKPath>();

        SKBitmap rect = new SKBitmap();
        SKRect sKRect = new SKRect();
        SKPaint paint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            BlendMode = SKBlendMode.Clear,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };

        bool IsScratched = false;
        long sensitivityCount = 0;

        public ScratchView()
        {
            InitializeComponent();

            paint.StrokeWidth = 80;

            ScratchView control = this;
            control.rect = control.GetBitMap("SC2.png");
        }

        SKBitmap GetBitMap(string resourceName)
        {
            SKBitmap resourceBitmap;

            Assembly assembly = GetType().GetTypeInfo().Assembly;
            string resourceId = $"{assembly.GetName().Name}.{resourceName}";

            //If the project name contains space, replace with underscore(_)...
            resourceId = resourceId.Replace(' ', '_');

            using(Stream stream = assembly.GetManifestResourceStream(resourceId))
            {
                resourceBitmap = SKBitmap.Decode(stream);
            }

            return resourceBitmap;
        }

        private void CanvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        {
            SKCanvas canvas = e.Surface.Canvas;
            canvas.Clear();
            canvas.DrawBitmap(rect, e.Info.Rect);

            float area = sKRect.Width * sKRect.Height;

            foreach(SKPath path in completedPaths)
            {
                canvas.DrawPath(path, paint);
                sKRect.Size = new SKSize(100, 100);
                path.GetBounds(out sKRect);
            }

            foreach(SKPath path in inProgressPaths.Values)
            {
                canvas.DrawPath(path, paint);
                sKRect.Size = new SKSize(100, 100);
                path.GetBounds(out sKRect);

                sensitivityCount++;

                if(sensitivityCount >= 120)
                {
                    if(!IsScratched)
                    {
                        IsScratched = true;
                        ScratchView control = this;
                        control.rect = control.GetBitMap("SC4.png");
                        Random random = new Random();
                        TxtTest.Text = random.Next(1, 100).ToString();
                    }
                }
            }
        }

        private void TouchEffect_TouchAction(object sender, TouchTracking.TouchActionEventArgs args)
        {
            switch(args.Type)
            {
                case TouchActionType.Pressed:
                    if(!inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = new SKPath();
                        path.MoveTo(ConvertToPixel(args.Location));
                        inProgressPaths.Add(args.Id, path);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Moved:
                    if(inProgressPaths.ContainsKey(args.Id))
                    {
                        SKPath path = inProgressPaths[args.Id];
                        path.LineTo(ConvertToPixel(args.Location));
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Released:
                    if(inProgressPaths.ContainsKey(args.Id))
                    {
                        completedPaths.Add(inProgressPaths[args.Id]);
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;

                case TouchActionType.Cancelled:
                    if(inProgressPaths.ContainsKey(args.Id))
                    {
                        inProgressPaths.Remove(args.Id);
                        canvasView.InvalidateSurface();
                    }
                    break;
            }
        }

        SKPoint ConvertToPixel(TouchTrackingPoint pt)
        {
            return new SKPoint((float)(canvasView.CanvasSize.Width * pt.X / canvasView.Width),
                               (float)(canvasView.CanvasSize.Height * pt.Y / canvasView.Height));
        }
    }
}