using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace BasicPhotoEditor
{
    public partial class SimplePhotoEditor
    {
        public StorageFile savefile = null;
        public CanvasRenderTarget canvasRenderTarget = null;
        public CanvasBitmap canvasBitmap = null;
        public CanvasRenderTarget canvasRenderTargetPreview = null;
        public MainPage parent = null;
        //public int thumbnailSize = 128;
        public int thumbnailSize = 144;

        public SimplePhotoEditor(MainPage xparent)
        {
            parent = xparent;
        }


        public CanvasBitmap GenerateThumbnail()
        {
            if (canvasBitmap == null)
            {
                if (parent != null)
                    parent.StartWritingOutput("Canvas Bitmap is null. Load a Photo.", 1);

                return null;
            }
            
            CanvasRenderTarget canvasRenderTargetThumbnail = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), (float)thumbnailSize, (float)thumbnailSize, canvasBitmap.Dpi);

            //if (parent != null)
            //    parent.StartWritingOutput("Creating Thumbnail : Dimensions: " + thumbnailSize.ToString() + " x " + thumbnailSize.ToString(), 1);

            int ww = (int)canvasBitmap.SizeInPixels.Width;
            int hh = (int)canvasBitmap.SizeInPixels.Height;

            double newww = ww;
            double newhh = hh;
            double offsetx = 0;
            double offsety = 0;
            double unscaledHeight = hh;
            double referenceHeight = thumbnailSize;

            //make a square
            if (ww > hh)
            {
                newww = hh;
                offsetx = (ww - hh) / 2.0;
                unscaledHeight = hh;
            }
            else
            {
                newhh = ww;
                offsety = (hh - ww) / 2.0;
                unscaledHeight = ww;
            }

            if (unscaledHeight<=0.1)
            {
                if (parent != null)
                    parent.StartWritingOutput("Error making thumbnail :  Invalid unscaled height. ", 1);

            }

            double scaleFactor = referenceHeight / (double)unscaledHeight;
            //if (scaleFactor < 1.0) //can be greater or smaller than 1
            
            using (var session = canvasRenderTargetThumbnail.CreateDrawingSession())
            {
                ScaleEffect scaleEffect = new ScaleEffect();
                scaleEffect.Source = canvasBitmap;
                scaleEffect.Scale = new System.Numerics.Vector2((float)scaleFactor, (float)scaleFactor);
                session.DrawImage(scaleEffect);
            }

            return canvasRenderTargetThumbnail;

        }


        public void ResetBitmapResources()
        {
            if (canvasBitmap != null)
            {
                canvasBitmap.Dispose();
                canvasBitmap = null;
            }

            if (canvasRenderTarget != null)
            {
                canvasRenderTarget.Dispose();
                canvasRenderTarget = null;
            }

            if (canvasRenderTargetPreview != null)
            {
                canvasRenderTargetPreview.Dispose();
                canvasRenderTargetPreview = null;
            }
            
        }

        


        //make a small size bitmap for preview / effects modification purposes
        public void CreatePreviewBitmap()
        {
            double referenceHeight = 800;
            if (canvasBitmap != null)
            {
                int ww = (int)canvasBitmap.SizeInPixels.Width;
                int hh = (int)canvasBitmap.SizeInPixels.Height;

                double newww = ww;
                double newhh = hh;

                double widthToHeightRatio = 1.0;
                if (hh > 0)
                    widthToHeightRatio = ww / (double)hh;
                double scaleFactor = referenceHeight / (double)hh;
                if (scaleFactor<1.0)
                {
                        newww = scaleFactor * ww;
                        newhh = referenceHeight;
                }


                if (scaleFactor >= 1.0)
                {
                    if (parent != null)
                        parent.StartWritingOutput("Using original canvasBitmap due to its small size: Dimensions: " + newww.ToString() + " x " + newhh.ToString(), 1);

                    if (canvasRenderTargetPreview != null)
                        canvasRenderTargetPreview.Dispose();

                    canvasRenderTargetPreview = null;

                    return;
                }

                if (canvasRenderTargetPreview != null)
                    canvasRenderTargetPreview.Dispose();

                canvasRenderTargetPreview = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), (float) newww, (float) newhh, canvasBitmap.Dpi);

                if (parent != null)
                    parent.StartWritingOutput("Creating canvasRenderTargetPreview : Dimensions: " + newww.ToString() + " x " + newhh.ToString(), 1);

                using (var session = canvasRenderTargetPreview.CreateDrawingSession())
                {
                    ScaleEffect scaleEffect = new ScaleEffect();                    
                    scaleEffect.Source = canvasBitmap;
                    scaleEffect.Scale = new System.Numerics.Vector2((float)scaleFactor, (float)scaleFactor);
                    session.DrawImage(scaleEffect);
                }


            }

        }

        public void DrawBitmap(CanvasControl drawingcanvas, CanvasDrawingSession session)
        {
            if (canvasBitmap != null)
            {
                try
                {
                    int ww = (int)canvasBitmap.SizeInPixels.Width;
                    int hh = (int)canvasBitmap.SizeInPixels.Height;

                    //Using Preview Size
                    if (canvasRenderTargetPreview != null)
                    {
                        ww = (int) canvasRenderTargetPreview.SizeInPixels.Width;
                        hh = (int)canvasRenderTargetPreview.SizeInPixels.Height;
                        
                    }
                    
                    double widthToHeightRatio = 1.0;
                    if (hh > 0)
                        widthToHeightRatio = ww / (double)hh;
                    double scaleFactor = (double)drawingcanvas.Height / (double)hh;

                    
                    //byte[] bb = canvasBitmap.GetPixelBytes(0, 0, ww, hh);
                    Rect rect = new Rect(0, 0, ww, hh);

                    //if (parent != null)
                    //    parent.StartWritingOutput("PixelSize : " + ww.ToString() + " x " + hh.ToString(), 1);
                    

                    ScaleEffect scaleEffect = new ScaleEffect();
                    if (canvasRenderTarget != null)
                    {
                        scaleEffect.Source = canvasRenderTarget;
                        //if (parent != null)
                        //    parent.StartWritingOutput("Draw using canvasRenderTarget", 1);
                    }
                    else if (canvasRenderTargetPreview != null)
                    {
                        //ver 0.71
                        scaleEffect.Source = canvasRenderTargetPreview;
                        if (parent != null)
                            parent.StartWritingOutput("Draw using canvasRenderTargetPreview", 1);
                    }
                    else
                    {
                        if (parent != null)
                            parent.StartWritingOutput("Draw using canvasBitmap", 1);
                        scaleEffect.Source = canvasBitmap;

                    }
                    scaleEffect.Scale = new System.Numerics.Vector2((float)scaleFactor, (float)scaleFactor);

                    double actualCanvasWdith = drawingcanvas.ActualWidth;
                    double offsetx = (actualCanvasWdith - (scaleFactor * ww)) / 2.0;
                    double offsety = 0;
                    
                    //args.DrawingSession.DrawImage(simplePhotoEditor.canvasBitmap);                    
                    session.DrawImage(scaleEffect, (float)offsetx, (float)offsety);
                }
                catch (Exception e)
                {
                    if (parent != null)
                        parent.StartWritingOutput("Draw Error : " + e.Message, 1);

                }
            }


        }


        public async Task SaveCanvasBitmap(StorageFile outfile,CanvasBitmapFileFormat canvasBitmapFileFormat)
        {

            
            IRandomAccessStream randomAccessStream   = await outfile.OpenAsync(FileAccessMode.ReadWrite);
            if (canvasRenderTarget != null)
            {
                try
                {
                    await canvasRenderTarget.SaveAsync(randomAccessStream, canvasBitmapFileFormat);
                    await randomAccessStream.FlushAsync();
                    randomAccessStream.Dispose();


                }
                catch (Exception e)
                {
                    if (parent != null)
                        parent.StartWritingOutput("Error Saving File : " + e.Message,1);

                    ContentDialog contentDialogA = new ContentDialog();
                    contentDialogA.Title = "File Saved";
                    contentDialogA.Content = "Error Saving File : " + e.Message;
                    contentDialogA.CloseButtonText = "OK";
                    await contentDialogA.ShowAsync();


                    return;
                }

                if (parent != null)
                    parent.StartWritingOutput("File Saved to " + outfile.Path,1);

                ContentDialog contentDialog = new ContentDialog();
                contentDialog.Title = "File Saved";
                contentDialog.Content = "The photo has been saved successfully.";
                contentDialog.CloseButtonText = "OK";                
                await contentDialog.ShowAsync();



                //canvasRenderTarget.Dispose();
                //canvasRenderTarget = null;
                //CreatePreviewBitmap(); 

            }


        }

        public async Task LoadCanvasBitmap()
        {
            IRandomAccessStream outputstream = null;
            if (savefile != null)
            {
                try
                {
                    //outputstream = await savefile.OpenReadAsync();
                    //await RandomAccessStream.CopyAsync(outputstream, inMemoryRandomAccessStream);
                    using (outputstream = await savefile.OpenAsync(FileAccessMode.Read))
                    {
                        
                        if (canvasRenderTargetPreview != null)
                        {
                            canvasRenderTargetPreview.Dispose();
                            canvasRenderTargetPreview = null;

                        }

                        if (canvasRenderTarget != null)
                        {
                            canvasRenderTarget.Dispose();
                            canvasRenderTarget = null;
                        }

                        if (canvasBitmap != null)
                            canvasBitmap.Dispose();

                        canvasBitmap = await CanvasBitmap.LoadAsync(CanvasDevice.GetSharedDevice(), outputstream);

                    }

                    int ww = (int)canvasBitmap.SizeInPixels.Width;
                    int hh = (int)canvasBitmap.SizeInPixels.Height;

                    if (parent != null)
                        parent.StartWritingOutput("Image Pixel Size : " + ww.ToString() + " x " + hh.ToString(), 1);


                }
                catch (Exception e)
                {
                    if (parent != null)
                        parent.StartWritingOutput("File Load Error : " + e.Message);

                }
            }

        }

    }
}
