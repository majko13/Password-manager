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
            ClearCredentials(); // Nejprve vymaž stará data

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

        // Pomocné metody pro konverzi
        public static SecureString ConvertToSecureString(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                System.Diagnostics.Debug.WriteLine($"[SecurePasswordManager] ConvertToSecureString: password is null or empty");
                return null;
            }

            System.Diagnostics.Debug.WriteLine($"[SecurePasswordManager] ConvertToSecureString: converting '{password}' (length: {password.Length})");

            var securePassword = new SecureString();

            // DEBUG: vypiš všechny znaky
            for (int i = 0; i < password.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine($"[SecurePasswordManager] Char {i}: '{password[i]}' ({(int)password[i]})");
            }

            foreach (char c in password)
            {
                securePassword.AppendChar(c);
            }

            securePassword.MakeReadOnly();

            System.Diagnostics.Debug.WriteLine($"[SecurePasswordManager] ConvertToSecureString: created SecureString with {securePassword.Length} chars");

            return securePassword;
        }

        public static string ConvertFromSecureString(SecureString securePassword)
        {
            if (securePassword == null)
            {
                System.Diagnostics.Debug.WriteLine("[SecurePasswordManager] ConvertFromSecureString: securePassword is NULL");
                return null;
            }

            IntPtr ptr = IntPtr.Zero;
            try
            {
                System.Diagnostics.Debug.WriteLine($"[SecurePasswordManager] ConvertFromSecureString: starting conversion");

                ptr = Marshal.SecureStringToBSTR(securePassword);
                System.Diagnostics.Debug.WriteLine($"[SecurePasswordManager] ConvertFromSecureString: got BSTR pointer: {ptr}");

                // Zkus různé metody:

                // Metoda 1: PtrToStringBSTR
                string result1 = Marshal.PtrToStringBSTR(ptr);
                System.Diagnostics.Debug.WriteLine($"[SecurePasswordManager] Method 1 (PtrToStringBSTR): '{result1}' (length: {result1?.Length ?? 0})");

                // Metoda 2: Přečíst délku a data ručně
                int length = Marshal.ReadInt32(ptr, -4); // Délka v bajtech
                System.Diagnostics.Debug.WriteLine($"[SecurePasswordManager] BSTR length in bytes: {length}");

                if (length > 0)
                {
                    byte[] bytes = new byte[length];
                    Marshal.Copy(ptr, bytes, 0, length);
                    string result2 = System.Text.Encoding.Unicode.GetString(bytes);
                    System.Diagnostics.Debug.WriteLine($"[SecurePasswordManager] Method 2 (manual): '{result2}' (length: {result2?.Length ?? 0})");

                    // Zkus i UTF8
                    string result3 = System.Text.Encoding.UTF8.GetString(bytes);
                    System.Diagnostics.Debug.WriteLine($"[SecurePasswordManager] Method 3 (UTF8): '{result3}' (length: {result3?.Length ?? 0})");

                    // Vrať tu, která vypadá nejlépe
                    if (result2 != null && result2.Length > 2)
                        return result2;
                }

                return result1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SecurePasswordManager] ConvertFromSecureString ERROR: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[SecurePasswordManager] ConvertFromSecureString ERROR StackTrace: {ex.StackTrace}");
                return null;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.ZeroFreeBSTR(ptr);
                    System.Diagnostics.Debug.WriteLine($"[SecurePasswordManager] ConvertFromSecureString: freed BSTR pointer");
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