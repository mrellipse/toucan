## Service Assembly
This is the 'domain' or 'business' layer. It contains concrete classes to be injected into the [composition root](http://blog.ploeh.dk/2011/07/28/CompositionRoot/) of your web (or console) application.

> A Composition Root is a (preferably) unique location in an application where modules are composed together.

Any classes created in here should implement interfaces or abstract classes defined in the 'contracts' assembly. 

> Program to an interface, not an implementation.
