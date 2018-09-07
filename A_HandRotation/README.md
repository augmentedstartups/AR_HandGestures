# Hand Rotation Tutorial in Unity
Watch the Youtube Tutorial over Here : https://youtu.be/ivcZJluOE0I

## Introduction
Hey guys and welcome back. In this tutorial I’ll be showing you how to use the force to rotate 3d objects by channeling chi from your hands. Haha So in other words the 3d game object rotates based on the rotation value detected using Manomotion SDK.
It’s really quite simple to do this since manomotion does all the hard work of hand gesture recognition and analysis to obtain the hand rotation along an axis relative to the camera image plane.
So lets get into it

## What will we need?
So, we’ll require the base-project which you can get from my github repo, https://github.com/reigngt09/AR_HandGestures/edit/master/A_HandRotation.

You will also need to have the latest Manomotion SDK to import any package that has not been exported from the base-project.
The project
So, start a new Unity Project, Let’s call it Hand_Rotation, Import those two files, first the base project and then Manomotion SDK. Also don’t forget to input your Manomotion License key under Manomotion Manager
Once imported we can go ahead and open up the rotation scene. So what we have here is similar to what we had in the original Manomotion demo but a few things are slimmed down a bit. Now because we are only doing rotation, we have our rotation gizmo, dead center.

So first up we are going drag it a bit to the side. If you want you can quickly build and run this app just to validate the rotation mechanics, you can skip to the building the app section in the video and then come back here for instructions on building the app.

Otherwise let’s carry on. What we are going to do now, is we are going to create a cube and manipulate that cube using our hands rotation. Now it does not have to be a cube, it can be any 3D game object or unity asset.

## Creating the cube
So first let go to game object >>3d object>>cube. And I want you to align that cube by the main camera. You should be able to see it in scene view. 

Once that is done drag the cube under main camera and delete the sphere.

Now because we fired the sphere, there is a vacancy under the Mano visualization class. Just drag the cube prefab into this spot. If we want to see the cube overlaid on the camera’s image, we need to activate the “Show_hand_layer” check box.
If you want you can go ahead and build and run this app again just check if the cube is indeed augmented to the cameras view. What you will see is that when you stick your hand out, it will occlude the cube, which is a really cool feature of manomotion! 
Rotation

Since we got the cube working, we now need to add some rotation to it. To do that, go to Rotation Manager and open up the Rotation Script in Mono develop or Visual Studio.

We first create a public gameobject and call it object. This is where we will put in our cube or any 3d asset. Also we will add in another Transform called roationObjectTransform

Then under Set RotationGizmoParts, we are going to copy and past one of these lines and change it for our cube. It simply becomes Object.transform.

Over here we have some semi hidden code under DisplayMethods. Expand it and we are going to copy and paste some lines here. We will also change this to rotationObjectTransform and the rest stays the same. We duplicate that for the else statement but take away the negation. 

In a nutshell, what is going on here is that we want to rotate our cube or 3d game object by the same value as the hand. Now you can decide which axis you want to rotate. Im going to do it across the y-axis. So im going to move angle to the middle.
Lastly before we build the app, we save everything and then under Rotation Manager, Rotation script, we drag and drop our cube prefab under object.

## Building the App
*	Let’s click File>> Build Settings. Change our source to Android, Add open scene, set to internal
*	Don’t forget to set players settings. Disable multithreaded rendering, Change our package ID as well as our Android OS package to Nougat 7.0. And finally ARCore checked. This should be second nature by now.
*	DEMO
*	So now following the process you can see that we are able to rotate the cube. We can add a multiplier to the code to make the cube spin faster with respect to our hand, if we want to.

## Enhancing the project
So now that you know the basics, you can head over to the unity asset store and add in any prefab that you want whether it may be free or paid. I bought this back Muscles Animated Asset. And you can see the results are amazing once we are able to rotate it with our hands. To do this all you have to do is perform the same process as the cube or you can make the 3d asset a child of the cube and just disable the mesh render for the cube.

Links to this prefab can be found in the description below:
Okay that is it from me, thank you for watching and see you in the next lectures

*	FREE AR Business Card Crash Course:
http://bit.ly/FREEARBusinessCardCrashCourse

*	Enroll Here in the Full ARCore MasterClass Course: 
http://bit.ly/ARCoreCourse-Teachable

*	AR Courses on Udemy:  http://bit.ly/UdemyARCourses




