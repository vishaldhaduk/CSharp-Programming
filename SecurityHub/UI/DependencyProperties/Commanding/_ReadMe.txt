Command Framework
-----------------
The commanding framework was added by Nigel Shaw on 11/07/2012 based on
code by Josh Smith here:  http://www.codeproject.com/Articles/28093/Using-RoutedCommands-with-a-ViewModel-in-WPF

This approach cuts out the middleman; the code-behind file. It allows your ViewModel objects 
to be the direct recipient of routed command execution and status query 
notifications. Most examples of using WPF with the MVP, MVC, or MVVM patterns 
involve the use of routed commands. Those commands are accompanied by 
CommandBindings that point to event handling methods in the code-behind 
of the View, which, in turn, delegate to the Presenter/Controller/ViewModel 
associated with that View. Many discussions about MVVM and commanding revolve
around a search to find a way for those RoutedCommands to talk directly 
to the ViewModels. This framework accomplishes that objective.

Benefits
--------
There are several distinct benefits in having the routed commands in the View 
talk directly to the ViewModels. Bypassing the code-behind of the View means 
the View is that much less coupled to a ViewModels. It also means that the 
ViewModel is not dependent on the View’s code-behind to properly handle 
a routed command’s events and delegate those calls off to the correct 
members on the ViewModel objects. Not only that, but it reduces the 
amount of coding required to create a View, which is important when 
working in the Designer-Developer workflow.