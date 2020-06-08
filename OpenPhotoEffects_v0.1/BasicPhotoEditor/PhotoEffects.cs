using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace BasicPhotoEditor
{
    partial class SimplePhotoEditor
    {

        //These effects have takes in an input Bitmap (workingBitmap)
        // and returns an output Bitmap (CanvasRenderTarget)

        public bool edgeDetectionOn = true;
        public bool edgeDetectionGrayscale = false;
        public double edgeDetectionExposure = 0;
        public double edgeDetectionContrast = 0.5;
        public double edgeDetectionAmount = 0.5;
        public double edgeDetectionBlurAmount = 0;
        public bool edgeDetectionOverlayImage = false;
        public bool edgeDetectionMaskInvert = true;
        public double edgeDetectionOverlayOpacity = 0.5;
        public BlendEffectMode edgeDetectionBlendEffectMode = BlendEffectMode.Screen;
        public EdgeDetectionEffectMode edgeDetectionBlurMode = EdgeDetectionEffectMode.Sobel;
        public CanvasRenderTarget applyEdgeDetectionEffects(CanvasBitmap workingBitmap)
        {
            //CanvasBitmap workingBitmap = SelectWorkingBitmap(useOriginalBitmap);
            

            if (workingBitmap != null)
            {
                int ww = (int)workingBitmap.SizeInPixels.Width;
                int hh = (int)workingBitmap.SizeInPixels.Height;

                //GrayscaleEffect grayscaleEffect = new GrayscaleEffect();
                //grayscaleEffect.Source=canvasBitmap;
                
                ContrastEffect contrastEffect = new ContrastEffect();
                contrastEffect.Contrast = (float)edgeDetectionContrast;                
                contrastEffect.Source = workingBitmap;

                ExposureEffect exposureEffect = new ExposureEffect();
                exposureEffect.Source = contrastEffect;
                exposureEffect.Exposure = (float) edgeDetectionExposure;
                

                EdgeDetectionEffect edgeDetectionEffect = new EdgeDetectionEffect();                
                edgeDetectionEffect.Source = exposureEffect;
                edgeDetectionEffect.Amount = (float)edgeDetectionAmount;
                edgeDetectionEffect.BlurAmount = (float)edgeDetectionBlurAmount;
                //edgeDetectionEffect.OverlayEdges = true;
                //edgeDetectionEffect.Mode = EdgeDetectionEffectMode.Prewitt;
                
                GrayscaleEffect grayscaleEffect = null;
                if (edgeDetectionGrayscale)
                {
                    grayscaleEffect = new GrayscaleEffect();
                    grayscaleEffect.Source = exposureEffect;
                    edgeDetectionEffect.Source = grayscaleEffect;

                }
                
                InvertEffect invertEdgeEffect = null;
                if (edgeDetectionMaskInvert)
                {
                    invertEdgeEffect = new InvertEffect();
                    invertEdgeEffect.Source = edgeDetectionEffect;

                }



                BlendEffect blendEffect = null;
                if (edgeDetectionOverlayImage)
                {
                    OpacityEffect opacityEffect = new OpacityEffect();
                    opacityEffect.Opacity = (float)edgeDetectionOverlayOpacity;
                    opacityEffect.Source = workingBitmap;
                    
                    blendEffect = new BlendEffect();
                    blendEffect.Foreground = edgeDetectionEffect;
                    blendEffect.Background = opacityEffect;
                    if (edgeDetectionMaskInvert)
                    {
                        //blendEffect.Background = invertEdgeEffect;
                        //blendEffect.Foreground = opacityEffect;

                        InvertEffect invertOrgEffect = new InvertEffect();
                        invertOrgEffect.Source = opacityEffect;
                        blendEffect.Background = invertOrgEffect;
                    }
                    
                    blendEffect.Mode = edgeDetectionBlendEffectMode;

                    

                }


                //if (canvasRenderTarget != null)
                //    canvasRenderTarget.Dispose();
                CanvasRenderTarget canvasRenderTarget = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), ww, hh, canvasBitmap.Dpi);
                using (var session = canvasRenderTarget.CreateDrawingSession())
                {
                    
                        if (edgeDetectionOverlayImage)
                        {
                            session.DrawImage(blendEffect);
                        }
                        else
                        {   
                            if (edgeDetectionMaskInvert)
                            {
                                session.DrawImage(invertEdgeEffect);
                            }
                            else
                                session.DrawImage(edgeDetectionEffect);
                        }
                    
                    
                }

                return canvasRenderTarget;

            }

            return null;

        }


        public double hueRotationAngle = 0;
        public double hueTemperature = -0.5;
        public double hueTint = 0.1;
        public bool hueDoPosterize = false;
        public int huePosterizeRedCount = 12;
        public int huePosterizeGreenCount = 12;
        public int huePosterizeBlueCount = 12;
        public CanvasRenderTarget applyHueRotationEffects(CanvasBitmap workingBitmap)
        {
            //CanvasBitmap workingBitmap = SelectWorkingBitmap(useOriginalBitmap);

            if (workingBitmap != null)
            {
                int ww = (int)workingBitmap.SizeInPixels.Width;
                int hh = (int)workingBitmap.SizeInPixels.Height;

                TemperatureAndTintEffect temperatureAndTintEffect = new TemperatureAndTintEffect();
                temperatureAndTintEffect.Source = workingBitmap;
                temperatureAndTintEffect.Temperature = (float) hueTemperature;
                temperatureAndTintEffect.Tint = (float)hueTint;

                HueRotationEffect hueRotationEffect = new HueRotationEffect();
                hueRotationEffect.Angle = (float) hueRotationAngle;
                hueRotationEffect.Source = temperatureAndTintEffect;

                PosterizeEffect posterizeEffect = null;
                EdgeDetectionEffect edgeDetectionEffect = null;
                if (hueDoPosterize)
                {
                    posterizeEffect = new PosterizeEffect();
                    posterizeEffect.Source = hueRotationEffect;
                    posterizeEffect.RedValueCount = huePosterizeRedCount;
                    posterizeEffect.BlueValueCount = huePosterizeBlueCount;
                    posterizeEffect.GreenValueCount = huePosterizeGreenCount;

                    edgeDetectionEffect = new EdgeDetectionEffect();
                    edgeDetectionEffect.Source = posterizeEffect;
                    edgeDetectionEffect.Amount = (float)0.9;
                    edgeDetectionEffect.BlurAmount = 1;
                    edgeDetectionEffect.OverlayEdges = true;


                }

                //if (canvasRenderTarget != null)
                //    canvasRenderTarget.Dispose();
                CanvasRenderTarget canvasRenderTarget = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), ww, hh, canvasBitmap.Dpi);
                using (var session = canvasRenderTarget.CreateDrawingSession())
                {

                    if (hueDoPosterize)
                    {
                        //session.DrawImage(posterizeEffect);                        
                        session.DrawImage(edgeDetectionEffect);
                        
                    }
                    else
                        session.DrawImage(hueRotationEffect);
                }

                return canvasRenderTarget;
            }

            return null;
        }


        public Color distantLightColor = Colors.White;
        public double distantAzimuth = 0.0;
        public double distantElevation = 0.0;
        public double distantDiffuseSaturation = 0.4;
        public double distantDiffuseEffectHeightMapScale = 6.0;
        public double distantSpecularHeightMapScale = 2.0;
        public double distantSpecularEffectSpecularExponent = 0.5;
        public double distantSpecularKernelWidth = 1.2;
        public double distantDiffuseKernelWidth = 1.0;
        public double gaussianBlurAmount = 0.0;
        public CanvasRenderTarget apply3DLightingEffects(CanvasBitmap workingBitmap)
        {
            //CanvasBitmap workingBitmap = SelectWorkingBitmap(useOriginalBitmap);

            if (workingBitmap != null)
            {
                int ww = (int)workingBitmap.SizeInPixels.Width;
                int hh = (int)workingBitmap.SizeInPixels.Height;

                //LuminanceToAlphaEffect heightField = new LuminanceToAlphaEffect();
                //heightField.Source = workingBitmap;


                LuminanceToAlphaEffect heightMap = new LuminanceToAlphaEffect();
                heightMap.Source = workingBitmap;

                GaussianBlurEffect heightField = new GaussianBlurEffect();
                heightField.BlurAmount = (float) gaussianBlurAmount;
                heightField.Source = heightMap;
                heightField.BorderMode = EffectBorderMode.Soft;

                DistantDiffuseEffect distantDiffuseEffect = new DistantDiffuseEffect();
                distantDiffuseEffect.Source = heightField;
                distantDiffuseEffect.HeightMapScale = (float) distantDiffuseEffectHeightMapScale;
                distantDiffuseEffect.HeightMapInterpolationMode = CanvasImageInterpolation.HighQualityCubic;
                distantDiffuseEffect.Azimuth = (float)distantAzimuth;
                distantDiffuseEffect.Elevation = (float)distantElevation;                
                distantDiffuseEffect.LightColor = distantLightColor;
                distantDiffuseEffect.HeightMapKernelSize = new System.Numerics.Vector2((float)distantDiffuseKernelWidth, (float)distantDiffuseKernelWidth);
                //distantDiffuseEffect.DiffuseAmount = (float) distantDiffuseAmount;

                DistantSpecularEffect distantSpecularEffect = new DistantSpecularEffect();
                distantSpecularEffect.Source = heightField;
                distantSpecularEffect.SpecularExponent = (float) distantSpecularEffectSpecularExponent;
                distantSpecularEffect.HeightMapInterpolationMode = CanvasImageInterpolation.HighQualityCubic;
                distantSpecularEffect.HeightMapKernelSize = new System.Numerics.Vector2((float)distantSpecularKernelWidth, (float)distantSpecularKernelWidth);
                distantSpecularEffect.HeightMapScale = (float)distantSpecularHeightMapScale;
                distantSpecularEffect.Azimuth = (float)distantAzimuth;
                distantSpecularEffect.Elevation = (float)distantElevation;
                //distantSpecularEffect.SpecularAmount = (float)distantSpecularAmount;
                //distantSpecularEffect.LightColor = distantLightColor;

                ArithmeticCompositeEffect arithmeticCompositeEffect = new ArithmeticCompositeEffect();
                arithmeticCompositeEffect.Source1 = distantDiffuseEffect;
                //arithmeticCompositeEffect.Source1 = blendedDiffuseEffect;
                arithmeticCompositeEffect.Source2 = distantSpecularEffect;
                arithmeticCompositeEffect.Source1Amount = 1;
                arithmeticCompositeEffect.Source2Amount = 1;
                arithmeticCompositeEffect.MultiplyAmount = 0;

                SaturationEffect saturationEffect = new SaturationEffect();
                saturationEffect.Source = workingBitmap;
                saturationEffect.Saturation = (float)distantDiffuseSaturation;

                ArithmeticCompositeEffect blendedDiffuseEffect = new ArithmeticCompositeEffect();
                //blendedDiffuseEffect.Source1 = workingBitmap;
                blendedDiffuseEffect.Source1 = saturationEffect;
                blendedDiffuseEffect.Source2 = arithmeticCompositeEffect;
                blendedDiffuseEffect.Source1Amount = 0;
                blendedDiffuseEffect.Source2Amount = 0;
                blendedDiffuseEffect.MultiplyAmount = 1;


                //if (canvasRenderTarget != null)
                //    canvasRenderTarget.Dispose();
                CanvasRenderTarget canvasRenderTarget = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), ww, hh, canvasBitmap.Dpi);
                using (var session = canvasRenderTarget.CreateDrawingSession())
                {
                    //session.DrawImage(arithmeticCompositeEffect);
                    session.DrawImage(blendedDiffuseEffect);

                }
                
                return canvasRenderTarget;
            }

            return null;
        }


        public double highlightClarity = 0;
        public double highlightHighlights = -0.5;
        public double highlightShadows = -0.5;
        public double highlightMaskBlur = 1.25;
        public CanvasRenderTarget applyHighlightEffects(CanvasBitmap workingBitmap)
        {
            //CanvasBitmap workingBitmap = SelectWorkingBitmap(useOriginalBitmap);

            if (workingBitmap != null)
            {
                int ww = (int)workingBitmap.SizeInPixels.Width;
                int hh = (int)workingBitmap.SizeInPixels.Height;

                HighlightsAndShadowsEffect highlightsAndShadowsEffect = new HighlightsAndShadowsEffect();
                highlightsAndShadowsEffect.Source = workingBitmap;
                highlightsAndShadowsEffect.Clarity = (float)highlightClarity;
                highlightsAndShadowsEffect.Shadows = (float)highlightShadows;
                highlightsAndShadowsEffect.MaskBlurAmount = (float)highlightMaskBlur;
                highlightsAndShadowsEffect.Highlights = (float)highlightHighlights;

                //if (canvasRenderTarget != null)
                //    canvasRenderTarget.Dispose();
                CanvasRenderTarget canvasRenderTarget = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), ww, hh, canvasBitmap.Dpi);
                using (var session = canvasRenderTarget.CreateDrawingSession())
                {
                    session.DrawImage(highlightsAndShadowsEffect);

                }

                return canvasRenderTarget;
            }


            return null;
        }


        
        public double sepiaIntensity = 0.5;
        public double vignetteAmount = 0.3;
        public double vignetteCurve = 0.5;
        public Color vignetteColor = Colors.White;
        public CanvasRenderTarget applySepiaEffects(CanvasBitmap workingBitmap)
        {

            //CanvasBitmap workingBitmap = SelectWorkingBitmap(useOriginalBitmap);
            

            if (workingBitmap != null)
            {
                int ww = (int)workingBitmap.SizeInPixels.Width;
                int hh = (int)workingBitmap.SizeInPixels.Height;
                
                //Microsoft.Graphics.Canvas.Effects.SepiaEffect
                SepiaEffect sepiaEffect = new SepiaEffect();
                sepiaEffect.Source = workingBitmap;
                sepiaEffect.Intensity = (float)sepiaIntensity;

                VignetteEffect vignetteEffect = new VignetteEffect();
                vignetteEffect.Source = sepiaEffect;
                vignetteEffect.Amount = (float)vignetteAmount;
                vignetteEffect.Curve = (float)vignetteCurve;
                vignetteEffect.Color = vignetteColor;

                //if (canvasRenderTarget != null)
                //    canvasRenderTarget.Dispose();
                CanvasRenderTarget canvasRenderTarget = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), ww, hh, canvasBitmap.Dpi);
                using (var session = canvasRenderTarget.CreateDrawingSession())
                {
                    //session.DrawImage(sepiaEffect);
                    session.DrawImage(vignetteEffect);

                }

                return canvasRenderTarget;

            }

            return null;
        }


        public CanvasRenderTarget applyOriginalEffects(CanvasBitmap workingBitmap)
        {
            //CanvasBitmap workingBitmap = SelectWorkingBitmap(useOriginalBitmap);

            if (workingBitmap != null)
            {
                int ww = (int)workingBitmap.SizeInPixels.Width;
                int hh = (int)workingBitmap.SizeInPixels.Height;

                //if (canvasRenderTarget != null)
                //    canvasRenderTarget.Dispose();
                CanvasRenderTarget canvasRenderTarget = new CanvasRenderTarget(CanvasDevice.GetSharedDevice(), ww, hh, canvasBitmap.Dpi);
                using (var session = canvasRenderTarget.CreateDrawingSession())
                {   
                    session.DrawImage(workingBitmap);

                }

                return canvasRenderTarget;
            }

            return null;
        }


        public CanvasBitmap SelectWorkingBitmap(bool useOriginalBitmap = false)
        {

            CanvasBitmap workingBitmap = null;
            if (canvasRenderTargetPreview != null)
                workingBitmap = canvasRenderTargetPreview;
            else
                workingBitmap = canvasBitmap;


            //by default not used due to large size of image
            //used only when user is saving the image
            if (useOriginalBitmap)
                workingBitmap = canvasBitmap;


            return workingBitmap;


        }



        //apply Effects for screen preview using the global variable canvasRenderTarget
        //or apply Effects for saving to file using original bitmap
        public void applyHighlightEffects(bool useOriginalBitmap = false)
        {
            CanvasBitmap workingBitmap = SelectWorkingBitmap(useOriginalBitmap);
            CanvasRenderTarget crt = applyHighlightEffects(workingBitmap);
            if (crt != null)
            {
                if (canvasRenderTarget != null)
                    canvasRenderTarget.Dispose();
                canvasRenderTarget = crt;
            }
        }


        public void applyOriginalEffects(bool useOriginalBitmap = false)
        {
            CanvasBitmap workingBitmap = SelectWorkingBitmap(useOriginalBitmap);
            CanvasRenderTarget crt = applyOriginalEffects(workingBitmap);
            if (crt != null)
            {
                if (canvasRenderTarget != null)
                    canvasRenderTarget.Dispose();
                canvasRenderTarget = crt;
            }
        }

        public void applyHueRotationEffects(bool useOriginalBitmap = false)
        {
            CanvasBitmap workingBitmap = SelectWorkingBitmap(useOriginalBitmap);
            CanvasRenderTarget crt = applyHueRotationEffects(workingBitmap);
            if (crt != null)
            {
                if (canvasRenderTarget != null)
                    canvasRenderTarget.Dispose();
                canvasRenderTarget = crt;
            }
        }

        public void applyEdgeDetectionEffects(bool useOriginalBitmap = false)
        {
            CanvasBitmap workingBitmap = SelectWorkingBitmap(useOriginalBitmap);
            CanvasRenderTarget crt = applyEdgeDetectionEffects(workingBitmap);
            if (crt != null)
            {
                if (canvasRenderTarget != null)
                    canvasRenderTarget.Dispose();
                canvasRenderTarget = crt;
            }
        }


        public void apply3DLightingEffects(bool useOriginalBitmap = false)
        {
            CanvasBitmap workingBitmap = SelectWorkingBitmap(useOriginalBitmap);
            CanvasRenderTarget crt = apply3DLightingEffects(workingBitmap);
            if (crt != null)
            {
                if (canvasRenderTarget != null)
                    canvasRenderTarget.Dispose();
                canvasRenderTarget = crt;
            }
        }

        public void applySepiaEffects(bool useOriginalBitmap = false)
        {
            CanvasBitmap workingBitmap = SelectWorkingBitmap(useOriginalBitmap);
            CanvasRenderTarget crt = applySepiaEffects(workingBitmap);
            if (crt != null)
            {
                if (canvasRenderTarget != null)
                    canvasRenderTarget.Dispose();
                canvasRenderTarget = crt;
            }
        }
        //////////////////////////


    }
}
