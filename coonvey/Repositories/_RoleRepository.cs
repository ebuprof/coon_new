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
    public class _RoleRepository
    {
        private NpgsqlQueryFactory _database = new NpgsqlQueryFactory();

        internal const string tableName = "AspNetRoles";
        internal const string fieldId = "Id";
        internal const string fieldName = "Name";
        internal static string fullTableName = Consts.Schema.Quoted() + "." + tableName.Quoted();

        internal static IdentityRole loadRole(Dictionary<string, string> row)
        {
            if (row == null) return null;
            IdentityRole role = null;
            role = (IdentityRole)Activator.CreateInstance(typeof(IdentityRole));
            role.Id = row[fieldId];
            role.Name = row[fieldName];

            return role;
        }

        internal static List<IdentityRole> loadRole(List<Dictionary<string, string>> rows)
        {
            if (rows == null) return null;
            IdentityRole role = null;
            List<IdentityRole> roleList = new List<IdentityRole>();
            foreach (Dictionary<string, string> row in rows)
            {
                role = (IdentityRole)Activator.CreateInstance(typeof(IdentityRole));
                foreach (KeyValuePair<string, string> kvp in row)
                {

                    if (kvp.Key.Contains(fieldId))
                    {
                        role.Id = kvp.Value;
                    }
                    if (kvp.Key.Contains(fieldName))
                    {
                        role.Name = kvp.Value;
                    }

                }
                roleList.Add(role);
            }

            return roleList;
        }

        public List<IdentityRole> GetRoles()
        {
            string commandText = "SELECT * FROM " + fullTableName;
            var row = _database.Query(commandText, null);
            return loadRole(row);
        }

        /// <summary>
        /// Deletes a role record from the AspNetRoles table.
        /// </summary>
        /// <param name="roleId">The role Id</param>
        /// <returns></returns>
        public int Delete(string roleId)
        {
            string commandText = "DELETE FROM " + fullTableName + " WHERE " + fieldId.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", roleId);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new Role record in the AspNetRoles table.
        /// </summary>
        /// <param name="roleName">The role's name.</param>
        /// <returns></returns>
        public int Insert(IdentityRole role)
        {
            string commandText = "INSERT INTO " + fullTableName + " (" + fieldId.Quoted() + ", " + fieldName.Quoted() + ") VALUES (@id, @name)";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@name", role.Name);
            parameters.Add("@id", role.Id);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Returns a role name given the roleId.
        /// </summary>
        /// <param name="roleId">The role Id.</param>
        /// <returns>Role name.</returns>
        public string GetRoleName(string roleId)
        {
            string commandText = "SELECT " + fieldName.Quoted() + " FROM " + fullTableName + " WHERE " + fieldId.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", roleId);

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Returns the role Id given a role name.
        /// </summary>
        /// <param name="roleName">Role's name.</param>
        /// <returns>Role's Id.</returns>
        public string GetRoleId(string roleName)
        {
            string roleId = null;
            string commandText = "SELECT " + fieldId.Quoted() + " FROM " + fullTableName + " WHERE " + fieldName.Quoted() + " = @name";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@name", roleName } };

            //var result = _database.ExecuteQuery(commandText, parameters);
            var result = _database.GetStrValue(commandText, parameters);
            if (result != null)
            {

                return Convert.ToString(result);
            }

            return roleId;
        }

        /// <summary>
        /// Gets the IdentityRole given the role Id.
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IdentityRole GetRoleById(string roleId)
        {
            var roleName = GetRoleName(roleId);
            IdentityRole role = null;

            if (roleName != null)
            {
                role = new IdentityRole(roleName);
            }

            return role;

        }

        /// <summary>
        /// Gets the IdentityRole given the role name.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public IdentityRole GetRoleByName(string roleName)
        {
            var roleId = GetRoleId(roleName);


            return GetRoleById(roleId);
        }

        public int Update(IdentityRole role)
        {
            string commandText = "UPDATE " + fullTableName + " SET " + fieldName.Quoted() + " = @name WHERE " + fieldId.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@id", role.Id);
            parameters.Add("@name", role.Name);

            return _database.Execute(commandText, parameters);
        }

        public IList<IdentityUser> GetUsersInRole(string roleName)
        {
            string commandText = "SELECT * FROM " + fullTableName + " WHERE " +
            fieldName.Quoted() + "  = @Name";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Name", roleName);

            var rows = _database.Query(commandText, parameters);
            //var list = rows.Select(row => UserTable<IdentityUser>.loadUser(row)).ToList();
            var list = rows.Select(row => _UserRepository.loadUser(row)).ToList();

            return list;
        }
    }

}