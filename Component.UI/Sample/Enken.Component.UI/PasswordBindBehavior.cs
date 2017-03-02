using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Enken.Component.UI
{
    /// <summary>
    /// パスワードをバインディングするためのビヘイビア
    /// </summary>
    class PasswordBindBehavior : Behavior<PasswordBox>
    {
        static bool isInitialized = false;

        /// <summary>
        /// パスワード
        /// </summary>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(PasswordBindBehavior), new PropertyMetadata(null));

        /// <summary>
        /// パスワード変更時のプロパティ反映
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void passwordChanged(object sender, RoutedEventArgs e)
        {
            Password = (sender as PasswordBox).Password;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (!isInitialized)
            {
                if (this.AssociatedObject is PasswordBox)
                {
                    this.AssociatedObject.Password = this.Password;
                }
                isInitialized = true;
            }
            this.AssociatedObject.PasswordChanged += passwordChanged;
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PasswordChanged -= passwordChanged;
        }
    }
}
