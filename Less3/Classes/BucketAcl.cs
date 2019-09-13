﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Less3.Classes
{
    /// <summary>
    /// Access control list entry for a bucket.
    /// </summary>
    internal class BucketAcl
    {
        #region Internal-Members

        internal int Id { get; set; }
        internal string UserGroup { get; set; }
        internal string UserGUID { get; set; }
        internal string IssuedByUserGUID { get; set; }
        internal bool PermitRead { get; set; }
        internal bool PermitWrite { get; set; }
        internal bool PermitReadAcp { get; set; }
        internal bool PermitWriteAcp { get; set; }
        internal bool FullControl { get; set; }

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories
         
        internal BucketAcl()
        {

        }

        internal static BucketAcl BucketGroupAcl(
            string groupName, 
            string issuedByUserGuid, 
            bool permitRead,
            bool permitWrite,
            bool permitReadAcp,
            bool permitWriteAcp,
            bool fullControl)
        {
            if (String.IsNullOrEmpty(groupName)) throw new ArgumentNullException(nameof(groupName));
            if (String.IsNullOrEmpty(issuedByUserGuid)) throw new ArgumentNullException(nameof(issuedByUserGuid));

            BucketAcl ret = new BucketAcl();

            ret.UserGroup = groupName;
            ret.UserGUID = null;
            ret.IssuedByUserGUID = issuedByUserGuid;

            ret.PermitRead = permitRead;
            ret.PermitWrite = permitWrite;
            ret.PermitReadAcp = permitReadAcp;
            ret.PermitWriteAcp = permitWriteAcp;
            ret.FullControl = fullControl;

            return ret;
        }

        internal static BucketAcl BucketUserAcl(
            string userGuid, 
            string issuedByUserGuid,
            bool permitRead,
            bool permitWrite,
            bool permitReadAcp,
            bool permitWriteAcp,
            bool fullControl)
        {
            if (String.IsNullOrEmpty(userGuid)) throw new ArgumentNullException(nameof(userGuid));
            if (String.IsNullOrEmpty(issuedByUserGuid)) throw new ArgumentNullException(nameof(issuedByUserGuid));

            BucketAcl ret = new BucketAcl();

            ret.UserGroup = null;
            ret.UserGUID = userGuid;
            ret.IssuedByUserGUID = issuedByUserGuid;

            ret.PermitRead = permitRead;
            ret.PermitWrite = permitWrite;
            ret.PermitReadAcp = permitReadAcp;
            ret.PermitWriteAcp = permitWriteAcp;
            ret.FullControl = fullControl;

            return ret;
        }
          
        internal static BucketAcl FromDataRow(DataRow row)
        {
            if (row == null) throw new ArgumentNullException(nameof(row));
             
            BucketAcl ret = new BucketAcl();

            if (row.Table.Columns.Contains("Id") && row["Id"] != DBNull.Value && row["Id"] != null)
                ret.Id = Convert.ToInt32(row["Id"]);

            if (row.Table.Columns.Contains("UserGroup") && row["UserGroup"] != DBNull.Value && row["UserGroup"] != null)
                ret.UserGroup = row["UserGroup"].ToString();

            if (row.Table.Columns.Contains("UserGUID") && row["UserGUID"] != DBNull.Value && row["UserGUID"] != null)
                ret.UserGUID = row["UserGUID"].ToString();

            if (row.Table.Columns.Contains("IssuedByUserGUID") && row["IssuedByUserGUID"] != DBNull.Value && row["IssuedByUserGUID"] != null)
                ret.IssuedByUserGUID = row["IssuedByUserGUID"].ToString();
            
            if (row.Table.Columns.Contains("PermitRead") && row["PermitRead"] != DBNull.Value && row["PermitRead"] != null)
                if (Convert.ToBoolean(row["PermitRead"])) ret.PermitRead = true;

            if (row.Table.Columns.Contains("PermitWrite") && row["PermitWrite"] != DBNull.Value && row["PermitWrite"] != null)
                if (Convert.ToBoolean(row["PermitWrite"])) ret.PermitWrite = true;

            if (row.Table.Columns.Contains("PermitReadAcp") && row["PermitReadAcp"] != DBNull.Value && row["PermitReadAcp"] != null)
                if (Convert.ToBoolean(row["PermitReadAcp"])) ret.PermitReadAcp = true;

            if (row.Table.Columns.Contains("PermitWriteAcp") && row["PermitWriteAcp"] != DBNull.Value && row["PermitWriteAcp"] != null)
                if (Convert.ToBoolean(row["PermitWriteAcp"])) ret.PermitWriteAcp = true;

            if (row.Table.Columns.Contains("FullControl") && row["FullControl"] != DBNull.Value && row["FullControl"] != null)
                if (Convert.ToBoolean(row["FullControl"])) ret.FullControl = true;

            return ret;
        }

        #endregion

        #region Public-Methods

        public override string ToString()
        {
            string
                ret = "--- Bucket ACL " + Id + " ---" + Environment.NewLine +
                "  User group      : " + UserGroup + Environment.NewLine +
                "  User GUID       : " + UserGUID + Environment.NewLine +
                "  Issued by       : " + IssuedByUserGUID + Environment.NewLine +
                "  Permissions     : " + Environment.NewLine +
                "    READ          : " + PermitRead.ToString() + Environment.NewLine +
                "    WRITE         : " + PermitWrite.ToString() + Environment.NewLine +
                "    READ_ACP      : " + PermitReadAcp.ToString() + Environment.NewLine +
                "    WRITE_ACP     : " + PermitWriteAcp.ToString() + Environment.NewLine +
                "    FULL_CONTROL  : " + FullControl.ToString() + Environment.NewLine; 

            return ret;
        }

        #endregion

        #region Private-Methods

        #endregion
    }
}
