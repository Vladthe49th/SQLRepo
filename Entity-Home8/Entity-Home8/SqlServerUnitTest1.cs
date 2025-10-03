using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using FakeItEasy.Sdk;
using Microsoft.Data.Tools.Schema.Sql.UnitTesting;
using Microsoft.Data.Tools.Schema.Sql.UnitTesting.Conditions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Entity_Home8
{

    Create PROCEDURE GetUsersWithCompanies
AS
BEGIN
    SELECT u.Id, u.Email, u.Age, u.Salary, c.Name AS CompanyName
    FROM Users u
    JOIN Companies c ON u.CompanyId = c.Id
END



}
