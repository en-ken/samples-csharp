using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Enken.Sample
{
    class MainWindowViewModel : DependencyObject
    {
        #region ClipboardText
        public string ClipboardText
        {
            get { return (string)GetValue(ClipboardTextProperty); }
            set { SetValue(ClipboardTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ClipboardText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClipboardTextProperty =
            DependencyProperty.Register("ClipboardText", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(propertyChanged));
        #endregion


        #region Password
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(MainWindowViewModel), new PropertyMetadata(propertyChanged));
        #endregion
        static void propertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Property.Name + " Changed : " + e.NewValue);
        }

        public MainWindowViewModel()
        {
            ClipboardText = string.Empty;
            Password = string.Empty;
        }
    }
}
