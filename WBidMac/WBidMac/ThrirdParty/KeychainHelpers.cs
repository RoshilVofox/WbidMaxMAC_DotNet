using System;
using Foundation;
using Security;
using System.IO;
using AppKit;

namespace iOSPasswordStorage
{
	/// <summary>
	/// Keychain helpers. These work on iOS7 onwards only. For compatibility with previoous iOS versions, the "Synchronizable" property must be removed / ignored.
	/// </summary>
	public static class KeychainHelpers
	{
		/// <summary>
		/// Deletes a username/password record.
		/// </summary>
		/// <param name="username">the username to query. Not case sensitive. May not be NULL.</param>
		/// <param name="serviceId">the service description to query. Not case sensitive.  May not be NULL.</param>
		/// <param name="synchronizable">
		/// Defines if the record you want to delete is syncable via iCloud keychain or not. Note that using the same username and service ID
		/// but different synchronization settings will result in two keychain entries.
		/// </param>
		/// <returns>Status code</returns>
		public static SecStatusCode DeletePasswordForUsername ( string username, string serviceId, bool synchronizable)
		{
			if ( username == null )
			{
				throw new ArgumentNullException ( "userName" );
			}
			
			if ( serviceId == null )
			{
				throw new ArgumentNullException ( "serviceId" );
			}
			
			// Querying is case sesitive - we don't want that.
			username = username.ToLower (  );
			serviceId = serviceId.ToLower (  );
			
			// Query and remove.
			SecRecord queryRec = new SecRecord ( SecKind.GenericPassword ) { Service = serviceId, Label = serviceId, Account = username, Synchronizable = synchronizable };
			SecStatusCode code = SecKeyChain.Remove ( queryRec );
			return code;
		}

		/// <summary>
		/// Sets a password for a specific username.
		/// </summary>
		/// <param name="username">the username to add the password for. Not case sensitive.  May not be NULL.</param>
		/// <param name="password">the password to associate with the record. May not be NULL.</param>
		/// <param name="serviceId">the service description to use. Not case sensitive.  May not be NULL.</param>
		/// <param name="secAccessible">defines how the keychain record is protected</param>
		/// <param name="synchronizable">
		/// Defines if keychain record can by synced via iCloud keychain.
		/// Note that using the same username and service ID but different synchronization settings will result in two keychain entries.
		/// </param>
		/// <returns>SecStatusCode.Success if everything went fine, otherwise some other status</returns>
		public static SecStatusCode SetPasswordForUsername ( string username, string password, string serviceId, SecAccessible secAccessible, bool synchronizable )
		{
			if ( username == null ) {
				throw new ArgumentNullException ( "userName" );
			}
			
			if ( serviceId == null ) {
				throw new ArgumentNullException ( "serviceId" );
			}
			
			if ( password == null ) {
				throw new ArgumentNullException ( "password" );
			}
			
			// Querying is case sesitive - we don't want that.
			username = username.ToLower (  );
			serviceId = serviceId.ToLower (  );
			
			// Don't bother updating. Delete existing record and create a new one.
			DeletePasswordForUsername ( username, serviceId, synchronizable );
			
			// Create a new record.
			// Store password UTF8 encoded.
			SecStatusCode code = SecKeyChain.Add ( new SecRecord ( SecKind.GenericPassword ) {
				Service = serviceId,
				Label = serviceId,
				Account = username,
				Generic = NSData.FromString ( password, NSStringEncoding.UTF8 ),
				Accessible = secAccessible,
				Synchronizable = synchronizable
			} );
			
			return code;
		}

