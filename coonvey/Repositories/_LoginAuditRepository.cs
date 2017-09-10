using coonvey.Constants;
using coonvey.Helpers;
using coonvey.Models;
using coonvey.Soul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace coonvey.Repositories
{
    public class _LoginAuditRepository
    {
        private NpgsqlQueryFactory _database = new NpgsqlQueryFactory();

        internal const string TableName = "LoginAudits";
        internal const string FieldUserAuditId = "AuditId";
        internal const string FieldUserId = "UserId";
        internal const string FieldTimestamp = "Timestamp";
        internal const string FieldAuditEvent = "AuditEvent";
        internal const string FieldIpAddress = "IpAddress";

        internal static string fullTableName = Consts.Schema.Quoted() + "." + TableName.Quoted();

        public int Insert(LoginAudits userAudit)
        {

            string commandText = "insert into " + fullTableName +
                "(" + FieldUserId.Quoted() + ", " +
                FieldUserAuditId.Quoted() + ", " +
                FieldAuditEvent.Quoted() + ", " +
                FieldTimestamp.Quoted() + ", " +
                FieldIpAddress.Quoted() + ")" +
                " VALUES (@UserId, @AuditId, @AuditEvent, @Timestamp, @IpAddress);";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", userAudit.UserId);
            parameters.Add("@AuditId", userAudit.AuditId);
            parameters.Add("@AuditEvent", userAudit.AuditEvent.ToString());
            parameters.Add("@Timestamp", GenericHelpers.Date());
            parameters.Add("@IpAddress", userAudit.IpAddress);

            return _database.Execute(commandText, parameters);
        }
    }

}