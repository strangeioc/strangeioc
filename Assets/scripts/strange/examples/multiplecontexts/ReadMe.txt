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
 
 ===========================================================================  


The MultipleContexts example demonstrates how to build modular applications with strange.

Inside this package are not one project but three, each of which is capable of running as
either a self-standing application or as part of an integrated whole. With strange, you
can build your applications as small, abstracted units, then bind them together when
it's time to integrate. This not only decreases interdependency, but vastly increases
testability and development speed.

Since this is just a demonstration, the three projects are vastly simplified from what a
real game might look like. The key is to give you an idea how you might go about
creating multiple components and tie them all together.

The components:
main - The top-level component that loads the others
game - A scene that pretends to be a game (but it doesn't do anything cool, so don't get excited)
social - A scene that mocks a component for Facebook integration
