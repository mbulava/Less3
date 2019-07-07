﻿using System;
using System.Collections.Generic;
using System.Text;

using SqliteWrapper;

namespace Less3.Classes
{
    /// <summary>
    /// Database queries.
    /// </summary>
    internal static class DatabaseQueries
    {
        #region Table-Creation

        internal static string CreateObjectTable()
        {
            string query =
                "CREATE TABLE IF NOT EXISTS Objects " +
                "(" +
                "  Id                INTEGER PRIMARY KEY, " +
                "  Owner             VARCHAR(64), " +
                "  Author            VARCHAR(64), " +
                "  Key               VARCHAR(256), " +
                "  ContentType       VARCHAR(128), " +
                "  ContentLength     INTEGER, " +
                "  Version           INTEGER, " +
                "  BlobFilename      VARCHAR(64), " +
                "  Etag              VARCHAR(64), " +
                "  RetentionType     VARCHAR(16), " +
                "  DeleteMarker      INTEGER, " +
                "  Md5               VARCHAR(32), " + 
                "  CreatedUtc        VARCHAR(32), " +
                "  LastUpdateUtc     VARCHAR(32), " +
                "  LastAccessUtc     VARCHAR(32), " +
                "  ExpirationUtc     VARCHAR(32) " +
                ")";
            return query;
        }

        internal static string CreateTagsTable()
        {
            string query = 
                "CREATE TABLE IF NOT EXISTS Tags " + 
                "(" +
                "  Id                INTEGER PRIMARY KEY, " +
                "  ObjectKey         VARCHAR(64), " +
                "  ObjectVersion     INTEGER, " +
                "  Key               VARCHAR(256), " +
                "  Value             VARCHAR(512) " +
                ")";
            return query;
        }

        #endregion

        #region Tags-Queries

        internal static string GetBucketTags()
        {
            string query =
                "SELECT * FROM Tags WHERE " +
                "  ObjectKey IS NULL " +
                "  AND ObjectVersion IS NULL";
            return query;
        }
            
        internal static string GetObjectTags(string key, long version)
        {
            string query =
                "SELECT * FROM Tags WHERE " +
                "  ObjectKey = '" + Sanitize(key) + "' " +
                "  AND ObjectVersion = '" + version + "'";
            return query;
        }

        internal static string DeleteBucketTags()
        {
            string query =
                "DELETE FROM Tags WHERE " +
                "  ObjectKey IS NULL " +
                "  AND ObjectVersion IS NULL";
            return query;
        }

        internal static string DeleteObjectTags(string key, long version)
        {
            string query =
                "DELETE FROM Tags WHERE " +
                "  ObjectKey = '" + Sanitize(key) + "' " +
                "  AND ObjectVersion = '" + version + "'";
            return query;
        }

        internal static string InsertBucketTags(Dictionary<string, string> tags)
        {
            if (tags == null || tags.Count < 1) throw new ArgumentNullException(nameof(tags));

            string query =
                "INSERT INTO Tags " +
                "( " +
                "  ObjectKey, " +
                "  ObjectVersion, " +
                "  Key, " +
                "  Value " +
                ") " +
                "VALUES ";

            int added = 0;

            foreach (KeyValuePair<string, string> curr in tags)
            {
                if (String.IsNullOrEmpty(curr.Key)) continue;

                if (added > 0) query += ",";

                query +=
                    "( " +
                    "  null, " +
                    "  null, " +
                    "  '" + Sanitize(curr.Key) + "', " +
                    "  '" + Sanitize(curr.Value) + "'" +
                    ") ";

                added++;
            }

            return query;
        }

