# Puzzle #84: Breadcrumb Components 

We're reviewing the breadcrumbs from the last episode and putting them into a component.

YouTube Video: https://youtu.be/WabhfwYBdwg

Blazor Puzzle Home Page: https://blazorpuzzle.com

## The Challenge

This is a .NET 9 Blazor Web App with global server interactivity.

In Puzzle 83, we implemented breadcrumbs as a navigation feature.

Our initial solution showed adding some navigation elements and event handlers to every page.
How can we wrap this into a component and abstract away that feature?

## The Solution using Templated Controls

The templated controls solution builds upon the basic breadcrumb implementation by adding customizable templates, allowing developers to control both the appearance and behavior of breadcrumb separators and links while maintaining the same core functionality.

### BreadCrumbState Class

The `BreadCrumbState` class remains unchanged from the basic solution:

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

The `BreadcrumbNav` component provides the same back button functionality as the basic solution:

- **Required Parameter**: `DisplayText` - the title to display for the current page
- **Automatic Breadcrumb Addition**: In `OnAfterRenderAsync`, it adds the current page to the breadcrumb stack if it's not already there
- **Back Navigation**: Provides a back button that pops the last breadcrumb and navigates to the previous URL
- **Navigation Handling**: Registers a `LocationChangingHandler` to manage state when navigation occurs
- **State Coordination**: Uses a `popped` flag to prevent duplicate breadcrumb additions when navigating back

### Breadcrumbs Component with Template Support

The enhanced `Breadcrumbs` component introduces **templated controls** for maximum customization:

#### Template Parameters

1. **Separator Template**: Allows customization of the separator between breadcrumb items
   - **Default**: Simple text separator " - "
   - **Customizable**: Any HTML or Razor markup can be used as a separator

2. **LinkTemplate**: Provides complete control over how breadcrumb links are rendered
   - **Parameters**: Receives a tuple with `(BreadCrumb item, Action<string> navigateTo)`
     - **Why a Tuple?**: The template needs both data (`BreadCrumb`) and behavior (`navigateTo` delegate)
     - **BreadCrumb item**: Contains the breadcrumb data (`Title` and `Url` properties)
     - **Action<string> navigateTo**: A delegate that executes the navigation logic when called
     - **Template Context**: The tuple is passed as the template's context, accessible via the `Context` parameter
   - **Default**: Simple anchor links with text
   - **Customizable**: Can render buttons, styled links, icons, or any custom markup

#### Template Usage Examples

**Custom Separator:**
```razor
<Breadcrumbs>
    <Separator>
        <span class="mx-1">|</span>
    </Separator>
</Breadcrumbs>
```

**Custom Link Template:**
```razor
<Breadcrumbs>
    <LinkTemplate Context="breadcrumb">
        <button class="btn btn-link" @onclick="() => breadcrumb.navigateTo(breadcrumb.item.Url)">
            @if (breadcrumb.item.Title == "Home")
            {
                <text>🏠</text>
            }
            <text>@breadcrumb.item.Title</text>
        </button>
    </LinkTemplate>
</Breadcrumbs>
```

#### Core Functionality

- **Display Logic**: Shows all breadcrumbs except the current page (skips the first item in the stack)
- **Interactive Navigation**: Template-rendered elements can trigger navigation back to specific pages
- **Smart Navigation**: When navigating to a breadcrumb, it removes all breadcrumbs after that point
- **Responsive Updates**: Listens for navigation changes and updates the display accordingly

### Key Benefits of Templated Controls

1. **Complete Customization**: Templates allow full control over both appearance and behavior
2. **Flexible Rendering**: Separators and links can be styled to match any design system
3. **Icon Support**: Easy integration of icons, emojis, or custom graphics
4. **Accessibility**: Templates can include proper ARIA labels and semantic markup
5. **Interactive Elements**: Links can be rendered as buttons, styled anchors, or custom components
6. **Consistent API**: Same simple usage pattern for pages - just include `<BreadcrumbNav DisplayText="Page Name" />`
7. **Backward Compatibility**: Default templates provide the same functionality as the basic solution

### Template Context and Parameters

The `LinkTemplate` receives a context object containing:
- `item`: The `BreadCrumb` record with `Title` and `Url` properties
- `navigateTo`: An `Action<string>` method to handle navigation to the breadcrumb URL

#### Understanding the Tuple Pattern with Delegates

The use of a tuple `(BreadCrumb item, Action<string> navigateTo)` in the template parameter demonstrates an important Blazor templating pattern:

**Why Use a Tuple?**
- Templates often need both **data** (what to display) and **behavior** (what to do when interacted with)
- A tuple provides a clean way to pass multiple related values as a single parameter
- This keeps the template parameter list simple while providing rich functionality

**The Delegate Pattern:**
```csharp
Action<string> navigateTo
```
- `Action<string>` is a delegate type that represents a method taking a string parameter and returning void
- The component creates this delegate and passes it to the template
- The template can invoke this delegate to trigger navigation without knowing the implementation details

**How It Works in Practice:**

1. **Component Side** (in `Breadcrumbs.razor`):
```csharp
// The component creates the delegate
private void NavigateTo(string url)
{
    // Component's navigation logic here
    while (State.BreadCrumbs.Count > 0 && State.BreadCrumbs.Peek().Url != url)
    {
        State.BreadCrumbs.Pop();
    }
    NavigationManager.NavigateTo(url);
}

// The tuple is passed to the template
@LinkTemplate((item, NavigateTo))
```

2. **Template Side** (in the consuming component):
```razor
<LinkTemplate Context="breadcrumb">
    <button @onclick="() => breadcrumb.navigateTo(breadcrumb.item.Url)">
        @breadcrumb.item.Title
    </button>
</LinkTemplate>
```

**Key Benefits of This Pattern:**
- **Separation of Concerns**: Template handles presentation, component handles business logic
- **Reusability**: The same navigation logic works with any template design
- **Type Safety**: Compile-time checking ensures the delegate signature matches
- **Flexibility**: Templates can choose when and how to invoke the delegate
- **Encapsulation**: Template doesn't need to know about `NavigationManager` or breadcrumb state

**Alternative Approaches and Why Tuple is Better:**
- **Multiple Parameters**: `RenderFragment<BreadCrumb, Action<string>>` - not supported in Blazor
- **Custom Class**: Could create a `BreadcrumbContext` class, but tuple is more concise for simple cases
- **Event Callbacks**: Could use `EventCallback<string>`, but `Action<string>` is simpler for this use case

This design pattern allows the template to access both the breadcrumb data and the navigation functionality while maintaining clean separation of concerns and providing maximum flexibility for template customization.

The templated controls solution demonstrates how Blazor's template features can transform a basic component into a highly customizable and reusable UI element that can adapt to different design requirements while maintaining the same simple developer experience.

