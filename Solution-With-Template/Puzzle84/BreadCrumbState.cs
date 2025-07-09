namespace Puzzle84;

/// <summary>
/// Represents the state of breadcrumbs, maintaining a stack of <see cref="BreadCrumb"/> instances
/// to track navigation history or path within an application.
/// </summary>
public class BreadCrumbState
{
	public Stack<BreadCrumb> BreadCrumbs { get; } = new();
}


/// <summary>
/// Represents a breadcrumb with a title and URL.
/// </summary>
/// <param name="Title"></param>
/// <param name="Url"></param>
public record BreadCrumb(string Title, string Url);