        internal static string InsertObjectTags(string key, long version, Dictionary<string, string> tags)
        {
            if (tags == null || tags.Count < 1) throw new ArgumentNullException(nameof(tags));

            string query =
                "INSERT INTO Tags " +
                "( " +
                "  ObjectKey, " +
                "  ObjectVersion, " +
                "  Key, " +
                "  Value " +
                ") " +
                "VALUES ";

            int added = 0;

            foreach (KeyValuePair<string, string> curr in tags)
            {
                if (String.IsNullOrEmpty(curr.Key)) continue;

                if (added > 0) query += ",";

                query += 
                    "( " +
                    "  '" + Sanitize(key) + "', " +
                    "  '" + version + "', " +
                    "  '" + Sanitize(curr.Key) + "', " +
                    "  '" + Sanitize(curr.Value) + "'" +
                    ") ";

                added++;
            }

            return query;
        }

        #endregion

        #region Object-Queries

        internal static string ObjectExists(string key)
        {
            string query =
                "SELECT * FROM Objects " +
                "WHERE Key = '" + Sanitize(key) + "' " +
                "ORDER BY LastUpdateUtc DESC " +
                "LIMIT 1";
            return query;
        }

        internal static string VersionExists(string key, long version)
        {
            string query =
                "SELECT * FROM Objects " +
                "WHERE Key = '" + Sanitize(key) + "' " +
                "AND Version = '" + version + "' " +
                "ORDER BY LastUpdateUtc DESC " +
                "LIMIT 1";
            return query;
        }

        internal static string InsertObject(Obj obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            string query =
                "INSERT INTO Objects " +
                "(" +
                "  Owner, " +
                "  Author, " +
                "  Key, " +
                "  ContentType, " +
                "  ContentLength, " +
                "  Version, " +
                "  BlobFilename, " +
                "  Etag, " +
                "  RetentionType, " +
                "  DeleteMarker, " +
                "  Md5, " + 
                "  CreatedUtc, " +
                "  LastUpdateUtc, " +
                "  LastAccessUtc, " +
                "  ExpirationUtc " +
                ") VALUES (" +
                "  '" + Sanitize(obj.Owner) + "', " +
                "  '" + Sanitize(obj.Author) + "', " +
                "  '" + Sanitize(obj.Key) + "', " +
                "  '" + Sanitize(obj.ContentType) + "', " +
                "  '" + obj.ContentLength + "', " +
                "  '" + obj.Version + "', " +
                "  '" + Sanitize(obj.BlobFilename) + "', " +
                "  '" + Sanitize(obj.Etag) + "', " +
                "  '" + Sanitize(obj.RetentionType) + "', " +
                "  '" + obj.DeleteMarker + "', " +
                "  '" + Sanitize(obj.Md5) + "', " + 
                "  '" + TimestampUtc(obj.CreatedUtc) + "', " +
                "  '" + TimestampUtc(obj.LastUpdateUtc) + "', " +
                "  '" + TimestampUtc(obj.LastAccessUtc) + "', " +
                "  '" + TimestampUtc(obj.ExpirationUtc) + "' " +
                ")";
            return query;
        }

        internal static string DeleteObject(string key, long version)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            string query =
                "DELETE FROM Objects WHERE Key = '" + Sanitize(key) + "' " +
                "AND Version = '" + version + "'";
            return query;
        }

        internal static string MarkObjectDeleted(Obj obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            DateTime ts = DateTime.Now.ToUniversalTime();
             
            string query =
                "INSERT INTO Objects " +
                "(" +
                "  Owner, " +
                "  Author, " +
                "  Key, " +
                "  ContentType, " +
                "  ContentLength, " +
                "  Version, " +
                "  BlobFilename, " +
                "  Etag, " +
                "  RetentionType, " +
                "  DeleteMarker, " +
                "  Md5, " + 
                "  CreatedUtc, " +
                "  LastUpdateUtc, " +
                "  LastAccessUtc, " +
                "  ExpirationUtc " +
                ") VALUES (" +
                "  '" + Sanitize(obj.Owner) + "', " +
                "  '" + Sanitize(obj.Author) + "', " +
                "  '" + Sanitize(obj.Key) + "', " +
                "  '" + Sanitize(obj.ContentType) + "', " +
                "  0, " +
                "  '" + (obj.Version + 1) + "', " +
                "  null, " +
                "  '" + Sanitize(obj.Etag) + "', " +
                "  '" + Sanitize(obj.RetentionType) + "', " +
                "  '1', " +
                "  null, " + 
                "  '" + TimestampUtc(obj.CreatedUtc) + "', " +
                "  '" + TimestampUtc(ts) + "', " +
                "  '" + TimestampUtc(obj.LastAccessUtc) + "', " +
                "  '" + TimestampUtc(obj.ExpirationUtc) + "' " +
                ")";
            return query;
        }

