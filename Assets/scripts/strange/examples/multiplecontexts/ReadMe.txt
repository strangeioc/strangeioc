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
