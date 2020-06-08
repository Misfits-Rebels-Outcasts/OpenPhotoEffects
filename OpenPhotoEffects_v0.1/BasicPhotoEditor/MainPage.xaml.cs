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
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BasicPhotoEditor
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SimplePhotoEditor simplePhotoEditor = null;
        DependencyObject placementTarget = null;
        double flyoutBorderThickness = 0.5;

        public MainPage()
        {
            this.InitializeComponent();
            simplePhotoEditor = new SimplePhotoEditor(this);

            tempInitLoadFile("/Assets/Kitty.jpg");
            placementTarget = canvas2d;

        }

        
        public async void onCanvasDraw(CanvasControl sender, CanvasDrawEventArgs args)
        {

            if (simplePhotoEditor == null)
            {
                return;
            }

            if (simplePhotoEditor.savefile == null)
            {
                args.DrawingSession.DrawEllipse(250, 85, 80, 30, Colors.Black, 3);
                args.DrawingSession.DrawText("Load Photo", 200, 70, Colors.Black);

                return;
            }

            simplePhotoEditor.DrawBitmap(canvas2d, args.DrawingSession);

        }

        public void StartWritingOutput(string msgstr, int add = 0)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (add == 1)
                    TextConsole.Text += System.Environment.NewLine + msgstr;
                else
                    TextConsole.Text = msgstr;
            });
        }

        public async Task tempInitLoadFile(string filename)
        {
            //BitmapImage bitmapImage = new BitmapImage(new Uri(this.BaseUri, "/Assets/image.jpg"));

            try
            {
                //StorageFile savefile = await KnownFolders.PicturesLibrary.GetFileAsync("Kitty.jpg");
                //StorageFile savefile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(this.BaseUri, "/Assets/Kitty1.png"));
                //string filestr = "/Assets/" + filename;
                StorageFile savefile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(this.BaseUri, filename));


                if (savefile != null)
                {
                    simplePhotoEditor.savefile = savefile;
                    
                    await simplePhotoEditor.LoadCanvasBitmap();

                    simplePhotoEditor.CreatePreviewBitmap();

                }

                canvas2d.Invalidate();
            }
            catch (Exception e)
            {
                StartWritingOutput("Error Loading Image : " + e.Message,1);

                //StartWritingOutput("Pitures Lib : " + KnownFolders.PicturesLibrary.DisplayName,1);

            }
            
        }


        public async Task PickFile()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".tif");
            openPicker.FileTypeFilter.Add(".tiff");
            openPicker.FileTypeFilter.Add(".gif");


            StorageFile savefile = await openPicker.PickSingleFileAsync();

            if (savefile != null)
            {
                simplePhotoEditor.savefile = savefile;
                //SaveFileTextBox.Text = savefile.Path;

                simplePhotoEditor.ResetBitmapResources();

                await simplePhotoEditor.LoadCanvasBitmap();
                
                simplePhotoEditor.CreatePreviewBitmap();

                canvas2d.Invalidate();

            }

            
        }

        public async Task PickSaveFile()
        {
            if (simplePhotoEditor.canvasRenderTarget == null)
            {
                StartWritingOutput("No Effects Applied");
                return;
            }


            FileSavePicker picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.SuggestedFileName = "SavedImage";
            picker.DefaultFileExtension = ".png";
            picker.FileTypeChoices.Add("Image Files (*.png, *.jpg, *.tiff, *.gif) ", new List<string> { ".png", ".jpg", ".tiff", ".gif" });

            StorageFile outfile = await picker.PickSaveFileAsync();
            if (outfile != null)
            {
                CanvasBitmapFileFormat canvasBitmapFileFormat = CanvasBitmapFileFormat.Png;
                
                
                string filepath = outfile.Path;
                filepath = filepath.ToLower();
                int index = filepath.IndexOf(".jpg");
                if (index > 0)
                    canvasBitmapFileFormat = CanvasBitmapFileFormat.Jpeg;
                else
                {
                    index = filepath.IndexOf(".png");
                    if (index > 0)
                        canvasBitmapFileFormat = CanvasBitmapFileFormat.Png;
                    else
                    {
                        index = filepath.IndexOf(".tif");
                        if (index > 0)
                            canvasBitmapFileFormat = CanvasBitmapFileFormat.Tiff;
                        else
                        {
                            index = filepath.IndexOf(".gif");
                            if (index > 0)
                                canvasBitmapFileFormat = CanvasBitmapFileFormat.Gif;

                        }
                    }
                }

                ApplyFilterEffects(true);
                

                await simplePhotoEditor.SaveCanvasBitmap(outfile, canvasBitmapFileFormat);

                ApplyFilterEffects(false,null,false);

                
                canvas2d.Invalidate();
                
            }

        }


        private async void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            await PickFile();


        }


        private void ApplyEffect_Click(object sender, RoutedEventArgs e)
        {
            if (simplePhotoEditor == null)
                return;

            if (simplePhotoEditor.canvasBitmap == null)
            {
                StartWritingOutput("Photo not loaded", 1);

            }

            ApplyFilterEffects();
            
        }

        private void ApplyFilterEffects(bool isSaving = false, string effectName = null, bool showUI = true)
        {
            if (effectName == null)
                effectName = (string) NameOfEffect.Text;

            if (effectName == "Original")
            {
                simplePhotoEditor.applyOriginalEffects(isSaving);
                
            }
            else if (effectName == "Edge Detection")
            {
                if ((isSaving == false) && (showUI))
                    OpenEdgeDetectionEffectsUI();
                simplePhotoEditor.applyEdgeDetectionEffects(isSaving);
                
            }
            else if (effectName == "Highlight & Shadows")
            {
                if ((isSaving == false)  && (showUI))
                    OpenHightlightShadowsEffectsUI();
                simplePhotoEditor.applyHighlightEffects(isSaving);

            }
            else if (effectName == "Hue Rotation")
            {
                if ((isSaving == false) && (showUI))
                    OpenHueRotationEffectsUI();
                simplePhotoEditor.applyHueRotationEffects(isSaving);

            }
            else if (effectName == "3D Lighting")
            {
                if ((isSaving == false) && (showUI))
                    Open3DLightingEffectsUI();
                simplePhotoEditor.apply3DLightingEffects(isSaving);

            }
            else
            {
                if ((isSaving == false)  && (showUI))
                    OpenSepiaEffectsUI();
                simplePhotoEditor.applySepiaEffects(isSaving);
                
            }

            if (effectName == "Original")
            {
                EditItem.Visibility = Visibility.Collapsed;

            }
            else
                EditItem.Visibility = Visibility.Visible;


            if (isSaving==false)
                canvas2d.Invalidate();
        }

        private async void  SaveImage_Click(object sender, RoutedEventArgs e)
        {
            await PickSaveFile();
            canvas2d.Invalidate();
        }

        private async Task AddThumbnail_FreeResource(StackPanel stackPanel, CanvasBitmap canvasBitmap, string effectName)
        {
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            if (canvasBitmap != null)
            {
                await canvasBitmap.SaveAsync(randomAccessStream, CanvasBitmapFileFormat.Png);

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.SetSource(randomAccessStream);
               
                Image image = new Image();
                image.Width = simplePhotoEditor.thumbnailSize;
                image.Height = simplePhotoEditor.thumbnailSize;
                image.Margin = new Thickness(10, 5, 10, 5);
                image.Source = bitmapImage;

                TextBlock textBlock = new TextBlock();
                textBlock.Text = effectName;
                textBlock.Margin = new Thickness(1, 1, 1, 10);
                textBlock.TextAlignment = TextAlignment.Center;
                

                StackPanel stackPanelBlock = new StackPanel();
                stackPanelBlock.Orientation = Orientation.Vertical;
                stackPanelBlock.Children.Add(image);
                stackPanelBlock.Children.Add(textBlock);
                stackPanelBlock.Tapped += Image_Tapped;

                stackPanel.Children.Add(stackPanelBlock);

                canvasBitmap.Dispose();
                canvasBitmap = null;

               
            }


        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string effectName = ((TextBlock)((StackPanel)sender).Children[1]).Text;

            ApplyFilterEffects(false,effectName);

            NameOfEffect.Text = effectName;

        }

        
        private async void GenThumbnails_Click(object sender, RoutedEventArgs e)
        {
            Flyout flyout = new Flyout();

            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            //stackPanel.Orientation = Orientation.Horizontal;
            //stackPanel.Height = 140;
            //stackPanel.Width = 140;

            CanvasBitmap canvasBitmap = simplePhotoEditor.GenerateThumbnail();
            CanvasBitmap canvasBitmapSepia = simplePhotoEditor.applySepiaEffects(canvasBitmap);
            CanvasBitmap canvasBitmapHue = simplePhotoEditor.applyHueRotationEffects(canvasBitmap);
            CanvasBitmap canvasBitmapHighlight = simplePhotoEditor.applyHighlightEffects(canvasBitmap);
            CanvasBitmap canvasBitmapEdge = simplePhotoEditor.applyEdgeDetectionEffects(canvasBitmap);
            CanvasBitmap canvasBitmap3D = simplePhotoEditor.apply3DLightingEffects(canvasBitmap);
            
            await AddThumbnail_FreeResource(stackPanel, canvasBitmap, "Original");
            await AddThumbnail_FreeResource(stackPanel, canvasBitmapEdge, "Edge Detection");
            await AddThumbnail_FreeResource(stackPanel, canvasBitmapSepia, "Sepia");
            await AddThumbnail_FreeResource(stackPanel, canvasBitmapHighlight, "Highlight & Shadows");
            await AddThumbnail_FreeResource(stackPanel, canvasBitmapHue, "Hue Rotation");            
            await AddThumbnail_FreeResource(stackPanel, canvasBitmap3D, "3D Lighting");
            
            
            Border border = new Border();
            border.BorderThickness = new Thickness(flyoutBorderThickness);
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.Child = stackPanel;

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = border;


            FlyoutShowOptions flyoutShowOptions = new FlyoutShowOptions();
            flyoutShowOptions.Placement = FlyoutPlacementMode.Left;
            flyout.Content = scrollViewer;
            //flyout.ShowAt(TextConsole, flyoutShowOptions);
            flyout.ShowAt(placementTarget, flyoutShowOptions);

        }

        private async void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                //NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
                //StartWritingOutput("Settings not yet implemented", 1);
                LaunchURI();
            }
            else if (args.InvokedItemContainer != null)
            {
                string itemTag = args.InvokedItemContainer.Tag.ToString();
                if (itemTag=="OpenFile")
                {
                    await PickFile();
                }
                else if (itemTag == "Gallery")
                {
                    await tempInitLoadFile("/Assets/Kitty1.jpg");
                    
                }
                else if (itemTag == "ApplyEffect")
                {
                    GenThumbnails_Click(null, null);
                }
                else if (itemTag == "EditEffect")
                {
                    ApplyEffect_Click(null, null);
                }
                else if (itemTag == "SaveFile")
                {
                    await PickSaveFile();
                    canvas2d.Invalidate();

                }
                else if (itemTag == "SourceCode")
                {
                    LaunchURI();

                }

            }

        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            

            /*
            NavigationMain.IsSettingsVisible = false;
            NavigationMain.IsBackButtonVisible = NavigationViewBackButtonVisible.Collapsed;


            NavigationViewItem settings = (NavigationViewItem)NavigationMain.SettingsItem;
            settings.Content = "Project Source";
            ToolTipService.SetToolTip(settings, "Project Source");
            //StartWritingOutput("Settings");
            //placementTarget = settings;
            */
        }


        private async void LaunchURI()
        {
            //Project Source Code
            Uri projectSourceURI = new Uri(@"https://github.com/Misfits-Rebels-Outcasts/OpenPhotoEffects");
            
            bool success = await Launcher.LaunchUriAsync(projectSourceURI);
            
        }

        private void NavigationMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //double hh = NavigationMain.ActualHeight;
            double hh = ContentFrame.ActualHeight; 
            double marginThickness = (hh - canvas2d.ActualHeight) / 2.0;
            canvas2d.Margin = new Thickness(0, marginThickness, 0,marginThickness);


        }
    }
}
