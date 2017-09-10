using coonvey.Constants;
using coonvey.Helpers;
using coonvey.Soul;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace coonvey.Repositories
{
    public class _UserRoleRepository
    {
        private NpgsqlQueryFactory _database = new NpgsqlQueryFactory();
        internal const string tableName = "AspNetUserRoles";
        internal const string fieldUserID = "UserId";
        internal const string fieldRoleID = "RoleId";
        internal static string fullTableName = Consts.Schema.Quoted() + "." + tableName.Quoted();

        /// <summary>
        /// Retorna uma lista de Roles do Usuário em questão
        /// </summary>
        /// <param name="userId">Código do Usuário</param>
        /// <returns></returns>
        public List<string> FindByUserId(Guid userId)
        {
            List<string> roles = new List<string>();
            //TODO: This probably does not work, and may need testing.

            string commandText = "select AspRoles." + _RoleRepository.fieldName.Quoted() + " from " + _UserRepository.fullTableName + " AspUsers" +
                " INNER JOIN " + fullTableName + " AspUserRoles " +
                " ON AspUsers." + _UserRepository.FieldId.Quoted() + " = AspUserRoles." + fieldUserID.Quoted() +
                " INNER JOIN " + _RoleRepository.fullTableName + " AspRoles " +
                " ON AspUserRoles." + fieldRoleID.Quoted() + " = AspRoles." + _RoleRepository.fieldId.Quoted() +
                " where AspUsers." + _UserRepository.FieldId.Quoted() + " = @userId";

            /*select AspNetRoles.Name from AspNetUsers
             * inner join AspNetUserRoles
             * ON AspNetUsers.ID = AspNetUserRoles.UserID
             * inner join AspNetRoles
             * ON aspNetUserRoles.RoleID = AspNetRoles.ID
             * where AspNetUser.ID = :id
            */

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", userId);

            var rows = _database.Query(commandText, parameters);
            foreach (var row in rows)
            {
                roles.Add(row[fieldRoleID]);
            }

            return roles;
        }

        /// <summary>
        /// Deletes all roles from a user in the AspNetUserRoles table.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            string commandText = "DELETE FROM " + fullTableName + " WHERE " + fieldUserID.Quoted() + " = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", userId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new role record for a user in the UserRoles table.
        /// </summary>
        /// <param name="ur"></param>
        /// <returns></returns>

        internal int Insert(IdentityUserRole ur)
        {
            string commandText = "INSERT INTO " + fullTableName + " (" + fieldUserID.Quoted() + ", " + fieldRoleID.Quoted() + ") VALUES (@userId, @roleId)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", ur.UserId);
            parameters.Add("roleId", ur.RoleId);

            return _database.Execute(commandText, parameters);
        }

        internal int Delete(Guid roleId, Guid userId)
        {
            string commandText = "DELETE FROM " + fullTableName + " WHERE " + fieldUserID.Quoted() + " = @userId and " + fieldRoleID.Quoted() + " = @roleId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", userId);
            parameters.Add("roleId", roleId);

            return _database.Execute(commandText, parameters);
        }

        public bool GetRoleExistsInUser(Guid userId, Guid roleId)
        {
            string commandText = "SELECT * from " + fullTableName + " " +
                                 "WHERE " + fieldUserID.Quoted() + " = @userId and " + fieldRoleID.Quoted() +
                                 " = @roleId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userId", userId);
            parameters.Add("roleId", roleId.ToString());

            //return _database.ExecuteSQL(commandText, parameters) > 0;
            //long u = _database.ExecuteSQL(commandText, parameters);

            List<Dictionary<string, string>> l = _database.Query(commandText, parameters);
            if (l.Count >= 1)
                return true;
            return false;
        }
    }

}