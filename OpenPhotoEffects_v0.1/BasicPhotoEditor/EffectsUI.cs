using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.DirectX;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace BasicPhotoEditor
{
    public sealed partial class MainPage : Page
    {
        public void AddTextToggleBlock(StackPanel stackPanel, string text, bool defaultval, RoutedEventHandler routedEventHandler)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Margin = new Thickness(10, 10, 10, 10);

        
            ToggleSwitch toggleSwitch = new ToggleSwitch();
            toggleSwitch.Toggled += routedEventHandler;
            toggleSwitch.Margin = new Thickness(10, 10, 10, 10);
            toggleSwitch.IsOn = defaultval;

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(toggleSwitch);


        }

        public void AddTextBlendBlock(StackPanel stackPanel, string text, BlendEffectMode blendEffectMode, SelectionChangedEventHandler selectionChangedEventHandler)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Margin = new Thickness(10, 10, 10, 10);

            ComboBox comboBox = new ComboBox();
            foreach (BlendEffectMode blendmode in Enum.GetValues(typeof(BlendEffectMode)))
            {
                comboBox.Items.Add(blendmode.ToString());
            }
            comboBox.SelectedItem = blendEffectMode.ToString();
            comboBox.SelectionChanged += selectionChangedEventHandler;

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(comboBox);


        }
        public void AddTextColorBlock(StackPanel stackPanel, string text, RoutedEventHandler routedEventHandler)
        {

            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Margin = new Thickness(10, 10, 10, 10);

            Button button = new Button();
            button.Content = "Choose";
            button.Margin = new Thickness(10, 10, 10, 10);
            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.Click += routedEventHandler;

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(button);
        }

        public void AddTextSliderBlock(StackPanel stackPanel, string text, double defaultval, double min, double max, RangeBaseValueChangedEventHandler rangeBaseValueChangedEventHandler)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Margin = new Thickness(10, 10, 10, 10);

            Slider slider = new Slider();
            slider.Margin = new Thickness(10, 10, 10, 10);
            slider.Maximum = max;
            slider.Minimum = min;
            slider.Value = defaultval;
            slider.TickFrequency = 20;
            slider.TickPlacement = TickPlacement.BottomRight;
            slider.ValueChanged += rangeBaseValueChangedEventHandler;

            stackPanel.Children.Add(textBlock);
            stackPanel.Children.Add(slider);


        }


        public void OpenEdgeDetectionEffectsUI()
        {
            Flyout flyout = new Flyout();

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Width = 130;

            AddTextToggleBlock(stackPanel, "Grayscale", simplePhotoEditor.edgeDetectionGrayscale, ToggleSwitchEdgeDGrayscale_Toggled);
            AddTextSliderBlock(stackPanel, "Exposure", simplePhotoEditor.edgeDetectionExposure * 100, -200, 200, SliderEdgeExposure_ValueChanged);
            AddTextSliderBlock(stackPanel, "Contrast", simplePhotoEditor.edgeDetectionContrast * 100, -100, 100, SliderEdgeContrast_ValueChanged);
            
            StackPanel stackPanelRight = new StackPanel();
            stackPanelRight.Orientation = Orientation.Vertical;
            stackPanelRight.Width = 130;
            //AddTextToggleBlock(stackPanelRight, "Edge Detect", simplePhotoEditor.edgeDetectionOn, ToggleSwitchEdgeDOn_Toggled);
            AddTextSliderBlock(stackPanelRight, "Edge Amount", simplePhotoEditor.edgeDetectionAmount * 100, 0, 100, SliderEdgeAmount_ValueChanged);
            AddTextSliderBlock(stackPanelRight, "Edge Blur", simplePhotoEditor.edgeDetectionBlurAmount * 100, 0, 1000, SliderEdgeBlur_ValueChanged);
            AddTextToggleBlock(stackPanelRight, "Invert", simplePhotoEditor.edgeDetectionMaskInvert, ToggleSwitchEdgeMaskInvert_Toggled);

            StackPanel stackPanelRightMost = new StackPanel();
            stackPanelRightMost.Orientation = Orientation.Vertical;
            stackPanelRightMost.Width = 130;
            AddTextToggleBlock(stackPanelRightMost, "Overlay Image", simplePhotoEditor.edgeDetectionOverlayImage, ToggleSwitchEdgeDOverlay_Toggled);            
            AddTextSliderBlock(stackPanelRightMost, "Overlay Opacity", simplePhotoEditor.edgeDetectionOverlayOpacity*100, 0, 100, SliderEdgeOOpacity_ValueChanged);
            AddTextBlendBlock(stackPanelRightMost, "Overlay Blend", simplePhotoEditor.edgeDetectionBlendEffectMode, BlendComboBox_SelectionChanged);

            StackPanel stackPanelCombined = new StackPanel();
            stackPanelCombined.Orientation = Orientation.Horizontal;
            stackPanelCombined.Children.Add(stackPanel);
            stackPanelCombined.Children.Add(stackPanelRight);
            stackPanelCombined.Children.Add(stackPanelRightMost);

            Border border = new Border();
            border.BorderThickness = new Thickness(flyoutBorderThickness);
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.Child = stackPanelCombined;

            FlyoutShowOptions flyoutShowOptions = new FlyoutShowOptions();
            flyoutShowOptions.Placement = FlyoutPlacementMode.RightEdgeAlignedBottom;
            flyout.Content = border;
            flyout.ShowAt(placementTarget, flyoutShowOptions);
            //flyout.ShowAt(TextConsole, flyoutShowOptions);


        }

        public void OpenSepiaEffectsUI()
        {
            Flyout flyout = new Flyout();

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Width = 150;

            AddTextSliderBlock(stackPanel, "Intensity", simplePhotoEditor.sepiaIntensity * 100, 0, 100, SliderSepia_ValueChanged);
            AddTextSliderBlock(stackPanel, "Vignette Amount", simplePhotoEditor.vignetteAmount * 100, 0, 100, SliderVignetteAmount_ValueChanged);
            AddTextSliderBlock(stackPanel, "Vignette Curve", simplePhotoEditor.vignetteCurve * 100, 0, 100, SliderVignetteCurve_ValueChanged);
            AddTextColorBlock(stackPanel, "Vignette Color", VignetteColorButton_Click);

            

            Border border = new Border();
            border.BorderThickness = new Thickness(flyoutBorderThickness);
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.Child = stackPanel;

            FlyoutShowOptions flyoutShowOptions = new FlyoutShowOptions();
            flyoutShowOptions.Placement = FlyoutPlacementMode.RightEdgeAlignedBottom;
            flyout.Content = border;
            flyout.ShowAt(placementTarget, flyoutShowOptions);

        }

        public void OpenHightlightShadowsEffectsUI()
        {
            Flyout flyout = new Flyout();

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Width = 150;

            AddTextSliderBlock(stackPanel, "Clarity", simplePhotoEditor.highlightClarity * 100, -100, 100, SliderHLClarity_ValueChanged);
            AddTextSliderBlock(stackPanel, "Highlights", simplePhotoEditor.highlightHighlights * 100, -100, 100, SliderHLHighlights_ValueChanged);
            AddTextSliderBlock(stackPanel, "Shadows", simplePhotoEditor.highlightShadows * 100, -100, 100, SliderHLShadows_ValueChanged);
            AddTextSliderBlock(stackPanel, "MaskBlur", simplePhotoEditor.highlightMaskBlur * 100, 0, 1000, SliderHLMaskBlur_ValueChanged);
            

            Border border = new Border();
            border.BorderThickness = new Thickness(flyoutBorderThickness);
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.Child = stackPanel;

            FlyoutShowOptions flyoutShowOptions = new FlyoutShowOptions();
            flyoutShowOptions.Placement = FlyoutPlacementMode.RightEdgeAlignedBottom;
            flyout.Content = border;

            flyout.ShowAt(placementTarget, flyoutShowOptions);

        }

        public void Open3DLightingEffectsUI()
        {

            Flyout flyout = new Flyout();

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Width = 130;

            AddTextSliderBlock(stackPanel, "Diffuse Height", simplePhotoEditor.distantDiffuseEffectHeightMapScale * 100.0, 100, 1000, SliderHeightMapScale_ValueChanged);
            AddTextSliderBlock(stackPanel, "Specular Height", simplePhotoEditor.distantSpecularHeightMapScale * 100.0, 100, 1000, SliderSpecHeightMapScale_ValueChanged);
            AddTextSliderBlock(stackPanel, "Specular Exp", simplePhotoEditor.distantSpecularEffectSpecularExponent * 100.0, 10, 600, SliderSpecularExp_ValueChanged);
            
            
            StackPanel stackPanelRight = new StackPanel();
            stackPanelRight.Orientation = Orientation.Vertical;
            stackPanelRight.Width = 130;

            AddTextSliderBlock(stackPanelRight, "Azimuth", simplePhotoEditor.distantAzimuth / (2 * Math.PI) * 360.0, 0, 360, SliderDistantAzimuth_ValueChanged);
            AddTextSliderBlock(stackPanelRight, "Elevation", simplePhotoEditor.distantElevation / (2 * Math.PI) * 360.0, 0, 360, SliderDistantElevation_ValueChanged);
            AddTextSliderBlock(stackPanelRight, "Saturation", simplePhotoEditor.distantDiffuseSaturation * 100.0, 0, 100, SliderDiffuseSaturation_ValueChanged);

            StackPanel stackPanelRightMost = new StackPanel();
            stackPanelRightMost.Orientation = Orientation.Vertical;
            stackPanelRightMost.Width = 130;
            AddTextSliderBlock(stackPanelRightMost, "Specular Kernel", simplePhotoEditor.distantSpecularKernelWidth * 100.0, 100, 1000, SliderDistantSpecularAmount_ValueChanged);
            AddTextSliderBlock(stackPanelRightMost, "Diffuse Kernel", simplePhotoEditor.distantDiffuseKernelWidth * 100.0, 100, 1000, SliderDistantDiffuseAmount_ValueChanged);
            AddTextSliderBlock(stackPanelRightMost, "Gaussian Blur", simplePhotoEditor.gaussianBlurAmount * 100.0, 0, 1200, SliderGaussBlur_ValueChanged);
            //AddTextColorBlock(stackPanelRightMost, "Diffuse Color", DistLightColorButton_Click);


            StackPanel stackPanelCombined = new StackPanel();
            stackPanelCombined.Orientation = Orientation.Horizontal;
            stackPanelCombined.Children.Add(stackPanel);
            stackPanelCombined.Children.Add(stackPanelRight);
            stackPanelCombined.Children.Add(stackPanelRightMost);

            Border border = new Border();
            border.BorderThickness = new Thickness(flyoutBorderThickness);
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.Child = stackPanelCombined;

            FlyoutShowOptions flyoutShowOptions = new FlyoutShowOptions();
            flyoutShowOptions.Placement = FlyoutPlacementMode.RightEdgeAlignedBottom;
            flyout.Content = border;
            flyout.ShowAt(placementTarget, flyoutShowOptions);


        }

        public void OpenHueRotationEffectsUI()
        {
            Flyout flyout = new Flyout();

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            stackPanel.Width = 130;

            AddTextSliderBlock(stackPanel, "Temperature", simplePhotoEditor.hueTemperature * 100.0, -100, 100, SliderHueTemperature_ValueChanged);
            AddTextSliderBlock(stackPanel, "Tint", simplePhotoEditor.hueTint  * 100.0, -100, 100, SliderHueTint_ValueChanged);
            AddTextSliderBlock(stackPanel, "Angle", simplePhotoEditor.hueRotationAngle / (2*Math.PI) * 360.0, 0, 360, SliderHueRotation_ValueChanged);

            StackPanel stackPanelRight = new StackPanel();
            stackPanelRight.Orientation = Orientation.Vertical;
            stackPanelRight.Width = 130;

            AddTextToggleBlock(stackPanelRight, "Posterize", simplePhotoEditor.hueDoPosterize, ToggleSwitchDoPosterize_Toggled);
            AddTextSliderBlock(stackPanelRight, "Red Count", simplePhotoEditor.huePosterizeRedCount, 4, 16, SliderRedCount_ValueChanged);
            AddTextSliderBlock(stackPanelRight, "Green Count", simplePhotoEditor.huePosterizeGreenCount, 4, 16, SliderGreenCount_ValueChanged);
            AddTextSliderBlock(stackPanelRight, "Blue Count", simplePhotoEditor.huePosterizeBlueCount, 4, 16, SliderBlueCount_ValueChanged);


            StackPanel stackPanelCombined = new StackPanel();
            stackPanelCombined.Orientation = Orientation.Horizontal;
            stackPanelCombined.Children.Add(stackPanel);
            stackPanelCombined.Children.Add(stackPanelRight);

            Border border = new Border();
            border.BorderThickness = new Thickness(flyoutBorderThickness);
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.Child = stackPanelCombined;

            FlyoutShowOptions flyoutShowOptions = new FlyoutShowOptions();
            flyoutShowOptions.Placement = FlyoutPlacementMode.RightEdgeAlignedBottom;
            flyout.Content = border;
            flyout.ShowAt(placementTarget, flyoutShowOptions);

        }


        private void VignetteColorButton_Click(object sender, RoutedEventArgs e)
        {   

            ColorPicker colorPicker = new ColorPicker();
            //colorPicker.Color = simplePhotoEditor.vignetteColor;
            colorPicker.ColorSpectrumShape = ColorSpectrumShape.Ring;
            colorPicker.IsHexInputVisible = true;
            colorPicker.ColorChanged += ColorPicker_ColorChanged;

            Flyout flyout = new Flyout();
            flyout.Content = colorPicker;
            //flyout.Closed += ColorFlyout_Closed;
            flyout.ShowAt((FrameworkElement) sender);

            //canvas2d.Invalidate();
        }

        private void DistLightColorButton_Click(object sender, RoutedEventArgs e)
        {

            ColorPicker colorPicker = new ColorPicker();
            colorPicker.ColorSpectrumShape = ColorSpectrumShape.Ring;
            colorPicker.IsHexInputVisible = true;
            colorPicker.ColorChanged += DistLightColorPicker_ColorChanged;

            Flyout flyout = new Flyout();
            flyout.Content = colorPicker;
            flyout.ShowAt((FrameworkElement)sender);
            
        }

        private void DistLightColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            simplePhotoEditor.distantLightColor = sender.Color;
            simplePhotoEditor.apply3DLightingEffects();
            canvas2d.Invalidate();

        }

        private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            simplePhotoEditor.vignetteColor = sender.Color;
            simplePhotoEditor.applySepiaEffects();
            canvas2d.Invalidate();
            
        }

        private void SliderSepia_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //throw new NotImplementedException();
            simplePhotoEditor.sepiaIntensity = e.NewValue / 100.0;
            simplePhotoEditor.applySepiaEffects();
            canvas2d.Invalidate();
        }

        private void SliderVignetteAmount_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.vignetteAmount = e.NewValue / 100.0;
            simplePhotoEditor.applySepiaEffects();
            canvas2d.Invalidate();

        }

        private void SliderVignetteCurve_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.vignetteCurve = e.NewValue / 100.0;
            simplePhotoEditor.applySepiaEffects();
            canvas2d.Invalidate();

        }


        private void SliderEdgeAmount_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.edgeDetectionAmount = e.NewValue / 100.0;
            simplePhotoEditor.applyEdgeDetectionEffects();
            canvas2d.Invalidate();
        }

        private void SliderEdgeContrast_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.edgeDetectionContrast = e.NewValue / 100.0;
            simplePhotoEditor.applyEdgeDetectionEffects();
            canvas2d.Invalidate();
        }

        private void SliderEdgeBlur_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.edgeDetectionBlurAmount = e.NewValue / 100.0;
            simplePhotoEditor.applyEdgeDetectionEffects();
            canvas2d.Invalidate();
        }


        private void SliderEdgeExposure_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.edgeDetectionExposure = e.NewValue / 100.0;
            simplePhotoEditor.applyEdgeDetectionEffects();
            canvas2d.Invalidate();
        }

        private void SliderEdgeOOpacity_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.edgeDetectionOverlayOpacity = e.NewValue / 100.0;
            simplePhotoEditor.applyEdgeDetectionEffects();
            canvas2d.Invalidate();
        }


        private void SliderHLClarity_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.highlightClarity = e.NewValue / 100.0;
            simplePhotoEditor.applyHighlightEffects();
            canvas2d.Invalidate();
        }

        private void SliderHLHighlights_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.highlightHighlights = e.NewValue / 100.0;
            simplePhotoEditor.applyHighlightEffects();
            canvas2d.Invalidate();

        }

        private void SliderHLShadows_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.highlightShadows = e.NewValue / 100.0;
            simplePhotoEditor.applyHighlightEffects();
            canvas2d.Invalidate();


        }

        private void SliderHLMaskBlur_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.highlightMaskBlur = e.NewValue / 100.0;
            simplePhotoEditor.applyHighlightEffects();
            canvas2d.Invalidate();



        }


        private void SliderHueTemperature_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.hueTemperature = e.NewValue / 100.0;
            simplePhotoEditor.applyHueRotationEffects();
            canvas2d.Invalidate();
        }

        private void SliderHueTint_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.hueTint = e.NewValue / 100.0;
            simplePhotoEditor.applyHueRotationEffects();
            canvas2d.Invalidate();
        }

        private void SliderHueRotation_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.hueRotationAngle = e.NewValue / 360.0 * 2 * Math.PI;
            if (simplePhotoEditor.hueRotationAngle > 2 * Math.PI)
                simplePhotoEditor.hueRotationAngle = 2*Math.PI;
            simplePhotoEditor.applyHueRotationEffects();
            canvas2d.Invalidate();
            
        }

     



        private void SliderRedCount_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.huePosterizeRedCount = (int) e.NewValue;
            simplePhotoEditor.applyHueRotationEffects();
            canvas2d.Invalidate();
        }

        private void SliderGreenCount_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.huePosterizeGreenCount = (int)e.NewValue;
            simplePhotoEditor.applyHueRotationEffects();
            canvas2d.Invalidate();
        }

        private void SliderBlueCount_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.huePosterizeBlueCount = (int)e.NewValue;
            simplePhotoEditor.applyHueRotationEffects();
            canvas2d.Invalidate();
        }


        private void SliderHeightMapScale_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.distantDiffuseEffectHeightMapScale = e.NewValue / 100.0;
            simplePhotoEditor.apply3DLightingEffects();
            canvas2d.Invalidate();
        }

        private void SliderSpecHeightMapScale_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.distantSpecularHeightMapScale = e.NewValue / 100.0;
            simplePhotoEditor.apply3DLightingEffects();
            canvas2d.Invalidate();
        }

        private void SliderDiffuseSaturation_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.distantDiffuseSaturation = e.NewValue / 100.0;
            simplePhotoEditor.apply3DLightingEffects();
            canvas2d.Invalidate();
        }

        private void SliderDistantSpecularAmount_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.distantSpecularKernelWidth = e.NewValue / 100.0;
            simplePhotoEditor.apply3DLightingEffects();
            canvas2d.Invalidate();
        }


        private void SliderDistantDiffuseAmount_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.distantDiffuseKernelWidth = e.NewValue / 100.0;
            simplePhotoEditor.apply3DLightingEffects();
            canvas2d.Invalidate();
        }


        private void SliderSpecularExp_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.distantSpecularEffectSpecularExponent = e.NewValue / 100.0;
            simplePhotoEditor.apply3DLightingEffects();
            canvas2d.Invalidate();
        }


        private void SliderDistantAzimuth_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.distantAzimuth = e.NewValue / 360.0 * 2 * Math.PI;
            if (simplePhotoEditor.distantAzimuth > 2 * Math.PI)
                simplePhotoEditor.distantAzimuth = 2 * Math.PI;
            simplePhotoEditor.apply3DLightingEffects();
            canvas2d.Invalidate();


        }

        private void SliderDistantElevation_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.distantElevation = e.NewValue / 360.0 * 2 * Math.PI;
            if (simplePhotoEditor.distantElevation > 2 * Math.PI)
                simplePhotoEditor.distantElevation = 2 * Math.PI;
            simplePhotoEditor.apply3DLightingEffects();
            canvas2d.Invalidate();

        }

        private void SliderGaussBlur_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            simplePhotoEditor.gaussianBlurAmount = e.NewValue / 100.0;
            simplePhotoEditor.apply3DLightingEffects();
            canvas2d.Invalidate();
        }



        private void ToggleSwitchEdgeDGrayscale_Toggled(object sender, RoutedEventArgs e)
        {
            if (((ToggleSwitch)sender).IsOn)
                simplePhotoEditor.edgeDetectionGrayscale = true;
            else
                simplePhotoEditor.edgeDetectionGrayscale = false;

            simplePhotoEditor.applyEdgeDetectionEffects();
            canvas2d.Invalidate();
        }

        private void ToggleSwitchEdgeDOn_Toggled(object sender, RoutedEventArgs e)
        {
            if (((ToggleSwitch)sender).IsOn)
                simplePhotoEditor.edgeDetectionOn = true;
            else
                simplePhotoEditor.edgeDetectionOn = false;

            simplePhotoEditor.applyEdgeDetectionEffects();
            canvas2d.Invalidate();
        }

        private void ToggleSwitchEdgeMaskInvert_Toggled(object sender, RoutedEventArgs e)
        {
            if (((ToggleSwitch)sender).IsOn)
                simplePhotoEditor.edgeDetectionMaskInvert = true;
            else
                simplePhotoEditor.edgeDetectionMaskInvert = false;

            simplePhotoEditor.applyEdgeDetectionEffects();
            canvas2d.Invalidate();
        }



        private void ToggleSwitchEdgeDOverlay_Toggled(object sender, RoutedEventArgs e)
        {
            if (((ToggleSwitch)sender).IsOn)
                simplePhotoEditor.edgeDetectionOverlayImage = true;
            else
                simplePhotoEditor.edgeDetectionOverlayImage = false;

            simplePhotoEditor.applyEdgeDetectionEffects();
            canvas2d.Invalidate();

        }

        private void ToggleSwitchDoPosterize_Toggled(object sender, RoutedEventArgs e)
        {
            if (((ToggleSwitch)sender).IsOn)
                simplePhotoEditor.hueDoPosterize = true;
            else
                simplePhotoEditor.hueDoPosterize = false;

            simplePhotoEditor.applyHueRotationEffects();
            canvas2d.Invalidate();
        }



        private void BlendComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //throw new NotImplementedException();
            //object item = ((ComboBox)sender).SelectedValue;

            BlendEffectMode selmode = (BlendEffectMode)Enum.Parse(typeof(BlendEffectMode), ((ComboBox)sender).SelectedItem.ToString());
            simplePhotoEditor.edgeDetectionBlendEffectMode = selmode;

            StartWritingOutput("Blend Mode :" + selmode.ToString());

            simplePhotoEditor.applyEdgeDetectionEffects();
            canvas2d.Invalidate();
        }


    }
}
