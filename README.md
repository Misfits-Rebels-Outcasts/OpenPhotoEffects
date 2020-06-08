# OpenPhotoEffects
A photo effects application and editor with real time visualization (UWP). 

Open Photo Effects Editor

The purpose of this project is to provide a basic template for a Photo Effect Editor in UWP.

By taking advantage of Win2D's capability for processing images very rapidly using GPU and video memory,
this app allows real time update of imaging effects. The user will be able visualize 
immediate change to the original photo as he or she works on the slider to modify the parameters
of each effect.

This project provides the basic features for loading a photo or image, applying an effect to the image, and saving the filtered image.
It also supports thumbnails, so that the user can see the output of each effect before applying it
to the photo. 

At this initial stage of the project, just a few effects have been implemented. However, these are sufficient 
for basic photo touch up such as changing the image temperature, tints , highlights and shadows.

Furthermore, a few advanced effects like edge detection, 3D lighting , posterization are also included, to
demonstrate some of the interesting stuff that may be produced with just a few lines of code.

The author believes this UWP Opensource Photo Editor will benefit many, especially the following groups.

Learners of Win2D can make use of this application to visualize and explore the various parameters provided
by each effect in Win2D.

A programmer looking into writing a photo effects editor will not need to start from scratch. This project may be used
as a basic template that can be extended further with more effects and features (such as text overlays) to make it a full fledge product.

Experienced artists trained in a paint software (such as PhotoShop or Paint Shop Pro) may
be adventurous enough to create their very own 'custom effect' app for listing on the app store.

**Flexibility in Simplicity - How it works**

This tool takes an (0)Original size photo and resizes it to produce a (1)Preview and several (2)Thumbnails.
Whether it is (0), (1) or (2), the result is a bitmap (known as the workingBitmap) that can
be applied with an effect. The programmer will just need to write a single function

*CanvasRenderTarget applyXXXXXEffects(CanvasBitmap workingBitmap)*

that will take as input the workingBitmap, and return a new bitmap of the same size.
Doing so will add a new effect to the existing project. Many examples of how such an effect is written is provided in the source code.

To help programmers understand the source, the user interface has been kept simple, linking each effect 
to a Flyout of 'minimalistic' style, containing just sliders and on-off switches. There is also a
(invisible) TextConsole, which visibility can be turned on for displaying debugging messages.


**Related Projects**

A related project by the same author that also employs Win2D extensively is the Open Screen Recorder at

https://github.com/Misfits-Rebels-Outcasts/OpenScreenRecorder

**Future Plans**

It is quite easy to modify the Photo Editor into a Video Editor using the same structure of the current project.
To do this, the loading of image files can just simply be replaced with loading of movie files.
The author intents to write a basic Video Editor in the same style as the current project. Stay-tuned.
