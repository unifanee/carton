using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using carton.ViewModels;
using System;
using System.Linq;

namespace carton.Views.Pages;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        SettingsTabStrip.Tapped += OnSettingsTabStripTapped;
        SettingsScrollViewer.ScrollChanged += OnSettingsScrollViewerScrollChanged;
    }

    private void OnSettingsTabStripTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (SettingsTabStrip.SelectedItem is TabStripItem item && item.Tag is string controlName)
        {
            var target = this.FindControl<Control>(controlName);
            if (target is null || SettingsScrollViewer.Content is not Control scrollContent)
                return;

            var offset = target.TranslatePoint(new Point(0, 0), scrollContent);
            if (offset.HasValue)
            {
                SettingsScrollViewer.Offset = new Vector(
                    SettingsScrollViewer.Offset.X,
                    Math.Max(0, offset.Value.Y - 8));
            }
        }
    }

    private void OnSettingsScrollViewerScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (sender is not ScrollViewer scrollViewer)
            return;

        var viewportBounds = new Rect(
            (Point)scrollViewer.Offset,
            new Size(scrollViewer.Viewport.Width, scrollViewer.Viewport.Height));

        if (scrollViewer.Content is not StackPanel panel)
            return;

        foreach (var child in panel.Children)
        {
            if (viewportBounds.Intersects(child.Bounds))
            {
                var matchingItem = SettingsTabStrip.Items
                    .OfType<TabStripItem>()
                    .FirstOrDefault(x => x.Tag is string tag && tag == child.Name);

                if (matchingItem != null && SettingsTabStrip.SelectedItem != matchingItem)
                {
                    SettingsTabStrip.SelectedItem = matchingItem;
                }

                break;
            }
        }
    }

    private void OnUseSystemThemeAccentClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel viewModel)
        {
            viewModel.SetThemeAccentMode(useSystemAccent: true);
        }
    }

    private void OnUseCustomThemeAccentClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel viewModel)
        {
            viewModel.SetThemeAccentMode(useSystemAccent: false);
        }
    }
}
