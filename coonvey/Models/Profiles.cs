using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace coonvey.Models
{
    public class Profiles
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string StateId { get; set; }
        public string CountryId { get; set; }
        public string ReligionId { get; set; }
        public string GenderId { get; set; }
        public string LgaId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PlaceOfBirth { get; set; }
        public DateTime DateCreated { get; set; }
        public bool MarkedForDeletion { get; set; }
        public DateTime DateMarkedForDeletion { get; set; }
        public DateTime DateModified { get; set; }
        public bool Activated { get; set; }
        public string MaritalStatusId { get; set; }
        public string PhotoId { get; set; }
    }
}