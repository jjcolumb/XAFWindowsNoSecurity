namespace DevExpress.UITemplates.Collection.Forms {
    using DevExpress.Mvvm;
    using DevExpress.Mvvm.POCO;
    using DevExpress.UITemplates.Collection.Utilities;
    using DevExpress.XtraEditors.Internal;

    public partial class LoginForm : HtmlFormBase {
        public LoginForm() {
            InitializeComponent();
            if(!mvvmContext.IsDesignMode)
                InitializeBinding();
        }
        [System.ComponentModel.Category("Data"), System.ComponentModel.Browsable(false)]
        public bool IsAuthorizationComplete {
            get { return LoginStatus == Status.AuthorizationComplete; }
        }
        [System.ComponentModel.Category("Data"), System.ComponentModel.Browsable(false)]
        public Status LoginStatus {
            get { return GetViewModel()?.Status ?? Status.Unknown; }
        }
        ViewModel GetViewModel() {
            return mvvmContext.OfType<ViewModel>().ViewModel;
        }
        void InitializeBinding() {
            var fluent = mvvmContext.OfType<ViewModel>();
            fluent.SetObjectDataSourceBinding(viewModelBindingSource, x => x.Validate);
            fluent.SetBinding(this, frm => frm.Text, x => x.Title);
            fluent.SetBinding(btnLogin, btn => btn.Text, x => x.Action);
            fluent.BindCommandToElement(layoutContainer, "recoveryLink", x => x.Recover);
            fluent.BindCommandToElement(layoutContainer, "registerLink", x => x.Register);
            fluent.BindCommand(btnLogin, x => x.LogIn);
        }
        public enum Status {
            Unknown,
            AuthorizationComplete,
            AuthorizationFailed,
            RecoveryRequested,
            RegistrationRequested
        }
        public class ViewModel {
            public string Title {
                get { return "Log In"; }
            }
            public string Action {
                get { return "Log In"; }
            }
            public string RecoveryLink {
                get { return "Forgot the password?"; }
            }
            public string RegisterLink {
                get { return "Create an account"; }
            }
            public virtual string Email { get; set; }
            public virtual string Password { get; set; }
            public virtual Status Status {
                get;
                protected set;
            }
            protected virtual void OnStatusChanged() {
                Validate();
            }
            bool isLoginDataValid;
            public void Validate() {
                isLoginDataValid = Utilities.Email.Validate(Email) && !string.IsNullOrEmpty(Password);
                this.RaiseCanExecuteChanged(x => x.Register());
                this.RaiseCanExecuteChanged(x => x.LogIn());
            }
            public void Recover() {
                Status = Status.RecoveryRequested;
                /* start recovery routine */
                CloseWindow();
            }
            public bool CanRegister() {
                return Status == Status.Unknown && isLoginDataValid;
            }
            public void Register() {
                Status = Status.RegistrationRequested;
                /* start registration routine with current Email/Password */
                CloseWindow();
            }
            public bool CanLogIn() {
                return Status == Status.Unknown && isLoginDataValid;
            }
            public void LogIn() {
                Status = QueryAuthorization() ? Status.AuthorizationComplete : Status.AuthorizationFailed;
            }
            bool QueryAuthorization() {
                /* do authorization request with current Email/Password */
                return true;
            }
            void CloseWindow() {
                var currentWindow = this.GetService<ICurrentWindowService>();
                if(currentWindow != null)
                    currentWindow.Close();
            }
        }
        #region Theme
        protected override object LoadDefaultHtmlImages() {
            return Base.FormAssets.DefaultHtmlImages;
        }
        protected override string LoadDefaultTemplate() {
            return Base.FormAssets.FixedDialog.Html;
        }
        protected override string LoadDefaultStyles() {
            return Base.FormAssets.FixedDialog.Css;
        }
        protected override string LoadDefaultLayoutTemplate() {
            return Assets.Layout.Html;
        }
        protected override string LoadDefaultLayoutStyles() {
            return Assets.Layout.Css;
        }
        public static class Assets {
            public static HtmlCssAsset Layout = new LoginFormLayoutHtmlCssAsset();
            //
            sealed class LoginFormLayoutHtmlCssAsset : HtmlCssAsset { }
        }
        #endregion
    }
}
