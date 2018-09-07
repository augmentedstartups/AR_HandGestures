# Trigger Gestures

Watch the Full Video Tutorial Here:

## Introduction
Hey guys and welcome back. In this video, we’ll be shooting kai blasts through our hands. (dragonball scene). So, this tutorial will be broken down into three fundamental parts.

1.	Triggering the cube color, so the cube color will change to red when we trigger a grab gesture and blue when we trigger a release gesture.
2.	The second part is we want the cube to follow our hand position so when we clench our fists, or open our hands, we want to instantiate our bullet from the hands position and not some arbitrary position.
3.	This brings us to three, which is prepping our sphere as a bullet or projectile that shoots from our hands. In the demo you see some particle effects added to the sphere. That part is optional and just involves replacing the sphere with particle fx from the Unity Asset store.
So essentially why we are doing this, is so you can understand where these gesture triggers classes are and also how to adapt these manomotion API’s for your own applications. 
A note to remember, I have disabled the background mode which is defaulted to a white background for simplicity reasons, if you cannot find a white background in your environment, then make sure to set enable the background color pallet so you can select colors that match your background.
So without further a due, let’s get into it.

## What will we need?
So, we’ll require the base-project which you can get from my GitHub repo, https://github.com/reigngt09/AR_HandGestures/tree/master/B_Trigger_Gestures

You will also need to have the latest Manomotion SDK to import any package that has not been exported from the base-project.
Starting the Project
So, start a new Unity Project, Let’s call it, Import those two files, first the base project called Trigger_Gestures and then Manomotion SDK to fill in excluded dependencies. Open up the Base_HandGestures scene and don’t forget to input your Manomotion License key under Manomotion Manager.
So this scene is similar to our last Rotation project, except that RotationManager is now called Gizmo Manager and handles all the these text UI element. They show the state transitions of trigger and continuous gestures as well as the Manoclasses that are operational. 

## Triggering Cube Color 
So what we are going to do now is change the color of this cube whenever we trigger a grab or release gesture.
So lets go over to Gizmo Manager and open up this script in visual studio or monodevelop.
Lets create first a 
```
public GameObject Object;
```

This Object will be filled by our cube game object.
Now we need to look for the DisplayTriggerGesture method and within the function we want to locate the grab and release cases. Under grab we type
```
Object.GetComponent<Renderer>().material.color = Color.red;
```
Lets copy and paste this for the release and make the color blue. 
Before we run the app to test if this works, we need go to main camera and drag and drop this cube into the Object field under Gizmo Manager.
We can now build and run the app, skip ahead to later in the video if you don’t know how to build the app, otherwise let’s continue.

You should now be able to change the colour of the cube with a clench of your fist.
  
## Tracking Hand Position with Cube
Now to ensure the kai blasts shoot from the hand, we need to obtain the hand position on screen. We can do this by getting the palm center transform position. The palm center class is held within ManoVisualization script. Let’s open up the script.
So, we already have the cube incorporated into the script, so all we have to do is change this from sphere to object in 2 places and then locate the method that contains the palm center goodies. We are going to type in this little line of code. 
layering_object.gameObject.transform.position = (3.0f * (palm_cent) + new Vector3(-1.5f, 0.0f, -9.0f)); 
This basically transfers the positional data of the palm center of the hand to cube. We also made some calibrations to ensure that the cube follows the hand more precisely. 
Now back in Unity, we can drag and drop our cube prefab into the Layering object field under Mano Visualization. Also, let’s resize our cube to 0.1 along all axes’ and then we can rebuild our app to see if everything works so far.

## Shooting Spheres
So, in this part we are going to create the projectile that gets fired when we trigger a grab or release gesture with our hands. Back in GizmoManager, we can add in the following variables:
    public float bulletSpeed = 10; 
    public Rigidbody bullet;  
And then we add in the function that will accelerate the projectile using the cubes position as the starting position,
```
    void Fire() //RK
    {

Rigidbody bulletClone = (Rigidbody)Instantiate(bullet, Object.transform.position,   transform.rotation);
        bulletClone.velocity = bullet.transform.forward * bulletSpeed;

    }
```
Now that we have defined our function, we can call the fire function when the release or grab gesture is called. We’ll trigger in both cases.
```
Fire();
```
*	Save the script and back in unity…
*	We need to create a sphere game object. 
*	Drag it as a prefab.
*	We are going to add a rigid body to it and disable gravity. 
*	Once we have refined our prefab, we can drag this sphere projectile under bullet.
*	Don’t forget to delete the original sphere from the hierarchy. 

## Building the App
*	Let’s click File>> Build Settings. Change our source to Android, Add open scene, set to internal
*	Don’t forget to set players settings. Disable multithreaded rendering, Change our package ID as well as our Android OS package to Nougat 7.0. And finally ARCore checked. This should be second nature by now.
*	So now following the process, when you clench your fist and release you’ll be shoot some spheres. Really cool right… now you can make this even more realistic but that we’ll leave as an optional exercise. 

## Enhancing the project
So now that you know the basics, you can head over to the unity asset store and add in any spell or particle effects prefab that you want whether it may be free or paid. I recommend using this one called Magic Spell by RDR. It will give your kai blasts that extra Oomph for your app.

You can download the Asset from here:
http://bit.ly/UnitySpell

Okay that is it from me, thank you for watching and see you in the next lectures

*	FREE AR Business Card Crash Course:
http://bit.ly/FREEARBusinessCardCrashCourse

*	Enroll Here in the Full ARCore MasterClass Course: 
http://bit.ly/ARCoreCourse-Teachable

*	AR Courses on Udemy:  http://bit.ly/UdemyARCourses









