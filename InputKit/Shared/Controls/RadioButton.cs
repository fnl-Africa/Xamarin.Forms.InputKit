﻿using Plugin.InputKit.Shared.Abstraction;
using Plugin.InputKit.Shared.Configuration;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Plugin.InputKit.Shared.Controls
{
    ///-----------------------------------------------------------------------------
    /// <summary>
    /// Radio Button with Text
    /// </summary>
    public partial class RadioButton : StackLayout
    {
        #region Statics
        /// <summary>
        /// Default values of RadioButton
        /// </summary>
        public static GlobalSetting GlobalSetting { get; private set; } = new GlobalSetting
        {
            Color = Color.Accent,
            BorderColor = Color.Black,
            TextColor = (Color)Label.TextColorProperty.DefaultValue,
            Size = Device.GetNamedSize(Device.RuntimePlatform == Device.iOS ? NamedSize.Large : NamedSize.Medium, typeof(Label)) * 1.2,
            CornerRadius = -1,
            FontSize = Device.GetNamedSize(Device.RuntimePlatform == Device.iOS ? NamedSize.Medium : NamedSize.Small, typeof(Label)),
        };
        #endregion

        #region Constants
        public const string RESOURCE_CIRCLE = "Plugin.InputKit.Shared.Resources.circle.png";
        public const string RESOURCE_DOT = "Plugin.InputKit.Shared.Resources.dot.png";
        #endregion

        #region Fields
        internal IconView iconCircle = new IconView { Source = ImageSource.FromResource(RESOURCE_CIRCLE), FillColor = GlobalSetting.BorderColor, VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.Center, HeightRequest = GlobalSetting.Size, WidthRequest = GlobalSetting.Size };
        internal IconView iconChecked = new IconView { Source = ImageSource.FromResource(RESOURCE_DOT), FillColor = GlobalSetting.Color, IsVisible = false, VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.Center, HeightRequest = GlobalSetting.Size, WidthRequest = GlobalSetting.Size };
        internal Label lblText = new Label { Margin = new Thickness(0, 5, 0, 0), Text = "", VerticalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.CenterAndExpand, TextColor = GlobalSetting.TextColor, FontSize = GlobalSetting.FontSize, FontFamily = GlobalSetting.FontFamily };
        private bool _isDisabled;
        #endregion

        #region Ctor
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Default Constructor
        /// </summary>
        public RadioButton()
        {
            InitVisualStates();
            if (Device.RuntimePlatform != Device.iOS)
                lblText.FontSize = lblText.FontSize *= 1.5;
            Orientation = StackOrientation.Horizontal;
            this.Children.Add(new Grid
            {
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children =
                {
                    iconCircle,
                    iconChecked
                },
                MinimumWidthRequest = GlobalSetting.Size * 1.66,
            });
            this.Children.Add(lblText);
            this.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(Tapped) });
        }
        ///-----------------------------------------------------------------------------
        /// <summary>
        /// Quick generating constructor.
        /// </summary>
        /// <param name="value">Value to keep in radio button</param>
        /// <param name="displayMember">If you send an ojbect as value. Which property will be displayed. Or override .ToString() inside of your object.</param>
        /// <param name="isChecked"> Checked or not situation</param>
        public RadioButton(object value, string displayMember, bool isChecked = false) : this()
        {
            this.Value = value;
            this.IsChecked = isChecked;
            string text;
            if (!String.IsNullOrEmpty(displayMember))
                text = value.GetType().GetProperty(displayMember)?.GetValue(value).ToString();
            else
                text = value.ToString();
            lblText.Text = text ?? " ";
        }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Quick generating constructor.
        /// </summary>
        /// <param name="text">Text to display right of Radio button </param>
        /// <param name="isChecked">IsSelected situation</param>
        public RadioButton(string text, bool isChecked = false) : this()
        {
            Value = text;
            lblText.Text = text;
            this.IsChecked = isChecked;
        }
        #endregion

        //-----------------------------------------------------------------------------
        /// <summary>
        /// Click event, triggered when clicked
        /// </summary>
        public event EventHandler Clicked;
        #region Properties
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Animator object to animate when touch.
        /// </summary>
        public IAnimator<RadioButton> Animator { get; set; } = new DefaultAnimator<RadioButton>();
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Click command, executed when clicked.  Parameter will be Value property if CommandParameter is not set
        /// </summary>
        public ICommand ClickCommand { get; set; }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// A command parameter will be sent to commands.
        /// </summary>
        public object CommandParameter { get; set; }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Value to keep inside of Radio Button
        /// </summary>
        public object Value { get; set; }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Gets or Sets, is that Radio Button selected/choosed/Checked
        /// </summary>
        public bool IsChecked
        {
            get => iconChecked.IsVisible; set
            {
                iconChecked.IsVisible = value;
                SetValue(IsCheckedProperty, value);
                Animator?.Animate(this);
            }
        }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// this control if is Disabled
        /// </summary>
        public bool IsDisabled { get => _isDisabled; set { _isDisabled = value; this.Opacity = value ? 0.6 : 1; } }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Text Description of Radio Button. It will be displayed right of Radio Button
        /// </summary>
        public string Text { get => lblText.Text; set => lblText.Text = value; }
        /// <summary>
        /// Fontsize of Description Text
        /// </summary>
        public double TextFontSize { get => lblText.FontSize; set { lblText.FontSize = value; } }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Size of Radio Button
        /// </summary>
        [Obsolete("This feature is obsolete. You can try to use 'CircleImage' to set your own image as circle", false)]
        public double CircleSize { get => iconCircle.Height; set => SetCircleSize(value); }

        //-----------------------------------------------------------------------------
        /// <summary>
        /// Set your own background image instead of default circle.
        /// </summary>
        public ImageSource CircleImage { get => (ImageSource)GetValue(CircleImageProperty); set => SetValue(CircleImageProperty, value); }
        //-----------------------------------------------------------------------------
        public ImageSource CheckedImage { get => (ImageSource)GetValue(CheckedImageProperty); set => SetValue(CheckedImageProperty, value); }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// To be added.
        /// </summary>
        public string FontFamily { get => lblText.FontFamily; set => lblText.FontFamily = value; }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Color of Radio Button's checked.
        /// </summary>
        public Color Color { get => (Color)GetValue(ColorProperty); set => SetValue(ColorProperty, value); }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Color of radio button's outline border 
        /// </summary>
        public Color CircleColor { get => (Color)GetValue(CircleColorProperty); set => SetValue(CircleColorProperty, value); }
        //-----------------------------------------------------------------------------
        /// <summary>
        /// Color of description text of Radio Button
        /// </summary>
        public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }

        public bool IsPressed { get => (bool)GetValue(IsPressedProperty); set => SetValue(IsPressedProperty, value); }
        #endregion

        #region BindableProperties
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public static readonly BindableProperty IsCheckedProperty = BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(RadioButton), false, propertyChanged: (bo, ov, nv) => (bo as RadioButton).IsChecked = (bool)nv);
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(RadioButton), "", propertyChanged: (bo, ov, nv) => (bo as RadioButton).Text = (string)nv);
        public static readonly BindableProperty TextFontSizeProperty = BindableProperty.Create(nameof(TextFontSize), typeof(double), typeof(RadioButton), 20.0, propertyChanged: (bo, ov, nv) => (bo as RadioButton).TextFontSize = (double)nv);
        public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(Color), typeof(Color), typeof(RadioButton), GlobalSetting.Color, propertyChanged: (bo, ov, nv) => (bo as RadioButton).UpdateColors());
        public static readonly BindableProperty CircleImageProperty = BindableProperty.Create(nameof(CircleImage), typeof(ImageSource), typeof(RadioButton), default(ImageSource), propertyChanged: (bo, ov, nv) => (bo as RadioButton).iconCircle.Source = nv as ImageSource ?? nv?.ToString());
        public static readonly BindableProperty CheckedImageProperty = BindableProperty.Create(nameof(CheckedImage), typeof(ImageSource), typeof(RadioButton), default(ImageSource), propertyChanged: (bo, ov, nv) => (bo as RadioButton).iconChecked.Source = nv as ImageSource ?? nv?.ToString());
        public static readonly BindableProperty CircleColorProperty = BindableProperty.Create(nameof(CircleColor), typeof(Color), typeof(RadioButton), GlobalSetting.BorderColor, propertyChanged: (bo, ov, nv) => (bo as RadioButton).UpdateColors());
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(RadioButton), GlobalSetting.TextColor, propertyChanged: (bo, ov, nv) => (bo as RadioButton).UpdateColors());
        public static readonly BindableProperty ClickCommandProperty = BindableProperty.Create(nameof(ClickCommand), typeof(ICommand), typeof(RadioButton), null, propertyChanged: (bo, ov, nv) => (bo as RadioButton).ClickCommand = (ICommand)nv);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(RadioButton), propertyChanged: (bo, ov, nv) => (bo as RadioButton).CommandParameter = nv);
        public static readonly BindableProperty IsPressedProperty = BindableProperty.Create(nameof(IsPressed), typeof(bool), typeof(RadioButton), propertyChanged: (bo, ov, nv) => (bo as RadioButton).iconCircle.ScaleTo((bool)nv ? .5 : 1, 100));
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        #endregion

        #region Methods
        //-----------------------------------------------------------------------------
        /// <summary>
        /// That handles tapps and triggers event, commands etc.
        /// </summary>
        void Tapped()
        {
            if (IsDisabled) return;
            IsChecked = !IsChecked;
            Clicked?.Invoke(this, new EventArgs());
            ClickCommand?.Execute(CommandParameter ?? Value);

        }

        //-----------------------------------------------------------------------------
        /// <summary>
        /// Sets size of Circle
        /// </summary>
        [Obsolete("This feature is obsolete. You can try to use 'CircleImage' to set your own image as circle", false)]
        void SetCircleSize(double value)
        {
            iconCircle.HeightRequest = this.CircleSize;
            iconCircle.WidthRequest = this.CircleSize;
            iconChecked.WidthRequest = this.CircleSize;
            iconChecked.WidthRequest = this.CircleSize;
            Debug.WriteLine("[InputKit] [RadioButton] - CircleSize is obsolete and doesn't affect now. You can try to set a image source to CircleImage to change circle");
        }

        void UpdateColors()
        {
            iconChecked.FillColor = this.Color;
            iconCircle.FillColor = IsChecked ? this.Color : this.CircleColor;
            lblText.TextColor = this.TextColor;
        }

        void InitVisualStates()
        {
            VisualStateManager.SetVisualStateGroups(this, new VisualStateGroupList
            {
                new VisualStateGroup
                {
                    Name = "InputKitStates",
                    TargetType = typeof(RadioButton),
                    States =
                    {
                        new VisualState
                        {
                            Name = "Pressed",
                            TargetType = typeof(RadioButton),
                            Setters =
                            {
                                new Setter { Property = RadioButton.IsPressedProperty, Value = true }
                            }
                        },
                        new VisualState
                        {
                            Name = "Normal",
                            TargetType = typeof(RadioButton),
                            Setters =
                            {
                                new Setter { Property = RadioButton.IsPressedProperty, Value = false }
                            }
                        }
                    }
                }
            });
        }
        #endregion
    }
}
