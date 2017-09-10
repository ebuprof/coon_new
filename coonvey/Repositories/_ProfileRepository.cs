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
    public class _ProfileRepository
    {
        private NpgsqlQueryFactory _database;
        private ApplicationDbContext db = new ApplicationDbContext();
        public _ProfileRepository()
        {
            _database = new NpgsqlQueryFactory();
        }
        internal const string TableName = "Profiles";
        internal const string FieldId = "Id";
        internal const string FieldUserId = "UserId";
        internal const string FieldFirstName = "FirstName";
        internal const string FieldMiddleName = "MiddleName";
        internal const string FieldLastName = "LastName";
        internal const string FieldAddress = "Address";
        internal const string FieldCity = "City";
        internal const string FieldStateId = "StateId";
        internal const string FieldCountryId = "CountryId";
        internal const string FieldReligionId = "ReligionId";
        internal const string FieldGenderId = "GenderId";
        internal const string FieldLgaId = "LgaId";
        internal const string FieldDateOfBirth = "DateOfBirth";
        internal const string FieldPlaceOfBirth = "PlaceOfBirth";
        internal const string FieldDateCreated = "DateCreated";
        internal const string FieldMarkedForDeletion = "MarkedForDeletion";
        internal const string FieldDateMarkedForDeletion = "DateMarkedForDeletion";
        internal const string FieldDateModified = "DateModified";
        internal const string FieldActivated = "Activated";
        internal const string FieldMaritalStatusId = "MaritalStatusId";
        internal const string FieldProfilePhotoId = "PhotoId";

        internal static string fullTableName = Consts.Schema.Quoted() + "." + TableName.Quoted();

        internal static Profiles loadProfile(Dictionary<string, string> row)
        {
            if (row == null) return null;
            Profiles user = null;
            user = (Profiles)Activator.CreateInstance(typeof(Profiles));
            user.Id = new Guid(row[FieldId]); //new Guid(row[FieldId]);
            user.UserId = new Guid(row[FieldUserId]);
            user.FirstName = string.IsNullOrEmpty(row[FieldFirstName]) ? null : row[FieldFirstName];
            user.MiddleName = string.IsNullOrEmpty(row[FieldMiddleName]) ? null : row[FieldMiddleName];
            user.LastName = string.IsNullOrEmpty(row[FieldLastName]) ? null : row[FieldLastName];
            user.Address = string.IsNullOrEmpty(row[FieldAddress]) ? null : row[FieldAddress];
            user.City = string.IsNullOrEmpty(row[FieldCity]) ? null : row[FieldCity];
            user.StateId = row[FieldStateId];
            user.CountryId = row[FieldCountryId];
            user.ReligionId = row[FieldReligionId];
            user.GenderId = row[FieldGenderId];
            user.LgaId = row[FieldLgaId];
            user.DateOfBirth = DateTime.Parse(row[FieldDateOfBirth]);
            user.PlaceOfBirth = string.IsNullOrEmpty(row[FieldPlaceOfBirth]) ? null : row[FieldPlaceOfBirth];
            user.DateCreated = DateTime.Parse(row[FieldDateCreated]);
            user.MarkedForDeletion = row[FieldMarkedForDeletion] == "1" ? true : false;
            user.DateMarkedForDeletion = string.IsNullOrEmpty(row[FieldDateMarkedForDeletion]) ? DateTime.Now : DateTime.Parse(row[FieldDateMarkedForDeletion]);
            user.DateModified = string.IsNullOrEmpty(row[FieldDateModified]) ? DateTime.Now : DateTime.Parse(row[FieldDateModified]);
            user.MaritalStatusId = row[FieldMaritalStatusId];
            user.Activated = row[FieldActivated] == "1" ? true : false;
            user.PhotoId = row[FieldProfilePhotoId];

            return user;

        }
        public string GetProfileIdByUserId(string userId)
        {
            string commandText = "SELECT " + FieldId.Quoted() + " FROM " + fullTableName + " WHERE " + FieldUserId.Quoted() + " = @userid";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@userid", userId } };

            return _database.GetStrValue(commandText, parameters);
        }

        public string GetUserIdByProfileId(string profileId)
        {
            string commandText = "SELECT " + FieldUserId.Quoted() + " FROM " + fullTableName + " WHERE " + FieldId.Quoted() + " = @profileid";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@profileid", profileId } };

            return _database.GetStrValue(commandText, parameters);
        }

        public string GetFirstNameByProfileId(string profileId)
        {
            string commandText = "SELECT " + FieldFirstName.Quoted() + " FROM " + fullTableName + " WHERE " + FieldId.Quoted() + " = @profileid";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@profileid", profileId } };

            return _database.GetStrValue(commandText, parameters);
        }

        public string GetFirstNameByUserId(string userId)
        {
            string commandText = "SELECT " + FieldFirstName.Quoted() + " FROM " + fullTableName + " WHERE " + FieldUserId.Quoted() + " = @userid";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@userid", userId } };

            return _database.GetStrValue(commandText, parameters);
        }

        public string GetFullNameByUserId(string userId)
        {
            string commandText = string.Format("select * from {0}('SELECT  concat({1},'' '',{2},'' '',{3}) as name') where {4} = @userid", fullTableName, FieldFirstName.Quoted(), FieldLastName.Quoted(), FieldMiddleName.Quoted(), FieldUserId.Quoted());
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@userid", userId } };

            return _database.GetStrValue(commandText, parameters);
        }

        public Profiles GetProfileByUserId(string userId)
        {
            string commandText = "SELECT * FROM " + fullTableName + " WHERE " + FieldUserId.Quoted() + " = @userid";
            Dictionary<string, object> parameters = new Dictionary<string, object>() { { "@userid", userId } };

            var row = _database.Query(commandText, parameters);
            return loadProfile(GenericHelpers.AddToDictionary(row));
        }

        public int Insert(Profiles n)
        {
            string commandText = "insert into " + fullTableName +
                "(" + FieldId.Quoted() + ", " +
                FieldUserId.Quoted() + ", " +
                FieldActivated.Quoted() + ", " +
                FieldCity.Quoted() + ", " +
                FieldAddress.Quoted() + ", " +
                FieldCountryId.Quoted() + ", " +
                FieldDateCreated.Quoted() + ", " +
                FieldDateMarkedForDeletion.Quoted() + ", " +
                FieldDateModified.Quoted() + ", " +
                FieldDateOfBirth.Quoted() + ", " +
                FieldFirstName.Quoted() + ", " +
                FieldGenderId.Quoted() + ", " +
                FieldLastName.Quoted() + ", " +
                FieldMiddleName.Quoted() + ", " +
                FieldLgaId.Quoted() + ", " +
                FieldMaritalStatusId.Quoted() + ", " +
                FieldMarkedForDeletion.Quoted() + ", " +
                FieldPlaceOfBirth.Quoted() + ", " +
                FieldProfilePhotoId.Quoted() + ", " +
                FieldReligionId.Quoted() + ", " +
                FieldStateId.Quoted() + ")" +
                " VALUES (@Id, @UserId, @Activated, @City, @Address, @CountryId, @DateCreated, @DateMarkedForDeletion, @DateModified, @DateOfBirth, @FirstName, @GenderId, @LastName, @MiddleName, @LgaId, @MaritalStatusId, @MarkedForDeletion, @PlaceOfBirth, @PhotoId, @ReligionId, @StateId);";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", n.Id);
            parameters.Add("@UserId", n.UserId);
            parameters.Add("@Activated", n.Activated);
            parameters.Add("@City", n.City);
            parameters.Add("@Address", n.Address);
            parameters.Add("@CountryId", n.CountryId);
            parameters.Add("@DateCreated", n.DateCreated);
            parameters.Add("@DateMarkedForDeletion", n.DateMarkedForDeletion);
            parameters.Add("@DateModified", n.DateModified);
            parameters.Add("@DateOfBirth", n.DateOfBirth);
            parameters.Add("@FirstName", n.FirstName);
            parameters.Add("@GenderId", n.GenderId);
            parameters.Add("@LastName", n.LastName);
            parameters.Add("@MiddleName", n.MiddleName);
            parameters.Add("@LgaId", n.LgaId);
            parameters.Add("@MaritalStatusId", n.MaritalStatusId);
            parameters.Add("@MarkedForDeletion", n.MarkedForDeletion);
            parameters.Add("@PlaceOfBirth", n.PlaceOfBirth);
            parameters.Add("@PhotoId", n.PhotoId);
            parameters.Add("@ReligionId", n.ReligionId);
            parameters.Add("@StateId", n.StateId);

            return _database.Execute(commandText, parameters);
        }

        public int Update(Profiles n)
        {
            string commandText = "UPDATE " + fullTableName + " set " +
                FieldActivated.Quoted() + " =  @Activated, " +
                FieldCity.Quoted() + " = @City, " +
                FieldAddress.Quoted() + " = @Address, " +
                FieldCountryId.Quoted() + " = @CountryId, " +
                FieldDateCreated.Quoted() + " = @DateCreated, " +
                FieldDateMarkedForDeletion.Quoted() + " = @DateMarkedForDeletion, " +
                FieldDateModified.Quoted() + " = @DateModified, " +
                FieldDateOfBirth.Quoted() + " = @DateOfBirth, " +
                FieldFirstName.Quoted() + " = @FirstName, " +
                FieldGenderId.Quoted() + " = @GenderId, " +
                FieldLastName.Quoted() + " = @LastName, " +
                FieldMiddleName.Quoted() + " @MiddleName, " +
                FieldLgaId.Quoted() + " = @LgaId, " +
                FieldMaritalStatusId.Quoted() + " = @MaritalStatusId, " +
                FieldMarkedForDeletion.Quoted() + " = @MarkedForDeletion, " +
                FieldPlaceOfBirth.Quoted() + " = @PlaceOfBirth, " +
                FieldProfilePhotoId.Quoted() + " = @PhotoId, " +
                FieldReligionId.Quoted() + " = @ReligionId, " +
                FieldStateId.Quoted() + " = @StateId " +
                " where " + FieldUserId.Quoted() + " = @UserId";

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", n.Id);
            parameters.Add("@UserId", n.UserId);
            parameters.Add("@Activated", n.Activated);
            parameters.Add("@City", n.City);
            parameters.Add("@Address", n.Address);
            parameters.Add("@CountryId", n.CountryId);
            parameters.Add("@DateCreated", n.DateCreated);
            parameters.Add("@DateMarkedForDeletion", n.DateMarkedForDeletion);
            parameters.Add("@DateModified", n.DateModified);
            parameters.Add("@DateOfBirth", n.DateOfBirth);
            parameters.Add("@FirstName", n.FirstName);
            parameters.Add("@GenderId", n.GenderId);
            parameters.Add("@LastName", n.LastName);
            parameters.Add("@MiddleName", n.MiddleName);
            parameters.Add("@LgaId", n.LgaId);
            parameters.Add("@MaritalStatusId", n.MaritalStatusId);
            parameters.Add("@MarkedForDeletion", n.MarkedForDeletion);
            parameters.Add("@PlaceOfBirth", n.PlaceOfBirth);
            parameters.Add("@PhotoId", n.PhotoId);
            parameters.Add("@ReligionId", n.ReligionId);
            parameters.Add("@StateId", n.StateId);

            return _database.Execute(commandText, parameters);
        }

        private int Delete(Guid id)
        {
            string commandText = "DELETE FROM " + fullTableName + " WHERE " + FieldId.Quoted() + " = @Id";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@userId", id);

            return _database.Execute(commandText, parameters);
        }

    }

}