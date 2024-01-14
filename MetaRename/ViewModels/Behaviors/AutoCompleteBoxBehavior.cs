﻿using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace MetaRename.ViewModels.Behaviors;

public class AutoCompleteBoxBehavior : Behavior<AutoCompleteBox>
{
    static AutoCompleteBoxBehavior() { }

    protected override void OnAttached() {
        if (AssociatedObject is not null) {
            AssociatedObject.KeyUp += OnKeyUp;
            AssociatedObject.DropDownOpening += DropDownOpening;
            AssociatedObject.GotFocus += OnGotFocus;
            AssociatedObject.PointerReleased += PointerReleased;
        }

        base.OnAttached();
    }

    protected override void OnDetaching() {
        if (AssociatedObject is not null) {
            AssociatedObject.KeyUp -= OnKeyUp;
            AssociatedObject.DropDownOpening -= DropDownOpening;
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.PointerReleased -= PointerReleased;
        }

        base.OnDetaching();
    }

    private void OnKeyUp(object? sender, Avalonia.Input.KeyEventArgs e) {
        if ((e.Key == Avalonia.Input.Key.Down || e.Key == Avalonia.Input.Key.F4)) {
            if (string.IsNullOrEmpty(AssociatedObject?.Text)) {
                ShowDropdown();
            }
        }
    }

    private void DropDownOpening(object? sender, System.ComponentModel.CancelEventArgs e) {
        var prop = AssociatedObject?.GetType().GetProperty("TextBox",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        var tb = (TextBox?) prop?.GetValue(AssociatedObject);
        if (tb is not null && tb.IsReadOnly) {
            e.Cancel = true;
        }
    }

    private void PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e) {
        if (string.IsNullOrEmpty(AssociatedObject?.Text)) {
            ShowDropdown();
        }
    }

    private void ShowDropdown() {
        if (AssociatedObject is not null && !AssociatedObject.IsDropDownOpen) {
            typeof(AutoCompleteBox)
                .GetMethod("PopulateDropDown",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(AssociatedObject, new object[] {AssociatedObject, EventArgs.Empty});
            typeof(AutoCompleteBox)
                .GetMethod("OpeningDropDown",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.Invoke(AssociatedObject, new object[] {false});

            if (!AssociatedObject.IsDropDownOpen) {
                var ipc = typeof(AutoCompleteBox).GetField("_ignorePropertyChange",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if ((bool) ipc?.GetValue(AssociatedObject)! == false)
                    ipc?.SetValue(AssociatedObject, true);

                AssociatedObject.SetCurrentValue<bool>(AutoCompleteBox.IsDropDownOpenProperty, true);
            }
        }
    }

    private void OnGotFocus(object? sender, RoutedEventArgs e) {
        
    }
}
