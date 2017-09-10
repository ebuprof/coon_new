--INSTRUCTION
--Create database called 'UnitApp-20170820011253' using UTF-8 and then run the following sql commands
/*
Add Npgsql2.2.7 from nugget
Add Npgsql.EntityFramework from Nugget
*/
/*
Navicat PGSQL Data Transfer

Source Server         : PGConn
Source Server Version : 90219
Source Host           : localhost:5432
Source Database       : Coonvey-20170909035332
Source Schema         : public
Source Owner		  : Chukwuebuka<ebukaprof@gmail.com>

Target Server Type    : PGSQL
Target Server Version : 90219
File Encoding         : UTF-8

Date: 2017-09-09 16:26:53
*/

CREATE TABLE "AspNetRoles" ( 
  "Id" varchar(128) NOT NULL,
  "Name" varchar(256) NOT NULL,
  PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetUsers" (
  "Id" character varying(128) NOT NULL,
  "UserName" character varying(256) NOT NULL,
  "PasswordHash" character varying(256),
  "SecurityStamp" character varying(256),
  "Email" character varying(256) DEFAULT NULL::character varying,
  "EmailConfirmed" boolean NOT NULL DEFAULT false,
  "PhoneNumber" character varying(256),
  "PhoneNumberConfirmed" boolean NOT NULL DEFAULT false,
  "TwoFactorEnabled" boolean NOT NULL DEFAULT false,
  "LockoutEndDateUtc" timestamp,
  "LockoutEnabled" boolean NOT NULL DEFAULT false,
  "AccessFailedCount" int NOT NULL DEFAULT 0,

  PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetUserClaims" ( 
  "Id" serial NOT NULL,
  "ClaimType" varchar(256) NULL,
  "ClaimValue" varchar(256) NULL,
  "UserId" varchar(128) NOT NULL,
  PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetUserLogins" ( 
  "UserId" varchar(128) NOT NULL,
  "LoginProvider" varchar(128) NOT NULL,
  "ProviderKey" varchar(128) NOT NULL,
  PRIMARY KEY ("UserId", "LoginProvider", "ProviderKey")
);

CREATE TABLE "AspNetUserRoles" ( 
  "UserId" varchar(128) NOT NULL,
  "RoleId" varchar(128) NOT NULL,
  PRIMARY KEY ("UserId", "RoleId")
);

-- ----------------------------
-- Table structure for Profiles
-- ----------------------------
DROP TABLE IF EXISTS "Profiles";
CREATE TABLE "Profiles" (
"Id" uuid NOT NULL,
"UserId" uuid NOT NULL,
"FirstName" varchar(50) COLLATE "default" NOT NULL,
"MiddleName" varchar(50) COLLATE "default",
"LastName" varchar(50) COLLATE "default" NOT NULL,
"Address" varchar(200) COLLATE "default",
"City" varchar(200) COLLATE "default",
"StateId" varchar(200) COLLATE "default",
"CountryId" varchar(200) COLLATE "default",
"ReligionId" varchar(200) COLLATE "default",
"GenderId" varchar(200) COLLATE "default",
"LgaId" varchar(200) COLLATE "default",
"DateOfBirth" date,
"PlaceOfBirth" varchar COLLATE "default",
"DateCreated" timestamp(6),
"MarkedForDeletion" bool DEFAULT false,
"DateMarkedForDeletion" timestamp(6),
"DateModified" timestamp(6),
"Activated" bool DEFAULT true NOT NULL,
"MaritalStatusId" varchar(200) COLLATE "default",
"PhotoId" varchar COLLATE "default"
)
WITH (OIDS=FALSE);

-- ----------------------------
-- Table structure for LoginCounts
-- ----------------------------
DROP TABLE IF EXISTS "public"."LoginCounts";
CREATE TABLE "public"."LoginCounts" (
"UserId" varchar COLLATE "default" NOT NULL,
"NumberOfTimes" int8,
"LastLoggedInDate" timestamp(6)
)
WITH (OIDS=FALSE);

-- ----------------------------
-- Table structure for LoginAudits
-- ----------------------------
DROP TABLE IF EXISTS "public"."LoginAudits";
CREATE TABLE "public"."LoginAudits" (
"UserId" varchar(150) COLLATE "default" NOT NULL,
"Timestamp" timestamp(15) NOT NULL,
"AuditEvent" varchar(15) COLLATE "default" NOT NULL,
"IpAddress" varchar(15) COLLATE "default" NOT NULL,
"AuditId" varchar(200) COLLATE "default" DEFAULT NULL::character varying NOT NULL
)
WITH (OIDS=FALSE);


CREATE INDEX "IX_AspNetUserClaims_UserId"	ON "AspNetUserClaims"	("UserId");
CREATE INDEX "IX_AspNetUserLogins_UserId"	ON "AspNetUserLogins"	("UserId");
CREATE INDEX "IX_AspNetUserRoles_RoleId"	ON "AspNetUserRoles"	("RoleId");
CREATE INDEX "IX_AspNetUserRoles_UserId"	ON "AspNetUserRoles"	("UserId");

ALTER TABLE "AspNetUserClaims"
  ADD CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_User_Id" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id")
  ON DELETE CASCADE;

ALTER TABLE "AspNetUserLogins"
  ADD CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id")
  ON DELETE CASCADE;

ALTER TABLE "AspNetUserRoles"
  ADD CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id")
  ON DELETE CASCADE;

ALTER TABLE "AspNetUserRoles"
  ADD CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id")
  ON DELETE CASCADE;

  -- ----------------------------
-- Primary Key structure for table Profiles
-- ----------------------------
ALTER TABLE "Profiles" ADD PRIMARY KEY ("Id");
-- ----------------------------
-- Primary Key structure for table LoginCounts
-- ----------------------------
ALTER TABLE "public"."LoginCounts" ADD PRIMARY KEY ("UserId");
-- ----------------------------
-- Primary Key structure for table LoginAudits
-- ----------------------------
ALTER TABLE "public"."LoginAudits" ADD PRIMARY KEY ("AuditId");