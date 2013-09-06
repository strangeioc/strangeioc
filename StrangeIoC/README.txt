/**********************************************/
  Copyright 2013 ThirdMotion, Inc.
 
 	Licensed under the Apache License, Version 2.0 (the "License");
 	you may not use this file except in compliance with the License.
 	You may obtain a copy of the License at
 
 		http://www.apache.org/licenses/LICENSE-2.0
 
 		Unless required by applicable law or agreed to in writing, software
 		distributed under the License is distributed on an "AS IS" BASIS,
 		WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 		See the License for the specific language governing permissions and
 		limitations under the License.
 

StrangeIoC v0.51a

1.0 CONTENTS
This directory contains the following:

1.1 LICENSE-2.0.txt
The full Apache 2 license under which this software is released.

1.2 README.txt
This readme.

1.3 scripts
The actual StrangeIoC code. This is the only bit you actually require in order to use Strange. scripts contains two folders:
 1.3a framework
 The core files that allow binding.
 1.3b extensions
 All other StranceIoC functionalities, such as injection and event dispatch.

1.4 .docs
Full documentation, including an overview, developer's manual, class library documentation, and more.

1.5 examples
Example scenes to help you start using StrangeIoC.



2.0 QUICK START

2.1 Quick start to try the examples

Create a new Unity project.

 - If you downloaded from the Unity Asset Store, import the download and uncheck the '.docs' folder.

 - If you pulled from GitHub, copy the StrangeIoC directory into your new project's Assets folder. Then delete the '.docs' folder.

In your new project, open Assets > StrangeIoC > examples > Assets > scenes > multiple contexts. Double-click on the scene 'main'.

Go to File > Build Settings… . Click 'Add Current'.

Open the remaining scenes, game, social and TestView (in scenes > myfirstproject). In each one, click 'Add Current' to add the scene to the project.

You can now run 'TextView' or 'main' to see how they work. Then open the solution in MonoDevelop to walk through the code.

2.2 Quick start to add StrangeIoC to your project

 - If you downloaded from the Unity Asset Store, import the download AND UNCHECK EVERYTHING EXCEPT StrangeIoC > scripts.

- If you pulled from GitHub, copy the StrangeIoC directory into your project's Assets folder. THEN DELETE EVERYTHING EXCEPT StrangeIoC > scripts.

You're good to go.

3.0 Documentation

There's documentation in the .docs folder. To make sure you've seen the latest, go to our site: http://thirdmotion.github.io/strangeioc/index.html

4.0 Version history

v0.5  - Initial public release
v0.5.1 - Reorganized repo for simplicity/uniformity with Unity Asset Store. Deprecated Sequencer.
v0.5.1a - To avoid errors on first install, renamed 'docs' to '.docs', which makes Unity ignore the folder.
v0.6.0 - Added Signals and improved Cross-Context support