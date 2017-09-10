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
    public class _LoginCountRepository
    {
        private NpgsqlQueryFactory _database = new NpgsqlQueryFactory();

        internal const string TableName = "LoginCounts";
        internal const string FieldUserId = "UserId";
        internal const string FieldNumberOfTimes = "NumberOfTimes";
        internal const string FieldLastLoggedInDate = "LastLoggedInDate";

        internal static string fullTableName = Consts.Schema.Quoted() + "." + TableName.Quoted();

        public _LoginCountRepository()
        {
        }

        public void CountUserLogin(string userid)
        {
            LoginCounts countLogin = this.GetLoginCountById(userid);
            if (countLogin != null)
            {
                countLogin.UserId = userid;
                if (string.IsNullOrEmpty(countLogin.NumberOfTimes.ToString()))
                {
                    countLogin.NumberOfTimes = 1;
                }
                else
                {
                    countLogin.NumberOfTimes = countLogin.NumberOfTimes + 1;
                }
                countLogin.LastLoggedInDate = DateTime.Parse(GenericHelpers.Date());
                this.Update(countLogin);
            }
            else
            {
                LoginCounts newcountLogin = new LoginCounts();
                newcountLogin.UserId = userid;
                if (string.IsNullOrEmpty(newcountLogin.NumberOfTimes.ToString()))
                {
                    newcountLogin.NumberOfTimes = 1;
                }
                else
                {
                    newcountLogin.NumberOfTimes = countLogin.NumberOfTimes + 1;
                }
                newcountLogin.LastLoggedInDate = DateTime.Parse(GenericHelpers.Date());
                this.Insert(newcountLogin);
            }
        }

        internal static LoginCounts loadLoginCount(Dictionary<string, string> row)
        {
            if (row == null) return null;
            if (row.Count == 0) return null;
            LoginCounts LC = null;
            LC = (LoginCounts)Activator.CreateInstance(typeof(LoginCounts));
            LC.UserId = row[FieldUserId];
            LC.NumberOfTimes = string.IsNullOrEmpty(row[FieldNumberOfTimes]) ? 0 : long.Parse(row[FieldNumberOfTimes]);
            LC.LastLoggedInDate = string.IsNullOrEmpty(row[FieldLastLoggedInDate]) ? DateTime.Now : DateTime.Parse(row[FieldLastLoggedInDate]);

            return LC;
        }



        public LoginCounts GetLoginCountById(string userId)
        {

            string commandText = "SELECT * FROM " + fullTableName + " WHERE " + FieldUserId.Quoted() + " = @id";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@id", userId } };

            var row = _database.Query(commandText, parameters);//.ExecuteQueryGetSingleRow
            return loadLoginCount(GenericHelpers.AddToDictionary(row));
        }

        public int Insert(LoginCounts lc)
        {
            //var lowerCaseEmail = user.Email == null ? null : user.Email.ToLower();

            string commandText = "insert into " + fullTableName +
                "(" + FieldUserId.Quoted() + ", " +
                FieldNumberOfTimes.Quoted() + ", " +
                FieldLastLoggedInDate.Quoted() + ")" +
                " VALUES (@UserId, @NumberOfTimes, @LastLoggedInDate);";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", lc.UserId);
            parameters.Add("@NumberOfTimes", lc.NumberOfTimes);
            parameters.Add("@LastLoggedInDate", lc.LastLoggedInDate);

            return _database.Execute(commandText, parameters);
        }

        public int Update(LoginCounts lc)
        {
            string commandText = "UPDATE " + fullTableName + " set " +
                FieldUserId.Quoted() + " = @UserId, " +
                FieldNumberOfTimes.Quoted() + " = @NumberOfTimes, " +
                FieldLastLoggedInDate.Quoted() + " =  @LastLoggedInDate " +
                " where " + FieldUserId.Quoted() + " = @UserId";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", lc.UserId);
            parameters.Add("@NumberOfTimes", lc.NumberOfTimes);
            parameters.Add("@LastLoggedInDate", lc.LastLoggedInDate);

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes a user from the AspNetUsers table.
        /// </summary>
        /// <param name="userId">The user's id.</param>
        /// <returns></returns>
        private int Delete(Guid userId)
        {
            string commandText = "DELETE FROM " + fullTableName + " WHERE " + FieldUserId.Quoted() + " = @userId";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", userId.ToString());

            return _database.Execute(commandText, parameters);
        }
    }

}