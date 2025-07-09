# Puzzle #83: Breadcrumbs

Carl and Jeff want to know how to implement a back button on every page with multiple levels.

YouTube Video: https://youtu.be/

Blazor Puzzle Home Page: https://blazorpuzzle.com

## The Challenge

This is a .NET 9 Blazor Web App with global server interactivity.

We want to add the idea of "breadcrumbs" to the app, implementing a "Back" button in every screen that takes us back to where we came from.

The browser back button is not enough. We want to give our user a chance to save changes. We also want to make sure that we can go multiple steps back, not just one step.

How can we achieve this?

## The Solution

In the solution we're going to use solutions from the last project (Puzzle 82) where we created a StateBag for shared data. We're also going to call on Puzzle 74 (Canceling Navigation) to intercept navigations before they happen. So, let's start by adding this class to the project:

*StateBag.cs*:

```c#
namespace Puzzle84;

public class StateBag
{
	public Stack<string> BreadCrumbs { get; } = new();
}
```

A Stack is a Last In First Out buffer. The last thing you push onto the stack is the item you get the next time something is popped off it. Push means "put" and Pop means "get", so to speak. This is the perfect construct for a trail of breadcrumbs.

Add StateBag as a scoped service in *Program.cs*:

```c#
builder.Services.AddScoped<StateBag>();
```

We are going to add all three of these lines to the top of *Home.razor*, "*Counter.razor*" and "*Weather.razor*":

```c#
@inject StateBag stateBag
@inject NavigationManager navigationManager
@implements IDisposable
```

Add the following code block to *Home.razor*:

```c#
@code 
{
    private IDisposable? navigationHandler;
    bool popped = false;

    protected override void OnInitialized()
    {
        navigationHandler = navigationManager.RegisterLocationChangingHandler(OnLocationChanging);
    }

    private async ValueTask OnLocationChanging(LocationChangingContext context)
    {
        if (popped)
        {
            popped = false;
            return;
        }
        stateBag.BreadCrumbs.Push("/");
    }

    private void GoBack()
    {
        if (stateBag.BreadCrumbs.Count > 0)
        {
            var url = stateBag.BreadCrumbs.Pop();
            popped = true;
            navigationManager.NavigateTo(url);
        }
    }

    public void Dispose()
    {
        navigationHandler?.Dispose();
    }
}
```

In [Puzzle #74](https://github.com/BlazorPuzzle/Puzzle-74/tree/master/Solution) we learned how to use the `RegisterLocationChangingHandler` method of the `NavigationManager` to create a handler that fires BEFORE a navigation. That will allow us to push our address into the `StateBag.BreadCrumbs` stack.

In `OnInitialized()` we register our handler, and in `Dispose()` we Dispose (or unregister) it.

At the top of the page we are now going to add our back button:

```html
<h1>Blazor Puzzle #83: Breadcrumbs</h1>

@if (stateBag.BreadCrumbs.Count > 0)
{
    <button class="btn btn-secondary mb-3" @onclick="GoBack">
        <span class="bi bi-arrow-left-short"></span> Back
    </button>
}
```

This basically says "if there is a place to go back to, show the Back button and link it to our `GoBack()` method".

The `GoBack()` method makes use of a local boolean `popped`, which we set to true:

```c#
private void GoBack()
{
    if (stateBag.BreadCrumbs.Count > 0)
    {
        var url = stateBag.BreadCrumbs.Pop();
        popped = true;
        navigationManager.NavigateTo(url);
    }
}
```

Before we navigate away, the `OnLocationChanging` handler kicks in:

```c#
private async ValueTask OnLocationChanging(LocationChangingContext context)
{
    if (popped)
    {
        popped = false;
        return;
    }
    stateBag.BreadCrumbs.Push("/");
}
```

Notice that if `popped` is true, we do NOT push our address onto the stack. That is so we don't get stuck in an infinite loop of navigation.

This same exact logic is expressed in *Counter.razor* and *Weather.razor*, the only difference being the route that they push onto the stack.

In *Counter.razor:*

```c#
private async ValueTask OnLocationChanging(LocationChangingContext context)
{
    if (popped)
    {
        popped = false;
        return;
    }
    stateBag.BreadCrumbs.Push("/Counter");
}
```

And in *Weather.razor*:

```c#
private async ValueTask OnLocationChanging(LocationChangingContext context)
{
    if (popped)
    {
        popped = false;
        return;
    }
    stateBag.BreadCrumbs.Push("/Weather");
}
```

## Bonus Challenge

We challenged our viewers to turn this into a reusable component so we did not have to repeat the plumbing code in every page. Go ahead and give it a try! Email us a blazorpuzzle@appvnext.com with your solution!

Boom!
