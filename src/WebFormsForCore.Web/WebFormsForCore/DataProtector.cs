#region Assembly System.Security, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Security.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace System.Security.Cryptography;

//
// Summary:
//     Provides the base class for data protectors.
public class DataProtector
{
	private string m_applicationName;

	private string m_primaryPurpose;

	private IEnumerable<string> m_specificPurposes;

	private volatile byte[] m_hashedPurpose;

	//
	// Summary:
	//     Gets the name of the application.
	//
	// Returns:
	//     The name of the application.
	protected string ApplicationName => m_applicationName;

	//
	// Summary:
	//     Specifies whether the hash is prepended to the text array before encryption.
	//
	//
	// Returns:
	//     Always true.
	protected virtual bool PrependHashedPurposeToPlaintext => true;

	//
	// Summary:
	//     Gets the primary purpose for the protected data.
	//
	// Returns:
	//     The primary purpose for the protected data.
	protected string PrimaryPurpose => m_primaryPurpose;

	//
	// Summary:
	//     Gets the specific purposes for the protected data.
	//
	// Returns:
	//     A collection of the specific purposes for the protected data.
	protected IEnumerable<string> SpecificPurposes => m_specificPurposes;

	//
	// Summary:
	//     Creates a new instance of the System.Security.Cryptography.DataProtector class
	//     by using the provided application name, primary purpose, and specific purposes.
	//
	//
	// Parameters:
	//   applicationName:
	//     The name of the application.
	//
	//   primaryPurpose:
	//     The primary purpose for the protected data.
	//
	//   specificPurposes:
	//     The specific purposes for the protected data.
	//
	// Exceptions:
	//   T:System.ArgumentException:
	//     applicationName is an empty string or null. -or- primaryPurpose is an empty string
	//     or null. -or- specificPurposes contains an empty string or null.
	protected DataProtector(string applicationName, string primaryPurpose, string[] specificPurposes)
	{
		if (string.IsNullOrWhiteSpace(applicationName))
		{
			throw new ArgumentException(SR.GetString(SR.Cryptography_DataProtector_InvalidAppNameOrPurpose));
		}

		if (string.IsNullOrWhiteSpace(primaryPurpose))
		{
			throw new ArgumentException(SR.GetString(SR.Cryptography_DataProtector_InvalidAppNameOrPurpose));
		}

		if (specificPurposes != null)
		{
			foreach (string value in specificPurposes)
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ArgumentException(SR.GetString(SR.Cryptography_DataProtector_InvalidAppNameOrPurpose));
				}
			}
		}

		m_applicationName = applicationName;
		m_primaryPurpose = primaryPurpose;
		List<string> list = new List<string>();
		if (specificPurposes != null)
		{
			list.AddRange(specificPurposes);
		}

		m_specificPurposes = list;
	}

	//
	// Summary:
	//     Creates a hash of the property values specified by the constructor.
	//
	// Returns:
	//     An array of bytes that contain the hash of the System.Security.Cryptography.DataProtector.ApplicationName,
	//     System.Security.Cryptography.DataProtector.PrimaryPurpose, and System.Security.Cryptography.DataProtector.SpecificPurposes
	//     properties.
	protected virtual byte[] GetHashedPurpose()
	{
		if (m_hashedPurpose == null)
		{
			using HashAlgorithm hashAlgorithm = HashAlgorithm.Create("System.Security.Cryptography.Sha256Cng");
			using (BinaryWriter binaryWriter = new BinaryWriter(new CryptoStream(new MemoryStream(), hashAlgorithm, CryptoStreamMode.Write), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)))
			{
				binaryWriter.Write(ApplicationName);
				binaryWriter.Write(PrimaryPurpose);
				foreach (string specificPurpose in SpecificPurposes)
				{
					binaryWriter.Write(specificPurpose);
				}
			}

			m_hashedPurpose = hashAlgorithm.Hash;
		}

		return m_hashedPurpose;
	}

	//
	// Summary:
	//     Determines if re-encryption is required for the specified encrypted data.
	//
	// Parameters:
	//   encryptedData:
	//     The encrypted data to be evaluated.
	//
	// Returns:
	//     true if the data must be re-encrypted; otherwise, false.
	public bool IsReprotectRequired(byte[] encryptedData) => false;

	//
	// Summary:
	//     Creates an instance of a data protector implementation by using the specified
	//     class name of the data protector, the application name, the primary purpose,
	//     and the specific purposes.
	//
	// Parameters:
	//   providerClass:
	//     The class name for the data protector.
	//
	//   applicationName:
	//     The name of the application.
	//
	//   primaryPurpose:
	//     The primary purpose for the protected data.
	//
	//   specificPurposes:
	//     The specific purposes for the protected data.
	//
	// Returns:
	//     A data protector implementation object.
	//
	// Exceptions:
	//   T:System.ArgumentNullException:
	//     providerClass is null.
	public static DataProtector Create(string providerClass, string applicationName, string primaryPurpose, params string[] specificPurposes)
		=> new(applicationName, primaryPurpose, specificPurposes);

	//
	// Summary:
	//     Protects the specified user data.
	//
	// Parameters:
	//   userData:
	//     The data to be protected.
	//
	// Returns:
	//     A byte array that contains the encrypted data.
	//
	// Exceptions:
	//   T:System.ArgumentNullException:
	//     userData is null.
	public byte[] Protect(byte[] userData)
	{
		if (userData == null)
		{
			throw new ArgumentNullException("userData");
		}

		if (PrependHashedPurposeToPlaintext)
		{
			byte[] hashedPurpose = GetHashedPurpose();
			byte[] array = new byte[userData.Length + hashedPurpose.Length];
			Array.Copy(hashedPurpose, 0, array, 0, hashedPurpose.Length);
			Array.Copy(userData, 0, array, hashedPurpose.Length, userData.Length);
			userData = array;
		}

		return ProviderProtect(userData);
	}

	//
	// Summary:
	//     Specifies the delegate method in the derived class that the System.Security.Cryptography.DataProtector.Protect(System.Byte[])
	//     method in the base class calls back into.
	//
	// Parameters:
	//   userData:
	//     The data to be encrypted.
	//
	// Returns:
	//     A byte array that contains the encrypted data.
	protected byte[] ProviderProtect(byte[] userData)
	{
		return ProtectedDataPortable.Protect(userData, null, DataProtectionScope.CurrentUser);
	}

	//
	// Summary:
	//     Specifies the delegate method in the derived class that the System.Security.Cryptography.DataProtector.Unprotect(System.Byte[])
	//     method in the base class calls back into.
	//
	// Parameters:
	//   encryptedData:
	//     The data to be unencrypted.
	//
	// Returns:
	//     The unencrypted data.
	protected byte[] ProviderUnprotect(byte[] encryptedData)
	{
		return ProtectedDataPortable.Unprotect(encryptedData, null, DataProtectionScope.CurrentUser);
	}
	bool CryptographicEquals(byte[] a, byte[] b, int count)
	{
		int num = 0;
		if (a.Length < count || b.Length < count)
		{
			return false;
		}

		for (int i = 0; i < count; i++)
		{
			num |= a[i] - b[i];
		}

		return num == 0;
	}

	//
	// Summary:
	//     Unprotects the specified protected data.
	//
	// Parameters:
	//   encryptedData:
	//     The encrypted data to be unprotected.
	//
	// Returns:
	//     A byte array that contains the plain-text data.
	//
	// Exceptions:
	//   T:System.ArgumentNullException:
	//     encryptedData is null.
	//
	//   T:System.Security.Cryptography.CryptographicException:
	//     encryptedData contained an invalid purpose.
	public byte[] Unprotect(byte[] encryptedData)
	{
		if (encryptedData == null)
		{
			throw new ArgumentNullException("encryptedData");
		}

		if (PrependHashedPurposeToPlaintext)
		{
			byte[] array = ProviderUnprotect(encryptedData);
			byte[] hashedPurpose = GetHashedPurpose();
			if (!CryptographicEquals(hashedPurpose, array, hashedPurpose.Length))
			{
				throw new CryptographicException(SR.GetString(SR.Cryptography_DataProtector_InvalidPurpose));
			}

			byte[] array2 = new byte[array.Length - hashedPurpose.Length];
			Array.Copy(array, hashedPurpose.Length, array2, 0, array2.Length);
			return array2;
		}

		return ProviderUnprotect(encryptedData);
	}
}
