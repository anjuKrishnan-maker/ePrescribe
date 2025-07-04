
namespace eRxWeb.ServerModel
{

    public class ChangePasswordModel
    {
        public string Status { get; set; }
        public string PasswordHelpText { get; set; }
        public bool Success { get; set; }

        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

    }
}