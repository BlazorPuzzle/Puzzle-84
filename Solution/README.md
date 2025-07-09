# Puzzle #84: Breadcrumb Components

We're reviewing the breadcrumbs from the last episode and putting them into a component.

YouTube Video: https://youtu.be/

Blazor Puzzle Home Page: https://blazorpuzzle.com

## The Challenge

This is a .NET 9 Blazor Web App with global server interactivity.

In Puzzle 83, we implemented breadcrumbs as a navigation feature.

Our initial solution showed adding some navigation elements and event handlers to every page.
How can we wrap this into a component and abstract away that feature?

## The Solution

The solution abstracts the breadcrumb functionality into reusable components and a shared state class, eliminating the need to add navigation code to every page.

### BreadCrumbState Class

The `BreadCrumbState` class serves as the central state management for breadcrumbs:

```csharp
public class BreadCrumbState
{
    public Stack<BreadCrumb> BreadCrumbs { get; } = new();
}

public record BreadCrumb(string Title, string Url);
```

- Uses a `Stack<BreadCrumb>` to maintain navigation history in LIFO (last in, first out) order
- The `BreadCrumb` record stores the display title and URL for each breadcrumb
- Registered as a service in the DI container to share state across components

### BreadcrumbNav Component

The `BreadcrumbNav` component handles the "Back" button functionality:

- **Required Parameter**: `DisplayText` - the title to display for the current page
- **Automatic Breadcrumb Addition**: In `OnAfterRenderAsync`, it adds the current page to the breadcrumb stack if it's not already there
- **Back Navigation**: Provides a back button that pops the last breadcrumb and navigates to the previous URL
- **Navigation Handling**: Registers a `LocationChangingHandler` to manage state when navigation occurs
- **State Coordination**: Uses a `popped` flag to prevent duplicate breadcrumb additions when navigating back

### Breadcrumbs Component

The `Breadcrumbs` component displays the breadcrumb trail:

- **Display Logic**: Shows all breadcrumbs except the current page (skips the first item in the stack)
- **Interactive Links**: Each breadcrumb is clickable and navigates back to that specific page
- **Smart Navigation**: When clicking a breadcrumb, it removes all breadcrumbs after that point in the navigation history
- **Responsive Updates**: Listens for navigation changes and updates the display accordingly

### Key Benefits

1. **Abstraction**: Pages only need to include `<BreadcrumbNav DisplayText="Page Name" />` 
2. **Automatic Management**: Breadcrumbs are added and removed automatically
3. **Clean Navigation**: Back button and breadcrumb links handle navigation logic internally
4. **Shared State**: All components share the same breadcrumb state through dependency injection
5. **Memory Management**: Components properly dispose of event handlers to prevent memory leaks

This solution transforms the breadcrumb feature from requiring manual implementation on each page to a simple, declarative component that handles all the complexity internally.

