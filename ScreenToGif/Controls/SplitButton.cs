﻿using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ScreenToGif.Util;

namespace ScreenToGif.Controls
{
    /// <inheritdoc />
    /// <summary>
    /// A Menu/ComboBox with a default button.
    /// </summary>
    public class SplitButton : ImageMenuItem
    {
        #region Variables

        private Grid _internalGrid;
        private Popup _mainPopup;
        private StackPanel _innerStackPanel;

        private ImageMenuItem _current;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty SelectedIndexProperty = DependencyProperty.Register("SelectedIndex", typeof(int), typeof(SplitButton), new FrameworkPropertyMetadata(0,
            FrameworkPropertyMetadataOptions.AffectsRender, SelectedIndex_ChangedCallback));

        #endregion

        #region Properties

        /// <summary>
        /// The index of selected item.
        /// </summary>
        [Description("The index of selected item."), Category("Common")]
        public int SelectedIndex
        {
            get => (int)GetValue(SelectedIndexProperty);
            set => SetCurrentValue(SelectedIndexProperty, value);
        }

        /// <summary>
        /// The text of the button.
        /// </summary>
        [Description("The text of the button."), Category("Common")]
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetCurrentValue(TextProperty, value);
        }

        /// <summary>
        /// The TextWrapping property controls whether or not text wraps 
        /// when it reaches the flow edge of its containing block box. 
        /// </summary>
        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        #endregion

        static SplitButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitButton), new FrameworkPropertyMetadata(typeof(SplitButton)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _internalGrid = Template.FindName("InternalGrid", this) as Grid;
            _mainPopup = Template.FindName("Popup", this) as Popup;
            _innerStackPanel = Template.FindName("InnerStackPanel", this) as StackPanel;

            if (_internalGrid == null)
                return;

            _internalGrid.MouseDown += OnClick;

            PrepareMainAction(this);

            //Close on click.
            foreach (var item in _innerStackPanel.Children.OfType<ImageMenuItem>().ToList())
                item.Click += (sender, args) => _mainPopup.IsOpen = false;
        }

        private static void SelectedIndex_ChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (!(o is SplitButton split))
                return;

            split.PrepareMainAction(split);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            Command?.Execute(null);

            _current?.RaiseEvent(new RoutedEventArgs(ImageMenuItem.ClickEvent));
        }

        private void PrepareMainAction(SplitButton split)
        {
            if (split.SelectedIndex < 0)
                return;

            var list = split._innerStackPanel.Children.OfType<ImageMenuItem>().ToList();

            if (split.SelectedIndex > list.Count - 1)
            {
                split.SelectedIndex = list.Count - 1;
                return;
            }

            split.Image = list[split.SelectedIndex].Image.XamlClone();
            split.Text = list[split.SelectedIndex].Header as string;
            split.Command = list[split.SelectedIndex].Command;

            _current = list[split.SelectedIndex];
        }
    }
}