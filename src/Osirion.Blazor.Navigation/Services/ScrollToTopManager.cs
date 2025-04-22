using Osirion.Blazor.Navigation.Options;

namespace Osirion.Blazor.Navigation.Services;

/// <summary>
/// Service for managing global scroll to top behavior
/// </summary>
public class ScrollToTopManager
{
    private bool _isEnabled;
    private Position _position = Position.BottomRight;
    private ScrollBehavior _behavior = ScrollBehavior.Smooth;
    private int _visibilityThreshold = 300;
    private string? _text;
    private string _title = "Scroll to top";
    private string? _cssClass;
    private string? _customIcon;

    /// <summary>
    /// Event raised when configuration changes
    /// </summary>
    public event EventHandler? ConfigurationChanged;

    /// <summary>
    /// Gets or sets whether the global scroll to top is enabled
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled != value)
            {
                _isEnabled = value;
                OnConfigurationChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the position of the button
    /// </summary>
    public Position Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                OnConfigurationChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the scroll behavior
    /// </summary>
    public ScrollBehavior Behavior
    {
        get => _behavior;
        set
        {
            if (_behavior != value)
            {
                _behavior = value;
                OnConfigurationChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the visibility threshold
    /// </summary>
    public int VisibilityThreshold
    {
        get => _visibilityThreshold;
        set
        {
            if (_visibilityThreshold != value)
            {
                _visibilityThreshold = value;
                OnConfigurationChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the button text
    /// </summary>
    public string? Text
    {
        get => _text;
        set
        {
            if (_text != value)
            {
                _text = value;
                OnConfigurationChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the button title
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                OnConfigurationChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the CSS class
    /// </summary>
    public string? CssClass
    {
        get => _cssClass;
        set
        {
            if (_cssClass != value)
            {
                _cssClass = value;
                OnConfigurationChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the custom icon
    /// </summary>
    public string? CustomIcon
    {
        get => _customIcon;
        set
        {
            if (_customIcon != value)
            {
                _customIcon = value;
                OnConfigurationChanged();
            }
        }
    }

    /// <summary>
    /// Applies configuration from options
    /// </summary>
    /// <param name="options">The options to apply</param>
    public void ApplyOptions(ScrollToTopOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        Position = options.Position;
        Behavior = options.Behavior;
        VisibilityThreshold = options.VisibilityThreshold;
        Text = options.Text;
        Title = options.Title;
        CssClass = options.CssClass;
        CustomIcon = options.CustomIcon;
    }

    /// <summary>
    /// Raises the ConfigurationChanged event
    /// </summary>
    protected virtual void OnConfigurationChanged()
    {
        ConfigurationChanged?.Invoke(this, EventArgs.Empty);
    }
}