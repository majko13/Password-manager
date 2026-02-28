using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Password_manager
{
    public static class SecurePasswordManager
    {
        private static SecureString _masterPassword;
        private static byte[] _userSalt;
        private static int _userId;

        public static SecureString MasterPassword
        {
            get { return _masterPassword?.Copy(); }
            private set { _masterPassword = value; }
        }

        public static byte[] UserSalt
        {
            get { return _userSalt?.Clone() as byte[]; }
            private set { _userSalt = value; }
        }

        public static int UserId
        {
            get { return _userId; }
            private set { _userId = value; }
        }

        public static void SetCredentials(string password, byte[] salt, int userId)
        {
            ClearCredentials();

            if (!string.IsNullOrEmpty(password))
            {
                MasterPassword = ConvertToSecureString(password);
            }

            UserSalt = salt;
            UserId = userId;
        }

        public static void ClearCredentials()
        {
            if (_masterPassword != null)
            {
                _masterPassword.Dispose();
                _masterPassword = null;
            }

            if (_userSalt != null)
            {
                Array.Clear(_userSalt, 0, _userSalt.Length);
                _userSalt = null;
            }

            _userId = 0;
        }

        public static string GetMasterPasswordAsString()
        {
            if (_masterPassword == null)
                return null;

            return ConvertFromSecureString(_masterPassword);
        }

        public static (SecureString masterPassword, byte[] userSalt, int userId) GetCredentials()
        {
            return (MasterPassword, UserSalt, UserId);
        }

        public static SecureString ConvertToSecureString(string password)
        {
            var securePassword = new SecureString();

            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }

            securePassword.MakeReadOnly();
            return securePassword;
        }

        public static string ConvertFromSecureString(SecureString securePassword)
        {
            if (securePassword == null)
                return null;

            IntPtr ptr = IntPtr.Zero;

            try
            {
                ptr = Marshal.SecureStringToBSTR(securePassword);
                return Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(ptr);
                }
            }
        }

        public static void ClearString(ref string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = new string('*', str.Length);
            }
        }
    }
}