        internal static string UpdateRecord(string key, long version, Dictionary<string, object> vals)
        {
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key)); 
            if (vals == null || vals.Count < 1) throw new ArgumentNullException(nameof(vals));

            int added = 0;
            string query =
                "UPDATE Objects SET ";

            foreach (KeyValuePair<string, object> curr in vals)
            {
                if (String.IsNullOrEmpty(curr.Key)) continue;

                if (added == 0)
                {
                    query += Sanitize(curr.Key) + " = "; 
                    if (curr.Value == null) query += " null ";
                    else if (curr.Value is DateTime) query += "'" + TimestampUtc(Convert.ToDateTime(curr.Value)) + "' ";
                    else if (curr.Value is string) query += "'" + Sanitize(curr.Value.ToString()) + "' ";
                    else query += "'" + curr.Value.ToString() + "' ";
                }
                else
                {
                    query += "," + Sanitize(curr.Key) + " = ";
                    if (curr.Value == null) query += " null ";
                    else if (curr.Value is DateTime) query += "'" + TimestampUtc(Convert.ToDateTime(curr.Value)) + "' ";
                    else if (curr.Value is string) query += "'" + Sanitize(curr.Value.ToString()) + "' ";
                    else query += "'" + curr.Value.ToString() + "' ";
                }
                added++;
            }

            query += 
                "WHERE Key = '" + Sanitize(key) + "' " +
                "AND Version = '" + version + "'";
            return query;
        }

        internal static string GetObjectCount()
        {
            return "SELECT COUNT(*) AS NumObjects, SUM(ContentLength) AS TotalBytes FROM Objects";
        }

        internal static string Enumerate(string prefix, long indexStart, int maxResults)
        {
            string query =
                "SELECT * FROM " +
                "( " +
                "  SELECT * FROM Objects " +
                "  WHERE Id > 0 " +
                "  AND DeleteMarker = 0 ";

            if (!String.IsNullOrEmpty(prefix))
                query += "AND Key LIKE '" + Sanitize(prefix) + "%' ";

            query += 
                "  ORDER BY LastUpdateUtc DESC " +
                ") " +
                "GROUP BY Key LIMIT " + maxResults + " OFFSET " + indexStart;
                 
            return query;
        }

        internal static string EnumerationVersions(string prefix, long indexStart, int maxResults)
        {
            string query =
                "SELECT * FROM " +
                "(" +
                "  SELECT * FROM Objects WHERE Id > 0 ";

            if (!String.IsNullOrEmpty(prefix))
                query += "AND Key LIKE '" + Sanitize(prefix) + "%' ";

            query +=
                "  ORDER BY LastUpdateUtc DESC " +
                ")" +
                "GROUP BY Key " +
                "LIMIT " + maxResults + " " +
                "OFFSET " + maxResults;

            return query;
        }

        #endregion

        internal static string TimestampFormat = "yyyy-MM-ddTHH:mm:ss.ffffffZ";

        internal static string Sanitize(string str)
        {
            return DatabaseClient.SanitizeString(str);
        }

        internal static string TimestampUtc()
        {
            return DateTime.Now.ToUniversalTime().ToString(TimestampFormat);
        }

        internal static string TimestampUtc(DateTime? ts)
        {
            if (ts == null) return null;
            return Convert.ToDateTime(ts).ToUniversalTime().ToString(TimestampFormat);
        }

        internal static string TimestampUtc(DateTime ts)
        {
            return ts.ToUniversalTime().ToString(TimestampFormat);
        }
    }
}