		/// <summary>
		/// Gets a password for a specific username.
		/// </summary>
		/// <param name="username">the username to query. Not case sensitive.  May not be NULL.</param>
		/// <param name="serviceId">the service description to use. Not case sensitive.  May not be NULL.</param>
		/// <param name="synchronizable">
		/// Defines if the record you want to get is syncable via iCloud keychain or not. Note that using the same username and service ID
		/// but different synchronization settings will result in two keychain entries.
		/// </param>
		/// <returns>
		/// The password or NULL if no matching record was found.
		/// </returns>
		public static string GetPasswordForUsername ( string username, string serviceId, bool synchronizable )
		{
			if ( username == null )
			{
				throw new ArgumentNullException ( "userName" );
			}
			
			if ( serviceId == null )
			{
				throw new ArgumentNullException ( "serviceId" );
			}
			
			// Querying is case sesitive - we don't want that.
			username = username.ToLower (  );
			serviceId = serviceId.ToLower (  );
			
			SecStatusCode code;
			// Query the record.
			SecRecord queryRec = new SecRecord ( SecKind.GenericPassword ) { Service = serviceId, Label = serviceId, Account = username, Synchronizable = synchronizable };
			queryRec = SecKeyChain.QueryAsRecord ( queryRec, out code );
				
			// If found, try to get password.
			if ( code == SecStatusCode.Success && queryRec != null && queryRec.Generic != null )
			{
				// Decode from UTF8.
				return NSString.FromData ( queryRec.Generic, NSStringEncoding.UTF8 );
			}
			
			// Something went wrong.
			return null;
		}
        /// <summary>
        /// Stores a bearer token in the Keychain.
        /// </summary>
        /// <param name="token">The bearer token to store. May not be NULL.</param>
        /// <param name="serviceId">The service identifier. May not be NULL.</param>
        /// <param name="secAccessible">Defines how the keychain record is protected.</param>
        /// <param name="synchronizable">Defines if the keychain record can be synced via iCloud keychain.</param>
        /// <returns>SecStatusCode.Success if everything went fine, otherwise an error status.</returns>
        public static SecStatusCode SetBearerToken(string token, string serviceId, SecAccessible secAccessible, bool synchronizable)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));

            if (string.IsNullOrEmpty(serviceId))
                throw new ArgumentNullException(nameof(serviceId));

            // Delete existing record before setting a new one
            DeleteBearerToken(serviceId, synchronizable);

            var record = new SecRecord(SecKind.GenericPassword)
            {
                Service = serviceId,
                Label = "OAuthBearerToken",
                Account = "OAuthBearerToken",
                Generic = NSData.FromString(token, NSStringEncoding.UTF8),
                Accessible = secAccessible,
                Synchronizable = synchronizable
            };

            return SecKeyChain.Add(record);
        }

        /// <summary>
        /// Retrieves the stored bearer token from the Keychain.
        /// </summary>
        /// <param name="serviceId">The service identifier. May not be NULL.</param>
        /// <param name="synchronizable">Defines if the record should be retrieved from the iCloud keychain.</param>
        /// <returns>The stored bearer token, or NULL if not found.</returns>
        public static string GetBearerToken(string serviceId, bool synchronizable)
        {
            if (string.IsNullOrEmpty(serviceId))
                throw new ArgumentNullException(nameof(serviceId));

            SecStatusCode code;
            var queryRec = new SecRecord(SecKind.GenericPassword)
            {
                Service = serviceId,
                Label = "OAuthBearerToken",
                Account = "OAuthBearerToken",
                Synchronizable = synchronizable
            };

            var result = SecKeyChain.QueryAsRecord(queryRec, out code);

            return (code == SecStatusCode.Success && result?.Generic != null)
                ? NSString.FromData(result.Generic, NSStringEncoding.UTF8)
                : null;
        }

        /// <summary>
        /// Deletes the stored bearer token from the Keychain.
        /// </summary>
        /// <param name="serviceId">The service identifier. May not be NULL.</param>
        /// <param name="synchronizable">Defines if the record should be removed from the iCloud keychain.</param>
        /// <returns>Status code indicating success or failure.</returns>
        public static SecStatusCode DeleteBearerToken(string serviceId, bool synchronizable)
        {
            if (string.IsNullOrEmpty(serviceId))
                throw new ArgumentNullException(nameof(serviceId));

            var queryRec = new SecRecord(SecKind.GenericPassword)
            {
                Service = serviceId,
                Label = "OAuthBearerToken",
                Account = "OAuthBearerToken",
                Synchronizable = synchronizable
            };

            return SecKeyChain.Remove(queryRec);
        }
    }
}

