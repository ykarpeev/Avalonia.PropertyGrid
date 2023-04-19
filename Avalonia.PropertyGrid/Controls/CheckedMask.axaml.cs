﻿using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.PropertyGrid.Model.ComponentModel;
using System;

namespace Avalonia.PropertyGrid.Controls
{
    public partial class CheckedMask : UserControl
    {
        CheckedMaskModel _Model;
        public static readonly DirectProperty<CheckedMask, CheckedMaskModel> ModelProperty = 
            AvaloniaProperty.RegisterDirect<CheckedMask, CheckedMaskModel>(
                nameof(Model), 
                o => o._Model, 
                (o, v) => o.SetAndRaise(ModelProperty, ref o._Model, v)
                );

        public CheckedMaskModel Model
        {
            get => _Model;
            set => SetAndRaise(ModelProperty, ref _Model, value);
        }

        public static readonly StyledProperty<int> ButtonMinWidthProperty =
            AvaloniaProperty.Register<CheckedMask, int>(nameof(ButtonMinWidth), 80);

        public int ButtonMinWidth
        {
            get => GetValue(ButtonMinWidthProperty);
            set => SetValue(ButtonMinWidthProperty, value);
        }


        static CheckedMask()
        {
            ModelProperty.Changed.Subscribe(OnModelChanged);
        }

        public CheckedMask()
        {
            InitializeComponent();
        }

        private static void OnModelChanged(AvaloniaPropertyChangedEventArgs<CheckedMaskModel> e)
        {
            if (e.Sender is CheckedMask cm)
            {
                cm.OnModelChanged(e.NewValue.Value);
            }
        }

        private void OnModelChanged(CheckedMaskModel value)
        {
            mainPanel.Children.Clear();

            if(value == null)
            {
                return;
            }

            ToggleButton allButton = new ToggleButton();
            allButton.Content = value.All;
            allButton.IsChecked = value.IsAllChecked;
            allButton.Margin = new Thickness(6);
            allButton.MinWidth = ButtonMinWidth;
            allButton.HorizontalContentAlignment = Layout.HorizontalAlignment.Center;

            allButton.Checked += (s, e) => 
            { 
                value.Check(value.All); 

                foreach(ToggleButton btn in mainPanel.Children)
                {
                    if(btn != allButton)
                    {
                        btn.IsChecked = false;
                    }
                }
            };

            allButton.Unchecked += (s, e) => { value.UnCheck(value.All); };
            
            mainPanel.Children.Add(allButton);

            foreach(var mask in value.Masks)
            {
                ToggleButton button = new ToggleButton();
                button.Content = mask.ToString();
                button.IsChecked = value.IsChecked(mask) && !value.IsChecked(value.All);
                button.Margin = new Thickness(6);
                button.MinWidth = ButtonMinWidth;
                button.HorizontalContentAlignment = Layout.HorizontalAlignment.Center;

                button.Checked += (s, e) => {
                    value.UnCheck(value.All);
                    value.Check(mask);

                    allButton.IsChecked = false;
                };
                button.Unchecked += (s, e) => { value.UnCheck(mask); };

                mainPanel.Children.Add(button);
            }
        }
    }
}